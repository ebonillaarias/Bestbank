using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
    public class PutCallsResponseModel
    {
        /// <summary>
        /// Estado del mensaje.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Texto del mensaje.
        /// </summary>
        public string Message { get; set; }
    }
}
