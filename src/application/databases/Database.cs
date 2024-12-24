using System;
using System.Collections.Generic;

namespace Flatlanders.Application.Databases;

public abstract class Database<T>
{
    private Dictionary<int, T> ObjectByID { get; }

    public Database()
    {
        ObjectByID = new Dictionary<int, T>();
    }

    public abstract void Load();

    public void Register(int id, T obj)
    {
        if (ObjectByID.ContainsKey(id))
        {
            throw new Exception($"An object with ID {id} is already added to database {obj.GetType().Name}.");
        }

        ObjectByID[id] = obj;
    }
}