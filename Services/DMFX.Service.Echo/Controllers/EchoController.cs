using DMFX.Interfaces;
using DMFX.Service.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace DMFX.Service.Echo.Controllers
{
    

    public class EchoController : ApiController
    {
        [Route("Echo/{Message}/{RequestID}/{SessionToken}")]
        public IHttpActionResult GetEcho(string Message, string RequestID, string SessionToken)
        {
            //ThreadPool.SetMinThreads(100, 10);
            EchoResponse response = new EchoResponse();
            try
            {
                string echo2Resp = string.Empty;
                using (var client = new HttpClient())
                {
                    /*
                    var sResp = client.DownloadString("http://192.168.0.103/api/echoservice2/Echo/" + Message +"/2/3");
                    */
                    var sResp = client.GetAsync("http://localhost/api/echoservice2/Echo/" + Message + "/2/3");
                    sResp.Wait();

                    var result = sResp.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsAsync<EchoResponse>();
                        readTask.Wait();

                        var echoResponse = readTask.Result;

                        echo2Resp = string.Format("EchoService2 responded - {0}", echoResponse.Payload.Message);
                    }
                    
                }

                response.Payload.Message = "Echo: " + echo2Resp;
                response.Success = true;
            }
            catch(Exception ex)
            {
                response.Payload.Message = "Echo: Error!";
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = ex.Message });
                response.Success = false;
            }           
            

            return Ok(response);
        }

    }
}
