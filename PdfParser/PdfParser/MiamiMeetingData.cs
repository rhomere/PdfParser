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
        #region Properties
        public ConsentAgenda ConsentAgenda { get; set; } = new ConsentAgenda();
        public MayoralVetoes MayoralVeotes { get; set; }
        public PublicHearings PublicHearings { get; set; }
        public SecondReading SecondReadings { get; set; }
        public Reading FirstReadings { get; set; }
        public Resolution Resolutions { get; set; }
        public AttorneyClientSession AttorneyClientSession { get; set; }
        public BoardsAndCommittee BoardsAndCommittee { get; set; }
        public DiscussionItemSection DiscussionItemSection { get; set; }
        public DistrictSection District3Section { get; set; }
        public FutureLegislationSection FutureLegislationSection { get; set; }
        #endregion

        public MiamiMeetingData()
        {

        }

        public void LoadData(PdfDocument doc)
        {
            var buffer = new StringBuilder();

            var pageCounter = 1;

            for (int i = 0; i < doc.Pages.Count; i++)
            {
                var pdfText = string.Empty;

                var pageBase = doc.Pages[i];
                buffer.Append(pageBase.ExtractText());
                pdfText = buffer.ToString();

                #region Mayoral Vetoes
                // As of 09/01/2019 there has not been a Mayoral Vetoes section with actual vetoes
                // However 08/02/2019 there was a separated pdf with description
                // "Mayor's Office - Item(s) Vetoed by the Mayor"
                if (pdfText.Contains("MV - MAYORAL VETOES") || pdfText.Contains("MV - MAYORAL VETOE"))
                {
                    MayoralVeotes = GetMayoralVetoes(doc.Pages, i);

                    // If text contains next section (Public Hearing) continue
                    // Otherwise increment page count.
                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("CA - CONSENT AGENDA"))
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
                #endregion

                #region Consent Agenda
                // CONSENT AGENDA
                if (pdfText.Contains("CA - CONSENT AGENDA"))
                {
                    ConsentAgenda = GetConsentAgendaResult(doc.Pages, i, out i);

                    // If text contains next section (Public Hearing) continue
                    // Otherwise increment page count.
                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("PH - PUBLIC HEARINGS"))
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
                #endregion

                #region Public Hearings
                // PUBLIC HEARINGS
                if (pdfText.Contains("PH - PUBLIC HEARINGS") || pdfText.Contains("PH - PUBLIC HEARING"))
                {
                    PublicHearings = GetPublicHearing(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("SR - SECOND READING ORDINANCES"))
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
                #endregion

                #region Second Reading Ordinances
                // SECOND READING ORDINANCES
                if (pdfText.Contains("SR - SECOND READING ORDINANCES") || pdfText.Contains("SR - SECOND READING ORDINANCE"))
                {
                    SecondReadings = GetSecondReading(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("FR - FIRST READING ORDINANCES"))
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
                #endregion

                #region First Reading Ordinances
                // FIRST READING ORDINANCES
                if (pdfText.Contains("FR - FIRST READING ORDINANCES") || pdfText.Contains("FR - FIRST READING ORDINANCE"))
                {
                    FirstReadings = GetFirstReading(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("RE - RESOLUTIONS"))
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
                #endregion

                #region Resolutions
                // RESOLUTIONS
                if (pdfText.Contains("RE - RESOLUTIONS") || pdfText.Contains("RE - RESOLUTION"))
                {
                    Resolutions = GetResolutions(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("AC - ATTORNEY-CLIENT SESSION"))
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
                #endregion

                #region Attorney-Client Session
                // ATTORNEY-CLIENT SESSION
                if (pdfText.Contains("AC - ATTORNEY-CLIENT SESSION"))
                {
                    AttorneyClientSession = GetAttorneyClientSession(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("BC - BOARDS AND COMMITTEES"))
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
                #endregion

                #region Boards and Committees
                //BOARDS AND COMMITTEES
                if (pdfText.Contains("BC - BOARDS AND COMMITTEES") || pdfText.Contains("BC - BOARDS AND COMMITTEE"))
                {
                    BoardsAndCommittee = GetMiamiMeetingMinutes(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("DI - DISCUSSION ITEMS"))
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
                #endregion

                #region Discussion Items
                // DISCUSSION ITEMS
                if (pdfText.Contains("DI - DISCUSSION ITEMS") || pdfText.Contains("DI - DISCUSSION ITEM"))
                {
                    DiscussionItemSection = GetDiscussionItems(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("D3 - DISTRICT 3"))
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
                #endregion

                #region District 3
                // DISTRICT 3
                if (pdfText.Contains("D3 - DISTRICT 3"))
                {
                    District3Section = GetDisctrict3Section(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("FL - FUTURE LEGISLATION"))
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
                #endregion

                #region Future Legislation
                // FUTURE LEGISLATION
                if (pdfText.Contains("FL - FUTURE LEGISLATION") || pdfText.Contains("FL - FUTURE LEGISLATIONS"))
                {
                    FutureLegislationSection = GetFutureLegislation(doc.Pages, i, out i);

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
                #endregion

                #region Public Comment Period For Planning and Zoning Items (March Meeting)

                #endregion

                #region Non Agenda Items
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
                #endregion

                buffer.Clear();
                pageCounter++;
            }

            doc.Close();
        }

        #region GetSections
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
        #endregion
    }
}
