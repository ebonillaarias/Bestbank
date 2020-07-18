using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
   public class GetBOCallsCustomersByCommercialsResponseModel : StatusResponseModel
   {
        /// <summary>
        /// contiene el listado de clientes
        /// </summary>
        public List<CustomerLiteModel> Customers { get; set; }
    }
}
