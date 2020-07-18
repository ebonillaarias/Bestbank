using inConcert.iMS.Enums;
using System;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class PutCallsRequestDto
    {
        /// <summary>
        /// Id. del cliente.
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// Fecha de inicio en formato UTC.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Fecha de fin en formato UTC.
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Rejected, NoAnswer, Completed
        /// </summary>
        public string CallResult { get; set; }
        /// <summary>
        /// Motivo del rechazo.
        /// </summary>
        public string RejectionReason { get; set; }
    }
}
