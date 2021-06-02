// ===========================================================================
using System;
using System.Text;
using System.Collections.Generic;
//
namespace company.world.Web.Rest.Utilities.PrimeNG.LazyLoading
{
    //
    /// <summary>
    /// PrimeNG structure, used by lazy loading feature.
    /// Class LazyLoadEvent ported from PrimeNG to this library.
    /// Generally, populated by the PrimeNG filter feature.
    /// <example>
    /// An example of the JSON:
    /// {"first":0,"rows":20,"sortOrder":1,
    ///   "filters":{"Id":[{"value":1,"matchMode":"eq"}],
    ///     "Name":[{"value":"John","matchMode":"eq"}],
    ///     "Sex":[{"value":"M","matchMode":"eq"}],
    ///    "globalFilter":null}
    /// </example>
    /// </summary>
    public class LazyLoadEvent
    {
        // {"first":0,"rows":20,"sortOrder":1,"filters":{},"globalFilter":null}
        /// <summary>
        /// First record #.
        /// </summary>
        public long first { get; set; }
        /// <summary>
        /// # of rows to return (page size).
        /// </summary>
        public long rows { get; set; }
        /// <summary>
        /// Sort field.
        /// </summary>
        public string sortField { get; set; }
        /// <summary>
        /// Ascending or desending sort order.
        /// </summary>
        /// <value> 1 = asc, -1 = desc</value>
        public int sortOrder { get; set; }
        /// <summary>
        /// multiSortMeta, not implemented.
        /// </summary>
        public object multiSortMeta { get; set; }

        // {"first":0,"rows":20,"sortOrder":1,"filters":{"city.name":[{"value":"Ka","matchMode":"startsWith","operator":"and"}]},"globalFilter":null}

        /// <summary>
        /// A dictionary of filters.
        /// Key of the dictionary is the field name, object is value(s)
        /// and match mode.
        /// </summary>
        /// <example>
        /// "filters":{"ServerId":[{"value":1,"matchMode":"eq"}],
        ///     "Mailed":[{"value":"false","matchMode":"eq"}],
        ///     "Closed":[{"value":"false","matchMode":"eq"}],
        ///     "Special":[{"value":"false","matchMode":"eq"}]},
        /// </example>
        public Dictionary<string, List<Dictionary<string, Object>>> filters { get; set; }
        /// <summary>
        /// globalFilter, not implemented.
        /// </summary>
        public object globalFilter { get; set; }
        //
        /// <summary>
        /// Returns a string that represents of the current object.
        /// This method overrides the default 'to string' method.
        /// </summary>
        /// <returns>
        /// A formatted string of the object's values.
        /// </returns>
        public override string ToString()
        {
            StringBuilder _return = new StringBuilder("record:[");
                _return.AppendFormat("first: {0}, rows: {1}, ", first, rows);
                _return.AppendFormat("sortField: {0}, sortOrder: {1}, ", sortField, sortOrder);
                _return.AppendFormat("multiSortMeta: {0}, ", multiSortMeta == null ? "null" : multiSortMeta.ToString());
                _return.AppendFormat("filters: {0}, ", filters == null ? "null" : filters.ToString());
                _return.AppendFormat("globalFilter: {0}]", globalFilter == null ? "null" : globalFilter.ToString());
            return _return.ToString();
        }
    }
}
// ===========================================================================
