using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> LoadAll();
        T Load(int id);
        T Save(T t);
    }
}