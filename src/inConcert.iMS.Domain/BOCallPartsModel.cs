using System;

namespace inConcert.iMS.Domain
{
   /// <summary>
   /// BackOffice CallParts Model.
   /// </summary>
   public class BOCallPartsModel
   {
      /// <summary>
      /// Numero de secuencia de la parte de una llamada
      /// </summary>
      public int CallPart { get; set; }

      /// <summary>
      /// Fecha y hora UTC de inicio de la parte.
      /// </summary>
      public DateTimeOffset StartDate { get; set; }

      /// <summary>
      /// Fecha y hora UTC de fin de la parte.
      /// </summary>      
      public DateTimeOffset? EndDate { get; set; }

      /// <summary>
      /// Duracion de la parte.
      /// </summary>
      public string Duration
      {
         get
         {
            if (EndDate.HasValue)
            {
               TimeSpan _duration = EndDate.Value - StartDate;
               return String.Format("{0:00}:{1:00}:{2:00}", _duration.TotalHours, _duration.Minutes, _duration.Seconds);
            }
            else
               return string.Empty;
         }
      }

      /// <summary>
      /// Ruta del archivo de la parte.
      /// </summary>
      public string Path { get; set; }
   }
}
