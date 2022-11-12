using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using OrthoAnalisis;

namespace Proyecto
{
    public partial class Form1 : Form
    {
        private bool ExistDevices;
        private FilterInfoCollection MyDevices;
        private VideoCaptureDevice MyWebCam;
        private VideoCaptureDevice MyWebCam2;

        private VideoCaptureDevice MyWebCam3;
        private VideoCaptureDevice MyWebCam4;
        int camera1 = 10;
        int camera2 = 10;
        int camera3 = 10;
        int camera4 = 10;
        Recorder rec;


        List<System.Drawing.Image> images = new List<System.Drawing.Image>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDevices();
        }

        public void LoadDevices()
        {
            MyDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (MyDevices.Count == 1)
            {
                ExistDevices = true;
                camera1 = 1;
                camera2 = 1;
            }
            else if (MyDevices.Count > 1) {
                ExistDevices = true;
                camera1 = 1;
                camera2 = 2;
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

        private void CloseWebCam3()
        {
            if (MyWebCam3 != null && MyWebCam3.IsRunning)
            {
                MyWebCam3.SignalToStop();
                MyWebCam3 = null;
            }
        }

        private void CloseWebCam4()
        {
            if (MyWebCam4 != null && MyWebCam4.IsRunning)
            {
                MyWebCam4.SignalToStop();
                MyWebCam4 = null;
            }
        }
        private void resetFields()
        {
            txtnombres.ResetText();
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

        private void Capture3(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = new Bitmap((Bitmap)eventArgs.Frame.Clone(), new Size(picBox3.Width, picBox3.Height));
            picBox3.Image = image;
        }
        private void Capture4(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = new Bitmap((Bitmap)eventArgs.Frame.Clone(), new Size(picBox4.Width, picBox4.Height));
            picBox4.Image = image;
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseWebCam();
            CloseWebCam2();
            CloseWebCam3();
            CloseWebCam4();
        }

        private void startCamera1(object sender, EventArgs e)
        {
            if (ExistDevices)
            {
                CloseWebCam();
                string deviceName = MyDevices[0].MonikerString;
                MyWebCam = new VideoCaptureDevice(deviceName);
                MyWebCam.NewFrame += new NewFrameEventHandler(Capture);
                MyWebCam.Start();
            }
        }
        private void startCamera2(object sender, EventArgs e)
        {
            if (ExistDevices)
            {
                CloseWebCam2();
                string deviceName = MyDevices[0].MonikerString;
                MyWebCam2 = new VideoCaptureDevice(deviceName);
                MyWebCam2.NewFrame += new NewFrameEventHandler(Capture2);
                MyWebCam2.Start();
                btnStartRecord.Visible = true;
            }
        }
        private void startCamera3(object sender, EventArgs e)
        {
            if (ExistDevices)
            {
                CloseWebCam3();
                string deviceName = MyDevices[1].MonikerString;
                MyWebCam3 = new VideoCaptureDevice(deviceName);
                MyWebCam3.NewFrame += new NewFrameEventHandler(Capture3);
                MyWebCam3.Start();
            }
        }
        private void startCamera4(object sender, EventArgs e)
        {
            if (ExistDevices)
            {
                CloseWebCam4();
                string deviceName = MyDevices[1].MonikerString;
                MyWebCam4 = new VideoCaptureDevice(deviceName);
                MyWebCam4.NewFrame += new NewFrameEventHandler(Capture4);
                MyWebCam4.Start();
                btnStartRecord.Visible = true;
            }
        }


        private void startRecord_Click(object sender, EventArgs e)
        {
            string filename = string.Format("{0}-{1}.avi", txtnombres.Text.Replace(" ", ""), DateTime.Now.ToString("dd-MM-yyyyHH-mm"));
            rec = new Recorder(new RecorderParams(filename, 10, SharpAvi.CodecIds.MotionJpeg, 70));
            
        }


        private void stopRecord_Click(object sender, EventArgs e)
        {
            rec.Dispose();
            btnStopCameras_Click(sender, e);
        }

        private void btnStopCameras_Click(object sender, EventArgs e)
        {
            CloseWebCam();
            CloseWebCam2();
            CloseWebCam3();
            CloseWebCam4();
            resetFields();
        }

        private void btnStartCameras_Click(object sender, EventArgs e)
        {
            btnStopCameras_Click(sender, e);
            startCamera1(sender, e);
            startCamera2(sender, e);
            startCamera3(sender, e);
            startCamera4(sender, e);
        }
    }
}
