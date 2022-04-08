// -----------------------------------------------------------------------
//  <copyright file="Database.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Linq.Expressions;
using LiteDB;

namespace Prism.Picshare.Data.LiteDB;

public class Database : IDatabase
{
    private readonly ILiteDatabase _liteDatabase;

    public Database(ILiteDatabase liteDatabase)
    {
        _liteDatabase = liteDatabase;
    }

    public int Count<T>()
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Count();
    }

    public int Count<T>(Expression<Func<T, bool>> predicate)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Count(predicate);
    }

    public bool Delete<T>(Guid id)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Delete(id);
    }

    public int DeleteMany<T>(Expression<Func<T, bool>> predicate)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.DeleteMany(predicate);
    }

    public bool Exists<T>(Expression<Func<T, bool>> predicate)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Exists(predicate);
    }

    public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate, int skip = 0, int limit = 2147483647)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Find(predicate, skip, limit);
    }

    public IEnumerable<T> FindAll<T>()
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.FindAll();
    }

    public T FindById<T>(Guid id)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.FindById(id);
    }

    public T FindOne<T>(Expression<Func<T, bool>> predicate)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.FindOne(predicate);
    }

    public int InsertMany<T>(IEnumerable<T> entities)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Insert(entities);
    }

    public Guid Insert<T>(T entity)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Insert(entity);
    }

    public int InsertBulk<T>(IEnumerable<T> entities, int batchSize = 5000)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.InsertBulk(entities, batchSize);
    }

    public K Max<T, K>(Expression<Func<T, K>> keySelector)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Max(keySelector);
    }

    public K Min<T, K>(Expression<Func<T, K>> keySelector)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Min(keySelector);
    }

    public int UpdateMany<T>(IEnumerable<T> entities)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Update(entities);
    }

    public bool Update<T>(T entity)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Update(entity);
    }

    public int UpsertMany<T>(IEnumerable<T> entities)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Upsert(entities);
    }

    public bool Upsert<T>(T entity)
    {
        var collection = _liteDatabase.GetCollection<T>();
        return collection.Upsert(entity);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _liteDatabase.Dispose();
        }
    }
}