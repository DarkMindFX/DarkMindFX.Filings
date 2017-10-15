﻿using DMFX.Interfaces;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DMFX.Service.Scheduler
{
    public class SchedulerService : ServiceBase
    {
        public object Any(StartScheduler request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: StartScheduler");

            StartSchedulerResponse response = new StartSchedulerResponse();

            try
            {

                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {

                    if (Global.Scheduler.CurrentState == Scheduler.ESchedulerState.Idle)
                    {
                        SchedulerParams schdParams = new SchedulerParams();
                        schdParams.SleepDelay = Int32.Parse(ConfigurationManager.AppSettings["SchedulerInterval"]);

                        Global.Scheduler.Start(schdParams);

                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add(new Interfaces.Error() { Code = Interfaces.EErrorCodes.SchedulerBusy, Type = Interfaces.EErrorType.Error, Message = string.Format("Scheduler is running, current state - {0}", Global.Scheduler.CurrentState) });
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
                }

            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unpexcted error: {0}", ex.Message)
                });
            }


            _logger.Log(EErrorType.Info, " ****** Call end: StartScheduler");
            return response;

        }

        public object Any(GetSchedulerState request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetSchedulerState");

            GetSchedulerStateResponse response = new GetSchedulerStateResponse();

            try
            {

                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {
                    response.Success = true;
                    response.State = Global.Scheduler.CurrentState.ToString();                    
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
                }

            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unpexcted error: {0}", ex.Message)
                });
            }


            _logger.Log(EErrorType.Info, " ****** Call end: GetSchedulerState");
            return response;
        }

        protected override bool IsValidSessionToken(RequestBase request)
        {
            return request.SessionToken != null && request.SessionToken.Equals(ConfigurationManager.AppSettings["ServiceSessionToken"]);
        }

    }
}