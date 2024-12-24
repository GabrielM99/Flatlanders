namespace Flatlanders.Core.Prefabs;

public abstract class Prefab
{
    public abstract void OnCreate(Entity entity);

    public Entity Create(EntityManager entityManager, string name = "")
    {
        Entity entity = entityManager.CreateEntity(name);
        OnCreate(entity);
        return entity;
    }
}