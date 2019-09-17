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
    public class CFTCSource : IQuotesSource
    {
        IQuotesSourceInitParams _initParams = null;

        public IQuotesSourceCanImportResult CanImport(IQuotesSourceCanImportParams canImportParams)
        {
            IQuotesSourceCanImportResult result = new CFTCSourceCanImportResult();
            
            ICFTCParserParams[] paramTypes = new ICFTCParserParams[]
            {
                new CFTCParserParamsCOTCmdtsFut(),
                new CFTCParserParamsCOTCmdtsFutOpt(),
                new CFTCParserParamsCOTFinFut(),
                new CFTCParserParamsCOTFinFutOpt()
            };

            foreach (var ticker in canImportParams.Tickers)
            {
                bool found = false;
                int i = 0;
                while (!found && i < paramTypes.Count())
                {
                    found = ticker.IndexOf(paramTypes[i].TickerPrefix) >= 0;
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
            IQuotesSourceGetQuotesResult result = new CFTCSourceGetQuotesResult();
            ICFTCParserParams parserParams = new CFTCParserParamsCOTFinFutOpt();
            parserParams.OnlyLast = getQuotesParams.PeriodEnd.Year == DateTime.Now.Year ? true : false;            

            CFTCParser cftcParser = new CFTCParser();

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
