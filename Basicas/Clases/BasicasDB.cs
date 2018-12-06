﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using HK.Clases;

namespace HK
{
    #region Extencion de Tablas
    public partial class LibroCompra
    {
        public void Calcular()
        {
            this.MontoTotal = this.MontoExento.GetValueOrDefault(0) + this.MontoGravable.GetValueOrDefault(0) + this.MontoIva.GetValueOrDefault(0);
        }        
    }

    public partial class Impuesto
    {
        string error = null;
        public string Error { get => error; set => error = value; }
    }
    public partial class Compra
    {
        string error = null;
        public string Error
        {
            set { error = value; }
            get 
            {
                error = null;
                if (this.IncluirLibroCompras.GetValueOrDefault(false) == true)
                {
                    if (string.IsNullOrEmpty(this.Numero))
                        error=error==null?"":error+"\nNumero de factura vacia";
                    if (string.IsNullOrEmpty(this.CedulaRif))
                        error=error== null?"":error + "\nCedula o Rif vacia";
                    else
                    if (this.CedulaRif.Length > 20)
                        error=error== null?"":error + "\nEl campo cedula no puede tener mas de 20 caracteres";
                    if (string.IsNullOrEmpty(this.RazonSocial))
                        error=error== null?"":error + "\nLa razon social no puede estar vacio";
                    else
                    if (this.RazonSocial.Length > 100)
                        error= error== null?"" : error + "\nEl campo razon social no puede tener mas de 100 caracteres";
                    if (this.MontoTotal.GetValueOrDefault(0) == 0)
                        error=error==null?"" : error + "\nEl monto total no puede ser cero";
                    return error;
                }
                return null;
            }
        }
        public void Totalizar()
        {
            this.MontoExento = this.ComprasIngredientes.Where(x => x.TasaIva.GetValueOrDefault(0) == 0).Sum(x => x.Cantidad * x.Costo);
            this.MontoGravable = this.ComprasIngredientes.Where(x => x.TasaIva.GetValueOrDefault(0) > 0).Sum(x => x.Cantidad * x.Costo);
            this.MontoIva =  this.ComprasIngredientes.Where(x => x.TasaIva.GetValueOrDefault(0) > 0).Sum(x => x.Cantidad * x.Costo * x.TasaIva /100);
            this.MontoTotal = this.MontoExento.GetValueOrDefault(0) + this.MontoGravable.GetValueOrDefault(0) + this.MontoIva.GetValueOrDefault(0);
        }
    }
    public partial class Factura
    {
        public void calcularSaldo()
        {
            this.Saldo = this.MontoTotal.GetValueOrDefault(0) - (this.Efectivo.GetValueOrDefault(0) + this.CestaTicket.GetValueOrDefault(0) + this.Tarjeta.GetValueOrDefault(0) + this.Cheque.GetValueOrDefault(0) + this.Cambio.GetValueOrDefault(0) + this.ConsumoInterno.GetValueOrDefault(0) + this.Credito.GetValueOrDefault(0));
            if (this.Saldo < 0)
            {
                this.Cambio = (double)decimal.Round((decimal)Saldo * -1, 2);
                this.Saldo = null;
            }
            else
            {
                this.Cambio = 0;
            }
        }
        public void Totalizar(bool Servicio, double descuento, double? tasaIva)
        {
            this.TasaIva = tasaIva;
            foreach (FacturasPlato item in this.FacturasPlatos.Where(x => x.TasaIva > 0))
            {
                item.TasaIva = tasaIva;
                item.PrecioConIva = Basicas.Round(item.Precio + (item.Precio * tasaIva / 100));
                item.Total = item.PrecioConIva * item.Cantidad;
            }
            this.MontoExento = this.FacturasPlatos.Where(x => x.TasaIva == 0).Sum(x => x.Cantidad * x.Precio);
            this.MontoGravable = this.FacturasPlatos.Where(x => x.TasaIva > 0).Sum(x => x.Cantidad * x.Precio);
            if (Servicio)
            {
                this.MontoServicio = Basicas.Round((MontoGravable.GetValueOrDefault(0) + MontoExento.GetValueOrDefault(0)) * 0.1);
            }

            this.MontoIva = this.FacturasPlatos.Where(x => x.TasaIva > 0).Sum(x => x.Cantidad * x.Precio * tasaIva / 100);
            this.MontoTotal = this.MontoGravable.GetValueOrDefault(0) + this.MontoExento.GetValueOrDefault(0) + this.MontoIva.GetValueOrDefault(0) + this.MontoServicio.GetValueOrDefault(0);
            this.MontoTotal = this.MontoTotal - (this.MontoTotal * descuento / 100);
            this.calcularSaldo();
        }
        public double getTasaIva()
        {
            if (this.TasaIva == null)
            {
                var item = this.FacturasPlatos.FirstOrDefault(x=> x.TasaIva > 0);
                if (item == null)
                {
                    return 12;
                }
                else 
                {
                    return item.TasaIva.GetValueOrDefault();
                }
            }
            return this.TasaIva.GetValueOrDefault();
        }
    }
    public partial class MesasAbierta
    {
        public void Totalizar(bool Servicio,List<MesasAbiertasPlato>platos,double? descuento)
        {
            this.MontoExento = platos.Where(x => x.TasaIva.GetValueOrDefault(0) == 0).Sum(x => x.Cantidad * x.Precio);
            this.MontoGravable = platos.Where(x => x.TasaIva.GetValueOrDefault(0) > 0).Sum(x => x.Cantidad * x.Precio);
            if (Servicio)
            {
                this.MontoServicio = (this.MontoGravable.GetValueOrDefault(0) + this.MontoExento.GetValueOrDefault(0)) * 0.1;
            }
            this.MontoIva = platos.Where(x => x.TasaIva.GetValueOrDefault(0) > 0).Sum(x => x.Cantidad * x.Precio * x.TasaIva / 100);
            this.MontoTotal = this.MontoTotal - (this.MontoTotal * descuento.GetValueOrDefault(0) / 100);
            this.MontoTotal = this.MontoGravable.GetValueOrDefault(0) + this.MontoExento.GetValueOrDefault(0) + this.MontoIva.GetValueOrDefault(0) + this.MontoServicio.GetValueOrDefault(0);
        }
    }
    public partial class Mesonero
    {
        string error = null;
        public string Error
        {
            set { error = value; }
            get
            {
                if (string.IsNullOrEmpty(this.Cedula))
                    return("Error el campo cedula no puede estar vacio");
                if (this.Cedula.Length > 20)
                    return("Error el campo cedula no puede tener mas de 20 caracteres");
                if (string.IsNullOrEmpty(this.Codigo))
                    return("Error el codigo no puede estar vacio");
                if (this.Codigo.Length > 20)
                    return("Error codigo no puede tener mas de 20 caracteres");
                if (string.IsNullOrEmpty(this.Nombre))
                    return("Error el Nombre  no puede estar vacio");
                if (this.Nombre.Length > 100)
                    return "Error el campo cedula no puede tener mas de 100 caracteres";
                return null;
            }
        }
    }
    public partial class Usuario
    {
        string error = null;
        public string Error
        {
            set { error = value; }
            get
            {
                if (string.IsNullOrEmpty(this.Cedula))
                    return ("Error el campo cedula no puede estar vacio");
                if (this.Cedula.Length > 20)
                    return ("Error el campo cedula no puede tener mas de 20 caracteres");
                if (string.IsNullOrEmpty(this.Nombre))
                    return ("Error el Nombre  no puede estar vacio");
                if (this.Nombre.Length > 100)
                    return "Error el campo cedula no puede tener mas de 100 caracteres";
                if (string.IsNullOrEmpty(this.TipoUsuario))
                    return ("Error el Tipo usuario no puede estar vacio");
                return null;
            }
        }
    }
    public partial class Ingrediente
    {
        string error = null;
        public string Error
        {
            set { error = value; }
            get
            {
                if (string.IsNullOrEmpty(this.Grupo))
                    return ("Error el grupo no puede estar vacio");
                if (this.Grupo.Length > 20)
                    return ("Error el campo Grupo no puede tener mas de 20 caracteres");
                if (string.IsNullOrEmpty(this.Descripcion))
                    return ("Error el descripcion no puede estar vacio");
                if (this.Descripcion.Length > 100)
                    return ("Error el campo Descripcion no puede tener mas de 100 caracteres");
                if (string.IsNullOrEmpty(this.UnidadMedida))
                    return ("Error el UnidadMedida  no puede estar vacio");
                if (this.UnidadMedida.Length > 20)
                    return "Error el campo UnidadMedida) no puede tener mas de 100 caracteres";
                return null;
            }
        }
        public Nullable<global::System.Double> CostoTotal
        {
            get
            {
                return this.Costo.GetValueOrDefault(0)  * this.Existencia.GetValueOrDefault(0);
            }
            set
            {
                _CostoTotal = value;
            }
        }
        private Nullable<global::System.Double> _CostoTotal;
    }
    partial class LibroInventario
    {
        public void Calcular()
        {
            this.Final = this.Inicio.GetValueOrDefault(0) + this.Entradas.GetValueOrDefault(0) - this.Salidas.GetValueOrDefault(0);
            this.InventarioFisico = this.Final.GetValueOrDefault() + this.Ajustes.GetValueOrDefault(0);
        }
    }
    partial class LibroVenta
    {
        public void Calcular()
        {
            this.MontoTotal = this.MontoExento.GetValueOrDefault(0) + this.MontoGravable.GetValueOrDefault(0) + this.MontoIva.GetValueOrDefault(0);
        }
    }
    public class Receta
    { 
        string platoDescripcion;

        public string PlatoDescripcion
        {
            get { return platoDescripcion; }
            set { platoDescripcion = value; }
        }
        string platoGrupo;

        public string PlatoGrupo
        {
            get { return platoGrupo; }
            set { platoGrupo = value; }
        }
    }

    #endregion
    #region consultas
    public class VentasxPlato
        {
            double costoPlatosVendidos = 0;
            string idPlato;
            string descripcion;

            public double CostoPlatosVendidos
            {
                get { return costoPlatosVendidos; }
                set { costoPlatosVendidos = value; }
            }
            int platosVendidos = 0;

            public int PlatosVendidos
            {
                get { return platosVendidos; }
                set { platosVendidos = value; }
            }

            double montoPlatosVendidos = 0;

            public double MontoPlatosVendidos
            {
                get { return montoPlatosVendidos; }
                set { montoPlatosVendidos = value; }
            }

        public string IdPlato { get => idPlato; set => idPlato = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
    }
        public class TotalxFormaPago
        {
            string formaPago;

            public string FormaPago
            {
                get { return formaPago; }
                set { formaPago = value; }
            }
            double bolivares = 0;

            public double Bolivares
            {
                get { return bolivares; }
                set { bolivares = value; }
            }
        }
        public class TotalxDia
        {
            int facturas = 0;

            public int Facturas
            {
                get { return facturas; }
                set { facturas = value; }
            }

            DateTime fecha;

            public DateTime Fecha
            {
                get { return fecha; }
                set { fecha = value; }
            }
            double bolivares;

            public double Bolivares
            {
                get { return bolivares; }
                set { bolivares = value; }
            }
            double promedio;

            public double Promedio
            {
                get { return promedio; }
                set { promedio = value; }
            }
        }
        public class TotalxDiaMesonero:TotalxDia
        {
            string mesonero;

            public string Mesonero
            {
                get { return mesonero; }
                set { mesonero = value; }
            }
        }
        public class Valores
        {
            string variable;

            public string Variable
            {
                get { return variable; }
                set { variable = value; }
            }
            double? valor = 0;

            public double? Valor
            {
                get { return valor; }
                set { valor = value; }
            }
        }
        public class IngredientesConsumo : Ingrediente
        {
            double cantidad = 0;

            public double Cantidad
            {
                get { return cantidad; }
                set { cantidad = value; }
            }
        }
        partial class FacturasPlato
        {
            List<string> comentarios;

            public List<string> Comentarios
            {
                get { return comentarios; }
                set { comentarios = value; }
            }
            List<string> contornos;

            public List<string> Contornos
            {
                get { return contornos; }
                set { contornos = value; }
            }
        }
        partial class MesasAbiertasPlato
        {
            List<string> comentarios;

            public List<string> Comentarios
            {
                get { return comentarios; }
                set { comentarios = value; }
            }
            List<string> contornos;

            public List<string> Contornos
            {
                get { return contornos; }
                set { contornos = value; }
            }
        }
        partial class Plato
        {
            double? coeficiente = 0;

            public double? Coeficiente
            {
                get
                {
                    if (Costo.GetValueOrDefault(0) > 0)
                        return Precio / Costo;
                    else
                        return 0;
                }
                set { coeficiente = value; }
            }
        }
        partial class Mesa
        {
            double? monto = 0;

            public double? Monto
            {
                get { return monto; }
                set { monto = value; }
            }
            bool impresa = false;

            public bool Impresa
            {
                get { return impresa; }
                set { impresa = value; }
            }
            DateTime apertura = DateTime.Now;

            public DateTime Apertura
            {
                get { return apertura; }
                set { apertura = value; }
            }
            string numero;

            public string Numero
            {
                get { return numero; }
                set { numero = value; }
            }

            string mesonero;

            public string Mesonero
            {
                get { return mesonero; }
                set { mesonero = value; }
            }
        }
        #endregion
}
