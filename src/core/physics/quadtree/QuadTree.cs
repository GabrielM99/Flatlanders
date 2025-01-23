using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Flatlanders.Core.Physics
{
    public class QuadTree
    {
        public const int DefaultMaxDepth = 7;

        public const int DefaultMaxObjectsPerNode = 25;

        public RectangleF NodeBounds { get; protected set; }
        public bool IsLeaf => Children.Count == 0;

        protected List<QuadTree> Children { get; }
        protected HashSet<QuadtreeData> Contents { get; }
        
        protected int CurrentDepth { get; set; }
        protected int MaxDepth { get; set; } = DefaultMaxDepth;
        protected int MaxObjectsPerNode { get; set; } = DefaultMaxObjectsPerNode;

        public QuadTree(RectangleF bounds)
        {
            Children = [];
            Contents = [];
            CurrentDepth = 0;
            NodeBounds = bounds;
        }
        public int NumTargets()
        {
            List<QuadtreeData> dirtyItems = new List<QuadtreeData>();
            var objectCount = 0;

            // Do BFS on nodes to count children.
            var process = new Queue<QuadTree>();
            process.Enqueue(this);
            while (process.Count > 0)
            {
                var processing = process.Dequeue();
                if (!processing.IsLeaf)
                {
                    foreach (var child in processing.Children)
                    {
                        process.Enqueue(child);
                    }
                }
                else
                {
                    foreach (var data in processing.Contents)
                    {
                        if (data.Dirty == false)
                        {
                            objectCount++;
                            data.MarkDirty();
                            dirtyItems.Add(data);
                        }
                    }
                }
            }
            foreach (var quadtreeData in dirtyItems)
            {
                quadtreeData.MarkClean();
            }
            return objectCount;
        }

        public void Insert(QuadtreeData data)
        {
            var actorBounds = data.Bounds;

            // Object doesn't fit into this node.
            if (!NodeBounds.Intersects(actorBounds))
            {
                return;
            }

            if (IsLeaf && Contents.Count >= MaxObjectsPerNode)
            {
                Split();
            }

            if (IsLeaf)
            {
                AddToLeaf(data);
            }
            else
            {
                foreach (var child in Children)
                {
                    child.Insert(data);
                }
            }
        }

        public void Remove(QuadtreeData data)
        {
            if (IsLeaf)
            {
                data.RemoveParent(this);
                Contents.Remove(data);
            }
            else
            {
                throw new InvalidOperationException($"Cannot remove from a non leaf {nameof(QuadTree)}");
            }
        }

        public void Shake()
        {
            if (IsLeaf)
            {
                return;
            }

            List<QuadtreeData> dirtyItems = new List<QuadtreeData>();

            var numObjects = NumTargets();
            if (numObjects == 0)
            {
                Children.Clear();
            }
            else if (numObjects < MaxObjectsPerNode)
            {
                var process = new Queue<QuadTree>();
                process.Enqueue(this);
                while (process.Count > 0)
                {
                    var processing = process.Dequeue();
                    if (!processing.IsLeaf)
                    {
                        foreach (var subTree in processing.Children)
                        {
                            process.Enqueue(subTree);
                        }
                    }
                    else
                    {
                        foreach (var data in processing.Contents)
                        {
                            if (data.Dirty == false)
                            {
                                AddToLeaf(data);
                                data.MarkDirty();
                                dirtyItems.Add(data);
                            }
                        }
                    }
                }
                Children.Clear();
            }

            foreach (var quadtreeData in dirtyItems)
            {
                quadtreeData.MarkClean();
            }
        }

        private void AddToLeaf(QuadtreeData data)
        {
            data.AddParent(this);
            Contents.Add(data);
        }

        public void Split()
        {
            if (CurrentDepth + 1 >= MaxDepth) return;

            var min = NodeBounds.TopLeft;
            var max = NodeBounds.BottomRight;
            var center = NodeBounds.Center;

            RectangleF[] childAreas =
            {
                RectangleF.CreateFrom(min, center),
                RectangleF.CreateFrom(new Vector2(center.X, min.Y), new Vector2(max.X, center.Y)),
                RectangleF.CreateFrom(center, max),
                RectangleF.CreateFrom(new Vector2(min.X, center.Y), new Vector2(center.X, max.Y))
            };

            for (var i = 0; i < childAreas.Length; ++i)
            {
                var node = new QuadTree(childAreas[i]);
                Children.Add(node);
                Children[i].CurrentDepth = CurrentDepth + 1;
            }

            foreach (QuadtreeData contentQuadtree in Contents)
            {
                foreach (QuadTree childQuadtree in Children)
                {
                    childQuadtree.Insert(contentQuadtree);
                }
            }
            Clear();
        }

        public void ClearAll()
        {
            foreach (QuadTree childQuadtree in Children)
                childQuadtree.ClearAll();
            Clear();
        }

        private void Clear()
        {
            foreach (QuadtreeData quadtreeData in Contents)
            {
                quadtreeData.RemoveParent(this);
            }
            Contents.Clear();
        }

        public List<QuadtreeData> Query(ref RectangleF area)
        {
            var recursiveResult = new List<QuadtreeData>();
            QueryWithoutReset(ref area, recursiveResult);
            foreach (var quadtreeData in recursiveResult)
            {
                quadtreeData.MarkClean();
            }
            return recursiveResult;
        }

        private void QueryWithoutReset(ref RectangleF area, List<QuadtreeData> recursiveResult)
        {
            if (!NodeBounds.Intersects(area))
                return;

            if (IsLeaf)
            {
                foreach (QuadtreeData quadtreeData in Contents)
                {
                    if (quadtreeData.Dirty == false && quadtreeData.Bounds.Intersects(area))
                    {
                        recursiveResult.Add(quadtreeData);
                        quadtreeData.MarkDirty();
                    }
                }
            }
            else
            {
                for (int i = 0, size = Children.Count; i < size; i++)
                {
                    Children[i].QueryWithoutReset(ref area, recursiveResult);
                }
            }
        }
    }
}
