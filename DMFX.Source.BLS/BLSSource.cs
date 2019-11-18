using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using DMFX.Source.BLS.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DMFX.Source.BLS
{
    [Export("BLS", typeof(IQuotesSource))]
    [Export(typeof(IQuotesSource))]
    public class BLSSource : IQuotesSource
    {
        private static string s_agencyCode = "BLS";

        IQuotesSourceInitParams _initParams = null;

        struct TickerInfo
        {

            public string Ticker_Symbol
            {
                get;
                set;
            }

            public string Ticker_Name
            {
                get;
                set;
            }

            public string Bls_Code
            {
                get;
                set;
            }

            public Dictionary<string, string> Metadata
            {
                get;
                set;
            }

        }

        Dictionary<string, TickerInfo> _tickers = new Dictionary<string, TickerInfo>();

        public BLSSource()
        {
            InitCanImportList();
        }

        public IQuotesSourceGetQuotesParams CreateGetQuotesParams()
        {
            return new BLSSourceGetQuotesParams();
        }

        public IQuotesSourceInitParams CreateInitParams()
        {
            return new BLSSourceInitParams();
        }

        public IQuotesSourceCanImportParams CreateCanImportParams()
        {
            return new BLSSourceCanImportParams();
        }

        public IQuotesSourceGetQuotesResult GetQuotes(IQuotesSourceGetQuotesParams getQuotesParams)
        {
            List<string> tickers = new List<string>(getQuotesParams.Tickers);
            // if no tickers were provided - importing al available
            if (tickers.Count == 0)
            {
                tickers.AddRange(_tickers.Keys);
            }

            DMFX.BLS.Api.BLSApi blsApi = new DMFX.BLS.Api.BLSApi();

            IQuotesSourceGetQuotesResult result = new BLSSourceGetQuotesResult();
            foreach (var t in tickers)
            {
                IQuotesSourceCanImportParams canImportParams = CreateCanImportParams();
                canImportParams.Tickers.Add(t);
                IQuotesSourceCanImportResult canImportRes = CanImport(canImportParams);
                if (canImportRes.Success)
                {
                    try
                    {
                        var response = blsApi.Download( _tickers[t].Bls_Code, getQuotesParams.PeriodStart, getQuotesParams.PeriodEnd);

                        IQuotesData qd = new BaseQuotesData();
                        qd.Country = getQuotesParams.Country;
                        qd.Ticker = _tickers[t].Ticker_Symbol;
                        qd.Name = _tickers[t].Ticker_Name;
                        qd.TimeFrame = GetTimeFrame(response.Timeframe);
                        qd.AgencyCode = s_agencyCode;
                        qd.Unit = EUnit.Value;
                        qd.Type = ETimeSeriesType.Indicator;

                        // adding value records
                        foreach (var q in response.Quotes)
                        {
                            ITimeSeriesRecord tsr = new CustomTimeseriesRecord(qd.Ticker, q.PeriodEnd, q.Value);
                            qd.AddRecord(tsr);
                        }

                        // adding metadata
                        if (_tickers[t].Metadata != null && _tickers[t].Metadata.Count > 0)
                        {
                            ITimeSeriesMetadata metadata = qd.CreateQuotesMetadata();
                            metadata.Values = _tickers[t].Metadata;
                            qd.Metadata = metadata;
                        }

                        result.QuotesData.Add(qd);

                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.AddError(EErrorCodes.QuotesSourceFail, EErrorType.Error, ex.Message);
                    }
                }
            }

            result.Success = result.QuotesData.Count > 0;
            if (result.Success && result.QuotesData.Count <= getQuotesParams.Tickers.Count)
            {
                result.AddError(EErrorCodes.QuotesNotFound, EErrorType.Warning, "Not all quotes were found");
            }
            else if (!result.Success)
            {
                result.AddError(EErrorCodes.QuotesNotFound, EErrorType.Error, "Requested tickers are not supported or quotes for them not found");
            }

            return result;
        }

        public IQuotesSourceCanImportResult CanImport(IQuotesSourceCanImportParams canImportParams)
        {
            IQuotesSourceCanImportResult result = new BLSSourceCanImportResult();

            foreach (var r in canImportParams.Tickers)
            {
                string ticker = r.ToUpper();
                if (_tickers.ContainsKey(ticker))
                {
                    result.Tickers.Add(ticker);
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


        public void Init(IQuotesSourceInitParams initParams)
        {
            _initParams = initParams;
        }

        #region Support methods

        private ETimeFrame GetTimeFrame(DMFX.BLS.Api.BLSApi.ETimeFrame blsTimeFrame)
        {
            ETimeFrame result;
            switch (blsTimeFrame)
            {
                case DMFX.BLS.Api.BLSApi.ETimeFrame.Annual:
                    result = ETimeFrame.Annually;
                    break;
                case DMFX.BLS.Api.BLSApi.ETimeFrame.Daily:
                    result = ETimeFrame.Daily;
                    break;
                case DMFX.BLS.Api.BLSApi.ETimeFrame.Quarterly:
                    result = ETimeFrame.Quarterly;
                    break;
                case DMFX.BLS.Api.BLSApi.ETimeFrame.SemiAnnual:
                    result = ETimeFrame.SemiAnnual;
                    break;
                case DMFX.BLS.Api.BLSApi.ETimeFrame.Monthly:
                default:
                    result = ETimeFrame.Monthly;
                    break;
            }

            return result;

        }

        private string TickerName(string ticker)
        {
            throw new NotImplementedException();
        }

        private void InitCanImportList()
        {
            string xmlFile = null;

            // SEC companies
            xmlFile = Resources.BLSIndicatorsList;
            XmlDocument docCompanies = new XmlDocument();
            docCompanies.LoadXml(xmlFile);

            XmlNodeList xnList = docCompanies.SelectNodes("/indicators/indicator");
            foreach (XmlNode v in xnList)
            {
                TickerInfo ti = new TickerInfo();
                ti.Ticker_Symbol = v.Attributes["ticker_symbol"].Value.ToUpper();
                ti.Bls_Code = v.Attributes["bls_code"].Value.ToUpper();
                ti.Ticker_Name = v.Attributes["name"].Value;
                ti.Metadata = new Dictionary<string, string>();

                string txtMetadata = v.Attributes["metadata"].Value;
                string[] values = txtMetadata.Split(new char[] { ';' });
                foreach (var pair in values)
                {
                    if (!string.IsNullOrEmpty(pair.Trim()))
                    {
                        string[] kv = pair.Split(new char[] { '=' });
                        if (kv.Count() >= 2)
                        {
                            string key = kv[0];
                            string value = kv[1];
                            if (!ti.Metadata.ContainsKey(key))
                            {
                                ti.Metadata.Add(key, value);
                            }

                        }
                    }

                }

                _tickers.Add(ti.Ticker_Symbol, ti);
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
                case ETimeFrame.Quarterly:
                    {
                        int quarterNumber = (end.Month - 1) / 3 + 1;
                        DateTime firstDayOfQuarter = new DateTime(end.Year, (quarterNumber - 1) * 3 + 1, 1);
                        result = firstDayOfQuarter.AddMonths(3).AddDays(-1);
                    }
                    break;
                case ETimeFrame.Annually:
                    result = new DateTime(end.Year, 1, 1);
                    break;
            }

            return result;
        }


        #endregion
    }
}
