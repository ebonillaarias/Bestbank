using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.iPBX.Api.DataTransferObjects
{
    public class RequestEndCallPartDto
    {
        /// <summary>
        ///  Identificador único de llamada.
        /// </summary>
        [Required]
        public string CallId { get; set; }
        /// <summary>
        /// Número secuencial de cada parte de una llamada, comenzando en 1.
        /// </summary>
        [Required]
        public string CallPartNumber { get; set; }
        /// <summary>
        ///  Posibles valores “NoAnswer”, “Rejected”, “Completed” y “Transferred”.
        /// </summary>
        [Required]
        public string CallResult { get; set; }
        /// <summary>
        ///  Posibles valores “Comercial” y “Cliente”.
        /// </summary>
        [Required]
        public string CallPartEndedBy { get; set; }
    }
}
