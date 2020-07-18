namespace inConcert.iMS.Domain
{
   public class PostUpdatePasswordRequestModel
   {
      /// <summary>
      /// El ID del comercial.
      /// </summary>
      public int CommercialId { get; set; }
      /// <summary>
      /// Contraseña actual del comercial.
      /// </summary>
      public string Password { get; set; }
      /// <summary>
      /// Nueva contraseña.
      /// </summary>
      public string NewPassword { get; set; }
   }
}
