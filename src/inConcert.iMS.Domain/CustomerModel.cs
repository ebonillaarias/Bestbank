using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
    public class CustomerModel
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
        public CustomerType CustomerType { get; set; }
        /// <summary>
        /// Listado de teléfonos.
        /// </summary>
        public List<PhoneNumberModel> PhoneNumberList { get; set; }
    }
}
