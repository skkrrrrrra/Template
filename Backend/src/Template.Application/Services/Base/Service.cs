using Microsoft.EntityFrameworkCore;
using DamnSmallMapper;
using Domain.Entities.Base;
using Template.Application.Common.Helpers;
using Template.Persistence.Context;

namespace Template.Application.Services.Base
{
    public class Service<TEntity>
        where TEntity : BaseEntity<long>
    {
        private readonly MainDbContext _dbContext;

        public Service(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<TEntity> GetByIdAsync(long id)
        {
            var dbSet = _dbContext.Set<TEntity>();
            var entity = dbSet.FirstOrDefaultAsync(entity => entity.Id == id);
            return entity;
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            var dbSet = _dbContext.Set<TEntity>();
            var entity = await GetByIdAsync(id);
            entity.DeletedAt = DateHelper.GetCurrentDateTime();
            entity.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            try
            {
                entity.CreatedAt = DateHelper.GetCurrentDateTime();
                entity.UpdatedAt = DateHelper.GetCurrentDateTime();
                await _dbContext.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var result = await GetByIdAsync(entity.Id);
            result = Mapper.Map<TEntity>(entity);
            result.UpdatedAt = DateHelper.GetCurrentDateTime();
            _dbContext.Update(result);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
