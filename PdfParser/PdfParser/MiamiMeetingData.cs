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
    }

    public class MeetingItem
    {
        public string ItemNumber { get; set; }
        public string Body { get; set; }
        public string EnactmentNumber { get; set; }
    }

    public class MayoralVetoes
    {
        private PdfPageCollection _pages;
        private int _mayoralVetoPageIndex;
        private string _pdfText { get; set; }
        private StringBuilder _buffer { get; set; } = new StringBuilder();
        private PdfPageBase _pageBase { get; set; }

        public bool HasVetoes { get; set; }


        public MayoralVetoes(PdfPageCollection pages, int mayoralVetoPageIndex)
        {
            _pages = pages;
            _mayoralVetoPageIndex = mayoralVetoPageIndex;
            _pageBase = pages[mayoralVetoPageIndex];
            _buffer.Append(_pageBase.ExtractText());
            _pdfText = _buffer.ToString();

            LoadMayoralVetoes();
        }

        private void LoadMayoralVetoes()
        {
            if (_pdfText.Contains("NO MAYORAL VETOES"))
            {
                HasVetoes = false;
            }
        }
    }

    public class PublicHearings
    {
        private string _resolution = "RESOLUTION";
        private string _resoltutionHeaderSpace = "RESOLUTION \r\n";
        private string _enactmentNumber = "ENACTMENT NUMBER:";
        private string _motionTo = "MOTION TO:";
        private string _result = "RESULT:";
        private string _mover = "MOVER:";
        private string _seconder = "SECONDER:";
        private string _ayes = "AYES:";
        private string _absent = "ABSENT:";

        private string _pdfText { get; set; }
        private StringBuilder _buffer { get; set; } = new StringBuilder();
        private PdfPageBase _pageBase { get; set; }
        private Spire.Pdf.Widget.PdfPageCollection _pages { get; set; }

        public List<PublicHearingResolution> PublicHearingResolutions { get; set; } = new List<PublicHearingResolution>();

        public PublicHearings(PdfPageCollection pages, int publicHearinsIndex)
        {
            _pages = pages;
            _pageBase = pages[publicHearinsIndex];
            _buffer.Append(_pageBase.ExtractText());
            _pdfText = _buffer.ToString();

            LoadResolutions(pages, publicHearinsIndex);
        }

        private void LoadResolutions(PdfPageCollection pages, int publicHearinsIndex)
        {
            var indexOfResolution = 0;

            var resolutionNumber = string.Empty;
            var motionTo = string.Empty;
            var result = string.Empty;
            var movers = new List<string>();
            var seconders = new List<string>();
            var ayes = new List<string>();
            var absent = new List<string>();
            var enactmentNumber = string.Empty;

            // Paragraph = Index of title + title length, Paragraph length - next title length
            while (_pdfText.Contains(_resolution))
            {
                indexOfResolution = _pdfText.IndexOf(_resolution);
                resolutionNumber = _pdfText.Substring(indexOfResolution, 40).Replace(_resoltutionHeaderSpace, string.Empty).Trim();
                enactmentNumber = _pdfText.Substring(_pdfText.IndexOf(_enactmentNumber) + _enactmentNumber.Length, 30).Trim();

                motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 30).Trim();
                result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 30).Trim();
                movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 40).Trim());
                seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 40).Trim());
                ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 40).Trim().Split(',').ToList());
                absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());

                var resolutionBodyLength = _pdfText.IndexOf(_enactmentNumber) - _pdfText.IndexOf(_resolution);
                var resolutionBody = _pdfText.Substring(_pdfText.IndexOf(_resolution) + _resolution.Length, resolutionBodyLength);
                var endOfResolution = _pdfText.IndexOf(enactmentNumber) + enactmentNumber.Length;

                // Clear last resolution
                _pdfText = _pdfText.Substring(endOfResolution, _pdfText.Length - endOfResolution);

                PublicHearingResolutions.Add(new PublicHearingResolution
                {
                    ItemNumber = resolutionNumber,
                    EnactmentNumber = enactmentNumber,
                    Body = resolutionBody,
                    MotionTo = motionTo,
                    Result = result,
                    Movers = movers,
                    Seconders = seconders,
                    Ayes = ayes,
                    Absent = absent
                });
            }
        }
    }

    public class PublicHearingResolution
    {
        private string _pdfText { get; set; }
        private StringBuilder _buffer { get; set; } = new StringBuilder();
        private PdfPageBase _pageBase { get; set; }
        private string _resolution => "RESOLUTION";
        private string ph1 => "PH.1";
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

        public PublicHearingResolution()
        {
            Movers = new List<string>();
            Seconders = new List<string>();
            Ayes = new List<string>();
            Absent = new List<string>();
        }
    }
}
