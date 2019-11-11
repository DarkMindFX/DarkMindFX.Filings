using DMFX.BEA.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DMFX.Test.BEAApi
{
    [TestClass]
    public class TestBEAApiCalls
    {
        [TestMethod]
        public void GetDataSetList_Success()
        {
            DMFX.BEA.Api.BEAApi api = new DMFX.BEA.Api.BEAApi();

            DMFX.BEA.Api.BEAApi.CallParams callParams = new BEA.Api.BEAApi.CallParams();

            BEAResponse<BEAGetDataSetList> response = api.GetDataSetList(callParams);

            Assert.IsTrue(response.BEAAPI != null);
            Assert.IsTrue(response.BEAAPI.Request != null);
            Assert.IsTrue(response.BEAAPI.Results != null);
            Assert.IsTrue(response.BEAAPI.Results.Error == null);
            Assert.IsTrue(response.BEAAPI.Request.RequestParam != null);
            Assert.IsTrue(response.BEAAPI.Request.RequestParam.Count > 0);
        }

        [TestMethod]
        public void GetParameterList_Success()
        {
            DMFX.BEA.Api.BEAApi api = new DMFX.BEA.Api.BEAApi();

            DMFX.BEA.Api.BEAApi.CallParams callParams = new BEA.Api.BEAApi.CallParams();
            callParams.Params.Add("datasetname", "Regional");

            BEAResponse<BEAGetParameterList> response = api.GetParameterList(callParams);

            Assert.IsTrue(response.BEAAPI != null);
            Assert.IsTrue(response.BEAAPI.Request != null);
            Assert.IsTrue(response.BEAAPI.Results != null);
            Assert.IsTrue(response.BEAAPI.Results.Error == null);
            Assert.IsTrue(response.BEAAPI.Request.RequestParam != null);
            Assert.IsTrue(response.BEAAPI.Request.RequestParam.Count > 0);
        }

        [TestMethod]
        public void GetParameterValues_Success()
        {
            DMFX.BEA.Api.BEAApi api = new DMFX.BEA.Api.BEAApi();

            DMFX.BEA.Api.BEAApi.CallParams callParams = new BEA.Api.BEAApi.CallParams();
            callParams.Params.Add("datasetname", "Regional");
            callParams.Params.Add("ParameterName", "TableName");

            BEAResponse<BEAGetParameterValues> response = api.GetParameterValues(callParams);

            Assert.IsTrue(response.BEAAPI != null);
            Assert.IsTrue(response.BEAAPI.Request != null);
            Assert.IsTrue(response.BEAAPI.Results != null);
            Assert.IsTrue(response.BEAAPI.Results.Error == null);
            Assert.IsTrue(response.BEAAPI.Request.RequestParam != null);
            Assert.IsTrue(response.BEAAPI.Request.RequestParam.Count > 0);
        }

        [TestMethod]
        public void GetParametersDetails_Success()
        {
            DMFX.BEA.Api.BEAApi api = new DMFX.BEA.Api.BEAApi();

            DMFX.BEA.Api.BEAApi.CallParams callParamsGetDataSetList = new BEA.Api.BEAApi.CallParams();

            BEAResponse<BEAGetDataSetList> respGetDataSetList = api.GetDataSetList(callParamsGetDataSetList);

            Dictionary<string, BEAParamValueItem> tables = new Dictionary<string, BEAParamValueItem>();

            foreach(var ds in respGetDataSetList.BEAAPI.Results.Dataset)
            {
                if (ds.DatasetName != "RegionalData")
                {
                    // getting list of parameters
                    DMFX.BEA.Api.BEAApi.CallParams callParamsGetParameterList = new BEA.Api.BEAApi.CallParams();
                    callParamsGetParameterList.Params.Add("datasetname", ds.DatasetName);

                    BEAResponse<BEAGetParameterList> respGetParameterList = api.GetParameterList(callParamsGetParameterList);

                    // getting list of tables
                    foreach(var item in respGetParameterList.BEAAPI.Results.Parameter)
                    {
                        DMFX.BEA.Api.BEAApi.CallParams callParamsGetParameterValues = new BEA.Api.BEAApi.CallParams();
                        callParamsGetParameterValues.Params.Add("datasetname", ds.DatasetName);
                        callParamsGetParameterValues.Params.Add("ParameterName", item.ParameterName);

                        BEAResponse<BEAGetParameterValues> response = api.GetParameterValues(callParamsGetParameterValues);

                        Assert.IsTrue(response.BEAAPI != null);
                        Assert.IsTrue(response.BEAAPI.Request != null);
                        Assert.IsTrue(response.BEAAPI.Results != null);
                        Assert.IsTrue(response.BEAAPI.Results.Error == null);
                        Assert.IsTrue(response.BEAAPI.Request.RequestParam != null);
                        Assert.IsTrue(response.BEAAPI.Request.RequestParam.Count > 0);                        
                    }
               }

            }
        }
    }
}
