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
        //private PdfPageCollection _pages;
        //private int _mayoralVetoPageIndex;
        //private string _pdfText { get; set; }
        //private StringBuilder _buffer { get; set; } = new StringBuilder();
        //private PdfPageBase _pageBase { get; set; }

        public bool HasVetoes { get; set; }


        public MayoralVetoes(PdfPageCollection pages, int mayoralVetoPageIndex)
        {
            _pages = pages;
            _index = mayoralVetoPageIndex;
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
}
