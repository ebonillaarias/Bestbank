using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class PostUpdatePasswordResponse200Dto : StatusResponseDto
   {
      /// <summary>
      /// Contraseña encriptada el comercial
      /// </summary>
      public string Password { get; set; }
   }
}
