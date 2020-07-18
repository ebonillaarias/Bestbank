using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
   public class GetCustomersByCallResponseModel : StatusResponseModel
   {
      /// <summary>
      /// contiene el listado de clientes
      /// </summary>
      public List<CallsCustomersModel> Customers { get; set; }
   }
}



