using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DMFX.Service.Echo.Controllers
{
    public class Echo2Controller : ApiController
    {
        [Route("Echo/{Message}/{RequestID}/{SessionToken}")]
        public IHttpActionResult GetEcho(string Message, string RequestID, string SessionToken)
        {
            EchoResponse response = new EchoResponse();
            response.Payload.Message = "Echo 2: " + Message;
            response.Success = true;

            return Ok(response);
        }

    }
}
