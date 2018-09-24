using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public static class SampleData
    {
        // This array is purposely abbreviated for readability in this article.
        // The complete list of presidents is available in the source download.
        public static President[] SamplePresidents = new President[]
        {

            new President {
                Id =  "1", FirstName = "George", LastName = "Washington",
                EmailAddress = Microsoft.SharePoint.SPContext.Current.Web.CurrentUser.ToString() },
            new President {
                Id =  "2", FirstName = "John", LastName = "Adams",
                EmailAddress = "jadams@email.com" },
            new President {
                Id =  "3", FirstName = "Thomas", LastName = "Jefferson",
                EmailAddress = "tjefferson@email.com" },
            new President {
                Id =  "4", FirstName = "James", LastName = "Madison",
                EmailAddress = "jmadison@email.com" },
            new President {
                Id =  "5", FirstName = "James", LastName = "Monroe",
                EmailAddress = "jmonroe@email.com" },
            new President {
                Id = "43", FirstName = "George W.", LastName = "Bush",
                EmailAddress = "gbush@email.com" },
            new President {
                Id = "44", FirstName = "Barack", LastName = "Obama",
                EmailAddress = "bobama@email.com" },
        };

        public static BrandRevenue[] SampleBrandRevenue = new BrandRevenue[]
           {

            new BrandRevenue {Brand="STRAIGHT TALK",Revenue=1650621696 },
            new BrandRevenue {Brand="NET10",Revenue=109325744 },
            new BrandRevenue {Brand="TRACFONE",Revenue=255239407 },
            new BrandRevenue {Brand="SAFELINK",Revenue=174613092 },
            new BrandRevenue {Brand="PAGE PLUS",Revenue=68405253 },
            new BrandRevenue {Brand="GO SMART",Revenue=19423100 },
            new BrandRevenue {Brand="TELCEL",Revenue=1559228 },
            new BrandRevenue {Brand="TOTAL WIRELESS",Revenue=99795064 },
            new BrandRevenue {Brand="SIMPLE MOBILE",Revenue=169936408 },
            new BrandRevenue {Brand="WFM",Revenue=132743622 },

           };
    }
}
