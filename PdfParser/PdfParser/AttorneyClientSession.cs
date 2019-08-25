using Spire.Pdf;
using Spire.Pdf.Widget;
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

        public List<AttorneyClientSessionItem> AttorneyClientSessionItems { get; set; } = new List<AttorneyClientSessionItem>();

        public AttorneyClientSession(PdfPageCollection pages, int index, out int outIndex)
        {
            _index = index;
            _pages = pages;
            _pageBase = pages[_index];
            _buffer.Append(_pageBase.ExtractText());
            _ = _buffer.ToString();

            // If Section is only one page
            if (_.Contains(_start) && _.Contains(_end))
            {
                LoadAttorneyClientSessionItems(singlePage: true);
            }

            while (!_.Contains("END OF RESOLUTIONS"))
            {
                LoadAttorneyClientSessionItems();
            }

            // I think this removes the end of the section from the text
            var endOfSRIndex = _.IndexOf(_end);
            var _pdftext = _.Substring(0, endOfSRIndex);

            LoadAttorneyClientSessionItems();

            outIndex = _index;
        }

        private void LoadAttorneyClientSessionItems(bool singlePage = false)
        {
            var indexOfItem = 0;
            var counter = 1;
            var agendaReadingNumber = "AC.";
            var startOfResolution = $"{agendaReadingNumber}{counter.ToString()}                         ATTORNEY-CLIENT SESSION";

            if (!singlePage)
            {
                // Paragraph = Index of title + title length, Paragraph length - next title length

                // While text contains item
                while (_.Contains(startOfResolution))
                {
                    // Declare variables
                    var itemNumber = string.Empty;
                    var motionTo = string.Empty;
                    var result = string.Empty;
                    var movers = new List<string>();
                    var seconders = new List<string>();
                    var ayes = new List<string>();
                    var absent = new List<string>();
                    var enactmentNumber = string.Empty;
                    var itemBodyLength = 0;
                    var itemBody = string.Empty;

                    indexOfItem = _.IndexOf(_attorneyClientSession);
                    // Item # is from title of item plus a certain number of spaces, the length of the 
                    // Item # should be 4 characters
                    itemNumber = _attorneyClientSession = _.Substring(indexOfItem, 40).Replace(_attorneyClientSessionHeaderSpace, string.Empty).Trim();

                    // Body length should be from startOfResolution to MotionTo: minus certain characters
                    // or it there is a consistent ". " space after the period.
                    //itemBodyLength = (_.IndexOf(_cityOfMiami) - _cityOfMiami.Length) - _.IndexOf(_resolution);
                    itemBody = _.Substring(_.IndexOf(startOfResolution), (_.IndexOf(_motionTo) - 1));
                }
            }
        }
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
