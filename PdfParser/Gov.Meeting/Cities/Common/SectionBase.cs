using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Meeting.Cities.Common
{
    public class SectionBase
    {
        internal int _index { get; set; } = 0;
        public int currentPageNumber { get; set; } = 0;
        public int newPageNumber { get; set; } = 0;
        internal string _textBackUp { get; set; }
        internal string _ { get; set; }
        internal StringBuilder _buffer { get; set; } = new StringBuilder();
        internal PdfPageBase _pageBase { get; set; }
        internal Spire.Pdf.Widget.PdfPageCollection _pages { get; set; }

        internal string _result => "RESULT:";
        internal string _mover => "MOVER:";
        internal string _seconder => "SECONDER:";
        internal string _ayes => "AYES:";
        internal string _absent => "ABSENT:";
        internal string _motionTo = "MOTION TO:";
    }
}
