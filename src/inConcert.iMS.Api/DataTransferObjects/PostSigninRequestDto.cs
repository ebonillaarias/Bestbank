using inConcert.iMS.Enums;
using System;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class PostSigninRequestDto
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
