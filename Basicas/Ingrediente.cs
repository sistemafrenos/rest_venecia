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
    
    public partial class Ingrediente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ingrediente()
        {
            this.IngredientesInventarios = new HashSet<IngredientesInventario>();
        }
    
        public string IdIngrediente { get; set; }
        public string Descripcion { get; set; }
        public string UnidadMedida { get; set; }
        public Nullable<double> Costo { get; set; }
        public Nullable<double> Raciones { get; set; }
        public Nullable<double> Existencia { get; set; }
        public string Grupo { get; set; }
        public Nullable<bool> Activo { get; set; }
        public Nullable<bool> Insumo { get; set; }
        public Nullable<bool> LlevaInventario { get; set; }
        public Nullable<double> TasaIva { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IngredientesInventario> IngredientesInventarios { get; set; }
    }
}
