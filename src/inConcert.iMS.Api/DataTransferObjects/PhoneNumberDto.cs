using inConcert.iMS.Enums;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class PhoneNumberDto
    {
        /// <summary>
        /// El número de teléfono del cliente.
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// El tipo de llamada.
        /// </summary>
        public string PhoneType { get; set; }
    }
}
