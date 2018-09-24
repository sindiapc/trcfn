using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class Activations
    {
        public string Brand { get; set; }
        public bool IsBrand { get; set; }

        public string TotalActivations { get; set; }
        public string NewActivations { get; set; }
        public string Reactivations { get; set; }
        public string Deactivations { get; set; }
        public string AdjustedGrossAdds { get; set; }
        public string Churn { get; set; }
        public string ChurnPercentage { get; set; }
        public string NetGains { get; set; }
        public string MTDNetGains { get; set; }
        public string MTDGainsRunRate { get; set; }
        public string ARPU { get; set; }
        public string SubBase { get; set; }
        public string PhoneExchangeShip { get; set; }
        public string SimExchangeShip { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
