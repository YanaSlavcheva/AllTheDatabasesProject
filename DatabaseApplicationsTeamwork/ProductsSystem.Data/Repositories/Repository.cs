﻿namespace ProductsSystem.Data.Repositories
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Linq.Expressions;
    using ProductsSystem.Data.Contexts;

    public class Repository<T> : IRepository<T>, IEnumerable
        where T : class
    {
        public Repository(IProductsSystemDbContext context)
        {
            this.Context = context;
            this.DbSet = this.Context.Set<T>();
        } 

        protected IProductsSystemDbContext Context { get; set; }

        protected IDbSet<T> DbSet { get; set; }
 
        public IQueryable<T> All()
        {
            return this.DbSet.AsQueryable();
        }

        public IQueryable<T> Find(Expression<System.Func<T, bool>> expression)
        {
            return this.DbSet.Where(expression).AsQueryable();
        }

        public T GetById(object id)
        {
            return this.DbSet.Find(id);
        }

        public T Add(T entity)
        {
            this.ChangeState(entity, EntityState.Added);
            return entity;
        }

        public void AddRange<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            foreach (TEntity entity in entities)
            {
                this.Context.Set<TEntity>().Add(entity);
            }
        }

        public T Update(T entity)
        {
            this.ChangeState(entity, EntityState.Modified);
            return entity;
        }

        public void Delete(T entity)
        {
            this.ChangeState(entity, EntityState.Deleted);
        }

        public void Detach(T entity)
        {
            var entry = this.Context.Entry(entity);
            entry.State = EntityState.Detached;
        }

        public void SaveChanges()
        {
            this.Context.SaveChanges();
        }

        private void ChangeState(T entity, EntityState state)
        {
            var entry = this.AttachIfDetached(entity);
            entry.State = state;
        }

        private DbEntityEntry AttachIfDetached(T entity)
        {
            var entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            return entry;
        }

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
