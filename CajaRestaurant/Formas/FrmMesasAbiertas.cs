﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HK.Clases;

namespace HK.Formas
{
    public partial class FrmMesasAbiertas : Form
    {
        List<Button> salones = new List<Button>();
        List<PictureBox> mesas = new List<PictureBox>();
        VeneciaEntities db = new VeneciaEntities();
        private bool ImpresoraEnUso = false;
        private string salon;

        public bool ImpresoraEnUso1 { get => ImpresoraEnUso; set => ImpresoraEnUso = value; }

        public FrmMesasAbiertas()
        {
            InitializeComponent();
            this.Load+=new EventHandler(FrmMesasAbiertas_Load);
        }
        void  FrmMesasAbiertas_Load(object sender, EventArgs e)
        {
            salones.AddRange(new Button[] { ubicacion1, ubicacion2, ubicacion3, ubicacion4, ubicacion5 });
            mesas.AddRange(new PictureBox[] { mesa01, mesa02, mesa03, mesa04, mesa05, mesa06, mesa07, mesa08, mesa09, mesa10,
                                              mesa11, mesa12, mesa13, mesa14, mesa15, mesa16, mesa17, mesa18, mesa19, mesa20,
                                              mesa21, mesa22, mesa23, mesa24, mesa25, mesa26, mesa27, mesa28, mesa29, mesa30});
            OcultarMesas();
            foreach (Button b in salones)
            {
                b.Click += new EventHandler(Salon_Click);
                b.Dock = DockStyle.None;
                b.Visible = false;
                b.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
            }
            foreach (PictureBox control in mesas)
            {
                control.Click += new EventHandler(Mesa_Click);
                control.Paint += new PaintEventHandler(mesa_Paint);
                control.Dock = DockStyle.None;
                control.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
            }
            CargarSalones();
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 100;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 50;
            timer1.Tick += new EventHandler(timer1_Tick);
            this.NombreCliente.Text = Basicas.parametros().Empresa;
            this.btnVale.Visible = FactoryUsuarios.UsuarioActivo.Vale.GetValueOrDefault(false);
            //this.btnCierreCaja.Visible = FactoryUsuarios.UsuarioActivo.Vale.GetValueOrDefault(false);
            this.btnReporteX.Visible = FactoryUsuarios.UsuarioActivo.Vale.GetValueOrDefault(false);
            this.btnReporteZ.Visible = FactoryUsuarios.UsuarioActivo.Vale.GetValueOrDefault(false);
            //this.btnContarDinero.Visible = FactoryUsuarios.UsuarioActivo.Vale.GetValueOrDefault(false);
            this.btnVale.Click += new EventHandler(btnVale_Click);
            this.btnCierreCaja.Click += new EventHandler(btnCierreCaja_Click);
            this.btnReporteX.Click += new EventHandler(btnReporteX_Click);
            this.btnReporteZ.Click += new EventHandler(btnReporteZ_Click);
            this.btnContarDinero.Click += new EventHandler(btnContarDinero_Click);
            this.btnSalir.Click += new EventHandler(btnSalir_Click);
            this.btnMinimizar.Click += new EventHandler(btnMinimizar_Click);
            this.CenterToScreen();
            //  ImprimirFacturasPendientes();
        }

        void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        void btnSalir_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        void btnContarDinero_Click(object sender, EventArgs e)
        {
            FrmContarDinero f = new FrmContarDinero();
            f.ShowDialog();
        }
        void btnReporteZ_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Esta seguro de emitir el reporte Z", "Atencion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;
            this.ImpresoraEnUso1 = true;
            Fiscales.FiscalBixolon2010 f = new Fiscales.FiscalBixolon2010();
            f.ReporteZ();
            f = null;
            this.ImpresoraEnUso1 = false;
        }
        void btnReporteX_Click(object sender, EventArgs e)
        {
            this.ImpresoraEnUso1 = true;
            Fiscales.FiscalBixolon2010 f = new Fiscales.FiscalBixolon2010();
            f.ReporteX();
            f = null;
            this.ImpresoraEnUso1 = false;
        }
        void btnCierreCaja_Click(object sender, EventArgs e)
        {
            //FrmReportes f = new FrmReportes();
            //f.CierreDeCaja();
            // Basicas.CierreDeCaja();
        }
        void btnVale_Click(object sender, EventArgs e)
        {
            FrmVale f = new FrmVale();
            f.Incluir();
        }
        void timer1_Tick(object sender, EventArgs e)
        {
            db = new VeneciaEntities();
            CargarMesasAbiertas(salon);
        }
        private void OcultarMesas()
        {
            foreach (PictureBox b in mesas)
            {
                b.CreateGraphics().Clear(this.BackColor);
            }
        }
        void Salon_Click(object sender, EventArgs e)
        {
            Button item = (Button)sender;
            CargarMesasAbiertas(item.Text);
            salon = item.Text;

        }
        void Mesa_Click(object sender, EventArgs e)
        {
            PictureBox item = (PictureBox)sender;
            this.timer1.Enabled = false;
            EditarMesa((Mesa)item.Tag);
            this.timer1.Enabled = true;
        }
        void CargarMesasAbiertas(string salon)
        {

            //    using (VeneciaEntities db = new VeneciaEntities())
            {
                var MesasAbiertas = (from xx in db.MesasAbiertas
                                    orderby xx.Mesa
                                    select xx).ToList();
                var mMesas = (from x in db.Mesas
                              orderby x.Descripcion
                              where x.Ubicacion == salon
                              select x).ToList();
                foreach (var m in mMesas)
                {
                   MesasAbierta mesa=MesasAbiertas.FirstOrDefault(x => x.IdMesa == m.IdMesa);
                   if (mesa != null)
                   {
                       m.Apertura = mesa.Apertura.GetValueOrDefault(DateTime.Now);
                       m.Numero = mesa.Numero;
                       m.Monto = mesa.MontoTotal;
                       m.Impresa = mesa.Estatus == "IMPRESA" ? true : false;
                       m.Mesonero = mesa.Mesonero;
                   }
                }   
                int i = 0;
                OcultarMesas();
                foreach (Mesa s in mMesas)
                {
                    try
                    {
                        mesas[i].Tag = s;
                        mesas[i].Refresh();
                        i++;
                    }
                    catch { }
                }
            }
        }
        void CargarSalones()
        {
            using (VeneciaEntities db = new VeneciaEntities())
            {
                List<string> mUbicaciones = FactoryMesas.getUbicaciones();
                int i = 0;
                if (mUbicaciones.Count == 0)
                {
                    return; 
                }

                foreach (string s in mUbicaciones)
                {
                    Mesa p = (from y in db.Mesas
                              where y.Ubicacion == s
                              orderby y.Ubicacion
                              select y).FirstOrDefault();
                    salones[i].Visible = true;
                    salones[i].Text = s;
                    i++;
                }
                salon = salones[0].Text;
                CargarMesasAbiertas(salon);
            }            
        }
        void EditarMesa(Mesa mesa)
        {
            if (mesa == null)
                return;
            FrmEditarMesa f = new FrmEditarMesa();
            f.mesa = FactoryMesas.Item(db,mesa.IdMesa); 
            f.mesaAbierta = (from xx in db.MesasAbiertas
                            where mesa.IdMesa == xx.IdMesa
                            select xx).FirstOrDefault();
            f.ShowDialog();
            db = new VeneciaEntities();
            CargarMesasAbiertas(salon);
            Application.DoEvents();
        }
        private void mesa_Paint(object sender, PaintEventArgs e)
        {
            Graphics control = e.Graphics;
            control.Clear(this.BackColor);
            Mesa m = (Mesa)((PictureBox)sender).Tag;
            if (m == null)
            {
                return;
            }
            Font fuente = new Font("Verdana", 10, FontStyle.Bold);
            control.FillRectangle(SystemBrushes.ActiveCaption, 0, 0, control.ClipBounds.Width, 20);
            control.DrawString(m.Descripcion,fuente , SystemBrushes.ActiveCaptionText, 0, 0);
            if (m.Impresa==true)
                control.DrawImage(this.imageCollection1.Images[0], new PointF(140, 2));
            if(m.Monto.GetValueOrDefault(0)>0)
            {
                control.DrawString(m.Numero, fuente, Brushes.Black, new PointF(10, 20));
                control.DrawString(m.Apertura.ToShortTimeString(), fuente, Brushes.Black, new PointF(110, 30));
                control.DrawString(m.Mesonero, fuente, Brushes.Black, new PointF(10, 45));
                control.DrawString(m.Monto.GetValueOrDefault(0).ToString("n2").PadLeft(15), fuente, Brushes.Black, new PointF(60, 60));
            }
          }
        }
    }

 