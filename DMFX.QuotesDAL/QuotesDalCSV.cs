using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesDAL
{
    [Export("CSV", typeof(IQuotesDal))]
    public class QuotesDalCSV : IQuotesDal
    {
        string _rootFolder = null;

        public void Init(IQuotesDalInitParams initParams)
        {
            _rootFolder = initParams.Parameters["RootFolder"];
        }

        public IQuotesDalGetTimeseriesValuesResult GetTimseriesValues(IQuotesDalGetTimeSeriesValuesParams getQuotesParams)
        {
            IQuotesDalGetTimeseriesValuesResult result = new QuotesDalCSVGetQuotesResult();

            foreach (var t in getQuotesParams.Tickers)
            {
                string fileName = t + "." + getQuotesParams.TimeFrame.ToString() + ".csv";
                string filePath = Path.Combine(_rootFolder, getQuotesParams.Country, fileName);

                if (File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open)))
                    {
                        IQuotesData data = new BaseQuotesData();
                        data.Ticker = t;
                        data.TimeFrame = getQuotesParams.TimeFrame;
                        data.Country = getQuotesParams.Country;

                        using (CsvHelper.CsvReader reader = new CsvHelper.CsvReader(sr))
                        {
                            reader.Read();
                            reader.ReadHeader();
                            while (reader.Read())
                            {
                                ITimeSeriesRecord record = new BaseQuotesRecord();
                                record.Time = reader.GetField<DateTime>("Time");
                                record["Open"] = reader.GetField<decimal>("Open");
                                record["High"] = reader.GetField<decimal>("High");
                                record["Low"] = reader.GetField<decimal>("Low");
                                record["Close"] = reader.GetField<decimal>("Close");
                                record["Volume"] = reader.GetField<decimal>("Volume");

                                if (record.Time <= getQuotesParams.PeriodEnd && record.Time >= getQuotesParams.PeriodStart)
                                {
                                    data.AddRecord(record);
                                }
                            }
                        }

                        result.Quotes.Add(data);
                    }
                }
                else
                {
                    result.AddError(Interfaces.EErrorCodes.QuotesNotFound, Interfaces.EErrorType.Warning, string.Format("Quotes not found: {0}, {1}", t, getQuotesParams.TimeFrame));
                }
            }

            if (result.Quotes.Count == 0 && getQuotesParams.Tickers.Count != 0)
            {
                result.Success = false;
            }
            else
            {
                result.Success = true;
            }

            return result;
        }

        public IQuotesDalGetTimeSeriesListResult GetTimeSeriesList(IQuotesDalGetTimeSeriesListParams getTsList)
        {
            throw new NotImplementedException();
        }

        public IQuotesDalGetTimeSeriesInfoResult GetTimeSeriesInfo(IQuotesDalGetTimeSeriesInfoParams getTsInfoParams)
        {
            throw new NotImplementedException();
        }

        public IQuotesDalSaveTimeseriesValuesResult SaveTimeseriesValues(IQuotesDalSaveTimeseriesValuesParams saveQuotesParams)
        {
            throw new NotImplementedException();
        }

        
        #region Create* methods

        public IQuotesDalGetTimeSeriesValuesParams CreateGetQuotesParams()
        {
            return new QuotesDalCSVGetQuotesParams();
        }

        public IQuotesDalInitParams CreateInitParams()
        {
            return new QuotesDalCSVInitParams();
        }

        public IQuotesDalSaveTimeseriesValuesParams CreateSaveTimeseriesValuesParams()
        {

            return new QuotesDalCSVSaveQuotesParams();
        }        

        public IQuotesDalGetTimeSeriesInfoParams CreateGetTimeSeriesInfoParams()
        {
            throw new NotImplementedException();
        }

        public IQuotesDalGetTimeSeriesListParams CreateGetTimeSeriesListParams()
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
