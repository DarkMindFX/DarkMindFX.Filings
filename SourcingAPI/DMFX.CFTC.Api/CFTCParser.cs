using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.CFTC.Api
{
    public interface ICFTCParserParams 
    {
        #region abstract methods to overload


        /// <summary>
        /// Returns the list of column indexes and corresponding time series names to be used
        /// </summary>
        /// <returns></returns>
        Dictionary<int, string> ColumnsToTimeseriesMapping
        {
            get;
        }

        /// <summary>
        /// Returns ticker prefix - this will be added to commodities ID to uniqily identify it
        /// </summary>
        /// <returns></returns>
        string TickerPrefix
        {
            get;
        }

        /// <summary>
        /// Returns path to the file with most recent COT data
        /// </summary>
        /// <returns></returns>
        string RecentDataPath
        {
            get;
        }

        /// <summary>
        /// Returns list of paths to recourses which contain historical data
        /// </summary>
        /// <returns></returns>
        IList<string> HistorticalDataPaths
        {
            get;
        }

        /// <summary>
        /// Set to TRUE if only recent report need to be parsed. Otherwise whole historical data wil be also downloaded
        /// </summary>
        /// <returns></returns>
        bool OnlyLast
        {
            get;
            set;
        }

        /// <summary>
        /// Use this method to clone the object & content
        /// </summary>
        /// <returns></returns>
        ICFTCParserParams Clone();

        string[] SplitLine(string line);

        #endregion
    }

    public interface ICFTCParserResult
    {
        Dictionary<string, CFTCInstrumentQuotes> Instruments
        {
            get;
            set;
        }
    }

    public class CFTCParser
    {
        public CFTCParser()
        {
        }

        public ICFTCParserResult Parse(ICFTCParserParams parserParams)
        {
            ICFTCParserResult result = new CFTCParserResult();

            // preparing list of files to be downloaded and parsed
            List<string> files = new List<string>();
            files.Add(parserParams.RecentDataPath);
            if (!parserParams.OnlyLast)
            {
                files.AddRange(parserParams.HistorticalDataPaths);
            }

            foreach (var path in files)
            {
                try
                {
                    string url = string.Format("https://www.cftc.gov{0}", path);

                    // downloading 
                    byte[] data = ServiceCall(url);
                    if (data != null)
                    {
                        Stream dataStream = Decompress(data);
                        data = null;
                        System.GC.Collect();
                        if (dataStream != null)
                        {
                            StreamReader sr = new StreamReader(dataStream);
                            string txt = sr.ReadToEnd();

                            ParseCSV(txt, true, parserParams, result);

                            dataStream.Close();
                        }
                    }
                }
                catch(Exception ex)
                {
                    // suppressing errors in case if one of the files was not parsed
                }
            }

            return result;
        }

        private byte[] ServiceCall(string url)
        {
            byte[] response = null;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (WebClient client = new WebClient())
            {
                response =
                client.DownloadData(url);
            }

            return response;
        }

        private static Stream Decompress(byte[] data)
        {
            var output = new MemoryStream();
            using (var compressedStream = new MemoryStream(data))
            {
                using (ZipFile zip = ZipFile.Read(compressedStream))
                {
                    ZipEntry e = zip[0];
                    e.Extract(output);
                }
            }

            output.Position = 0;

            return output;
        }

        private void ParseCSV(string text, bool header, ICFTCParserParams parserParams, ICFTCParserResult result)
        {
            CFTCInstrumentQuotes curInstrument = null;

            int colName = 0;
            int colReportDate = 2;
            int colCFTCMarketCode = 3;

            StringReader sr = new StringReader(text);
            
            string line = null;
            bool headerSkipped = false;
            do
            {
                line = sr.ReadLine();
                if (line != null && header && !headerSkipped)
                {
                    headerSkipped = true;
                }
                else if (line != null)
                {

                    string[] vals = parserParams.SplitLine(line.Trim());
                    if (vals.Count() >= parserParams.ColumnsToTimeseriesMapping.Keys.Max() + 1)
                    {
                        // reading common values
                        CultureInfo enUS = new CultureInfo("en-US");
                        DateTime repDate;
                        // WARNING! There can be different time formats in the files
                        if (!DateTime.TryParse(vals[colReportDate], out repDate) 
                            && !DateTime.TryParseExact(vals[colReportDate], "M/dd/yyyy hh:mm:ss tt", enUS, DateTimeStyles.None, out repDate))
                        {
                            // fixing line with bad comma in description
                            int badComma = line.IndexOf(',');
                            StringBuilder sb = new StringBuilder(line);
                            sb[badComma] = ':';
                            vals = parserParams.SplitLine(sb.ToString().Trim());

                            DateTime.TryParse(vals[colReportDate], out repDate);
                        }

                        string name = vals[colName].Trim(new char[] { '"' });
                        string marketCode = vals[colCFTCMarketCode];

                        // checking - if its new instrument 
                        if (curInstrument == null || !curInstrument.Ticker.Equals(parserParams.TickerPrefix + marketCode))
                        {
                            curInstrument = new CFTCInstrumentQuotes()
                            {
                                Ticker = parserParams.TickerPrefix + marketCode,
                                Description = name
                            };
                            curInstrument.Timeseries.AddRange(parserParams.ColumnsToTimeseriesMapping.Values);

                            CFTCInstrumentQuotes instrument = null;
                            if (!result.Instruments.TryGetValue(curInstrument.Ticker, out instrument))
                            {
                                result.Instruments.Add(curInstrument.Ticker, curInstrument);
                            }
                            else
                            {
                                curInstrument = instrument;
                            }

                        }

                        // creating new record
                        CFTCRecord rec = new CFTCRecord(parserParams.ColumnsToTimeseriesMapping.Count);
                        rec.ReportDate = repDate;
                        int i = 0;
                        foreach (int col in parserParams.ColumnsToTimeseriesMapping.Keys)
                        {
                            rec[i] = Decimal.Parse(vals[col]);
                            ++i;
                        }

                        curInstrument.Quotes.Add(rec);



                    }
                }
            }
            while (line != null);

        }


    }
}
