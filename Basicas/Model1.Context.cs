﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VeneciaEntities : DbContext
    {
        public VeneciaEntities()
            : base("name=VeneciaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Compra> Compras { get; set; }
        public virtual DbSet<ComprasIngrediente> ComprasIngredientes { get; set; }
        public virtual DbSet<Contadore> Contadores { get; set; }
        public virtual DbSet<Factura> Facturas { get; set; }
        public virtual DbSet<FacturasPlato> FacturasPlatos { get; set; }
        public virtual DbSet<Impuesto> Impuestos { get; set; }
        public virtual DbSet<Ingrediente> Ingredientes { get; set; }
        public virtual DbSet<IngredientesInventario> IngredientesInventarios { get; set; }
        public virtual DbSet<IngredientesInventariosHistorial> IngredientesInventariosHistorials { get; set; }
        public virtual DbSet<LibroCompra> LibroCompras { get; set; }
        public virtual DbSet<LibroInventario> LibroInventarios { get; set; }
        public virtual DbSet<LibroVenta> LibroVentas { get; set; }
        public virtual DbSet<Mesa> Mesas { get; set; }
        public virtual DbSet<MesasAbierta> MesasAbiertas { get; set; }
        public virtual DbSet<MesasAbiertasPlato> MesasAbiertasPlatos { get; set; }
        public virtual DbSet<MesasAbiertasPlatosAnulado> MesasAbiertasPlatosAnulados { get; set; }
        public virtual DbSet<Mesonero> Mesoneros { get; set; }
        public virtual DbSet<Parametro> Parametros { get; set; }
        public virtual DbSet<Plato> Platos { get; set; }
        public virtual DbSet<PlatosCombo> PlatosCombos { get; set; }
        public virtual DbSet<PlatosComentario> PlatosComentarios { get; set; }
        public virtual DbSet<PlatosContorno> PlatosContornos { get; set; }
        public virtual DbSet<PlatosIngrediente> PlatosIngredientes { get; set; }
        public virtual DbSet<Proveedore> Proveedores { get; set; }
        public virtual DbSet<Retencione> Retenciones { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Vale> Vales { get; set; }
    }
}
