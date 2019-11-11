using DMFX.BEA.Api;
using DMFX.Interfaces;
using DMFX.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAEDump
{
    class Program
    {
        private static ILogger _logger = null;
        private static DMFX.BEA.Api.BEAApi _beaClient = null;
        static void Main(string[] args)
        {
            Console.WriteLine("Reading...");
            DirectoryInfo dirInfo = Directory.CreateDirectory(ConfigurationManager.AppSettings["LogsFolder"]);

            _logger = new FileLogger();
            ILoggerParams loggerParams = _logger.CreateParams();

            

            loggerParams.Parameters["LogFolder"] = ConfigurationManager.AppSettings["LogsFolder"];
            loggerParams.Parameters["NameTemplate"] = ConfigurationManager.AppSettings["LogFileNameTemplate"];

            _logger.Init(loggerParams);

            _beaClient = new DMFX.BEA.Api.BEAApi();

            DMFX.BEA.Api.BEAApi.CallParams callParams = new DMFX.BEA.Api.BEAApi.CallParams();

            BEAResponse<BEAGetDataSetList> respGetDataSetList = _beaClient.GetDataSetList(callParams);

            foreach (var ds in respGetDataSetList.BEAAPI.Results.Dataset)
            {
                if (ds.DatasetName != "RegionalData")
                {
                    _logger.Log(EErrorType.Info, string.Format("DATASET: {0}\tDesc: {1}", ds.DatasetName, ds.DatasetDescription));
                    DumpDataSet(ds.DatasetName);
                }
            }

            Console.WriteLine("Dumping...");

            System.Threading.Thread.Sleep(60000);

            Console.WriteLine("Done");
        }

        static void DumpDataSet(string dataset)
        {
            Console.WriteLine(string.Format("Dataset - {0}", dataset));
            // getting list of parameters
            DMFX.BEA.Api.BEAApi.CallParams callParamsGetParameterList = new DMFX.BEA.Api.BEAApi.CallParams();
            callParamsGetParameterList.Params.Add("datasetname", dataset);

            BEAResponse<BEAGetParameterList> respGetParameterList = _beaClient.GetParameterList(callParamsGetParameterList);

            // getting list of tables
            foreach (var item in respGetParameterList.BEAAPI.Results.Parameter)
            {
                _logger.Log(EErrorType.Info, string.Format("Param: {0}\tDesc: {1}\r\n\t\tData Type: {2}\r\n\t\tIs Required: {3}\r\n\tMultipleAccepted: {4}\r\n\t\tAll Value: {5}", 
                            item.ParameterName, 
                            item.ParameterDescription, 
                            item.ParameterDataType,
                            item.ParameterIsRequiredFlag,
                            item.MultipleAcceptedFlag,
                            item.AllValue));

                DMFX.BEA.Api.BEAApi.CallParams callParamsGetParameterValues = new DMFX.BEA.Api.BEAApi.CallParams();
                callParamsGetParameterValues.Params.Add("datasetname", dataset);
                callParamsGetParameterValues.Params.Add("ParameterName", item.ParameterName);

                _logger.Log(EErrorType.Info, "\tValues:");

                BEAResponse<BEAGetParameterValues> respGetParameterValues = _beaClient.GetParameterValues(callParamsGetParameterValues);
                foreach(var paramVal in respGetParameterValues.BEAAPI.Results.ParamValue)
                {
                    _logger.Log(EErrorType.Info, string.Format("\t{0}: {1}", paramVal.Key, paramVal.Desc));
                }
                
            }
        }
    }
}
