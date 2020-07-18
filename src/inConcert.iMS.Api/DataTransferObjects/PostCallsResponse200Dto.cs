using System.Collections.Generic;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class PostCallsResponse200Dto
    {
        /// <summary>
        /// Estado del mensage.
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Testo del mensaje.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Listado de clientes.
        /// </summary>
        public List<CustomerDto> CustomerList { get; set; }
    }
}
