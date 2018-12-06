using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HK.Clases;
using HK.Formas;

namespace HK
{
    public partial class FrmPagar : Form
    {
        public Factura factura = new Factura();
        public Cliente cliente = new Cliente();
        public bool? cobraServicio=false;
        public double descuento = 0;
        public FrmPagar()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmPagar_Load);
        }
        void txtCedulaRif_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FrmBuscarEntidades f = new FrmBuscarEntidades();
            f.BuscarClientes("");
            LeerCliente((Cliente)f.registro);
        }
        void txtCedulaRif_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit Editor = (DevExpress.XtraEditors.TextEdit)sender;
            if (!Editor.IsModified)
                return;
            string texto = Editor.Text;
            this.facturaBindingSource.EndEdit();
            List<Cliente> items = FactoryClientes.getItems(texto);
            switch (items.Count)
            {
                case 0:
                    LeerCliente(null, texto);
                    break;
                case 1:
                    LeerCliente(items[0]);
                    break;
                default:
                    FrmBuscarEntidades f = new FrmBuscarEntidades();
                    f.BuscarClientes(texto);
                    LeerCliente((Cliente) f.registro);
                    break;
            }
            
        }
        private void LeerCliente(Cliente cliente = null, string cedulaRif = null)
        {
            if (cliente == null)
            {
                factura.CedulaRif = Basicas.CedulaRif(cedulaRif);
                factura.RazonSocial = "";
                factura.Direccion = Basicas.parametros().Ciudad;
            }
            else 
            {
                factura.CedulaRif = cliente.CedulaRif;
                factura.RazonSocial = cliente.RazonSocial;
                factura.Direccion = cliente.Direccion;      
            }
            this.facturaBindingSource.ResetCurrentItem();
        }
        void ConsumoInterno_Click(object sender, EventArgs e)
        {
            PagarConsumoInterno();
        }
        void CestaTicket_Click(object sender, EventArgs e)
        {
            PagarCestaTicket();
        }
        void Tarjeta_Click(object sender, EventArgs e)
        {
            PagarTarjeta();
        }
        void Cheque_Click(object sender, EventArgs e)
        {
            PagarCheque();
        }
        void Efectivo_Click(object sender, EventArgs e)
        {
            PagarEfectivo();
        }
        void TarjetaTextEdit_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            factura.Tarjeta = (double)editor.Value;
            this.facturaBindingSource.ResetCurrentItem();
        }
        void CestaTicketTextEdit_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            factura.CestaTicket = (double)editor.Value;
            this.facturaBindingSource.ResetCurrentItem();
        }
        void ChequeTextEdit_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            factura.Cheque = (double)editor.Value;
            this.facturaBindingSource.ResetCurrentItem();
        }
        void EfectivoTextEdit_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            factura.Efectivo = (double)editor.Value;
            this.facturaBindingSource.ResetCurrentItem();
        }
        void Editor_Enter(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            editor.Value = (decimal)factura.Saldo;
            editor.SelectAll();
        }
        void FrmPagar_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Cancelar.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.F12:
                    this.Aceptar.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.F4:
                    PagarEfectivo();
                    e.Handled = true;
                    break;
                case Keys.F5:
                    PagarTarjeta();
                    e.Handled = true;
                    break;
                case Keys.F6:
                    PagarCestaTicket();
                    e.Handled = true;
                    break;
                case Keys.F7:
                    PagarCheque();
                    e.Handled = true;
                    break;
                case Keys.F11:
                    PagarConsumoInterno();
                    e.Handled = true;
                    break;
            } 
        }
        private void PagarCheque()
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            factura.calcularSaldo();
            ChequeTextEdit.Value = (decimal)factura.Saldo;
            ChequeTextEdit.SelectAll();
            this.ChequeTextEdit.Focus();
        }
        private void PagarCestaTicket()
        {            
            LimpiarPagos(Basicas.parametros().TasaIva);
            factura.calcularSaldo();
            CestaTicketTextEdit.Value = (decimal)factura.Saldo;
            CestaTicketTextEdit.SelectAll();
            this.CestaTicketTextEdit.Focus();
        }
        private void PagarTarjeta()
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            factura.Tarjeta = factura.MontoTotal;
            factura.Totalizar(cobraServicio.Value, descuento, CalcularTasaIva());            
            factura.Tarjeta = factura.MontoTotal;
            factura.Saldo = 0;
            TarjetaTextEdit.Value = (decimal)factura.Tarjeta;
            MontoTotalTextEdit.EditValue = factura.MontoTotal;
            SaldoTextEdit.EditValue = 0;
        }
        private void PagarEfectivo()
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            factura.calcularSaldo();
            EfectivoTextEdit.Value = (decimal)factura.Saldo;
            EfectivoTextEdit.SelectAll();
            this.EfectivoTextEdit.Focus();
        }
        private void PagarConsumoInterno()
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            factura.calcularSaldo();
            factura.ConsumoInterno = factura.MontoTotal;
            this.txtAutorizadoPor.Properties.Items.AddRange(FactoryUsuarios.getUsuarios());
            this.txtAutorizadoPor.Focus();
        }
        private void LimpiarPagos(double? tasaIva)
        {
            factura.Cheque = null;
            factura.Tarjeta = null;
            factura.CestaTicket = null;
            factura.Efectivo = null;
            factura.ConsumoInterno = null;
            factura.Credito = null;
            factura.Totalizar(cobraServicio.Value, descuento, tasaIva);
            factura.calcularSaldo();
            ChequeTextEdit.Text = null;
            EfectivoTextEdit.Text = null;
            CestaTicketTextEdit.Text = null;
            TarjetaTextEdit.Text = null;
            MontoTotalTextEdit.EditValue = factura.MontoTotal;
        }
        void Cancelar_Click(object sender, EventArgs e)
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        void Aceptar_Click(object sender, EventArgs e)
        {
            facturaBindingSource.EndEdit();
            if (factura.CedulaRif == null)
            {
                MessageBox.Show("Cedula o rif no puede estar vacio", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtCedulaRif.Focus();
                return;
            }
            if (factura.RazonSocial == null)
            {
                MessageBox.Show("El nombre no puede estar vacio", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtRazonSocial.Focus();
                return;
            }
            if (factura.Efectivo.HasValue)
            {
                if (factura.Cambio.GetValueOrDefault(0) > factura.Efectivo.GetValueOrDefault(0))
                {
                    MessageBox.Show("El cambio no puede ser mayor al monto en efectivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                factura.Efectivo = factura.Efectivo.GetValueOrDefault(0) - factura.Cambio.GetValueOrDefault(0);
            }
            if ((factura.ConsumoInterno.HasValue && FactoryUsuarios.UsuarioActivo.PuedeDarConsumoInterno == false) || (factura.Credito.HasValue && FactoryUsuarios.UsuarioActivo.PuedeDarCreditos == false))
            {
                Usuario Autorizado = FactoryUsuarios.Item(txtAutorizadoPor.Text, txtContraseña.Text);
                if (Autorizado == null)
                {
                    MessageBox.Show("Es necesario elejir un usuario con autorizacion para otorgar el consumo interno");
                    return;
                }
                if (Autorizado.PuedeDarConsumoInterno == false)
                {
                    MessageBox.Show("El usuario elegido no tiene de autorizado el consumo interno");
                    return;
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        void FrmPagar_Load(object sender, EventArgs e)
        {
            this.Aceptar.Click += new EventHandler(Aceptar_Click);
            this.Cancelar.Click += new EventHandler(Cancelar_Click);
            this.KeyDown += new KeyEventHandler(FrmPagar_KeyDown);
            this.EfectivoTextEdit.Enter += new EventHandler(Editor_Enter);
            this.ChequeTextEdit.Enter += new EventHandler(Editor_Enter);
            this.TarjetaTextEdit.Enter += new EventHandler(Editor_Enter);
            this.CestaTicketTextEdit.Enter += new EventHandler(Editor_Enter);
            this.EfectivoTextEdit.Validating += new CancelEventHandler(EfectivoTextEdit_Validating);
            this.ChequeTextEdit.Validating += new CancelEventHandler(ChequeTextEdit_Validating);
            this.CestaTicketTextEdit.Validating += new CancelEventHandler(CestaTicketTextEdit_Validating);
            this.TarjetaTextEdit.Validating += new CancelEventHandler(TarjetaTextEdit_Validating);
            this.Efectivo.Click += new EventHandler(Efectivo_Click);
            this.Cheque.Click += new EventHandler(Cheque_Click);
            this.Tarjeta.Click += new EventHandler(Tarjeta_Click);
            this.CestaTicket.Click += new EventHandler(CestaTicket_Click);
            this.ConsumoInterno.Click += new EventHandler(ConsumoInterno_Click);
            this.Credito.Click += new EventHandler(Credito_Click);
            this.txtCedulaRif.Validating += new CancelEventHandler(txtCedulaRif_Validating);
            this.txtCedulaRif.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(txtCedulaRif_ButtonClick);
            factura.CedulaRif = Basicas.CedulaRif("0");
            factura.RazonSocial = "CONTADO";
            factura.Direccion = Basicas.parametros().Ciudad;
            this.KeyPreview = true;
            if (FactoryUsuarios.UsuarioActivo.PuedeDarConsumoInterno == true)
            {
                this.txtAutorizadoPor.Visible = false;
                this.txtContraseña.Visible = false;
            }
            factura.Saldo = factura.MontoTotal;
            this.facturaBindingSource.DataSource = factura;
            this.facturaBindingSource.ResetBindings(true);
        }

        void Credito_Click(object sender, EventArgs e)
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            factura.Credito = factura.MontoTotal;
            factura.Totalizar(cobraServicio.Value, descuento,12);
            this.txtAutorizadoPor.Properties.Items.AddRange(FactoryUsuarios.getUsuarios());
            this.txtAutorizadoPor.Focus();
        }

        double CalcularTasaIva()
        {
            var tasa = Basicas.parametros().TasaIva.GetValueOrDefault();
            if( factura.Tarjeta < factura.MontoTotal )
            {
                return tasa;
            }
            if( this.txtCedulaRif.Text.Length > 0)
            {
                var db = new VeneciaEntities();
                var impuesto = (from xx in db.Impuestos select xx).FirstOrDefault();
                if (impuesto == null)
                {
                    return tasa;
                }
                if (impuesto.InicialesReducido.Contains(this.txtCedulaRif.Text[0]) &&
                    factura.MontoTotal <= (double)impuesto.TopeReduccion)
                {
                    return (double)impuesto.TasaReducida;
                }
                if (impuesto.InicialesReducido2.Contains(this.txtCedulaRif.Text[0]))
                {
                    return (double)impuesto.TasaReducida2;
                }
            }
            return tasa;
        }
    }
}
