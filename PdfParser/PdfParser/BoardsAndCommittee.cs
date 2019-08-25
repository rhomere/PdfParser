﻿using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfParser
{
    public class BoardsAndCommittee : Base
    {
        private string _resolution = "RESOLUTION";
        private string _resoltutionHeaderSpace = "RESOLUTION \r\n";
        private string _enactmentNumber = "ENACTMENT NUMBER:";
        private string _cityOfMiami = "City of Miami";// Problematic because "City of Miami" may exist in resolution body
        private string _textToRemove = "Evaluation Warning : The document was created with Spire.PDF for .NET.";
        private string _textToRemove2 = "City Commission                                          Marked Agenda                                            ";
        private string _start = "BC - BOARDS AND COMMITTEES";
        private string _end = "END OF BOARDS AND COMMITTEES";
        private string _appointees = "APPOINTEES:";
        private string _appointee = "APPOINTEE:";
        private string _nominatedBy = "NOMINATED BY:";

        public List<BoardsAndCommitteeItem> BoardsAndCommitteeItems { get; set; } = new List<BoardsAndCommitteeItem>();

        public BoardsAndCommittee(PdfPageCollection pages, int index, out int outIndex)
        {
            _index = index;
            _pages = pages;
            _pageBase = pages[_index];
            _buffer.Append(_pageBase.ExtractText());
            _ = _buffer.ToString();

            // If Section is only one page
            if (_.Contains(_start) && _.Contains(_end))
            {
                LoadBoardAndCommitteeItems(singlePage: true);
            }

            while (!_.Contains(_end))
            {
                LoadBoardAndCommitteeItems();
            }

            LoadBoardAndCommitteeItems();

            outIndex = _index;
        }

        private void LoadBoardAndCommitteeItems(bool singlePage = false)
        {
            var indexOfItem = 0;
            var counter = 1;
            var sectionItemNumber = "BC.";
            // Will probably have to get the exact text for below
            var startOfResolution = $"{sectionItemNumber}{counter.ToString()}                         RESOLUTION";
            var oldStartOfResolution = string.Empty;

            // Get Page #
            var pageFooterTerm = "City of Miami                                                 Page ";
            var pageFooterIndex = _.IndexOf(pageFooterTerm) + pageFooterTerm.Length;
            var pageNumber = _.Substring(pageFooterIndex, 2);
            currentPageNumber = Int32.Parse(pageNumber);

            if (!singlePage)
            {
                // While text contains item
                while (_.Contains(startOfResolution))
                {
                    // Declare variables
                    var motionTo = string.Empty;
                    var result = string.Empty;
                    var movers = new List<string>();
                    var seconders = new List<string>();
                    var ayes = new List<string>();
                    var absent = new List<string>();
                    var enactmentNumber = string.Empty;
                    var itemNumber = string.Empty;
                    var itemBodyLength = 0;
                    var itemBody = string.Empty;

                    oldStartOfResolution = startOfResolution;

                    // Remove everything before start of resolution
                    _ = _.Remove(0, _.IndexOf(startOfResolution));

                    indexOfItem = _.IndexOf(_resolution);
                    // Item # is from title of item plus a certain number of spaces, the length of the 
                    // Item # should be 4 characters
                    itemNumber = _.Substring(indexOfItem, 40).Replace(_resoltutionHeaderSpace, string.Empty).Trim();

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
                    try
                    {
                        itemBody = _.Substring(_.IndexOf(startOfResolution), (_.IndexOf(_appointees) - 1));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        itemBody = _.Substring(_.IndexOf(startOfResolution), (_.IndexOf("APPOINTEE") - 1));
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
                        ayes.AddRange(_.Substring(_.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                        absent.AddRange(_.Substring(_.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());
                    }
                    else if (_.Contains(_result))
                    {
                        result = _.Substring(_.IndexOf(_result) + _result.Length, 40).Trim();

                        // Remove result
                        _ = _.Remove(0, _.IndexOf(_result) + 40);
                    }
                    

                    // Increment counter and check for next
                    counter++;
                    if (counter < 10)
                    {
                        startOfResolution = $"{sectionItemNumber}{counter.ToString()}                         RESOLUTION";
                    }
                    else
                    {
                        startOfResolution = $"{sectionItemNumber}{counter.ToString()}                        RESOLUTION";
                    }

                    // If result is empty, go to next page to get votes
                    if (result == string.Empty)
                    {
                        // Next page
                        _buffer.Clear();
                        _pageBase = _pages[++_index];
                        _buffer.Append(_pageBase.ExtractText());
                        _ = _buffer.ToString();

                        _pdfText = _;

                        // Remove everthing from start of next resolution and on
                        if (_.Contains(startOfResolution))
                        {
                            _ = _.Substring(0, _.IndexOf(startOfResolution));
                        }
                        // Otherwise remove everything from end of section and on
                        else if (_.Contains(_end))
                        {
                            _ = _.Substring(0, _.IndexOf(_end));
                        }

                        // Get votes
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
                        else if (_.Contains(_result))
                        {
                            result = _.Substring(_.IndexOf(_result) + _result.Length, 40).Trim();

                            // Remove result
                            _ = _.Remove(0, _.IndexOf(_result) + 40);
                        }
                    }

                    // Add Item


                    if (!string.IsNullOrWhiteSpace(_pdfText))
                    {
                        // Get pdfText that was stored earlier
                        _ = _pdfText;

                        // If contains oldStartResolution, remove it
                        if (_.Contains(oldStartOfResolution) && _.Contains(startOfResolution))
                        {
                            _ = _.Remove(0, _.IndexOf(startOfResolution));
                        }

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
                            LoadBoardAndCommitteeItems(false);
                        }
                    }
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

    public class BoardsAndCommitteeItem
    {
        private string _resolution => "RESOLUTION";
        private string _result => "RESULT:";
        private string _mover => "MOVER:";
        private string _seconder => "SECONDER:";
        private string _ayes => "AYES:";
        private string _absent => "ABSENT:";

        public string ItemNumber { get; set; }
        public string Body { get; set; }
        public string EnactmentNumber { get; set; }

        public string MotionTo { get; set; }
        public string Result { get; set; }
        public List<string> Movers { get; set; }
        public List<string> Seconders { get; set; }
        public List<string> Ayes { get; set; }
        public List<string> Absent { get; set; }
        public List<(string, string)> AppointeesAndNominees { get; set; } = new List<(string, string)>();


        public BoardsAndCommitteeItem()
        {
            Movers = new List<string>();
            Seconders = new List<string>();
            Ayes = new List<string>();
            Absent = new List<string>();
        }
    }
}
