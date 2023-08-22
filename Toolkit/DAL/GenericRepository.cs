using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToolKit.Entities;

namespace ToolKit.DAL
{
    public class GenericRepository<TEntity, TDto>
    where TEntity : EntityBase
    where TDto : DtoBase
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly IMapper _mapper;

        public GenericRepository(DbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public virtual FilterResult<TDto> GetFilteredWithPagination(Func<TEntity, bool> filter, int page, int pageSize, object option)
        {
            var filteredEntities = _dbSet.Where(filter).AsQueryable();

            // Call the virtual method to modify the query based on the provided option
            filteredEntities = ModifyQueryWithOption(filteredEntities, option);

            if (pageSize == -1)
            {
                return new FilterResult<TDto>()
                {
                    Items = _mapper.ProjectTo<TDto>(filteredEntities),
                    PageSize = pageSize,
                    PageNumber = page,
                    Total = filteredEntities.Count()
                };
            }

            return new FilterResult<TDto>()
            {
                Items = _mapper.ProjectTo<TDto>(filteredEntities.Skip((page - 1) * pageSize).Take(pageSize)),
                PageSize = pageSize,
                PageNumber = page,
                Total = filteredEntities.Count()
            };
        }

        // Virtual method to modify the query based on the provided option
        protected virtual IQueryable<TEntity> ModifyQueryWithOption(IQueryable<TEntity> query, object option)
        {
            // Default implementation returns the original query without modification
            return query;
        }

        public virtual TDto GetById(Guid id)
        {
            var entity = _dbSet.First(e => e.Id == id);
            return _mapper.Map<TDto>(entity);
        }

        public virtual void UpdateOrCreate(IEnumerable<TDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var entity = _mapper.Map<TEntity>(dto);

                if (entity.Id == Guid.Empty)
                {
                    entity.Id = Guid.NewGuid();
                    _dbSet.Add(entity);
                }
                else
                {
                    _dbSet.Attach(entity);
                    _context.Entry(entity).State = EntityState.Modified;
                }
            }

            _context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var entityToDelete = _dbSet.Find(id);
            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
                _context.SaveChanges();
            }
        }
    }
}
