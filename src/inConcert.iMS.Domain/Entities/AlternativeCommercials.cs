using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain.Entities
{
    public class AlternativeCommercials
    {
        /// <summary>
        /// Id del comercial principal.
        /// </summary>
        public int CommercialId { get; set; }
        public Commercials Commercial { get; set; }

        /// <summary>
        /// Id del comercial alternativo.
        /// </summary>
        public int AlternativeCommercialId { get; set; }
        public Commercials AlternativeCommercialProp { get; set; }

        /// <summary>
        /// Número de orden del comercial alternativo para el comercial principal.
        /// </summary>
        public int Order { get; set; }
    }
}
