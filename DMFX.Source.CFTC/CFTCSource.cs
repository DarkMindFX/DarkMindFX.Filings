using DMFX.CFTC.Api;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Source.CFTC
{
    [Export("CFTC", typeof(IQuotesSource))]
    [Export(typeof(IQuotesSource))]
    public class CFTCSource : IQuotesSource
    {
        private static string s_agencyCode = "CFTC";

        IQuotesSourceInitParams _initParams = null;

        static ICFTCParserParams[] s_paramTypes = new ICFTCParserParams[]
            {
                new CFTCParserParamsCOTCmdtsFut(),
                new CFTCParserParamsCOTCmdtsFutOpt(),
                new CFTCParserParamsCOTFinFut(),
                new CFTCParserParamsCOTFinFutOpt()
            };

        public IQuotesSourceCanImportResult CanImport(IQuotesSourceCanImportParams canImportParams)
        {
            IQuotesSourceCanImportResult result = new CFTCSourceCanImportResult();
            
            
            foreach (var ticker in canImportParams.Tickers)
            {
                bool found = false;
                int i = 0;
                while (!found && i < s_paramTypes.Count())
                {
                    found = ticker.IndexOf(s_paramTypes[i].TickerPrefix) >= 0;
                    ++i;
                }

                if (found)
                {
                    result.Tickers.Add(ticker);
                }
            }

            result.Success = result.Tickers.Count > 0;

            return result;
        }        

        public IQuotesSourceGetQuotesResult GetQuotes(IQuotesSourceGetQuotesParams getQuotesParams)
        {
            // For COT reports we don't extract selected items only because it will be too expensive
            // Rather we're loading the whole report with all items there
            // For this purposes we only identifying the types of items we need to extract based on indicator prefixes

            IQuotesSourceGetQuotesResult result = new CFTCSourceGetQuotesResult();                       

            CFTCParser cftcParser = new CFTCParser();

            // checking which types we need and preparing parameters
            List<ICFTCParserParams> parserParams = new List<ICFTCParserParams>();
            foreach (var pt in s_paramTypes)
            {
                ICFTCParserParams cotTypeParams = null;

                // at least one ticker has a prefix of given type - adding corresponding params object to parse proper report
                // if no tickers are specified - importing all
                if ((getQuotesParams.Tickers == null || getQuotesParams.Tickers.Count() == 0) || getQuotesParams.Tickers.Count(x => x.Contains(pt.TickerPrefix)) > 0)
                {
                    cotTypeParams = pt.Clone();
                    cotTypeParams.OnlyLast = getQuotesParams.PeriodStart.Year == DateTime.Now.Year ? true : false;
                    parserParams.Add(cotTypeParams);
                }
                
            }

            // for the list of parameters - calling parser to load proper report
            foreach (var parserParam in parserParams)
            {
                ICFTCParserResult parserResult = cftcParser.Parse(parserParam);

                foreach (CFTCInstrumentQuotes i in parserResult.Instruments.Values)
                {
                    IQuotesData qd = new BaseQuotesData();
                    qd.Country = getQuotesParams.Country;
                    qd.Ticker = i.Ticker;
                    qd.Name = i.Description;
                    qd.Unit = TickerUnit(i.Ticker);
                    qd.Type = TickerType(i.Ticker);
                    qd.TimeFrame = ETimeFrame.Weekly;
                    
                    foreach (var q in i.Quotes)
                    {
                        ITimeSeriesRecord tsr = new CustomTimeseriesRecord(i.Timeseries, q.ReportDate, q.Values);
                        qd.AddRecord(tsr);
                    }

                    qd.AgencyCode = s_agencyCode;

                    result.QuotesData.Add(qd);
                }
            }

            result.Success = result.QuotesData.Count > 0;
            if (result.Success && result.QuotesData.Count < getQuotesParams.Tickers.Count)
            {
                result.AddError(Interfaces.EErrorCodes.QuotesNotFound, Interfaces.EErrorType.Warning, "Not all quotes were found");
            }
            else if (!result.Success)
            {
                result.AddError(Interfaces.EErrorCodes.QuotesNotFound, Interfaces.EErrorType.Error, "Requested tickers are not supported or quotes for them not found");
            }

            return result;
        }

        public void Init(IQuotesSourceInitParams initParams)
        {
            _initParams = initParams;

        }

        public EUnit TickerUnit(string ticker)
        {
            IQuotesSourceCanImportParams canImportParams = CreateCanImportParams();
            canImportParams.Tickers.Add(ticker);

            IQuotesSourceCanImportResult canImportRes = CanImport(canImportParams);
            if (canImportRes.Success)
            {
                return EUnit.Value;
            }
            else
            {
                throw new ArgumentException("Unsupported ticker provided"); 
            }
        }

        

        public ETimeSeriesType TickerType(string ticker)
        {
            IQuotesSourceCanImportParams canImportParams = CreateCanImportParams();
            canImportParams.Tickers.Add(ticker);

            IQuotesSourceCanImportResult canImportRes = CanImport(canImportParams);
            if (canImportRes.Success)
            {
                return ETimeSeriesType.Indicator;
            }
            else
            {
                throw new ArgumentException("Unsupported ticker provided");
            }
        }

        #region Create* functions
        public IQuotesSourceCanImportParams CreateCanImportParams()
        {
            return new CFTCSourceCanImportParams();
        }

        public IQuotesSourceGetQuotesParams CreateGetQuotesParams()
        {
            return new CFTCSourceGetQuotesParams();
        }

        public IQuotesSourceInitParams CreateInitParams()
        {
            return new CFTCSourceInitParams();
        }
        #endregion
    }
}
