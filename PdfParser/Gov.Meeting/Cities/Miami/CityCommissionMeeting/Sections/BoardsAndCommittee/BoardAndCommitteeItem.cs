using Gov.Meeting.Cities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.BoardsAndCommittee
{
    public class BoardAndCommitteeItem : ItemBase
    {
        public List<(string, string)> AppointeesAndNominators { get; set; } = new List<(string, string)>();

    }
}
