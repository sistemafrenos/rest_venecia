using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace HK.Formas
{
    public partial class FrmImpuestos : Form
    {
        VeneciaEntities db = new VeneciaEntities();
        Impuesto impuesto = new Impuesto();
        public FrmImpuestos()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmParametros_Load);
        }

        void FrmParametros_Load(object sender, EventArgs e)
        {
            db = new VeneciaEntities();
            impuesto = (from xx in db.Impuestos
                            select xx).FirstOrDefault();
            if (impuesto == null)
            {
                impuesto = new Impuesto();
                impuesto.InicialesReducido = "VEJG";
                impuesto.InicialesReducido2 = "VEJG";
                impuesto.TipoImpuesto = "EXCLUIDO";
                impuesto.TasaNormal = 12;
                impuesto.TasaReducida = 9;
                impuesto.TasaReducida2 = 7;
                impuesto.TopeReduccion = 2000000;
            }
            this.impuestoBindingSource.DataSource = impuesto;
            this.impuestoBindingSource.ResetBindings(true);
            this.Aceptar.Click += new EventHandler(Aceptar_Click);
            this.Cancelar.Click += new EventHandler(Cancelar_Click);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FrmParametros_KeyDown);
        }

        private void Aceptar_Click(object sender, EventArgs e)
        {
            try
            {
                impuestoBindingSource.EndEdit();
                impuesto = (Impuesto)impuestoBindingSource.Current;
                if ( impuesto.Id == 0)
                {
                    db.Impuestos.Add(impuesto);
                }
                db.SaveChanges();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los datos \n" + ex.Source + "\n" + ex.Message, "Atencion", MessageBoxButtons.OK);
            }
        }
        private void Cancelar_Click(object sender, EventArgs e)
        {
            this.impuestoBindingSource.ResetCurrentItem();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void FrmParametros_KeyDown(object sender, KeyEventArgs e)
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
            }
        }
    }
}
