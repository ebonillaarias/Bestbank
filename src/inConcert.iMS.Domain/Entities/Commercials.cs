using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.Domain.Entities
{
    public class Commercials
    {
        /// <summary>
        /// Id del comercial.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dirección de correo electrónico del comercial. Único.
        /// </summary>

        public string Email { get; set; }
        /// <summary>
        /// Contraseña asociada al comercial.
        /// </summary>

        public string Password { get; set; }
        /// <summary>
        /// Nombre y apellido del comercial.
        /// </summary>

        public string Name { get; set; }
        /// <summary>
        /// Peer asociado al comercial.
        /// </summary>

        public int Peer { get; set; }
        /// <summary>
        /// Identificador único que este comercial tiene en Siebel.
        /// </summary>

        public string SiebelId { get; set; }
        /// <summary>
        /// Número de teléfono fijo asociado al comercial.
        /// </summary>

        public string PBXPhoneNumber { get; set; }
        /// <summary>
        /// Número de teléfono móvil asociado al comercial.
        /// </summary>

        public string MobilePhoneNumber { get; set; }
        /// <summary>
        ///  Indicador de si el comercial está o no activo para utiliza el Sistema.
        /// </summary>

        public bool Active { get; set; }
        /// <summary>
        ///  Indicador de si se requiere o no el cambio de contraseña del comercial en el siguiente inicio de sesión.
        /// </summary>
        public bool PasswordChangeRequired { get; set; }
        /// <summary>
        /// Listado de comerciales alternativos.
        /// </summary>
        public ICollection<AlternativeCommercials> AlternativeCommercials { get; set; }
        /// <summary>
        /// Comercial alternativo (referencia).
        /// </summary>
        public AlternativeCommercials AlternativeCommercial { get; set; }
        /// <summary>
        /// Sesión del comercial.
        /// </summary>
        public Sessions Session { get; set; }
        //public LogsGenerals LogsGeneral { get; set; }
    }
}
