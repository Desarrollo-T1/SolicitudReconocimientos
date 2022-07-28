namespace Rec.Models
{
    using SolicitudReconocimientos.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Quien")]
    public partial class Quien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quien()
        {
            Reconocimientos = new HashSet<Reconocimientos>();
        }

        [Key]
        public long IdQuien { get; set; }

        [Required]
        [StringLength(10)]
        public string Nombre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reconocimientos> Reconocimientos { get; set; }
    }
}
