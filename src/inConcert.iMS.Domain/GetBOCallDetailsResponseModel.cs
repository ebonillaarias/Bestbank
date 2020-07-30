using inConcert.iMS.Domain.Entities;
using System.Collections.Generic;

namespace inConcert.iMS.Domain
{
    public class GetBOCallDetailsResponseModel : StatusResponseModel
    {
        /// <summary>
        /// Información de una llamada.
        /// </summary>
        public BOCallModel Info { get; set; }

        /// <summary>
        /// Listado con información de las partes de una llamada.
        /// </summary>
        public List<BOCallPartsModel> Parts { get; set; }

        /// <summary>
        /// Listado con información de las grabaciones pausadas.
        /// </summary>
        public List<CallsRecords> Records { get; set; }
    }
}
