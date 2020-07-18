using inConcert.iMS.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace inConcert.iMS.DataAccess.Repositories.Interfaces
{
   public interface IGenericRepository<T> where T : class
   {
      DbContext GetContext();
      DbSet<T> GetTable();
      IEnumerable<T> GetAll();
      T GetById(params object[] keyValues);
      void Insert(T obj);
      void InsertRange(IEnumerable<T> itemsToAdd);
      void Update(T obj);
      void Delete(T obj);
      void DeleteRange(IEnumerable<T> itemsToDelete);
      void Save();
      IEnumerable<T> FindAll(Func<T, bool> exp);
      T Single(Func<T, bool> exp);
      T SingleOrDefault(Func<T, bool> exp);
   }
}
