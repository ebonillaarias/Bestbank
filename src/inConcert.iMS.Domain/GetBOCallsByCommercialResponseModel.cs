using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class GetBOCallsByCommercialResponseModel : StatusResponseModel
   {
      public List<CallsRecentsModel> ListCustomer { get; set; }
   }
}
