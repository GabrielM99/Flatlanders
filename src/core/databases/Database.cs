using System;
using System.Collections.Generic;

namespace Flatlanders.Core;

public abstract class Database
{
    public virtual bool AutoLoad => true;
    
    public bool IsLoaded { get; private set; }

    protected abstract void OnLoad(Engine engine);

    public void Load(Engine engine)
    {
        if (!IsLoaded)
        {
            Refresh(engine);
            IsLoaded = true;
        }
    }

    public void Refresh(Engine engine)
    {
        OnLoad(engine);
    }
}

public abstract class Database<T> : Database
{
    private Dictionary<int, T> ObjectByID { get; }

    public Database()
    {
        ObjectByID = new Dictionary<int, T>();
    }

    public void Register(int id, T obj)
    {
        if (ObjectByID.ContainsKey(id))
        {
            throw new Exception($"An object with ID {id} is already added to database {obj.GetType().Name}.");
        }

        ObjectByID[id] = obj;
    }
}