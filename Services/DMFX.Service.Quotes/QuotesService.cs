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

        public GetTickerListResponse Any(GetTickerList request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetTickerList");
            GetTickerListResponse response = new GetTickerListResponse();

            TransferHeader(request, response);

            try
            {
                if (IsValidSessionToken(request))
                {
                    IQuotesDalGetTickersListParams getTSListParams = _dal.CreateGetTickersListParams();
                    getTSListParams.CountryCode = request.CountryCode;
                    getTSListParams.Type = request.Type;

                    IQuotesDalGetTickersListResult getTSListResult = _dal.GetTickersList(getTSListParams);

                    if (getTSListResult.Success)
                    {
                        foreach (var t in getTSListResult.Tickers)
                        {
                            response.Tickers.Add(
                                new DTO.TickerListItem()
                                {
                                     CountryCode = request.CountryCode,
                                     Ticker = t.Ticker,
                                     Name = t.Name,
                                     Unit = t.Unit,
                                     Type = t.Type
                                }
                            );
                            
                        }

                        response.Success = true;
                        
                    }
                    else
                    {
                        response.Errors.AddRange(getTSListResult.Errors);
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

            _logger.Log(EErrorType.Info, " ****** Call end: GetTickersList");

            return response;
        }

        public GetTimeSeriesInfoResponse Any(GetTimeSeriesInfo request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetTimeSeriesInfo");
            GetTimeSeriesInfoResponse response = new GetTimeSeriesInfoResponse();

            TransferHeader(request, response);

            try
            {
                if (IsValidSessionToken(request))
                {
                    IQuotesDalGetTimeSeriesInfoParams getTInfoParams = _dal.CreateGetTimeSeriesInfoParams();
                    getTInfoParams.CountryCode = request.CountryCode;
                    getTInfoParams.Ticker = request.Ticker;

                    IQuotesDalGetTimeSeriesInfoResult getTInfoResult = _dal.GetTimeSeriesInfo(getTInfoParams);

                    if (getTInfoResult.Success)
                    {
                        response.Ticker = request.Ticker;
                        response.Type = getTInfoResult.Type;
                        response.Unit = getTInfoResult.Unit;
                        response.Name = getTInfoResult.Name;
                        response.CountryCode = request.CountryCode;
                        
                        foreach (var t in getTInfoResult.Series)
                        {
                            response.Series.Add(
                                new TimeSeriesInfoItem()
                                {
                                    TimeFrame = (DTO.ETimeFrame)t.Timeframe,
                                    PeriodEnd = t.PeriodEnd,
                                    PeriodStart = t.PeriodStart
                                }
                                );
                        }

                        foreach (var c in getTInfoResult.Columns)
                        {
                            response.Columns.Add(c);
                        }

                        response.Success = true;

                    }
                    else
                    {
                        response.Errors.AddRange(getTInfoResult.Errors);
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

            _logger.Log(EErrorType.Info, " ****** Call end: GetTimeSeriesList");

            return response;
        }

        public GetTimeSeriesResponse Any(GetTimeSeries request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetTimeSeries");
            GetTimeSeriesResponse response = new GetTimeSeriesResponse();

            TransferHeader(request, response);

            try
            {
                if (IsValidSessionToken(request))
                {
                    IQuotesDalGetTimeSeriesValuesParams getParams = _dal.CreateGetQuotesParams();
                    getParams.Country = request.CountryCode;
                    getParams.Tickers.Add(request.Ticker);
                    getParams.PeriodEnd = request.PeriodEnd != null ? (DateTime)request.PeriodEnd : DateTime.Now;
                    getParams.PeriodStart = request.PeriodStart != null ? (DateTime)request.PeriodStart : DateTime.Parse(ConfigurationManager.AppSettings["DefaultPeriodStart"]);
                    getParams.TimeFrame = (QuotesInterfaces.ETimeFrame)request.TimeFrame;

                    IQuotesDalGetTimeseriesValuesResult getResult = _dal.GetTimseriesValues(getParams);
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
                        response.Errors.AddRange(getResult.Errors);
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

        private void TranslateToTickerQuotes(GetTimeSeriesResponse response, IQuotesDalGetTimeseriesValuesResult getResult)
        {
            IQuotesData quotesData = getResult.Quotes[0];
            TickerQuotes tickerQuotes = new TickerQuotes();
            tickerQuotes.Code = quotesData.Ticker;
            tickerQuotes.TimePeriod = quotesData.TimeFrame;
            tickerQuotes.PeriodStart = quotesData.Quotes.FirstOrDefault().Time;
            tickerQuotes.PeriodEnd = quotesData.Quotes.LastOrDefault().Time;
            tickerQuotes.Quotes.AddRange(quotesData.Quotes.Select(x => new QuoteRecord( x.Time, x.Values )).ToList());

            response.Values = tickerQuotes;

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
            initParams.Parameters["ConnectionStringTimeSeries"] = ConfigurationManager.AppSettings["ConnectionStringTimeSeries"];
            _dal.Init(initParams);

        }

        #endregion
    }
}