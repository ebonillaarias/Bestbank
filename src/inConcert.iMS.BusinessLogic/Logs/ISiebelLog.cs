using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.BusinessLogic.Logs
{
   public interface ISiebelLog
   {
      public const int LOGTYPE_SIEBEL = 1;
      public const int LOGTYPE_FIREBASE = 2;

      void Write(string service, string request, string response, int typeLog = LOGTYPE_SIEBEL);
   }
}
