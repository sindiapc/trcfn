using Microsoft.SharePoint.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Data;
using System.Data.SqlClient;

using TRAC.Intranet.Services.Model;
using Microsoft.SharePoint;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;




namespace TRAC.Intranet.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ReportFindings : IReportFindings
    {
        
        //const string dbConnectionString = "Data Source=DPVMSPDEV;Initial Catalog=TracfoneBIReporting;Integrated Security=true;";
        
        const string dbConnectionString = "Data Source=DPBLSPDB01\\SHAREPOINT,1644;Initial Catalog=TracfoneBIReporting;Integrated Security=true;";


        //const string olddbConnectionString = "Data Source=DPBLSPDB01,1544;Initial Catalog=TracfoneBIReporting;Integrated Security=true;";
        const string jUserName = "read-only";
        const string jPassword = "read-only";
        #region Private Members
        private List<OtherProject> GetJiraInitiatives(string jiraUrl)
        {
            string method = "GET";
            string mergedCredentials = string.Format("{0}:{1}", jUserName, jPassword);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);


            OtherProjectsRoot otherProjectsRoot = new OtherProjectsRoot();
            HttpWebRequest newRequest = WebRequest.Create(jiraUrl) as HttpWebRequest;
            newRequest.ContentType = "application/json";
            newRequest.Method = method;

            string base64Credentials = Convert.ToBase64String(byteCredentials);
            newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            using (HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse)
            {
                string result = string.Empty;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
                otherProjectsRoot = JsonConvert.DeserializeObject<OtherProjectsRoot>(result);
            }
            newRequest = null;
            return otherProjectsRoot.issues;
        }
        private JiraRoot GetTop5PriorityProjects()
        {
            string method = "GET";

            
            JiraRoot priorityIssues = new JiraRoot();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                string mergedCredentials = string.Format("{0}:{1}", jUserName, jPassword);
                byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);

                string jiraUrl = "http://dpvmjira:8080/rest/api/2/search?maxResults=5&fields=summary,key,customfield_10808,customfield_10715,customfield_10716,priority&jql=project = EPO AND issuetype = EPO AND resolution = Unresolved AND labels = priority  and '% of Completion' >0";

                HttpWebRequest newRequest = WebRequest.Create(jiraUrl) as HttpWebRequest;
                newRequest.ContentType = "application/json";
                newRequest.Method = method;

                string base64Credentials = Convert.ToBase64String(byteCredentials);
                newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);

                using (HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse)
                {

                    string result = string.Empty;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                    priorityIssues = JsonConvert.DeserializeObject<JiraRoot>(result);

                }
                newRequest = null;

            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return priorityIssues;

        }
        private JiraRoot GetJiraIssuesByPriority(string priority, int count)
        {
            string method = "GET";

            string jiraUrlFormat = "http://dpvmjira:8080/rest/api/2/search?maxResults={0}&fields=summary,key,customfield_10808,customfield_10715,priority&jql=project = EPO and priority ={1} and '% of Completion' >0";
            JiraRoot priorityIssues = new JiraRoot();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                string mergedCredentials = string.Format("{0}:{1}", jUserName, jPassword);
                byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);

                string jiraUrl = string.Format(jiraUrlFormat, count, priority);

                HttpWebRequest newRequest = WebRequest.Create(jiraUrl) as HttpWebRequest;
                newRequest.ContentType = "application/json";
                newRequest.Method = method;

                string base64Credentials = Convert.ToBase64String(byteCredentials);
                newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);

                HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse;

                string result = string.Empty;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
                //JObject json = JObject.Parse(result);
                priorityIssues = JsonConvert.DeserializeObject<JiraRoot>(result);

                newRequest = null;
                response = null;




            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return priorityIssues;

        }
        private List<Employee> GetUsersFromADWithCaching(string alphabet)
        {
            string cacheKey = "Trac_ADUsers_" + alphabet;
            List<Employee> users = CacheLayer.Get<List<Employee>>(cacheKey);
            if (users == null)
            {
                users = GetUsersFromAD(alphabet);
                CacheLayer.Add(users, cacheKey);
            }
            return users;
        }
        private List<Employee> GetUsersFromAD(string startsWithLetter)
        {
            List<Employee> userResults = new List<Employee>();
            PrincipalSearchResult<Principal> searchResult;

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, null))
                {
                    using (UserPrincipal userPrincipal = new UserPrincipal(principalContext))
                    {
                        userPrincipal.Name = String.Format("{0}*", startsWithLetter);

                        using (PrincipalSearcher searcher = new PrincipalSearcher(userPrincipal))
                        {

                            //searcher.QueryFilter = userPrincipal;
                            searchResult = searcher.FindAll();
                            Parallel.ForEach(searchResult, principal =>
                            //foreach (Principal principal in searchResult)
                            {
                                DirectoryEntry user = (DirectoryEntry)principal.GetUnderlyingObject();
                                //512 is Active Normal User
                                //544 is password not required users
                                if ((Convert.ToString(user.Properties["department"].Value)) != "" && (Convert.ToString(user.Properties["userAccountControl"].Value) == "512"|| Convert.ToString(user.Properties["userAccountControl"].Value) == "544")) 
                                {
                                    userResults.Add(new Employee()
                                    {
                                        Name = Convert.ToString(user.Properties["name"].Value),
                                        Email = Convert.ToString(user.Properties["mail"].Value),
                                        Phone = Convert.ToString(user.Properties["telephoneNumber"].Value),
                                        Title = Convert.ToString(user.Properties["title"].Value),
                                        Department = Convert.ToString(user.Properties["department"].Value),
                                        AccountName = Convert.ToString(user.Properties["sAMAccountName"].Value),
                                        UserLink = "http://dpvmspdev/Person.aspx?accountname=i%3A0%23%2Ew%7C" + "topp_telecom%5C" + Convert.ToString(user.Properties["sAMAccountName"].Value),
                                        Photo = user.Properties["thumbnailPhoto"].Value == null ? string.Empty : "data:image/Bmp;base64," + Convert.ToBase64String((byte[])(user.Properties["thumbnailPhoto"].Value))

                                    });
                                }
                                // string userLink = "http://dpvmspdev/Person.aspx?accountname=i%3A0%23%2Ew%7C" +"topp_telecom%5C" + AccountName;
                                //buildHtml.AppendFormat(userFormat, "", userName, userTitle, userDept, userPhone, userEmail);
                            });//parallel foreach
                        }
                    }
                }
            });

            return userResults.OrderBy(item => item.Name).ToList();
        }
        private void LogMessage(string message)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                EventLog logger = new EventLog();
                logger.Source = "TRAC.Intranet";
                logger.WriteEntry(message, EventLogEntryType.Error);
            });
        }
        #endregion

        #region No Caching Code
        private List<Subscriber> GetActiveSubscribersTotalNoCache()
        {
            List<Subscriber> totalActiveSubscribers = new List<Subscriber>();
            try
            {

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        connection.Open();


                        using (SqlCommand cmd = new SqlCommand("TRAC_GetActiveSubscribersTotal", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {

                                        totalActiveSubscribers.Add(new Subscriber()
                                        {
                                            Brand = reader["BrandName"].ToString().Trim(),
                                            ActiveSubscribers = Convert.ToInt64(reader["SubBase"]),
                                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                            IsBrand = Convert.ToBoolean(reader["IsBrand"]),
                                            Goal = Convert.ToString(reader["Goal"]),
                                            NetGains = Convert.ToString(reader["NetGains"]),
                                        });
                                    }

                                }
                            }

                        }

                    }
                });


            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }
            finally
            {

            }
            return totalActiveSubscribers;
        }
        private List<Subscriber> GetActiveSubscribersByBrandNoCache()
        {
            List<Subscriber> activeSubscribers = new List<Subscriber>();
            try
            {

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        connection.Open();


                        using (SqlCommand cmd = new SqlCommand("TRAC_GetActiveSubscribersByBrand", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {

                                        activeSubscribers.Add(new Subscriber()
                                        {
                                            Brand = reader["BrandName"].ToString().Trim(),
                                            ActiveSubscribers = Convert.ToInt64(reader["SubBase"]),
                                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                            IsBrand = Convert.ToBoolean(reader["IsBrand"]),
                                            Goal = Convert.ToString(reader["Goal"]),
                                            NetGains = Convert.ToString(reader["NetGains"]),
                                        });
                                    }

                                }
                            }

                        }

                    }
                });


            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return activeSubscribers;
        }        
        private List<YTDRevenue> GetYTDRevenueNoCache()
        {
            List<YTDRevenue> ytdRevenue = new List<YTDRevenue>();
            try
            {


                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        connection.Open();

                        using (SqlCommand cmd = new SqlCommand("TRAC_GetYTDRevenue", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        ytdRevenue.Add(new YTDRevenue()
                                        {
                                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                            Year = Convert.ToString(reader["Year"]),
                                            Revenue = Convert.ToString(reader["Revenue"]),
                                            IsActual = Convert.ToBoolean(reader["IsActual"]),
                                            IsCurrentYear = Convert.ToBoolean(reader["IsCurrentYear"]),
                                            PlannedPercentage = Convert.ToString(reader["PercentPlan"]),
                                            PrevYearPercentage = Convert.ToString(reader["PercentPrevYear"])

                                        });
                                    }


                                }
                            }

                        }

                    }
                });



            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return ytdRevenue;
        }
        private List<MTDRevenue> GetMTDRevenueNoCache()
        {
            List<MTDRevenue> mtdRevenue = new List<MTDRevenue>();
            try
            {


                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        connection.Open();

                        using (SqlCommand cmd = new SqlCommand("TRAC_GetMTDRevenue", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        mtdRevenue.Add(new MTDRevenue()
                                        {
                                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                            Year = Convert.ToString(reader["Year"]),
                                            Month = Convert.ToString(reader["MonthName"]),
                                            Revenue = Convert.ToString(reader["Revenue"]),
                                            IsActual = Convert.ToBoolean(reader["IsActual"]),
                                            IsCurrentYear = Convert.ToBoolean(reader["IsCurrentYear"]),
                                            IsCurrentMonth = Convert.ToBoolean(reader["IsCurrentMonth"]),
                                            PlannedPercentage = Convert.ToString(reader["PercentPlan"]),
                                            PrevYearPercentage = Convert.ToString(reader["PercentPrevYear"])
                                        });
                                    }


                                }
                            }

                        }

                    }
                });

            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return mtdRevenue;
        }        
        private List<NPS> GetNPSByBrandNoCache()
        {
            List<NPS> npsScores = new List<NPS>();
            try
            {

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        connection.Open();


                        using (SqlCommand cmd = new SqlCommand("TRAC_GetNPSByBrand", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {

                                        npsScores.Add(new NPS() { Brand = reader["BrandName"].ToString().Trim(), Score = Convert.ToDecimal(reader["NPS"]), SurveyDate = Convert.ToString(reader["SurveyDate"]) });
                                    }

                                }
                            }

                        }

                    }
                });


            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return npsScores;
        }
        private List<NPS> GetNPSTrendByBrandNoCache(string brandName)
        {
            List<NPS> npsScores = new List<NPS>();
            try
            {

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        connection.Open();


                        using (SqlCommand cmd = new SqlCommand("TRAC_GetNPSTrend", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            //SqlParameter brandParam = new SqlParameter("@brandName", SqlDbType.NChar);
                            cmd.Parameters.Add("@brandName", SqlDbType.NChar, 50).Value = brandName;



                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        npsScores.Add(new NPS()
                                        {
                                            Brand = reader["BrandName"].ToString().Trim(),
                                            IsBrand = Convert.ToBoolean(reader["IsBrand"]),
                                            Score = Convert.ToDecimal(reader["NPS"]),
                                            SurveyMonth = Convert.ToString(reader["SurveyMonth"]),
                                            SurveyYear = Convert.ToString(reader["SurveyYear"]),
                                            SurveyDate = Convert.ToString(reader["SurveyDate"])
                                        });
                                    }

                                }
                            }

                        }

                    }
                });


            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return npsScores;
        }
        
        private List<JiraIssue> GetJiraPriorityProjectsNoCache()
        {
            
            JiraRoot top5PriorityProjects = GetTop5PriorityProjects();
            return top5PriorityProjects.issues;
        }       
        private OtherInitiatives GetJiraOtherProjectsNoCache()
        {

            OtherInitiatives otherInitiatives = new OtherInitiatives();
            
            try
            {
                
                string jiraUrlProjectsInProgress = "http://dpvmjira:8080/rest/api/2/search?fields=summary,key,description,customfield_10715,status,customfield_11101,customfield_10708&jql=project = EPO AND issuetype = EPO AND status in (Execution, \"In Testing\", Closing) AND NOT labels in (IT_OPs_Project, Strategic-Initiative)";
                otherInitiatives.ProjectsInProgress = GetJiraInitiatives(jiraUrlProjectsInProgress);

                string jiraUrlOpportunities = "http://dpvmjira:8080/rest/api/2/search?fields=summary,key,description,customfield_10715,status,customfield_11101,customfield_10708&jql=project = EPO AND issuetype = EPO AND status in (Planning, Initiation) AND NOT labels in (IT_OPs_Project, Strategic-Initiative)";
                otherInitiatives.Oppoprtunities = GetJiraInitiatives(jiraUrlOpportunities);

                return otherInitiatives;

            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
                return otherInitiatives;
            }

            
        }
        private ProjectRoot GetJiraProjectByKeyNoCache(string projectKey)
        {
            string method = "GET";

            string jiraUrlFormat = "http://dpvmjira:8080/rest/api/2/issue/{0}?fields=summary,key,customfield_10715,priority,description,status,customfield_11101,customfield_10708,customfield_10705,customfield_10610,customfield_10611,customfield_10706,customfield_10707,comment,customfield_10716";
            ProjectRoot project = new ProjectRoot();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                string mergedCredentials = string.Format("{0}:{1}", jUserName, jPassword);
                byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);

                string jiraUrl = string.Format(jiraUrlFormat, projectKey);

                HttpWebRequest newRequest = WebRequest.Create(jiraUrl) as HttpWebRequest;

                newRequest.ContentType = "application/json";
                newRequest.Method = method;

                string base64Credentials = Convert.ToBase64String(byteCredentials);
                newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);

                using (HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse)
                {

                    string result = string.Empty;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                    project = JsonConvert.DeserializeObject<ProjectRoot>(result);


                }
                newRequest = null;



            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return project;
        }
        public WFReport GetReportData()
        {
            WFReport reportData = new WFReport();
            reportData.Activations = new List<Activations>();
            reportData.Operations = new Operations();
            reportData.BYOPs = new List<BYOP>();
            reportData.CaseVolume = new CaseVolume();
            reportData.PhoneTypes = new List<PhoneType>();
            reportData.TransferRates = new List<TransferRate>();
            reportData.YTDNetGains = new List<NetGain>();
            reportData.RetailActivations = new List<RetailActivation>();
            reportData.AirtimeRevenues = new List<Airtime>();

            reportData.IsActivationsAvailable = false;
            reportData.IsBYOPsAvailable = false;
            reportData.IsPhoneTypesAvailable = false;
            reportData.IsTransferRatesAvailable = false;
            reportData.IsYTDNetGainsAvailable = false;
            reportData.IsRetailActivationAvailable = false;
            reportData.IsAirtimeAvailable = false;

            reportData.Operations.IsAvailable = false;
            reportData.CaseVolume.IsAvailable = false;
            reportData.IsRetailActivationAvailable = false;
            try
            {
                #region to be done in phase 2

                //SPSecurity.RunWithElevatedPrivileges(delegate ()
                //{
                //    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                //    {
                //        connection.Open();

                //        using (SqlCommand cmd = new SqlCommand("TRAC_GetReportData", connection))
                //        {
                //            cmd.CommandType = CommandType.StoredProcedure;

                //            using (SqlDataReader reader = cmd.ExecuteReader())
                //            {

                //                if (reader.HasRows)
                //                {
                //                    //Report Date
                //                    #region Report Date

                //                    while (reader.Read())
                //                    {
                //                        reportData.ReportDate = Convert.ToString(reader["ReportDate"]);
                //                    }

                //                    #endregion

                //                    //Activations
                //                    #region Activations
                //                    if (reader.NextResult())
                //                    {
                //                        while (reader.Read())
                //                        {
                //                            reportData.IsActivationsAvailable = true;
                //                            reportData.Activations.Add(new Activations()
                //                            {
                //                                Brand = Convert.ToString(reader["BrandName"]).Trim(),
                //                                IsBrand = Convert.ToBoolean(reader["IsBrand"]),
                //                                TotalActivations = Convert.ToString(reader["TotalActivations"]),
                //                                NewActivations = Convert.ToString(reader["NewActivations"]),
                //                                Reactivations = Convert.ToString(reader["Reactivations"]),
                //                                Deactivations = Convert.ToString(reader["Deactivations"]),
                //                                AdjustedGrossAdds = Convert.ToString(reader["AdjustedGrossAdds"]),
                //                                Churn = Convert.ToString(reader["Churn"]),
                //                                ChurnPercentage = Convert.ToString(reader["ChurnPercentage"]),
                //                                NetGains = Convert.ToString(reader["NetGains"]),
                //                                MTDNetGains = Convert.ToString(reader["MTDNetGains"]),
                //                                MTDGainsRunRate = Convert.ToString(reader["MTDGainsRunRate"]),
                //                                ARPU = Convert.ToString(reader["ARPU"]),
                //                                SubBase = Convert.ToString(reader["SubBase"]),
                //                                PhoneExchangeShip = Convert.ToString(reader["PhoneExchangeShip"]),
                //                                SimExchangeShip = Convert.ToString(reader["SimExchangeShip"]),
                //                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),

                //                            });


                //                        }
                //                    }
                //                    #endregion

                //                    //BYOPSales
                //                    #region BYOPSales
                //                    if (reader.NextResult())
                //                    {
                //                        while (reader.Read())
                //                        {
                //                            reportData.IsBYOPsAvailable = true;

                //                            reportData.BYOPs.Add(new BYOP()
                //                            {
                //                                Brand = Convert.ToString(reader["BrandName"]).Trim(),
                //                                IsBrand = Convert.ToBoolean(reader["IsBrand"]),
                //                                PhonesShipped = Convert.ToString(reader["PhonesShipped"]),
                //                                SIMCardShipped = Convert.ToString(reader["SIMCardShipped"]),
                //                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])

                //                            });

                //                        }
                //                    }
                //                    #endregion

                //                    //Case Volumn
                //                    #region Case Volumn
                //                    if (reader.NextResult())
                //                    {
                //                        while (reader.Read())
                //                        {
                //                            reportData.CaseVolume.IsAvailable = true;
                //                            reportData.CaseVolume.NewCase = Convert.ToString(reader["NewCase"]);
                //                            reportData.CaseVolume.ReopenedCase = Convert.ToString(reader["ReopenedCase"]);
                //                            reportData.CaseVolume.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);

                //                        }
                //                    }
                //                    #endregion

                //                    //Operations
                //                    #region Operations
                //                    if (reader.NextResult())
                //                    {
                //                        while (reader.Read())
                //                        {
                //                            reportData.Operations.IsAvailable = true;
                //                            reportData.Operations.TotalCallVol = Convert.ToString(reader["TotalCallVol"]);
                //                            reportData.Operations.AgentCallVol = Convert.ToString(reader["AgentCallVol"]);
                //                            reportData.Operations.AnswerRate = Convert.ToString(reader["AnswerRate"]);
                //                            reportData.Operations.CustTermCalls = Convert.ToString(reader["CustTermCalls"]);
                //                            reportData.Operations.MTDCustTermCalls = Convert.ToString(reader["MTDCustTermCalls"]);
                //                            reportData.Operations.CustDropCalls = Convert.ToString(reader["CustDropCalls"]);
                //                            reportData.Operations.MTDCustDropCalls = Convert.ToString(reader["MTDCustDropCalls"]);
                //                            reportData.Operations.SystemicOperationalIssue = Convert.ToString(reader["SystemicOperationalIssue"]);
                //                            reportData.Operations.MTDSystemicOperationalIssue = Convert.ToString(reader["MTDSystemicOperationalIssue"]);
                //                            reportData.Operations.AirtimePins = Convert.ToString(reader["AirtimePins"]);
                //                            reportData.Operations.MTDAirtimePins = Convert.ToString(reader["MTDAirtimePins"]);
                //                            reportData.Operations.AgentASA = Convert.ToString(reader["AgentASA"]);
                //                            reportData.Operations.ActualCarrierDeactivation = Convert.ToString(reader["ActualCarrierDeactivation"]);
                //                            reportData.Operations.ActualATTDeactivation = Convert.ToString(reader["ActualATTDeactivation"]);
                //                            reportData.Operations.TMobBYOPSales = Convert.ToString(reader["TMobBYOPSales"]);
                //                            reportData.Operations.TotalCarrierDeactivation = Convert.ToString(reader["TotalCarrierDeactivation"]);
                //                            reportData.Operations.CingularDeactivation = Convert.ToString(reader["CingularDeactivation"]);
                //                            reportData.Operations.SIMCardExchanges = Convert.ToString(reader["SIMCardExchanges"]);
                //                            reportData.Operations.SIMCardReplacements = Convert.ToString(reader["SIMCardReplacements"]);
                //                            reportData.Operations.MTDSIMCardExchanges = Convert.ToString(reader["MTDSIMCardExchanges"]);
                //                            reportData.Operations.MTDSIMCardReplacements = Convert.ToString(reader["MTDSIMCardReplacements"]);
                //                            reportData.Operations.GSMPhoneExchanges = Convert.ToString(reader["GSMPhoneExchanges"]);
                //                            reportData.Operations.CDMAPhoneExchanges = Convert.ToString(reader["CDMAPhoneExchanges"]);
                //                            reportData.Operations.MTDGSMPhoneExchanges = Convert.ToString(reader["MTDGSMPhoneExchanges"]);
                //                            reportData.Operations.MTDCDMAPhoneExchanges = Convert.ToString(reader["MTDCDMAPhoneExchanges"]);
                //                            reportData.Operations.GSM = Convert.ToString(reader["GSM"]);
                //                            reportData.Operations.CDMA = Convert.ToString(reader["CDMA"]);


                //                            reportData.Operations.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);


                //                        }
                //                    }
                //                    #endregion

                //                    //Phone Type
                //                    #region Phone Types
                //                    if (reader.NextResult())
                //                    {
                //                        while (reader.Read())
                //                        {
                //                            reportData.IsPhoneTypesAvailable = true;
                //                            reportData.PhoneTypes.Add(new PhoneType()
                //                            {
                //                                TypeName = Convert.ToString(reader["TypeName"]).Trim(),
                //                                SmartPhone = Convert.ToString(reader["SmartPhone"]),
                //                                BYOP = Convert.ToString(reader["BYOP"]),
                //                                TFBYOP = Convert.ToString(reader["TFBYOP"]),
                //                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])

                //                            });

                //                        }
                //                    }
                //                    #endregion

                //                    //Transfer Rates
                //                    #region Transfer Rates

                //                    if (reader.NextResult())
                //                    {
                //                        while (reader.Read())
                //                        {
                //                            reportData.IsTransferRatesAvailable = true;
                //                            reportData.TransferRates.Add(new TransferRate()
                //                            {
                //                                Brand = Convert.ToString(reader["BrandName"]).Trim(),
                //                                IsBrand = Convert.ToBoolean(reader["IsBrand"]),
                //                                Redemption = Convert.ToString(reader["Redemption"]),
                //                                Programming = Convert.ToString(reader["Programming"]),
                //                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])

                //                            });

                //                        }
                //                    }
                //                    #endregion

                //                    //YTD Net Gains
                //                    #region YTD Net Gains
                //                    if (reader.NextResult())
                //                    {
                //                        while (reader.Read())
                //                        {
                //                            reportData.IsYTDNetGainsAvailable = true;
                //                            reportData.YTDNetGains.Add(new NetGain()
                //                            {
                //                                Brand = Convert.ToString(reader["BrandName"]).Trim(),
                //                                IsBrand = Convert.ToBoolean(reader["IsBrand"]),
                //                                Year = Convert.ToString(reader["Year"]),
                //                                NetGains = Convert.ToString(reader["NetGains"]),
                //                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])

                //                            });

                //                        }
                //                    }
                //                    #endregion
                //                    //retails activations
                //                    #region Retail Activations
                //                    if (reader.NextResult())
                //                    {
                //                        while (reader.Read())
                //                        {
                //                            reportData.IsRetailActivationAvailable = true;
                //                            reportData.RetailActivations.Add(new RetailActivation()
                //                            {
                //                                Brand = Convert.ToString(reader["BrandName"]).Trim(),
                //                                IsBrand = Convert.ToBoolean(reader["IsBrand"]),
                //                                RetailerName = Convert.ToString(reader["RetailerName"]).Trim(),
                //                                ActivationCount = Convert.ToString(reader["ActivationCount"]),
                //                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])

                //                            });

                //                        }
                //                    }
                //                    #endregion

                //                    //Airtime Revenue
                //                    #region Airtime Revenue
                //                    if (reader.NextResult())
                //                    {
                //                        while (reader.Read())
                //                        {
                //                            reportData.IsAirtimeAvailable = true;
                //                            reportData.AirtimeRevenues.Add(new Airtime()
                //                            {
                //                                Brand = Convert.ToString(reader["BrandName"]).Trim(),
                //                                IsBrand = Convert.ToBoolean(reader["IsBrand"]),
                //                                TotalRevenue = Convert.ToString(reader["TotalRevenue"]),
                //                                RevenueRunRate = Convert.ToString(reader["RevenueRunRate"]),
                //                                AirtimeRevenue = Convert.ToString(reader["AirtimeRevenue"]),
                //                                MTDRevenue = Convert.ToString(reader["MTDRevenue"]),
                //                                AirtimeRevenueMinutes = Convert.ToString(reader["AirtimeRevenueMinutes"]),
                //                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])

                //                            });

                //                        }
                //                    }
                //                    #endregion

                //                }

                //            }

                //        }

                //    }
                //});
                #endregion

            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return reportData;
        }


        #endregion

        #region IReportFindings Implementation
        public List<Subscriber> GetActiveSubscribersTotal()
        {
            string cacheKey = "Trac_ActiveSubscribersTotal";
            List<Subscriber> activeSubscribersTotal = CacheLayer.Get<List<Subscriber>>(cacheKey);
            if (activeSubscribersTotal == null)
            {
                activeSubscribersTotal = GetActiveSubscribersTotalNoCache();
                CacheLayer.Add(activeSubscribersTotal, cacheKey);
            }
            return activeSubscribersTotal;
        }
        public List<Subscriber> GetActiveSubscribersByBrand()
        {
            string cacheKey = "Trac_ActiveSubscribersByBrand";
            List<Subscriber> activeSubscribersByBrand = CacheLayer.Get<List<Subscriber>>(cacheKey);
            if (activeSubscribersByBrand == null)
            {
                activeSubscribersByBrand = GetActiveSubscribersByBrandNoCache();
                CacheLayer.Add(activeSubscribersByBrand, cacheKey);
            }
            return activeSubscribersByBrand;
        }
        public List<YTDRevenue> GetYTDRevenue()
        {
            string cacheKey = "Trac_YTDRevenue";
            List<YTDRevenue> ytdRevenue = CacheLayer.Get<List<YTDRevenue>>(cacheKey);
            if (ytdRevenue == null)
            {
                ytdRevenue = GetYTDRevenueNoCache();
                CacheLayer.Add(ytdRevenue, cacheKey);
            }
            return ytdRevenue;
        }
        public List<MTDRevenue> GetMTDRevenue()
        {
            string cacheKey = "Trac_MTDRevenue";
            List<MTDRevenue> mtdRevenue = CacheLayer.Get<List<MTDRevenue>>(cacheKey);
            if (mtdRevenue == null)
            {
                mtdRevenue = GetMTDRevenueNoCache();
                CacheLayer.Add(mtdRevenue, cacheKey);
            }
            return mtdRevenue;
        }
        public List<NPS> GetNPSByBrand()
        {
            string cacheKey = "Trac_NPSByBrand";
            List<NPS> npsByBrand = CacheLayer.Get<List<NPS>>(cacheKey);
            if (npsByBrand == null)
            {
                npsByBrand = GetNPSByBrandNoCache();
                CacheLayer.Add(npsByBrand, cacheKey);
            }
            return npsByBrand;
        }
        public List<JiraIssue> GetJiraPriorityProjects()
        {

            string cacheKey = "Trac_JiraPriorityProjects";
            List<JiraIssue> jiraPriorityProjects = CacheLayer.Get<List<JiraIssue>>(cacheKey);
            if (jiraPriorityProjects == null)
            {
                jiraPriorityProjects = GetJiraPriorityProjectsNoCache();
                CacheLayer.Add(jiraPriorityProjects, cacheKey);
            }
            return jiraPriorityProjects;
        }
        public List<NPS> GetNPSTrendByBrand(string brandName)
        {
            string cacheKey = "Trac_TrendByBrand_" + brandName;
            List<NPS> npsTrendByBrand = CacheLayer.Get<List<NPS>>(cacheKey);
            if (npsTrendByBrand == null)
            {
                npsTrendByBrand = GetNPSTrendByBrandNoCache(brandName);
                CacheLayer.Add(npsTrendByBrand, cacheKey);
            }
            return npsTrendByBrand;

        }
        
        public OtherInitiatives GetJiraOtherProjects()
        {

            string cacheKey = "Trac_JiraOtherProjects";
            OtherInitiatives jiraOtherProjects = CacheLayer.Get<OtherInitiatives>(cacheKey);
            if (jiraOtherProjects == null)
            {
                jiraOtherProjects = GetJiraOtherProjectsNoCache();
                CacheLayer.Add(jiraOtherProjects, cacheKey);
            }
            return jiraOtherProjects;


        }
        public ProjectRoot GetJiraProjectByKey(string projectKey)
        {
            string cacheKey = "Trac_JiraProjectByKey_" + projectKey;
            ProjectRoot jiraProjectByKey = CacheLayer.Get<ProjectRoot>(cacheKey);
            if (jiraProjectByKey == null)
            {
                jiraProjectByKey = GetJiraProjectByKeyNoCache(projectKey);
                CacheLayer.Add(jiraProjectByKey, cacheKey);
            }
            return jiraProjectByKey;
        }
        public List<Employee> GetEmployees(string startingAlphabet, string useCaching)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                if (useCaching == "true")
                {
                    employees = GetUsersFromADWithCaching(startingAlphabet);
                }
                else
                {
                    employees = GetUsersFromAD(startingAlphabet);
                }
            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            return employees;
        }

        public HomeCharts GetHomeChartsData()
        {
            string cacheKey = "Trac_HomeChartsDataJira";
            HomeCharts homeChartsData = CacheLayer.Get<HomeCharts>(cacheKey);
            if (homeChartsData == null)
            {
                homeChartsData = new HomeCharts();
                homeChartsData.MTDRevenueData = GetMTDRevenueNoCache();
                homeChartsData.YTDRevenueData = GetYTDRevenueNoCache();
                homeChartsData.ActiveSubscribersTotalData = GetActiveSubscribersTotalNoCache();
                homeChartsData.ActiveSubscribersByBrandData = GetActiveSubscribersByBrandNoCache();
                homeChartsData.NPSByBrandData = GetNPSByBrandNoCache();
                homeChartsData.JiraPriorityProjectsData = GetJiraPriorityProjectsNoCache();


                CacheLayer.Add(homeChartsData, cacheKey);
            }
            return homeChartsData;
        }
        public HomeCharts GetHomeChartsDataForce()
        {
            string cacheKey = "Trac_HomeChartsDataJira";
            HomeCharts homeChartsData = new HomeCharts();

            homeChartsData = new HomeCharts();
            homeChartsData.MTDRevenueData = GetMTDRevenueNoCache();
            homeChartsData.YTDRevenueData = GetYTDRevenueNoCache();
            homeChartsData.ActiveSubscribersTotalData = GetActiveSubscribersTotalNoCache();
            homeChartsData.ActiveSubscribersByBrandData = GetActiveSubscribersByBrandNoCache();
            homeChartsData.NPSByBrandData = GetNPSByBrandNoCache();
            homeChartsData.JiraPriorityProjectsData = GetJiraPriorityProjectsNoCache();


            CacheLayer.Add(homeChartsData, cacheKey);

            return homeChartsData;
        }
        #endregion

        //public List<JiraIssue> GetJiraPriorityProjects()
        //{
        //    List<JiraIssue> priorityIssues = new List<JiraIssue>();
        //    int totalReceived = 0;
        //    JiraRoot highestPriorityIssues = GetJiraIssuesByPriority("Highest", 5);
        //    if (highestPriorityIssues.issues.Count > 0)
        //    {
        //        priorityIssues.AddRange(highestPriorityIssues.issues);
        //        totalReceived = highestPriorityIssues.total;
        //    }
        //    if (totalReceived < 5)
        //    {
        //        JiraRoot highPriorityIssues = GetJiraIssuesByPriority("High", 5 - totalReceived);
        //        if (highPriorityIssues.issues.Count > 0)
        //        {
        //            totalReceived = totalReceived + highPriorityIssues.total;
        //            priorityIssues.AddRange(highPriorityIssues.issues);
        //        }
        //    }
        //    if (totalReceived < 5)
        //    {
        //        JiraRoot mediumPriorityIssues = GetJiraIssuesByPriority("Medium", 5 - totalReceived);
        //        if (mediumPriorityIssues.issues.Count > 0)
        //        {
        //            totalReceived = totalReceived + mediumPriorityIssues.total;
        //            priorityIssues.AddRange(mediumPriorityIssues.issues);
        //        }
        //    }

        //    return priorityIssues;
        //}
        //public YTDRevenueChart GetRevenueByBrand()
        //{
        //    YTDRevenueChart ytdRevenueChartData = new YTDRevenueChart();
        //    try
        //    {

        //        List<BrandRevenue> actualRevenueByBrand = new List<BrandRevenue>();
        //        List<BrandRevenue> plannedRevenueByBrand = new List<BrandRevenue>();

        //        SPSecurity.RunWithElevatedPrivileges(delegate ()
        //        {
        //            using (SqlConnection connection = new SqlConnection(dbConnectionString))
        //            {
        //                connection.Open();

        //                using (SqlCommand cmd = new SqlCommand("TRAC_GetRevenueByBrand", connection))
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;

        //                    using (SqlDataReader reader = cmd.ExecuteReader())
        //                    {

        //                        if (reader.HasRows)
        //                        {
        //                            while (reader.Read())
        //                            {

        //                                actualRevenueByBrand.Add(new BrandRevenue()
        //                                {
        //                                    Brand = reader["BrandName"].ToString().Trim(),
        //                                    Revenue = Convert.ToInt64(reader["YTDRevenue"]),
        //                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
        //                                });
        //                            }
        //                            if (reader.NextResult())
        //                            {
        //                                while (reader.Read())
        //                                {

        //                                    plannedRevenueByBrand.Add(new BrandRevenue()
        //                                    {
        //                                        Brand = reader["BrandName"].ToString().Trim(),
        //                                        Revenue = Convert.ToInt64(reader["ForecastRevenue"]),
        //                                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
        //                                    });

        //                                }
        //                            }
        //                            if (reader.NextResult())
        //                            {
        //                                while (reader.Read())
        //                                {
        //                                    ytdRevenueChartData.CurrentYearTotalRevenue = new BrandRevenue()
        //                                    {
        //                                        Brand = reader["BrandName"].ToString().Trim(),
        //                                        Revenue = Convert.ToInt64(reader["YTDRevenue"]),
        //                                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
        //                                    };

        //                                }

        //                            }
        //                            if (reader.NextResult())
        //                            {
        //                                while (reader.Read())
        //                                {
        //                                    ytdRevenueChartData.PrevYearTotalRevenue = new BrandRevenue()
        //                                    {
        //                                        Brand = reader["BrandName"].ToString().Trim(),
        //                                        Revenue = Convert.ToInt64(reader["YTDRevenue"]),
        //                                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
        //                                    };

        //                                }
        //                            }
        //                        }
        //                    }

        //                }

        //            }
        //        });
        //        ytdRevenueChartData.ActualRevenue = actualRevenueByBrand;
        //        ytdRevenueChartData.PlannedRevenue = plannedRevenueByBrand;


        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage(ex.ToString());
        //    }

        //    return ytdRevenueChartData;
        //}
    }
}