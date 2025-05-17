using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class Specification<T> : ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Include { get; set; } = new List<Expression<Func<T, object>>>();
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPaginationEnable { get; set; }
        public List<string> IncludeStrings { get; } = new();
        public Specification()
        {
            
        }
        public Specification(Expression<Func<T, bool>> expression)
        {
            Criteria = expression;
        }
        public void ApplyPagination(int skip , int take)
        {
            Skip = skip;
            Take = take;
            IsPaginationEnable = true;
        }
        public void AddInclude(string include) // Add this
        {
            IncludeStrings.Add(include);
        }
    }
}
