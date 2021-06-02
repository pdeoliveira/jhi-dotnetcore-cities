using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace company.world.Web.Rest.Utilities.PrimeNG.LazyLoading
{
    public class LazyLoading<TEntity> where TEntity : class{

        private LazyLoadEvent loadEvent;
        private Expression expression;

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
                        if(matchMode == "startsWith" && value != null) {
                            this.SetExpressionFromFilterByMethod("StartsWith", property, (string)value, expressionProperty, filterOperator);
                        }
                        if(matchMode == "contains" && value != null) {
                            this.SetExpressionFromFilterByMethod("Contains", property, (string)value, expressionProperty, filterOperator);
                        }
                        if(matchMode == "notContains" && value != null) {
                            this.SetExpressionFromFilterByMethod("notContains", property, (string)value, expressionProperty, filterOperator);
                        }
                        if(matchMode == "endsWith" && value != null) {
                            this.SetExpressionFromFilterByMethod("EndsWith", property, (string)value, expressionProperty, filterOperator);
                        }
                        if(matchMode == "equals" && value != null) {
                            if(int.TryParse(value.ToString(), out int n)) {
                                this.SetExpressionFromFilterEqual(expressionProperty, Convert.ToInt32(value), filterOperator);
                            }
                            else {
                                this.SetExpressionFromFilterByMethod("Equals", property, (string)value, expressionProperty, filterOperator);
                            }
                        }
                        if(matchMode == "notEquals" && value != null) {
                            this.SetExpressionFromFilterByMethod("notEquals", property, (string)value, expressionProperty, filterOperator);
                        }
                    }
                }
                if(this.expression == null) return null;
                return Expression.Lambda<Func<TEntity, bool>>(this.expression, expressionInputParameter);
            }
            return null;
        }

        private void SetExpressionFromFilterByMethod(string method, string property, string value, MemberExpression expressionProperty, string filterOperator) {
            bool isNot = method.StartsWith("not");
            if(isNot) { // notContains to Contains
                method = method.Substring(3);
            }
            PropertyInfo propertyInfo = typeof(TEntity).GetProperty(property);
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

        private void SetExpressionFromFilterEqual(MemberExpression expressionProperty, int value, string filterOperator) {
            var expressionClause = Expression.Equal(expressionProperty, Expression.Convert(Expression.Constant(value), typeof(Nullable<int>)));
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