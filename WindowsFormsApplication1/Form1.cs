using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        float[] force = new float[4];
        float[] force0 = new float[4];
        bool isUpdate = false;
        bool isFirst = true;
        string data;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (!serialPort1.IsOpen) return;
            try
            {
                data += serialPort1.ReadExisting();
                var lines = data.Split('\r');
                int index = data.LastIndexOf("\n");
                if (index != -1)
                {
                    data = data.Substring(index + 1);
                }
                var values = lines[0].Split(',');
                if (values.Count() != 4) return;
                for(var i = 0; i < 4; i ++)
                {
                    force[i] = float.Parse(values[i]);
                }
                if (isFirst)
                {
                    for (var i = 0; i < 4; i++) force0[i] = force[i];
                    isFirst = false;
                }
                isUpdate = true;
            }
            catch (Exception) { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!isUpdate) return;
            Bitmap canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(canvas);

            Pen p = new Pen(Color.Black, 2);

            float f0 = force[0] - force0[0];
            float f1 = force[1] - force0[1];
            float f2 = force[2] - force0[2];
            float f3 = force[3] - force0[3];
            float fa = f0 + f1 + f2 + f3;

            float gx = (-f0 + f1 + f2 - f3) / fa;
            float gy = ( f0 + f1 - f2 - f3) / fa;

            if (fa > 0.001)
            {
                int px = (int)((gx + 2) * pictureBox1.Width / 4);
                int py = (int)((gy + 2) * pictureBox1.Height / 4);
                g.DrawEllipse(p, px, py, 20, 20);
            }

            p.Dispose();
            g.Dispose();

            pictureBox1.Image = canvas;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.Open();
        }
    }
}
