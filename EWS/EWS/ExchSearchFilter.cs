using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWS
{
    public class ExchSearchFilter
    {
        SearchFilter _searchFilter;
        public SearchFilter Filter { get { return _searchFilter; } }
        public ExchSearchFilter() { }

        public void ContainsSubstring(PropertyDefinitionBase propertybase, string searchstring, ContainmentMode containmentmode, ComparisonMode comparisonmode)
        {
            _searchFilter = new SearchFilter.ContainsSubstring(propertybase, searchstring, containmentmode, comparisonmode);
        }
    }
}
