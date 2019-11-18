using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.BLS.Api
{
    public class BLSApi
    {
        private static string _baseUrl = "https://beta.bls.gov/dataViewer/csv?selectedSeriesIds={0}&startYear={1}&endYear={2}";

        public enum ETimeFrame
        {
            Daily,
            Weekly,
            Monthly,
            Quarterly,
            SemiAnnual,
            Annual
        }

        public CSVQuotes Download(string ticker, DateTime from, DateTime to, bool header = true)
        {
            CSVQuotes result = null;

            string url = string.Format(_baseUrl,
                ticker,
                from.ToString("yyyy"),
                to.ToString("yyyy"));

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                string text = sr.ReadToEnd();

                if (!string.IsNullOrEmpty(resp.Headers["Content-disposition"]) && resp.Headers["Content-disposition"].Contains("error"))
                {
                    throw new InvalidOperationException(text);
                }

                ETimeFrame timeFrame = ETimeFrame.Annual;
                result = new CSVQuotes();
                result.Ticker = ticker.ToUpper();
                result.Quotes = ParseCSV(text, header, out timeFrame);
                result.Timeframe = timeFrame;
            }

            return result;

        }

        private List<CSVRecord> ParseCSV(string text, bool header, out ETimeFrame timeframe)
        {
            timeframe = ETimeFrame.Monthly;

            List<CSVRecord> result = new List<CSVRecord>();
            string[] lines = text.Split(new char[] { '\r', '\n' });
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
                    if (vals.Count() >= 5)
                    {
                        DateTime dtStart, dtEnd;
                        ETimeFrame tf;

                        ToStartDate(vals[1], vals[2], out dtStart, out dtEnd, out tf);

                        timeframe = tf;

                        CSVRecord rec = new CSVRecord();
                        rec.PeriodStart = dtStart;
                        rec.PeriodEnd = dtEnd;
                        rec.Value = Decimal.Parse(vals[4]);
                        

                        result.Add(rec);
                    }
                }
            }

            return result;
        }

        void ToStartDate(string year, string period, out DateTime periodStart, out DateTime periodEnd, out ETimeFrame timeframe)
        {
            string periodId = period.Substring(1, period.Length - 1);
            string periodType = period.Substring(0, 1);

            int nYear = Int32.Parse(year);
            int nPeriod = Int32.Parse(periodId);

            switch (periodType)
            {
                case "M":
                    timeframe = ETimeFrame.Monthly;
                    periodStart = new DateTime(nYear, nPeriod, 1);
                    periodEnd = nPeriod < 12 ? new DateTime(nYear, nPeriod + 1, 1) - TimeSpan.FromDays(1) : new DateTime(nYear, 12, 31);
                    break;
                case "Q":
                    timeframe = ETimeFrame.Quarterly;
                    periodStart = new DateTime(nYear, (nPeriod - 1) * 3 + 1, 1);
                    periodEnd = nPeriod < 4 ? new DateTime(nYear, nPeriod * 3 + 1, 1) - TimeSpan.FromDays(1) : new DateTime(nYear, 12, 31);
                    break;
                case "S":
                    timeframe = ETimeFrame.SemiAnnual;
                    periodStart = new DateTime(nYear, (nPeriod - 1) * 6 + 1, 1);
                    periodEnd = nPeriod < 2 ? new DateTime(nYear, nPeriod * 6 + 1, 1) - TimeSpan.FromDays(1) : new DateTime(nYear, 12, 31);
                    break;
                case "A":
                default:
                    timeframe = ETimeFrame.Annual;
                    periodStart = new DateTime(nYear, 1, 1);
                    periodEnd = new DateTime(nYear, 12, 31);
                    break;
            }

        }
    }
}
