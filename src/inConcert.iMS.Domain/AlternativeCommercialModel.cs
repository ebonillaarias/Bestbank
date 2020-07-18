using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
    public class AlternativeCommercialModel
    {
        /// <summary>
        /// Número de orden del comercial alternativo.
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// Datos del comercial alternativo.
        /// </summary>
        public CommercialModel Commercial { get; set; }
   }
}
