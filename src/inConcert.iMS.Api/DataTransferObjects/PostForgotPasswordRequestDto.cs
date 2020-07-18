using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inConcert.iMS.Api.DataTransferObjects
{
   public class PostForgotPasswordRequestDto
   {
      /// <summary>
      /// Email para reenviar la nueva contraseña
      /// </summary>
      public string User { get; set; }
   }
}
