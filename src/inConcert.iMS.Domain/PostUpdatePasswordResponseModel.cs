using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class PostUpdatePasswordResponseModel : StatusResponseModel
   {
      /// <summary>
      /// Contraseña encriptada el comercial
      /// </summary>
      public string Password { get; set; }
   }
}
