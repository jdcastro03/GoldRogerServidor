﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Data.Repositories.Core
{
    public class GenericRepository<TEntity> where TEntity : class
    {

        internal GoldRogerContext context;
        internal DbSet<TEntity> dbSet;
        public GenericRepository(GoldRogerContext dbContext)
        {
            this.context = dbContext;
            this.dbSet = context.Set<TEntity>();
        }
        public virtual IQueryable<TEntity> Get(
             Expression<Func<TEntity, bool>>? filter = null,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
             string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id)!;
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity? entityToDelete = dbSet.Find(id);
            if (entityToDelete != null)
                Delete(entityToDelete!);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);  // Esto agrega todos los elementos al DbSet
        }
    }
}
