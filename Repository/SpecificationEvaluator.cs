using Core.Entites;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public static class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> InitialQuery, ISpecification<T> Spec , bool AsNoTraking = false)
        {
            var query = InitialQuery;
            if (Spec.Criteria is not null)
                query = query.Where(Spec.Criteria);

            if(Spec.IsPaginationEnable)
                query = query.Skip(Spec.Skip).Take(Spec.Take);

            query = Spec.Include.Aggregate(query, (input, output) => input.Include(output));
            query = Spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            if (AsNoTraking)
                query = query.AsNoTracking();
            
            return query;
        }
    }
}
