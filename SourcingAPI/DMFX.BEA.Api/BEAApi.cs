using DMFX.BEA.Api.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DMFX.BEA.Api
{

    class BEAParamValueItemConverter : JavaScriptConverter
    {
        public BEAParamValueItemConverter()
        {
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get 
            { 
                return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(BEAParamValueItem) })); 
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (type == typeof(BEAParamValueItem))
            {
                // Create the instance to deserialize into.
                BEAParamValueItem item = new BEAParamValueItem();

                item.Key = dictionary[dictionary.Keys.First()] as string;
                item.Desc = dictionary[dictionary.Keys.Last()] as string;

                return item;
            }
            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class BEAApi
    {
        private static string BaseURL = "https://apps.bea.gov/api/data?";
        private static string ResultFormat = "JSON";
        private static string UserIDParam = "UserID";
        private static string MethodParam = "method";
        private static string ResultFormatParam = "ResultFormat";

        private string _apiKey = string.Empty;

        public class CallParams
        {
            public CallParams()
            {
                Params = new Dictionary<string, string>();
            }

            public Dictionary<string, string> Params
            {
                get;
                set;
            }
        }

        public BEAApi()
        {
            _apiKey = Resources.BEAKey;
        }

        public BEAResponse<BEAGetParameterList> GetParameterList(CallParams callParams)
        {
            BEAResponse<BEAGetParameterList> response = CallMethod<BEAGetParameterList>("GetParameterList", callParams);
            
            return response;
        }

        public BEAResponse<BEAGetDataSetList> GetDataSetList(CallParams callParams)
        {
            BEAResponse<BEAGetDataSetList> response = CallMethod<BEAGetDataSetList>("GetDataSetList", callParams);

            return response;
        }

        public BEAResponse<BEAGetParameterValues> GetParameterValues(CallParams callParams)
        {
            BEAResponse<BEAGetParameterValues> response = CallMethod<BEAGetParameterValues>("GetParameterValues", callParams);

            return response;
        }

        #region Support methods
        BEAResponse<TResults> CallMethod<TResults>(string method, CallParams callParams)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string url = PrepareUrl(method, callParams);
            using (var client = new BEAWebClient())
            { 
                var json = client.DownloadString(url);
                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new BEAParamValueItemConverter() });
                BEAResponse<TResults> model = serializer.Deserialize<BEAResponse<TResults>>(json);
                return model;
            }

        }

        string PrepareUrl(string method, CallParams callParams)
        {
            string result = BaseURL + "&" + MethodParam + "=" + method + "&" + UserIDParam + "=" + _apiKey;
            if (callParams != null)
            {
                foreach (var k in callParams.Params.Keys)
                {
                    string v = callParams.Params[k];

                    result += "&" + k + "=" + v;
                }
            }

            result += "&" + ResultFormatParam + "=" + ResultFormat;
 

            return result;
        }

        #endregion
    }
}
