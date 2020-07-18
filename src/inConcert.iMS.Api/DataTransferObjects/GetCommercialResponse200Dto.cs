using inConcert.iMS.Enums;
using System.Collections.Generic;

namespace inConcert.iMS.Api.DataTransferObjects
{
    public class GetCommercialResponse200Dto
    {
        /// <summary>
        /// Listado de comerciales.
        /// </summary>
        public List<CommercialDto> CommercialList { get; set; }

    }
}
