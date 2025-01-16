using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Flatlanders.Core.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;

namespace Flatlanders.Core
{
    public class Physics : GameComponent
    {
        public const string DEFAULT_LAYER_NAME = "default";

        private Dictionary<string, Layer> _layers = new();

        public IReadOnlyDictionary<string, Layer> Layers => _layers;

        private HashSet<(Layer, Layer)> _layerCollision = new();

        private Engine Engine { get; }

        public Physics(Engine engine, RectangleF boundary) : base(engine)
        {
            Engine = engine;
            SetDefaultLayer(new Layer(new QuadTreeSpace(boundary)));
        }

        public void SetDefaultLayer(Layer layer)
        {
            if (_layers.ContainsKey(DEFAULT_LAYER_NAME))
                RemoveLayer(DEFAULT_LAYER_NAME);
            AddLayer(DEFAULT_LAYER_NAME, layer);
            foreach (var otherLayer in _layers.Values)
                AddCollisionBetweenLayer(layer, otherLayer);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var layer in _layers.Values)
            {
                layer.Reset();
            }

            foreach (var (firstLayer, secondLayer) in _layerCollision)
            {
                foreach (ICollisionActor actor in firstLayer.Space)
                {
                    Engine.Graphics.Draw(new Transform() { Position = actor.Bounds.Position, Size = actor.Bounds.BoundingRectangle.Size }, new RectangleDrawer(1f), Color.Red, short.MaxValue);
                }
            }
        }

        public int CastCollider(IShapeF shape, in CollisionEventArgs[] collisionInfo, string layerName = DEFAULT_LAYER_NAME, ICollisionActor exclude = null)
        {
            if (collisionInfo == null)
            {
                return 0;
            }

            int collisionCount = 0;

            if (_layers.TryGetValue(layerName, out Layer layer))
            {
                IEnumerable<ICollisionActor> collisions = layer.Space.Query(shape.BoundingRectangle);

                foreach (ICollisionActor other in collisions)
                {
                    if (other != exclude && shape.Intersects(other.Bounds))
                    {
                        Vector2 penetrationVector = CalculatePenetrationVector(shape, other.Bounds);

                        if (collisionCount < collisionInfo.Length)
                        {
                            collisionInfo[collisionCount] = new CollisionEventArgs
                            {
                                Other = other,
                                PenetrationVector = penetrationVector
                            };
                        }

                        collisionCount++;
                    }
                }
            }

            return collisionCount;
        }

        public void AddCollider(ICollisionActor target)
        {
            var layerName = target.LayerName ?? DEFAULT_LAYER_NAME;

            if (!_layers.TryGetValue(layerName, out var layer))
            {
                throw new UndefinedLayerException(layerName);
            }

            layer.Space.Insert(target);
        }

        public void RemoveCollider(ICollisionActor target)
        {
            if (target.LayerName is not null)
            {
                _layers[target.LayerName].Space.Remove(target);
            }
            else
            {
                foreach (var layer in _layers.Values)
                {
                    if (layer.Space.Remove(target))
                    {
                        return;
                    }
                }
            }
        }

        #region Layers

        public void AddLayer(string name, Layer layer)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (!_layers.TryAdd(name, layer))
                throw new DuplicateNameException(name);

            if (name != DEFAULT_LAYER_NAME)
                AddCollisionBetweenLayer(_layers[DEFAULT_LAYER_NAME], layer);
        }

        public void RemoveLayer(string name = null, Layer layer = null)
        {
            name ??= _layers.First(x => x.Value == layer).Key;
            _layers.Remove(name, out layer);
            _layerCollision.RemoveWhere(tuple => tuple.Item1 == layer || tuple.Item2 == layer);
        }

        public void AddCollisionBetweenLayer(Layer a, Layer b)
        {
            _layerCollision.Add((a, b));
        }

        public void AddCollisionBetweenLayer(string nameA, string nameB)
        {
            _layerCollision.Add((_layers[nameA], _layers[nameB]));
        }

        #endregion

        #region Penetration Vectors

        private static Vector2 CalculatePenetrationVector(IShapeF a, IShapeF b)
        {
            return a switch
            {
                CircleF circleA when b is CircleF circleB => PenetrationVector(circleA, circleB),
                CircleF circleA when b is RectangleF rectangleB => PenetrationVector(circleA, rectangleB),
                CircleF circleA when b is OrientedRectangle orientedRectangleB => PenetrationVector(circleA, orientedRectangleB),

                RectangleF rectangleA when b is CircleF circleB => PenetrationVector(rectangleA, circleB),
                RectangleF rectangleA when b is RectangleF rectangleB => PenetrationVector(rectangleA, rectangleB),
                RectangleF rectangleA when b is OrientedRectangle orientedRectangleB => PenetrationVector(rectangleA, orientedRectangleB),

                OrientedRectangle orientedRectangleA when b is CircleF circleB => PenetrationVector(orientedRectangleA, circleB),
                OrientedRectangle orientedRectangleA when b is RectangleF rectangleB => PenetrationVector(orientedRectangleA, rectangleB),
                OrientedRectangle orientedRectangleA when b is OrientedRectangle orientedRectangleB => PenetrationVector(orientedRectangleA, orientedRectangleB),

                _ => throw new ArgumentOutOfRangeException(nameof(a))
            };
        }

        private static Vector2 PenetrationVector(CircleF circ1, CircleF circ2)
        {
            if (!circ1.Intersects(circ2))
            {
                return Vector2.Zero;
            }

            var displacement = circ1.Center - circ2.Center;

            Vector2 desiredDisplacement;
            if (displacement != Vector2.Zero)
            {
                desiredDisplacement = displacement.NormalizedCopy() * (circ1.Radius + circ2.Radius);
            }
            else
            {
                desiredDisplacement = -Vector2.UnitY * (circ1.Radius + circ2.Radius);
            }


            var penetration = displacement - desiredDisplacement;
            return penetration;
        }

        private static Vector2 PenetrationVector(CircleF circ, RectangleF rect)
        {
            var collisionPoint = rect.ClosestPointTo(circ.Center);
            var cToCollPoint = collisionPoint - circ.Center;

            if (rect.Contains(circ.Center) || cToCollPoint.Equals(Vector2.Zero))
            {
                var displacement = circ.Center - rect.Center;

                Vector2 desiredDisplacement;
                if (displacement != Vector2.Zero)
                {
                    // Calculate penetration as only in X or Y direction.
                    // Whichever is lower.
                    var dispx = new Vector2(displacement.X, 0);
                    var dispy = new Vector2(0, displacement.Y);
                    dispx.Normalize();
                    dispy.Normalize();

                    dispx *= (circ.Radius + rect.Width / 2);
                    dispy *= (circ.Radius + rect.Height / 2);

                    if (dispx.LengthSquared() < dispy.LengthSquared())
                    {
                        desiredDisplacement = dispx;
                        displacement.Y = 0;
                    }
                    else
                    {
                        desiredDisplacement = dispy;
                        displacement.X = 0;
                    }
                }
                else
                {
                    desiredDisplacement = -Vector2.UnitY * (circ.Radius + rect.Height / 2);
                }

                var penetration = displacement - desiredDisplacement;
                return penetration;
            }
            else
            {
                var penetration = circ.Radius * cToCollPoint.NormalizedCopy() - cToCollPoint;
                return penetration;
            }
        }

        private static Vector2 PenetrationVector(CircleF circleA, OrientedRectangle orientedRectangleB)
        {
            var rotation = Matrix3x2.CreateRotationZ(orientedRectangleB.Orientation.Rotation);
            var circleCenterInRectangleSpace = rotation.Transform(circleA.Center - orientedRectangleB.Center);
            var circleInRectangleSpace = new CircleF(circleCenterInRectangleSpace, circleA.Radius);
            var boundingRectangle = new BoundingRectangle(new Vector2(), orientedRectangleB.Radii);

            var penetrationVector = PenetrationVector(circleInRectangleSpace, boundingRectangle);
            var inverseRotation = Matrix3x2.CreateRotationZ(-orientedRectangleB.Orientation.Rotation);
            var transformedPenetration = inverseRotation.Transform(penetrationVector);

            return transformedPenetration;
        }

        private static Vector2 PenetrationVector(RectangleF rect, CircleF circ)
        {
            return -PenetrationVector(circ, rect);
        }

        private static Vector2 PenetrationVector(RectangleF rect1, RectangleF rect2)
        {
            var intersectingRectangle = RectangleF.Intersection(rect1, rect2);
            Debug.Assert(!intersectingRectangle.IsEmpty,
                "Violation of: !intersect.IsEmpty; Rectangles must intersect to calculate a penetration vector.");

            Vector2 penetration;
            if (intersectingRectangle.Width < intersectingRectangle.Height)
            {
                var d = rect1.Center.X < rect2.Center.X
                    ? intersectingRectangle.Width
                    : -intersectingRectangle.Width;
                penetration = new Vector2(d, 0);
            }
            else
            {
                var d = rect1.Center.Y < rect2.Center.Y
                    ? intersectingRectangle.Height
                    : -intersectingRectangle.Height;
                penetration = new Vector2(0, d);
            }

            return penetration;
        }

        private static Vector2 PenetrationVector(RectangleF rectangleA, OrientedRectangle orientedRectangleB)
        {
            return PenetrationVector((OrientedRectangle)rectangleA, orientedRectangleB);
        }

        private static Vector2 PenetrationVector(OrientedRectangle orientedRectangleA, CircleF circleB)
        {
            return -PenetrationVector(circleB, orientedRectangleA);
        }

        private static Vector2 PenetrationVector(OrientedRectangle orientedRectangleA, RectangleF rectangleB)
        {
            return -PenetrationVector(rectangleB, orientedRectangleA);
        }

        private static Vector2 PenetrationVector(OrientedRectangle orientedRectangleA, OrientedRectangle orientedRectangleB)
        {
            return OrientedRectangle.Intersects(orientedRectangleA, orientedRectangleB)
                .MinimumTranslationVector;
        }

        #endregion
    }
}
