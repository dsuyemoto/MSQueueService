using NUnit.Framework;
using Microsoft.Exchange.WebServices.Data;
using static Microsoft.Exchange.WebServices.Data.SearchFilter;

namespace EWS.Tests
{
    [TestFixture()]
    public class ExchSearchFilterTests
    {
        [Test()]
        public void ExchSearchFilterTest()
        {
            var testExchSearchFilter = new ExchSearchFilter();

            Assert.IsNull(testExchSearchFilter.Filter);
        }

        [Test()]
        public void ContainsSubstringTest()
        {    
            var testSchema = ItemSchema.Subject;
            var testSearchstring = "searchstring";
            var testContainmentMode = ContainmentMode.Substring;
            var testComparisonMode = ComparisonMode.IgnoreCase;
            var testFilter = new ContainsSubstring(testSchema, testSearchstring, testContainmentMode, testComparisonMode);

            var exchSearchFilter = new ExchSearchFilter();
            exchSearchFilter.ContainsSubstring(testSchema, testSearchstring, testContainmentMode, testComparisonMode);
            var filter = (ContainsSubstring)exchSearchFilter.Filter;
            
            Assert.AreEqual(testFilter.PropertyDefinition, filter.PropertyDefinition);
            Assert.AreEqual(testFilter.Value, filter.Value);
            Assert.AreEqual(testFilter.ContainmentMode, filter.ContainmentMode);
            Assert.AreEqual(testFilter.ComparisonMode, filter.ComparisonMode);
        }
    }
}