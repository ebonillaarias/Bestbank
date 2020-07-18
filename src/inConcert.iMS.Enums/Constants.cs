using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Enums
{
   public static class Constants
   {
      #region Commercial Inputs
      /// <summary>
      /// Largo maximo del campo Email
      /// </summary>
      public const int Commercials_Email_MaxLen = 450;

      /// <summary>
      /// Largo maximo del campo Name
      /// </summary>
      public const int Commercials_Name_MaxLen = 255;

      /// <summary>
      /// Largo maximo del campo SiebelID
      /// </summary>
      public const int Commercials_SiebelId_MaxLen = 15;

      /// <summary>
      /// Largo maximo del campo PBX Number
      /// </summary>
      public const int Commercials_PBXNumber_MaxLen = 20;

      /// <summary>
      /// Largo maximo del campo Mobile Phone Number
      /// </summary>
      public const int Commercials_MobilePhoneNumber_MaxLen = 20;
      #endregion


      public const string REJECTED_CALL = "REJECTED_CALL";
   }
}
