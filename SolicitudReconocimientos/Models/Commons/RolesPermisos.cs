using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model.Commons
{
    public enum RolesPermisos
    {
        #region Alumnos
        vista_usuario = 1,
        vista_administrador = 2,
        vista_supeadministrador = 3,
        Vista_Foliador=4
        #endregion
    }
}