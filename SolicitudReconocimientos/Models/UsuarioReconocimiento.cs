namespace SolicitudReconocimientos.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UsuarioReconocimiento")]
    public partial class UsuarioReconocimiento
    {
        [Key]
        public long Indentificador { get; set; }

        public int UsuarioId { get; set; }

        public long Reconocimiento { get; set; }

        public virtual Reconocimientos Reconocimientos { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
