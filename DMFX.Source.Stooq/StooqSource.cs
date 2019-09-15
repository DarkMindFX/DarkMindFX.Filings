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

            StooqSourceGetQuotesResult result = new StooqSourceGetQuotesResult();

            try
            {

                var quotes = api.Download(getQuotesParams.Ticker,
                                getQuotesParams.PeriodStart,
                                getQuotesParams.PeriodEnd,
                                getQuotesParams.Country,
                                getQuotesParams.TimeFrame == ETimeFrame.Daily ? StooqApi.ETimeFrame.Daily : (getQuotesParams.TimeFrame == ETimeFrame.Weekly ? StooqApi.ETimeFrame.Weekly : StooqApi.ETimeFrame.Monthly));


                foreach (var q in quotes.Quotes)
                {
                    ITimeSeriesRecord newRec = result.QuotesData.CreateQuotesRecord();
                    newRec["Close"] = q.Close;
                    newRec["High"] = q.High;
                    newRec["Open"] = q.Open;
                    newRec["Low"] = q.Low;
                    newRec["Volume"] = q.Volume;
                    newRec.Time = ToPeriodStart(q.PeriodEnd, getQuotesParams.TimeFrame);

                    result.QuotesData.AddRecord(newRec);

                }

                result.QuotesData.Country = getQuotesParams.Country;
                result.QuotesData.Ticker = getQuotesParams.Ticker;
                result.QuotesData.TimeFrame = getQuotesParams.TimeFrame;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.AddError(Interfaces.EErrorCodes.QuotesSourceFail, Interfaces.EErrorType.Error, ex.Message);
            }

            return result;
        }

        public IQuotesSourceCanImportResult CanImport(IQuotesSourceCanImportParams canImportParams)
        {
            IQuotesSourceCanImportResult result = new StooqSourceCanImportResult();

            string xmlFile = Resources.SECCompanyList;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);

            XmlNodeList xnList = doc.SelectNodes("/companies/company");
            foreach (XmlNode xn in xnList)
            {
                if (canImportParams.Tickers.IndexOf(xn.Attributes["ticker"].Value) >= 0)
                {
                    result.Tickers.Add(xn.Attributes["ticker"].Value);
                }
            }

            result.Success = result.Tickers.Count > 0;
            
            return result;
        }

        public void Init(IQuotesSourceInitParams initParams)
        {
            _initParams = initParams;
        }

        #region Support methods
        DateTime ToPeriodStart(DateTime end, ETimeFrame timeFrame)
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
