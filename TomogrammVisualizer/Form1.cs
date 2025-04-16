using System;
using System.Windows.Forms;

namespace TomogrammVisualizer
{
    public partial class Form1: Form
    {
        private Bin bin;
        private View view;
        private bool loaded = false;
        private int currentLayer;
        private DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        private int FrameCount;
        private bool needReload = false;

        private enum Mode
        {
            Quads_H,
            Quads_V,
            QuadStrip_H,
            QuadStrip_V,
            Texture
        };
        private Mode mode = Mode.Quads_H;
        
        private int min;
        private int width;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
            bin = new Bin();
            view = new View();
            currentLayer = 0;
            min = trackBar2.Value;
            width = trackBar3.Value;
            radioButton1.Checked = true;
        }
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0}, frame={1})", FrameCount, currentLayer);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
                string str = op.FileName;
                bin.readBIN(str);
                trackBar1.Maximum = Bin.Z - 1;
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }
        
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                switch (mode)
                {
                    case Mode.Quads_H:
                        view.DrawQuads(currentLayer, min, width);
                        glControl1.SwapBuffers();
                        break;
                    case Mode.Quads_V:
                        view.DrawQuadsVertical(currentLayer, min, width);
                        glControl1.SwapBuffers();
                        break;
                    case Mode.Texture:
                        if (needReload)
                        {
                            view.generateTextureImage(currentLayer, min, width);
                            view.Load2DTexture();
                            needReload = false;
                        }
                        view.DrawTexture();
                        glControl1.SwapBuffers();
                        break;
                    case Mode.QuadStrip_H:
                        view.DrawQuadStrip(currentLayer, min, width);
                        glControl1.SwapBuffers();
                        break;
                    case Mode.QuadStrip_V:
                        view.DrawQuadStripVertical(currentLayer, min, width);
                        glControl1.SwapBuffers();
                        break;
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            min = trackBar2.Value;
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            width = trackBar3.Value;
            needReload = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            mode = Mode.Quads_H;
            trackBar1.Maximum = Bin.Z - 1;
            trackBar1.Value = trackBar1.Minimum;
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            mode = Mode.QuadStrip_H;
            trackBar1.Maximum = Bin.Z - 1;
            trackBar1.Value = trackBar1.Minimum;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            mode = Mode.Texture;
            trackBar1.Maximum = Bin.Z - 1;
            trackBar1.Value = trackBar1.Minimum;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            mode = Mode.Quads_V;
            trackBar1.Maximum = Bin.Y - 1;
            trackBar1.Value = trackBar1.Minimum;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            mode = Mode.QuadStrip_V;
            trackBar1.Maximum = Bin.Y - 1;
            trackBar1.Value = trackBar1.Minimum;
        }
    }
}
