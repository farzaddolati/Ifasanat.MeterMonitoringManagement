

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ifasanat.MeterMonitoringManagement.Helper
{
    public class DataProcessor<T>
    {
        public PagedData<T> ProcessData(IEnumerable<T> data, DataSourceRequest request)
        {
            // Apply filtering
            var filteredData = ApplyFiltering(data, request.Filters);

            // Apply sorting
            var sortedData = ApplySorting(filteredData, request.Sorts);

            // Apply pagination
            var pagedData = ApplyPagination(sortedData, request.Page, request.PageSize);

            return new PagedData<T>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = filteredData.Count(),
                Data = pagedData.ToList()
            };
        }

        private IEnumerable<T> ApplyFiltering(IEnumerable<T> data, IList<FilterDescriptor> filters)
        {
            if (filters == null || filters.Count == 0)
                return data;

            var filterFields = new HashSet<string>(filters.Select(filter => filter.Member));

            if (filterFields.Count == 0)
                return data;

            var filteredData = new List<T>();
            var syncObj = new object();

            Parallel.ForEach(data, d =>
            {
                bool include = true;

                foreach (var filter in filters)
                {
                    if (filterFields.Contains(filter.Member) && !FilterEvaluator.Evaluate(d, filter))
                    {
                        include = false;
                        break;
                    }
                }

                if (include)
                {
                    lock (syncObj)
                    {
                        filteredData.Add(d);
                    }
                }
            });

            return filteredData;
        }


        private IOrderedQueryable<T> ApplySorting(IEnumerable<T> data, IList<SortDescriptor> sorts)
        {
            if (sorts == null || sorts.Count == 0)
                return data.AsQueryable().OrderBy(x => 0); // No sorting

            IOrderedQueryable<T> orderedData = null;

            foreach (var sort in sorts)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyExpression = Expression.PropertyOrField(parameter, sort.Member);
                var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExpression, typeof(object)), parameter);

                if (orderedData == null)
                {
                    orderedData = (sort.SortDirection == SortDirection.Ascending) ? data.AsQueryable().OrderBy(lambda) : data.AsQueryable().OrderByDescending(lambda);
                }
                else
                {
                    orderedData = (sort.SortDirection == SortDirection.Ascending) ? orderedData.ThenBy(lambda) : orderedData.ThenByDescending(lambda);
                }
            }

            return orderedData;
        }


        private IEnumerable<T> ApplyPagination(IEnumerable<T> data, int page, int pageSize)
        {
            return data.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }

    public class PagedData<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IList<T> Data { get; set; }
    }

    public class DataSourceRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<FilterDescriptor> Filters { get; set; }
        public IList<SortDescriptor> Sorts { get; set; }
    }

    public class FilterDescriptor
    {
        public string Member { get; set; }
        public object Value { get; set; }
        public FilterOperator Operator { get; set; }
    }

    public enum FilterOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }

    public class SortDescriptor
    {
        public string Member { get; set; }
        public SortDirection SortDirection { get; set; }
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public static class FilterEvaluator
    {
        public static bool Evaluate<T>(T data, FilterDescriptor filter)
        {
            filter.Value = filter.Value.ToString().Split(':').Last().Trim().Trim('"');
            var parameter = Expression.Parameter(typeof(T), "x");
            var memberExpression = Expression.PropertyOrField(parameter, filter.Member);
            var valueExpression = Expression.Constant(filter.Value);
            var comparisonExpression = BuildComparisonExpression(memberExpression, valueExpression, filter.Operator);
            var lambda = Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
            var compiledLambda = lambda.Compile();
            return compiledLambda(data);
        }

        private static Expression BuildComparisonExpression(MemberExpression member, ConstantExpression value, FilterOperator filterOperator)
        {
            if (value.Type == typeof(JsonElement))
            {
                // Handle comparison with JsonElement
                var jsonElementValue = (JsonElement)value.Value;
                var jsonElementExpression = Expression.Constant(jsonElementValue);

                switch (filterOperator)
                {
                    case FilterOperator.Equal:
                        return Expression.Call(jsonElementExpression, "Equals", null, member);
                    case FilterOperator.NotEqual:
                        return Expression.Not(Expression.Call(jsonElementExpression, "Equals", null, member));
                    // Add other cases as needed
                    default:
                        throw new ArgumentException($"Unsupported filter operator: {filterOperator}");
                }
            }
            else
            {
                // Handle comparison with other types (e.g., string, int, etc.)
                switch (filterOperator)
                {
                    case FilterOperator.Equal:
                        return Expression.Equal(member, value);
                    case FilterOperator.NotEqual:
                        return Expression.NotEqual(member, value);
                    // Add other cases as needed
                    default:
                        throw new ArgumentException($"Unsupported filter operator: {filterOperator}");
                }
            }
        }


    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;

//namespace Ifasanat.MeterMonitoringManagement.Helper
//{
//    public class DataProcessor<T>
//    {
//        public PagedData<T> ProcessData(IEnumerable<T> data, DataSourceRequest request)
//        {
//            // Apply filtering
//            var filteredData = ApplyFiltering(data, request.Filters);

//            // Apply sorting
//            var sortedData = ApplySorting(filteredData, request.Sorts);

//            // Apply pagination
//            var pagedData = ApplyPagination(sortedData, request.Page, request.PageSize);

//            return new PagedData<T>
//            {
//                Page = request.Page,
//                PageSize = request.PageSize,
//                TotalCount = filteredData.Count(),
//                Data = pagedData.ToList()
//            };
//        }

//        private IEnumerable<T> ApplyFiltering(IEnumerable<T> data, IList<FilterDescriptor> filters)
//        {
//            if (filters == null || filters.Count == 0)
//                return data;

//            foreach (var filter in filters)
//            {
//                data = data.Where(d => FilterEvaluator.Evaluate(d, filter));
//            }

//            return data;
//        }

//        private IEnumerable<T> ApplySorting(IEnumerable<T> data, IList<SortDescriptor> sorts)
//        {
//            if (sorts == null || sorts.Count == 0)
//                return data;

//            var orderedData = data.AsQueryable();

//            foreach (var sort in sorts)
//            {
//                orderedData = orderedData.OrderBy(sort.Member, sort.SortDirection == SortDirection.Ascending);
//            }

//            return orderedData;
//        }

//        private IEnumerable<T> ApplyPagination(IEnumerable<T> data, int page, int pageSize)
//        {
//            var skip = (page - 1) * pageSize;
//            var paginatedData = data.Skip(skip).Take(pageSize);
//            return paginatedData;
//        }
//    }

//    public class PagedData<T>
//    {
//        public int Page { get; set; }
//        public int PageSize { get; set; }
//        public int TotalCount { get; set; }
//        public IList<T> Data { get; set; }
//    }

//    public class DataSourceRequest
//    {
//        public int Page { get; set; }
//        public int PageSize { get; set; }
//        public IList<FilterDescriptor> Filters { get; set; }
//        public IList<SortDescriptor> Sorts { get; set; }
//    }

//    public class FilterDescriptor
//    {
//        public string Member { get; set; }
//        public object Value { get; set; }
//        public FilterOperator Operator { get; set; }
//    }

//    public enum FilterOperator
//    {
//        Equal,
//        NotEqual,
//        GreaterThan,
//        GreaterThanOrEqual,
//        LessThan,
//        LessThanOrEqual,
//        Contains,
//        StartsWith,
//        EndsWith
//    }

//    public class SortDescriptor
//    {
//        public string Member { get; set; }
//        public SortDirection SortDirection { get; set; }
//    }

//    public enum SortDirection
//    {
//        Ascending,
//        Descending
//    }

//    public static class QueryableExtensions
//    {
//        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool ascending)
//        {
//            var propertyInfo = typeof(T).GetProperty(propertyName);
//            if (propertyInfo == null)
//                throw new ArgumentException($"Property {propertyName} not found on type {typeof(T)}");

//            var parameter = Expression.Parameter(typeof(T), "x");
//            var property = Expression.Property(parameter, propertyInfo);
//            var lambda = Expression.Lambda(property, parameter);

//            string methodName = ascending ? "OrderBy" : "OrderByDescending";
//            var methodCallExpression = Expression.Call(
//                typeof(Queryable),
//                methodName,
//                new[] { typeof(T), propertyInfo.PropertyType },
//                source.Expression,
//                Expression.Quote(lambda)
//            );

//            return source.Provider.CreateQuery<T>(methodCallExpression);
//        }
//    }

//    public static class FilterEvaluator
//    {
//        public static bool Evaluate<T>(T obj, FilterDescriptor filter)
//        {
//            var propertyValue = GetPropertyValue(obj, filter.Member);


//                filter.Value = filter.Value.ToString().Split(':').Last().Trim().Trim('"');


//            switch (filter.Operator)
//            {
//                case FilterOperator.Equal:
//                    return Equals(propertyValue, filter.Value);
//                case FilterOperator.NotEqual:
//                    return !Equals(propertyValue, filter.Value);
//                case FilterOperator.GreaterThan:
//                    return Comparer<object>.Default.Compare(propertyValue, filter.Value) > 0;
//                case FilterOperator.GreaterThanOrEqual:
//                    return Comparer<object>.Default.Compare(propertyValue, filter.Value) >= 0;
//                case FilterOperator.LessThan:
//                    return Comparer<object>.Default.Compare(propertyValue, filter.Value) < 0;
//                case FilterOperator.LessThanOrEqual:
//                    return Comparer<object>.Default.Compare(propertyValue, filter.Value) <= 0;
//                case FilterOperator.Contains:
//                    return propertyValue.ToString().Contains(filter.Value.ToString());
//                case FilterOperator.StartsWith:
//                    return propertyValue.ToString().StartsWith(filter.Value.ToString());
//                case FilterOperator.EndsWith:
//                    return propertyValue.ToString().EndsWith(filter.Value.ToString());
//                default:
//                    throw new ArgumentException($"Unsupported filter operator: {filter.Operator}");

//            }
//        }

//        private static object GetPropertyValue<T>(T obj, string propertyName)
//        {
//            var propertyInfo = typeof(T).GetProperty(propertyName);
//            if (propertyInfo == null)
//                throw new ArgumentException($"Property {propertyName} not found on type {typeof(T)}");

//            return propertyInfo.GetValue(obj);
//        }
//    }
//}
