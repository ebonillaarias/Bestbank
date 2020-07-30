using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
    /// <summary>
    /// BackOffice Call Model.
    /// </summary>
    public class BOCallModel
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
        /// Duración de la llamada formato Horas:Minutos:segundos (pudiendo ser más de 23 horas).
        /// </summary>      
        public string Duration { get; set; }

        /// <summary>
        /// Nombre del cliente al que el comercial asoció la llamada.
        /// Puede ir vacío.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Id del Cliente
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Listado de partes que conforman la llamada.
        /// </summary>
        public ICollection<CallParts> CallParts { get; set; }

        /// <summary>
        /// Nombre del comercial al cual fue tranferida la llamada.
        /// </summary>
        public string Transferred { get; set; }

        /// <summary>
        /// Resultado de la llamada (Rechazada, NoContestada, Completada, Transferida).
        /// </summary>
        public CallResult Result { get; set; }

        /// <summary>
        /// flag para saber si tuvo pausas en la grabación.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// cantidad de pausas.
        /// </summary>
        public int NroPaused { get; set; }
    }
}
