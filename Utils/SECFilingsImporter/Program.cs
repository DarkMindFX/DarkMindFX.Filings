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
        static void Main(string[] args)
        {
            string sessionToken = ConfigurationManager.AppSettings["FilingsSourcingServiceToken"];
            DateTime periodEnd = ConfigurationManager.AppSettings["PeriodEnd"] != null ? DateTime.Parse(ConfigurationManager.AppSettings["PeriodEnd"]) : DateTime.Now;
            DateTime periodStart = ConfigurationManager.AppSettings["PeriodStart"] != null ? DateTime.Parse(ConfigurationManager.AppSettings["PeriodStart"]) : DateTime.Now - TimeSpan.FromDays(365);
            int importPeriodDays = Int32.Parse(ConfigurationManager.AppSettings["ImportPeriodDays"]);
            int statePollPeriodSeconds = Int32.Parse(ConfigurationManager.AppSettings["StatePollPeriodSeconds"]);
            string regulator = ConfigurationManager.AppSettings["Regulator"];
            string symbols = ConfigurationManager.AppSettings["Symbols"];

            DateTime impPeriodEnd = periodEnd;
            DateTime impPeriodStart = periodEnd - TimeSpan.FromDays(importPeriodDays);

            DMFX.Client.FilingsSourcing.ServiceClient fsClinet = new DMFX.Client.FilingsSourcing.ServiceClient();

            ForceRunImport reqRunImport = new ForceRunImport();
            reqRunImport.CompanyCodes = !string.IsNullOrEmpty(symbols) ? symbols.Split(new char[] { ',' }) : null;
            reqRunImport.RegulatorCode = regulator;
            reqRunImport.SessionToken = sessionToken;

            GetImporterState reqGetImpState = new GetImporterState();
            reqGetImpState.SessionToken = sessionToken;

            while (impPeriodStart > periodStart)
            {
                reqRunImport.DateEnd = impPeriodEnd;
                reqRunImport.DateStart = impPeriodStart;

                Console.WriteLine(string.Format("Importing for {0} - {1}", impPeriodStart, impPeriodEnd));

                var resRunImport = fsClinet.PostForceRunImport(reqRunImport);
                if(resRunImport.Success)
                {
                    bool isRunning = true;
                    while(isRunning)
                    {
                        Thread.Sleep(statePollPeriodSeconds * 1000);
                        var resGetImpState = fsClinet.PostGetImporterState(reqGetImpState);
                        if(resGetImpState.Success && resGetImpState.Payload.State == "Idle" )
                        {
                            isRunning = false;
                            Console.WriteLine(string.Format("[{1}] Import Completed, State = {0}", resGetImpState.Payload.State, DateTime.Now));
                        }
                        else
                        {
                            Console.WriteLine(string.Format("[{1}] Importing, State = {0}", resGetImpState.Payload.State, DateTime.Now));
                        }
                    }

                    impPeriodEnd = impPeriodStart;
                    impPeriodStart = impPeriodEnd - TimeSpan.FromDays(importPeriodDays);

                }
                else
                {
                    Console.WriteLine(string.Format("Failed to impoty for {0} - {1}", impPeriodStart, impPeriodEnd));
                    break;
                }



            }

        }
    }
}
