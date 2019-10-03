using Gov.Meeting.Cities.Common;
using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.MayoralVetoe
{
    public class MayoralVetoe : SectionBase
    {
        // Todo, Complete Mayoral Vetoes, Add Item class if applicable
        public bool HasVetoes { get; set; }
        private string _discussionItem = string.Empty;
        private string _discussionItemHeaderSpace = string.Empty;

        public MayoralVetoe(PdfPageCollection pages, int mayoralVetoPageIndex)
        {
            _pages = pages;
            _index = mayoralVetoPageIndex;
            _pageBase = pages[mayoralVetoPageIndex];
            _buffer.Append(_pageBase.ExtractText());
            _ = _buffer.ToString();

            LoadMayoralVetoes();
        }

        private void LoadMayoralVetoes()
        {
            if (_.Contains("NO MAYORAL VETOES"))
            {
                HasVetoes = false;
            }
        }
    }
}
