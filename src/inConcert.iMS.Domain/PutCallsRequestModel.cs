using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
    public class PutCallsRequestModel
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
        public CallResult CallResult { get; set; }
        /// <summary>
        /// Motivo del rechazo.
        /// </summary>
        public string RejectionReason { get; set; }
    }
}
