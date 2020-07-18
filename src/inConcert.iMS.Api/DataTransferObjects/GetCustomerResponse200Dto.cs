using System.Collections.Generic;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class GetCustomerResponse200Dto
    {
        /// <summary>
        /// Listado de clientes.
        /// </summary>
        public List<CustomerDto> CustomerList { get; set; }
    }
}
