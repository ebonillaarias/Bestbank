using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.iPBX.Api.DataTransferObjects
{
    public class RequestStartOutboundCallDto
    {
        /// <summary>
        ///  Es el Peer del comercial que inicia la llamada.
        /// </summary>
        [Required]
        public string Peer { get; set; }
        /// <summary>
        ///  Teléfono llamado.
        /// </summary>
        [Required]
        public string CalledId { get; set; }
    }
}
