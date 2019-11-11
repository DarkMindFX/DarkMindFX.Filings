using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Template.DMFX.Source.XYZ
{
    [Export("XYZ", typeof(IQuotesSource))]
    public class XYZSource : IQuotesSource
    {
        IQuotesSourceInitParams _initParams = null;

        Dictionary<string, string> _tickers = new Dictionary<string, string>();

        public XYZSource()
        {
            InitCanImportList();
        }

        public IQuotesSourceGetQuotesParams CreateGetQuotesParams()
        {
            return new XYZSourceGetQuotesParams();
        }

        public IQuotesSourceInitParams CreateInitParams()
        {
            return new XYZSourceInitParams();
        }

        public IQuotesSourceCanImportParams CreateCanImportParams()
        {
            return new XYZSourceCanImportParams();
        }

        public IQuotesSourceGetQuotesResult GetQuotes(IQuotesSourceGetQuotesParams getQuotesParams)
        {
            

            IQuotesSourceGetQuotesResult result = new XYZSourceGetQuotesResult();
            foreach (var t in getQuotesParams.Tickers)
            {
                IQuotesSourceCanImportParams canImportParams = CreateCanImportParams();
                canImportParams.Tickers.Add(t);
                IQuotesSourceCanImportResult canImportRes = CanImport(canImportParams);
                if (canImportRes.Success)
                {
                    try
                    {

                        
                        // TODO: result.QuotesData.Add(qd);

                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.AddError(EErrorCodes.QuotesSourceFail, EErrorType.Error, ex.Message);
                    }
                }
            }

            result.Success = result.QuotesData.Count > 0;
            if (result.Success && result.QuotesData.Count != getQuotesParams.Tickers.Count)
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
            IQuotesSourceCanImportResult result = new XYZSourceCanImportResult();                       

            // TODO: Impl;

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
            throw new NotImplementedException();
        }

        private void InitCanImportList()
        {
            throw new NotImplementedException();
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
