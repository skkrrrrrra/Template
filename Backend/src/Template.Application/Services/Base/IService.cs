namespace Template.Application.Services.Base
{
    public interface IService<TEntity>
    {
        public Task<TEntity> GetByIdAsync(long id);
        public Task<TEntity> UpdateAsync(TEntity entity);
        public Task<TEntity> AddAsync(TEntity entity);
        public Task<bool> DeleteByIdAsync(long id);
    }
}
