using System;
using System.Collections.Generic;
using System.Linq;
using HK.Formas;
using System.Drawing;
using System.IO;
using HK;

namespace HK.Clases
{
    public partial class Basicas
    {
        public static void ComprasxGrupo()
        {
            try
            {
            FrmLapso f = new FrmLapso();
            f.Text = "Resumen Compras x Lapso";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            using (var db = new VeneciaEntities())
            {
                var q = from compra in db.Compras
                        join compraingrediente in db.ComprasIngredientes on compra.IdCompra equals compraingrediente.IdCompra
                        orderby compraingrediente.Grupo
                        where compra.Fecha.Value >= f.Desde && compra.Fecha.Value <= f.Hasta
                        group compraingrediente by compraingrediente.Grupo into compraxGrupo
                        select new 
                        {
                            Grupo = compraxGrupo.Key,
                            Total = compraxGrupo.Sum(x=> x.Total)
                        };
                #region impresion
                StringWriter l = new StringWriter();
                l.WriteLine("COMPRAS X LAPSO");
                l.WriteLine("{0} al {1}",f.Desde.Date.ToShortDateString(),f.Hasta.Date.ToShortDateString());
                l.WriteLine("========================================");
                foreach (var mgrupo in q)
                {
                    l.WriteLine("=>{0} X {1}", mgrupo.Grupo.PadRight(15, '_').Substring(0, 15), mgrupo.Total.Value.ToString("n2").PadLeft(10));
                }
                l.WriteLine("========================================");
                l.WriteLine("TOTAL COMPRAS => {0}", q.Sum(x => x.Total).GetValueOrDefault(0).ToString("n2").PadLeft(10));
                l.WriteLine("\n\n");
                l.WriteLine("IMPRESO:{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                l.WriteLine("\n\n\n.\n");
                #endregion
                FrmVisualizarReportes f2 = new FrmVisualizarReportes();
                f2.texto = l;
                f2.Mostrar();
            }
            }
            catch (Exception ex)
            {
                throw ex;
            }                                                  
        }
        public static void ImprimirFactura(Factura factura)
        {
            Fiscales.FiscalBixolon2010 f = new Fiscales.FiscalBixolon2010();
            f.ImprimeFactura(factura);
        }
        public static void ImprimirFacturaWindows(Factura factura)
        {
            factura.Hora = DateTime.Now;
            factura.Fecha = DateTime.Today;
            factura.Numero = FactoryContadores.GetMax("Consumo");
            factura.Tipo = "FACTURA";
            factura.Anulado = false;
        }
        public static void ImprimirCorteCuenta(MesasAbierta mesaAbierta)
        {
            switch(Basicas.parametros().ImprimirCorteCuenta)
            {
               case "FISCAL":
                    ImprimirCorteCuentaFiscal(mesaAbierta);
                    break;
                case "WINDOWS":
                    ImprimirCorteCuentaWindows(mesaAbierta);
                    break;
                default:
                    ImprimirCorteCuentaWindows(mesaAbierta);
                    break;
            }
        }
        static void ImprimirCorteCuentaFiscal(MesasAbierta mesaAbierta)
        {
            Fiscales.FiscalBixolon2010 f = new Fiscales.FiscalBixolon2010();
            f.ImprimeCorte(mesaAbierta);
        }
        public static void ImprimirAnulacion(MesasAbiertasPlatosAnulado item)
        {
            int Lineas = 0;
                try
                {
                    LPrintWriter l = new LPrintWriter();
                    l.Font = new Font("FontA12", (float)18.0);
                    l.WriteLine("ANULACION");
                    l.Font = new Font("FontA11", (float)9.0);
                    l.WriteLine("========================================");
                    l.WriteLine(string.Format("TICKET:{0}", item.Numero));
                    l.Font = new Font("FontA11", (float)9.0);
                    l.WriteLine(string.Format("FECHA :{0}    HORA:{1}", DateTime.Today.ToShortDateString(), DateTime.Now.ToShortTimeString()));
                    l.WriteLine(string.Format("MESA:{0}", item.Mesa));
                    l.WriteLine(string.Format("MESONERO:{0}", item.Mesonero));
                    l.WriteLine("========================================");
                    l.WriteLine(" {0}) {1} ", item.Cantidad.Value.ToString("N0"), item.Plato);
                    l.WriteLine("========================================");
                    for (Lineas = 0; Lineas < 6; Lineas++)
                    {
                        l.WriteLine(" ");
                    }
                    l.WriteLine(" ");
                    l.WriteLine(".");
                    l.WriteLine(" ");
                    l.WriteLine(".");
                    l.WriteLine(" ");
                    l.WriteLine(".");
                    l.Flush();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
        }
        static void ImprimirCorteCuentaWindowsOLD(MesasAbierta documento)
        {
            try
            {
                int Lineas = 1;
                LPrintWriter l = new LPrintWriter();
                l.WriteLine(Basicas.parametros().Empresa );
                l.WriteLine("");
                l.Font = new Font("FontA11", (float)9.0);
                l.WriteLine("CORTE DE CUENTA");
                l.WriteLine("           NO FISCAL  ");
                l.WriteLine("       ===============");
                l.WriteLine(" ");
                l.WriteLine(String.Format("   FECHA:{0}", DateTime.Now));
                l.WriteLine(String.Format("  NUMERO:{0}", documento.Numero));
                l.WriteLine(String.Format("    MESA:{0}", documento.Mesa));
                l.WriteLine(String.Format("MESONERO:{0}", documento.Mesonero));
                l.WriteLine(" ");
                l.WriteLine("           NO FISCAL  ");
                l.WriteLine("========================================");
                l.WriteLine("CANT  DESCRIPCION          MONTO        ");
                l.WriteLine("========================================");
                using(var db=new VeneciaEntities())
                {
                    var MesaAbiertaPlatos = from p in db.MesasAbiertasPlatos
                                            where p.IdMesaAbierta == documento.IdMesaAbierta
                                            select p;
                    var Acumulado = from p in MesaAbiertaPlatos
                                    group p by new { p.Descripcion } into itemResumido
                                              select new 
                                              {
                                                  Descripcion =itemResumido.Key.Descripcion,
                                                  Cantidad = itemResumido.Sum(x=> x.Cantidad),
                                                  Total = itemResumido.Sum(x=>x.Precio * x.Cantidad)
                                              };

                    foreach (var Item in Acumulado)
                    {
                        l.WriteLine("{0} {1} {2}",Item.Cantidad.Value.ToString("000"),Item.Descripcion.PadRight(24,'_').Substring(0, 24),(Item.Total.Value).ToString("N2").PadLeft(8));
                    }
                }
                l.WriteLine("           NO FISCAL  ");
                l.WriteLine("========================================");
                l.WriteLine(String.Format(" Monto Gravable:".PadLeft(30) + documento.MontoGravable.Value.ToString("N2").PadLeft(8)));
                l.WriteLine(String.Format("   Monto Exento:".PadLeft(30) + documento.MontoExento.Value.ToString("N2").PadLeft(8)));
                l.WriteLine(String.Format("      Monto Iva:".PadLeft(30) + documento.MontoIva.Value.ToString("N2").PadLeft(8)));
                if(documento.MontoServicio.GetValueOrDefault(0)!=0)
                {
                    l.WriteLine(String.Format(" Monto Servicio:".PadLeft(30) + documento.MontoServicio.Value.ToString("N2").PadLeft(8)));
                }
                l.WriteLine(String.Format("Monto TOTAL=>".PadLeft(30) + documento.MontoTotal.Value.ToString("N2").PadLeft(8)));
                if( documento.MontoServicio.GetValueOrDefault(0)==0)
                {
                l.WriteLine(String.Format("* NO COBRAMOS 10 % SERVICIO *"));
                }
                l.WriteLine("========================================");
                l.WriteLine("           NO FISCAL  ");
                l.WriteLine(String.Format("POR EXIGENCIAS DEL SENIAT "));
                l.WriteLine(String.Format("NECESITAMOS LO SIGUIENTE:"));
                l.WriteLine(String.Format("CEDULA/RIF:__________________________"));
                l.WriteLine(String.Format("    NOMBRE:__________________________"));
                l.WriteLine("           NO FISCAL  ");

                for (Lineas = 0; Lineas < 6; Lineas++)
                {
                l.WriteLine(" ");
                }
                l.WriteLine(" "); 
                l.WriteLine(".");
                l.WriteLine(" ");
                l.WriteLine(".");
                l.WriteLine(" ");
                l.WriteLine(".");
                l.Flush();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static void ImprimirCorteCuentaWindows(MesasAbierta documento)
        {
            try
            {
                int Lineas = 1;
                LPrintWriter l = new LPrintWriter();
              //  l.WriteLine(Basicas.parametros().Empresa);
                l.WriteLine("");
                l.Font = new Font("Tahoma", (float)9.0);
                l.WriteLine("PLATOS CONSUMIDOS");
                l.WriteLine(" ");
                l.WriteLine(String.Format("   FECHA:{0}", DateTime.Now));
                l.WriteLine(String.Format("    MESA:{0}", documento.Mesa));
                l.WriteLine(" ");
                l.WriteLine("========================================");
                l.WriteLine("CANT  DESCRIPCION                 ");
                l.WriteLine("========================================");
                using (var db = new VeneciaEntities())
                {
                    var MesaAbiertaPlatos = from p in db.MesasAbiertasPlatos
                                            where p.IdMesaAbierta == documento.IdMesaAbierta
                                            select p;
                    var Acumulado = from p in MesaAbiertaPlatos
                                    group p by new { p.Descripcion } into itemResumido
                                    select new
                                    {
                                        Descripcion = itemResumido.Key.Descripcion,
                                        Cantidad = itemResumido.Sum(x => x.Cantidad),
                                        Total = itemResumido.Sum(x => x.Precio * x.Cantidad)
                                    };

                    foreach (var Item in Acumulado)
                    {
                        l.WriteLine("{0} {1} ", Item.Cantidad.Value.ToString("000"), Item.Descripcion.PadRight(30, ' ').Substring(0, 30));
                    }
                }
                l.WriteLine(" ");
                l.WriteLine(String.Format("POR EXIGENCIAS DEL SENIAT "));
                l.WriteLine(String.Format("NECESITAMOS LO SIGUIENTE:"));
                l.WriteLine(String.Format("PARA EMITIR SU FACTURA:"));
                l.WriteLine(" ");
                l.WriteLine(String.Format("CEDULA/RIF:__________________________"));
                l.WriteLine(" ");
                l.WriteLine(String.Format("    NOMBRE:__________________________"));
                l.WriteLine(" ");
                l.WriteLine(String.Format("INDIQUE SU FORMA DE PAGO:"));
                l.WriteLine(String.Format("  EFECTIVO:__________________________"));
                l.WriteLine(" ");
                l.WriteLine(String.Format("    CHEQUE:__________________________"));
                l.WriteLine(" ");
                l.WriteLine(String.Format("  TARJETAS:__________________________"));
                l.WriteLine("POR REGULACIONES GUBERNAMENTALES");
                l.WriteLine("EMITIDAS POR EL SENIAT EN LA PRO");
                l.WriteLine("VIDENCIA NO.0071 GAC.OFC. 39.795");
                l.WriteLine("DEL  08.11.2011, NO NOS ESTA PER");
                l.WriteLine("MITIDO  EMITIR  NINGUN DOCUMENTO");
                l.WriteLine("CON  MONTOS O PRECIOS, SALVO QUE");
                l.WriteLine("SEA  LA FACTURA FINAL, POR  ESTA");
                l.WriteLine("RAZON ENLISTAMOS SOLAMENTE LOS");
                l.WriteLine("ALIMENTOS Y/O BEBIDAS CONSUMIDOS");
                l.WriteLine(" ");
                l.WriteLine(" *** NO COBRAMOS 10% ***");
                for (Lineas = 0; Lineas < 6; Lineas++)
                {
                    l.WriteLine(" ");
                }
                l.WriteLine(" ");
                l.WriteLine(".");
                l.WriteLine(" ");
                l.WriteLine(".");
                l.WriteLine(" ");
                l.WriteLine(".");
                l.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void ImprimirComanda(MesasAbierta documento, List<MesasAbiertasPlato> items)
        {
            using (var db = new VeneciaEntities())
            {
                var porimprimir = from p in items
                                  where p.IdMesaAbiertaPlato == null && !String.IsNullOrEmpty(p.EnviarComanda)
                                  select p;
                if (porimprimir.Count() == 0)
                    return;
                porimprimir=porimprimir.Where(x=>x.EnviarComanda!="FISCAL").OrderBy(x => x.EnviarComanda);
                if (porimprimir.Count() < 1)
                    return;
                try
                {
                    LPrintWriter l = new LPrintWriter();
                    Font Fuente = new Font("Arial", (float)11.0);
                    l.Font = Fuente;
                    string Grupo = "";
                    foreach (var item in porimprimir)
                    {
                        if(item.EnviarComanda!=Grupo)
                        {
                            l.WriteLine("\n\n\n");
                            l.WriteLine(string.Format("TICKET:{0} COMANDA:{1}", documento.Numero, FactoryContadores.GetMax("Comanda")));
                            l.WriteLine(string.Format("FECHA :{0}    HORA:{1}", DateTime.Today.ToShortDateString(), DateTime.Now.ToShortTimeString()));
                            l.WriteLine(string.Format("MESA:{0}", documento.Mesa));
                            l.WriteLine(string.Format("MESONERO:{0}", documento.Mesonero));
                            l.WriteLine("========================================");
                            l.WriteLine("{0}", item.EnviarComanda);
                            Grupo = item.EnviarComanda;
                        }
                        l.WriteLine(" {0}) {1} ", item.Cantidad.Value.ToString("N0"), item.Descripcion);
                        if (item.Comentarios != null)
                        {
                            foreach (string p in item.Comentarios)
                            {
                                l.WriteLine(p);
                            }
                        }
                        if (item.Contornos != null)
                        {
                            foreach (string p in item.Contornos)
                            {
                                l.WriteLine(p);
                            }
                        }
                    }
                    l.WriteLine("========================================");
                    l.WriteLine("\n\n\n\n\n\n.");
                    l.Flush();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void ImprimirOrden(Factura documento)
        {
            switch(Basicas.parametros().ImprimirOrden)
            {
                case "FISCAL":
                    ImprimirOrdenFiscal(documento);
                    break;
                case "WINDOWS":
                    ImprimirOrdenWindows(documento);
                    break;
                default:
                    break;
            }
        }
        public static void ImprimirOrdenWindows(Factura documento)
        {
            //PrintDocument pd = new PrintDocument();
            //String OldPrinter = pd.PrinterSettings.PrinterName;
            int Lineas = 0;
            //  Basicas.SetDefaultPrinter("RECIBOS");
            try
            {
                LPrintWriter l = new LPrintWriter();
                Font Fuente = new Font("Arial", (float)11.0);
                l.Font = Fuente;                
                l.WriteLine("COMANDA");
                l.WriteLine(" ");
                l.WriteLine("  FECHA:{0}",DateTime.Today.ToShortDateString());
                l.WriteLine("   HORA:{0}", DateTime.Now.ToShortTimeString());
                l.WriteLine("CLIENTE:{0}", documento.RazonSocial);
                l.WriteLine("    {0}:{1}", documento.Tipo, documento.NumeroOrden);
                foreach (var item in documento.FacturasPlatos)
                {
                    if (item.Cantidad.GetValueOrDefault(0) > 1)
                    {
                        l.WriteLine(" X {0}", item.Cantidad.Value.ToString("N0"));
                    }
                    l.WriteLine(string.Format(item.Descripcion));
                    if (item.Comentarios != null)
                    {
                        foreach (string p in item.Comentarios)
                        {
                            l.WriteLine(p);
                        }
                    }
                    if (item.Contornos != null)
                    {
                        foreach (string p in item.Contornos)
                        {
                            l.WriteLine(p);
                        }
                    }
                }
                l.WriteLine(" ");
                for (Lineas = 0; Lineas < 6; Lineas++)
                {
                    l.WriteLine(" ");
                }
                l.WriteLine(" ");
                l.WriteLine(".");
                l.WriteLine(" ");
                l.WriteLine(".");
                l.WriteLine(" ");
                l.WriteLine(".");
                l.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void ImprimirOrdenFiscal(Factura documento)
        {
            Fiscales.FiscalBixolon2010 f = new Fiscales.FiscalBixolon2010();
            f.ImprimeOrden(documento);
        }
        public static List<IngredientesConsumo> ComprasxLapso(DateTime desde, DateTime hasta)
        {
            using (var db = new VeneciaEntities())
            {
                List<IngredientesConsumo> q = (from compra in db.Compras
                                               join compraitem in db.ComprasIngredientes on compra.IdCompra equals compraitem.IdCompra
                                               where compra.Fecha.Value >= desde && compra.Fecha.Value <= hasta 
                                               select new IngredientesConsumo
                                               {
                                                   IdIngrediente = compraitem.IdIngrediente,
                                                   Grupo = compraitem.Grupo,
                                                   Descripcion = compraitem.Ingrediente,
                                                   Cantidad = (double)compraitem.Cantidad,
                                                   Costo=compraitem.Total
                                               }).ToList();
                var ResumenxIngrediente = from p in q
                                          group p by new  { p.Grupo, p.Descripcion }  into ConsumoxProducto
                                          select new IngredientesConsumo
                                          {
                                              Grupo = ConsumoxProducto.Key.Grupo,
                                              Descripcion = ConsumoxProducto.Key.Descripcion,
                                              Cantidad = ConsumoxProducto.Sum(x => x.Cantidad),
                                              Costo = ConsumoxProducto.Sum(x => x.Costo)
                                          };
                //var ordenado = from a in ResumenxIngrediente
                //               orderby a.Descripcion
                //               select a;
                return ResumenxIngrediente.ToList();
            }
        }
        public static List<TotalxFormaPago> VentasxLapso(DateTime desde, DateTime hasta)
        {
            List<TotalxFormaPago> retorno = new List<TotalxFormaPago>();
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false)
                               select q;
                TotalxFormaPago efectivo = new TotalxFormaPago();
                efectivo.FormaPago = "EFECTIVO";
                double cambio = consulta.Sum(x => x.Cambio).GetValueOrDefault(0);
                efectivo.Bolivares = consulta.Sum(x => x.Efectivo).GetValueOrDefault(0) - cambio;
                retorno.Add(efectivo);
                TotalxFormaPago cheque = new TotalxFormaPago();
                cheque.FormaPago = "CHEQUE";
                cheque.Bolivares = consulta.Sum(x => x.Cheque).GetValueOrDefault(0);
                retorno.Add(cheque);
                TotalxFormaPago tarjeta = new TotalxFormaPago();
                tarjeta.FormaPago = "TARJETA";
                tarjeta.Bolivares = consulta.Sum(x => x.Tarjeta).GetValueOrDefault(0);
                retorno.Add(tarjeta);
                TotalxFormaPago cestaTicket = new TotalxFormaPago();
                cestaTicket.FormaPago = "CESTA TICKET";
                cestaTicket.Bolivares = consulta.Sum(x => x.CestaTicket).GetValueOrDefault(0);
                retorno.Add(cestaTicket);
                TotalxFormaPago consumoInterno = new TotalxFormaPago();
                consumoInterno.FormaPago = "CONSUMO INTERNO";
                consumoInterno.Bolivares = consulta.Sum(x => x.ConsumoInterno).GetValueOrDefault(0);
                retorno.Add(consumoInterno);
                return retorno;
            }
        }
        public static List<TotalxFormaPago> VentasxLapso(DateTime desde, DateTime hasta, Usuario cajero)
        {
            List<TotalxFormaPago> retorno = new List<TotalxFormaPago>();
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false && q.IdCajero == cajero.IdUsuario)
                               select q;
                TotalxFormaPago efectivo = new TotalxFormaPago();
                efectivo.FormaPago = "EFECTIVO";
                double cambio = consulta.Sum(x => x.Cambio).GetValueOrDefault(0);
                efectivo.Bolivares = consulta.Sum(x => x.Efectivo).GetValueOrDefault(0) - cambio;
                retorno.Add(efectivo);
                TotalxFormaPago cheque = new TotalxFormaPago();
                cheque.FormaPago = "CHEQUE";
                cheque.Bolivares = consulta.Sum(x => x.Cheque).GetValueOrDefault(0);
                retorno.Add(cheque);
                TotalxFormaPago tarjeta = new TotalxFormaPago();
                tarjeta.FormaPago = "TARJETA";
                tarjeta.Bolivares = consulta.Sum(x => x.Tarjeta).GetValueOrDefault(0);
                retorno.Add(tarjeta);
                TotalxFormaPago cestaTicket = new TotalxFormaPago();
                cestaTicket.FormaPago = "CESTA TICKET";
                cestaTicket.Bolivares = consulta.Sum(x => x.CestaTicket).GetValueOrDefault(0);
                retorno.Add(cestaTicket);
                TotalxFormaPago consumoInterno = new TotalxFormaPago();
                consumoInterno.FormaPago = "CONSUMO INTERNO";
                consumoInterno.Bolivares = consulta.Sum(x => x.ConsumoInterno).GetValueOrDefault(0);
                retorno.Add(consumoInterno);
                return retorno;
            }
        }
        public static List<TotalxFormaPago> VentasxHoras(DateTime desde, DateTime hasta)
        {
            List<TotalxFormaPago> retorno = new List<TotalxFormaPago>();
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value == DateTime.Today.Date && q.Hora.Value >= desde && q.Hora.Value <= hasta && q.Anulado == false)
                               select q;
                TotalxFormaPago efectivo = new TotalxFormaPago();
                double cambio = consulta.Sum(x => x.Cambio).GetValueOrDefault(0);
                efectivo.FormaPago = "EFECTIVO";
                efectivo.Bolivares = consulta.Sum(x => x.Efectivo).GetValueOrDefault(0) - cambio;
                retorno.Add(efectivo);
                TotalxFormaPago cheque = new TotalxFormaPago();
                cheque.FormaPago = "CHEQUE";
                cheque.Bolivares = consulta.Sum(x => x.Cheque).GetValueOrDefault(0);
                retorno.Add(cheque);
                TotalxFormaPago tarjeta = new TotalxFormaPago();
                tarjeta.FormaPago = "TARJETA";
                tarjeta.Bolivares = consulta.Sum(x => x.Tarjeta).GetValueOrDefault(0);
                retorno.Add(tarjeta);
                TotalxFormaPago cestaTicket = new TotalxFormaPago();
                cestaTicket.FormaPago = "CESTA TICKET";
                cestaTicket.Bolivares = consulta.Sum(x => x.CestaTicket).GetValueOrDefault(0);
                retorno.Add(cestaTicket);
                TotalxFormaPago consumoInterno = new TotalxFormaPago();
                consumoInterno.FormaPago = "CONSUMO INTERNO";
                consumoInterno.Bolivares = consulta.Sum(x => x.ConsumoInterno).GetValueOrDefault(0);
                retorno.Add(consumoInterno);
                return retorno;
            }
        }
        public static List<TotalxDia> VentasDiariasxLapso(DateTime desde, DateTime hasta)
        {
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false)
                               group q by q.Fecha into ventaxDia
                               select new TotalxDia
                               {
                                   Fecha = ventaxDia.Key.Value,
                                   Facturas = (int)ventaxDia.Count(),
                                   Promedio = (double)ventaxDia.Average(x => x.MontoTotal),
                                   Bolivares = (double)ventaxDia.Sum(x => x.MontoTotal)
                               };
                return consulta.ToList();
            }
        }
        public static List<TotalxDiaMesonero> VentasDiariasxMesonero(DateTime desde, DateTime hasta)
        {
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false)
                               group q by new { q.Fecha, q.Mesonero }  into ventaxDia
                               select new TotalxDiaMesonero
                               {
                                   Fecha = ventaxDia.Key.Fecha.Value,
                                   Mesonero = ventaxDia.Key.Mesonero,
                                   Facturas = (int)ventaxDia.Count(),
                                   Bolivares = (double)ventaxDia.Sum(x => x.MontoTotal)
                               };
                return consulta.ToList();
            }
        }
        public static List<TotalxDia> VentasDiariasxLapso(DateTime desde, DateTime hasta, Usuario cajero)
        {
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false && q.IdCajero == cajero.IdUsuario)
                               group q by q.Fecha into ventaxDia
                               select new TotalxDia
                               {
                                   Fecha = ventaxDia.Key.Value,
                                   Facturas = (int)ventaxDia.Count(),
                                   Promedio = (double)ventaxDia.Average(x => x.MontoTotal),
                                   Bolivares = (double)ventaxDia.Sum(x => x.MontoTotal)
                               };
                return consulta.ToList();
            }
        }
        public static List<TotalxDia> VentasDiariasxHoras(DateTime desde, DateTime hasta)
        {
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Hora.Value >= desde && q.Hora.Value <= hasta && q.Fecha == DateTime.Today && q.Anulado == false)
                               group q by q.Fecha into ventaxDia
                               select new TotalxDia
                               {
                                   Fecha = ventaxDia.Key.Value,
                                   Facturas = (int)ventaxDia.Count(),
                                   Promedio = (double)ventaxDia.Average(x => x.MontoTotal),
                                   Bolivares = (double)ventaxDia.Sum(x => x.MontoTotal)
                               };
                return consulta.ToList();
            }
        }
        public static List<Factura> LibroDeVentas(DateTime fecha)
        {
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value.Month >= fecha.Month && q.Fecha.Value.Year <= fecha.Year) && q.Tipo == "FACTURA"
                               orderby q.Numero
                               select q;
                List<Factura> x = consulta.ToList();
                return x;
            }
        }
        public static List<IngredientesConsumo> ConsumoPorFechaCajero(DateTime fecha, Usuario cajero)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {

                List<IngredientesConsumo> q = (from factura in db.Facturas
                                               join facturaplato in db.FacturasPlatos on factura.IdFactura equals facturaplato.IdFactura
                                               join productoIngrediente in db.PlatosIngredientes on facturaplato.Idplato equals productoIngrediente.IdPlato
                                               where factura.Fecha.Value == fecha && factura.IdCajero == cajero.IdUsuario && (factura.Tipo == "FACTURA" || factura.Tipo == "CONSUMO")
                                               select new IngredientesConsumo
                                               {
                                                   IdIngrediente = productoIngrediente.IdIngrediente,
                                                   Descripcion = productoIngrediente.Ingrediente,
                                                   Cantidad = (double)facturaplato.Cantidad * (double)productoIngrediente.Cantidad
                                               }).ToList();

                var ResumenxIngrediente = from p in q
                                          group p by p.Descripcion into ConsumoxProducto
                                          select new IngredientesConsumo
                                          {
                                              Descripcion = ConsumoxProducto.Key,
                                              Cantidad = ConsumoxProducto.Sum(x => x.Cantidad)
                                          };
                var ordenado = from a in ResumenxIngrediente
                               orderby a.Descripcion
                               select a;
                return ordenado.ToList();
            }
        }
        public static List<IngredientesConsumo> ConsumoPorFecha(DateTime fecha)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {

                List<IngredientesConsumo> q = (from factura in db.Facturas
                                               join facturaplato in db.FacturasPlatos on factura.IdFactura equals facturaplato.IdFactura
                                               join productoIngrediente in db.PlatosIngredientes on facturaplato.Idplato equals productoIngrediente.IdPlato
                                               where factura.Fecha.Value == fecha && (factura.Tipo == "FACTURA" || factura.Tipo == "CONSUMO")
                                               select new IngredientesConsumo
                                               {
                                                   IdIngrediente = productoIngrediente.IdIngrediente,
                                                   Descripcion = productoIngrediente.Ingrediente,
                                                   Cantidad = (double)facturaplato.Cantidad * (double)productoIngrediente.Cantidad
                                               }).ToList();

                var ResumenxIngrediente = from p in q
                                          group p by p.Descripcion into ConsumoxProducto
                                          select new IngredientesConsumo
                                          {
                                              Descripcion = ConsumoxProducto.Key,
                                              Cantidad = ConsumoxProducto.Sum(x => x.Cantidad)
                                          };
                var ordenado = from a in ResumenxIngrediente
                               orderby a.Descripcion
                               select a;
                return ordenado.ToList();
            }
        }
        public static List<IngredientesConsumo> ConsumoPlatosxLapso(DateTime desde, DateTime hasta)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
                List<IngredientesConsumo> q = (from factura in db.Facturas
                                               join facturaplato in db.FacturasPlatos on factura.IdFactura equals facturaplato.IdFactura
                                               join productoIngrediente in db.PlatosIngredientes on facturaplato.Idplato equals productoIngrediente.IdPlato
                                               where factura.Fecha.Value >= desde && factura.Fecha.Value <= hasta && (factura.Tipo == "FACTURA" || factura.Tipo == "CONSUMO")
                                               select new IngredientesConsumo
                                               {
                                                   IdIngrediente = productoIngrediente.IdIngrediente,
                                                   Descripcion = productoIngrediente.Ingrediente,
                                                   Cantidad = (double)facturaplato.Cantidad * (double)productoIngrediente.Cantidad
                                               }).ToList();

                var ResumenxIngrediente = from p in q
                                          group p by p.Descripcion into ConsumoxProducto
                                          select new IngredientesConsumo
                                          {
                                              Descripcion = ConsumoxProducto.Key,
                                              Cantidad = ConsumoxProducto.Sum(x => x.Cantidad)
                                          };
                var ordenado = from a in ResumenxIngrediente
                               orderby a.Descripcion
                               select a;
                return ordenado.ToList();
            }
        }
        public static List<Factura> FacturasDiariasxLapso(DateTime desde, DateTime hasta)
        {
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas                               
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false) && (q.Tipo == "FACTURA")
                               select q; 
                return consulta.ToList();
            }
        }
        public static List<Factura> ConsumoDiariosxLapso(DateTime desde, DateTime hasta)
        {
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false) && (q.Tipo == "CONSUMO")
                               select q;
                return consulta.ToList();
            }
        }
        public static List<Factura> ConsumoDiariosxLapso(DateTime desde, DateTime hasta,Usuario cajero)
        {
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where ((q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false) && (q.Tipo == "CONSUMO") )&& q.IdCajero == cajero.IdUsuario
                               select q;
                return consulta.ToList();
            }
        }
        public static List<Factura> FacturasDiariasxLapso(DateTime desde, DateTime hasta,Usuario cajero)
        {
            using (var db = new VeneciaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false) && (q.Tipo == "FACTURA" ) && q.IdCajero == cajero.IdUsuario
                               select q;
                return consulta.ToList();
            }
        }
    }
}
