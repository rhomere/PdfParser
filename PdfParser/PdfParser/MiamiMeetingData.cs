using Spire.Pdf;
using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfParser
{
    public class MiamiMeetingData
    {
        public ConsentAgenda ConsentAgenda { get; set; } = new ConsentAgenda();
        public MayoralVetoes MayoralVeotes { get; set; }
        public PublicHearings PublicHearings { get; set; }
        public SecondReading SecondReadings { get; set; }
        public Reading FirstReadings { get; set; }
        public Resolution Resolutions { get; set; }
        public AttorneyClientSession AttorneyClientSession { get; set; }
        public BoardsAndCommittee BoardsAndCommittee { get; set; }
    }
}
