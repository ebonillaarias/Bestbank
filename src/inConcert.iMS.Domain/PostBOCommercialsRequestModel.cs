using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
   public class PostBOCommercialsRequestModel
   {
      /// <summary>
      /// El ID del comercial.
      /// </summary>
      public int CommercialId { get; set; }
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
      /// <summary>
      /// El peer del comercial.
      /// </summary>
      public int Peer { get; set; }
      /// <summary>
      /// El Id del comercial en SIEBEL.
      /// </summary>
      public string SiebelId { get; set; }
      /// <summary>
      /// El numero de telefono en la PBX del comercial.
      /// </summary>
      public string PBXPhoneNumber { get; set; }
      /// <summary>
      /// El numero de celular del comercial.
      /// </summary>
      public string MobilePhoneNumber { get; set; }
      /// <summary>
      /// Estado del comercial.
      /// </summary>
      public bool Active { get; set; }
      /// <summary>
      /// Comerciales alternativos.
      /// </summary>
      public List<AlternativeCommercialModel> Alternatives { get; set; }
      
      /// <summary>
      /// Link para descargar la app Android.
      /// </summary>
      public string LinkAppAndroid { get; set; }
   }
}
