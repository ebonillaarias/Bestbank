using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
   public class GetCustomersResponseModel : StatusResponseModel
   {
      /// <summary>
      /// contiene el listado de clientes
      /// </summary>
      public List<CustomerModel> Customers { get; set; }
   }
}
