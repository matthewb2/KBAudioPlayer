using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KBAudioPlayer
{
    public partial class Form2 : Form
    {
        private EqualizerBand[] bs;
        private Equalizer eq;
        private IWavePlayer wo;

        public Form2()
        {
            InitializeComponent();
            //
            trackBar1.ValueChanged += new System.EventHandler(trackBar1_ValueChanged);
            trackBar2.ValueChanged += new System.EventHandler(trackBar2_ValueChanged);
            trackBar3.ValueChanged += new System.EventHandler(trackBar3_ValueChanged);
            trackBar4.ValueChanged += new System.EventHandler(trackBar4_ValueChanged);
            trackBar5.ValueChanged += new System.EventHandler(trackBar5_ValueChanged);
            trackBar6.ValueChanged += new System.EventHandler(trackBar6_ValueChanged);
            trackBar7.ValueChanged += new System.EventHandler(trackBar7_ValueChanged);
            trackBar8.ValueChanged += new System.EventHandler(trackBar8_ValueChanged);
            preAmpTrackBar.ValueChanged += new System.EventHandler(preAmpTrackBar_ValueChanged);
            //
            comboBox1.Items.Add("평면");
            comboBox1.Items.Add("R&B");
            comboBox1.Items.Add("락");
            comboBox1.Items.Add("재즈");
            comboBox1.Items.Add("클래식");
            comboBox1.Items.Add("팝");

            this.comboBox1.SelectedIndexChanged +=
            new System.EventHandler(comboBox1_SelectedIndexChanged);


        }

        private void updateEQ()
        {
            if (bs != null)
            {
                bs[0] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 30, Gain = trackBar1.Value };
                bs[1] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 60, Gain = trackBar2.Value };
                bs[2] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 125, Gain = trackBar3.Value };
                bs[3] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 250, Gain = trackBar4.Value };
                bs[4] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 500, Gain = trackBar5.Value };
                bs[5] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 1000, Gain = trackBar6.Value };
                bs[6] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 2000, Gain = trackBar7.Value };
                bs[7] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 4000, Gain = trackBar8.Value };
                bs[8] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 8000, Gain = trackBar9.Value };
                bs[9] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 16000, Gain = trackBar10.Value };
            }

            if (eq != null)
                eq.Update();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (comboBox1.SelectedIndex == 0)
            {
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;
                trackBar4.Value = 0;
                trackBar5.Value = 0;
                trackBar6.Value = 0;
                trackBar7.Value = 0;
                trackBar8.Value = 0;
                trackBar9.Value = 0;
                trackBar10.Value = 0;
                //
                updateEQ();
            } else if (comboBox1.SelectedIndex == 1)
            {
                trackBar1.Value = 0;
                trackBar2.Value = 5;
                trackBar3.Value = 9;
                trackBar4.Value = 5;
                trackBar5.Value = 0;
                trackBar6.Value = 5;
                trackBar7.Value = 7;
                trackBar8.Value = 10;
                trackBar9.Value = 8;
                trackBar10.Value = 7;
                //

                updateEQ();
            }
            else if (comboBox1.SelectedIndex == 2) //락
            {
                trackBar1.Value = 15;
                trackBar2.Value = 13;
                trackBar3.Value = 10;
                trackBar4.Value = 5;
                trackBar5.Value = 0;
                trackBar6.Value = 5;
                trackBar7.Value = 10;
                trackBar8.Value = 12;
                trackBar9.Value = 15;
                trackBar10.Value = 15;

                updateEQ();
            }
            else if (comboBox1.SelectedIndex == 3)  //재즈
            {
                trackBar1.Value = 10;
                trackBar2.Value = 7;
                trackBar3.Value = 0;
                trackBar4.Value = 5;
                trackBar5.Value = 7;
                trackBar6.Value = 6;
                trackBar7.Value = 8;
                trackBar8.Value = 12;
                trackBar9.Value = 12;
                trackBar10.Value = 12;

                updateEQ();
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                trackBar1.Value = 7;
                trackBar2.Value = 7;
                trackBar3.Value = 7;
                trackBar4.Value = 3;
                trackBar5.Value = 3;
                trackBar6.Value = 3;
                trackBar7.Value = 0;
                trackBar8.Value = 3;
                trackBar9.Value = 7;
                trackBar10.Value = 7;

                updateEQ();
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                trackBar1.Value = 0;
                trackBar2.Value = 5;
                trackBar3.Value = 7;
                trackBar4.Value = 12;
                trackBar5.Value = 10;
                trackBar6.Value = 5;
                trackBar7.Value = 0;
                trackBar8.Value = 5;
                trackBar9.Value = 7;
                trackBar10.Value = 12;

                updateEQ();
            }


        }

        public void setEqualizer(EqualizerBand[] bands, Equalizer equalizer, IWavePlayer waveOut)
        {
            bs = bands;
            eq = equalizer;
            wo = waveOut;

            foreach (EqualizerBand tmp in this.bs)
            {

              //  Debug.WriteLine(tmp.Bandwidth.ToString() + " " + tmp.Gain);
            }
            
        }

        private void trackBar1_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs !=null)
                bs[0] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 30, Gain = trackBar1.Value };
            
            if (eq != null)
                eq.Update();
        }
        private void trackBar2_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs != null)
                bs[1] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 60, Gain = trackBar2.Value };
            
            if (eq != null)
                eq.Update();
        }
        private void trackBar3_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs != null)
                bs[2] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 125, Gain = trackBar3.Value };
            
            if (eq != null)
                eq.Update();
        }
        private void trackBar4_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs != null)
                bs[3] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 250, Gain = trackBar4.Value };
            
            if (eq != null)
                eq.Update();
        }
        private void trackBar5_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs != null)
                bs[4] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 500, Gain = trackBar5.Value };
            
            if (eq != null)
                eq.Update();
        }
        private void trackBar6_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs != null)
                bs[5] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 1000, Gain = trackBar6.Value };
            
            if (eq != null)
                eq.Update();
        }
        private void trackBar7_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs != null)
                bs[6] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 2000, Gain = trackBar7.Value };
            
            if (eq != null)
                eq.Update();
        }
        private void trackBar8_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs != null)
                bs[7] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 4000, Gain = trackBar8.Value };
            
            if (eq != null)
                eq.Update();
        }
        private void trackBar9_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs != null)
                bs[8] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 4000, Gain = trackBar9.Value };

            if (eq != null)
                eq.Update();
        }
        private void trackBar10_ValueChanged(object sender, System.EventArgs e)
        {
            if (bs != null)
                bs[9] = new EqualizerBand { Bandwidth = 0.8f, Frequency = 4000, Gain = trackBar10.Value };

            if (eq != null)
                eq.Update();
        }
        private void preAmpTrackBar_ValueChanged(object sender, System.EventArgs e)
        {
            wo.Volume = (float)preAmpTrackBar.Value / 100f;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
