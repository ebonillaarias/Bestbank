using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace inConcert.iMS.Domain
{
    public class StartInboundCallModel
    {
        /// <summary>
        /// Teléfono llamante. Es el número de teléfono de un cliente.
        /// </summary>
        public string CallerId { get; set; }
        /// <summary>
        /// Teléfono llamado.
        /// </summary>
        public string CalledId { get; set; }
    }
}
