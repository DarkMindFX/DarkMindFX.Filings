using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SECFilingsImporter
{
    class Program
    {
        static DMFX.Client.FilingsSourcing.ServiceClient fsClinet;
        static DMFX.Client.TechUtils.ServiceClient tuClient;

        static string sessionToken;
        static string techUtilsToken;
        static int sanitizePeriodMins = 60;

        static void Main(string[] args)
        {
            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;
            DateTime dtLastSanitize = DateTime.MinValue;

            try
            {
                Console.WriteLine(string.Format("[{0}] Import Started", dtStart));

                sessionToken = ConfigurationManager.AppSettings["FilingsSourcingServiceToken"];
                techUtilsToken = ConfigurationManager.AppSettings["TechUtilsServiceToken"];
                sanitizePeriodMins = Int32.Parse(ConfigurationManager.AppSettings["SanitizePeriodMinutes"]);

                DateTime periodEnd = ConfigurationManager.AppSettings["PeriodEnd"] != null ? DateTime.Parse(ConfigurationManager.AppSettings["PeriodEnd"]) : DateTime.Now;
                DateTime periodStart = ConfigurationManager.AppSettings["PeriodStart"] != null ? DateTime.Parse(ConfigurationManager.AppSettings["PeriodStart"]) : DateTime.Now - TimeSpan.FromDays(365);
                int importPeriodDays = Int32.Parse(ConfigurationManager.AppSettings["ImportPeriodDays"]);
                int statePollPeriodSeconds = Int32.Parse(ConfigurationManager.AppSettings["StatePollPeriodSeconds"]);
                string regulator = ConfigurationManager.AppSettings["Regulator"];
                string symbols = ConfigurationManager.AppSettings["Symbols"];
                string types = ConfigurationManager.AppSettings["Types"];

                DateTime impPeriodEnd = periodEnd;
                DateTime impPeriodStart = periodEnd - TimeSpan.FromDays(importPeriodDays);

                fsClinet = new DMFX.Client.FilingsSourcing.ServiceClient();
                tuClient = new DMFX.Client.TechUtils.ServiceClient();

                ForceRunImport reqRunImport = new ForceRunImport();
                reqRunImport.CompanyCodes = !string.IsNullOrEmpty(symbols) ? symbols.Split(new char[] { ',' }) : null;
                reqRunImport.RegulatorCode = regulator;
                reqRunImport.SessionToken = sessionToken;
                reqRunImport.Types = !string.IsNullOrEmpty(types) ? types.Split(new char[] { ',' }) : null;

                GetImporterState reqGetImpState = new GetImporterState();
                reqGetImpState.SessionToken = sessionToken;

                while (impPeriodStart >= periodStart)
                {
                    reqRunImport.DateEnd = impPeriodEnd;
                    reqRunImport.DateStart = impPeriodStart;

                    Console.WriteLine(string.Format("[{0}] Importing for {1} - {2}", DateTime.Now, impPeriodStart.ToShortDateString(), impPeriodEnd.ToShortDateString()));

                    try
                    {
                        var resRunImport = fsClinet.PostForceRunImport(reqRunImport);
                        if (resRunImport.Success)
                        {
                            bool isRunning = true;
                            while (isRunning)
                            {
                                // checking if sanitization is needed
                                if (DateTime.Now - dtLastSanitize >= TimeSpan.FromMinutes(sanitizePeriodMins))
                                {
                                    Sanitize();
                                    dtLastSanitize = DateTime.Now;

                                }

                                // waiting
                                Thread.Sleep(statePollPeriodSeconds * 1000);

                                try
                                {
                                    // checking state
                                    var resGetImpState = fsClinet.PostGetImporterState(reqGetImpState);
                                    if (resGetImpState.Success && resGetImpState.Payload.State == "Idle")
                                    {
                                        isRunning = false;
                                        Console.WriteLine(string.Format("[{0}] Import Completed, State = {1}, Imported = {2}", DateTime.Now, resGetImpState.Payload.State, resGetImpState.Payload.ProcessedCount));
                                    }
                                    else
                                    {
                                        Console.WriteLine(string.Format("[{0}] Importing, State = {1}, Imported = {2}", DateTime.Now, resGetImpState.Payload.State, resGetImpState.Payload.ProcessedCount));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(string.Format("[{0}] Exception: {1} PostGetImporterState failed", DateTime.Now, ex.Message));
                                    // waiting
                                    Thread.Sleep(statePollPeriodSeconds * 1000);
                                }
                            }

                            impPeriodEnd = impPeriodStart;
                            impPeriodStart = impPeriodEnd - TimeSpan.FromDays(importPeriodDays);

                        }
                        else
                        {
                            Console.WriteLine(string.Format("Failed to impoty for {0} - {1}, Reason: {2}, {3}", impPeriodStart, impPeriodEnd, resRunImport.Errors[0].Code, resRunImport.Errors[0].Message));
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("[{0}] Exception: {1} PostForceRunImport failed", DateTime.Now, ex.Message));
                        // waiting
                        Thread.Sleep(statePollPeriodSeconds * 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("[{0}] Exception: {1}\r\n\tStack: {2}", DateTime.Now, ex.Message, ex.StackTrace));
            }
            finally
            {
                dtEnd = DateTime.Now;
            }

            Console.WriteLine(string.Format("[{0}] Import Completed", dtEnd));

        }

        static void Sanitize()
        {
            Sanitize reqSanitize = new Sanitize();
            reqSanitize.SessionToken = techUtilsToken;

            SanitizeResponse respSanitize = tuClient.PostSanitize(reqSanitize);
            if (respSanitize.Success)
            {
                Console.WriteLine(string.Format("[{0}] Sanitization - Success", DateTime.Now));
            }
            else
            {
                Console.WriteLine(string.Format("[{0}] Sanitization - Failed: Error Code - {1}, Message: {2}",
                    DateTime.Now, respSanitize.Errors[0].Code, respSanitize.Errors[0].Message));
            }
        }
    }
}
