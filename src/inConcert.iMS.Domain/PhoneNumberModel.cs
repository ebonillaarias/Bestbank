using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
    public class PhoneNumberModel
    {
        /// <summary>
        /// El número de teléfono del cliente.
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// El tipo de llamada.
        /// </summary>
        public PhoneType PhoneType { get; set; }
    }
}
