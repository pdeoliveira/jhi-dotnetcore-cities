using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace company.world.Web.Rest.Utilities.PrimeNG.LazyLoading
{
    public class LazyLoading<TEntity> where TEntity : class{

        private LazyLoadEvent loadEvent;
        private Expression expression;
        
        enum NumericFilters
        {
            Equal,
            NotEqual,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual
        };

        private Dictionary<string, string> stringMethods = new Dictionary<string, string>()
        {
            { "startsWith", "StartsWith" },
            { "contains", "Contains" },
            { "notContains", "notContains" },
            { "endsWith", "EndsWith" },
            { "equals", "Equals" },
            { "notEquals", "notEquals" }
        };
            
        private Dictionary<string, NumericFilters> numericFilters = new Dictionary<string, NumericFilters>()
        {
            { "equals", NumericFilters.Equal },
            { "notEquals", NumericFilters.NotEqual },
            { "lt", NumericFilters.LessThan },
            { "lte", NumericFilters.LessThanOrEqual },
            { "gt", NumericFilters.GreaterThan },
            { "gte", NumericFilters.GreaterThanOrEqual }
        };

        public LazyLoading(LazyLoadEvent loadEvent) {
            this.loadEvent = loadEvent;
        }

        // "filters":{
        // "city.name":[{"value":"Al","matchMode":"startsWith","operator":"and"}],
        // "city.countryCode":[{"value":null,"matchMode":"startsWith","operator":"and"}],
        // "city.district":[{"value":null,"matchMode":"startsWith","operator":"and"}],
        // "city.population":[{"value":null,"matchMode":"equals","operator":"and"}
        // ]}
        public Expression<Func<TEntity, bool>> ExpressionFromFilters() {
            var expressionInputParameter = Expression.Parameter(typeof(TEntity), "x");
            if(this.loadEvent != null && this.loadEvent.filters != null && this.loadEvent.filters.Count > 0) {
                var filters = loadEvent.filters;
                foreach(KeyValuePair<string, List<Dictionary<string, Object>>> filter in filters) {
                    var property = filter.Key.Split(".")[1];
                    var expressionProperty = Expression.Property(expressionInputParameter, property);
                    for(var i = 0; i < filter.Value.Count; i++) {
                        var matchMode = (string)filter.Value[i]["matchMode"];
                        var value = filter.Value[i]["value"];
                        var filterOperator = (string)filter.Value[i]["operator"];
                        if(value != null) {                        
                            if(int.TryParse(value.ToString(), out int intValue)) { // int value
                                this.SetExpression(intValue, typeof(Nullable<int>), numericFilters[matchMode], expressionProperty, filterOperator);
                            }
                            else if(double.TryParse(value.ToString(), out double doubleValue)) { // int value
                                this.SetExpression(doubleValue, typeof(Nullable<double>), numericFilters[matchMode], expressionProperty, filterOperator);
                            }
                            else { // string value
                                this.SetExpression((string)value, stringMethods[matchMode], expressionProperty, filterOperator);
                            }
                        }
                    }
                }
                if(this.expression == null) return null;
                return Expression.Lambda<Func<TEntity, bool>>(this.expression, expressionInputParameter);
            }
            return null;
        }

        private void SetExpression(string value, string method, MemberExpression expressionProperty, string filterOperator) {
            bool isNot = method.StartsWith("not");
            if(isNot) { // notContains to Contains
                method = method.Substring(3);
            }
            PropertyInfo propertyInfo = typeof(TEntity).GetProperty(expressionProperty.Member.Name);
            ConstantExpression c = Expression.Constant(value, typeof(string));
            MethodInfo mi = typeof(string).GetMethod(method, new Type[] { typeof(string) });
            Expression expressionClause;
            if(isNot) {
                expressionClause = Expression.Not(Expression.Call(expressionProperty, mi, c));
            }
            else {
                expressionClause = Expression.Call(expressionProperty, mi, c);
            }
            this.SetExpressionAfterOperator(expressionClause, filterOperator);
        }

        private void SetExpression(object value, Type type, NumericFilters filter, MemberExpression expressionProperty, string filterOperator) {
            Expression expressionClause = Expression.Convert(Expression.Constant(value), type);
            switch(filter) {
                case NumericFilters.Equal:
                    expressionClause = Expression.Equal(expressionProperty, expressionClause);
                    break;
                case NumericFilters.NotEqual:
                    expressionClause = Expression.NotEqual(expressionProperty, expressionClause);
                    break;
                case NumericFilters.LessThan:
                    expressionClause = Expression.LessThan(expressionProperty, expressionClause);
                    break;
                case NumericFilters.LessThanOrEqual:
                    expressionClause = Expression.LessThanOrEqual(expressionProperty, expressionClause);
                    break;
                case NumericFilters.GreaterThan:
                    expressionClause = Expression.GreaterThan(expressionProperty, expressionClause);
                    break;
                case NumericFilters.GreaterThanOrEqual:
                    expressionClause = Expression.GreaterThanOrEqual(expressionProperty, expressionClause);
                    break;
                default:
                    throw new InvalidFilterCriteriaException();
            }
            this.SetExpressionAfterOperator(expressionClause, filterOperator);
        }

        private void SetExpressionAfterOperator(Expression expressionClause, string filterOperator) {
            if(this.expression == null) {
                this.expression = expressionClause;
            }
            else {
                if(filterOperator.Equals("and")) {
                    this.expression = Expression.AndAlso(this.expression, expressionClause);    
                }
                else {
                    this.expression = Expression.OrElse(this.expression, expressionClause);
                }
            }
        }
    }
}