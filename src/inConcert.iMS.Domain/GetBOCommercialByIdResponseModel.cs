using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
    public class GetBOCommercialByIdResponseModel
   {
        /// <summary>
        /// contiene el listado de comerciales
        /// </summary>
        public CommercialModel Commercial { get; set; }
        public ResultStatus status { get; set; }
    }
}
