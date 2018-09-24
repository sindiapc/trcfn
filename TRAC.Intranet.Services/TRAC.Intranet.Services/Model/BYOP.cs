using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class BYOP
    {
        public string Brand { get; set; }
        public bool IsBrand { get; set; }
        public string PhonesShipped { get; set; }
        public string SIMCardShipped { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
