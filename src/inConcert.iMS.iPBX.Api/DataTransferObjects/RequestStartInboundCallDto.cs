using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.iPBX.Api.DataTransferObjects
{
    public class RequestStartInboundCallDto
    {
        /// <summary>
        /// Teléfono llamante. Es el número de teléfono de un cliente.
        /// </summary>
        [Required]
        public string CallerId { get; set; }
        /// <summary>
        /// Teléfono llamado.
        /// </summary>
        [Required]
        public string CalledId { get; set; }
    }
}
