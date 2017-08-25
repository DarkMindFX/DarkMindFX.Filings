using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMFX.Service.Sourcing
{
    public class SourcingService : ServiceStack.Service
    {
        public object Any(Echo request)
        {
            EchoResponse response = new EchoResponse();
            TransferHeader(request, response);

            response.Message = request.Message;
            response.Success = true;

            return response;
        }

        public object Any(GetImportLastRun request)
        {
            GetImportLastRunResponse response = new GetImportLastRunResponse();
            TransferHeader(request, response);

            response.LastRun = Global.Importer.LastRun;
            response.Success = true;

            return response;
        }

        public object Any(GetImportCurrentState request)
        {
            GetImportCurrentStateResponse response = new GetImportCurrentStateResponse();
            TransferHeader(request, response);

            response.State = Global.Importer.CurrentState.ToString();
            response.Success = true;

            return response;
        }

        public object Any(ForceRunImport request)
        {
            ForceRunImportResponse response = new ForceRunImportResponse();
            TransferHeader(request, response);

            if (Global.Importer.CurrentState == Importer.EImportState.Idle)
            {
                 response.Success = Global.Importer.StartImport();
            }
            else
            {
                response.Errors.Add(new Interfaces.Error() { Code = Interfaces.EErrorCodes.ImporterError, Type = Interfaces.EErrorType.Error, Message = string.Format("Importing is running, current state - {0}", Global.Importer.CurrentState) });
                response.Success = false;
            }
            

            return response;
        }

        #region Support methods
        protected void TransferHeader(RequestBase request, ResponseBase response)
        {
            response.RequestID = request.RequestID;
            response.SessionToken = request.SessionToken;
        }
        #endregion
    }
}