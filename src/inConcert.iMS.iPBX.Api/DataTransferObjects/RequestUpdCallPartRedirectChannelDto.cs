using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.iPBX.Api.DataTransferObjects
{
    public class RequestUpdCallPartRedirectChannelDto
    {
        /// <summary>
        /// Identificador único de llamada.
        /// </summary>
        [Required]
        public string CallId { get; set; }
        /// <summary>
        /// Número secuencial de cada parte de una llamada, comenzando en 1.
        /// </summary>
        [Required]
        public string CallPartNumber { get; set; }
        /// <summary>
        ///  Nombre del canal SIP a utilizar como parámetro RedirectChannel en
        ///  las llamadas al método RedirectCall del interfaz iAMI.
        /// </summary>
        [Required]
        public string RedirectChannel { get; set; }
    }
}
