using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flatlanders.Core;

public class DatabaseManager : GameComponent
{
    private Engine Engine { get; }
    private Dictionary<Type, Database> DatabaseByType { get; }

    public DatabaseManager(Engine engine) : base(engine)
    {
        Engine = engine;
        DatabaseByType = new Dictionary<Type, Database>();
    }

    public override void Initialize()
    {
        base.Initialize();

        foreach (Database database in DatabaseByType.Values)
        {
            if (database.AutoLoad)
            {
                database.Load(Engine);
            }
        }
    }

    public T AddDatabase<T>() where T : Database
    {
        Type type = typeof(T);
        T database = (T)Activator.CreateInstance(type);
        DatabaseByType[type] = database;
        return database;
    }

    public T GetDatabase<T>() where T : Database
    {
        Type type = typeof(T);

        if (DatabaseByType.TryGetValue(type, out Database database))
        {
            return (T)database;
        }

        throw new Exception($"Database of type {type} doesn't exist.");
    }
}