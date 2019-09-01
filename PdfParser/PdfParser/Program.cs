using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf;
using Spire.Pdf.Widget;

namespace PdfParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a pdf document

            var result = MiamiMeetingMinutes();

            

            //MiamiAgendaItem(pdfText);

            

            Console.ReadLine();
            //save text
            //String fileName = "TextInPdf.txt";
            //File.WriteAllText(fileName, buffer.ToString());

            //System.Diagnostics.Process.Start(fileName);

        }

        private static MiamiMeetingData MiamiMeetingMinutes()
        {
            PdfDocument doc = new PdfDocument();
            var miamiMeetingMinutes = new MiamiMeetingData();

            doc.LoadFromFile(@"C:\Users\User\OneDrive\Projects\Regular Meeting\01-10-2019\Meeting.pdf");

            StringBuilder buffer = new StringBuilder();

            var pageCounter = 1;
            var mayoralVetoresStart = false;
            var mayoralVetoresEnd = false;
            var consentAgendaStart = false;
            var consentAgendaEnd = false;
            var publicHearingStart = false;
            var publicHearingEnd = false;
            var secondReadingStart = false;
            var secondReadingEnd = false;

            for (int i = 0; i < doc.Pages.Count; i++)
            {
                var pdfText = string.Empty;

                var pageBase = doc.Pages[i];
                buffer.Append(pageBase.ExtractText());
                pdfText = buffer.ToString();

                // As of 09/01/2019 there has not been a Mayoral Vetoes section with actual vetoes
                // However 08/02/2019 there was a separated pdf with description
                // "Mayor's Office - Item(s) Vetoed by the Mayor"
                if (pdfText.Contains("MV - MAYORAL VETOES"))
                {
                    miamiMeetingMinutes.MayoralVeotes = GetMayoralVetoes(doc.Pages, i);
                    mayoralVetoresEnd = true;

                    // If text contains next section (Public Hearing) continue
                    // Otherwise increment page count.
                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("CA - CONSENT AGENDA"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                // CONSENT AGENDA
                if (pdfText.Contains("CA - CONSENT AGENDA"))
                {
                    miamiMeetingMinutes.ConsentAgenda = GetConsentAgendaResult(doc.Pages, i, out i);
                    consentAgendaEnd = true;

                    // If text contains next section (Public Hearing) continue
                    // Otherwise increment page count.
                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("PH - PUBLIC HEARINGS"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                // PUBLIC HEARINGS
                if (pdfText.Contains("PH - PUBLIC HEARINGS"))
                {
                    miamiMeetingMinutes.PublicHearings = GetPublicHearing(doc.Pages, i, out i);
                    publicHearingEnd = true;

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("SR - SECOND READING ORDINANCES"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                // SECOND READING ORDINANCES
                if (pdfText.Contains("SR - SECOND READING ORDINANCES"))
                {
                    miamiMeetingMinutes.SecondReadings = GetSecondReading(doc.Pages, i, out i);
                    secondReadingEnd = true;

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("FR - FIRST READING ORDINANCES"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                // FIRST READING ORDINANCES
                if (pdfText.Contains("FR - FIRST READING ORDINANCES"))
                {
                    miamiMeetingMinutes.FirstReadings = GetFirstReading(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("RE - RESOLUTIONS"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                // RESOLUTIONS
                if (pdfText.Contains("RE - RESOLUTIONS"))
                {
                    miamiMeetingMinutes.Resolutions = GetResolutions(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("AC - ATTORNEY-CLIENT SESSION"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                // ATTORNEY-CLIENT SESSION
                if (pdfText.Contains("AC - ATTORNEY-CLIENT SESSION"))
                {
                    miamiMeetingMinutes.AttorneyClientSession = GetAttorneyClientSession(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("BC - BOARDS AND COMMITTEES"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                //BOARDS AND COMMITTEES
                if (pdfText.Contains("BC - BOARDS AND COMMITTEES"))
                {
                    miamiMeetingMinutes.BoardsAndCommittee = GetMiamiMeetingMinutes(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("DI - DISCUSSION ITEMS"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                // DISCUSSION ITEMS
                if (pdfText.Contains("DI - DISCUSSION ITEMS"))
                {
                    miamiMeetingMinutes.DiscussionItemSection = GetDiscussionItems(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("D3 - DISTRICT 3"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                // DISTRICT 3
                if (pdfText.Contains("D3 - DISTRICT 3"))
                {
                    miamiMeetingMinutes.District3Section = GetDisctrict3Section(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    if (pdfText.Contains("FL - FUTURE LEGISLATION"))
                    {
                        continue;
                    }
                    else
                    {
                        buffer.Clear();
                        i++;
                        pageBase = doc.Pages[i];
                        buffer.Append(pageBase.ExtractText());
                        pdfText = buffer.ToString();
                    }
                }

                // FUTURE LEGISLATION
                if (pdfText.Contains("FL - FUTURE LEGISLATION"))
                {
                    miamiMeetingMinutes.FutureLegislationSection = GetFutureLegislation(doc.Pages, i, out i);

                    //pageBase = doc.Pages[i];
                    //buffer.Append(pageBase.ExtractText());
                    //pdfText = buffer.ToString();

                    break;
                    //if(pdfText.Contains("NA - NON-AGENDA ITEM(S)"))
                    //{
                    //    continue;
                    //}
                    //else
                    //{
                    //    buffer.Clear();
                    //    i++;
                    //    pageBase = doc.Pages[i];
                    //    buffer.Append(pageBase.ExtractText());
                    //    pdfText = buffer.ToString();
                    //}
                }

                // Non Agenda Items contain Discussion Items, Resolutions 
                // and Attorney Client Sessions
                //if (pdfText.Contains("NA - NON-AGENDA ITEM(S)"))
                //{
                    

                //    pageBase = doc.Pages[i];
                //    buffer.Append(pageBase.ExtractText());
                //    pdfText = buffer.ToString();

                //    if (pdfText.Contains("NA - NON-AGENDA ITEM(S)"))
                //    {
                //        continue;
                //    }
                //    else
                //    {
                //        buffer.Clear();
                //        i++;
                //        pageBase = doc.Pages[i];
                //        buffer.Append(pageBase.ExtractText());
                //        pdfText = buffer.ToString();
                //    }
                //}

                buffer.Clear();
                pageCounter++;
            }

            doc.Close();

            return miamiMeetingMinutes;
        }

        private static FutureLegislationSection GetFutureLegislation(PdfPageCollection pages, int index, out int outIndex)
        {
            return new FutureLegislationSection(pages, index, out outIndex);
        }

        private static DistrictSection GetDisctrict3Section(PdfPageCollection pages, int index, out int outIndex)
        {
            return new DistrictSection(pages, index, out outIndex);
        }

        private static DiscussionItemSection GetDiscussionItems(PdfPageCollection pages, int index, out int outIndex)
        {
            return new DiscussionItemSection(pages, index, out outIndex);
        }

        private static BoardsAndCommittee GetMiamiMeetingMinutes(PdfPageCollection pages, int index, out int outIndex)
        {
            return new BoardsAndCommittee(pages, index, out outIndex);
        }

        private static AttorneyClientSession GetAttorneyClientSession(PdfPageCollection pages, int index, out int outIndex)
        {
            return new AttorneyClientSession(pages, index, out outIndex);
        }

        private static Resolution GetResolutions(PdfPageCollection pages, int index, out int outIndex)
        {
            return new Resolution(pages, index, out outIndex);
        }

        private static Reading GetFirstReading(PdfPageCollection pages, int index, out int outIndex)
        {
            return new Reading(pages, index, out outIndex);
        }

        private static SecondReading GetSecondReading(PdfPageCollection pages, int index, out int outIndex)
        {
            return new SecondReading(pages, index, out outIndex);
        }

        private static PublicHearings GetPublicHearing(PdfPageCollection pages, int index, out int outIndex)
        {
            return new PublicHearings(pages, index, out outIndex);
        }

        private static MayoralVetoes GetMayoralVetoes(PdfPageCollection pages, int mayoralVetoPageIndex)
        {
            return new MayoralVetoes(pages, mayoralVetoPageIndex);
        }

        private static ConsentAgenda GetConsentAgendaResult(Spire.Pdf.Widget.PdfPageCollection pages, int consentAgendaPageIndex, out int outIndex)
        {
            return new ConsentAgenda(pages, consentAgendaPageIndex, out outIndex);
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
