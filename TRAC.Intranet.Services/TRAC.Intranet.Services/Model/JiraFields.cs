using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class JiraFields
    {
        public string summary { get; set; }
        //% of Completion
        public double customfield_10808 { get; set; }
        public Priority priority { get; set; }
        public customfield_10715 customfield_10715 { get; set; }
    }
}
