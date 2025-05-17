using Core.Entites;
using Core.IRepositories;
using Repository.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;
        private Hashtable repo;
        public UnitOfWork(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
            repo = new Hashtable();
        }
        public IGenaricRepo<T> Entity<T>() where T : BaseEntity
        {
            var type = typeof(T).Name;
            if(!repo.ContainsKey(type))
            {
                var Response = new GenaricRepo<T>(dbContext);
                repo.Add(type, Response);
            }
            return repo[type] as IGenaricRepo<T>;
        }
        public async Task<int> SaveAsync() => await dbContext.SaveChangesAsync();
        public async ValueTask DisposeAsync()=> await dbContext.DisposeAsync();
    }
}
