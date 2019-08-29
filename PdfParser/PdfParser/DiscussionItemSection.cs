using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfParser
{
    public class DiscussionItemSection : Base
    {
        private string _discussionItem = "ATTORNEY-CLIENT SESSION";
        private string _discussionItemHeaderSpace = "DISCUSSION ITEM \r\n";
        private string _cityOfMiami = "City of Miami";// Problematic because "City of Miami" may exist in resolution body
        private string _textToRemove = "Evaluation Warning : The document was created with Spire.PDF for .NET.";
        private string _textToRemove2 = "City Commission                                          Marked Agenda                                            ";
        private string _start = "DI - DISCUSSION ITEMS";
        private string _end = "END OF DISCUSSION ITEMS";

        public List<DiscussionItem> DiscussionItems { get; set; } = new List<DiscussionItem>();

        public DiscussionItemSection(PdfPageCollection pages, int index, out int outIndex)
        {
            _index = index;
            _pages = pages;
            _pageBase = pages[_index];
            _buffer.Append(_pageBase.ExtractText());
            _ = _buffer.ToString();

            // If Section is only one page
            if (_.Contains(_start) && _.Contains(_end))
            {
                LoadDiscussionItems(singlePage: true);
            }
        }

        private void LoadDiscussionItems(bool singlePage = false)
        {
            var indexOfItem = 0;
            var counter = 1;
            var agendaReadingNumber = "DI.";
            var startOfResolution = $"{agendaReadingNumber}{counter.ToString()}                         ATTORNEY-CLIENT SESSION";
            var oldStartOfResolution = string.Empty;

            // Get Page #
            var pageFooterTerm = "City of Miami                                                 Page ";
            var pageFooterIndex = _.IndexOf(pageFooterTerm) + pageFooterTerm.Length;
            var pageNumber = _.Substring(pageFooterIndex, 2);
            currentPageNumber = Int32.Parse(pageNumber);

            if (!singlePage)
            {
                // Paragraph = Index of title + title length, Paragraph length - next title length

                // While text contains item
                while (_.Contains(startOfResolution))
                {
                    // Declare variables
                    var itemNumber = string.Empty;
                    var motionTo = string.Empty;
                    var result = string.Empty;
                    var movers = new List<string>();
                    var seconders = new List<string>();
                    var ayes = new List<string>();
                    var absent = new List<string>();
                    var enactmentNumber = string.Empty;
                    var itemBodyLength = 0;
                    var itemBody = string.Empty;

                    oldStartOfResolution = startOfResolution;

                    indexOfItem = _.IndexOf(_discussionItem);
                    // Item # is from title of item plus a certain number of spaces, the length of the 
                    // Item # should be 4 characters
                    itemNumber = _discussionItem = _.Substring(indexOfItem, 40).Replace(_attorneyClientSessionHeaderSpace, string.Empty).Trim();

                    // Body length should be from startOfResolution to MotionTo: minus certain characters
                    // or it there is a consistent ". " space after the period.
                    //itemBodyLength = (_.IndexOf(_cityOfMiami) - _cityOfMiami.Length) - _.IndexOf(_resolution);
                    itemBody = _.Substring(_.IndexOf(startOfResolution), (_.IndexOf(_motionTo) - 1));

                    if (_.Contains(_motionTo))
                    {
                        // Clear resolution
                        _ = _.Remove(0, _.IndexOf(_motionTo));

                        // Get vote info
                        motionTo = _.Substring(_.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                        result = _.Substring(_.IndexOf(_result) + _result.Length, 40).Trim();
                        movers.Add(_.Substring(_.IndexOf(_mover) + _mover.Length, 50).Trim());
                        seconders.Add(_.Substring(_.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                        ayes.AddRange(_.Substring(_.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                        absent.AddRange(_.Substring(_.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());
                    }
                    else
                    {
                        // Clear resolution
                        throw new Exception("Find way to clear resolution");

                        // Get vote info
                        //motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                        //result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 40).Trim();
                        //movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 50).Trim());
                        //seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                        //ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                        //absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());
                    }

                    // Add Item
                    DiscussionItems.Add(new DiscussionItem
                    {
                        ItemNumber = itemNumber,
                        EnactmentNumber = enactmentNumber,
                        Body = itemBody,
                        MotionTo = motionTo,
                        Result = result,
                        Movers = movers,
                        Seconders = seconders,
                        Ayes = ayes,
                        Absent = absent
                    });

                    // Remove votes and check for end of section and break
                    _ = _.Remove(0, _.IndexOf(_absent) + 40);

                    // Maybe be pre-mature to break here, what if there is another Item?
                    if (_.Contains(_end))
                    {
                        break;
                    }


                    counter++;
                    startOfResolution = $"{agendaReadingNumber}{counter.ToString()}                         ATTORNEY-CLIENT SESSION";

                    if (_.Contains(startOfResolution))
                    {
                        continue;
                    }
                    else
                    {
                        // Next page
                        _buffer.Clear();
                        _pageBase = _pages[++_index];
                        _buffer.Append(_pageBase.ExtractText());
                        _ = _buffer.ToString();

                        // Get Page #
                        pageFooterIndex = _.IndexOf(pageFooterTerm) + pageFooterTerm.Length;
                        pageNumber = _.Substring(pageFooterIndex, 2);
                        newPageNumber = Int32.Parse(pageNumber);

                        // If startOfItem doesn't match && page # is different && end of section doesn't exist
                        // Possible intential duplicate item.
                        if (newPageNumber > currentPageNumber && _.Contains(oldStartOfResolution))
                        {
                            LoadDiscussionItems(false);
                        }
                    }
                }
            }
        }
    }

    public class DiscussionItem
    {
        public string ItemNumber { get; set; }
        public string Body { get; set; }
        public string EnactmentNumber { get; set; }

        public string MotionTo { get; set; }
        public string Result { get; set; }
        public List<string> Movers { get; set; }
        public List<string> Seconders { get; set; }
        public List<string> Ayes { get; set; }
        public List<string> Absent { get; set; }

        public DiscussionItem()
        {
            Movers = new List<string>();
            Seconders = new List<string>();
            Ayes = new List<string>();
            Absent = new List<string>();
        }
    }
}
