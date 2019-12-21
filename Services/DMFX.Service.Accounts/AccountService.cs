using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;


namespace DMFX.Service.Accounts
{
    public class AccountService : ServiceBase
    {
                
        public object Any(GetSessionInfo request)
        {
            var response = Global.RequestsProcessor.Any(request);
            TransferHeader(request, response);
            return response;
        }

        public object Any(CreateAccount request)
        {
            var response = Global.RequestsProcessor.Any(request);
            TransferHeader(request, response);
            return response;
        }

        public object Any(InitSession request)
        {
            var response = Global.RequestsProcessor.Any(request);
            response.RequestID = request.RequestID;
            return response;
        }

        public object Any(CloseSession request)
        {
            var response = Global.RequestsProcessor.Any(request);
            TransferHeader(request, response);
            return response;
        }

        public object Any(Login request)
        {
            var response = Global.RequestsProcessor.Any(request);
            TransferHeader(request, response);
            return response;
        }

        public object Any(GetAccountInfo request)
        {
            var response = Global.RequestsProcessor.Any(request);
            TransferHeader(request, response);
            return response;
        }

        public object Any(ActivateAccount request)
        {
            var response = Global.RequestsProcessor.Any(request);
            TransferHeader(request, response);
            return response;
        }

        public object Any(UpdateAccount request)
        {
            var response = Global.RequestsProcessor.Any(request);
            TransferHeader(request, response);
            return response;
        }

        public object Any(ChangePassword request)
        {
            var response = Global.RequestsProcessor.Any(request);
            TransferHeader(request, response);
            return response;
        }

        public object Any(ResetPassword request)
        {
            var response = Global.RequestsProcessor.Any(request);
            TransferHeader(request, response);
            return response;
        }
        
    }
}