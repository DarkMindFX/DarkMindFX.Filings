using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMFX.Service.Sourcing
{
    public class ImporterParams
    {
        public ImporterParams()
        {
            DateEnd = DateTime.UtcNow;
            DateStart = DateTime.MinValue;
            CompanyCodes = new HashSet<string>();
            Types = new HashSet<string>();
        }
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

        public string RegulatorCode
        {
            get;
            set;
        }

        public HashSet<string> CompanyCodes
        {
            get;
            set;
        }

        public HashSet<string> Types
        {
            get;
            set;
        }
    }

    public class Importer
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
        private ImportResults _currImport = null;
        private Interfaces.DAL.IDal _dal = null;
        private ILogger _logger = null;
        private IStorage _storage = null;
        private IDictionary _dictionary = null;
        private ImporterParams _impParams = null;
        private bool _isRunning = false;

        public Importer(CompositionContainer compContainer)
        {

            try
            {
                CompositionContainer = compContainer;

                _logger = Global.Container.GetExport<ILogger>(ConfigurationManager.AppSettings["LoggerType"]).Value;

                InitDAL();
                initDictionary();
                InitStorage();
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

        #region Properties

        public EImportState CurrentState { get; private set; } = EImportState.Idle;

        public DateTime ImportStart { get; private set; } = DateTime.MinValue;

        public DateTime ImportEnd { get; private set; } = DateTime.MinValue;

        public HashSet<string> CompaniesProcessed
        {
            get
            {
                HashSet<string> result = null;
                if(LastImportResults != null)
                {
                    result = new HashSet<string>( LastImportResults.Companies.Select(c => c.CompanyCode) );
                }
                else
                {
                    result = new HashSet<string>();
                }

                return result;
            }
        }

        public ImportResults LastImportResults
        {
            get
            {
                return _currImport;
            }
            private set
            {
                _currImport = value;
            }
        }

        public CompositionContainer CompositionContainer { get; set; } = null;

        #endregion

        public bool StartImport(ImporterParams impParams)
        {
            if (CurrentState == EImportState.Idle)
            {
                _isRunning = true;
                _impParams = impParams;
                CompaniesProcessed.Clear();
                ImportStart = DateTime.UtcNow;

                LastImportResults = new ImportResults();

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
            return true;
        }

        protected void ImportThread()
        {
            _logger.Log(EErrorType.Info, "ImportThread started");
            if (CompositionContainer != null)
            {

                // validating if there are anything need to be imported
                CurrentState = EImportState.Init;

                // TODO: this list need to be extracted from DB
                List<string> regulators = GetRegulatorsToUpdate();

                foreach (var regulator in regulators)
                {
                    // break if stopped
                    if (!_isRunning)
                    {
                        break;
                    }

                    Lazy<ISource> source = null;
                    try
                    {
                        source = CompositionContainer.GetExport<ISource>(regulator);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(ex);
                        LastImportResults.AddError(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Warning, Message = string.Format("ISource importer not registered for '{0}' - skipped", regulator) });
                    }
                    if (source != null && source.Value != null)
                    {
                        // preparing source
                        ISourceInitParams srcInitParams = source.Value.CreateInitParams();
                        srcInitParams.Logger = _logger;
                        srcInitParams.Storage = _storage;
                        srcInitParams.Dictionary = _dictionary;
                        srcInitParams.ExtractFromStorage = Boolean.Parse(ConfigurationManager.AppSettings["LoadFromStorage"]);
                        source.Value.Init(srcInitParams);

                        // getting list of companies
                        List<string> companies = GetCompaniesToUpdate(regulator);

                        List<ImportTaskParams> taskParams = new List<ImportTaskParams>();
                        List<Task> importTasks = new List<Task>();

                        // preparing params

                        int maxImportThreads = Int32.Parse(ConfigurationManager.AppSettings["MaxImportThreads"]);
                        int importThreads = companies.Count >= maxImportThreads ? maxImportThreads : 1;
                        int compsPerThread = (int)Math.Ceiling((decimal)companies.Count / importThreads);
                        for (int i = 0; i < importThreads; ++i)
                        {
                            ImportTaskParams importParams = new ImportTaskParams();
                            importParams.RegulatorCode = regulator;
                            importParams.Source = source.Value;
                            importParams.Companies.AddRange(companies.GetRange(i * compsPerThread, Math.Min(compsPerThread, companies.Count - i * compsPerThread)));

                            taskParams.Add(importParams);
                        }

                        // preparing tasks
                        foreach (var prms in taskParams)
                        {
                            Task importTask = new Task(() => ImportTask(prms));
                            importTasks.Add(importTask);
                            importTask.Start();
                        }

                        Task.WaitAll(importTasks.ToArray());

                    }
                    else
                    {
                        LastImportResults.AddError(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Warning, Message = string.Format("ISource importer not registered for '{0}' - skipped", regulator) });
                    }
                }
            }
            else
            {
                LastImportResults.AddError(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Error, Message = "Import failed. Composition comtainer is NULL" });
            }

            CurrentState = EImportState.Idle;
            _isRunning = false;
            ImportEnd = DateTime.UtcNow;

            _logger.Log(EErrorType.Info, string.Format("ImportThread finished. Total errors: {0}, Time: {1}", LastImportResults.ErrorsLog.Count, ImportEnd - ImportStart));

        }

        class ImportTaskParams
        {
            public ImportTaskParams()
            {
                Companies = new List<string>();
            }

            public string RegulatorCode
            {
                get;
                set;
            }

            public ISource Source
            {
                get;
                set;
            }

            public List<string> Companies
            {
                get;
                set;
            }
        }

        private void ImportTask(ImportTaskParams importParams)
        {
            foreach (string companyCode in importParams.Companies)
            {
                // break if stopped
                if (!_isRunning)
                {
                    break;
                }

                try
                {
                    _logger.Log(EErrorType.Info, string.Format("Processing start:\t{0}", companyCode));
                    ImportPipeline(importParams.RegulatorCode, companyCode, importParams.Source);
                    _logger.Log(EErrorType.Info, string.Format("Processing done:\t{0}", companyCode));
                }
                catch (Exception ex)
                {
                    // SEC stub - when
                    if (ex.Message.Equals("Too Many Requests"))
                    {
                        _isRunning = false;
                    }
                    _logger.Log(EErrorType.Error, string.Format("Error on import - {0}", companyCode));
                    _logger.Log(ex);
                    LastImportResults.AddError(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Error, Message = string.Format("Import failed: Regulator '{0}', Company '{1}', Error '{2}'", importParams.RegulatorCode, companyCode, ex.Message) });

                    
                }
            }

            CurrentState = EImportState.Idle;
        }

        #region Support method
        private void InitStorage()
        {
            bool useStorage = ConfigurationManager.AppSettings["UseStorage"] != null ? Boolean.Parse(ConfigurationManager.AppSettings["UseStorage"]) : false;
            if (useStorage)
            {
                _logger.Log(EErrorType.Info, string.Format("InitStorage: Folder '{0}'", ConfigurationManager.AppSettings["StorageRootFolder"]));

                var storage = CompositionContainer.GetExport<IStorage>(ConfigurationManager.AppSettings["StorageType"]);
                IStorageParams stgParams = storage.Value.CreateStorageParams();
                stgParams.Parameters["RootFolder"] = Path.Combine(CompositionContainer.GetExportedValue<string>("ServiceRootFolder"), ConfigurationManager.AppSettings["StorageRootFolder"]);

                storage.Value.Init(stgParams);

                _storage = storage.Value;
            }


        }
        private void InitDAL()
        {
            _logger.Log(EErrorType.Info, string.Format("InitDAL: Connecting to '{0}'", ConfigurationManager.AppSettings["ConnectionStringFilings"]));

            Lazy<Interfaces.DAL.IDal> dal = CompositionContainer.GetExport<Interfaces.DAL.IDal>();
            Interfaces.DAL.IDalParams dalParams = dal.Value.CreateDalParams();
            dalParams.Parameters.Add("ConnectionStringFilings", ConfigurationManager.AppSettings["ConnectionStringFilings"]);
 
            dal.Value.Init(dalParams);

            _dal = dal.Value;
        }

        private void initDictionary()
        {
            _dictionary = Global.Container.GetExport<IDictionary>("File").Value;
        }
    

        private List<string> GetRegulatorsToUpdate()
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(_impParams.RegulatorCode))
            {
                result.Add(_impParams.RegulatorCode);
            }
            else
            {
                var valDictionary = CompositionContainer.GetExport<IDictionary>("DB");
                if (valDictionary != null && valDictionary.Value != null)
                {
                    List<Interfaces.RegulatorInfo> companies = valDictionary.Value.GetRegulators();
                    if (companies != null)
                    {
                        result.AddRange(companies.Select(x => x.Code));
                    }
                }
                #region deprecated
                // TODO: need to read from dictionary
                /*
                result.Add("SEC");
                 */
                #endregion
            }
            return result;
        }

        private List<string> GetCompaniesToUpdate(string regulatorCode)
        {
            List<string> result = new List<string>();

            if (_impParams.CompanyCodes.Count > 0)
            {
                result.AddRange(_impParams.CompanyCodes.ToArray());
            }
            else
            {
                var valDictionary = CompositionContainer.GetExport<IDictionary>("DB");
                if(valDictionary != null && valDictionary.Value != null)
                {
                    List<Interfaces.CompanyInfo> companies = valDictionary.Value.GetCompaniesByRegulator(regulatorCode);
                    if(companies != null)
                    {
                        result.AddRange( companies.Select(x => x.Code) );
                    }
                }

                #region depricated
                // TODO: right now returning predefined set - need to add access to DB here and read from dictionary
                /*
                result.Add("AAPL");
                result.Add("AMZN");
                result.Add("MMM");
                result.Add("ABT");
                result.Add("ABBV");
                result.Add("ACN");
                result.Add("ATVI");
                result.Add("AYI");
                result.Add("ADBE");
                result.Add("AMD");
                result.Add("AAP");
                result.Add("AES");
                result.Add("AET");
                result.Add("AMG");
                result.Add("AFL");
                result.Add("A");
                result.Add("APD");
                result.Add("AKAM");
                result.Add("ALK");
                result.Add("ALB");
                result.Add("ARE");
                result.Add("ALXN");
                result.Add("ALGN");
                result.Add("ALLE");
                result.Add("AGN");
                result.Add("ADS");
                result.Add("LNT");
                result.Add("ALL");
                result.Add("GOOG");
                result.Add("MO");
                result.Add("AEE");
                result.Add("AAL");
                result.Add("AEP");
                result.Add("AXP");
                result.Add("AIG");
                result.Add("AMT");
                result.Add("AWK");
                result.Add("AMP");
                result.Add("ABC");
                result.Add("AME");
                result.Add("AMGN");
                result.Add("APH");
                result.Add("APC");
                result.Add("ADI");
                result.Add("ANDV");
                result.Add("ANSS");
                result.Add("ANTM");
                result.Add("AON");
                result.Add("AOS");
                result.Add("APA");
                result.Add("AIV");
                result.Add("AMAT");
                result.Add("ADM");
                result.Add("ARNC");
                result.Add("AJG");
                result.Add("AIZ");
                result.Add("T");
                result.Add("ADSK");
                result.Add("ADP");
                result.Add("AZO");
                result.Add("AVB");
                result.Add("AVY");
                result.Add("BHGE");
                result.Add("BLL");
                result.Add("BAC");
                result.Add("BK");
                result.Add("BCR");
                result.Add("BAX");
                result.Add("BBT");
                result.Add("BDX");
                result.Add("BRK.B");
                result.Add("BBY");
                result.Add("BIIB");
                result.Add("BLK");
                result.Add("HRB");
                result.Add("BA");
                result.Add("BWA");
                result.Add("BXP");
                result.Add("BSX");
                result.Add("BHF");
                result.Add("BMY");
                result.Add("AVGO");
                result.Add("BF.B");
                result.Add("CHRW");
                result.Add("CA");
                result.Add("COG");
                result.Add("CPB");
                result.Add("COF");
                result.Add("CAH");
                result.Add("CBOE");
                result.Add("KMX");
                result.Add("CCL");
                result.Add("CAT");
                result.Add("CBG");
                result.Add("CBS");
                result.Add("CELG");
                result.Add("CNC");
                result.Add("CNP");
                result.Add("CTL");
                result.Add("CERN");
                result.Add("CF");
                result.Add("SCHW");
                result.Add("CHTR");
                result.Add("CHK");
                result.Add("CVX");
                result.Add("CMG");
                result.Add("CB");
                result.Add("CHD");
                result.Add("CI");
                result.Add("XEC");
                result.Add("CINF");
                result.Add("CTAS");
                result.Add("CSCO");
                result.Add("C");
                result.Add("CFG");
                result.Add("CTXS");
                result.Add("CLX");
                result.Add("CME");
                result.Add("CMS");
                result.Add("COH");
                result.Add("KO");
                result.Add("CTSH");
                result.Add("CL");
                result.Add("CMCSA");
                result.Add("CMA");
                result.Add("CAG");
                result.Add("CXO");
                result.Add("COP");
                result.Add("ED");
                result.Add("STZ");
                result.Add("COO");
                result.Add("GLW");
                result.Add("COST");
                result.Add("COTY");
                result.Add("CCI");
                result.Add("CSRA");
                result.Add("CSX");
                result.Add("CMI");
                result.Add("CVS");
                result.Add("DHI");
                result.Add("DHR");
                result.Add("DRI");
                result.Add("DVA");
                result.Add("DE");
                result.Add("DLPH");
                result.Add("DAL");
                result.Add("XRAY");
                result.Add("DVN");
                result.Add("DLR");
                result.Add("DFS");
                result.Add("DISCA");
                result.Add("DISH");
                result.Add("DG");
                result.Add("DLTR");
                result.Add("D");
                result.Add("DOV");
                result.Add("DOW");
                result.Add("DPS");
                result.Add("DTE");
                result.Add("DRE");
                result.Add("DD");
                result.Add("DUK");
                result.Add("DXC");
                result.Add("ETFC");
                result.Add("EMN");
                result.Add("ETN");
                result.Add("EBAY");
                result.Add("ECL");
                result.Add("EIX");
                result.Add("EW");
                result.Add("EA");
                result.Add("EMR");
                result.Add("ETR");
                result.Add("EVHC");
                result.Add("EOG");
                result.Add("EQT");
                result.Add("EFX");
                result.Add("EQIX");
                result.Add("EQR");
                result.Add("ESS");
                result.Add("EL");
                result.Add("ES");
                result.Add("RE");
                result.Add("EXC");
                result.Add("EXPE");
                result.Add("EXPD");
                result.Add("ESRX");
                result.Add("EXR");
                result.Add("XOM");
                result.Add("FFIV");
                result.Add("FB");
                result.Add("FAST");
                result.Add("FRT");
                result.Add("FDX");
                result.Add("FIS");
                result.Add("FITB");
                result.Add("FE");
                result.Add("FISV");
                result.Add("FLIR");
                result.Add("FLS");
                result.Add("FLR");
                result.Add("FMC");
                result.Add("FL");
                result.Add("F");
                result.Add("FTV");
                result.Add("FBHS");
                result.Add("BEN");
                result.Add("FCX");
                result.Add("GPS");
                result.Add("GRMN");
                result.Add("IT");
                result.Add("GD");
                result.Add("GE");
                result.Add("GGP");
                result.Add("GIS");
                result.Add("GM");
                result.Add("GPC");
                result.Add("GILD");
                result.Add("GPN");
                result.Add("GS");
                result.Add("GT");
                result.Add("GWW");
                result.Add("HAL");
                result.Add("HBI");
                result.Add("HOG");
                result.Add("HRS");
                result.Add("HIG");
                result.Add("HAS");
                result.Add("HCA");
                result.Add("HCP");
                result.Add("HP");
                result.Add("HSIC");
                result.Add("HSY");
                result.Add("HES");
                result.Add("HPE");
                result.Add("HLT");
                result.Add("HOLX");
                result.Add("HD");
                result.Add("HON");
                result.Add("HRL");
                result.Add("HST");
                result.Add("HPQ");
                result.Add("HUM");
                result.Add("HBAN");
                result.Add("IDXX");
                result.Add("INFO");
                result.Add("ITW");
                result.Add("ILMN");
                result.Add("IR");
                result.Add("INTC");
                result.Add("ICE");
                result.Add("IBM");
                result.Add("INCY");
                result.Add("IP");
                result.Add("IPG");
                result.Add("IFF");
                result.Add("INTU");
                result.Add("ISRG");
                result.Add("IVZ");
                result.Add("IRM");
                result.Add("JEC");
                result.Add("JBHT");
                result.Add("SJM");
                result.Add("JNJ");
                result.Add("JCI");
                result.Add("JPM");
                result.Add("JNPR");
                result.Add("KSU");
                result.Add("K");
                result.Add("KEY");
                result.Add("KMB");
                result.Add("KIM");
                result.Add("KMI");
                result.Add("KLAC");
                result.Add("KSS");
                result.Add("KHC");
                result.Add("KR");
                result.Add("LB");
                result.Add("LLL");
                result.Add("LH");
                result.Add("LRCX");
                result.Add("LEG");
                result.Add("LEN");
                result.Add("LVLT");
                result.Add("LUK");
                result.Add("LLY");
                result.Add("LNC");
                result.Add("LKQ");
                result.Add("LMT");
                result.Add("L");
                result.Add("LOW");
                result.Add("LYB");
                result.Add("MTB");
                result.Add("MAC");
                result.Add("M");
                result.Add("MRO");
                result.Add("MPC");
                result.Add("MAR");
                result.Add("MMC");
                result.Add("MLM");
                result.Add("MAS");
                result.Add("MA");
                result.Add("MAT");
                result.Add("MKC");
                result.Add("MCD");
                result.Add("MCK");
                result.Add("MDT");
                result.Add("MRK");
                result.Add("MET");
                result.Add("MTD");
                result.Add("MGM");
                result.Add("KORS");
                result.Add("MCHP");
                result.Add("MU");
                result.Add("MSFT");
                result.Add("MAA");
                result.Add("MHK");
                result.Add("TAP");
                result.Add("MDLZ");
                result.Add("MON");
                result.Add("MNST");
                result.Add("MCO");
                result.Add("MS");
                result.Add("MOS");
                result.Add("MSI");
                result.Add("MYL");
                result.Add("NDAQ");
                result.Add("NOV");
                result.Add("NAVI");
                result.Add("NTAP");
                result.Add("NFLX");
                result.Add("NWL");
                result.Add("NFX");
                result.Add("NEM");
                result.Add("NWS");
                result.Add("NEE");
                result.Add("NLSN");
                result.Add("NKE");
                result.Add("NI");
                result.Add("NBL");
                result.Add("JWN");
                result.Add("NSC");
                result.Add("NTRS");
                result.Add("NOC");
                result.Add("NRG");
                result.Add("NUE");
                result.Add("NVDA");
                result.Add("ORLY");
                result.Add("OXY");
                result.Add("OMC");
                result.Add("OKE");
                result.Add("ORCL");
                result.Add("PCAR");
                result.Add("PKG");
                result.Add("PH");
                result.Add("PDCO");
                result.Add("PAYX");
                result.Add("PYPL");
                result.Add("PNR");
                result.Add("PBCT");
                result.Add("PEP");
                result.Add("PKI");
                result.Add("PRGO");
                result.Add("PFE");
                result.Add("PCG");
                result.Add("PM");
                result.Add("PSX");
                result.Add("PNW");
                result.Add("PXD");
                result.Add("PNC");
                result.Add("RL");
                result.Add("PPG");
                result.Add("PPL");
                result.Add("PX");
                result.Add("PCLN");
                result.Add("PFG");
                result.Add("PG");
                result.Add("PGR");
                result.Add("PLD");
                result.Add("PRU");
                result.Add("PEG");
                result.Add("PSA");
                result.Add("PHM");
                result.Add("PVH");
                result.Add("QRVO");
                result.Add("PWR");
                result.Add("QCOM");
                result.Add("DGX");
                result.Add("RRC");
                result.Add("RJF");
                result.Add("RTN");
                result.Add("O");
                result.Add("RHT");
                result.Add("REG");
                result.Add("REGN");
                result.Add("RF");
                result.Add("RSG");
                result.Add("RMD");
                result.Add("RHI");
                result.Add("ROK");
                result.Add("COL");
                result.Add("ROP");
                result.Add("ROST");
                result.Add("RCL");
                result.Add("CRM");
                result.Add("SCG");
                result.Add("SLB");
                result.Add("SNI");
                result.Add("STX");
                result.Add("SEE");
                result.Add("SRE");
                result.Add("SHW");
                result.Add("SIG");
                result.Add("SPG");
                result.Add("SWKS");
                result.Add("SLG");
                result.Add("SNA");
                result.Add("SO");
                result.Add("LUV");
                result.Add("SPGI");
                result.Add("SWK");
                result.Add("SPLS");
                result.Add("SBUX");
                result.Add("STT");
                result.Add("SRCL");
                result.Add("SYK");
                result.Add("STI");
                result.Add("SYMC");
                result.Add("SYF");
                result.Add("SNPS");
                result.Add("SYY");
                result.Add("TROW");
                result.Add("TGT");
                result.Add("TEL");
                result.Add("FTI");
                result.Add("TXN");
                result.Add("TXT");
                result.Add("TMO");
                result.Add("TIF");
                result.Add("TWX");
                result.Add("TJX");
                result.Add("TMK");
                result.Add("TSS");
                result.Add("TSCO");
                result.Add("TDG");
                result.Add("TRV");
                result.Add("TRIP");
                result.Add("FOX");
                result.Add("TSN");
                result.Add("UDR");
                result.Add("ULTA");
                result.Add("USB");
                result.Add("UA");
                result.Add("UNP");
                result.Add("UAL");
                result.Add("UNH");
                result.Add("UPS");
                result.Add("URI");
                result.Add("UTX");
                result.Add("UHS");
                result.Add("UNM");
                result.Add("VFC");
                result.Add("VLO");
                result.Add("VAR");
                result.Add("VTR");
                result.Add("VRSN");
                result.Add("VRSK");
                result.Add("VZ");
                result.Add("VRTX");
                result.Add("VIAB");
                result.Add("V");
                result.Add("VNO");
                result.Add("VMC");
                result.Add("WMT");
                result.Add("WBA");
                result.Add("DIS");
                result.Add("WM");
                result.Add("WAT");
                result.Add("WEC");
                result.Add("WFC");
                result.Add("HCN");
                result.Add("WDC");
                result.Add("WU");
                result.Add("WRK");
                result.Add("WY");
                result.Add("WHR");
                result.Add("WFM");
                result.Add("WMB");
                result.Add("WLTW");
                result.Add("WYN");
                result.Add("WYNN");
                result.Add("XEL");
                result.Add("XRX");
                result.Add("XLNX");
                result.Add("XL");
                result.Add("XYL");
                result.Add("YUM");
                result.Add("ZBH");
                result.Add("ZION");
                result.Add("ZTS");
                */
                #endregion
            }
            return result;

        }

        private ISourceFilingsListResult GetFilingsList(string regulatorCode, string companyCode, ISource source)
        {
            _logger.Log(EErrorType.Info, string.Format("ValidateNewReports - {0} / {1}", regulatorCode, companyCode));

            CurrentState = EImportState.ValidatingSource;

            ISourceValidateParams vldParams = source.CreateValidateParams();
            vldParams.RegulatorCode = regulatorCode;
            vldParams.CompanyCode = companyCode;
            vldParams.UpdateFromDate = _impParams.DateStart;
            vldParams.UpdateToDate = _impParams.DateEnd;

            ISourceFilingsListResult vldResult = source.GetFilingsList(vldParams);

            return vldResult;
        }

        private IList<ISourceItemInfo> IdentifyNewFilings(string regulatorCode, string companyCode, List<ISourceItemInfo> itemsAtRegulator)
        {
            var result = new List<ISourceItemInfo>();

            // getting list of exisiting reports
            GetCompanyFilingsInfoParams paramsGetFilingsInfo = new GetCompanyFilingsInfoParams()
            {
                CompanyCode = companyCode,
                RegulatorCode = regulatorCode,
                PeriodStart = _impParams.DateStart,
                PeriodEnd = _impParams.DateEnd
            };

            if(_impParams.Types != null && _impParams.Types.Count > 0)
            {
                paramsGetFilingsInfo.Types = new HashSet<string>(_impParams.Types);
            }
            var resGetCompanyFilingsInfo = _dal.GetCompanyFilingsInfo(paramsGetFilingsInfo);

            if(resGetCompanyFilingsInfo != null && resGetCompanyFilingsInfo.Filings != null)
            {
                // some filings exist for the given set of conditions - finding which reports are new
                HashSet<string> existingReports = new HashSet<string>(resGetCompanyFilingsInfo.Filings.Select( x => x.Name ));
                result.AddRange(
                    itemsAtRegulator.Where(x => !existingReports.Contains(x.Name))
                    );
            }
            else
            {
                result.AddRange(itemsAtRegulator);
            }

            return result;
        }

        private ISourceSubmissionsInfoResult GetListOfSubmissions(string regulatorCode, string companyCode, ISource source, IList<ISourceItemInfo> submissions)
        {
            _logger.Log(EErrorType.Info, string.Format("GetListOfSubmissions - {0} / {1}", regulatorCode, companyCode));

            CurrentState = EImportState.ReadingFilingInfo;
            ISourceSubmissionsInfoParams subInfoParams = source.CreateSourceSubmissionsInfoParams();
            subInfoParams.CompanyCode = companyCode;
            subInfoParams.RegulatorCode = regulatorCode;

            subInfoParams.Items.AddRange(submissions);

            ISourceSubmissionsInfoResult subInfoResult = source.GetSubmissionsInfo(subInfoParams);

            return subInfoResult;
        }

        private void ProcessSubmission(string regulatorCode, 
                                        string companyCode, 
                                        ISource source, 
                                        ISourceSubmissionInfo submissionInfo, 
                                        IParsersRepository parsersRepository, 
                                        CompanyImportResult companyImpResult)
        {
            DateTime dtSrart = DateTime.UtcNow;

            _logger.Log(EErrorType.Info, string.Format("ProcessSubmission - {0} / {1} / {2}", regulatorCode, companyCode, submissionInfo.Name));

            try
            {
                CurrentState = EImportState.ImportSources;

                // filing folder info
                ISourceItemInfo srcItemInfo = source.CreateSourceItemInfo();
                srcItemInfo.Name = submissionInfo.Name;

                // main item to parse
                ISourceItemInfo srcFilingItemInfo = source.CreateSourceItemInfo();
                srcFilingItemInfo.Name = submissionInfo.Report;

                ISourceExtractFilingItemsParams extrItemsParams = source.CreateSourceExtractFilingItemsParams();
                extrItemsParams.RegulatorCode = regulatorCode;
                extrItemsParams.CompanyCode = companyCode;
                extrItemsParams.Filing = srcItemInfo;
                extrItemsParams.Items.Add(srcFilingItemInfo);

                _logger.Log(EErrorType.Info, string.Format("Extracting report files"));

                ISourceExtractResult extrResult = source.ExtractFilingItems(extrItemsParams);

                if (extrResult != null && _isRunning)
                {

                    ISourceItem filingContent = extrResult.Items.FirstOrDefault(i => i.Name == submissionInfo.Report);
                    if (filingContent != null && _isRunning)
                    {
                        MemoryStream ms = new MemoryStream(filingContent.Content.ToArray());

                        // 4. Parsing
                        CurrentState = EImportState.Parsing;

                        if (parsersRepository != null && _isRunning)
                        {
                            IFilingParser parser = parsersRepository.GetParser(companyCode, submissionInfo.Type);

                            if (parser != null && _isRunning)
                            {
                                _logger.Log(EErrorType.Info, string.Format("Parsing: {0}", submissionInfo.Report));

                                IFilingParserParams parserParams = parser.CreateFilingParserParams();
                                parserParams.FileContent = ms;

                                IFilingParserResult parserResults = parser.Parse(parserParams);

                                if (parserResults != null && parserResults.Success)
                                {
                                    // Adding to DB and storage
                                    StoreFiling(regulatorCode, companyCode, submissionInfo, parserResults);

                                    // adding record that submission was processed
                                    FilingImportInfo fii = new FilingImportInfo();
                                    fii.Name = submissionInfo.Name;
                                    fii.Submitted = submissionInfo.Submitted;
                                    fii.Type = submissionInfo.Type;

                                    companyImpResult.Filings.Add(fii);
                                }
                                else
                                {
                                    var parserFailError = new Error()
                                    {
                                        Code = EErrorCodes.ParserError,
                                        Type = EErrorType.Warning,
                                        Message = string.Format("Parser failed for {0} / {1} / {2}, Type {3}",
                                        regulatorCode,
                                        companyCode,
                                        submissionInfo.Name,
                                        submissionInfo.Type)
                                    };

                                    _logger.Log(parserFailError.Type, parserFailError.Message);
                                    LastImportResults.AddError(parserFailError);
                                    if (parserResults != null)
                                    {
                                        foreach (var e in parserResults.Errors)
                                        {
                                            _logger.Log(EErrorType.Error, string.Format("Parser error:\r\n\t Error: {0}\r\n\t: Message: {1}", e.Code.ToString(), e.Message.ToString()));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _logger.Log(EErrorType.Warning, string.Format("Parser not found for filing {0}, Company {1}, Type {2}",
                                    submissionInfo.Name,
                                    companyCode,
                                    submissionInfo.Type));
                            }

                        }

                    }
                    else
                    {
                        _logger.Log(EErrorType.Warning, string.Format("Filing content was not extract from results - skipping {0} / {1} / {2}",
                                    regulatorCode,
                                    companyCode,
                                    submissionInfo.Report
                                    ));
                    }
                }

                DateTime dtEnd = DateTime.UtcNow;
                TimeSpan time = dtEnd - dtSrart;
                _logger.Log(EErrorType.Info, string.Format("\tDONE - {0} / {1} / {2}, Time:\t{3}", regulatorCode, companyCode, submissionInfo.Name, time));
            }
            catch (Exception ex)
            {
                _logger.Log(EErrorType.Error, string.Format("FAIL: submission {0} / {1} / {2},\r\n\tError: {3}\r\n\t{4}", regulatorCode, companyCode, submissionInfo.Name, ex.Message, ex.StackTrace));
            }

        }

        private void ImportPipeline(string regulatorCode, string companyCode, ISource source)
        {
            // Preparing object to store import info
            CompanyImportResult companyImpResult = new CompanyImportResult()
            {
                CompanyCode = companyCode,
                RegulatorCode = regulatorCode,
                ImportStartTime = DateTime.UtcNow
            };

            // 1. validating if new reports are available

            var parsersRepository = CompositionContainer.GetExport<IParsersRepository>(regulatorCode);

            ISourceFilingsListResult vldResult = GetFilingsList(regulatorCode, companyCode, source);

            IList<ISourceItemInfo> newFilings = IdentifyNewFilings(regulatorCode, companyCode, vldResult.Filings);

            // 2. if any - importing from source
            if (vldResult.Success && newFilings != null && newFilings.Count > 0)
            {
                _logger.Log(EErrorType.Info, string.Format("Delta {0} / {1}: {2}", regulatorCode, companyCode, vldResult.Filings.Count));

                ISourceSubmissionsInfoResult subInfoResult = GetListOfSubmissions(regulatorCode, companyCode, source, newFilings);

                if (subInfoResult != null && subInfoResult.Submissions != null && subInfoResult.Submissions.Count > 0)
                {
                    // 3. importing sources

                    foreach (var submissionInfo in subInfoResult.Submissions)
                    {
                        // break if stopped
                        if (!_isRunning)
                        {
                            break;
                        }

                        if (!string.IsNullOrEmpty(submissionInfo.Report))
                        {
                            // checking which types of reports to import - if no types are specified just importing everything
                            if (_impParams.Types.Count == 0 || _impParams.Types.Contains(submissionInfo.Type))
                            {
                                ProcessSubmission(regulatorCode, 
                                                  companyCode, 
                                                  source, 
                                                  submissionInfo, 
                                                  parsersRepository.Value, 
                                                  companyImpResult);
                            }
                        }
                        else
                        {
                            _logger.Log(EErrorType.Warning, string.Format("Report name was not extracted - skipping submission {0}", submissionInfo.Name));
                        }
                    } // foreach
                }
                
            }
            else
            {
                _logger.Log(EErrorType.Info, string.Format("No Delta - skipping {0}", companyCode));
            }

            companyImpResult.ImportEndTime = DateTime.UtcNow;
            // adding to import results
            LastImportResults.AddCompanyImportResult(companyImpResult);

        }

        private bool StoreFiling(string regulatorCode, string companyCode, ISourceSubmissionInfo submissionInfo, IFilingParserResult parserResults)
        {
            DateTime dtLocalStart = DateTime.UtcNow;
            DateTime dtLocalEnd = DateTime.UtcNow;

            int totalRecordsCount = 0;
            foreach (var s in parserResults.Statements)
            {
                totalRecordsCount += s.Records.Count;
            }

            _logger.Log(EErrorType.Info, string.Format("Prepare for saving to STG: {0} / {1} / {2},\t Type - {3}",
                regulatorCode,
                companyCode,
                submissionInfo.Name,
                submissionInfo.Type));

            CurrentState = EImportState.Saving;

            // preparing params
            Interfaces.DAL.InsertFilingDetailsParams insertParams = new Interfaces.DAL.InsertFilingDetailsParams();

            // preparing metadata
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "RegulatorCode", Value = regulatorCode, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "CompanyCode", Value = companyCode, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "FilingName", Value = submissionInfo.Name, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "Source", Value = submissionInfo.Report, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "FilingType", Value = submissionInfo.Type, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "Submitted", Value = submissionInfo.Submitted.ToString(), Type = "DateTime" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "PeriodStart", Value = parserResults.PeriodStart.ToString(), Type = "DateTime" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "PeriodEnd", Value = parserResults.PeriodEnd.ToString(), Type = "DateTime" });

            // preparing filing data records
            foreach (var statement in parserResults.Statements)
            {
                foreach (var r in statement.Records)
                {
                    insertParams.Data.Add(new Interfaces.DAL.FilingRecord()
                    {
                        Code = r.Title,
                        Instant = r.Instant,
                        PeriodEnd = r.PeriodEnd,
                        PeriodStart = r.PeriodStart,
                        Unit = r.Unit,
                        Value = r.Value is decimal ? (decimal?)r.Value : null,
                        Value_Str = r.Value is string ? (string)r.Value : null,
                        Value_Dttm = r.Value is DateTime ? (DateTime?)r.Value : null,
                        SourceFactId = r.SourceFactId,
                        FactId = r.FactId
                    });
                }
            }

            _logger.Log(EErrorType.Info, string.Format("Saving to STG: {0},\tType - {1},\tPeriod: {2} - {3},\tRecords: {4}",
                submissionInfo.Name,
                submissionInfo.Type,
                parserResults.PeriodStart.ToString(),
                parserResults.PeriodEnd.ToString(),
                totalRecordsCount));

            dtLocalStart = DateTime.UtcNow;
            _dal.InsertFilingDetails(insertParams);
            dtLocalEnd = DateTime.UtcNow;

            _logger.Log(EErrorType.Info, string.Format("DAL call: Time {0}", dtLocalEnd - dtLocalEnd));

            return true;


        }
        #endregion
    }
}