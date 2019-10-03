using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Meeting.Cities.Common
{
    public class ItemBase
    {
        public string ItemNumber { get; set; }
        public string Body { get; set; }
        public string EnactmentNumber { get; set; }
        public string MotionTo { get; set; }
        public string Result { get; set; }
        public List<string> Movers { get; set; } = new List<string>();
        public List<string> Seconders { get; set; } = new List<string>();
        public List<string> Ayes { get; set; } = new List<string>();
        public List<string> Absent { get; set; } = new List<string>();
    }
}
