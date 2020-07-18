using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class PostSigninRequestModel
   {
      /// <summary>
      /// Host para hacer la conexión
      /// </summary>
      public string host { get; set; }
      
      /// <summary>
      /// Usuario a conectar
      /// </summary>
      public string user { get; set; }
      
      /// <summary>
      /// Contraseña del usuario
      /// </summary>
      public string password { get; set; }

      /// <summary>
      /// Token del usuario para Firebase.
      /// </summary>
      public string firebase_token { get; set; }
   }
}
