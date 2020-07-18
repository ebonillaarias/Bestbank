using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace inConcert.iMS.BusinessLogic.Logs
{
   public class SiebelLog : ISiebelLog
   {
      private const string LOG_FILE_NAME = "Siebel_logs.txt";

      public string SiebelBaseAddress { get; }

      public SiebelLog(string baseAddress)
      {
         SiebelBaseAddress = baseAddress;
      }

      public void Write(string service, string request, string response, int typeLog = ISiebelLog.LOGTYPE_SIEBEL)
      {
         using (StreamWriter writetext = new StreamWriter(LOG_FILE_NAME, true))
         {
            StringBuilder strLog = new StringBuilder();

            if (typeLog == ISiebelLog.LOGTYPE_SIEBEL)
            {
               strLog.Append(string.Format("[SIEBEL  ]"));
               strLog.Append(string.Format("[DateTime: {0}]", DateTime.Now.ToString()));
               strLog.Append(string.Format("[Endpoint: {0}{1}]", SiebelBaseAddress, service));
               strLog.Append(string.Format("[Request: {0}]", request));
               strLog.Append(string.Format("[Response: {0}]", response));            
            }
            else if (typeLog == ISiebelLog.LOGTYPE_FIREBASE)
            {
               strLog.Append(string.Format("[FIREBASE]"));
               strLog.Append(string.Format("[DateTime: {0}]", DateTime.Now.ToString()));
               strLog.Append(string.Format("[DeviceToken: {0}", request));
               strLog.Append(string.Format("[Response: {0}]", response));
            }

            writetext.WriteLine(strLog.ToString());
         }
      }
   }
}
