using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using HK.Clases;
using System.Linq;
using Microsoft.Reporting.WinForms;

namespace HK.Formas
{
    public partial class FrmReportes : Form
    {
        public FrmReportes()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(FrmReportes_FormClosing);
        }
        void FrmReportes_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.reportViewer1.LocalReport.ReleaseSandboxAppDomain();
        }
        public void ListadoPlatos(List<Plato> lista)
        {            
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoPlatos.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Platos", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros",new  Parametro[] { Basicas.parametros() } ));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoPlatosIngredientes()
        {
            using (var db = new VeneciaEntities())
            {
                var recetas = from plato in db.Platos
                        join ingredientes in db.PlatosIngredientes on plato.IdPlato equals ingredientes.IdPlato
                        select new Receta
                        {
                            PlatoGrupo = plato.Grupo,
                            PlatoDescripcion= plato.Descripcion,                            
                        };
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoPlatosIngredientes.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Recetas", recetas.ToList()));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
            }
        }
        public void ListadoUsuarios(List<Usuario> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoUsuarios.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Usuarios", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoMesoneros(List<Mesonero> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoMesoneros.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Mesoneros", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoCajeros(List<Usuario> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoCajeros.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Usuarios", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoProveedor(List<Proveedore> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoProveedor.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Proveedores", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void CierreDeCaja()
        {
            FrmFechaCajero f = new FrmFechaCajero();
            f.Text = "Cierre De Caja x Usuario";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            Usuario cajero = f.cajero;            
            if (cajero == null)
            {
                List <TotalxFormaPago> pagos = Basicas.VentasxLapso(f.Fecha, f.Fecha);
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\CierreDeCaja.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TotalxFormaPago", pagos));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.Fecha, f.Fecha)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Facturas",  Basicas.FacturasDiariasxLapso(f.Fecha, f.Fecha)));
                ReportParameter pUsuario = new
                        ReportParameter("cajero", string.Format("TODOS LOS CAJEROS"));
                        this.reportViewer1.LocalReport.SetParameters(pUsuario);                
                double? EfectivoEnCaja = pagos.FirstOrDefault(x=> x.FormaPago=="EFECTIVO").Bolivares;
                this.reportViewer1.LocalReport.SetParameters(new 
                    ReportParameter("EfectivoEnCaja", (EfectivoEnCaja.GetValueOrDefault(0)).ToString()));
                 //this.reportViewer1.LocalReport.SetParameters(new
                 //   ReportParameter("Detallado", f.DetallePlatos.ToString()));
                 //double coeficiente = 0;
                 //try
                 //{
                 //    coeficiente = listaVentasxPlato.Average(x => x.MontoPlatosVendidos / x.CostoPlatosVendidos);
                 //} catch {}
                 //this.reportViewer1.LocalReport.SetParameters(new
                 //   ReportParameter("Coeficiente", coeficiente.ToString()));
            }
            else
            {
                List<TotalxFormaPago> pagos = Basicas.VentasxLapso(f.Fecha, f.Fecha,cajero);
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\CierreDeCaja.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TotalxFormaPago", pagos));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.Fecha, f.Fecha,cajero)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Facturas",  Basicas.FacturasDiariasxLapso(f.Fecha, f.Fecha,cajero)));
                ReportParameter pUsuario = new
                        ReportParameter("cajero", string.Format("CAJERO:{0}",cajero.Nombre));
                        this.reportViewer1.LocalReport.SetParameters(pUsuario);                
                double? EfectivoEnCaja = pagos.FirstOrDefault(x=> x.FormaPago=="EFECTIVO").Bolivares;
                this.reportViewer1.LocalReport.SetParameters(new 
                    ReportParameter("EfectivoEnCaja", EfectivoEnCaja.GetValueOrDefault(0).ToString()));
                // this.reportViewer1.LocalReport.SetParameters(new
                //    ReportParameter("Detallado", f.DetallePlatos.ToString()));
                //double coeficiente = listaVentasxPlato.Average(x=>x.MontoPlatosVendidos/x.CostoPlatosVendidos);
                // this.reportViewer1.LocalReport.SetParameters(new
                //    ReportParameter("Coeficiente", coeficiente.ToString()));
            }
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void CierreDeCajaRestaurant()
        {
            FrmFechaCajero f = new FrmFechaCajero();
            f.Text = "Cierre De Caja x Usuario";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            Usuario cajero = f.cajero;
            using( var db= new VeneciaEntities())
            {
                List<MesasAbierta> MesasAbiertas = (from p in db.MesasAbiertas
                                                    orderby p.Numero
                                                    select p).ToList();
            if (cajero == null)
            {
                List<TotalxFormaPago> pagos = Basicas.VentasxLapso(f.Fecha, f.Fecha);
                List<Factura> Facturas = FactoryFacturas.getFacturasLapso(f.Fecha, f.Fecha);
                List<Factura> consumoInterno = FactoryFacturas.getConsumoLapso(f.Fecha, f.Fecha);
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\CierreDeCajaRestaurant.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Facturas", Facturas));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("MesasAbiertas", MesasAbiertas));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("ConsumoInterno", consumoInterno));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TotalxFormaPago", pagos));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.Fecha, f.Fecha)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Facturas", Basicas.FacturasDiariasxLapso(f.Fecha, f.Fecha)));
                ReportParameter pUsuario = new
                        ReportParameter("cajero", string.Format("TODOS LOS CAJEROS"));
                this.reportViewer1.LocalReport.SetParameters(pUsuario);
                double? EfectivoEnCaja = pagos.FirstOrDefault(x => x.FormaPago == "EFECTIVO").Bolivares;
                //this.reportViewer1.LocalReport.SetParameters(new
                ////   ReportParameter("Detallado", f.DetallePlatos.ToString()));
                //double coeficiente = 0;
                //try
                //{
                //    coeficiente = listaVentasxPlato.Average(x => x.MontoPlatosVendidos / x.CostoPlatosVendidos);
                //}
                //catch { }
                //this.reportViewer1.LocalReport.SetParameters(new
                //   ReportParameter("Coeficiente", coeficiente.ToString()));
            }
            else
            {
                List<TotalxFormaPago> pagos = Basicas.VentasxLapso(f.Fecha, f.Fecha, cajero);
                List<Factura> Facturas = FactoryFacturas.getFacturasLapso(f.Fecha, f.Fecha,cajero);
                List<Factura> consumoInterno = FactoryFacturas.getConsumoLapso(f.Fecha, f.Fecha,cajero);
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\CierreDeCajaRestaurant.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Facturas", Facturas));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("MesasAbiertas", MesasAbiertas));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("ConsumoInterno", consumoInterno));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TotalxFormaPago", pagos));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.Fecha, f.Fecha, cajero)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Facturas", Basicas.FacturasDiariasxLapso(f.Fecha, f.Fecha, cajero)));
                ReportParameter pUsuario = new
                        ReportParameter("cajero", string.Format("CAJERO:{0}", cajero.Nombre));
                this.reportViewer1.LocalReport.SetParameters(pUsuario);
                double? EfectivoEnCaja = pagos.FirstOrDefault(x => x.FormaPago == "EFECTIVO").Bolivares;
                this.reportViewer1.LocalReport.SetParameters(new
                    ReportParameter("EfectivoEnCaja", EfectivoEnCaja.GetValueOrDefault(0).ToString()));
                //this.reportViewer1.LocalReport.SetParameters(new
                //   ReportParameter("Detallado", f.DetallePlatos.ToString()));
                //double coeficiente = listaVentasxPlato.Average(x => x.MontoPlatosVendidos / x.CostoPlatosVendidos);
                //this.reportViewer1.LocalReport.SetParameters(new
                //   ReportParameter("Coeficiente", coeficiente.ToString()));
            }
               this.reportViewer1.RefreshReport();
            }
            this.ShowDialog();
        }
        public void CierreDeCajaHoras()
        {
            FrmLapsoHoras f = new FrmLapsoHoras();
            f.Text = "Corte De Caja x Horas";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\CierreDeCajaHoras.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TotalxFormaPago", Basicas.VentasxHoras(f.Desde, f.Hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.Desde, f.Hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            ReportParameter desde = new
                    ReportParameter("desde", f.Desde.ToString("t"));
            ReportParameter hasta = new
                    ReportParameter("hasta", value: f.Hasta.ToString("t"));
            ReportParameter fecha = new
                    ReportParameter("fecha", DateTime.Today.ToString("d"));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { desde, hasta, fecha });
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void VentasxLapso()
        {
            FrmLapso f = new FrmLapso();
            f.Text = "Ventas x Lapso";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\VentasxLapso.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.Desde, f.Hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ComprasxLapso()
        {
            FrmLapso f = new FrmLapso();
            f.Text = "Compras x Lapso";
            f.Detallado = true;
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            List<IngredientesConsumo> lista = Basicas.ComprasxLapso(f.Desde, f.Hasta);
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ComprasxLapso.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Ingredientes", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Lapso", string.Format("Desde:{0} Hasta:{1}", f.Desde.ToShortDateString(), f.Hasta.ToShortDateString())));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Detallado", f.Detallado.ToString()));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ConsumoxLapso()
        {
            FrmLapso f = new FrmLapso();
            f.Text = "Ventas x Lapso";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            List<IngredientesConsumo> lista = Basicas.ConsumoPlatosxLapso(f.Desde, f.Hasta);
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ConsumoxLapso.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Consumo", Basicas.ConsumoPlatosxLapso(f.Desde, f.Hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Lapso", string.Format("Desde:{0} Hasta:{1}", f.Desde.ToShortDateString(), f.Hasta.ToShortDateString())));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ConsumoxFecha()
        {
            FrmFechaCajero f = new FrmFechaCajero();
            f.Text = "Consumo x Fecha y Cajero";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            Usuario cajero = f.cajero;
            if (cajero == null)
            {
                List<IngredientesConsumo> lista = Basicas.ConsumoPorFecha(f.Fecha);
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ConsumoxLapso.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Consumo", Basicas.ConsumoPorFecha(f.Fecha)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Lapso", string.Format("Fecha:{0}", f.Fecha.ToShortDateString())));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
            else
            {
                List<IngredientesConsumo> lista = Basicas.ConsumoPorFechaCajero(f.Fecha, f.cajero);
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ConsumoxLapso.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Consumo", lista));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Lapso", string.Format("Cajero:{0} Fecha:{1}", f.cajero.Nombre, f.Fecha.ToShortDateString())));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
        }
        public void VentasxProducto()
        {
            FrmLapso f = new FrmLapso();
            f.Text = "Ventas x Producto";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\VentasxPlatos.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoIngredientes(List<Ingrediente> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoIngredientes.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Ingredientes", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoInventarios()
        {
            List<Ingrediente> lista = FactoryIngredientes.getItemsConInventario();
            lista = (from x in lista 
                    orderby x.Grupo,x.Descripcion 
                    select x).ToList();
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoInventarios.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Ingredientes",lista ));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("TotalInventarios", lista.Sum(x=> x.CostoTotal ).Value.ToString("n2")));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void IngredienteInventarios(List<IngredientesInventario> lista)
        {
            DateTime? desde = lista.Min(x => x.FechaInicio);
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\IngredientesInventarios.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("IngredientesInventarios", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", string.Format("desde {0} hasta {1}", desde.Value.ToShortDateString(), DateTime.Today.ToShortDateString())));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoMesas(List<Mesa> Lista)
        {
            if (Lista.Count < 1)
                return;
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoMesas.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Mesas", Lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ReporteFacturasNaturalesyJuridicas()
        {
            FrmLapso f = new FrmLapso();
            f.Text = "Listado de Facturas Naturales y Juridicas";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            List<Factura> Lista = FactoryFacturas.getNaturalesJuridicas(f.Desde,f.Hasta);
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoFacturasNaturalesJuridicas.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Facturas", Lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        #region LibrosFiscales
        public void LibroDeVentas()
        {
            FrmMesyAño f = new FrmMesyAño();
            f.Text = "Libro de Ventas";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            using (var db = new VeneciaEntities())
            {
                int Mes = f.Mes;
                int Año = f.Año;
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value.Month == Mes && q.Fecha.Value.Year == f.Año) && q.Tipo == "FACTURA"
                               orderby q.Numero
                               select q;
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeVentas.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("LibroVentas", consulta.ToList()));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", string.Format("Mes {0} Año {1}", Mes, Año)));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
        }
        public void LibroDeVentas(List<LibroVenta> Lista, string periodo)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeVentas.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("LibroVentas", Lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", periodo));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void LibroDeCompras()
        {
            FrmMesyAño f = new FrmMesyAño();
            f.Text = "Libro de Compras";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            using (var db = new VeneciaEntities())
            {
                int Mes = f.Mes;
                int Año = f.Año;
                var consulta = from q in db.LibroCompras
                               where (q.Fecha.Value.Month == Mes && q.Fecha.Value.Year == f.Año)
                               orderby q.Fecha
                               select q;
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeCompras.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Compras", consulta.ToList()));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
        }
        public void LibroDeCompras(List<LibroCompra> Lista, string periodo)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeCompras.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Compras", Lista.ToList()));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", periodo));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void LibroDeVentasResumido()
        {
            FrmMesyAño f = new FrmMesyAño();
            f.Text = "Libro de Ventas Resumido";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            using (var db = new VeneciaEntities())
            {
                int Mes = f.Mes;
                int Año = f.Año;
                List<LibroVenta> libro = new List<LibroVenta>();
                var consulta = from q in db.LibroVentas
                               where (q.Mes == Mes && q.Año == f.Año)
                               orderby q.Factura
                               select q;
                if (consulta.FirstOrDefault() == null)
                    return;
                LibroVenta itemActual = null;
                LibroVenta itemInicial = null;
                LibroVenta ultimoItem = null;
                string Inicio = "";
                foreach (LibroVenta item in consulta)
                {
                    if (itemActual == null)
                    {
                        itemActual = item;
                        ultimoItem = item;
                        Inicio = item.Factura;
                    }
                    if (item.Fecha == itemActual.Fecha && item.NumeroZ == itemActual.NumeroZ && (item.CedulaRif.Substring(0, 1) == "V" || item.CedulaRif.Substring(0, 1) == "E"))
                    {
                        itemActual.MontoIva = itemActual.MontoIva + item.MontoIva;
                        itemActual.MontoTotal = itemActual.MontoTotal + item.MontoTotal;
                        itemActual.MontoExento = itemActual.MontoExento + item.MontoExento;
                        itemActual.MontoGravable = itemActual.MontoGravable + item.MontoGravable;
                        itemActual.RazonSocial = "CONTADO";
                        itemActual.CedulaRif = "V000000000";
                        itemActual.Factura = Inicio + " " + item.Factura;
                        ultimoItem = item;
                    }
                    else
                    {
                        if (item.CedulaRif.Substring(0, 1) == "V" || item.CedulaRif.Substring(0, 1) == "E")
                        {
                            libro.Add(itemActual);
                            itemActual = item;
                            itemInicial = item;
                            ultimoItem = item;
                            Inicio = item.Factura;
                        }
                        else
                        {
                            if (item.IdLibroVentas != itemActual.IdLibroVentas)
                            {
                                libro.Add(itemActual);
                            }
                            libro.Add(item);
                            itemActual = null;
                        }
                    }
                }
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeVentasResumido.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("LibroVentas", libro));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", string.Format("Mes {0} Año {1}", Mes, Año)));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
        }
        public void LibroDeInventarios(List<LibroInventario> Lista, string periodo)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeInventarios.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("LibroInventarios", Lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", periodo));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        #endregion
        public void VentasxPorMesonero()
        {
            FrmLapso f = new FrmLapso();
            f.Detallado = false;
            f.Text = "Ventas x Lapso";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\VentasxMesonero.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.Desde, f.Hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
    }
}
