using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class CaseVolume
    {
        public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
        public string NewCase { get; set; }
        public string ReopenedCase { get; set; }
    }
}
