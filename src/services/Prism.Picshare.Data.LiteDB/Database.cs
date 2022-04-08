// -----------------------------------------------------------------------
//  <copyright file="Database.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Linq.Expressions;
using LiteDB;

namespace Prism.Picshare.Data.LiteDB;

public class Database: IDatabase, IDisposable
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
        throw new NotImplementedException();
    }

    public int DeleteMany<T>(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public bool Exists<T>(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate, int skip = 0, int limit = 2147483647)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> FindAll<T>()
    {
        throw new NotImplementedException();
    }

    public T FindById<T>(Guid id)
    {
        throw new NotImplementedException();
    }

    public T FindOne<T>(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public int Insert<T>(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

    public bool Insert<T>(T entity)
    {
        throw new NotImplementedException();
    }

    public int InsertBulk<T>(IEnumerable<T> entities, int batchSize = 5000)
    {
        throw new NotImplementedException();
    }

    public K Max<T, K>(Expression<Func<T, K>> keySelector)
    {
        throw new NotImplementedException();
    }

    public K Min<T, K>(Expression<Func<T, K>> keySelector)
    {
        throw new NotImplementedException();
    }

    public int Update<T>(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

    public bool Update<T>(T entity)
    {
        throw new NotImplementedException();
    }

    public int Upsert<T>(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

    public bool Upsert<T>(T entity)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _liteDatabase.Dispose();
    }
}