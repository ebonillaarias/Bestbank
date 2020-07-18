using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class CustomersPhonesModel
   {
      /// <summary>
      /// Id del CustomerPhone.
      /// </summary>
      public int Id { get; set; }
      /// <summary>
      /// Id del cliente.
      /// </summary>
      public string CustomerId { get; set; }
      /// <summary>
      /// Numero de telefono
      /// </summary>
      public string PhoneNumber { get; set; }
      /// <summary>
      /// Tipo: R= residencial, Mobile= Celular
      /// </summary>
      public string PhoneType { get; set; }
   }
}
