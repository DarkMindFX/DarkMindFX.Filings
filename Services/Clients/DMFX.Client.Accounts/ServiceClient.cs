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
            var response = Post<ActivateAccount, ActivateAccountResponse>("ActivateAccount", request);

            return response;
        }

        public ChangePasswordResponse PostChangePassword(ChangePassword request)
        {
            var response = Post<ChangePassword, ChangePasswordResponse>("ChangePassword", request);

            return response;
        }

        public CloseSessionResponse PostCloseSession(CloseSession request)
        {
            var response = Post<CloseSession, CloseSessionResponse>("CloseSession", request);

            return response;
        }

        public CreateAccountResponse PostCreateAccount(CreateAccount request)
        {
            var response = Post<CreateAccount, CreateAccountResponse>("CreateAccount", request);

            return response;
        }

        public GetAccountInfoResponse PostGetAccountInfo(GetAccountInfo request)
        {
            var response = Post<GetAccountInfo, GetAccountInfoResponse>("GetAccountInfo", request);

            return response;
        }

        public GetSessionInfoResponse PostGetSessionInfo(GetSessionInfo request)
        {
            var response = Post<GetSessionInfo, GetSessionInfoResponse>("GetSessionInfo",request);

            return response;
        }

        public InitSessionResponse PostInitSession(InitSession request)
        {
            var response = Post<InitSession, InitSessionResponse>("InitSession", request);

            return response;
        }

        public LoginResponse PostLogin(Login request)
        {
            var response = Post<Login, LoginResponse>("Login", request);

            return response;
        }

        public ResetPasswordResponse PostResetPassword(ResetPassword request)
        {
            var response = Post<ResetPassword, ResetPasswordResponse>("ResetPassword", request);

            return response;
        }

        public UpdateAccountResponse PostUpdateAccount(UpdateAccount request)
        {
            var response = Post<UpdateAccount, UpdateAccountResponse>("UpdateAccount", request);

            return response;
        }
    }
}
