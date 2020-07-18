using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class ForgotPasswordResponseModel : StatusResponseModel
   {
      /// <summary>
      /// El nombre del comercial.
      /// </summary>
      public string CommercialName { get; set; }
      /// <summary>
      /// El email del comercial.
      /// </summary>
      public string CommercialEmail { get; set; }
      /// <summary>
      /// La contrasena del comercial.
      /// </summary>
      public string CommercialPassword { get; set; }
   }
}
