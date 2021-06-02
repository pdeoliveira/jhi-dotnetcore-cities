using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
// using System.ComponentModel;

namespace company.world.Web.Rest.Utilities.PrimeNG.LazyLoading
{
    public class LazyLoading<TEntity> where TEntity : class{

        private LazyLoadEvent loadEvent;

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
                Expression expression = null;
                var filters = loadEvent.filters;
                foreach(KeyValuePair<string, List<Dictionary<string, Object>>> filter in filters) {
                    var property = filter.Key.Split(".")[1];
                    var expressionProperty = Expression.Property(expressionInputParameter, property);
                    for(var i = 0; i < filter.Value.Count; i++) {
                        var matchMode = (string)filter.Value[i]["matchMode"];
                        var value = filter.Value[i]["value"];
                        var filterOperator = (string)filter.Value[i]["operator"];
                        if(matchMode == "startsWith" && value != null) {
                            expression = ExpressionFromFilterByMethod("StartsWith", expression, property, (string)value, expressionProperty, filterOperator);
                        }
                        if(matchMode == "contains" && value != null) {
                            expression = ExpressionFromFilterByMethod("Contains", expression, property, (string)value, expressionProperty, filterOperator);
                        }
                        if(matchMode == "notContains" && value != null) {
                            // TODO BUG se usar mais de um filtro
                            expression = Expression.Not(ExpressionFromFilterByMethod("Contains", expression, property, (string)value, expressionProperty, filterOperator));
                        }
                        if(matchMode == "endsWith" && value != null) {
                            expression = ExpressionFromFilterByMethod("EndsWith", expression, property, (string)value, expressionProperty, filterOperator);
                        }
                        if(matchMode == "equals" && value != null) {
                            if(int.TryParse(value.ToString(), out int n)) {
                                Type t = typeof(int?);
                                Type u = Nullable.GetUnderlyingType(t);
                                int? nValue = (int?)Convert.ChangeType(value, u);
                                expression = ExpressionFromFilterEqual(expression, expressionProperty, nValue, filterOperator);
                            }
                            else {
                                expression = ExpressionFromFilterByMethod("Equals", expression, property, (string)value, expressionProperty, filterOperator);
                            }
                        }
                        if(matchMode == "notEquals" && value != null) {
                            // TODO BUG se usar mais de um filtro
                            expression = Expression.Not(ExpressionFromFilterByMethod("Equals", expression, property, (string)value, expressionProperty, filterOperator));
                        }
                    }
                }
                if(expression == null) return null;
                return Expression.Lambda<Func<TEntity, bool>>(expression, expressionInputParameter);
            }
            return null;
        }

        private static Expression ExpressionFromFilterByMethod(string method, Expression expression, string property, string value, MemberExpression expressionProperty, string filterOperator) {
            PropertyInfo propertyInfo = typeof(TEntity).GetProperty(property);
            ConstantExpression c = Expression.Constant(value, typeof(string));
            MethodInfo mi = typeof(string).GetMethod(method, new Type[] { typeof(string) });
            var expressionClause = Expression.Call(expressionProperty, mi, c);
            if(expression == null) {
                expression = expressionClause;
            }
            else {
                if(filterOperator.Equals("and")) {
                    expression = Expression.AndAlso(expression, expressionClause);    
                }
                else {
                    expression = Expression.OrElse(expression, expressionClause);
                }
            }
            return expression;
        }

        private static Expression ExpressionFromFilterEqual(Expression expression, MemberExpression expressionProperty, int? value, string filterOperator) {
            // var propertyType = ((PropertyInfo)expressionProperty.Member).PropertyType;
            // var converter = TypeDescriptor.GetConverter(propertyType);
            // if(!converter.CanConvertFrom(typeof(int))) throw new NotSupportedException();
            // var propertyValue = converter.ConvertToInvariantString(value.ToString());

            var expressionClause = Expression.Equal(expressionProperty, Expression.Constant(value));
            if(expression == null) {
                expression = expressionClause;
            }
            else {
                if(filterOperator.Equals("and")) {
                    expression = Expression.AndAlso(expression, expressionClause);    
                }
                else {
                    expression = Expression.OrElse(expression, expressionClause);
                }
            }
            return expression;
        }
    }
}