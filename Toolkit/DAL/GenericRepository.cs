using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToolKit.Entities;

namespace ToolKit.DAL
{
    public class GenericRepository<TEntity>
        where TEntity : EntityBase
    {
#pragma warning disable CA1051 // Ne pas déclarer de champs d'instances visibles
#pragma warning disable SA1401 // Fields should be private
        protected DbContext _context;
        protected DbSet<TEntity> _dbSet;
        protected IMapper _mapper;
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CA1051 // Ne pas déclarer de champs d'instances visibles

        public GenericRepository(DbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public virtual async Task<IEnumerable<TDo>> GetAllAsync<TDo>()
        {
            return _mapper.Map<IEnumerable<TDo>>(await _dbSet.ToListAsync());
        }

        public virtual async Task<FilterResult<TDo>> GetFilteredWithPaginationAsync<TDo>(int page, int pageSize, object option)
        {
            var filteredEntities = _dbSet.AsQueryable();

            // Call the virtual method to modify the query based on the provided option
            filteredEntities = ModifyQueryWithOption(filteredEntities, option);

            if (pageSize == -1)
            {
                return new FilterResult<TDo>()
                {
                    Items = _mapper.ProjectTo<TDo>(filteredEntities),
                    PageSize = pageSize,
                    PageNumber = page,
                    Total = await filteredEntities.CountAsync()
                };
            }

            var lists = filteredEntities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new FilterResult<TDo>()
            {
                Items = lists.Select(_mapper.Map<TDo>),
                PageSize = pageSize,
                PageNumber = page,
                Total = await filteredEntities.CountAsync()
            };
        }

        // Virtual method to modify the query based on the provided option
        protected virtual IQueryable<TEntity> ModifyQueryWithOption(IQueryable<TEntity> query, object option)
        {
            // Default implementation returns the original query without modification
            return query;
        }

        public virtual async Task<TDo> GetByIdAsync<TDo>(Guid id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
            return _mapper.Map<TDo>(entity);
        }

        public virtual async Task UpdateOrCreateAsync(IEnumerable<TEntity> dtos)
        {
            foreach (var dto in dtos)
            {
                var entity = _mapper.Map<TEntity>(dto);

                if (entity.Id == Guid.Empty)
                {
                    entity.Id = Guid.NewGuid();
                    await _dbSet.AddAsync(entity);
                }
                else
                {
                    _dbSet.Attach(entity);
                    _context.Entry(entity).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
