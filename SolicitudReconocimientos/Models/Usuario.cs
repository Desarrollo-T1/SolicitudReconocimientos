namespace SolicitudReconocimientos.Models
{
    using Helper;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("Usuario")]
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            UsuarioReconocimiento = new HashSet<UsuarioReconocimiento>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreTipo { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(32)]
        public string Contraseña { get; set; }

        public int Rol_id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string ApellidoP { get; set; }

        [Required]
        [StringLength(100)]
        public string ApellidoM { get; set; }

        public long Unidad_id { get; set; }

        public virtual Rol Rol { get; set; }

        public virtual Unidad Unidad { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioReconocimiento> UsuarioReconocimiento { get; set; }

        public ResponseModel Autenticar()
        {
            var rm = new ResponseModel();

            try
            {
                using (var ctx = new ReconocimientosContext())
                {
                    var usuario = ctx.Usuario.Where(x => x.UserName == this.UserName && x.Contraseña == this.Contraseña).SingleOrDefault();
                    if (usuario != null)
                    {
                        SessionHelper.AddUserToSession(usuario.id.ToString());
                        rm.SetResponse(true);
                    }
                    else
                    {
                        rm.SetResponse(false, "Acceso denegado al sistema");
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return rm;
        }

        public Usuario Obtener(int id)
        {
            var usuario = new Usuario();

            try
            {
                using (var ctx = new ReconocimientosContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;

                    usuario = ctx.Usuario.Include("Rol")
                                         .Include("Rol.Permiso")
                                         .Where(x => x.id == id).SingleOrDefault();
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return usuario;
        }


    }
}
