namespace SolicitudReconocimientos.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TipoDocumento")]
    public partial class TipoDocumento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TipoDocumento()
        {
            Reconocimientos = new HashSet<Reconocimientos>();
        }

        [Key]
        public long Identificador { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reconocimientos> Reconocimientos { get; set; }
    }
}
