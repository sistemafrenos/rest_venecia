using System;
using System.Collections.Generic;
using System.Linq;

namespace HK.Clases
{

    public class FactoryFacturas
    {
        public static Factura Item(string id)
        {
            using (VeneciaEntities db= new VeneciaEntities())
            {
                var item = (from x in db.Facturas
                            where (x.IdFactura == id)
                            select x).FirstOrDefault();
                return item;
            }
        }
        public static Factura Item(VeneciaEntities db, string id)
        {
            var item = (from x in db.Facturas
                        where (x.IdFactura == id)
                        select x).FirstOrDefault();
            return item;
        }
        public static List<Factura> getFacturasPendientes(VeneciaEntities db, string texto)
        {
            var mFacturas = (from x in db.Facturas
                           orderby x.IdFactura
                           where (x.Tipo=="PENDIENTE")
                           select x).ToList();
            return mFacturas;
        }
        public static List<Factura> getFacturasLapso( DateTime Desde, DateTime Hasta)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
                var mFacturas = (from x in db.Facturas
                                 orderby x.Numero
                                 where (x.Tipo == "FACTURA")
                                 where x.Fecha >= Desde && x.Fecha<= Hasta
                                 select x).ToList();
                return mFacturas.ToList();
            }
        }
        public static List<Factura> getConsumoLapso(DateTime Desde, DateTime Hasta)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
                var mFacturas = (from x in db.Facturas
                                 orderby x.Numero
                                 where (x.Tipo == "CONSUMO")
                                 where x.Fecha >= Desde && x.Fecha <= Hasta
                                 select x).ToList();
                return mFacturas;
            }
        }
        public static List<Factura> getConsumoLapso(DateTime Desde, DateTime Hasta, Usuario cajero)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
                var mFacturas = (from x in db.Facturas
                                 where x.IdCajero == cajero.IdUsuario && x.Fecha >= Desde && x.Fecha<= Hasta
                                 where (x.Tipo == "CONSUMO")
                                 orderby x.Numero
                                 select x).ToList();
                return mFacturas;
            }
        }
        public static List<Factura> getFacturasLapso(DateTime Desde, DateTime Hasta, Usuario cajero)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
                var mFacturas = (from x in db.Facturas
                                 where x.IdCajero == cajero.IdUsuario && x.Fecha >= Desde && x.Fecha <= Hasta
                                 where (x.Tipo == "CONSUMO")
                                 orderby x.Numero
                                 select x).ToList();
                return mFacturas;
            }
        }
        public static List<Factura> getNaturalesJuridicas(DateTime inicio,DateTime final)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
            var mFacturas = (from x in db.Facturas
                             orderby  x.Numero 
                             where (x.Fecha>=inicio && x.Fecha<=final )
                             select x).ToList();
            return mFacturas;
           }
        }
        public static void DescontarInventario(Factura factura)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
                factura = (from x in db.Facturas
                           where x.IdFactura == factura.IdFactura
                           select x).FirstOrDefault();
                foreach (FacturasPlato plato in factura.FacturasPlatos)
                {
                    List<PlatosIngrediente> ingredientes = FactoryPlatos.getIngredientes(plato.Idplato);
                    foreach (PlatosIngrediente ingrediente in ingredientes)
                    {
                        IngredientesInventario item = (from x in db.IngredientesInventarios
                                                       where x.IdIngrediente == ingrediente.IdIngrediente
                                                       select x).FirstOrDefault();
                        if (item == null)
                        {
                            string Grupo = (from x in db.Ingredientes
                                            where x.IdIngrediente == ingrediente.IdIngrediente
                                            select x.Grupo).FirstOrDefault();
                            item = new IngredientesInventario();
                            item.IdIngrediente = ingrediente.IdIngrediente;
                            item.Ingrediente = ingrediente.Ingrediente;
                            item.FechaInicio = factura.Fecha;
                            item.Ajuste = 0;
                            item.Entradas = 0;
                            item.Final = 0;
                            item.Grupo = Grupo;
                            item.Inicio = 0;
                            item.Salidas = 0;
                        }
                        item.Salidas = item.Salidas + (ingrediente.Cantidad * plato.Cantidad);
                        item.Final = item.Inicio + item.Entradas - item.Salidas;
                        item.InventarioFisico = item.Final;
                        item.Ajuste = 0;
                        if (item.IdIngredienteInventario == null)
                        {
                            item.IdIngredienteInventario = FactoryContadores.GetMax("IdIngredienteInventario");
                            db.IngredientesInventarios.Add(item);
                        }
                    }
                }
                factura.Inventarios = true;
                db.SaveChanges();
            }
        }
        public static void DevolverInventario(Factura factura)
        {
            foreach (FacturasPlato plato in factura.FacturasPlatos)
            {
                List<PlatosIngrediente> ingredientes = FactoryPlatos.getIngredientes(plato.Idplato);
                foreach (PlatosIngrediente ingrediente in ingredientes)
                {
                    using (VeneciaEntities db = new VeneciaEntities())
                    {
                        IngredientesInventario item = (from x in db.IngredientesInventarios
                                                       where x.IdIngrediente == ingrediente.IdIngrediente
                                                       select x).FirstOrDefault();
                        if (item == null)
                        {
                            string Grupo = (from x in db.Ingredientes
                                            where x.IdIngrediente == ingrediente.IdIngrediente
                                            select x.Grupo).FirstOrDefault();
                            item = new IngredientesInventario();
                            item.IdIngrediente = ingrediente.IdIngrediente;
                            item.Ingrediente = ingrediente.Ingrediente;
                            item.FechaInicio = factura.Fecha;
                            item.Ajuste = 0;
                            item.Entradas = 0;
                            item.Final = 0;
                            item.Grupo = Grupo;
                            item.Inicio = 0;
                            item.Salidas = 0;
                        }
                        item.Salidas = item.Salidas - (ingrediente.Cantidad * plato.Cantidad);
                        item.Final = item.Inicio + item.Entradas - item.Salidas;
                        item.InventarioFisico = item.Final;
                        item.Ajuste = 0;
                        if (item.IdIngredienteInventario == null)
                        {
                            item.IdIngredienteInventario = FactoryContadores.GetMax("IdIngredienteInventario");
                            db.IngredientesInventarios.Add(item);
                        }
                        db.SaveChanges();
                    }
                }
            }
        }
        public static void PasarFacturasLibro()
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
                var x = from p in db.Facturas
                        where p.Tipo=="FACTURA" && p.LibroVentas != true
                        select p;
                foreach (var item in x)
                {
                    if (!FactoryLibroVentas.Existe(item))
                    {
                        FactoryLibroVentas.EscribirItemFactura(item);
                        item.LibroVentas = true;
                    }
                }
                db.SaveChanges();
            }
        }
        public static void PasarFacturasLibro(int mes, int año)
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
                var id = int.Parse(FactoryContadores.GetMax("idLibroVentas"));
                db.Database.ExecuteSqlCommand("Delete LibroVentas Where año={0} and mes={1}",año, mes);
                var Facturas =
                    from items in db.Facturas
                    where items.Fecha.Value.Month == mes && items.Fecha.Value.Year == año && items.NumeroZ.Length>0
                    select items;
                foreach (var factura in Facturas)
                {
                    LibroVenta item = new LibroVenta();
                    item.Año = factura.Fecha.Value.Year;
                    item.Mes = factura.Fecha.Value.Month;
                    item.CedulaRif = factura.CedulaRif;
                    item.Factura = factura.Numero;
                    item.Fecha = factura.Fecha;
                    item.IdLibroVentas = id.ToString("000000");
                    item.MontoExento = factura.MontoExento;
                    item.MontoGravable = factura.MontoGravable;
                    item.MontoIva = factura.MontoIva;
                    item.MontoTotal = factura.MontoTotal;
                    item.NumeroZ = factura.NumeroZ;
                    item.RazonSocial = factura.RazonSocial;
                    item.RegistroMaquinaFiscal = factura.MaquinaFiscal;
                    item.TasaIva = factura.getTasaIva();
                    item.TipoOperacion = "01";
                    id++;
                    db.LibroVentas.Add(item);
                }
                db.SaveChanges();
                FactoryContadores.SetMax("idlibroVentas", id);
            }
        }
    }
}
