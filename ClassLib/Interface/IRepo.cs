using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Interface
{
    public interface IRepo<T>
    {
        List<T> GetAll();
        T GetById(int id);
        T Add(T entity);
        T Update(T entity, int id);
        T Delete(int id);
    }
}
