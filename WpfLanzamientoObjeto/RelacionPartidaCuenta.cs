//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfLanzamientoObjeto
{
    using System;
    using System.Collections.Generic;
    
    public partial class RelacionPartidaCuenta
    {
        public int IdPartidaCuenta { get; set; }
        public int Posicion { get; set; }
        public int CodigoPartida { get; set; }
        public string CuentaCorreoElectronico { get; set; }
    
        public virtual Partida Partida { get; set; }
        public virtual Cuenta Cuenta { get; set; }
    }
}
