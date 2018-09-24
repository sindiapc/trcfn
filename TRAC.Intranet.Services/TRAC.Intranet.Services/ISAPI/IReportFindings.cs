using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using TRAC.Intranet.Services.Model;


namespace TRAC.Intranet.Services
{
    [ServiceContract]
    interface IReportFindings
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetHomeChartsDataForce",
           ResponseFormat = WebMessageFormat.Json)]
        HomeCharts GetHomeChartsDataForce();

        [OperationContract]
        [WebGet(UriTemplate = "GetHomeChartsData",
           ResponseFormat = WebMessageFormat.Json)]
        HomeCharts GetHomeChartsData();

        [OperationContract]
        [WebGet(UriTemplate = "GetEmployees/{startingAlphabet}/{useCaching}",
            ResponseFormat = WebMessageFormat.Json)]
        List<Employee> GetEmployees(string startingAlphabet,string useCaching);        

        //[OperationContract]
        //[WebGet(UriTemplate = "GetRevenueByBrand",
        //    ResponseFormat = WebMessageFormat.Json)]
        //YTDRevenueChart GetRevenueByBrand();
        [OperationContract]
        [WebGet(UriTemplate = "GetYTDRevenue",
            ResponseFormat = WebMessageFormat.Json)]
        List<YTDRevenue> GetYTDRevenue();

        [OperationContract]
        [WebGet(UriTemplate = "GetMTDRevenue",
            ResponseFormat = WebMessageFormat.Json)]
        List<MTDRevenue> GetMTDRevenue();

        [OperationContract]
        [WebGet(UriTemplate = "GetActiveSubscribersTotal",
            ResponseFormat = WebMessageFormat.Json)]
        List<Subscriber> GetActiveSubscribersTotal();

        [OperationContract]
        [WebGet(UriTemplate = "GetActiveSubscribersByBrand",
            ResponseFormat = WebMessageFormat.Json)]
        List<Subscriber> GetActiveSubscribersByBrand();

        [OperationContract]
        [WebGet(UriTemplate = "GetNPSByBrand",
            ResponseFormat = WebMessageFormat.Json)]
        List<NPS> GetNPSByBrand();

        [OperationContract]
        [WebGet(UriTemplate = "GetNPSTrendByBrand/{brandName}",
            ResponseFormat = WebMessageFormat.Json)]
        List<NPS> GetNPSTrendByBrand(string brandName);

        [OperationContract]
        [WebGet(UriTemplate = "GetJiraPriorityProjects",
           ResponseFormat = WebMessageFormat.Json)]
        List<JiraIssue> GetJiraPriorityProjects();

        [OperationContract]
        [WebGet(UriTemplate = "GetReportData",
            ResponseFormat = WebMessageFormat.Json)]
        WFReport GetReportData();

        
        [OperationContract]
        [WebGet(UriTemplate = "GetJiraOtherProjects",
           ResponseFormat = WebMessageFormat.Json)]
        OtherInitiatives GetJiraOtherProjects();

        [OperationContract]
        [WebGet(UriTemplate = "GetJiraProjectByKey/{projectKey}",
            ResponseFormat = WebMessageFormat.Json)]
        ProjectRoot GetJiraProjectByKey(string projectKey);


    }

}
