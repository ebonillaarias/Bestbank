using inConcert.iMS.DataAccess.Data;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace inConcert.iMS.DataAccess.Repositories
{
   public class GenericRepository<T> : IGenericRepository<T> where T : class
   {
      private readonly InConcertDbContext _context;
      private readonly DbSet<T> _table;

      public GenericRepository(InConcertDbContext context)
      {
         _context = context;
         _table = _context.Set<T>();
      }

      public IEnumerable<T> GetAll()
      {
         return _table.ToList();
      }

      public DbContext GetContext()
      {
         return _context;
      }

      public DbSet<T> GetTable()
      {
         return _table;
      }

      public T GetById(params object[] keyValues)
      {
         //return _context.Set<T>().Find(keyValues);
         return _table.Find(keyValues);
      }

      public IEnumerable<T> FindAll(Func<T, bool> exp)
      {
         return _table.Where<T>(exp);
      }
      public T Single(Func<T, bool> exp)
      {
         var singleRow = _table.Single(exp);
         return singleRow;
      }
      public T SingleOrDefault(Func<T, bool> exp)
      {
         var singleRow = _table.SingleOrDefault(exp);
         return singleRow;
      }
      public void Insert(T obj)
      {
         _table.Add(obj);
      }
      public void InsertRange(IEnumerable<T> itemsToAdd)
      {
         _table.AddRange(itemsToAdd);
      }
      public void Update(T obj)
      {
         _table.Attach(obj);
         _context.Entry(obj).State = EntityState.Modified;
      }
      
      // PRECONDICION:
      // El objeto indicado por el parametro 'obj' existe en la BD.
      public void Delete(T obj)
      {
         _table.Remove(obj);
      }
      public void DeleteRange(IEnumerable<T> itemsToDelete)
      {
         _table.RemoveRange(itemsToDelete);
      }
      public void Save()
      {
         _context.SaveChanges();
      }
   }
}
