using Core.Entites;
using Core.IRepositories;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class GenaricRepo<T> : IGenaricRepo<T> where T : BaseEntity
    {
        private readonly AppDbContext dbContext;

        public GenaricRepo(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync() => await dbContext.Set<T>().ToListAsync();
        public async Task<T> GetByIdAsync(int Id) => await dbContext.Set<T>().FindAsync(Id);
        public async Task AddAsync(T Entity) => await dbContext.Set<T>().AddAsync(Entity);
        public void Delete(T Entity) => dbContext.Set<T>().Remove(Entity);
        public void Update(T Entity) => dbContext.Set<T>().Update(Entity);

        public async Task<IReadOnlyList<T>> GetAllIncldedWithSpec(ISpecification<T> Spec , bool AsNoTraking = false)
            => await SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>() , Spec , AsNoTraking).ToListAsync();

        public async Task<T> GetSpacificWithSpec(ISpecification<T> Spec, bool AsNoTraking = false)
            => await SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>(), Spec , AsNoTraking).FirstOrDefaultAsync();

        public async Task<int> GetCountWithSpec(ISpecification<T> Spec, bool AsNoTraking = false)
            => await SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>() , Spec, AsNoTraking).CountAsync();
    }
}
