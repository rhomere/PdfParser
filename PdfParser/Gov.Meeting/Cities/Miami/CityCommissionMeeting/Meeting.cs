using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.AttorneyClient;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.BoardsAndCommittee;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.ConsentAgenda;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.Discussion;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.District;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.FutureLegislation;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.MayoralVetoe;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.PublicHearing;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.Reading;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.Resolution;
using Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.SecondReading;
using Spire.Pdf;
using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Meeting.Cities.Miami.CityCommissionMeeting
{
    public class Meeting
    {
        #region Properties
        public ConsentAgenda ConsentAgenda { get; set; }
        public MayoralVetoe MayoralVetoe { get; set; }
        public PublicHearing PublicHearing { get; set; }
        public SecondReading SecondReading { get; set; }
        public Reading FirstReading { get; set; }
        public Resolution Resolution { get; set; }
        public AttorneyClientSession AttorneyClientSession { get; set; }
        public BoardAndCommittee BoardsAndCommittee { get; set; }
        public Discussion Discussion { get; set; }
        public District District3 { get; set; }
        public FutureLegislation FutureLegislation { get; set; }
        #endregion

        public Meeting()
        {

        }

        public void LoadData(PdfDocument doc)
        {
            // Use to hold text from page
            var buffer = new StringBuilder();

            var pageCounter = 1;

            // Loop through each page in the doc
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
                    MayoralVetoe = GetMayoralVetoe(doc.Pages, i);

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
                    PublicHearing = GetPublicHearing(doc.Pages, i, out i);

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
                    SecondReading = GetSecondReading(doc.Pages, i, out i);

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
                    FirstReading = GetFirstReading(doc.Pages, i, out i);

                    pageBase = doc.Pages[i];
                    buffer.Append(pageBase.ExtractText());
                    pdfText = buffer.ToString();

                    //if (pdfText.Contains("RE - Resolution"))
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

                #region Resolution
                // Resolution
                if (pdfText.Contains("RE - Resolution") || pdfText.Contains("RE - RESOLUTION"))
                {
                    Resolution = GetResolution(doc.Pages, i, out i);

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
                    Discussion = GetDiscussionItems(doc.Pages, i, out i);

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
                    District3 = GetDisctrict3(doc.Pages, i, out i);

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
                    FutureLegislation = GetFutureLegislation(doc.Pages, i, out i);

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
                // Non Agenda Items contain Discussion Items, Resolution 
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
        private static FutureLegislation GetFutureLegislation(PdfPageCollection pages, int index, out int outIndex)
        {
            return new FutureLegislation(pages, index, out outIndex);
        }

        private static District GetDisctrict3(PdfPageCollection pages, int index, out int outIndex)
        {
            return new District(pages, index, out outIndex);
        }

        private static Discussion GetDiscussionItems(PdfPageCollection pages, int index, out int outIndex)
        {
            return new Discussion(pages, index, out outIndex);
        }

        private static BoardAndCommittee GetMiamiMeetingMinutes(PdfPageCollection pages, int index, out int outIndex)
        {
            return new BoardAndCommittee(pages, index, out outIndex);
        }

        private static AttorneyClientSession GetAttorneyClientSession(PdfPageCollection pages, int index, out int outIndex)
        {
            return new AttorneyClientSession(pages, index, out outIndex);
        }

        private static Resolution GetResolution(PdfPageCollection pages, int index, out int outIndex)
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

        private static PublicHearing GetPublicHearing(PdfPageCollection pages, int index, out int outIndex)
        {
            return new PublicHearing(pages, index, out outIndex);
        }

        private static MayoralVetoe GetMayoralVetoe(PdfPageCollection pages, int mayoralVetoPageIndex)
        {
            return new MayoralVetoe(pages, mayoralVetoPageIndex);
        }

        private static ConsentAgenda GetConsentAgendaResult(Spire.Pdf.Widget.PdfPageCollection pages, int consentAgendaPageIndex, out int outIndex)
        {
            return new ConsentAgenda(pages, consentAgendaPageIndex, out outIndex);
        }
        #endregion
    }
}
