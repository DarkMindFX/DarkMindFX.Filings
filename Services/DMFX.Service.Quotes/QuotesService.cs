using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DMFX.Service.Quotes
{
    public class QuotesService : ServiceBase
    {
        CompositionContainer _compContainer = null;
        IQuotesDal _dal = null;


        public QuotesService()
        {
            _compContainer = Global.Container;
            InitDAL();
        }

        public GetQuotesResponse Any(GetQuotes request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetQuotes");
            GetQuotesResponse response = new GetQuotesResponse();

            TransferHeader(request, response);

            try
            {
                if (IsValidSessionToken(request))
                {
                    IQuotesDalGetQuotesParams getParams = _dal.CreateGetQuotesParams();
                    getParams.Country = request.CountryCode;
                    getParams.Tickers.Add(request.Ticker);
                    getParams.PeriodEnd = request.PeriodEnd != null ? (DateTime)request.PeriodEnd : DateTime.Now;
                    getParams.PeriodStart = request.PeriodStart != null ? (DateTime)request.PeriodStart : DateTime.Parse(ConfigurationManager.AppSettings["DefaultPeriodStart"]);
                    getParams.TimeFrame = (QuotesInterfaces.ETimeFrame)request.TimeFrame;

                    IQuotesDalGetQuotesResult getResult = _dal.GetQuotes(getParams);
                    // copying list of errors - there can be also warnings too
                    response.Errors.AddRange(response.Errors);
                    if (getResult.Success)
                    {
                        TranslateToTickerQuotes(response, getResult);
                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;                        
                    }
                }                
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unpexcted error: {0}", ex.Message)
                });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: GetQuotes");

            return response;
        }

        #region Support methods

        private void TranslateToTickerQuotes(GetQuotesResponse response, IQuotesDalGetQuotesResult getResult)
        {
            IQuotesData quotesData = getResult.Quotes[0];
            TickerQuotes tickerQuotes = new TickerQuotes();
            tickerQuotes.Code = quotesData.Ticker;
            tickerQuotes.TimePeriod = (DTO.ETimeFrame)quotesData.TimeFrame;
            tickerQuotes.PeriodStart = quotesData.Quotes.FirstOrDefault().Time;
            tickerQuotes.PeriodEnd = quotesData.Quotes.LastOrDefault().Time;
            tickerQuotes.Quotes.AddRange(quotesData.Quotes.Select(x => new QuoteRecord()
            {
                AdjClose = x["Close"],
                Close = x["Close"],
                Open = x["Open"],
                High = x["High"],
                Low = x["Low"],
                Time = x.Time,
                Volume = x["Volume"]
            }).ToList());

            response.Quotes = tickerQuotes;

        }

        protected override bool IsValidSessionToken(RequestBase request)
        {
            return request.SessionToken != null && request.SessionToken.Equals(ConfigurationManager.AppSettings["ServiceSessionToken"]);
        }

        private void InitDAL()
        {
            string dalType = ConfigurationManager.AppSettings["QuotesDalType"];
            _dal = _compContainer.GetExport<IQuotesDal>(dalType).Value;

            IQuotesDalInitParams initParams = _dal.CreateInitParams();
            initParams.Parameters["RootFolder"] = Path.Combine(_compContainer.GetExportedValue<string>("ServiceRootFolder"), ConfigurationManager.AppSettings["CSVDalRootFolder"]);
            _dal.Init(initParams);

        }

        #endregion
    }
}