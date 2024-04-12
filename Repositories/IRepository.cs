namespace Repositories;
using System.Collections.Generic;

public interface IRepository<T>
{
    T Get(string id);
    IEnumerable<T> GetAll();
    long Insert(T obj);
    long Insert(IEnumerable<T> list);
    bool Update(T obj);
    bool Update(IEnumerable<T> list);
    bool Delete(T obj);
    bool Delete(IEnumerable<T> list);
}