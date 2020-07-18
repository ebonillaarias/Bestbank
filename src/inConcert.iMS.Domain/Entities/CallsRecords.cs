using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain.Entities
{
    public class CallsRecords
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Indica si se pone en pause/play la grabación
        /// </summary>
        public bool Record { get; set; }

        /// <summary>
        /// Identificador único de llamada.
        /// </summary>
        public int CallId { get; set; }

        /// <summary>
        /// Número secuencial de cada parte de una llamada, comenzando en 1.
        /// </summary>
        public string CallPartNumber { get; set; }
    }
}
