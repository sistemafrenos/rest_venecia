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
    
    public partial class LibroCompra
    {
        public string IdLibroCompras { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<int> Mes { get; set; }
        public Nullable<int> Año { get; set; }
        public string Numero { get; set; }
        public string NumeroControl { get; set; }
        public Nullable<double> MontoExento { get; set; }
        public Nullable<double> MontoGravable { get; set; }
        public Nullable<double> MontoIva { get; set; }
        public Nullable<double> MontoTotal { get; set; }
        public string CedulaRif { get; set; }
        public string RazonSocial { get; set; }
        public Nullable<double> TasaIva { get; set; }
        public string Direccion { get; set; }
        public Nullable<double> IvaRetenido { get; set; }
        public string ComprobanteRetencion { get; set; }
        public Nullable<double> MontoGrabableB { get; set; }
        public Nullable<double> TasaIvaB { get; set; }
    }
}
