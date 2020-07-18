using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
   public class PostSigninResponseModel : StatusResponseModel
   {
      /// <summary>
      /// El token contiene email, peer, session_id y user id del usuario.
      /// </summary>
      public string AccessToken { get; set; }

      /// <summary>
      /// Identificador de la conexión
      /// </summary>
      public string SessionId { get; set; }

      /// <summary>
      /// Peer para comunicarse con la PBX
      /// </summary>
      public int Peer { get; set; }

      /// <summary>
      /// Host de PBX
      /// </summary>
      public string PbxHost { get; set; }

      /// <summary>
      /// Contraseña encriptada el comercial
      /// </summary>
      public string Password { get; set; }
   }
}
