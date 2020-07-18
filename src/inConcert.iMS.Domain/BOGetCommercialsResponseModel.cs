using inConcert.iMS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain
{
    public class BOGetCommercialsResponseModel
    {
        /// <summary>
        /// contiene el listado de comerciales
        /// </summary>
        public List<CommercialModel> commercials { get; set; }
        public ResultStatus status { get; set; }
    }
}
