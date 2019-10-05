using DMFX.Client.Accounts.Properties;
using DMFX.Client.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Client.Accounts
{
    public class ServiceClient : ServiceClientBase
    {
        string serviceUrl = null;

        public ServiceClient()
        {
            serviceUrl = Resources.ServiceUrl;
            ServiceClientBaseInitParams initParams = new ServiceClientBaseInitParams();
            initParams.ServiceUrl = serviceUrl;

            Init(initParams);
        }

        public ActivateAccountResponse PostActivateAccount(ActivateAccount request)
        {
            var response = _client.Post<ActivateAccountResponse>(request);

            return response;
        }

        public ChangePasswordResponse PostChangePassword(ChangePassword request)
        {
            var response = _client.Post<ChangePasswordResponse>(request);

            return response;
        }

        public CloseSessionResponse PostCloseSession(CloseSession request)
        {
            var response = _client.Post<CloseSessionResponse>(request);

            return response;
        }

        public CreateAccountResponse PostCreateAccount(CreateAccount request)
        {
            var response = _client.Post<CreateAccountResponse>(request);

            return response;
        }

        public GetAccountInfoResponse PostGetAccountInfo(GetAccountInfo request)
        {
            var response = _client.Post<GetAccountInfoResponse>(request);

            return response;
        }

        public GetSessionInfoResponse PostGetSessionInfo(GetSessionInfo request)
        {
            var response = _client.Post<GetSessionInfoResponse>(request);

            return response;
        }

        public InitSessionResponse PostInitSession(InitSession request)
        {
            var response = _client.Post<InitSessionResponse>(request);

            return response;
        }

        public LoginResponse PostLogin(Login request)
        {
            var response = _client.Post<LoginResponse>(request);

            return response;
        }

        public ResetPasswordResponse PostResetPassword(ResetPassword request)
        {
            var response = _client.Post<ResetPasswordResponse>(request);

            return response;
        }

        public UpdateAccountResponse PostUpdateAccount(UpdateAccount request)
        {
            var response = _client.Post<UpdateAccountResponse>(request);

            return response;
        }
    }
}
