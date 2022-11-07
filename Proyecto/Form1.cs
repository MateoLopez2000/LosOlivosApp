using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using AForge.Video.DirectShow;
using AForge.Video;
using System.util;
using OrthoAnalisis.Properties;

namespace Proyecto
{
    public partial class Form1 : Form
    {
        private bool ExistDevices;
        private FilterInfoCollection MyDevices;
        private VideoCaptureDevice MyWebCam;
        private VideoCaptureDevice MyWebCam2;
        private bool photoTaken1 = false;
        private bool photoTaken2 = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void btndescargar_Click(object sender, EventArgs e)
        {
            if (validateFields()) {
                SaveFileDialog savefile = new SaveFileDialog();

                string PaginaHTML_Texto = Resources.Plantilla.ToString();
                PaginaHTML_Texto = PaginaHTML_Texto.Replace("@Nombre", txtnombres.Text);
                PaginaHTML_Texto = PaginaHTML_Texto.Replace("@Edad", txtEdad.Text);
                PaginaHTML_Texto = PaginaHTML_Texto.Replace("@Genero", combosex.SelectedItem.ToString());
                PaginaHTML_Texto = PaginaHTML_Texto.Replace("@Diagnostico", txtDiagnostico.Text);
                PaginaHTML_Texto = PaginaHTML_Texto.Replace("@Fecha", DateTime.Now.ToString("dd/MM/yyyy"));


                savefile.FileName = string.Format("{0}-{1}.pdf", txtnombres.Text.Replace(" ", "") ,DateTime.Now.ToString("dd-MM-yyyy"));


                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                    {
                        Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);

                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        pdfDoc.Add(new Phrase(""));

                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Resources.olivos, System.Drawing.Imaging.ImageFormat.Png);
                        img.ScaleToFit(120, 120);
                        img.Alignment = iTextSharp.text.Image.UNDERLYING;

                        img.SetAbsolutePosition(pdfDoc.LeftMargin, pdfDoc.Top - 60);
                        pdfDoc.Add(img);

                        iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(picBox1.Image, System.Drawing.Imaging.ImageFormat.Png);
                        img2.ScaleToFit(240, 240);
                        img2.Alignment = iTextSharp.text.Image.UNDERLYING;

                        img2.SetAbsolutePosition(pdfDoc.LeftMargin + 170, pdfDoc.Bottom + 262);
                        pdfDoc.Add(img2);


                        iTextSharp.text.Image img3 = iTextSharp.text.Image.GetInstance(picBox2.Image, System.Drawing.Imaging.ImageFormat.Png);
                        img3.ScaleToFit(250, 250);
                        img3.Alignment = iTextSharp.text.Image.UNDERLYING;

                        img3.SetAbsolutePosition(pdfDoc.LeftMargin + 170, pdfDoc.Bottom + 8);
                        pdfDoc.Add(img3);

                        using (StringReader sr = new StringReader(PaginaHTML_Texto))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                        }

                        pdfDoc.Close();
                        stream.Close();
                        btnDeleteAll_Click(sender, e);
                    }
                }
            } else
            {
                MessageBox.Show("Llene Todos los Campos y Capture las dos Imagenes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool validateFields()
        {
            return ( photoTaken1 && photoTaken2 
                && txtDiagnostico.Text.Length > 0
                && txtEdad.Text.Length > 0
                && txtnombres.Text.Length > 0 ? true : false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDevices();
            combosex.SelectedIndex = 0;
        }

        public void LoadDevices()
        {
            MyDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (MyDevices.Count > 0)
            {
                ExistDevices = true;
                for (int i = 0; i < MyDevices.Count; i++) { 
                        cboDevices.Items.Add(MyDevices[i].Name.ToString());
                        cboDevices2.Items.Add(MyDevices[i].Name.ToString());
                    }
                cboDevices.Text = MyDevices[0].Name.ToString();
                cboDevices2.Text = MyDevices[0].Name.ToString();
            }
            else
                ExistDevices = false;
        }

        private void CloseWebCam()
        {
            if (MyWebCam != null && MyWebCam.IsRunning)
            {
                MyWebCam.SignalToStop();
                MyWebCam = null;
            }
        }

        private void CloseWebCam2()
        {
            if (MyWebCam2 != null && MyWebCam2.IsRunning)
            {
                MyWebCam2.SignalToStop();
                MyWebCam2 = null;
            }
        }
        private void resetFields()
        {
            txtnombres.ResetText();
            txtEdad.ResetText();
            txtDiagnostico.ResetText();
            combosex.SelectedIndex=0;
        }

        private new void Capture(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = new Bitmap((Bitmap)eventArgs.Frame.Clone(), new Size(picBox1.Width,picBox1.Height));
            picBox1.Image = image;
        }
        private void Capture2(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = new Bitmap((Bitmap)eventArgs.Frame.Clone(), new Size(picBox2.Width, picBox2.Height));
            picBox2.Image = image;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseWebCam();
            CloseWebCam2();
        }

        private void startCamera1(object sender, EventArgs e)
        {
            if (ExistDevices)
            {
                CloseWebCam();
                photoTaken1 = false;
                int i = cboDevices.SelectedIndex;
                string deviceName = MyDevices[i].MonikerString;
                MyWebCam = new VideoCaptureDevice(deviceName);
                MyWebCam.NewFrame += new NewFrameEventHandler(Capture);
                MyWebCam.Start();
                btnTakePhoto.Visible = true;
            }
        }
        private void startCamera2(object sender, EventArgs e)
        {
            if (ExistDevices)
            {
                CloseWebCam2();
                photoTaken2 = false;
                int i = cboDevices2.SelectedIndex;
                string deviceName = MyDevices[i].MonikerString;
                MyWebCam2 = new VideoCaptureDevice(deviceName);
                MyWebCam2.NewFrame += new NewFrameEventHandler(Capture2);
                MyWebCam2.Start();
                btnTakePhoto2.Visible = true;
            }
        }

        private void btnTakePhoto_Click(object sender, EventArgs e)
        {
            if (MyWebCam != null && MyWebCam.IsRunning)
            {
                photoTaken1 = true;
                CloseWebCam();
                btnTakePhoto.Visible = false;
                btnCaptureAgain.Visible = true;
            }
        }

        private void btnTakePhoto2_Click(object sender, EventArgs e)
        {
            if (MyWebCam2 != null && MyWebCam2.IsRunning)
            {
                photoTaken2 = true;
                CloseWebCam2();
                btnCaptureAgain2.Visible = true;
                btnTakePhoto2.Visible = false;
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            startCamera1(sender, e);
            startCamera2(sender, e);
            resetFields();
            photoTaken1 = false;
            photoTaken2 = false;
        }

    }
}
