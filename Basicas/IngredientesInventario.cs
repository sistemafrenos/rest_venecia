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
    
    public partial class IngredientesInventario
    {
        public string IdIngredienteInventario { get; set; }
        public string IdIngrediente { get; set; }
        public string Grupo { get; set; }
        public string Ingrediente { get; set; }
        public Nullable<System.DateTime> FechaInicio { get; set; }
        public Nullable<System.DateTime> FechaFinal { get; set; }
        public Nullable<double> Inicio { get; set; }
        public Nullable<double> Entradas { get; set; }
        public Nullable<double> Salidas { get; set; }
        public Nullable<double> Final { get; set; }
        public Nullable<double> InventarioFisico { get; set; }
        public Nullable<double> Ajuste { get; set; }
    
        public virtual Ingrediente Ingrediente1 { get; set; }
    }
}