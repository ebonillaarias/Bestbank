using inConcert.iMS.Domain;
using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
   public class GetCustomersResult : StatusResponseModel
   {
      public List<CustomerModel> CustomerList { get; set; }
   }
}
