using Service.Product.Shared.Utility;
using System.Linq.Dynamic.Core; // Dynamic LINQ
using System.Reflection;

namespace Service.Product.Work.Utility
{
    public class OperationHelper<TEntity> : BaseOperationHelper<TEntity>
    {
        public override IQueryable<TEntity> FilterSingle(IQueryable<TEntity> entity, object objectParameter, string parameterKey = "FilterBy")
        {
            string? filterBy = GetObjectValue(objectParameter, parameterKey, MemberTypes.Property) as string;
            if (!entity.Any() || string.IsNullOrWhiteSpace(filterBy))
            {
                return entity;
            }

            var result = PredicateBuilder.True<TEntity>();
            string[] filterCriteriaList = filterBy.Split(",");

            string filterQuery = "Category.CategoryName.ToLower().Contains(@0) || ProductName.ToLower().Contains(@0)";
            return entity.Where(filterQuery, filterCriteriaList);
        }

        public override IQueryable<TEntity> FilterRange(IQueryable<TEntity> entity, object objectParameter, string parameterKey = "RangeFilterBy")
        {
            if (!entity.Any())
            {
                return entity;
            }

            Dictionary<string, Tuple<string?, string?>>? filterByRangeList = GetProductRangeFilterDetail(objectParameter) as Dictionary<string, Tuple<string?, string?>>;

            if (filterByRangeList is null || filterByRangeList.Count < 1)
            {
                return entity;
            }

            var filterQuery = ExpressionPredicateBuilder.BuildRange<TEntity>(filterByRangeList);

            return entity.Where(filterQuery);
        }

        /// <summary>
        /// Get range filter property key and details.<string>
        /// </summary>
        /// <param name="content">Object</param>
        /// <param name="propertyName">Object property name [Default=RangeFilterBy]</param>
        /// <returns>Object</returns>
        private object? GetProductRangeFilterDetail(object content, string propertyName = "RangeFilterBy")
        {
            if (content is null || string.IsNullOrWhiteSpace(propertyName))
                return null;

            try
            {
                var methodInfo = content.GetType().GetMethod(propertyName, bindingFlags);
                if (methodInfo != null)
                {
                    // Dictionary<string, Tuple<string?, string?>>
                    // "UnitPrice", (min, max)
                    return methodInfo.Invoke(content, new object[] { });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

    }
}


public static class temp
{
    public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
}