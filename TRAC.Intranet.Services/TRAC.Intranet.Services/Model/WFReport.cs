using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class WFReport
    {
        public string ReportDate { get; set; }
        public List<Activations> Activations { get; set; }
        public List<PhoneType> PhoneTypes { get; set; }
        public List<TransferRate> TransferRates { get; set; }
        public List<BYOP> BYOPs { get; set; }
        public List<NetGain> YTDNetGains { get; set; }
        public List<RetailActivation> RetailActivations { get; set; }
        public List<Airtime> AirtimeRevenues { get; set; }

        public Operations Operations { get; set; }        
        public CaseVolume CaseVolume { get; set; }
       
        public bool IsActivationsAvailable { get; set; }
        public bool IsTransferRatesAvailable { get; set; }
        public bool IsBYOPsAvailable { get; set; }
        public bool IsPhoneTypesAvailable { get; set; }
        public bool IsYTDNetGainsAvailable { get; set; }
        public bool IsRetailActivationAvailable { get; set; }
        public bool IsAirtimeAvailable { get; set; }


    }
}
