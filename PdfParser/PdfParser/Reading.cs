using Spire.Pdf;
using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfParser
{
    public class Reading : Base
    {
        private string _resolution = "ORDINANCE";
        private string _resolutionHeaderSpace = "ORDINANCE                                                        First Reading \r\n";
        private string _enactmentNumber = "ENACTMENT NUMBER:";
        private string _cityOfMiami = "City of Miami";// Problematic because "City of Miami" may exist in resolution body
        private string _textToRemove = "Evaluation Warning : The document was created with Spire.PDF for .NET.";
        private string _textToRemove2 = $"City Commission                                          Marked Agenda                                            January 10, 2019";
        private string _start = "FR - FIRST READING ORDINANCES";
        private string _end = "END OF FIRST READING ORDINANCES";
        private bool _splitPage { get; set; }

        public List<ReadingOrdinance> ReadingOrdinances { get; set; } = new List<ReadingOrdinance>();

        public Reading(PdfPageCollection pages, int index, out int outIndex)
        {
            _index = index;
            _pages = pages;
            _pageBase = pages[_index];
            _buffer.Append(_pageBase.ExtractText());
            _ = _buffer.ToString();
            // Pass in FIRST OR SECOND

            LoadOrdinances();

            outIndex = _index;
        }

        private void LoadOrdinances(bool singlePage = false)
        {
            var indexOfItem = 0;
            var counter = 1;
            var sectionItemNumber = "FR.";
            // Get proper spacing
            var startOfResolution = $"{sectionItemNumber}{counter.ToString()}                         ORDINANCE";
            var oldStartOfResolution = string.Empty;

            // Get Page #
            var pageFooterTerm = "City of Miami                                                 Page ";
            var pageFooterIndex = _.IndexOf(pageFooterTerm) + pageFooterTerm.Length;
            var pageNumber = _.Substring(pageFooterIndex, 2);
            currentPageNumber = Int32.Parse(pageNumber);

            while (_.Contains(startOfResolution))
            {
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

                // Clear everything StartOfResolution
                _ = _.Remove(0, _.IndexOf(startOfResolution));

                indexOfItem = _.IndexOf(_resolution);
                // Item # is from title of item plus a certain number of spaces, the length of the 
                // Item # should be 4 characters
                var t = _.Substring(indexOfItem, 105);
                itemNumber = _.Substring(indexOfItem, 105).Replace(_resolutionHeaderSpace, string.Empty).Trim();

                // Check for next item
                if (_.Contains(GetItemHeader(sectionItemNumber, counter + 1)))
                {
                    // Store text in _pdfText;
                    _pdfText = _;

                    // Remove the next item because the votes were getting mistaken for this one.
                    _ = _.Substring(0, _.IndexOf(GetItemHeader(sectionItemNumber, counter + 1)));

                }

                // Body length should be from startOfResolution to MotionTo: minus certain characters
                // or it there is a consistent ". " space after the period.
                //itemBodyLength = (_.IndexOf(_cityOfMiami) - _cityOfMiami.Length) - _.IndexOf(_resolution);

                if (_.Contains(_motionTo))
                {
                    itemBodyLength = (_.IndexOf(_motionTo) - 1) - _.IndexOf(startOfResolution);
                    itemBody = _.Substring(_.IndexOf(startOfResolution), itemBodyLength).TrimEnd();
                }
                else if (_.Contains(_result))
                {
                    itemBodyLength = (_.IndexOf(_result) - 1) - _.IndexOf(startOfResolution);
                    itemBody = _.Substring(_.IndexOf(startOfResolution), itemBodyLength).TrimEnd();
                }
                // Else the whole page is part of the itemBody
                // Get index of bottom line minus 1
                // Itembody = startOfResolution to index of bottom line
                else
                {
                    itemBodyLength = (_.IndexOf(pageFooterTerm) - 1) - _.IndexOf(startOfResolution);
                    itemBody = _.Substring(_.IndexOf(startOfResolution), itemBodyLength).TrimEnd();

                    //Increment page and continue
                    _buffer.Clear();
                    _pageBase = _pages[++_index];
                    _buffer.Append(_pageBase.ExtractText());
                    _ = _buffer.ToString();

                    // If it contains the next resolution, remove everything from the beginning of 
                    // the next resolution
                    // Check for next item
                    if (_.Contains(GetItemHeader(sectionItemNumber, counter + 1)))
                    {
                        // Store text in _pdfText;
                        _pdfText = _;

                        // Remove the next item because the votes were getting mistaken for this one.
                        _ = _.Substring(0, _.IndexOf(GetItemHeader(sectionItemNumber, counter + 1)));

                    }

                    // Clear any misc text
                    _ = _.Replace(_textToRemove, string.Empty);
                    _ = _.Replace(_textToRemove2, string.Empty);
                    _ = _.TrimStart();

                    // If contains motionTo
                    // Add everything from 0 to start
                    if (_.Contains(_motionTo))
                    {
                        itemBody += " " + _.Substring(0, _.IndexOf(_motionTo));
                    }

                    // else if contains result
                    // Add everything from 0 to start 
                    else if (_.Contains(_result))
                    {
                        itemBody = " " + _.Substring(0, _.IndexOf(_result));
                    }

                    // Continue on to votes
                }


                if (_.Contains(_motionTo))
                {
                    // Clear resolution
                    _ = _.Remove(0, _.IndexOf(_motionTo));

                    // Get vote info
                    motionTo = _.Substring(_.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                    result = _.Substring(_.IndexOf(_result) + _result.Length, 40).Trim();
                    movers.Add(_.Substring(_.IndexOf(_mover) + _mover.Length, 50).Trim());
                    seconders.Add(_.Substring(_.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                    ayes.AddRange(_.Substring(_.IndexOf(_ayes) + _ayes.Length, 60).Trim().Split(',').ToList());

                    if (_.Contains(_absent))
                    {
                        absent.AddRange(_.Substring(_.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());
                    }
                }
                else if (_.Contains(_result))
                {
                    result = _.Substring(_.IndexOf(_result) + _result.Length, 40).Trim();

                    // Remove result
                    _ = _.Remove(0, _.IndexOf(_result) + 40);
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

                // Increment counter and check for next
                counter++;
                if (counter < 10)
                {
                    startOfResolution = $"{sectionItemNumber}{counter.ToString()}                         ORDINANCE";
                }
                else
                {
                    startOfResolution = $"{sectionItemNumber}{counter.ToString()}                        ORDINANCE";
                }

                // Add Item
                ReadingOrdinances.Add(new ReadingOrdinance
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

                // Get rest of page that was stored in _pdfText
                if (!string.IsNullOrWhiteSpace(_pdfText))
                {
                    // Get pdfText that was stored earlier
                    _ = _pdfText;

                    // If contains oldStartResolution, remove it
                    //if (_.Contains(oldStartOfResolution) && _.Contains(startOfResolution))
                    //{
                    //    _ = _.Remove(0, _.IndexOf(startOfResolution));
                    //}

                    // Correction to the above logic
                    // Remove everything prior to current resolution (startOfResolution)
                    _ = _.Remove(0, _.IndexOf(startOfResolution));

                    _pdfText = null;
                }

                // When the item increments to double digits it looses a space and it no 
                // longer similar to startOfResolution
                if (_.Contains(startOfResolution))
                {
                    continue;
                }
                else if (_.Contains(_end))
                {
                    break;
                }
                else
                {
                    //Increment page and continue
                    _buffer.Clear();
                    _pageBase = _pages[++_index];
                    _buffer.Append(_pageBase.ExtractText());
                    _ = _buffer.ToString();
                }
            }
        }

        private string GetItemHeader(string sectionItemNumber, int counter)
        {
            if (counter < 10)
            {
                return $"{sectionItemNumber}{counter.ToString()}                         ORDINANCE";
            }
            else
            {
                return $"{sectionItemNumber}{counter.ToString()}                        ORDINANCE";
            }
        }
    }

    public class ReadingOrdinance
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

        public ReadingOrdinance()
        {
            Movers = new List<string>();
            Seconders = new List<string>();
            Ayes = new List<string>();
            Absent = new List<string>();
        }
    }
}
