using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf;
using Spire.Pdf.Widget;

namespace PdfParser
{
    class Program
    {
        static void Main(string[] args)
        {
            TestMeetingData();
        }

        public static void TestMeetingData()
        {
            PdfDocument doc = new PdfDocument();

            //doc.LoadFromFile(@"C:\Users\User\OneDrive\Projects\Regular Meeting\01-10-2019\Meeting.pdf");
            //doc.LoadFromFile(@"C:\Users\User\OneDrive\Projects\Regular Meeting\02-14-2019\Meeting.pdf");
            //doc.LoadFromFile(@"C:\Users\User\OneDrive\Projects\Regular Meeting\03-14-2019\Meeting.pdf");
            //doc.LoadFromFile(@"C:\Users\User\OneDrive\Projects\Regular Meeting\04-11-2019\Meeting.pdf");
            doc.LoadFromFile(@"C:\Users\User\OneDrive\Projects\Regular Meeting\05-09-2019\Meeting.pdf");

            var data = new MiamiMeetingData();

            data.LoadData(doc);

            Console.ReadLine();
        }
    }
}
