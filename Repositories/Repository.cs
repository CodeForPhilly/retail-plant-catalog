namespace Repositories;
using System.Collections.Generic;
using System.Data;
using Dapper.Contrib.Extensions;
using Shared;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly IDbConnection conn;

    public Repository(IDbConnection conn)
    {
        this.conn = conn;
    }

    public bool Delete(T obj)
    {
        return conn.Delete(obj);
    }

    public bool Delete(IEnumerable<T> list)
    {
        return conn.Delete(list);
    }

    public virtual T Get(string id)
    {
        return conn.Get<T>(id);
    }

    public IEnumerable<T> GetAll()
    {
        return conn.GetAll<T>();
    }

    public virtual long Insert(T obj)
    {
            return conn.Insert<T>(obj);
    }

    public virtual long Insert(IEnumerable<T> list)
    {
        return conn.Insert(list);
    }

    public virtual bool Update(T obj)
    {
        return conn.Update(obj);
    }

    public virtual bool Update(IEnumerable<T> list)
    {
        return conn.Update(list);
    }
}