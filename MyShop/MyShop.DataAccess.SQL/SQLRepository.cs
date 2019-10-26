using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.SQL
{
    public class SQLRepository<T> : IRepository<T> where T : BaseEntity
    {
        internal DataContex contex;
        internal DbSet<T> dbSet;


        public SQLRepository(DataContex contex)
        {
            this.contex = contex;
            this.dbSet = contex.Set<T>();
        }
        public IQueryable<T> Collection()
        {
            return dbSet;
        }

        public void Commit()
        {
            contex.SaveChanges();
        }

        public void Delete(string Id)
        {
            var t = Find(Id);
            if(contex.Entry(t).State==EntityState.Detached)
            {
                dbSet.Attach(t);
                dbSet.Remove(t);
            }
        }

        public T Find(string Id)
        {
            return dbSet.Find(Id);
        }

        public void Insert(T t)
        {
            dbSet.Add(t);
        }

        public void Update(T t)
        {
            dbSet.Attach(t);
            contex.Entry(t).State = EntityState.Modified;
        }
    }
}
