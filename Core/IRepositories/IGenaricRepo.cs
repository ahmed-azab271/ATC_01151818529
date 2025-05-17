using Core.Entites;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IRepositories
{
    public interface IGenaricRepo<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int Id);
        Task AddAsync(T Entity);
        void Update(T Entity);
        void Delete(T Entity);

        Task<IReadOnlyList<T>> GetAllIncldedWithSpec(ISpecification<T> Spec, bool AsNoTraking = false);
        Task<T> GetSpacificWithSpec(ISpecification<T> Spec, bool AsNoTraking = false);
        Task<int>  GetCountWithSpec(ISpecification<T> Spec , bool AsNoTraking = false);
    }
}
