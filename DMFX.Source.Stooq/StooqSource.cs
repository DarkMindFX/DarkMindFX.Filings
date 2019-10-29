using DMFX.QuotesInterfaces;
using DMFX.Stooq.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DMFX.Source.Stooq
{
    [Export("Stooq", typeof(IQuotesSource))]
    public class StooqSource : IQuotesSource
    {
        IQuotesSourceInitParams _initParams = null;

        Dictionary<string, string> _tickers = new Dictionary<string, string>();

        public StooqSource()
        {
            InitCanImportList();
        }

        public IQuotesSourceGetQuotesParams CreateGetQuotesParams()
        {
            return new StooqSourceGetQuotesParams();
        }

        public IQuotesSourceInitParams CreateInitParams()
        {
            return new StooqSourceInitParams();
        }

        public IQuotesSourceCanImportParams CreateCanImportParams()
        {
            return new StooqSourceCanImportParams();
        }

        public IQuotesSourceGetQuotesResult GetQuotes(IQuotesSourceGetQuotesParams getQuotesParams)
        {
            StooqApi api = new StooqApi();

            IQuotesSourceGetQuotesResult result = new StooqSourceGetQuotesResult();
            foreach (var t in getQuotesParams.Tickers)
            {
                IQuotesSourceCanImportParams canImportParams = CreateCanImportParams();
                canImportParams.Tickers.Add(t);
                IQuotesSourceCanImportResult canImportRes = CanImport(canImportParams);
                if (canImportRes.Success)
                {
                    try
                    {

                        IQuotesData qd = new BaseQuotesData();

                        var quotes = api.Download(t,
                                        getQuotesParams.PeriodStart,
                                        getQuotesParams.PeriodEnd,
                                        getQuotesParams.Country,
                                        getQuotesParams.TimeFrame == ETimeFrame.Daily ? StooqApi.ETimeFrame.Daily : (getQuotesParams.TimeFrame == ETimeFrame.Weekly ? StooqApi.ETimeFrame.Weekly : StooqApi.ETimeFrame.Monthly));

                        foreach (var q in quotes.Quotes)
                        {
                            ITimeSeriesRecord newRec = qd.CreateQuotesRecord();
                            newRec["Close"] = q.Close;
                            newRec["High"] = q.High;
                            newRec["Open"] = q.Open;
                            newRec["Low"] = q.Low;
                            newRec["Volume"] = q.Volume;
                            newRec.Time = ToPeriodStart(q.PeriodEnd, getQuotesParams.TimeFrame);

                            qd.AddRecord(newRec);
                        }

                        qd.Country = getQuotesParams.Country;
                        qd.Ticker = t;
                        qd.Name = this.TickerName(t);
                        qd.TimeFrame = getQuotesParams.TimeFrame;
                        qd.Type = this.TickerType(t);
                        qd.Unit = this.TickerUnit(t);

                        result.QuotesData.Add(qd);

                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.AddError(Interfaces.EErrorCodes.QuotesSourceFail, Interfaces.EErrorType.Error, ex.Message);
                    }
                }
            }

            result.Success = result.QuotesData.Count > 0;
            if (result.Success && result.QuotesData.Count != getQuotesParams.Tickers.Count)
            {
                result.AddError(Interfaces.EErrorCodes.QuotesNotFound, Interfaces.EErrorType.Warning, "Not all quotes were found");
            }
            else if (!result.Success)
            {
                result.AddError(Interfaces.EErrorCodes.QuotesNotFound, Interfaces.EErrorType.Error, "Requested tickers are not supported or quotes for them not found");
            }

            return result;
        }

        public IQuotesSourceCanImportResult CanImport(IQuotesSourceCanImportParams canImportParams)
        {
            IQuotesSourceCanImportResult result = new StooqSourceCanImportResult();
                       

            foreach (string t in canImportParams.Tickers)
            {
                if (_tickers.ContainsKey(t))
                {
                    result.Tickers.Add(t);
                }
            }

            result.Success = result.Tickers.Count > 0;

            return result;
        }

        public EUnit TickerUnit(string ticker)
        {
            IQuotesSourceCanImportParams canImportParams = CreateCanImportParams();
            canImportParams.Tickers.Add(ticker);

            IQuotesSourceCanImportResult canImportRes = CanImport(canImportParams);
            if (canImportRes.Success)
            {
                return EUnit.USD;
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
                return ETimeSeriesType.Price;
            }
            else
            {
                throw new ArgumentException("Unsupported ticker provided");
            }
        }

        

        public void Init(IQuotesSourceInitParams initParams)
        {
            _initParams = initParams;
        }

        #region Support methods

        private string TickerName(string ticker)
        {
            if (_tickers.ContainsKey(ticker))
            {
                return _tickers[ticker];
            }
            else
            {
                throw new ArgumentException("Unsupported ticker provided");
            }
        }

        private void InitCanImportList()
        {
            string xmlFile = null;

            // SEC companies
            xmlFile = Resources.SECCompanyList;
            XmlDocument docCompanies = new XmlDocument();
            docCompanies.LoadXml(xmlFile);

            XmlNodeList xnList = docCompanies.SelectNodes("/companies/company");
            foreach (XmlNode v in xnList)
            {
                _tickers.Add(v.Attributes["ticker"].Value, v.Attributes["name"].Value);
            }

            // ETFs
            xmlFile = Resources.ETFTickersList;
            XmlDocument docETFs = new XmlDocument();
            docETFs.LoadXml(xmlFile);

            xnList = docETFs.SelectNodes("/etfs/etf");

            foreach (XmlNode v in xnList)
            {
                _tickers.Add(v.Attributes["ticker"].Value, v.Attributes["name"].Value);
            }
        }

        private DateTime ToPeriodStart(DateTime end, ETimeFrame timeFrame)
        {
            DateTime result = end;
            switch (timeFrame)
            {
                case ETimeFrame.Daily:
                    result = end;
                    break;
                case ETimeFrame.Weekly:
                    throw new NotImplementedException();
                    break;
                case ETimeFrame.Monthly:
                    result = new DateTime(end.Year, end.Month, 1);
                    break;
            }

            return result;
        }


        #endregion
    }
}
