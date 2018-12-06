﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security;

namespace HK.Formas
{
    public partial class FrmParametros : Form
    {
        VeneciaEntities db = new VeneciaEntities();
        Parametro parametro = new Parametro();
        public FrmParametros()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmParametros_Load);
        }

        void FrmParametros_Load(object sender, EventArgs e)
        {
            db = new VeneciaEntities();
            parametro = (from xx in db.Parametros
                            select xx).FirstOrDefault();
            this.parametroBindingSource.DataSource = parametro;
            this.parametroBindingSource.ResetBindings(true);
            this.Aceptar.Click += new EventHandler(Aceptar_Click);
            this.Cancelar.Click += new EventHandler(Cancelar_Click);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FrmParametros_KeyDown);            
            this.PuertoImpresoraFiscalComboBoxEdit.Properties.Items.AddRange(new string[] { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6","COM7", "COM8" });
            this.ImprimirOrdenComboBoxEdit.Properties.Items.AddRange(new string[] { "WINDOWS", "FISCAL" });
            this.ImprimirCorteCuentaComboBoxEdit.Properties.Items.AddRange(new string[] { "WINDOWS", "FISCAL" });
        }

        private void Aceptar_Click(object sender, EventArgs e)
        {
            PassEncrypt.Encrypt claves = new PassEncrypt.Encrypt();
            try
            {
                parametroBindingSource.EndEdit();
                parametro = (Parametro)parametroBindingSource.Current;
                parametro.Licencia = claves.GetHashKey(parametro.Empresa);
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
            this.parametroBindingSource.ResetCurrentItem();
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
