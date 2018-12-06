﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HK.Clases;

namespace HK.Formas
{
    public partial class FrmCajeros : Form
    {
        public FrmCajeros()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmCajeros_Load);
            this.FormClosed += new FormClosedEventHandler(FrmCajeros_FormClosed);
        }

        void FrmCajeros_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pantallas.CajerosLista = null;
        }

        void FrmCajeros_Load(object sender, EventArgs e)
        {
            this.gridControl1.KeyDown += new KeyEventHandler(gridControl1_KeyDown);
            this.gridControl1.DoubleClick += new EventHandler(gridControl1_DoubleClick);
            this.btnNuevo.Click += new EventHandler(btnNuevo_Click);
            this.btnEditar.Click += new EventHandler(btnEditar_Click);
            this.btnEliminar.Click += new EventHandler(btnEliminar_Click);
            this.btnBuscar.Click += new EventHandler(btnBuscar_Click);
            this.txtBuscar.Validating += new CancelEventHandler(txtBuscar_Validating);
            this.btnImprimir.Click += new EventHandler(btnImprimir_Click);
            Busqueda();
        }
        VeneciaEntities db = new VeneciaEntities();
        List<Usuario> Lista = new List<Usuario>();
        public string filtro;
        void btnImprimir_Click(object sender, EventArgs e)
        {
            FrmReportes f = new FrmReportes();
            f.ListadoUsuarios(Lista);
            f = null;
        }
        void txtBuscar_Validating(object sender, CancelEventArgs e)
        {
            Busqueda();
        }
        void btnBuscar_Click(object sender, EventArgs e)
        {
            Busqueda();
        }
        void btnEditar_Click(object sender, EventArgs e)
        {
            EditarRegistro();
        }
        void btnNuevo_Click(object sender, EventArgs e)
        {
            AgregarRegistro();
        }
        private void Buscar_Click(object sender, EventArgs e)
        {
            Busqueda();
        }
        private void Busqueda()
        {
            db = new VeneciaEntities();
            Lista = FactoryUsuarios.getItems(db,this.txtBuscar.Text,"CAJERO");
            this.bs.DataSource = Lista;
            this.bs.ResetBindings(true);
        }
        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gridView1.ActiveEditor == null)
            {
                if (e.KeyCode == Keys.Return)
                {
                    EditarRegistro();
                }
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Subtract)
                {
                    EliminarRegistro();
                }
                if (e.KeyCode == Keys.Insert)
                {
                    AgregarRegistro();
                }
            }
        }
        private void AgregarRegistro()
        {
            FrmCajerosItem F = new FrmCajerosItem();
            F.Incluir();
            if (F.DialogResult == DialogResult.OK)
            {
                F.registro.Activo = true;
                F.registro.IdUsuario = FactoryContadores.GetMax("IdUsuario");
                db.Usuarios.Add(F.registro);                
                db.SaveChanges();
                Busqueda();
            }
        }
        private void EditarRegistro()
        {
            FrmCajerosItem F = new FrmCajerosItem();
            Usuario registro = (Usuario)this.bs.Current;
            if (registro == null)
                return;
            F.registro = registro;
            F.Modificar();
            if (F.DialogResult == DialogResult.OK)
            {
                db.SaveChanges();
                Busqueda();
            }
        }
        private void EliminarRegistro()
        {
            if (this.gridView1.IsFocusedView)
            {
                Usuario Registro = (Usuario)this.bs.Current;
                if (Registro == null)
                    return;
                if (MessageBox.Show("Esta seguro de eliminar este registro", "Atencion", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                try
                {
                    Registro.Activo = false;
                    db.SaveChanges();
                    Busqueda();
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
                }
            }
        }
        private void Nuevo_Click(object sender, EventArgs e)
        {
            AgregarRegistro();
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            EliminarRegistro();
        }
        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            EditarRegistro();
        }
    }
}
