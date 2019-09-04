using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfParser
{
    public class MayoralVetoes : Base
    {
        public bool HasVetoes { get; set; }
        private string _discussionItem = string.Empty;
        private string _discussionItemHeaderSpace = string.Empty;
        private string _cityOfMiami = "City of Miami";// Problematic because "City of Miami" may exist in resolution body
        private string _textToRemove = "Evaluation Warning : The document was created with Spire.PDF for .NET.";
        private string _textToRemove2 = $"City Commission                                          Marked Agenda                                            January 10, 2019";
        private string _start = "MV - MAYORAL VETOES";
        private string _end = "END OF MAYORAL VETOES";

        public MayoralVetoes(PdfPageCollection pages, int mayoralVetoPageIndex)
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
