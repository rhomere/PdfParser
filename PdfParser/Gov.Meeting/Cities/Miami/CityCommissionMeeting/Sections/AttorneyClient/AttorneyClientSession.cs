using Gov.Meeting.Cities.Common;
using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.AttorneyClient
{
    public class AttorneyClientSession : CityBase
    {
        #region Private Properties
        private string _resolution = "ATTORNEY-CLIENT SESSION";
        private string _resolutionHeaderSpace = "ATTORNEY-CLIENT SESSION \r\n";
        private string _textToRemove = "Evaluation Warning : The document was created with Spire.PDF for .NET.";
        private string _textToRemove2 = $"City Commission                                          Marked Agenda                                            March 14, 2019";
        private string _pageFooterTerm = "City of Miami                                                 Page ";
        private string _end = "END OF ATTORNEY-CLIENT SESSION";
        #endregion

        public List<AttorneyClientSessionItem> AttorneyClientSessionItems { get; set; } = new List<AttorneyClientSessionItem>();

        public AttorneyClientSession(PdfPageCollection pages, int index, out int outIndex)
        {
            _index = index;
            _pages = pages;
            _pageBase = pages[_index];
            _buffer.Append(_pageBase.ExtractText());
            _ = _buffer.ToString();

            LoadAttorneyClientSessionItems();

            outIndex = _index;
        }

        private void LoadAttorneyClientSessionItems()
        {
            var indexOfItem = 0;
            var counter = 1;
            var sectionItemNumber = "AC.";
            var startOfResolution = $"{sectionItemNumber}{counter.ToString()}                         ATTORNEY-CLIENT SESSION";
            var oldStartOfResolution = string.Empty;
            var currentPageNumber = 0;
            var oldPageNumber = 0;

            while (_.Contains(startOfResolution) || (currentPageNumber > oldPageNumber))
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

                if (currentPageNumber > oldPageNumber)
                {
                    startOfResolution = oldStartOfResolution;
                }

                // Get Page #
                currentPageNumber = GetPageNumber();

                oldStartOfResolution = startOfResolution;
                oldPageNumber = currentPageNumber;

                // Clear everything up to StartOfResolution
                _ = _.Remove(0, _.IndexOf(startOfResolution));

                indexOfItem = _.IndexOf(_resolution);
                // Item # is from title of item plus a certain number of spaces, the length of the 
                // Item # should be 4 characters
                itemNumber = _.Substring(indexOfItem, 105).Replace(_resolutionHeaderSpace, string.Empty).Trim();

                // Check for next item
                if (_.Contains(GetItemHeader(sectionItemNumber, counter + 1)))
                {
                    // Store text in _textBackUp;
                    _textBackUp = _;

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
                    itemBodyLength = (_.IndexOf(_pageFooterTerm) - 1) - _.IndexOf(startOfResolution);
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
                        // Store text in _textBackUp;
                        _textBackUp = _;

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
                    //motionTo = _textBackUp.Substring(_textBackUp.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                    //result = _textBackUp.Substring(_textBackUp.IndexOf(_result) + _result.Length, 40).Trim();
                    //movers.Add(_textBackUp.Substring(_textBackUp.IndexOf(_mover) + _mover.Length, 50).Trim());
                    //seconders.Add(_textBackUp.Substring(_textBackUp.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                    //ayes.AddRange(_textBackUp.Substring(_textBackUp.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                    //absent.AddRange(_textBackUp.Substring(_textBackUp.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());
                }

                // Increment counter and check for next
                counter++;
                if (counter < 10)
                {
                    startOfResolution = $"{sectionItemNumber}{counter.ToString()}                         ATTORNEY-CLIENT SESSION";
                }
                else
                {
                    startOfResolution = $"{sectionItemNumber}{counter.ToString()}                        ATTORNEY-CLIENT SESSION";
                }

                // Add Item
                AttorneyClientSessionItems.Add(new AttorneyClientSessionItem
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

                // Get rest of page that was stored in _textBackUp
                if (!string.IsNullOrWhiteSpace(_textBackUp))
                {
                    // Get pdfText that was stored earlier
                    _ = _textBackUp;

                    // If contains oldStartResolution, remove it
                    //if (_.Contains(oldStartOfResolution) && _.Contains(startOfResolution))
                    //{
                    //    _ = _.Remove(0, _.IndexOf(startOfResolution));
                    //}

                    // Correction to the above logic
                    // Remove everything prior to current resolution (startOfResolution)
                    _ = _.Remove(0, _.IndexOf(startOfResolution));

                    _textBackUp = null;
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
                    currentPageNumber = GetPageNumber();
                }
            }
        }

        private int GetPageNumber()
        {
            // Get Page #
            var pageFooterTerm = "City of Miami                                                 Page ";
            var pageFooterIndex = _.IndexOf(pageFooterTerm) + pageFooterTerm.Length;
            var pageNumber = _.Substring(pageFooterIndex, 2);
            return Int32.Parse(pageNumber);
        }

        private string GetItemHeader(string sectionItemNumber, int counter)
        {
            if (counter < 10)
            {
                return $"{sectionItemNumber}{counter.ToString()}                         ATTORNEY-CLIENT SESSION";
            }
            else
            {
                return $"{sectionItemNumber}{counter.ToString()}                        ATTORNEY-CLIENT SESSION";
            }
        }
    }
}
