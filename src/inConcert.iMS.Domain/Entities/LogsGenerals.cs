using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain.Entities
{
    public class LogsGenerals
    {
        /// <summary>
        /// Id del log
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Tipo de log.
        /// </summary>
        public string TypeLog { get; set; }
        /// <summary>
        /// Descripcion del Log
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Fecha y hora UTC del registro del Log
        /// </summary>
        public DateTimeOffset HourLog { get; set; }
        /// <summary>
        /// Id del comercial que genero el log.
        /// </summary>
        public int? UserId { get; set; }
        //public Commercials Comercial { get; set; }
        /// <summary>
        ///  Id de la llamada la cual nos mostrara el tipo de llama y el numero llamado y llamante
        /// </summary>
        public int? CallsId { get; set; }



        //public Calls Call { get; set; }
        ///<summary>
        ///Id De Supervisor
        ///</summary>
        //public int? SupervisorsId { get; set; }
        //public string SupervisorsEmail { get; set; }
        //public Supervisors Supervisors { get; set; }


    }
}
