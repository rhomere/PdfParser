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
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(@"C:\Projects\Snapshot-29208 (1).pdf");

            StringBuilder buffer = new StringBuilder();

            foreach (PdfPageBase pageBase in doc.Pages)
            {
                buffer.Append(pageBase.ExtractText());
            }

            doc.Close();

            var pdfText = buffer.ToString();

            MiamiAgendaItem(pdfText);

            

            Console.ReadLine();
            //save text
            //String fileName = "TextInPdf.txt";
            //File.WriteAllText(fileName, buffer.ToString());

            //System.Diagnostics.Process.Start(fileName);

        }

        private static void MiamiAgendaItem(string pdfText)
        {
            // City of Miami Agenda Item

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
