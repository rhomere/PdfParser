﻿using Gov.Meeting.Cities.Common;
using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Meeting.Cities.Miami.CityCommissionMeeting.Sections.PublicHearing
{
    public class PublicHearing : SectionBase
    {
        #region Private Properties
        private string _resolution = "RESOLUTION";
        private string _resolutionHeaderSpace = "RESOLUTION \r\n";
        private string _enactmentNumber = "ENACTMENT NUMBER:";
        private string _cityOfMiami = "City of Miami";// Problematic because "City of Miami" may exist in resolution body
        private string _textToRemove = "Evaluation Warning : The document was created with Spire.PDF for .NET.";
        // Update Date per form
        private string _textToRemove2 = $"City Commission                                          Marked Agenda                                            March 14, 2019";
        private string _start = "PH - PUBLIC HEARINGS";
        private string _end = "END OF PUBLIC HEARINGS";
        #endregion

        public List<PublicHearingResolution> PublicHearingResolutions { get; set; } = new List<PublicHearingResolution>();

        public PublicHearing(PdfPageCollection pages, int publicHearingsIndex, out int outIndex)
        {
            _index = publicHearingsIndex;
            _pages = pages;
            _pageBase = pages[_index];
            _buffer.Append(_pageBase.ExtractText());
            _ = _buffer.ToString();

            LoadResolutions(singlePage: true);

            outIndex = _index;
        }

        private void LoadResolutions(bool singlePage)
        {
            #region Initialize local variables
            var indexOfItem = 0;
            var counter = 1;
            var sectionItemNumber = "PH.";
            var startOfNextResolution = $"{sectionItemNumber}{counter.ToString()}                         RESOLUTION";
            var previousResolution = string.Empty;
            #endregion

            #region Get Page #
            // Get Page #
            var pageFooterTerm = "City of Miami                                                 Page ";
            var pageFooterIndex = _.IndexOf(pageFooterTerm) + pageFooterTerm.Length;
            var pageNumber = _.Substring(pageFooterIndex, 2);
            currentPageNumber = Int32.Parse(pageNumber);
            #endregion

            // While text contains item
            while (_.Contains(startOfNextResolution))
            {
                #region Delcare Variables
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
                #endregion

                previousResolution = startOfNextResolution;

                // Clear everything prior to startOfNextResolution
                _ = _.Remove(0, _.IndexOf(startOfNextResolution));

                #region Get item Number
                indexOfItem = _.IndexOf(_resolution);
                // Item # is from title of item plus a certain number of spaces, the length of the 
                // Item # should be 4 characters
                itemNumber = _.Substring(indexOfItem, 40).Replace(_resolutionHeaderSpace, string.Empty).Trim();
                #endregion

                #region Remove Any Other Items, to avoid any confusion of votes
                // Check for next item
                if (_.Contains(GetItemHeader(sectionItemNumber, counter + 1)))
                {
                    // Store text in _textBackUp;
                    _textBackUp = _;

                    // Remove the next item because the votes were getting mistaken for this one.
                    _ = _.Substring(0, _.IndexOf(GetItemHeader(sectionItemNumber, counter + 1)));

                }
                #endregion

                #region Get Item Body
                // Body length should be from startOfNextResolution to MotionTo: minus certain characters
                // or it there is a consistent ". " space after the period.
                //itemBodyLength = (_.IndexOf(_cityOfMiami) - _cityOfMiami.Length) - _.IndexOf(_resolution);
                if (_.Contains(_motionTo))
                {
                    itemBodyLength = (_.IndexOf(_motionTo) - 1) - _.IndexOf(startOfNextResolution);
                    itemBody = _.Substring(_.IndexOf(startOfNextResolution), itemBodyLength).TrimEnd();
                }
                else if (_.Contains(_result))
                {
                    itemBodyLength = (_.IndexOf(_result) - 1) - _.IndexOf(startOfNextResolution);
                    itemBody = _.Substring(_.IndexOf(startOfNextResolution), itemBodyLength).TrimEnd();
                }
                // Else the whole page is part of the itemBody
                // Get index of bottom line minus 1
                // Itembody = startOfNextResolution to index of bottom line
                else
                {
                    itemBodyLength = (_.IndexOf(pageFooterTerm) - 1) - _.IndexOf(startOfNextResolution);
                    itemBody = _.Substring(_.IndexOf(startOfNextResolution), itemBodyLength).TrimEnd();

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
                #endregion

                #region Get Votes
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
                #endregion

                #region Increment counter to check for next item
                // Increment counter to check for next item
                counter++;

                // When the item increments to double digits it looses a space and it no 
                // longer similar to startOfNextResolution
                if (counter < 10)
                {
                    startOfNextResolution = $"{sectionItemNumber}{counter.ToString()}                         RESOLUTION";
                }
                else
                {
                    startOfNextResolution = $"{sectionItemNumber}{counter.ToString()}                        RESOLUTION";
                }
                #endregion

                #region Add item
                // Add Item
                PublicHearingResolutions.Add(new PublicHearingResolution
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
                #endregion

                #region Get rest of page that was stored in _textBackUp
                // Get rest of page that was stored in _textBackUp
                if (!string.IsNullOrWhiteSpace(_textBackUp))
                {
                    // Get pdfText that was stored earlier
                    _ = _textBackUp;

                    // Correction to the above logic
                    // Remove everything prior to current resolution (startOfNextResolution)
                    _ = _.Remove(0, _.IndexOf(startOfNextResolution));

                    _textBackUp = null;
                }
                #endregion

                if (_.Contains(startOfNextResolution))
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
                return $"{sectionItemNumber}{counter.ToString()}                         RESOLUTION";
            }
            else
            {
                return $"{sectionItemNumber}{counter.ToString()}                        RESOLUTION";
            }
        }
    }
}
