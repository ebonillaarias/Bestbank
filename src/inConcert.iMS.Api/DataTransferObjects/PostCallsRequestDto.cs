using inConcert.iMS.Enums;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class PostCallsRequestDto
    {
        /// <summary>
        /// Interacción.
        /// </summary>
        public string InteractionId { get; set; }
        /// <summary>
        /// Dirección de la llamada.
        /// </summary>
        public string Direction { get; set; }
        /// <summary>
        /// Id del que llama(depende de la dirección de la llamada).
        /// </summary>
        public string CallerId { get; set; }
        /// <summary>
        /// Id al que llaman(depende de la dirección de la llamada).
        /// </summary>
        public string CalledId { get; set; }
        /// <summary>
        /// Id del usuario.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Id del cliente.
        /// </summary>
        public int CustomerId { get; set; }
    }
}
