using Spire.Pdf.Widget;
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

            LoadBoardAndCommitteeItems();

            outIndex = _index;
        }

        private void LoadBoardAndCommitteeItems()
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

                var appointeeAndNominators = new List<(string, string)>();

                // Appointees
                var t = _.IndexOf("NOMINATED BY: \r\n") + 18;
                _ = _.Remove(0, t);
                _ = _.Remove(0, _.IndexOf("\r\n") + 4);
                _ = _.TrimStart();

                while (CanParseAppointee(_))
                {
                    var line = _.Substring(0, _.IndexOf("\r\n") + 4);

                    // Loop through this process to get appointee and nominators
                    var appointee = line.Substring(0, line.IndexOf("  "));

                    // If appointee is any of the officials
                    // set appointee to null
                    if (IsAppointeeAnCityOfficial(appointee))
                    {
                        appointee = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(appointee))
                    {
                        line = line.Replace(appointee, string.Empty);
                    }

                    line = line.TrimStart();
                    var nominator = line.Substring(0, line.IndexOf("\r\n"));
                    line = line.Replace(nominator, string.Empty);

                    // Remove everything to the first breakLine (the appointee line we just processed)
                    _ = _.Remove(0, _.IndexOf("\r\n") + 4);

                    // If one breakLine to next official do current logic
                    if (breakLineCount(_) == 1)
                    {
                        var secondLine = _.Substring(0, _.IndexOf("\r\n"));
                        secondLine = secondLine.TrimStart().TrimEnd();

                        if (!string.IsNullOrEmpty(secondLine))
                        {
                            appointee = secondLine;
                        }
                    }
                    else
                    {
                        // Get text from next two breakLines
                        var line1 = _.Substring(0, _.IndexOf("\r\n"));
                        line1 = line1.Trim();

                        var line2 = _.Remove(0, _.IndexOf("\r\n") + 4);
                        line2 = line2.Substring(0, line2.IndexOf("\r\n"));
                        line2 = line2.Trim();

                        appointee = line1 + " " + line2;

                        _ = _.Remove(0, _.IndexOf("\r\n") + 4);
                    }

                    // Remove double space in between appointees
                    _ = _.Remove(0, _.IndexOf("\r\n") + 4);

                    _ = _.TrimStart();

                    appointeeAndNominators.Add((appointee, nominator));
                }

                Votes:
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
                    // Should rarely go in here, when the votes get cut off the page

                    // Next page
                    _buffer.Clear();
                    _pageBase = _pages[++_index];
                    _buffer.Append(_pageBase.ExtractText());
                    _ = _buffer.ToString();

                    // Get index of next resolution
                    counter++;
                    if (counter < 10)
                    {
                        startOfResolution = $"{sectionItemNumber}{counter.ToString()}                         RESOLUTION";
                    }
                    else
                    {
                        startOfResolution = $"{sectionItemNumber}{counter.ToString()}                        RESOLUTION";
                    }

                    //Store text
                    _pdfText = _; 

                    if (_.Contains(startOfResolution))
                    {
                        //Get everything prior to next resolution, i.e. the votes
                        _ = _.Substring(0, _.IndexOf(startOfResolution));

                        // Remove misc text
                        _ = _.Replace(_textToRemove, string.Empty);
                        goto Votes;
                    }

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
                BoardsAndCommitteeItems.Add(new BoardsAndCommitteeItem
                {
                    Body = itemBody,
                    ItemNumber = itemNumber,
                    MotionTo = motionTo,
                    Result = result,
                    Movers = movers,
                    Seconders = seconders,
                    Ayes = ayes,
                    Absent = absent,
                    AppointeesAndNominators = appointeeAndNominators
                });

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
                        LoadBoardAndCommitteeItems();
                    }
                }
            }
        }

        private int breakLineCount(string _)
        {
            var text = _;
            var counter = 1;

            // While does not contain official or end of section i.e. canParseAppointee is false
            while (CanParseNominator(text) && CanMoveToNextLine(text))
            {
                text = text.Remove(0, text.IndexOf("\r\n") + 4);
                counter++;
            }

            return counter;
        }

        private bool CanParseNominator(string _)
        {
            var text = _;

            text = text.Remove(0, text.IndexOf("\r\n") + 4);

            text = text.TrimStart();

            if (CanMoveToNextLine(text))
            {
                return true;
            }
            return false;
        }

        private bool CanParseAppointee(string _)
        {
            var firstSixLetters = _.Substring(0, 6);

            switch (firstSixLetters)
            {
                case "ENACTM":
                    return false;
                case "RESULT":
                    return false;
                case "MOTION":
                    return false;
                default:
                    return true;
            }
        }

        private bool CanMoveToNextLine(string _)
        {
            var firstSixLetters = _.Substring(0, 6);

            switch (firstSixLetters)
            {
                case "ENACTM":
                    return false;
                case "RESULT":
                    return false;
                case "MOTION":
                    return false;
                case "Chair ":
                    return false;
                case "Commis":
                    return false;
                case "City M":
                    return false;
                case "Vice C":
                    return false;
                case "Mayor ":
                    return false;
                default:
                    return true;
            }
        }

        private bool IsAppointeeAnCityOfficial(string appointee)
        {
            if (appointee.Contains("Keon Hardemon"))
                return true;
            if (appointee.Contains("Francis Suarez"))
                return true;
            if (appointee.Contains("Ken Russell"))
                return true;
            if (appointee.Contains("Manolo Reyes"))
                return true;
            if (appointee.Contains("Joe Carollo"))
                return true;
            if (appointee.Contains("Wifredo (Willy) Gort"))
                return true;
            if (appointee.Contains("Emilio T. Gonzalez"))
                return true;
            if (appointee.Contains("Commission-At-Large"))
                return true;
            if (appointee.Contains("IAFF"))
                return true;
            if (appointee.Contains("FOP"))
                return true;
            if (appointee.Contains("AFSCME 1907"))
                return true;
            if (appointee.Contains("AFSCME 871"))
                return true;

            return false;
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
        public List<(string, string)> AppointeesAndNominators { get; set; } = new List<(string, string)>();


        public BoardsAndCommitteeItem()
        {
            Movers = new List<string>();
            Seconders = new List<string>();
            Ayes = new List<string>();
            Absent = new List<string>();
        }
    }
}
