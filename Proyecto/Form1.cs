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
using System.Linq;

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
        int camera1 = 10;
        int camera2 = 10;


        List<System.Drawing.Image> images = new List<System.Drawing.Image>();
        public Form1()
        {
            InitializeComponent();
        }

        private void btndescargar_Click(object sender, EventArgs e)
        {
            if (validateFields()) {
                SaveFileDialog savefile = new SaveFileDialog();

                string PaginaHTML_Texto = Resources.Plantilla.ToString();
                string Pagina2 = Resources.Plantilla2.ToString();
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

                        using (StringReader sr = new StringReader(PaginaHTML_Texto))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                        }


                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Resources.olivos, System.Drawing.Imaging.ImageFormat.Png);
                        logo.ScaleToFit(120, 120);
                        logo.Alignment = iTextSharp.text.Image.UNDERLYING;

                        logo.SetAbsolutePosition(pdfDoc.LeftMargin, pdfDoc.Top - 60);
                        pdfDoc.Add(logo);

                        iTextSharp.text.Image img1 = iTextSharp.text.Image.GetInstance(images[0], System.Drawing.Imaging.ImageFormat.Png);
                        img1.ScaleToFit(220, 220);
                        img1.Alignment = iTextSharp.text.Image.UNDERLYING;

                        img1.SetAbsolutePosition(pdfDoc.LeftMargin + 170, pdfDoc.Bottom + 256);
                        pdfDoc.Add(img1);


                        iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(images[1], System.Drawing.Imaging.ImageFormat.Png);
                        img2.ScaleToFit(220, 220);
                        img2.Alignment = iTextSharp.text.Image.UNDERLYING;

                        img2.SetAbsolutePosition(pdfDoc.LeftMargin + 170, pdfDoc.Bottom + 32);
                        pdfDoc.Add(img2);


                        pdfDoc.NewPage();


                        using (StringReader sr = new StringReader(Pagina2))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                        }

                        pdfDoc.Add(logo);

                        iTextSharp.text.Image img3 = iTextSharp.text.Image.GetInstance(images[2], System.Drawing.Imaging.ImageFormat.Png);
                        img3.ScaleToFit(220, 220);
                        img3.Alignment = iTextSharp.text.Image.UNDERLYING;

                        img3.SetAbsolutePosition(pdfDoc.LeftMargin + 170, pdfDoc.Bottom + 456);
                        pdfDoc.Add(img3);


                        iTextSharp.text.Image img4 = iTextSharp.text.Image.GetInstance(images[3], System.Drawing.Imaging.ImageFormat.Png);
                        img4.ScaleToFit(220, 220);
                        img4.Alignment = iTextSharp.text.Image.UNDERLYING;

                        img4.SetAbsolutePosition(pdfDoc.LeftMargin + 170, pdfDoc.Bottom + 232);
                        pdfDoc.Add(img4);

                        iTextSharp.text.Image img5 = iTextSharp.text.Image.GetInstance(images[4], System.Drawing.Imaging.ImageFormat.Png);
                        img5.ScaleToFit(220, 220);
                        img5.Alignment = iTextSharp.text.Image.UNDERLYING;

                        img5.SetAbsolutePosition(pdfDoc.LeftMargin + 170, pdfDoc.Bottom + 8);
                        pdfDoc.Add(img5);

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

            startCamera1(sender, e);
            startCamera2(sender, e);
            combosex.SelectedIndex = 0;
        }

        public void LoadDevices()
        {
            MyDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (MyDevices.Count == 1)
            {
                ExistDevices = true;
                camera1 = 0;
                camera2 = 0;
            }
            else if (MyDevices.Count > 1) {
                ExistDevices = true;
                camera1 = 0;
                camera2 = 1;
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
                string deviceName = MyDevices[camera1].MonikerString;
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
                string deviceName = MyDevices[camera2].MonikerString;
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
                nextButton1.Visible = true;
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
                nextButton2.Visible = true;
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            images.Clear();
            LoadDevices();
            startCamera1(sender, e);
            startCamera2(sender, e);
            resetFields();
            photoTaken1 = false;
            photoTaken2 = false;
        }


        private void nextButton1_Click(object sender, EventArgs e)
        {
            images.Add(picBox1.Image);
            if(MyDevices.Count > camera1 + 2 )
            {
                camera1 = camera1 + 2;
                startCamera1(sender, e);
            }
            else
            {
                nextButton1.Visible = false;
                picBox1.Image = Resources.ready;
                CloseWebCam();

                if (images.Count < 3 )
                {
                    images.Add(picBox1.Image);
                    images.Add(picBox2.Image);
                }
            }
        }

        private void nextButton2_Click(object sender, EventArgs e)
        {
            images.Add(picBox2.Image);
            if (MyDevices.Count > camera2 + 2)
            {
                camera2 = camera2 + 2;
                startCamera2(sender, e);
            }
            else {
                nextButton2.Visible = false;
                picBox2.Image = Resources.ready;
                CloseWebCam2();
                if (MyDevices.Count < 3)
                {
                    images.Add(picBox2.Image);
                }
            }

        }
    }
}
