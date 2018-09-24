using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{

    public class JiraValue
    {
        public string value { get; set; }
    }

    public class JiraUser
    {
        public string displayName { get; set; }
    }
    public class JiraText
    {
        public string name { get; set; }
    }
    public class Comment2
    {
        
        public string body { get; set; }
    }

    public class Comment
    {
        public List<Comment2> comments { get; set; }
    }


    public class ProjectFields
    {
        public string summary { get; set; }
        public string description { get; set; }
        //Target Start Date
        public string customfield_11101 { get; set; }
        //Target End Date
        public string customfield_10708 { get; set; }
        //Business Owner
        public JiraUser customfield_10610 { get; set; }
        //Executive Owner
        public JiraUser customfield_10611 { get; set; }
        //Health
        public JiraValue customfield_10715 { get; set; }
        //Project Manager
        public JiraUser customfield_10705 { get; set; }      
        //Vertical Owner
        public JiraUser customfield_10706 { get; set; }
        //Technical Lead
        public JiraUser customfield_10707 { get; set; }
        //Latest Updates
        public string customfield_10716 { get; set; }
        //Latest Updates
        public Comment comment { get; set; }
        //public DateTime updated { get; set; }
        public JiraText status { get; set; }
        public JiraText priority { get; set; }

    }

    public class ProjectRoot
    {
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public ProjectFields fields { get; set; }
    }
}
