using System;
using System.Collections.Generic;
using System.Text;
using inConcert.iMS.Enums;

namespace inConcert.iMS.Domain
{
   public class CallsRecentsModel
   {
      public string CalledNumber { get; set; }

      public CallDirection Direction { get; set; }

      public CallResult CallPartResult { get; set; }

      public DateTimeOffset DateTime { get; set; }

      public CustomerModel Customer { get; set; }
   }
}
