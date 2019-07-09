using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf;

namespace PdfParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a pdf document

            MiamiMeetingMinutes();

            

            //MiamiAgendaItem(pdfText);

            

            Console.ReadLine();
            //save text
            //String fileName = "TextInPdf.txt";
            //File.WriteAllText(fileName, buffer.ToString());

            //System.Diagnostics.Process.Start(fileName);

        }

        private static void MiamiMeetingMinutes()
        {
            PdfDocument doc = new PdfDocument();

            doc.LoadFromFile(@"C:\Users\User\OneDrive\Projects\Regular Meeting\01-10-2019\Meeting.pdf");

            StringBuilder buffer = new StringBuilder();

            var pageCounter = 1;
            var mayoralVetoresStart = false;
            var mayoralVetoresEnd = false;
            var consentAgendaStart = false;
            var consentAgendaEnd = false;

            for (int i = 0; i < doc.Pages.Count; i++)
            {
                var pdfText = string.Empty;

                var pageBase = doc.Pages[i];
                buffer.Append(pageBase.ExtractText());
                pdfText = buffer.ToString();

                if (pdfText == null) return;

                if (pdfText.Contains("MV - MAYORAL VETOES"))
                {
                    mayoralVetoresStart = true;
                }

                if (pdfText.Contains("END OF MAYORAL VETOES"))
                {
                    mayoralVetoresEnd = true;
                }

                if (pdfText.Contains("CA - CONSENT AGENDA"))
                {
                    consentAgendaStart = true;
                }

                if (consentAgendaStart)
                {
                    GetConsentAgendaResult(doc.Pages, i);
                }

                if (pdfText.Contains("END OF CONSENT AGENDA"))
                {
                    consentAgendaEnd = true;
                }

                if (pdfText.Contains("PH - PUBLIC HEARINGS"))
                {
                    
                }

                if (pdfText.Contains("END OF PUBLIC HEARINGS"))
                {

                }

                if (pdfText.Contains("SR - SECOND READING ORDINANCES"))
                {

                }

                if (pdfText.Contains("END OF SECOND READING ORDINANCES"))
                {

                }

                if (pdfText.Contains("FR - FIRST READING ORDINANCES"))
                {

                }

                if (pdfText.Contains("END OF FIRST READING ORDINANCES"))
                {

                }

                if (pdfText.Contains("RE - RESOLUTIONS"))
                {

                }

                if (pdfText.Contains("END OF RESOLUTIONS"))
                {

                }

                if (pdfText.Contains("AC - ATTORNEY-CLIENT SESSION"))
                {

                }

                if (pdfText.Contains("END OF ATTORNEY-CLIENT SESSION"))
                {

                }

                if (pdfText.Contains("BC - BOARDS AND COMMITTEES"))
                {

                }

                if (pdfText.Contains("END OF BOARDS AND COMMITTEES"))
                {

                }

                if (pdfText.Contains("DI - DISCUSSION ITEMS"))
                {

                }

                if (pdfText.Contains("END OF DISCUSSION ITEMS"))
                {

                }

                if (pdfText.Contains("D3 - DISTRICT 3"))
                {

                }

                if (pdfText.Contains("END OF DISTRICT 3"))
                {

                }

                if (pdfText.Contains("FL - FUTURE LEGISLATION"))
                {

                }

                if (pdfText.Contains("END OF FUTURE LEGISLATION"))
                {

                }

                buffer.Clear();
                pageCounter++;
            }

            //foreach (PdfPageBase pageBase in doc.Pages)
            //{
            //    buffer.Append(pageBase.ExtractText());
            //}

            doc.Close();

            //var pdfText = buffer.ToString();
        }

        private static void GetConsentAgendaResult(Spire.Pdf.Widget.PdfPageCollection pages, int consentAgendaPageIndex)
        {
            var pdfText = string.Empty;
            StringBuilder buffer = new StringBuilder();

            var pageBase = pages[consentAgendaPageIndex];
            buffer.Append(pageBase.ExtractText());
            pdfText = buffer.ToString();

            // Length = Next Paragraph title - Current Paragraph title

            var _result = "RESULT:";
            var _mover = "MOVER:";
            var _seconder = "SECONDER:";
            var _ayes = "AYES:";
            var _absent = "ABSENT:";
            var _ca1 = "CA.1";

            var _resolution = "RESOLUTION";
            var _resoltutionHeaderSpace = "RESOLUTION \r\n";
            var _enactmentNumber = "ENACTMENT NUMBER:";
            var _agendaItemEnd = "This matter was ADOPTED on the Consent Agenda.";

            var resultLength = 0;
            var moverLength = 0;
            var seconderLength = 0;
            var ayesLength = 0;
            var absentLength = 0;


            var result = string.Empty;
            var mover = string.Empty;
            var seconder = string.Empty;
            var ayes = string.Empty;
            var absent = string.Empty;

            var resolutionNumber = string.Empty;
            var enactmentNumber = string.Empty;

            if (pdfText.Contains("The following item(s) was Adopted on the Consent Agenda"))
            {
                if (pdfText.Contains(_result))
                {
                    resultLength = pdfText.IndexOf(_mover) - pdfText.IndexOf(_result);
                }
                if (pdfText.Contains(_mover))
                {
                    moverLength = pdfText.IndexOf(_seconder) - pdfText.IndexOf(_mover);
                }
                if (pdfText.Contains(_seconder))
                {
                    seconderLength = pdfText.IndexOf(_ayes) - pdfText.IndexOf(_seconder);
                }
                if (pdfText.Contains(_ayes))
                {
                    ayesLength = pdfText.IndexOf(_absent) - pdfText.IndexOf(_ayes);
                }
                if (pdfText.Contains(_absent))
                {
                    absentLength = pdfText.IndexOf(_ca1) - pdfText.IndexOf(_absent);
                }

                // Value = Index of Value title + Length of title
                result = pdfText.Substring(pdfText.IndexOf(_result) + _result.Length, 30).Trim();
                mover = pdfText.Substring(pdfText.IndexOf(_mover) + _mover.Length, 30).Trim();
                seconder = pdfText.Substring(pdfText.IndexOf(_seconder) + _seconder.Length, 30).Trim();
                ayes = pdfText.Substring(pdfText.IndexOf(_ayes) + _ayes.Length, 30).Trim();
                absent = pdfText.Substring(pdfText.IndexOf(_absent) + _absent.Length, 30).Trim();

                //var ca1_Start = pdfText.IndexOf(_ca1);
                //var o = pdfText.Substring(ca1_Start, 143);

                GetConsentAgendaResolutions(pages, consentAgendaPageIndex);
                
                //if (pdfText.Contains(_agendaItemEnd))
                //{
                //    var t = pdfText.IndexOf(_agendaItemEnd) + _agendaItemEnd.Length;
                //    var u = pdfText
                //}
            }
        }

        private static void GetConsentAgendaResolutions(Spire.Pdf.Widget.PdfPageCollection pages, int consentAgendaPageIndex)
        {
            var _resolution = "RESOLUTION";
            var _endOfConsentAgenda = "END OF CONSENT AGENDA";

            var pdfText = string.Empty;
            StringBuilder buffer = new StringBuilder();

            var pageBase = pages[consentAgendaPageIndex];
            buffer.Append(pageBase.ExtractText());
            pdfText = buffer.ToString();

            var _consentAgendaEnd = "END OF CONSENT AGENDA";

            while (!pdfText.Contains(_consentAgendaEnd))
            {
                GetResolutions(pdfText);
                


                buffer.Clear();
                pageBase = pages[++consentAgendaPageIndex];
                buffer.Append(pageBase.ExtractText());
                pdfText = buffer.ToString();
            }

            var endOfConsentIndex = pdfText.IndexOf(_endOfConsentAgenda);
            var textBeforeEndOfConsent = pdfText.Substring(0, endOfConsentIndex);

            GetResolutions(textBeforeEndOfConsent);
        }

        private static void GetResolutions(string pdfText)
        {
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

                var endOfResolution = pdfText.IndexOf(enactmentNumber) + enactmentNumber.Length;

                pdfText = pdfText.Substring(endOfResolution, pdfText.Length - endOfResolution);
            }
        }

        private static void MiamiAgendaItem()
        {
            // City of Miami Agenda Item

            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(@"C:\Projects\Snapshot-29208 (1).pdf");

            StringBuilder buffer = new StringBuilder();


            foreach (PdfPageBase pageBase in doc.Pages)
            {
                buffer.Append(pageBase.ExtractText());
            }

            doc.Close();

            var pdfText = buffer.ToString();

            var _breakline = "\r\n";
            var _fileId = "File ID:";
            var _date = "Date:";
            var _commMeetDate = "Commission Meeting Date:";
            var _requestingDepartment = "Requesting Department:";
            var _type = "Type:";
            var _subject = "Subject:";
            var _purposeOfItem = "Purpose of Item:";
            var _backgroundOfitem = "Background of Item:";
            var _budgetImpactAnalysis = "Budget Impact Analysis";
            var _totalFiscalImpact = "Total Fiscal Impact:";
            var _reviewedBy = "Reviewed By";

            // Length = Next Paragraph title - Current Paragraph title
            var purposeOfItemLength = pdfText.IndexOf(_backgroundOfitem) - pdfText.IndexOf(_purposeOfItem);
            var backgroundOfItemLength = pdfText.IndexOf(_budgetImpactAnalysis) - pdfText.IndexOf(_backgroundOfitem);
            var budgetImpactAnalysisLength = pdfText.IndexOf(_totalFiscalImpact) - pdfText.IndexOf(_budgetImpactAnalysis);
            var totalFiscalImpactLength = pdfText.IndexOf(_reviewedBy) - pdfText.IndexOf(_totalFiscalImpact);

            // Value = Index of Value title + Length of title
            var fileid = pdfText.Substring(pdfText.IndexOf(_fileId) + _fileId.Length, 6).Replace("#", string.Empty).Trim();
            var date = pdfText.Substring(pdfText.IndexOf(_date) + _date.Length, 12).Trim();
            var commissionMeetingDate = pdfText.Substring(pdfText.IndexOf(_commMeetDate) + _commMeetDate.Length, 12).Trim();
            var requestingDepartment = pdfText.Substring(pdfText.IndexOf(_requestingDepartment) + _requestingDepartment.Length, 35).Trim();
            var type = pdfText.Substring(pdfText.IndexOf(_type) + _type.Length, 30).Trim();
            var subject = pdfText.Substring(pdfText.IndexOf(_subject) + _subject.Length, pdfText.IndexOf(_breakline)).Trim();

            // Paragraph = Index of title + title length, Paragraph length - next title length
            var purposeOfItem = pdfText.Substring(pdfText.IndexOf(_purposeOfItem) + _purposeOfItem.Length, purposeOfItemLength - _backgroundOfitem.Length).Replace(_breakline, string.Empty).Replace("                    ", " ").Trim();
            var backgroundOfItem = pdfText.Substring(pdfText.IndexOf(_backgroundOfitem) + _backgroundOfitem.Length, backgroundOfItemLength - _budgetImpactAnalysis.Length).Replace(_breakline, string.Empty).Replace("                    ", " ").Trim();
            var budgetImpactAnalysis = pdfText.Substring(pdfText.IndexOf(_budgetImpactAnalysis) + _budgetImpactAnalysis.Length, budgetImpactAnalysisLength - _totalFiscalImpact.Length).Replace(_breakline, string.Empty).Replace("                    ", " ").Replace("To", string.Empty).Trim();
            var totalFiscalImpact = pdfText.Substring(pdfText.IndexOf(_totalFiscalImpact) + _totalFiscalImpact.Length, totalFiscalImpactLength - _reviewedBy.Length).Replace(_breakline, string.Empty).Replace("                    ", " ").Replace("Reviewed", string.Empty).Trim();
        }
    }
}
