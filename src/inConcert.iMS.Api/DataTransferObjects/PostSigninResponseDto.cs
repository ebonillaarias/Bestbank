using System.Text.Json.Serialization;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class PostSigninResponseDto
   {
      /// <summary>
      /// El token contiene email, peer, session_id y user id del usuario.
      /// </summary>
      [JsonPropertyName("access_token")]
      public string AccessToken { get; set; }
      
      /// <summary>
      /// Identificador de la conexión
      /// </summary>
      [JsonPropertyName("session_id")]
      public string SessionId { get; set; }
      
      /// <summary>
      /// Peer para comunicarse con la PBX
      /// </summary>
      [JsonPropertyName("peer")]
      public int Peer { get; set; }

      /// <summary>
      /// Host de PBX
      /// </summary>
      [JsonPropertyName("pbx_host")]
      public string PbxHost { get; set; }

      /// <summary>
      /// Contraseña encriptada el comercial
      /// </summary>
      [JsonPropertyName("encrypted_password")]
      public string Password { get; set; }
   }
}
