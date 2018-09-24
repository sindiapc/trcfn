using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class Operations
    {
        public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TotalCallVol { get; set; }
        public string AgentCallVol { get; set; }
        public string AnswerRate { get; set; }
        public string CustTermCalls { get; set; }
        public string MTDCustTermCalls { get; set; }
        public string CustDropCalls { get; set; }
        public string MTDCustDropCalls { get; set; }
        public string SystemicOperationalIssue { get; set; }
        public string MTDSystemicOperationalIssue { get; set; }
        public string AirtimePins { get; set; }
        public string MTDAirtimePins { get; set; }
        public string AgentASA { get; set; }
        public string ActualCarrierDeactivation { get; set; }
        public string ActualATTDeactivation { get; set; }
        public string TMobBYOPSales { get; set; }
        public string TotalCarrierDeactivation { get; set; }
        public string CingularDeactivation { get; set; }
        public string SIMCardExchanges { get; set; }
        public string SIMCardReplacements { get; set; }
        public string MTDSIMCardExchanges { get; set; }
        public string MTDSIMCardReplacements { get; set; }
        public string GSMPhoneExchanges { get; set; }
        public string CDMAPhoneExchanges { get; set; }
        public string MTDGSMPhoneExchanges { get; set; }
        public string MTDCDMAPhoneExchanges { get; set; }
        public string GSM { get; set; }
        public string CDMA { get; set; }
    }
}
