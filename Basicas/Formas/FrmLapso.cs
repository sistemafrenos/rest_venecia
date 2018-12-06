using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HK.Formas
{
    public partial class FrmLapso : Form
    {
        private DateTime desde = DateTime.Today;
        private DateTime hasta = DateTime.Today;
        private bool detallado = false;

        public DateTime Desde { get => desde; set => desde = value; }
        public DateTime Hasta { get => hasta; set => hasta = value; }
        public bool Detallado { get => detallado; set => detallado = value; }

        public FrmLapso()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmLapso_Load);
        }

        void FrmLapso_Load(object sender, EventArgs e)
        {
            this.txtDesde.DateTime = Desde;
            this.txtHasta.DateTime = Hasta;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FrmLapso_KeyDown);
            this.Aceptar.Click+=new EventHandler(Aceptar_Click);
            this.Cancelar.Click+=new EventHandler(Cancelar_Click);
            this.txtDetallado.Checked = Detallado;
            this.txtDetallado.Visible = Detallado;
        }

        void FrmLapso_KeyDown(object sender, KeyEventArgs e)
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
        void Cancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        void Aceptar_Click(object sender, EventArgs e)
        {
            this.Desde = txtDesde.DateTime;
            this.Hasta = txtHasta.DateTime;
            this.Detallado = txtDetallado.Checked;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
