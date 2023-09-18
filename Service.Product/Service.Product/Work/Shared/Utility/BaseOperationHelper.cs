using System.Linq.Dynamic.Core; // Dynamic LINQ
using System.Reflection;
using System.Text;

namespace Service.Product.Shared.Utility
{
    /// <summary>
    /// Base abstract class that contains Sorting and Filtering logic.
    /// </summary>
    public abstract class BaseOperationHelper<TEntity> : IBaseOperationHelper<TEntity>
    {
        protected static PropertyInfo[] propertyInfos = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        protected const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public virtual IQueryable<TEntity> Sort(
            IQueryable<TEntity> entity,
            object objectParameter,
            string parameterKey = "OrderBy")
        {
            string? orderBy = GetObjectValue(objectParameter, parameterKey, MemberTypes.Property) as string;

            if (!entity.Any() || string.IsNullOrWhiteSpace(orderBy))
            {
                return entity;
            }

            var orderParamsList = orderBy.Trim().Split(',');
            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParamsList)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                if (ValidateInputAgainstDatabaseColumn<TEntity>(param.Trim(), out string propertyName))
                {
                    var sortingOrder = param.Trim().StartsWith("-") ? "descending" : "ascending";
                    orderQueryBuilder.Append($"{propertyName} {sortingOrder}, ");
                }
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            return entity.OrderBy(orderQuery);
        }

        public abstract IQueryable<TEntity> FilterSingle(
            IQueryable<TEntity> entity,
            object objectParameter,
            string parameterKey = "FilterBy");

        public abstract IQueryable<TEntity> FilterRange(
            IQueryable<TEntity> entity,
            object objectParameter,
            string parameterKey = "RangeFilterBy");

        public virtual IQueryable<TEntity> RunAll(
            IQueryable<TEntity> entity,
            object objectParameter)
        {
            var model = entity;
            model = FilterSingle(model, objectParameter);
            model = FilterRange(model, objectParameter);
            model = Sort(model, objectParameter);
            return model;
        }


        /// <summary>
        /// Get object's name, if found.
        /// </summary>
        /// <param name="content">Object</param>
        /// <param name="propertyName">Object property name</param>
        /// <param name="memberType">MemberTypes</param>
        /// <returns>Object type</returns>
        protected string? GetObjectName(object content, string propertyName, MemberTypes memberType)
        {
            if (string.IsNullOrWhiteSpace(propertyName) || content is null)
                return null;

            try
            {
                if (memberType == MemberTypes.Field)
                {
                    var fieldInfo = content.GetType().GetField(propertyName, bindingFlags);
                    if (fieldInfo != null)
                    {
                        Type type = Nullable.GetUnderlyingType(fieldInfo.FieldType) ?? fieldInfo.FieldType;
                        return fieldInfo.GetType().Name;
                    }
                }
                if (memberType == MemberTypes.Property)
                {
                    var propertyInfo = content.GetType().GetProperty(propertyName, bindingFlags);
                    if (propertyInfo != null)
                    {
                        Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                        return propertyInfo.GetType().Name;
                    }
                }
                if (memberType == MemberTypes.Method)
                {
                    var methodInfo = content.GetType().GetMethod(propertyName, bindingFlags);
                    if (methodInfo != null)
                    {
                        return methodInfo.GetType().Name;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Get property name and value in an object, if found.
        /// </summary>
        /// <param name="content">Object</param>
        /// <param name="propertyNameToSearch">Object property name to search</param>
        /// <returns>Object as Tuple<Type, string, string?></returns>
        protected object? GetObjectProperty(object content, string propertyNameToSearch)
        {
            if (content is null || string.IsNullOrWhiteSpace(propertyNameToSearch))
                return null;

            var propertyInfo = content.GetType().GetProperty(propertyNameToSearch, bindingFlags);
            if (propertyInfo != null && propertyInfo.Name.Equals(propertyNameToSearch))
            {
                var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var name = propertyInfo.Name;
                var value = propertyInfo.GetValue(content, null);
                return (type, name, value);
            }
            return null;
        }

        /// <summary>
        /// Get object's value, if found.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="propertyName"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        protected object? GetObjectValue(object content, string propertyName, MemberTypes memberType)
        {
            if (content is null || string.IsNullOrWhiteSpace(propertyName))
                return null;

            try
            {
                if (memberType == MemberTypes.Field)
                {
                    var fieldInfo = content.GetType().GetField(propertyName, bindingFlags);
                    if (fieldInfo != null)
                    {
                        return fieldInfo.GetValue(content);
                    }
                }
                if (memberType == MemberTypes.Property)
                {
                    var propertyInfo = content.GetType().GetProperty(propertyName, bindingFlags);
                    if (propertyInfo != null)
                    {
                        return propertyInfo.GetValue(content, null);
                    }
                }
                if (memberType == MemberTypes.Method)
                {
                    var methodInfo = content.GetType().GetMethod(propertyName, bindingFlags);
                    if (methodInfo != null)
                    {
                        return methodInfo.Invoke(content, new object[] { });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Validate input against <see cref="TEntity"/> database column.
        /// </summary>
        /// <typeparam name="TEntity">Database entity</typeparam>
        /// <param name="input">Input parameter</param>
        /// <param name="name">out Database property name</param>
        /// <returns></returns>
        protected bool ValidateInputAgainstDatabaseColumn<TEntity>(string input, out string name)
        {
            name = "";
            var propertyFromQueryName = input.Split(" ")[0];
            var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

            if (objectProperty != null)
            {
                name = objectProperty.Name.ToString();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validate min and max value of any given pair.
        /// </summary>
        /// <typeparam name="TType">Type</typeparam>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Validate result. [True] Valid range. [False] Invalid range. </returns>
        protected bool ValidateRange<TType>(TType? min, TType? max) where TType : struct, IComparable<TType>
        {
            if (min.HasValue && max.HasValue)
            {
                return !(min.Value.CompareTo(max.Value) > 0);
            }
            return true;
        }

    }

}