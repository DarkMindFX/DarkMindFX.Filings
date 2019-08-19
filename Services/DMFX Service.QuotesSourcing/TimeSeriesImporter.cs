using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DMFX.Service.QuotesSourcing
{
    public class TimeSeriesImporterParams
    {
        public DateTime DateStart
        {
            get;
            set;
        }

        public DateTime DateEnd
        {
            get;
            set;
        }

        public DTO.ETimeFrame TimeFrame
        {
            get;
            set;
        }

        public HashSet<string> Tickers
        {
            get;
            set;
        }
    }
    public class TimeSeriesImporter
    {
        public enum EImportState
        {
            Idle = 0,
            Init = 1,
            ValidatingSource = 2,
            ReadingFilingInfo = 3,
            ImportSources = 4,
            Parsing = 5,
            Saving = 6
        }

        private Task _importTask = null;
        private DateTime _importStart = DateTime.MinValue;
        private DateTime _importEnd = DateTime.MinValue;
        private EImportState _currentState = EImportState.Idle;
        private CompositionContainer _compContainer = null;
        private List<Error> _errorsLog = new List<Error>();
        private IQuotesDal _dal = null;
        private ILogger _logger = null;
        private TimeSeriesImporterParams _impParams = null;
        private bool _isRunning = false;
        private HashSet<string> _tickersProcessed;

        public TimeSeriesImporter(CompositionContainer compContainer)
        {
            try
            {
                _compContainer = compContainer;
                Errors = new List<Error>();
                _tickersProcessed = new HashSet<string>();

                _logger = Global.Container.GetExport<ILogger>(ConfigurationManager.AppSettings["LoggerType"]).Value;

                InitDAL();

                _logger.Log(EErrorType.Info, "TimeSeriesImporter ready");

            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.Log(EErrorType.Error, "Importer constructor failed");
                    _logger.Log(ex);
                }
            }
        }

        public bool StartImport(TimeSeriesImporterParams impParams)
        {
            if (CurrentState == EImportState.Idle)
            {
                _tickersProcessed.Clear();
                _isRunning = true;
                _impParams = impParams;

                _importStart = DateTime.UtcNow;
                _errorsLog.Clear();

                _importTask = new Task(ImportThread);
                _importTask.Start();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool StopImport()
        {
            _isRunning = false;
            _importEnd = DateTime.UtcNow;
            return true;
        }

        #region Properties

        public EImportState CurrentState
        {
            get
            {
                return _currentState;
            }
            private set
            {
                _currentState = value;
            }
        }

        public DateTime ImportStart
        {
            get
            {
                return _importStart;
            }
        }

        public DateTime ImportEnd
        {
            get
            {
                return _importEnd;
            }
        }

        public HashSet<string> TickersProcessed
        {
            get
            {
                return _tickersProcessed;
            }
        }

        public List<Error> Errors
        {
            get
            {
                return _errorsLog;
            }
            private set
            {
                _errorsLog = value;
            }
        }

        public CompositionContainer CompositionContainer
        {
            get
            {
                return _compContainer;
            }
            set
            {
                _compContainer = value;
            }
        }

        #endregion

        private void InitDAL()
        {
            _logger.Log(EErrorType.Info, string.Format("InitDAL: Connecting to '{0}'", ConfigurationManager.AppSettings["ConnectionStringTimeSeries"]));

            Lazy<IQuotesDal> dal = _compContainer.GetExport<IQuotesDal>(ConfigurationManager.AppSettings["DALType"]);
            IQuotesDalInitParams dalParams = dal.Value.CreateInitParams();
            dalParams.Parameters.Add("ConnectionStringTimeSeries", ConfigurationManager.AppSettings["ConnectionStringTimeSeries"]);

            dal.Value.Init(dalParams);

            _dal = dal.Value;
        }

        protected void ImportThread()
        {
            _logger.Log(EErrorType.Info, "ImportThread started");
            if (_compContainer != null)
            {
                // Clearing all errors
                Errors.Clear();

                // validating if there are anything need to be imported
                CurrentState = EImportState.Init;

                List<string> sources = new List<string>();
                sources.Add("Stooq");

                foreach (var src in sources)
                {
                    // break if stopped
                    if (!_isRunning)
                    {
                        break;
                    }

                    Lazy<IQuotesSource> source = null;
                    try
                    {
                        source = _compContainer.GetExport<IQuotesSource>(src);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(ex);
                        Errors.Add(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Warning, Message = string.Format("ISource importer not registered for '{0}' - skipped", src) });
                    }
                    if (source != null && source.Value != null)
                    {
                        CurrentState = EImportState.ImportSources;

                        foreach (var t in _impParams.Tickers)
                        {
                            try
                            {
                                _logger.Log(EErrorType.Info, string.Format("Importing {0}", t));
                                IQuotesDalSaveTimeseriesValuesParams saveParams = _dal.CreateSaveTimeseriesValuesParams();

                                IQuotesSourceGetQuotesParams getQuotesParams = source.Value.CreateGetQuotesParams();
                                getQuotesParams.Country = ConfigurationManager.AppSettings["DefaultCountry"];
                                getQuotesParams.Ticker = t;
                                getQuotesParams.PeriodStart = _impParams.DateStart;
                                getQuotesParams.PeriodEnd = _impParams.DateEnd;
                                getQuotesParams.TimeFrame = (ETimeFrame)_impParams.TimeFrame;

                                CurrentState = EImportState.ImportSources;

                                IQuotesSourceGetQuotesResult getQuotesResult = source.Value.GetQuotes(getQuotesParams);

                                saveParams.Quotes.Add(getQuotesResult.QuotesData);

                                getQuotesResult.QuotesData.Unit = EUnit.USD;
                                getQuotesResult.QuotesData.Type = ETimeSeriesType.Price;

                                CurrentState = EImportState.Saving;

                                IQuotesDalSaveTimeseriesValuesResult saveResult = _dal.SaveTimeseriesValues(saveParams);

                                _tickersProcessed.Add(t);

                                _logger.Log(EErrorType.Info, string.Format("Import {0} done", t));
                            }
                            catch (Exception ex)
                            {
                                _logger.Log(ex);
                                Errors.Add(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Warning, Message = string.Format("ISource importer not registered for '{0}' - skipped", src) });
                            }

                        }
                    }
                    else
                    {
                        Errors.Add(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Warning, Message = string.Format("ISource importer not registered for '{0}' - skipped", src) });
                    }
                }
            }
            else
            {
                Errors.Add(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Error, Message = "Import failed. Composition comtainer is NULL" });
            }

            CurrentState = EImportState.Idle;
            _isRunning = false;
            _importEnd = DateTime.UtcNow;

            _logger.Log(EErrorType.Info, string.Format("ImportThread finished. Total errors: {0}, Time: {1}", Errors.Count, _importEnd - _importStart));

        }
    }
}