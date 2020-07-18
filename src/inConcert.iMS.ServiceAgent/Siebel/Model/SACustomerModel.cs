namespace inConcert.iMS.ServiceAgent.Siebel
{
   public class SACustomerModel
    {
        /// <summary>
        /// El ID del cliente.
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// El nombre del cliente.
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// El tipo cliente.
        /// </summary>
        public string CustomerType { get; set; }
        /// <summary>
        /// Listado de teléfonos.
        /// </summary>
        public SAListOfPhoneNumberModel ListOfPhoneNumber { get; set; }
    }
}
