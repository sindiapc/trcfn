using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{  
    public class OtherProjectFields
    {
        public string summary { get; set; }
        public string description { get; set; }
        public string customfield_10708 { get; set; }
        public string customfield_11101 { get; set; }
        public JiraValue customfield_10715 { get; set; }
        public JiraText status { get; set; }
    }

    public class OtherProject
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public OtherProjectFields fields { get; set; }
    }

    public class OtherProjectsRoot
    {
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<OtherProject> issues { get; set; }
    }
}
