namespace UnitTest.Web.Repository
{
    public interface IRepository<T>where T:class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetById(int id);
        Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
