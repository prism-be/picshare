// -----------------------------------------------------------------------
//  <copyright file="IDatabase.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Linq.Expressions;

namespace Prism.Picshare.Data;

public interface IDatabase: IDisposable
{
    int Count<T>();

    int Count<T>(Expression<Func<T, bool>> predicate);

    bool Delete<T>(Guid id);

    int DeleteMany<T>(Expression<Func<T, bool>> predicate);

    bool Exists<T>(Expression<Func<T, bool>> predicate);

    IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate, int skip = 0, int limit = 2147483647);

    IEnumerable<T> FindAll<T>();

    T FindById<T>(Guid id);

    T FindOne<T>(Expression<Func<T, bool>> predicate);

    int InsertMany<T>(IEnumerable<T> entities);

    Guid Insert<T>(T entity);

    int InsertBulk<T>(IEnumerable<T> entities, int batchSize = 5000);

    K Max<T, K>(Expression<Func<T, K>> keySelector);

    K Min<T, K>(Expression<Func<T, K>> keySelector);
    int UpdateMany<T>(IEnumerable<T> entities);

    bool Update<T>(T entity);

    int UpsertMany<T>(IEnumerable<T> entities);

    bool Upsert<T>(T entity);
}