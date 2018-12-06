using System;
using System.Collections.Generic;
using System.Linq;
using TfhkaNet.IF.VE;

namespace HK.Fiscales
{
    public class FiscalBixolon2010
    {
        #region campos
        public static DateTime Fecha;
        public static double? MontoPorPagar = 0;
        public static double? SubTotalBases = 0;
        public static double? SubTotalIva = 0;
        public string RIF;
        public  int? ultimoZ = null;
        #endregion
        public  bool bRet;
        public  int? iError = 0;
        public  int? iStatus = 0;
        private double? montoZ = 0;
        private string NumeroFactura;
        private string Puerto;
        private string ultimaDevolucion;
        private Parametro parametro;
        Tfhka fiscal;
        public FiscalBixolon2010()
        {
            fiscal = new Tfhka();
            var db = new VeneciaEntities();
            parametro = (from xx in db.Parametros
                         select xx).FirstOrDefault();
            Puerto = parametro.PuertoImpresoraFiscal;
            DetectarImpresora();
        }
        ~FiscalBixolon2010()
        {
            fiscal.CloseFpCtrl();
            fiscal = null;
            Dispose();
        }
        public void CerrarPuerto()
        {
            fiscal.CloseFpCtrl();
        }
        public double? MontoZ 
            {
                get
                {
                    CargarS1(true);
                    return montoZ;
                }
            }
        public string NumeroRegistro
            { set; get;}       
        public void DetectarImpresora()
        {
            try
            {
                bool test1 = fiscal.CheckFPrinter();
                if (fiscal.OpenFpCtrl(Puerto))
                {
                    if (!fiscal.ReadFpStatus())
                    {
                        throw (new Exception(string.Format("Error de conexión, Estatus {0} verifique el puerto por favor...", fiscal.Status_Error)));
                    }
                }
                else
                {
                    var texto = fiscal.ComPort;
                    bool test = fiscal.CheckFPrinter();
                    var x = fiscal.Estado;
                    throw (new Exception(string.Format("Error al abrir el puerto {0}", Puerto)));
                }
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
        }
        private void CargarS1(bool conectar)
        {
            try
            {
                S1PrinterData data =  fiscal.GetS1PrinterData();
                NumeroFactura = data.LastInvoiceNumber.ToString("00000000");
                montoZ = data.TotalDailySales;
                ultimoZ = data.DailyClosureCounter+1;
                NumeroRegistro = data.RegisteredMachineNumber;
                RIF = data.RIF;
                Fecha = data.CurrentPrinterDateTime.Date;
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        private void CargarS2()
        {
            try
            {
                S2PrinterData data = fiscal.GetS2PrinterData();
                SubTotalBases = data.SubTotalBases;
                SubTotalIva = data.SubTotalTax;
                MontoPorPagar = data.AmountPayable;
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        private void ImprimeComentarios(string s)
        {
            try
            {
                if (s.Length <= 40 && s.Length > 0)
                   fiscal.SendCmd("@"+s);
                if (s.Length > 40)
                {
                    fiscal.SendCmd("@" + s.Substring(0, 40));
                    fiscal.SendCmd("@" + s.Substring(40, s.Length - 40));
                }
                if (s.Length > 80)
                {
                    fiscal.SendCmd("@" + s.Substring(80, s.Length - 80));
                }
                if (s.Length > 120)
                {
                    fiscal.SendCmd("@" + s.Substring(120, s.Length - 120));
                }
                if (s.Length > 160)
                {
                    fiscal.SendCmd("@" + s.Substring(160, s.Length - 160));
                }
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        private string MostrarStatus(int status)
        {
            switch (fiscal.Estado)
            {
                case "0":
                    return "Status Desconocido ";
                case "1":
                    return "En Modo Prueba y en Espera ";
                case "2":
                    return "Modo Prueba y EmisiÃ³n de Documentos Fiscales ";
                case "3":
                    return "Modo Prueba y EmisiÃ³n de Documentos No Fiscales ";
                case "4":
                    return "Fiscal y en Espera ";
                case "5":
                    return "Modo Fiscal y EmisiÃ³n de Documentos Fiscales ";
                case "6":
                    return "Fiscal y EmisiÃ³n de Documentos No Fiscales ";
                case "7":
                    return "Modo Fiscal y Cercana Carga Completa De La Memoria Fiscal Y en Espera ";
                case "8":
                    return "En Modo Fiscal y Cercana Carga Completa De La Memoria Fiscal Y en EmisiÃ³n de Documentos Fiscales ";
                case "9":
                    return "En Modo Fiscal y Cercana Carga Completa De La Memoria Fiscal Y en EmisiÃ³n de Documentos No Fiscales ";
                case "10":
                    return "En Modo Fiscal y Carga Completa De La Memoria Fiscal Y en Espera ";
                case "11":
                    return "En Modo Fiscal y Carga Completa De La Memoria Fiscal Y en EmisiÃ³n de Documentos Fiscales ";
                case "12":
                    return "En Modo Fiscal y Carga Completa De La Memoria Fiscal Y en EmisiÃ³n de Documentos No Fiscales ";
                default:
                    return null;
            }
        }
        private string MostrarError(int error)
        {
            switch (fiscal.Status_Error)
            {
                case "0":
                    return "No hay Error  VALIDO ";
                case "1":
                    return "Fin en la Entrega de papel  VALIDO ";
                case "2":
                    return "Error de Ã­ndole MecÃ¡nico en la entrega de Papel   VALIDO ";
                case "3":
                    return "Fin en la Entrega de papel y Error MecÃ¡nico  VALIDO ";
                case "4":
                    return "Comando Invalido / Valor Invalido  INVALIDO ";
                case "5":
                    return "Tasa Invalida  INVALIDO ";
                case "6":
                    return "No hay Asignadas Directivas   INVALIDO ";
                case "7":
                    return "Comando Invalido  INVALIDO ";
                case "8":
                    return "Error Fiscal  INVALIDO ";
                case "9":
                    return "Error de la Memoria Fiscal  INVALIDO ";
                case "10":
                    return "Memoria Fiscal llena  INVALIDO ";
                case "11":
                    return "Buffer Completo   INVALIDO ";
                case "12":
                    return "Error en la ComunicaciÃ³n   INVALIDO ";
                case "13":
                    return "No Hay Respuesta   INVALIDO ";
                case "14":
                    return "Error LRC   INVALIDO ";
                case "145":
                    return "Error Interno  API  INVALIDO ";
                case "153":
                    return "Error  en la Apertura del Archivo  INVALIDO ";
                default:
                    return null;
            }
        }
        public void ImprimeFactura(Factura documento)
        {
            try
                {
                    if (documento == null)
                    {
                        throw new Exception("Documento en blanco no se puede imprimir");
                    }
                    if (documento.MontoTotal <= 0)
                    {
                        throw new Exception("Esta factura no tiene productos");
                    }
                    if (fiscal.Estado != "OK")
                    {
                        throw new Exception(string.Format("Error en impresora Fiscal Error {0}, Estado {1} ", fiscal.Status_Error,fiscal.StatusPort));
                    }
                    double SubTotal = 0;
                    double MontoIva = 0;
                    System.Threading.Thread.Sleep(500);
                    fiscal.SendCmd("i01Cedula/Rif:" + documento.CedulaRif);
                    fiscal.SendCmd("i02Razon Social:");
                    if (documento.RazonSocial.Length <= 37)
                    {
                        fiscal.SendCmd("i03" + documento.RazonSocial);
                    }
                    else
                    {
                        fiscal.SendCmd("i03" + documento.RazonSocial.Substring(0, 36));
                        fiscal.SendCmd("i04" + documento.RazonSocial.Substring(36, (documento.RazonSocial.Length - 36)));
                    }
                    if (documento.CedulaRif == "V000000000")
                    {
                        fiscal.SendCmd("i04 SIN DERECHO A CREDITO FISCAL");
                    }
                    if (documento.Direccion != null)
                    {
                        if (documento.Direccion.Length > 40)
                        {
                            fiscal.SendCmd("i05" + documento.Direccion);
                            fiscal.SendCmd("i06" + documento.Direccion.Substring(40, documento.Direccion.Length - 40));
                        }
                        else
                        {
                            fiscal.SendCmd("i06" + documento.Direccion);
                        }

                    }
                    if (!string.IsNullOrEmpty(documento.NumeroOrden))
                    {
                        fiscal.SendCmd(string.Format("i07       MESA:{0}", documento.NumeroOrden));
                    }
                    var Acumulado = from p in documento.FacturasPlatos.Where(x => x.Cantidad.GetValueOrDefault(0) >= 0.5)
                                    group p by new { p.Descripcion, p.TasaIva, p.Precio, p.PrecioConIva }
                                        into itemResumido
                                        select new
                                        {
                                            Descripcion = itemResumido.Key.Descripcion,
                                            TasaIva = itemResumido.Key.TasaIva,
                                            Cantidad = itemResumido.Sum(x => x.Cantidad),
                                            Precio = itemResumido.Key.Precio,
                                            PrecioConIva = itemResumido.Key.PrecioConIva
                                        };
                    System.Threading.Thread.Sleep(500);
                    foreach (var d in Acumulado.Where(x=>x.Cantidad.GetValueOrDefault()>0 && x.Precio.GetValueOrDefault()>0))
                    {
                        var sCmd = "!"; // Default parametros.TasaIva
                        if (d.TasaIva == 0)
                        {
                            sCmd = " ";
                        }
                        if (d.TasaIva == parametro.TasaIvaB)
                        {
                            sCmd = '"'.ToString();
                        }
                        if (d.TasaIva == parametro.TasaIvaC)
                        {
                            sCmd = '#'.ToString();
                        }
                        SubTotal = ((double)d.Cantidad.GetValueOrDefault(0) * (double)d.PrecioConIva.GetValueOrDefault(0));
                        MontoIva += ((double)d.Cantidad.GetValueOrDefault(0) * ((double)d.PrecioConIva.GetValueOrDefault(0)) - (double)d.Precio.GetValueOrDefault(0));
                        string Precio = "0000000000";
                        if (parametro.TipoIva== "INCLUIDO")
                        {
                            Precio = (d.PrecioConIva.GetValueOrDefault(0) * 100).ToString("0000000000");
                        }
                        else
                        {
                            Precio = (d.Precio.GetValueOrDefault(0) * 100).ToString("0000000000");
                        }
                        string Cantidad = (d.Cantidad.GetValueOrDefault(0) * 1000).ToString("00000000");
                        string Descripcion = d.Descripcion.PadRight(37);
                        if (d.Descripcion.Length <= 37)
                        {
                            bRet = fiscal.SendCmd( sCmd + Precio + Cantidad + d.Descripcion);
                            if (!bRet)
                            {
                                TfhkaNet.IF.PrinterStatus e = fiscal.GetPrinterStatus();                                
                                // OK.ManejarException(new Exception( string.Format("Estatus:{0},Error:{1}",e.PrinterStatusDescription,e.PrinterErrorDescription)));
                            }
                        }
                        else
                        {
                            bRet = fiscal.SendCmd( sCmd + Precio + Cantidad + Descripcion.Substring(0, 36));
                            string Descripcion2 = d.Descripcion.Substring(36, (d.Descripcion.Length - 36));
                            bRet = fiscal.SendCmd( "@" + Descripcion2);
                            if (!bRet)
                            {
                                TfhkaNet.IF.PrinterStatus e = fiscal.GetPrinterStatus();
                                // OK.ManejarException(new Exception( string.Format("Estatus:{0},Error:{1}",e.PrinterStatusDescription,e.PrinterErrorDescription)));
                            }
                        }
                    }
                    //if (documento.DescuentoBs.GetValueOrDefault(0) != 0)
                    //{

                    //    documento.DescuentoBs = documento.MontoTotal.GetValueOrDefault(0) - SubTotal - MontoIva;
                    //    documento.DescuentoBs = documento.DescuentoBs.GetValueOrDefault(0) * -1;
                    //    documento.DescuentoPorcentaje = (documento.DescuentoBs.GetValueOrDefault(0) * 100) / (SubTotal + MontoIva);
                    //    bRet = fiscal.SendCmd( "3");
                    //    string DescuentoPorcentaje = (documento.DescuentoPorcentaje.GetValueOrDefault(0) * 100).ToString("0000");
                    //    bRet = fiscal.SendCmd( "p-" + DescuentoPorcentaje);
                    //}
                    //Pagos
                    // CargarS2();         

                    //if (documento.MontoServicio.GetValueOrDefault(0) > 0)
                    //{
                    //    sCmd =  " ";
                    //    string Precio = (documento.MontoServicio.GetValueOrDefault(0) * 100).ToString("0000000000");
                    //    bRet = fiscal.SendCmd( sCmd + Precio + "00001000" + "SERVICIO 10%");
                    //}
                    System.Threading.Thread.Sleep(500);
                    double TotalPagos = 0;
                    if (documento.Efectivo.GetValueOrDefault(0) != 0)
                    {
                        double x = documento.Efectivo.GetValueOrDefault(0) + documento.Cambio.GetValueOrDefault(0);
                        fiscal.SendCmd( "201" + (x * 100).ToString("000000000000"));
                        TotalPagos += documento.Efectivo.Value;
                    }
                    if (documento.CestaTicket.GetValueOrDefault(0) != 0)
                    {
                        fiscal.SendCmd( "202" + ((double)documento.CestaTicket * 100).ToString("000000000000"));
                        TotalPagos += documento.CestaTicket.Value;
                    }
                    //
                    if (documento.Cheque.GetValueOrDefault(0) != 0)
                    {
                        fiscal.SendCmd( "205" + ((double)documento.Cheque * 100).ToString("000000000000"));
                        TotalPagos += documento.Cheque.Value;
                    }
                    if (documento.Tarjeta.GetValueOrDefault(0) != 0)
                    {
                        fiscal.SendCmd( "209" + ((double)documento.Tarjeta * 100).ToString("000000000000"));
                        TotalPagos += documento.Tarjeta.Value;
                    }
                    if (documento.Credito.GetValueOrDefault(0) != 0)
                    {
                        fiscal.SendCmd( "216" + ((double)documento.Credito * 100).ToString("000000000000"));
                        TotalPagos += documento.Credito.Value;
                    }
                    System.Threading.Thread.Sleep(1000);
                    CargarS2();
                    if (MontoPorPagar > 0)
                    {
                        fiscal.SendCmd( "115" + (MontoPorPagar.Value * 100).ToString("000000000000"));
                    }
                    System.Threading.Thread.Sleep(1000);
                    CargarS1(false);
                    documento.Fecha = Fecha;
                    documento.Hora = DateTime.Now;
                    documento.Numero = NumeroFactura;
                 //   documento.MontoGravable = SubTotalBases;
                 //   documento.MontoIva = MontoIva;
                 //   documento.MontoTotal = SubTotalBases + MontoIva;
                    documento.NumeroZ  = (ultimoZ.Value).ToString("0000");
                    documento.MaquinaFiscal = NumeroRegistro;
                }
                catch (TfhkaNet.IF.PrinterException x)
                {
                    throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

                }
                catch (Exception x)
                {
                    throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
                }
        }
        public void ImprimeNotaCredito(Factura documento)
        {
            if (documento == null)
            {
                throw new Exception("Documento en blanco no se puede imprimir");
            }
            unsafe
            {
                try
                {
                    string sCmd;
                    bool bRet = false;
                    System.Threading.Thread.Sleep(500);
                    fiscal.SendCmd("i01Cedula/Rif:" + documento.CedulaRif);
                    fiscal.SendCmd("i02Razon Social:");
                    fiscal.SendCmd("i03" + documento.RazonSocial);
                    fiscal.SendCmd("i04Direccion:");
                    if (documento.Direccion != null)
                    {
                        if (documento.Direccion.Length > 40)
                        {
                            fiscal.SendCmd("i05" + documento.Direccion);
                            fiscal.SendCmd("i06" + documento.Direccion.Substring(40, documento.Direccion.Length - 40));
                        }
                        else
                        {
                            fiscal.SendCmd("i06" + documento.Direccion);
                        }

                    }
                    fiscal.SendCmd("i07 # FACTURA AFECTADA:" + documento.Numero);
                    // Agrego el servicio en la ultima fila
                    // DS.ImpresionTicket.AddImpresionTicketRow(1, 1, 1, 1, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, 1, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "SERVICIO 10%", 1, documento[0].MontoServicio, 0, "", "", "");
                    double SubTotal = 0;
                    double MontoIva = 0;
                    System.Threading.Thread.Sleep(500);
                    foreach (var d in documento.FacturasPlatos.ToList())
                    {
                        sCmd = "d1"; // Default parametros.TasaIva
                        if (d.TasaIva == 0)
                        {
                            sCmd = "d0";
                        }
                        if (d.TasaIva == parametro.TasaIvaB)
                        {
                            sCmd = "d2";
                        }
                        if (d.TasaIva == parametro.TasaIvaC)
                        {
                            sCmd = "d3";
                        }
                        SubTotal += ((double)d.Cantidad * (double)d.Precio.GetValueOrDefault(0));
                        MontoIva += ((double)d.Cantidad * ((double)d.PrecioConIva.GetValueOrDefault(0)) - (double)d.Precio.GetValueOrDefault(0));
                        string Precio = "0000000000";
                        if (parametro.TipoIva == "INCLUIDO")
                        {
                            Precio = (d.PrecioConIva.GetValueOrDefault(0) * 100).ToString("0000000000");
                        }
                        else
                        {
                            Precio = (d.Precio.GetValueOrDefault(0) * 100).ToString("0000000000");
                        }
                        string Cantidad = ((int)(d.Cantidad * 1000)).ToString("00000000");
                        string Descripcion = d.Descripcion.PadRight(37);
                        if (d.Descripcion.Length <= 37)
                        {
                            bRet = fiscal.SendCmd(sCmd + Precio + Cantidad + d.Descripcion);
                            if (!bRet)
                            {
                                TfhkaNet.IF.PrinterStatus e = fiscal.GetPrinterStatus();
                                // OK.ManejarException(new Exception(string.Format("Estatus:{0},Error:{1}", e.PrinterStatusDescription, e.PrinterErrorDescription)));
                            }
                        }
                        else
                        {
                            bRet = fiscal.SendCmd(sCmd + Precio + Cantidad + Descripcion.Substring(0, 36));
                            if (!bRet)
                            {
                                TfhkaNet.IF.PrinterStatus e = fiscal.GetPrinterStatus();
                                // OK.ManejarException(new Exception(string.Format("Estatus:{0},Error:{1}", e.PrinterStatusDescription, e.PrinterErrorDescription)));
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(500);
                    //if (documento.DescuentoBs != 0)
                    //{

                    //    documento.DescuentoBs = documento.MontoTotal - SubTotal - MontoIva;
                    //    documento.DescuentoBs = documento.DescuentoBs * -1;
                    //    documento.DescuentoPorcentaje = (documento.DescuentoBs * 100) / (SubTotal + MontoIva);
                    //    bRet = fiscal.SendCmd( "3");
                    //    string DescuentoPorcentaje = ((double)documento.DescuentoPorcentaje * 100).ToString("0000");
                    //    bRet = fiscal.SendCmd( "p-" + DescuentoPorcentaje);
                    //}
                    //Pagos
                    double TotalPagos = 0;
                    //if (documento.MontoServicio.GetValueOrDefault(0) > 0)
                    //{
                    //    sCmd = "d0";
                    //    string Precio = (documento.MontoServicio.GetValueOrDefault(0) * 100).ToString("0000000000");
                    //    bRet = fiscal.SendCmd( sCmd + Precio + "00001000" + "SERVICIO 10%");
                    //}
                    // Pago pago = pago; // != null ? pagos : new Pago();
                    if (documento.Efectivo.GetValueOrDefault(0) != 0)
                    {
                        double x = documento.Efectivo.GetValueOrDefault(0) + documento.Cambio.GetValueOrDefault(0);
                        sCmd = "f01" + (x * 100).ToString("000000000000");
                        bRet = fiscal.SendCmd( sCmd);
                        TotalPagos += documento.Efectivo.Value;
                    }
                    if (documento.CestaTicket.GetValueOrDefault(0) != 0)
                    {
                        sCmd = "f02" + ((double)documento.CestaTicket * 100).ToString("000000000000");
                        bRet = fiscal.SendCmd( sCmd);
                        TotalPagos += documento.CestaTicket.Value;
                    }

                    if (documento.Cheque.GetValueOrDefault(0) != 0)
                    {
                        sCmd = "f05" + ((double)documento.Cheque * 100).ToString("000000000000");
                        bRet = fiscal.SendCmd( sCmd);
                        TotalPagos += documento.Cheque.Value;
                    }
                    if (documento.Tarjeta.GetValueOrDefault(0) != 0)
                    {
                        sCmd = "f09" + ((double)documento.Tarjeta * 100).ToString("000000000000");
                        bRet = fiscal.SendCmd( sCmd);
                        TotalPagos += documento.Tarjeta.Value;
                    }
                    if (documento.MontoTotal.GetValueOrDefault(0) > TotalPagos)
                    {
                        sCmd = "f16" + ((double)documento.MontoTotal - (double)TotalPagos * 100).ToString("000000000000");
                        bRet = fiscal.SendCmd( sCmd);

                    }
                    CargarX();
                    CargarS1(false); 
                    System.Threading.Thread.Sleep(500);
                    documento.NumeroZ = ultimoZ.Value.ToString("0000");
                    documento.MaquinaFiscal = NumeroRegistro;
                    documento.Numero = ultimaDevolucion;
                }
                catch (TfhkaNet.IF.PrinterException x)
                {
                    throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

                }
                catch (Exception x)
                {
                    throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
                }
            }
        }
        public void ImprimeFacturaCopia(Factura p)
        {
            bool bRet;

            unsafe
            {
                System.Threading.Thread.Sleep(1000);
                string Numero = p.Numero;
                string n = Numero.Substring(1, 7);
                bRet = fiscal.SendCmd( String.Format("RF{0}{0}", n));
                if (!bRet)
                {
                    throw new Exception("Esta impresora no puede imprimir ese resumen esa funcion es de la Bixolon 350 exclusivamente");
                }
                //return bRet;
            }
        }
        public void ReporteX()
        {
            string sCmd;
            bool bRet;
            try
            {
                //************ Imprimir Reporte X *******************
                sCmd = "I0X";
                bRet = fiscal.SendCmd( sCmd);
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        public void ReporteZ()
        {
            try
            {
                S1PrinterData d = fiscal.GetS1PrinterData();
                if (d.QuantityOfInvoicesToday < 1)
                {
                    throw new Exception("No hay facturas aún hoy");
                }
                ultimoZ = d.DailyClosureCounter + 2;
                //************ Imprimir Reporte Z *******************
                fiscal.SendCmd( "I0Z");
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        public void CargarX()
        {
            try
            {  
                ReportData x = fiscal.GetXReport();
                ultimaDevolucion = x.NumberOfLastCreditNote.ToString("00000000");
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        private double strToDouble(string p)
        {
            double Base = Convert.ToDouble(p.Substring(0, 10));
            double Decimales = Convert.ToDouble(p.Substring(10, 2));
            return Base + (Decimales / 100);
        }
        public void ReporteMensualIVA(DateTime dateTime, DateTime dateTime_2)
        {
            string sCmd;
            bool bRet;
            unsafe
            {
                //************ Imprimir Reporte Z *******************
                try
                {
                    System.Threading.Thread.Sleep(500);
                    string Inicio = dateTime.Day.ToString("00") + dateTime.Month.ToString("00") + dateTime.Year.ToString("00").Substring(2, 2);
                    string Final = dateTime_2.Day.ToString("00") + dateTime_2.Month.ToString("00") + dateTime_2.Year.ToString("00").Substring(2, 2);
                    sCmd = string.Format("I2M", Inicio, Final);
                    bRet = fiscal.SendCmd( sCmd);
                    if (bRet == false)
                    {
                        throw new Exception("Esta impresora no puede imprimir ese resumen esa funcion es de la Bixolon 350 exclusivamente");
                    }
                }
                catch (TfhkaNet.IF.PrinterException x)
                {
                    throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

                }
                catch (Exception x)
                {
                    throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
                }
            }
        }
        public bool IsOK()
        {
            if (iError == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void DocumentoNoFiscal(string[] Texto)
    {
            try
            {
                System.Threading.Thread.Sleep(500);
                foreach (string linea in Texto)
                {
                    bRet = fiscal.SendCmd( "800" + linea);
                }
                bRet = fiscal.SendCmd( "810");
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        public void ImprimeOrden(Factura documento)
        {
            try
            {
                System.Threading.Thread.Sleep(500);
                bRet = fiscal.SendCmd( string.Format("800 CLIENTE:{0}\n", documento.RazonSocial));
                bRet = fiscal.SendCmd( "800" + "      COMANDA    ");
                bRet = fiscal.SendCmd( String.Format("800{0} {1}", documento.Tipo, documento.NumeroOrden));
                foreach (var item in documento.FacturasPlatos)
                {
                    if (item.Cantidad.GetValueOrDefault(0) > 1)
                    {
                        bRet = fiscal.SendCmd( "800" + string.Format(" X {0}", item.Cantidad.Value.ToString("N0")));
                    }
                    bRet = fiscal.SendCmd( "800" + item.Descripcion);
                }
                bRet = fiscal.SendCmd( "810  ==========");
                // bRet = fiscal.SendCmd( "800 USTED SERA LLAMADO POR EL NUMERO=>" + documento.Numero.Substring(5,3));
                System.Threading.Thread.Sleep(500);
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                bRet = fiscal.SendCmd("810  ERROR.");
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        public void ImprimeCorte(MesasAbierta documento)
        {
            try
            {
                System.Threading.Thread.Sleep(500);
                bRet = fiscal.SendCmd( "800" + "   NOTA DE CONSUMO    ");
                bRet = fiscal.SendCmd( String.Format("800ATENDIDO POR:{0}", documento.Mesonero));
                bRet = fiscal.SendCmd( "800" + "MESA:" + documento.Mesa);
                if (!string.IsNullOrEmpty(documento.CedulaRif))
                {
                    bRet = fiscal.SendCmd( String.Format("800CEDULA/RIF:{0}", documento.CedulaRif));
                }
                if (!string.IsNullOrEmpty(documento.RazonSocial))
                {
                    bRet = fiscal.SendCmd( "800" + "RAZON SOCIAL:" + documento.RazonSocial);
                }
                bRet = fiscal.SendCmd( "800" + "CANT  DESCRIPCION                    MONTO" + documento.Mesonero);
                bRet = fiscal.SendCmd( "800" + "==========================================");
                foreach (var item in documento.MesasAbiertasPlatos)
                {
                    bRet = fiscal.SendCmd( string.Format("800" + "{0} {1} {2}", item.Cantidad.Value.ToString("n2").PadLeft(5), item.Descripcion.PadRight(25).Substring(0, 25), (item.Precio.GetValueOrDefault(0) * item.Cantidad.GetValueOrDefault(0)).ToString("n2").PadLeft(8)));
                }
                double Total = documento.MontoTotal.GetValueOrDefault(0);
                bRet = fiscal.SendCmd( "800" + "==========================================");
                bRet = fiscal.SendCmd( "800" + string.Format("   SUB TOTAL:{0}".PadLeft(33), (documento.MontoGravable.GetValueOrDefault(0)).ToString("N2").PadLeft(8)));
                bRet = fiscal.SendCmd( "800" + string.Format("SERVICIO 10%:{0}".PadLeft(33), (documento.MontoServicio.GetValueOrDefault(0)).ToString("N2").PadLeft(8)));
                bRet = fiscal.SendCmd( "800" + string.Format("   MONTO IVA:{0}".PadLeft(33), (documento.MontoIva.GetValueOrDefault(0)).ToString("N2").PadLeft(8)));
                bRet = fiscal.SendCmd( "800" + string.Format(" MONTO TOTAL:{0}".PadLeft(33), (Total).ToString("N2").PadLeft(8)));
                bRet = fiscal.SendCmd( "800" + "==========================================");
                bRet = fiscal.SendCmd( "800" + "Por favor escriba sus datos:");
                bRet = fiscal.SendCmd( "800" + "Cedula / Rif.:____________________________");
                bRet = fiscal.SendCmd( "800" + "Nombre / Razon Social:");
                bRet = fiscal.SendCmd( "800" + "__________________________________________");
                bRet = fiscal.SendCmd( "800" + "Direccion:");
                bRet = fiscal.SendCmd( "810" + "__________________________________________");

            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                bRet = fiscal.SendCmd("810  ERROR.");
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        public void ImprimeCorteSinMontos(MesasAbierta documento)
        {
            try
            {
                System.Threading.Thread.Sleep(500);
                bRet = fiscal.SendCmd("800" + "   PLATOS CONSUMIDOS    ");
                bRet = fiscal.SendCmd(String.Format("800ATENDIDO POR:{0}", documento.Mesonero));
                bRet = fiscal.SendCmd("800" + "MESA:" + documento.Mesa);                
                if (!string.IsNullOrEmpty(documento.CedulaRif))
                {
                    bRet = fiscal.SendCmd("800" + "CEDULA/RIF:" + documento.CedulaRif);
                }
                if (!string.IsNullOrEmpty(documento.RazonSocial))
                {
                    bRet = fiscal.SendCmd("800" + "RAZON SOCIAL:" + documento.RazonSocial);
                }                
                bRet = fiscal.SendCmd("800" + "CANT  DESCRIPCION               " + documento.Mesonero);
                bRet = fiscal.SendCmd("800" + "======================================");
                var resumido = (from x in documento.MesasAbiertasPlatos
                               where x.Precio.GetValueOrDefault() > 0 && x.Cantidad.GetValueOrDefault() > 0
                               group x by x.Descripcion);
                foreach (var item in resumido)
                {
                    bRet = fiscal.SendCmd(string.Format("800" + "{0} {1}",
                        item.Sum(c => c.Cantidad).Value.ToString("n2").PadLeft(5), item.Key.PadRight(22).Substring(0, 22)));
                }
                //double Total = documento.MontoTotal.GetValueOrDefault(0);
                //bRet = fiscal.SendCmd( "800" + "======================================");
                //bRet = fiscal.SendCmd( "800" + string.Format("   SUB TOTAL:{0}".PadLeft(33), (Total * 0.9).ToString("N2").PadLeft(8)));
                //bRet = fiscal.SendCmd( "800" + string.Format("SERVICIO 10%:{0}".PadLeft(33), (Total * 0.1).ToString("N2").PadLeft(8)));
                //bRet = fiscal.SendCmd( "800" + string.Format(" MONTO TOTAL:{0}".PadLeft(33), (Total).ToString("N2").PadLeft(8)));
                bRet = fiscal.SendCmd("810" + "======================================");
            }
            catch (TfhkaNet.IF.PrinterException x)
            {
                throw new Exception(string.Format("Error de impresora: {0}, estatus {1}", x.Message, fiscal.Estado));

            }
            catch (Exception x)
            {
                bRet = fiscal.SendCmd("810  ERROR.");
                throw new Exception(string.Format("Error de conexión\n{0}\nEstatus {1}", x.Message, fiscal.Status_Error));
            }
        }
        public int UltimoZ()
        {
            if (ultimoZ == null)
            {
                CargarS1(true);
            }
            return ultimoZ.GetValueOrDefault(0);
        }
        internal void ImprimirListado()
        {
            return;
        }
        public void Dispose()
        {
          //  DesconectarImpresora();
        }
    }
}
