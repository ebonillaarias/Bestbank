using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
    public class CustomerLiteModel
    {
        /// <summary>
        /// El ID del cliente.
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// El nombre del cliente.
        /// </summary>
        public string CustomerName { get; set; }
    }
}
