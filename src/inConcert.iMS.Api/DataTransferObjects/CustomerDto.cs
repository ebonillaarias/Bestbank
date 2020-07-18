using inConcert.iMS.Enums;
using System.Collections.Generic;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class CustomerDto
    {
        /// <summary>
        /// El ID del cliente.
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// El nombre del cliente.
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// El tipo cliente.
        /// </summary>
        public string CustomerType { get; set; }
        /// <summary>
        /// Listado de teléfonos.
        /// </summary>
        public List<PhoneNumberDto> PhoneNumberList { get; set; }
    }
}
