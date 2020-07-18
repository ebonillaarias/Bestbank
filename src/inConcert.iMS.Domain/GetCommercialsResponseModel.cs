using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
   public class GetCommercialsResponseModel : StatusResponseModel
   {
      /// <summary>
      /// Listado de comerciales
      /// </summary>
      public List<CommercialModel> Commercials { get; set; }
   }
}
