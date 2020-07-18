using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class StatusResponseModel
   {
      public ResultStatus Status { get; set; }

      public string Message { get; set; }
   }
}
