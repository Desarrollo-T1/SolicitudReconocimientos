namespace SolicitudReconocimientos.Models
{
    using Rec.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Reconocimientos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reconocimientos()
        {
            UsuarioReconocimiento = new HashSet<UsuarioReconocimiento>();
        }

        [Key]
        public long Identificador { get; set; }

        public int IdUsuario { get; set; }

        [Required]
        [StringLength(300)]
        public string Nombre { get; set; }

        public long IdTipoDocumento { get; set; }

        [Required]
        [StringLength(500)]
        public string Motivo { get; set; }

        [Required]
        [StringLength(50)]
        public string FechaEvento { get; set; }

        public long? IdFirma { get; set; }

        public long? IdQuien { get; set; }

        [Required]
        [StringLength(50)]
        public string FechaRegistro { get; set; }

        [StringLength(500)]
        public string Autorizado { get; set; }

        [StringLength(300)]
        public string Archivo { get; set; }

        [StringLength(300)]
        public string AcuseEntrega { get; set; }

        [StringLength(10)]
        public string Folio { get; set; }

        [Required]
        [StringLength(10)]
        public string Foja { get; set; }

        [Required]
        [StringLength(10)]
        public string Libro { get; set; }

        [StringLength(500)]
        public string Comentario { get; set; }

        public virtual Firma Firma { get; set; }

        public virtual Quien Quien { get; set; }

        public virtual TipoDocumento TipoDocumento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioReconocimiento> UsuarioReconocimiento { get; set; }
    }
}
