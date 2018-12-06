//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HK
{
    using System;
    using System.Collections.Generic;
    
    public partial class MesasAbierta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MesasAbierta()
        {
            this.MesasAbiertasPlatos = new HashSet<MesasAbiertasPlato>();
        }
    
        public string IdMesaAbierta { get; set; }
        public string IdMesa { get; set; }
        public string IdMesonero { get; set; }
        public string Mesa { get; set; }
        public string Mesonero { get; set; }
        public Nullable<int> Personas { get; set; }
        public Nullable<System.DateTime> Apertura { get; set; }
        public Nullable<double> MontoGravable { get; set; }
        public Nullable<double> MontoExento { get; set; }
        public Nullable<double> MontoIva { get; set; }
        public Nullable<double> MontoTotal { get; set; }
        public Nullable<double> MontoServicio { get; set; }
        public string Estatus { get; set; }
        public string Numero { get; set; }
        public Nullable<bool> Impresa { get; set; }
        public string CedulaRif { get; set; }
        public string RazonSocial { get; set; }
        public string Categoria { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MesasAbiertasPlato> MesasAbiertasPlatos { get; set; }
    }
}