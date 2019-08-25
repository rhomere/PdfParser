using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfParser
{
    public class AttorneyClientSession : Base
    {
        private string _attorneyClientSession = "ATTORNEY-CLIENT SESSION";
        private string _attorneyClientSessionHeaderSpace = "ATTORNEY-CLIENT SESSION \r\n";
        private string _cityOfMiami = "City of Miami";// Problematic because "City of Miami" may exist in resolution body
        private string _textToRemove = "Evaluation Warning : The document was created with Spire.PDF for .NET.";
        private string _textToRemove2 = "City Commission                                          Marked Agenda                                            ";
        private string _start = "AC - ATTORNEY-CLIENT SESSION";
        private string _end = "END OF ATTORNEY-CLIENT SESSION";


    }

    public class AttorneyClientSessionItem
    {
        private string _pdfText { get; set; }
        private StringBuilder _buffer { get; set; } = new StringBuilder();
        private PdfPageBase _pageBase { get; set; }
        private string _result => "RESULT:";
        private string _mover => "MOVER:";
        private string _seconder => "SECONDER:";
        private string _ayes => "AYES:";
        private string _absent => "ABSENT:";

        public string ItemNumber { get; set; }
        public string Body { get; set; }
        public string EnactmentNumber { get; set; }

        public string MotionTo { get; set; }
        public string Result { get; set; }
        public List<string> Movers { get; set; }
        public List<string> Seconders { get; set; }
        public List<string> Ayes { get; set; }
        public List<string> Absent { get; set; }

        public AttorneyClientSessionItem()
        {
            Movers = new List<string>();
            Seconders = new List<string>();
            Ayes = new List<string>();
            Absent = new List<string>();
        }
    }
}
