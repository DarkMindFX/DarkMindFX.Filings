using Ionic.Zip;
using System;
using System.Collections.Generic;
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

        #endregion
    }

    public interface ICFTCParserResult
    {
        HashSet<CFTCInstrumentQuotes> Instruments
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
                string url = string.Format("https://www.cftc.gov{0}", path);

                // downloading 
                byte[] data = ServiceCall(url);
                if (data != null)
                {
                    Stream dataStream = Decompress(data);
                    if (dataStream != null)
                    {
                        StreamReader sr = new StreamReader(dataStream);
                        string txt = sr.ReadToEnd();

                        ParseCSV(txt, true, parserParams, result);
                    }
                }
            }

            return result;
        }

        private byte[] ServiceCall(string url)
        {
            byte[] response = null;

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
                    if (vals.Count() >= parserParams.ColumnsToTimeseriesMapping.Keys.Max() + 1)
                    {
                        // reading common values
                        string name = vals[colName];
                        string marketCode = vals[colCFTCMarketCode];
                        DateTime repDate = DateTime.Parse(vals[colReportDate]);

                        // checking - if its new instrument 
                        if (curInstrument == null || !curInstrument.Ticker.Equals(parserParams.TickerPrefix + marketCode))
                        {
                            curInstrument = new CFTCInstrumentQuotes()
                            {
                                Ticker = parserParams.TickerPrefix + marketCode,
                                Description = name
                            };
                            curInstrument.Timeseries.AddRange(parserParams.ColumnsToTimeseriesMapping.Values);

                            result.Instruments.Add(curInstrument);
                        }

                        // creating new record
                        CFTCRecord rec = new CFTCRecord(parserParams.ColumnsToTimeseriesMapping.Count);
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

        }


    }
}
