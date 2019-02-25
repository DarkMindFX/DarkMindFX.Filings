using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Stooq.Api
{
    public class StooqApi
    {
        private static string _baseUrl = "https://stooq.com/q/d/l/?s={0}.{1}&d1={2}&d2={3}&i={4}";
        public enum ETimeFrame
        {
            Daily,
            Weekly,
            Monthly
        }

        public CSVQuotes Download(string ticker, DateTime from, DateTime to, string country = "US", ETimeFrame tf = ETimeFrame.Daily, bool header = true)
        {
            CSVQuotes result = null;

            string url = string.Format(_baseUrl, 
                ticker, 
                country, 
                from.ToString("yyyyMMdd"),
                to.ToString("yyyyMMdd"),
                (tf == ETimeFrame.Daily ? "d" : tf == ETimeFrame.Weekly ? "w" : "m"));

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                string text = sr.ReadToEnd();

                if (!string.IsNullOrEmpty(resp.Headers["Content-disposition"]) && resp.Headers["Content-disposition"].Contains("error"))
                {
                    throw new InvalidOperationException(text);
                }     
                

                result = new CSVQuotes();
                result.Ticker = ticker.ToUpper();
                result.Quotes = ParseCSV(text, header);
            }

            return result;

        }

        private List<CSVRecord> ParseCSV(string text, bool header)
        {
            List<CSVRecord> result = new List<CSVRecord>();
            string[] lines = text.Split(new char[] { '\r' });
            bool headerSkipped = false;
            foreach (var line in lines)
            {
                if (header && !headerSkipped)
                {
                    headerSkipped = true;
                }
                else
                {

                    string[] vals = line.Trim().Split(new char[] { ',' });
                    if (vals.Count() >= 6)
                    {
                        CSVRecord rec = new CSVRecord();
                        rec.PeriodEnd = DateTime.Parse(vals[0]);
                        rec.Open = Decimal.Parse(vals[1]);
                        rec.High = Decimal.Parse(vals[2]);
                        rec.Low = Decimal.Parse(vals[3]);
                        rec.Close = Decimal.Parse(vals[4]);
                        rec.Volume = Decimal.Parse(vals[5]);

                        result.Add(rec);
                    }
                }
            }

            return result;
        }


    }
}
