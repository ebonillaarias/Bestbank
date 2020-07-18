using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;

namespace inConcert.iMS.Domain.Entities
{
   public class Calls
    {
        /// <summary>
        /// Id de la llamada.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Teléfono llamante.
        /// </summary>
        public string CallerId { get; set; }
        /// <summary>
        /// Teléfono llamado.
        /// </summary>
        public string CalledId { get; set; }
        /// <summary>
        /// Dirección de la llamada.
        /// </summary>
        public CallDirection Direction { get; set; }
        /// <summary>
        /// Fecha y hora UTC de inicio de la llamada.
        /// </summary>
        public DateTimeOffset StartDate { get; set; }
        /// <summary>
        /// Fecha y hora UTC de fin de la llamada.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }
        /// <summary>
        ///  Identificador único del cliente en Siebel al que el comercial asoció la llamada.Puede ir vacío.
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        ///  Nombre del cliente en Siebel al que el comercial asoció la llamada.
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Listado de partes que conforman la llamada.
        /// </summary>
        public ICollection<CallParts> CallParts { get; set; }

         //public LogsGenerals LogsGeneral { get; set; }
    }
}
