namespace MvcMovieDAL
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Get();
        TEntity GetByID(int entityId);
        void Insert(TEntity entity);
        void Delete(int entityID);
        void Update(TEntity entity);
        void Save();
        bool Exists(int entityId);
    }
}