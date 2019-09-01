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
            _pdfText = _buffer.ToString();
            // Pass in FIRST OR SECOND

            if (_pdfText.Contains("FR - FIRST READING ORDINANCES") && _pdfText.Contains("END OF FIRST READING ORDINANCES"))
            {
                LoadOrdinances(true);
            }

            while (!_pdfText.Contains("END OF FIRST READING ORDINANCES"))
            {
                LoadOrdinances();
                //_buffer.Clear();
                //_pageBase = _pages[++_index];
                //_buffer.Append(_pageBase.ExtractText());
                //_pdfText = _buffer.ToString();
            }
            // Pass in FIRST OR SECOND
            var endOfSRIndex = _pdfText.IndexOf("END OF FIRST READING ORDINANCES");
            var _pdftext = _pdfText.Substring(0, endOfSRIndex);

            LoadOrdinances();

            outIndex = _index;
        }

        private void LoadOrdinances(bool singlePage = false)
        {
            var indexOfResolution = 0;
            var counter = 1;
            var agendaReadingNumber = "FR.";
            var startOfOrdinance = $"FR.{counter.ToString()}                         ORDINANCE";
            if (!singlePage)
            {
                // Paragraph = Index of title + title length, Paragraph length - next title length
                // Pass in FIRST OR SECOND
                while (!_pdfText.Contains("END OF FIRST READING ORDINANCES") && _pdfText.Contains(_resolution))
                {
                    var ordinanceNumber = string.Empty;
                    var motionTo = string.Empty;
                    var result = string.Empty;
                    var movers = new List<string>();
                    var seconders = new List<string>();
                    var ayes = new List<string>();
                    var absent = new List<string>();
                    var enactmentNumber = string.Empty;
                    var resolutionBodyLength = 0;
                    var resolutionBody = string.Empty;

                    indexOfResolution = _pdfText.IndexOf(_resolution);
                    ordinanceNumber = _pdfText.Substring(indexOfResolution, 40).Replace(_resoltutionHeaderSpace, string.Empty).Trim();
                    if (_pdfText.Contains(_enactmentNumber))
                    {
                        enactmentNumber = _pdfText.Substring(_pdfText.IndexOf(_enactmentNumber) + _enactmentNumber.Length, 30).Trim();
                    }
                    else
                    {
                        _splitPage = true;
                        // Get first half of resolution
                        resolutionBodyLength = (_pdfText.IndexOf(_cityOfMiami) - _cityOfMiami.Length) - _pdfText.IndexOf(_resolution);
                        resolutionBody = _pdfText.Substring(_pdfText.IndexOf(_resolution) + _resolution.Length, resolutionBodyLength).TrimEnd();

                        // Next page
                        _buffer.Clear();
                        _pageBase = _pages[++_index];
                        _buffer.Append(_pageBase.ExtractText());
                        _pdfText = _buffer.ToString();

                        // Get second half of resolution
                        resolutionBody += _pdfText.Substring(0, _pdfText.IndexOf(_motionTo)).TrimEnd();
                        resolutionBody = resolutionBody.Replace(_textToRemove, string.Empty).Replace(_textToRemove2, string.Empty).Replace("January 10, 2019", string.Empty).TrimStart();

                        if (_pdfText.Contains(_motionTo))
                        {
                            // Continue
                            motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                            result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 40).Trim();
                            movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 50).Trim());
                            seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                            ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                            absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());

                            var end = _pdfText.IndexOf("\r\n                                                  \r\n                                                   ");

                            // Clear resolution we've just done
                            _pdfText = _pdfText.Substring(end, _pdfText.Length - end);
                        }
                        else
                        {
                            // Move on to next page
                            _buffer.Clear();
                            _pageBase = _pages[++_index];
                            _buffer.Append(_pageBase.ExtractText());
                            _pdfText = _buffer.ToString();

                            // Get Ordinance results/votes
                            motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                            result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 40).Trim();
                            movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 50).Trim());
                            seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                            ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                            absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());
                        }

                        ReadingOrdinances.Add(new ReadingOrdinance
                        {
                            ItemNumber = ordinanceNumber,
                            EnactmentNumber = enactmentNumber,
                            Body = resolutionBody,
                            MotionTo = motionTo,
                            Result = result,
                            Movers = movers,
                            Seconders = seconders,
                            Ayes = ayes,
                            Absent = absent
                        });
                        continue;
                    }

                    resolutionBodyLength = _pdfText.IndexOf(_enactmentNumber) - _pdfText.IndexOf(_resolution);
                    resolutionBody = _pdfText.Substring(_pdfText.IndexOf(_resolution) + _resolution.Length, resolutionBodyLength);

                    if (_pdfText.Contains(_motionTo))
                    {
                        // Continue
                        motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                        result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 40).Trim();
                        movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 50).Trim());
                        seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                        ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                        absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());

                        //var end = _pdfText.IndexOf("\r\n                                                  \r\n                                                   ");

                        //// Clear resolution we've just done
                        //_pdfText = _pdfText.Substring(end, _pdfText.Length - end);
                    }
                    else
                    {
                        // Move on to next page
                        _buffer.Clear();
                        _pageBase = _pages[++_index];
                        _buffer.Append(_pageBase.ExtractText());
                        _pdfText = _buffer.ToString();

                        // Get Ordinance results/votes
                        motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                        result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 40).Trim();
                        movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 50).Trim());
                        seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                        ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                        absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());
                    }

                    //var endOfResolution = _pdfText.IndexOf(enactmentNumber) + enactmentNumber.Length;
                    var threeBreakLines = " \r\n                        \r\n                        \r\n";
                    var endOfResolution = _pdfText.IndexOf(threeBreakLines);

                    if (endOfResolution == 0)
                    {
                        //// Clear resolution we've just done
                        endOfResolution = _pdfText.IndexOf(_ayes);
                        _pdfText = _pdfText.Substring(endOfResolution, _pdfText.Length - endOfResolution);
                    }
                    else
                    {
                        // Clear resolution we've just done
                        _pdfText = _pdfText.Substring(endOfResolution, _pdfText.Length - endOfResolution);
                    }

                    ReadingOrdinances.Add(new ReadingOrdinance
                    {
                        ItemNumber = ordinanceNumber,
                        EnactmentNumber = enactmentNumber,
                        Body = resolutionBody,
                        MotionTo = motionTo,
                        Result = result,
                        Movers = movers,
                        Seconders = seconders,
                        Ayes = ayes,
                        Absent = absent
                    });
                }
            }
            else
            {
                // Paragraph = Index of title + title length, Paragraph length - next title length
                // Pass in FIRST OR SECOND
                while (_pdfText.Contains(startOfOrdinance))
                {
                    var ordinanceNumber = string.Empty;
                    var motionTo = string.Empty;
                    var result = string.Empty;
                    var movers = new List<string>();
                    var seconders = new List<string>();
                    var ayes = new List<string>();
                    var absent = new List<string>();
                    var enactmentNumber = string.Empty;
                    var resolutionBodyLength = 0;
                    var resolutionBody = string.Empty;

                    indexOfResolution = _pdfText.IndexOf(_resolution);
                    ordinanceNumber = _pdfText.Substring(indexOfResolution, 40).Replace(_resoltutionHeaderSpace, string.Empty).Trim();
                    if (_pdfText.Contains(_enactmentNumber))
                    {
                        enactmentNumber = _pdfText.Substring(_pdfText.IndexOf(_enactmentNumber) + _enactmentNumber.Length, 30).Trim();
                    }
                    else
                    {
                        // Clear everything before FR.1
                        agendaReadingNumber = "FR." + counter.ToString();
                        _pdfText = _pdfText.Remove(0, _pdfText.IndexOf($"{agendaReadingNumber}"));

                        var ordinanceEnd = "\r\n                                                  \r\n                                                   ";
                        resolutionBodyLength = (_pdfText.IndexOf(ordinanceEnd)) - _pdfText.IndexOf(_resolution);
                        resolutionBody = _pdfText.Substring(_pdfText.IndexOf(_resolution) + _resolution.Length, resolutionBodyLength).TrimEnd();

                        //// Next page
                        //_buffer.Clear();
                        //_pageBase = _pages[++_index];
                        //_buffer.Append(_pageBase.ExtractText());
                        //_pdfText = _buffer.ToString();

                        // Get second half of resolution
                        //resolutionBody += _pdfText.Substring(0, _pdfText.IndexOf(_motionTo)).TrimEnd();
                        //resolutionBody = resolutionBody.Replace(_textToRemove, string.Empty).Replace(_textToRemove2, string.Empty).Replace("January 10, 2019", string.Empty).TrimStart();

                        if (_pdfText.Contains(_motionTo))
                        {
                            // Continue
                            motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                            result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 40).Trim();
                            movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 50).Trim());
                            seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                            ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                            absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());

                            var end = _pdfText.IndexOf(_ayes);

                            // Clear resolution we've just done
                            _pdfText = _pdfText.Remove(0, end);
                        }
                        else
                        {
                            // Move on to next page
                            _buffer.Clear();
                            _pageBase = _pages[++_index];
                            _buffer.Append(_pageBase.ExtractText());
                            _pdfText = _buffer.ToString();

                            // Get Ordinance results/votes
                            motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                            result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 40).Trim();
                            movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 50).Trim());
                            seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                            ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                            absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());
                        }

                        ReadingOrdinances.Add(new ReadingOrdinance
                        {
                            ItemNumber = ordinanceNumber,
                            EnactmentNumber = enactmentNumber,
                            Body = resolutionBody,
                            MotionTo = motionTo,
                            Result = result,
                            Movers = movers,
                            Seconders = seconders,
                            Ayes = ayes,
                            Absent = absent
                        });
                        counter++;
                        startOfOrdinance = $"FR.{counter.ToString()}                         ORDINANCE";
                        continue;
                    }

                    resolutionBodyLength = _pdfText.IndexOf(_enactmentNumber) - _pdfText.IndexOf(_resolution);
                    resolutionBody = _pdfText.Substring(_pdfText.IndexOf(_resolution) + _resolution.Length, resolutionBodyLength);

                    if (_pdfText.Contains(_motionTo))
                    {
                        // Continue
                        motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                        result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 40).Trim();
                        movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 50).Trim());
                        seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                        ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                        absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());

                        //var end = _pdfText.IndexOf("\r\n                                                  \r\n                                                   ");

                        //// Clear resolution we've just done
                        //_pdfText = _pdfText.Substring(end, _pdfText.Length - end);
                    }
                    else
                    {
                        // Move on to next page
                        _buffer.Clear();
                        _pageBase = _pages[++_index];
                        _buffer.Append(_pageBase.ExtractText());
                        _pdfText = _buffer.ToString();

                        // Get Ordinance results/votes
                        motionTo = _pdfText.Substring(_pdfText.IndexOf(_motionTo) + _motionTo.Length, 40).Trim();
                        result = _pdfText.Substring(_pdfText.IndexOf(_result) + _result.Length, 40).Trim();
                        movers.Add(_pdfText.Substring(_pdfText.IndexOf(_mover) + _mover.Length, 50).Trim());
                        seconders.Add(_pdfText.Substring(_pdfText.IndexOf(_seconder) + _seconder.Length, 50).Trim());
                        ayes.AddRange(_pdfText.Substring(_pdfText.IndexOf(_ayes) + _ayes.Length, 50).Trim().Split(',').ToList());
                        absent.AddRange(_pdfText.Substring(_pdfText.IndexOf(_absent) + _absent.Length, 40).Trim().Split(',').ToList());
                    }

                    //var endOfResolution = _pdfText.IndexOf(enactmentNumber) + enactmentNumber.Length;
                    var threeBreakLines = " \r\n                        \r\n                        \r\n";
                    var endOfResolution = _pdfText.IndexOf(threeBreakLines);

                    if (endOfResolution == 0)
                    {
                        //// Clear resolution we've just done
                        endOfResolution = _pdfText.IndexOf(_ayes);
                        _pdfText = _pdfText.Substring(endOfResolution, _pdfText.Length - endOfResolution);
                    }
                    else
                    {
                        // Clear resolution we've just done
                        _pdfText = _pdfText.Substring(endOfResolution, _pdfText.Length - endOfResolution);
                    }

                    ReadingOrdinances.Add(new ReadingOrdinance
                    {
                        ItemNumber = ordinanceNumber,
                        EnactmentNumber = enactmentNumber,
                        Body = resolutionBody,
                        MotionTo = motionTo,
                        Result = result,
                        Movers = movers,
                        Seconders = seconders,
                        Ayes = ayes,
                        Absent = absent
                    });
                }
            }
        }
    }

    public class ReadingOrdinance
    {
        private string _pdfText { get; set; }
        private StringBuilder _buffer { get; set; } = new StringBuilder();
        private PdfPageBase _pageBase { get; set; }
        private string _resolution => "ORDINANCE";
        private string ph1 => "PH.1";
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

        public ReadingOrdinance()
        {
            Movers = new List<string>();
            Seconders = new List<string>();
            Ayes = new List<string>();
            Absent = new List<string>();
        }
    }
}
