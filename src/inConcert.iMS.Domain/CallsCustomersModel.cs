using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class CallsCustomersModel
   {
      /// <summary>
      /// Id de la llamada
      /// </summary>
      public int Id { get; set; }
      /// <summary>
      /// Id de la llamada
      /// </summary>
      public int CallId { get; set; }
      /// <summary>
      /// Id del cliente
      /// </summary>
      public string CustomerId { get; set; }
      public List<CustomersPhonesModel> PhoneNumberList { get; set; }
      /// <summary>
      /// Nombre del cliente
      /// </summary>
      public string CustomerName { get; set; }
      /// <summary>
      /// Tipo: R= residencial, B= Business
      /// </summary>
      public CustomerType CustomerType { get; set; }
   }
}
