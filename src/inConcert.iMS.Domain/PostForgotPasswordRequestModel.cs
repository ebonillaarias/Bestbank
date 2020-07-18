using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class PostForgotPasswordRequestModel
   {
      /// <summary>
      /// Usuario a reenviar la contraseña
      /// </summary>
      public string User { get; set; }
      /// <summary>
      /// Nueva contraseña
      /// </summary>
      public string Password { get; set; }
   }
}
