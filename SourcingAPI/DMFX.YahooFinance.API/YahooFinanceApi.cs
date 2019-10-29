using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.YahooFinance.Api
{
    public class YahooFinanceApi
    {
        private static string _baseUrl = "https://query1.finance.yahoo.com/v7/finance/download/{0}?period1={1}&period2={2}&interval={3}&events=history&crumb=bgB7rlcXhFh";
        private static int _attempts = 5;
        private static int _attemptDelaySec = 10;
        public enum ETimeFrame
        {
            Daily,
            Weekly,
            Monthly
        }

        public CSVQuotes Download(string ticker, DateTime from, DateTime to, ETimeFrame tf = ETimeFrame.Daily, bool header = true)
        {
            CSVQuotes result = null;

            string secFrom = ((int)(from - DateTime.Parse("1970/01/01")).TotalSeconds).ToString();
            string secTo = ((int)(to - DateTime.Parse("1970/01/01")).TotalSeconds).ToString();

            string url = string.Format(_baseUrl,
                ticker,
                secFrom,
                secTo,
                (tf == ETimeFrame.Daily ? "1d" : tf == ETimeFrame.Weekly ? "1wk" : "1mo"));

            int attempt = 0;
            while (attempt < _attempts && result == null)
            {
                try
                {
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
                }
                catch(Exception ex)
                {
                    ++attempt;
                    System.Threading.Thread.Sleep(_attemptDelaySec * 1000);
                }
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
                        rec.AdjClose = Decimal.Parse(vals[5]);
                        rec.Volume = Decimal.Parse(vals[6]);

                        result.Add(rec);
                    }
                }
            }

            return result;
        }
    }
}
