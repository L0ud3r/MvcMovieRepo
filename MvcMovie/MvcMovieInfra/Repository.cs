using Microsoft.EntityFrameworkCore;
using MvcMovieDAL;
using MvcMovieDAL.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcMovieInfra
{
    public class Repository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : DefaultEntity
        where TContext : DbContext
    {
        private TContext _context { get; set; }

        public Repository(TContext context)
        {
            this._context = context;
        }

        public void Delete(int entityID)
        {
            var entity = _context.Set<TEntity>().AsQueryable().Where(x => x.Id == entityID).SingleOrDefault();

            if(entity != null)
            {
                _context.Remove(entity);
            }
        }

        public IQueryable<TEntity> Get()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public TEntity GetByID(int entityId)
        {
            return _context.Set<TEntity>().Where(x => x.Id == entityId).SingleOrDefault();
        }

        //Evitar que adicione outro Genre igual
        public void Insert(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public bool Exists(int entityId)
        {
            return _context.Set<TEntity>().Where(x => x.Id == entityId).Any();
        }
    }
}
