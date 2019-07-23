using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfParser
{
    public class ConsentAgenda
    {
        private string _pdfText { get; set; }
        private StringBuilder _buffer { get; set; } = new StringBuilder();
        private PdfPageBase _pageBase { get; set; }
        private string _result => "RESULT:";
        private string _mover => "MOVER:";
        private string _seconder => "SECONDER:";
        private string _ayes => "AYES:";
        private string _absent => "ABSENT:";
        private string _ca1 => "CA.1";
        private Spire.Pdf.Widget.PdfPageCollection _pages { get; set; }
        private int _consentAgendaPageIndex { get; set; }

        private int resultLength = 0;
        private int moverLength = 0;
        private int seconderLength = 0;
        private int ayesLength = 0;
        private int absentLength = 0;

        public int consentAgendaPageIndexOut { get; set; }
        public List<MeetingItem> Resolutions { get; set; }
        public string Result { get; set; }
        public List<string> Movers { get; set; }
        public List<string> Seconders { get; set; }
        public List<string> Ayes { get; set; }
        public List<string> Absent { get; set; }

        public bool ConsentAgendComplete { get; set; } = false;

        public ConsentAgenda(Spire.Pdf.Widget.PdfPageCollection pages, int consentAgendaPageIndex, out int outIndex)
        {
            Movers = new List<string>();
            Seconders = new List<string>();
            Ayes = new List<string>();
            Absent = new List<string>();

            _pages = pages;
            _consentAgendaPageIndex = consentAgendaPageIndex;
            _pageBase = pages[consentAgendaPageIndex];
            _buffer.Append(_pageBase.ExtractText());
            _pdfText = _buffer.ToString();

            LoadConsentAgenda(out outIndex);

            ConsentAgendComplete = true;
        }

        public ConsentAgenda()
        {
        }

        private void LoadConsentAgenda(out int consentAgendaPageIndexOut)
        {
            int outIndex = 0;
            if (_pdfText.Contains("The following item(s) was Adopted on the Consent Agenda"))
            {
                LoadResultBoxLengths();

                // Value = Index of Value title + Length of title
                Result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 30).Trim();
                Movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 40).Trim());
                Seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 40).Trim());
                Ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 40).Trim().Split(',').ToList());
                Absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 30).Trim().Split(',').ToList());
                Resolutions = GetConsentAgendaResolutions(_pages, _consentAgendaPageIndex, out outIndex);
            }
            consentAgendaPageIndexOut = outIndex;

        }

        private void LoadResultBoxLengths()
        {
            if (_pdfText.Contains(_result))
            {
                resultLength = _pdfText.IndexOf(_mover) - _pdfText.IndexOf(_result);
            }
            if (_pdfText.Contains(_mover))
            {
                moverLength = _pdfText.IndexOf(_seconder) - _pdfText.IndexOf(_mover);
            }
            if (_pdfText.Contains(_seconder))
            {
                seconderLength = _pdfText.IndexOf(_ayes) - _pdfText.IndexOf(_seconder);
            }
            if (_pdfText.Contains(_ayes))
            {
                ayesLength = _pdfText.IndexOf(_absent) - _pdfText.IndexOf(_ayes);
            }
            if (_pdfText.Contains(_absent))
            {
                absentLength = _pdfText.IndexOf(_ca1) - _pdfText.IndexOf(_absent);
            }
        }

        private static List<MeetingItem> GetConsentAgendaResolutions(Spire.Pdf.Widget.PdfPageCollection pages, int consentAgendaPageIndex, out int indexOut)
        {
            var resolutions = new List<MeetingItem>();

            //var _resolution = "RESOLUTION";
            var _endOfConsentAgenda = "END OF CONSENT AGENDA";

            var pdfText = string.Empty;
            StringBuilder buffer = new StringBuilder();

            var pageBase = pages[consentAgendaPageIndex];
            buffer.Append(pageBase.ExtractText());
            pdfText = buffer.ToString();

            var _consentAgendaEnd = "END OF CONSENT AGENDA";

            while (!pdfText.Contains(_consentAgendaEnd))
            {
                resolutions.AddRange(GetResolutions(pdfText));

                buffer.Clear();
                pageBase = pages[++consentAgendaPageIndex];
                buffer.Append(pageBase.ExtractText());
                pdfText = buffer.ToString();
            }

            var endOfConsentIndex = pdfText.IndexOf(_endOfConsentAgenda);
            var textBeforeEndOfConsent = pdfText.Substring(0, endOfConsentIndex);

            resolutions.AddRange(GetResolutions(textBeforeEndOfConsent));
            indexOut = consentAgendaPageIndex;
            return resolutions;
        }

        private static List<MeetingItem> GetResolutions(string pdfText)
        {
            var resolutions = new List<MeetingItem>();

            var indexOfResolution = 0;
            var _resolution = "RESOLUTION";
            var _resoltutionHeaderSpace = "RESOLUTION \r\n";
            var _enactmentNumber = "ENACTMENT NUMBER:";

            var resolutionNumber = string.Empty;
            var enactmentNumber = string.Empty;

            // Paragraph = Index of title + title length, Paragraph length - next title length
            while (pdfText.Contains(_resolution))
            {
                indexOfResolution = pdfText.IndexOf(_resolution);
                resolutionNumber = pdfText.Substring(indexOfResolution, 40).Replace(_resoltutionHeaderSpace, string.Empty).Trim();
                enactmentNumber = pdfText.Substring(pdfText.IndexOf(_enactmentNumber) + _enactmentNumber.Length, 30).Trim();
                var resolutionBodyLength = pdfText.IndexOf(_enactmentNumber) - pdfText.IndexOf(_resolution);
                var resolutionBody = pdfText.Substring(pdfText.IndexOf(_resolution) + _resolution.Length, resolutionBodyLength);
                var endOfResolution = pdfText.IndexOf(enactmentNumber) + enactmentNumber.Length;

                // Clear last resolution
                pdfText = pdfText.Substring(endOfResolution, pdfText.Length - endOfResolution);

                resolutions.Add(new MeetingItem
                {
                    ItemNumber = resolutionNumber,
                    EnactmentNumber = enactmentNumber,
                    Body = resolutionBody
                });
            }

            return resolutions;
        }
    }
}
