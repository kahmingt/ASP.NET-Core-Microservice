using System.Linq.Expressions;

namespace Service.Product.Shared.Utility
{
    /// <summary>
    /// Custom Expression Predicate Builder.
    /// </summary>
    public static class ExpressionPredicateBuilder
    {
        public static readonly string _EntityName = "e";

        /// <summary>
        /// Comparison enum.
        /// </summary>
        public enum Comparison
        {
            Equal,
            NotEqual,
            GreaterThan,
            GreaterThanOrEqualTo,
            LessThan,
            LessThanOrEqualTo,
        }

        /// <summary>
        /// Convert expression, based on expression target (left/1 or right/2).
        /// </summary>
        public enum Target { Left, Right }


        /// <summary>
        /// Generate single filter expression predicate.
        /// </summary>
        /// <typeparam name="TEntity">Database entity name</typeparam>
        /// <typeparam name="TValue">Value</typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        public static Expression<Func<TEntity, bool>> Build<TEntity, TValue>(
            string propertyName,
            TValue value,
            Comparison comparison)
        {
            var result = PredicateBuilder.True<TEntity>();

            var param = Expression.Parameter(typeof(TEntity), _EntityName);

            Expression left = Expression.Property(param, propertyName);     // propertyName is validated outside before passing in.
            Expression right = Expression.Constant(value, typeof(TValue));

            EnsureSamePropertyType(ref left, ref right);

            var condition = Expression.Condition(
                Expression.Constant(true),
                GenerateComparisonExpression(left, right, comparison),
                Expression.Constant(false)
            );

            return Expression.Lambda<Func<TEntity, bool>>(condition, param);
        }


        /// <summary>
        /// Generate range-based filter expression predicate.
        /// </summary>
        /// <typeparam name="TEntity">Database entity name</typeparam>
        /// <param name="dictionary">
        ///     TKey: Property to be filtered<br/>
        ///     TValue: Filter range as tuple string
        /// </param>
        public static Expression<Func<TEntity, bool>> BuildRange<TEntity>(
            Dictionary<string, Tuple<string?, string?>> dictionary)
        {
            var result = PredicateBuilder.True<TEntity>();

            foreach (KeyValuePair<string, Tuple<string?, string?>> pair in dictionary)
            {
                double min, max;

                if (double.TryParse(pair.Value.Item1, out min) &&
                    double.TryParse(pair.Value.Item2, out max) &&
                    (min > max))
                {
                    continue;
                }

                if (double.TryParse(pair.Value.Item1, out min))
                {
                    var e1 = Build<TEntity, double>(pair.Key, min, Comparison.GreaterThanOrEqualTo);
                    result = result.And(e1);
                }
                if (double.TryParse(pair.Value.Item2, out max))
                {
                    var e2 = Build<TEntity, double>(pair.Key, max, Comparison.LessThanOrEqualTo);
                    result = result.And(e2);
                }
            }
            return result;
        }


        /// <summary>
        /// Generate comparison expression from <see cref="Comparison"/>.
        /// </summary>
        /// <param name="left">Expression 1</param>
        /// <param name="right">Expression 2</param>
        /// <param name="comparison"><see cref="Comparison"/> enum</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static Expression GenerateComparisonExpression(Expression left, Expression right, Comparison comparison)
        {
            return comparison switch
            {
                Comparison.LessThan => Expression.LessThan(left, right),
                Comparison.LessThanOrEqualTo => Expression.LessThanOrEqual(left, right),
                Comparison.Equal => Expression.Equal(left, right),
                Comparison.NotEqual => Expression.NotEqual(left, right),
                Comparison.GreaterThan => Expression.GreaterThan(left, right),
                Comparison.GreaterThanOrEqualTo => Expression.GreaterThanOrEqual(left, right),
                _ => throw new ArgumentOutOfRangeException(nameof(comparison)),
            };
        }

        /// <summary>
        /// Convert expression ensure both are of same type.
        /// </summary>
        /// <param name="expr1">Expression 1</param>
        /// <param name="expr2">Expression 2</param>
        /// <param name="target">Convert to which expression <see cref="Target"/></param>
        private static void EnsureSamePropertyType(ref Expression expr1, ref Expression expr2, Target target = Target.Left)
        {
            //
            if (expr1.Type != expr2.Type && target == Target.Left)
            {
                expr2 = Expression.Convert(expr2, expr1.Type);
            }
            if (expr1.Type != expr2.Type && target == Target.Right)
            {
                expr1 = Expression.Convert(expr1, expr2.Type);
            }

            // Convert to nullable type
            if (IsNullableType(expr1.Type) && !IsNullableType(expr2.Type))
                expr2 = Expression.Convert(expr2, expr1.Type);
            else if (!IsNullableType(expr1.Type) && IsNullableType(expr2.Type))
                expr1 = Expression.Convert(expr1, expr2.Type);
        }

        /// <summary>
        /// Determine if expression is of a nullable type.
        /// </summary>
        /// <param name="t">Expression type</param>
        private static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

    }


    /// <summary>
    /// Dynamically Composing Expression Predicates.
    /// <para>https://www.albahari.com/nutshell/predicatebuilder.aspx</para>
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Creates a predicate that evaluates to true.
        /// </summary>
        public static Expression<Func<T, bool>> True<T>()
        {
            return param => true;
        }

        /// <summary>
        /// Creates a predicate that evaluates to false.
        /// </summary>
        public static Expression<Func<T, bool>> False<T>()
        {
            return param => false;
        }

        /// <summary>
        /// Creates a predicate expression from the specified lambda expression.
        /// </summary>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
        {
            return predicate;
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Negates the predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
            .Select((f, i) => new { f, s = second.Parameters[i] })
            .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        class ParameterRebinder : ExpressionVisitor
        {
            readonly Dictionary<ParameterExpression, ParameterExpression> map;

            ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;

                if (map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }

                return base.VisitParameter(p);
            }
        }
    }
}


