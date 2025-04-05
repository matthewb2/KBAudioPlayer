using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;
using NAudio.Wave.SampleProviders;
using System.Globalization;
using CSCore;
using CSCore.SoundOut;
using CSCore.Codecs;
using Deveck.Ui.Controls.Scrollbar;
using Newtonsoft.Json;



namespace KBAudioPlayer
{
    
    public partial class Form1 : Form
    {

        EventWaitHandle _ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
        //
        private List<string> playlist = new List<string>();
        private List<string> playlist2 = new List<string>();

        private AudioFileReader reader;
        private IWaveSource soundSource;

        private IWavePlayer waveOut = new WaveOutEvent(); // or WaveOutEvent()
        private ISoundOut soundOut;
        //
        private Equalizer equalizer;
        private EqualizerBand[] bands;
        //
        private Boolean isPaused = false;
        private Boolean isPlay = false;
        private int timeElapsed = 0;
        private TimeSpan duration;
        //
        private string current = null;
        private int currentNum = 0;

        private int NumLEDS = 20;
        //
        private RealTimePlayback _playback;
        private static string path = null;
        // The current value.
        private float SliderValue = 0.0f;
        // The minimum and maximum allowed values.
        private const float MinimumValue = 0.0f;
        private const float MaximumValue = 1.0f;
        // Move the needle to this position.
        private bool MouseIsDown = false;
        private Form2 equalizerForm;
        private Point MouseDownLocation;
        private Boolean isTitle = true;
        private Boolean defaultskin = true;
        //
        private int targetIndex = -1;
        private int item_index = -1;
        private string item_current = null;

        //
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int TCM_SETMINTABWIDTH = 0x1300 + 49;
        //
        const int WM_NCLBUTTONDOWN = 0x00A1;
        //
        ListView newlstSongs;

        
        public Form1()
        {
            InitializeComponent();
            //current directory
            path = Directory.GetCurrentDirectory();
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.Load += Form1_Load;
            this.Closing += Form1_Closing;
            this.Resize += Form1_Resize;
            this.MouseClick += Form1_MouseClick;
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;

            tabControl1.HandleCreated += new EventHandler(tabControl1_HandleCreated);

            tabControl1.MouseClick += new MouseEventHandler(tabControl1_MouseClick);
            tabControl1.MouseDown += new MouseEventHandler(tabControl1_MouseDown);

            volumeSlider.ValueChanged += new System.EventHandler(volumeSlider_ValueChanged);
            volumeSlider.MouseUp += new MouseEventHandler(volumeSlider_MouseUp);
            volumeSlider.MouseWheel += new MouseEventHandler(volumeSlider_MouseWheel);
            volumeSlider.MouseHover += new System.EventHandler(volumeSlider_MouseHover);

            lstSongs.MouseDoubleClick += new MouseEventHandler(lstSongs_MouseDoubleClick);
            lstSongs.MouseClick += new MouseEventHandler(lstSongs_MouseClick);
            //lstSongs.MouseEnter += new System.EventHandler(lstSongs_MouseEnter);
            lstSongs.DragDrop += new System.Windows.Forms.DragEventHandler(lstSongs_DragDrop);
            lstSongs.DragEnter += new System.Windows.Forms.DragEventHandler(lstSongs_DragEnter);
            lstSongs.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(lstSongs_ItemDrag);
            lstSongs.DragOver += new DragEventHandler(lstSongs_DragOver);



            // volume spectrum
            this.peakMeterCtrl1.SetRange(40, 70, 100);
            this.peakMeterCtrl1.Start(33);
            //
            lstSongs.Columns.Add("순번");
            lstSongs.Columns.Add("곡명");
            lstSongs.Columns.Add("길이");
            lstSongs.Columns[0].Width = 25;
            lstSongs.Columns[1].Width = 183;
            lstSongs.Columns[2].Width = 40;
            
            //
            bands = new EqualizerBand[]
                   {
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 30, Gain = 0},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 60, Gain = 0},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 125, Gain = 0},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 250, Gain = 0},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 500, Gain = 0},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 1000, Gain = 0},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 2000, Gain = 0},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 4000, Gain = 0},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 8000, Gain = 0},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 16000, Gain = 0},
                   };


            equalizerForm = new Form2();
            //
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;
            this.Location = new Point(area.Width-this.Size.Width-200, area.Height - this.ClientSize.Height-200);
            //
            ToolTip addtoolTip = new ToolTip();

            addtoolTip.AutoPopDelay = 5000;
            addtoolTip.InitialDelay = 1000;
            addtoolTip.ReshowDelay = 500;
            addtoolTip.ShowAlways = true;
            addtoolTip.IsBalloon = true;

            addtoolTip.SetToolTip(this.button3, "추가");
            //
            tabPage1.Text = "선곡1";
            tabPage2.Text = "+";
            //
            SetHeight(lstSongs, 30);
            //

            lstSongs.VScrollbar = new ScrollbarCollector(scrollbar1);
            //
            //lstSongs.Scrollable = false;
            HideHorizontalScrollBar();


        }

        private void lstSongs_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        [System.Runtime.InteropServices.DllImport("user32", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]

        private static extern bool ShowScrollBar(IntPtr hwnd, int wBar, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool bShow);
        
        private void HideHorizontalScrollBar()
        {
            ShowScrollBar(lstSongs.Handle, 0, false);

        }


        private void SetHeight(ListView listView, int height)
        {
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, height);
            listView.SmallImageList = imgList;
        }



        private void lstSongs_ItemDrag(object sender, ItemDragEventArgs e)
        {
            
            item_index = lstSongs.SelectedIndices[0];
            item_current = playlist[item_index];
            lstSongs.DoDragDrop(e.Item, DragDropEffects.Move);
         
        }


        private void lstSongs_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;

        }

        private void lstSongs_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse pointer.
            Point targetPoint = lstSongs.PointToClient(new Point(e.X, e.Y));

            // Retrieve the index of the item closest to the mouse pointer.
            targetIndex = lstSongs.InsertionMark.NearestIndex(targetPoint);
            
        }

        private void lstSongs_DragDrop(object sender, DragEventArgs e)
        {
            //Debug.WriteLine(targetIndex);
            playlist.RemoveAt(item_index);
            playlist.Insert(targetIndex, item_current);
            UpdatePlaylist();
        }

        private void lstSongs_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
              // MessageBox.Show("kkk");
            }

        }

        private void volumeSlider_MouseHover(object sender, EventArgs e)
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(volumeSlider, volumeSlider.Value.ToString());
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;
            tabControl1.Size = new Size(control.Size.Width-20, control.Size.Height - (450 - 208));
            btnToTop.Location = new Point(4, control.Size.Height - (470 - 389));
            btnUp.Location = new Point(40, control.Size.Height - (470 - 389));
            btnDown.Location = new Point(76, control.Size.Height - (470 - 389));
            btnToBottom.Location = new Point(112, control.Size.Height - (470 - 389));
            btnDelete.Location = new Point(146, control.Size.Height - (470 - 389));
            button3.Location = new Point(242, control.Size.Height - (470 - 389));
            //
            scrollbar1.Size = new Size(scrollbar1.Size.Width, lstSongs.Size.Height);

        }
        // set focus to volume slider when click form
        void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            volumeSlider.Focus();
        }
        

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            //
            if (m.Msg == WM_NCLBUTTONDOWN)
            {
                this.Cursor = new Cursor(Cursor.Current.Handle);
                volumeSlider.Focus();
            }
        }



        void lstSongs_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            playTime.Start();
            this._playback = new RealTimePlayback();
            this._playback.Start();

            var item = lstSongs.SelectedItems[0];
            current = item.SubItems[1].Text;
            Debug.WriteLine(current);
            if (current != null)
            {
                label1.Text = current;
                //
                int index = lstSongs.SelectedIndices[0];
                if (index >= 0)
                {
                    waveOut.Stop();
                    PlayAudio(playlist[index]);
                    duration = reader.TotalTime;
                    label3.Text = duration.ToString(@"mm\:ss");
                }
            }
            // change play button icon to pause icon
            if (isPlay)
            {
                button1.Image = new Bitmap(@"res\control-pause-icon.png");
                
            }
            //button1.Image = new Bitmap(@"res\control-pause-icon.png");


        }

        private int getIndexFromSelect(string current)
        {
            // add escape character for regular match
            for (int i = 0; i <= playlist.Count-1; i++)
            {
                string name = playlist[i];
                name = Path.GetFileName(name);
                
                FileInfo fi = new FileInfo(name);
                string ext = fi.Extension;
                name = name.Replace(ext, "");
                if (string.Equals(current, name))
                {
                    return i;
                }

            }
            return -1;
        }

        private void picSlider_MouseDown(object sender, MouseEventArgs e)
        {
            MouseIsDown = true;
            SliderValue = XtoValue(e.X);

            // Redraw to show the new value.
            picSlider.Refresh();
            
            if (reader != null)
            {
                // reposition of reader
                double tmp = SliderValue * reader.TotalTime.TotalMilliseconds;
                reader.CurrentTime = TimeSpan.FromMilliseconds(tmp);
            }

        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == this.tabControl1.TabCount - 1)
                e.Cancel = true;
        }
        
        private void tabControl1_HandleCreated(object sender, EventArgs e)
        {
            //MessageBox.Show("You are in the Control.HandleCreated event.");
            SendMessage(this.tabControl1.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
            //SendMessage(this.tabControl1.Handle, 10, IntPtr.Zero, (IntPtr)16);
        }




        private void picSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MouseIsDown) return;
            SetValue(XtoValue(e.X));
        }
        private void picSlider_MouseUp(object sender, MouseEventArgs e)
        {
            MouseIsDown = false;
        }

        // Convert an X coordinate to a value.
        private float XtoValue(int x)
        {
            return MinimumValue + (MaximumValue - MinimumValue) *
                x / (float)(picSlider.ClientSize.Width - 1);
        }

        // Convert value to an X coordinate.
        private float ValueToX(float value)
        {
            return (picSlider.ClientSize.Width - 1) *
                (value - MinimumValue) / (float)(MaximumValue - MinimumValue);
        }

        // Draw the needle.
        private void picSlider_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the needle's X coordinate.
            float x = ValueToX(SliderValue);
            int hgt = picSlider.ClientSize.Height;
            Color rob = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            // Draw it.
            if (x > 0)
            {
                e.Graphics.FillRectangle(new SolidBrush(rob), 0, 0, x, hgt);
                using (Pen pen = new Pen(rob, 3))
                {
                    e.Graphics.DrawLine(pen, x, 0, x, picSlider.ClientSize.Height);
                }
            }
        }
        // Set the slider's value. If the value has changed,
        // display the value tooltip.
        private void SetValue(float value)
        {
            // Make sure the new value is within bounds.
            if (value < MinimumValue) value = MinimumValue;
            if (value > MaximumValue) value = MaximumValue;
            // See if the value has changed.
            if (SliderValue == value) return;
            // Save the new value.
            SliderValue = value;
            // Redraw to show the new value.
            picSlider.Refresh();
           
        }

        private void playTime_Tick(object sender, EventArgs e)
        {
            FillMeterData();
            //
            int index2;
            string cur;
            if (tabControl1.SelectedIndex == 1)
            {
                index2 = newlstSongs.SelectedIndices[0];
                cur = playlist2[index2];
            }
            else
            {
                index2 = lstSongs.SelectedIndices[0];
                cur = playlist[index2];
            }
            //string cur = playlist[index2];
            cur = Path.GetFileName(cur);
            FileInfo fi = new FileInfo(cur);
            label1.Text = cur.Replace(fi.Extension, "");
            if (fi.Extension == ".flac")
            {

                TimeSpan tmp2 = soundSource.GetPosition();
                //Debug.WriteLine(tmp2.ToString(@"mm\:ss"));
                TimeSpan totalTime = soundSource.GetLength();
                label2.Text = tmp2.ToString(@"mm\:ss");
                //move progress
                float elapse = (float)(tmp2.TotalSeconds / totalTime.TotalSeconds);
                SetValue(elapse);

            }
            else
            {
                if (reader != null)
                {
                    TimeSpan tmp = reader.CurrentTime;
                    label2.Text = tmp.ToString(@"mm\:ss");
                    //move progress
                    float elapse = (float)(tmp.TotalSeconds / reader.TotalTime.TotalSeconds);
                    SetValue(elapse);
                    //
                    if (reader.CurrentTime == reader.TotalTime)
                    {
                        int index = getIndexFromSelect(current);
                        //
                        if (tabControl1.SelectedIndex == 1)
                        {
                            if (index <= playlist2.Count - 1)
                            {
                                newlstSongs.Items[index].Selected = false;
                                newlstSongs.Items[index + 1].Selected = true;
                                waveOut.Stop();
                                //
                                PlayAudio(playlist2[index + 1]);
                                // update current song
                                current = playlist2[index + 1];

                            }
                            else
                            {
                                waveOut.Stop();
                                if (reader != null) playTime.Stop();
                                if (_playback != null)
                                {
                                    if (_playback.IsPlaying()) _playback.Stop();
                                }
                            }
                        }
                        else
                        {
                            if (index <= playlist.Count - 1 && index>=0)
                            {
                                //Debug.WriteLine(index);
                                lstSongs.Items[index].Selected = false;
                                lstSongs.Items[index + 1].Selected = true;
                                waveOut.Stop();
                                //
                                PlayAudio(playlist[index + 1]);
                                // update current song
                                current = playlist[index + 1];

                            }
                            else
                            {
                                waveOut.Stop();
                                if (reader != null) playTime.Stop();
                                if (_playback != null)
                                {
                                    if (_playback.IsPlaying()) _playback.Stop();
                                }
                            }
                        }

                    }
                }
            }
        }


        private void volumeSlider_MouseUp(object sender, MouseEventArgs e)
        {
            
        }


        private void PlayAudio(string name)
        {
            if (File.Exists(name))
            {
                string result = Path.GetFileName(name);
                // delete mp3 from filename
                result = result.Replace(".mp3", "");
                FileInfo fi = new FileInfo(name);
              
                if (isPaused == true)
                {
                    //waveOut.Play();
					playTime.Start();
                        reader = new AudioFileReader(name);
                        equalizer = new Equalizer(reader, bands);
                        //
                        equalizerForm.setEqualizer(bands, equalizer, waveOut);

                        waveOut.Stop();
                        waveOut.Init(equalizer);
                        waveOut.Play();
                        //
                        label1.Text = result;
                        duration = reader.TotalTime;
                        label3.Text = duration.ToString(@"mm\:ss");
                    isPlay = true;
                }
                else
                {
                    if (_playback.IsPlaying()) waveOut.Stop();
                    //
                    if (fi.Extension == ".mp3")
                    {
                        playTime.Start();
                        reader = new AudioFileReader(name);
                        equalizer = new Equalizer(reader, bands);
                        //
                        equalizerForm.setEqualizer(bands, equalizer, waveOut);

                        waveOut.Stop();
                        waveOut.Init(equalizer);
                        waveOut.Play();
                        //
                        label1.Text = result;
                        duration = reader.TotalTime;
                        label3.Text = duration.ToString(@"mm\:ss");
                        isPlay = true;

                    }
                    else if (fi.Extension == ".flac")
                    {
                        playTime.Start();
                        soundSource = CodecFactory.Instance.GetCodec(name);
                        soundOut = new CSCore.SoundOut.DirectSoundOut();
                        soundOut.Initialize(soundSource);
                        soundOut.Volume = (float)volumeSlider.Value / 100f;
                        soundOut.Play();
                        TimeSpan totalTime = soundSource.GetLength();
                        
                        label1.Text = result.Replace(".flac", "");
                        duration =  soundSource.GetLength();
                        label3.Text = duration.ToString(@"mm\:ss");

                    }

                }
                waveOut.Volume = (float)(volumeSlider.Value / 100);
                //
                var file = TagLib.File.Create(name);
                if (file.Tag.Pictures.Length >= 1)
                {
                    var bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                    pictureBox2.Image = Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(65, 65, null, IntPtr.Zero);
                }
                else { pictureBox2.Image = Image.FromFile(@"res\scaled2.png"); }

                popupNotifier1.TitleText = "재생";
                popupNotifier1.ContentText = result;
                popupNotifier1.ShowCloseButton = true;
                popupNotifier1.ShowOptionsButton = true;
                popupNotifier1.ShowGrip = true;
                popupNotifier1.ContentPadding = new Padding(5);
                popupNotifier1.IsRightToLeft = false;
                popupNotifier1.Popup();
            } else
                {
                    if (_playback.IsPlaying()) _playback.Stop();
                    playTime.Stop();

                }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Creating and setting the  
            /*
            playlist.Add(@"D:\윤하 - 그 거리.mp3");
            playlist.Add(@"D:\이승환 - 잘못-2001년 12월 15일-.mp3");
            playlist.Add(@"F:\-펑티모- 한국에서도 많은 사랑을 받은노래 《소행운 小幸?》 영화 나의소녀시대OST.mp3");
            */
            System.IO.StreamReader file = new System.IO.StreamReader(path + @"\playlist.dat", Encoding.UTF8);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                //check file exists
                if (File.Exists(line))
                    playlist.Add(line);
            }
            file.Close();
            //
            UpdatePlaylist();

            string filePath = "pref.json";

            // JSON 파일 읽기
            string jsonData = File.ReadAllText(filePath);

            // JSON 데이터 파싱
            Person person = JsonConvert.DeserializeObject<Person>(jsonData);

            Debug.WriteLine(person.Volume);
            volumeSlider.Value = person.Volume;
            /*
            using (XmlReader rd = XmlReader.Create(path + @"\pref.xml"))
            {
                //Debug.WriteLine(path);
                while (rd.Read())
                {
                    if (rd.IsStartElement())
                    {
                        if (rd.Name == "Volume")
                        {
                            float tmp = float.Parse(rd.ReadString(), CultureInfo.InvariantCulture.NumberFormat);
                            Debug.WriteLine(tmp);
                            volumeSlider.Value = (int)(tmp* 100f);
                        }

                        if (rd.Name == "Height")
                        {
                            
                            float tmp = float.Parse(rd.ReadString(), CultureInfo.InvariantCulture.NumberFormat);
                            this.ClientSize = new System.Drawing.Size(284, (int)tmp);
                        }

                    }
                }
            }
            */

        }
        private void setVolume(int volume)
        {
            volumeSlider.Value = volume;

        }

        // Enable and disable the appropriate buttons.
        private void lstSongs_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
            var item = lstSongs.SelectedItems[0];
            string select = item.SubItems[1].Text;

        }

        // Move the selected item to the top of the list (index 0).
        private void btnToTop_Click(object sender, EventArgs e)
        {
            var item = lstSongs.SelectedItems[0];
            current = item.SubItems[1].Text;
            int index = getIndexFromSelect(current);
            //
            
            string tmp = playlist[index];
            playlist.RemoveAt(index);
            playlist.Insert(0, tmp);
            //
            UpdatePlaylist();
            lstSongs.Items[0].Selected = true;

        }

        // Move the selected item up one position.
        private void btnUp_Click(object sender, EventArgs e)
        {
            var item = lstSongs.SelectedItems[0];
            current = item.SubItems[1].Text;
            int index = getIndexFromSelect(current);
            
            string[] items = { };

            if (index > 0)
            {
                string tmp = playlist[index];
                playlist.RemoveAt(index);
                playlist.Insert(index - 1, tmp);
                //
                UpdatePlaylist();
                lstSongs.Items[index -1].Selected = true;

            }
        }

        // Move the selected item down one position.
        private void btnDown_Click(object sender, EventArgs e)
        {
            var item = lstSongs.SelectedItems[0];
            current = item.SubItems[1].Text;
            int index = getIndexFromSelect(current);

            if (index < lstSongs.Items.Count - 1)
            {
                //
                string tmp = playlist[index];
                playlist.RemoveAt(index);
                playlist.Insert(index + 1, tmp);
                //
                UpdatePlaylist();
                lstSongs.Items[index + 1].Selected = true;

            }
        }

        // Move the selected item to the end of the list.
        private void btnToBottom_Click(object sender, EventArgs e)
        {
            var item = lstSongs.SelectedItems[0];
            current = item.SubItems[1].Text;
            int index = getIndexFromSelect(current);
            string tmp = playlist[index];
            playlist.RemoveAt(index);
            playlist.Add(tmp);
            UpdatePlaylist();
            lstSongs.Items[playlist.Count-1].Selected = true;


        }

        private static int[] NormalizeData(IEnumerable<float> data, int min, int max)
        {
            double dataMax = data.Max();
            double dataMin = data.Min();
            double range = dataMax - dataMin;

            return data
                .Select(d => (d - dataMin) / range)
                .Select(n => (int)((1 - n) * min + n * max))
                .ToArray();
        }

        private void FillMeterData()
        {
            int[] meters1 = new int[NumLEDS];
            Random rand = new Random();
            for (int i = 0; i < meters1.Length; i++)
            {
                meters1[i] = rand.Next(0, 100);
            }
            // fill meter data
            float[] tmpdata1 = new float[NumLEDS]; ;

            if (_playback.GetFFTData() != null)
            {
                tmpdata1 = _playback.GetFFTData();
                int[] tmpdata2 = new int[NumLEDS];
                float[] tmpdata3 = new float[NumLEDS];
                int step = 0;
                for (int i = 0; i < NumLEDS; i++)
                {
                    step += (int)(2048 / 20);
                    tmpdata3[i] = tmpdata1[step];
                }
                tmpdata2 = NormalizeData(tmpdata3, 0, 100);

                this.peakMeterCtrl1.SetData(tmpdata2, 0, tmpdata2.Length);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string tmp;
            int index;
            //get current active tab
            if (tabControl1.SelectedIndex == 1)
            {
                var item = newlstSongs.SelectedItems[0];
                current = item.SubItems[1].Text;
                index = getIndexFromSelect(current);
                currentNum = index;

                tmp = Path.GetFileName(playlist2[index]);
                //
            }
            else
            {
                var item = lstSongs.SelectedItems[0];
                current = item.SubItems[1].Text;
                index = getIndexFromSelect(current);
                currentNum = index;

                tmp = Path.GetFileName(playlist[index]);
            }
                //
                FileInfo fi = new FileInfo(tmp);
                //Debug.WriteLine(fi.Extension);


                if (isPlay)
                {
                    waveOut.Pause();
                    isPaused = true;
                    isPlay = false;
                    button1.Image = new Bitmap(@"res\control-right-icon.png");
                    //specturm stop
                    playTime.Stop();

                }
                else
                {
                    if (isPaused)
                    {
                        playTime.Start();
                        if (fi.Extension == ".mp3")
                        {
                            waveOut.Play();
                        }
                        isPaused = false;

                    }
                    else
                    {
                        if (this._playback == null) this._playback = new RealTimePlayback();
                        if (!_playback.IsPlaying()) this._playback.Start();

                        PlayAudio(playlist[index]);
                    }
                    isPlay = true;
                    button1.Image = new Bitmap(@"res\control-pause-icon.png");
                }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isPlay = false;
            button1.Image = new Bitmap(@"res\control-right-icon.png");
            
            //fade out to stop
            timer1.Interval = 100;
            timer1.Start();
            //restore cover image
            pictureBox2.Image = new Bitmap(@"res\Music-Folder-icon.png");
            SliderValue = 0;
            picSlider.Refresh();
            label2.Text = "00:00";
            if (reader != null)
                this._playback.Stop();
            //specturm stop
            playTime.Stop();
            //




        }

        private int getPlayIndex(string current)
        {
            //Debug.WriteLine(current);

            current = Path.GetFileName(current);
            FileInfo fi2 = new FileInfo(current);
            if (fi2.Extension.Length > 0)
            {
                current = current.Replace(fi2.Extension, "");
            }

            for (int i = 0; i <= playlist.Count-1; i++)
            {
                string name = playlist[i];
                name = Path.GetFileName(name);

                FileInfo fi = new FileInfo(name);
                string ext = fi.Extension;
                name = name.Replace(ext, "");
                //Debug.WriteLine(name);
                //
                if (string.Equals(current, name))
                {
                    return i;
                }


            }
            return -1;
        }

        

        private void volumeSlider_ValueChanged(object sender, System.EventArgs e)
        {
            if (current != null)
            {
                string cur = null;
                if (getPlayIndex(current) < 0)
                {
                    cur = playlist[currentNum];
                } else cur = playlist[getPlayIndex(current)];
                //
                cur = Path.GetFileName(cur);
                FileInfo fi = new FileInfo(cur);
                if (fi.Extension == ".flac")
                {
                    if (volumeSlider.Value == 0)
                    {
                        speakerButton.Tag = "cg.png";
                        speakerButton.Image = new Bitmap(@"res\cg.png");
                        soundOut.Volume = 0;
                    }
                    else
                    {
                        soundOut.Volume = (float)volumeSlider.Value / 100f;
                        speakerButton.Tag = "cb.png";
                        this.speakerButton.Image = global::KBAudioPlayer.Properties.Resources._62789_speaker_medium_volume_icon;

                    }
                }
                else
                {
                    if (volumeSlider.Value == 0)
                    {
                        speakerButton.Tag = "cg.png";
                        speakerButton.Image = new Bitmap(@"res\cg.png");
                        waveOut.Volume = 0;
                    }
                    else
                    {
                        waveOut.Volume = (float)volumeSlider.Value / 100f;
                        speakerButton.Tag = "cb.png";
                        this.speakerButton.Image = global::KBAudioPlayer.Properties.Resources._62789_speaker_medium_volume_icon;

                    }
                }
                label1.Text = volumeSlider.Value.ToString() + "%";
            }
        }

        private void UpdatePlaylist()
        {
            lstSongs.Items.Clear();

            for (int i = 0; i < playlist.Count; i++)
            {
                string result = Path.GetFileName(playlist[i]);
                // delete mp3 from filename
                result = result.Replace(".mp3", "");
                result = result.Replace(".flac", "");
                //if file exist
                if (File.Exists(playlist[i])){
                    TagLib.File f = TagLib.File.Create(playlist[i], TagLib.ReadStyle.Average);
                    TimeSpan duration = f.Properties.Duration;
                    string songlength = duration.ToString("mm':'ss");
                    string[] item = { (i + 1).ToString(), result, songlength };
                    lstSongs.Items.Add(new ListViewItem(item));
                }
                else
                {
                    string[] item = { (i + 1).ToString(), "!" + result, ""};
                    ListViewItem tmp = new ListViewItem(item);
                    tmp.ToolTipText = "파일이 이동되었거나 삭제되었습니다";
                    tmp.ForeColor = System.Drawing.Color.Red;
                    lstSongs.Items.Add(tmp);
              
                }


            }
            foreach (string tmp in playlist)
            {
                //Debug.WriteLine(tmp);
            }
            if (reader != null) playTime.Stop();
            if (_playback != null)
            {
                if (_playback.IsPlaying()) _playback.Stop();
            }
            // clear file before writing
            File.WriteAllText(path + @"\playlist.dat", string.Empty);
            FileStream fs = File.Open(path + @"\playlist.dat", FileMode.Open);
            //write again
            StreamWriter writer = new StreamWriter(fs, Encoding.UTF8);
            {
                foreach (string tmp in playlist)
                {
                    //Debug.WriteLine(tmp);
                    writer.WriteLine(tmp);
                }

            }
            writer.Dispose();
            writer.Close();
            WriteXML();

        }
        

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "오디오 파일|*.mp3;*.flac|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true; // 파일 다중 선택

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    string filePath = openFileDialog.FileNames[i];
                    playlist.Add(filePath);
                    UpdatePlaylist();
                }
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            waveOut.Pause();
            isPaused = true;
            playTime.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timeElapsed > 3000)
            {
                waveOut.Stop();
                isPaused = false;
                timer1.Stop();
                timeElapsed = 0;
            }
            timeElapsed += 1000;
            float step = 1 / 3;
            waveOut.Volume -= step;

        }

        public void WriteXML()
        {
            Employee emp = new Employee();
            emp.Volume = (float)volumeSlider.Value/100;
            emp.Height = this.ClientSize.Height;

            string path2 = path + @"\pref.xml";
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(emp.GetType());

            System.IO.FileStream file = System.IO.File.Create(path2);

            writer.Serialize(file, emp);
            file.Close();

        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBox.Show("close");
            // stop the player
            playerClose();

        }

        private void playerClose()
        {
            foreach (string tmp in playlist)
            {
                //Debug.WriteLine(tmp);
            }
            if (reader != null) playTime.Stop();
            if (_playback != null)
            {
                if (_playback.IsPlaying())
                {
                    _playback.Stop();
                    //MessageBox.Show("close");
                    waveOut.Stop();
                    waveOut.Dispose();
                }

            }
            // clear file before writing
            File.WriteAllText(path + @"\playlist.dat", string.Empty);
            FileStream fs = File.Open(path + @"\playlist.dat", FileMode.Open);
            //write again
            StreamWriter writer = new StreamWriter(fs, Encoding.UTF8);
            {
                foreach (string tmp in playlist)
                {
                    //Debug.WriteLine(tmp);
                    writer.WriteLine(tmp);
                }

            }
            waveOut.Stop();
            waveOut.Dispose();
            writer.Dispose();
            writer.Close();
            WriteXML();
            WriteJson();
            Application.Exit();
            
            timer1.Stop();


        }

        void WriteJson()
        {
            // JSON 파일 경로
            string filePath = "pref.json";

            // Person 객체 생성
            Person person = new Person
            {
                Name = "John",
                Volume = (int)(volumeSlider.Value),
                City = "New York"
            };

            // 객체를 JSON 형식으로 직렬화
            string jsonData = JsonConvert.SerializeObject(person, Newtonsoft.Json.Formatting.Indented);

            // JSON 데이터 파일에 쓰기
            File.WriteAllText(filePath, jsonData);
        }
        void volumeSlider_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;//disable default mouse wheel
            if (e.Delta > 0)
            {
                if (volumeSlider.Value < volumeSlider.Maximum)
                {
                    try
                    {
                        volumeSlider.Value +=3;
                    }
                    catch { }
                }
            }
            else
            {
                if (volumeSlider.Value > volumeSlider.Minimum)
                {
                    try
                    {
                        volumeSlider.Value -=3;
                    }
                    catch { }
                }
            }
        }

        public class Person
        {
            public string Name { get; set; }
            public int Volume { get; set; }
            public string City { get; set; }
        }


        public class Employee   // public 이어야 함
        {
            public int Seq;
            public int Id { get; set; }
            public string Name { get; set; }
            public string Dept { get; set; }
            public float Volume { get; set; }
            public int Height { get; set; }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            playTime.Stop();
            waveOut.Stop();

            var item = lstSongs.SelectedItems[0];
            current = item.SubItems[1].Text;

            int index = getPlayIndex(current);
            if (index < 0) index = currentNum;

            if (index <= playlist.Count - 1)
            {
                lstSongs.Items[index].Selected = false;
                lstSongs.Items[index + 1].Selected = true;
                current = Path.GetFileName(current);
                FileInfo fi2 = new FileInfo(current);
                if (fi2.Extension == ".mp3")
                {
                    //mp3 tag
                    var file = TagLib.File.Create(playlist[index + 1]);
                    if (file.Tag.Pictures.Length >= 1)
                    {
                        var bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                        pictureBox2.Image = Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(65, 65, null, IntPtr.Zero);
                    }
                    else pictureBox2.Image = Image.FromFile(@"res\scaled2.png");
                    //
                    duration = file.Properties.Duration;
                    label3.Text = duration.ToString(@"mm\:ss");
                    currentNum++;
                    //
                }

                if (_playback != null)
                {
                    if (_playback.IsPlaying())
                    {
                        _playback.Stop();
                        PlayAudio(playlist[index + 1]);
                        this._playback.Start();
                        playTime.Start();
                        current = playlist[index + 1];
                        currentNum++;
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var item = lstSongs.SelectedItems[0];
            Console.WriteLine(item);
            current = item.SubItems[1].Text;
            int index = getIndexFromSelect(current);
            //Console.WriteLine(index);
            
            if (index > -1)
            {
                playlist.RemoveAt(index);
            }
            //
            UpdatePlaylist();

        }

        private void speakerButton_Click(object sender, EventArgs e)
        {

            //
            int tmpVolume= (int)volumeSlider.Value; 
            speakerButton.Image.Dispose();
            if (speakerButton.Tag.ToString() == "cb.png") {
                speakerButton.Tag = "cg.png";
                speakerButton.Image = new Bitmap(@"res\cg.png");
                tmpVolume = (int)volumeSlider.Value;
                waveOut.Volume = 0;
            } else
            {
                speakerButton.Tag = "cb.png";
                this.speakerButton.Image = global::KBAudioPlayer.Properties.Resources._62789_speaker_medium_volume_icon;
                waveOut.Volume = (float)tmpVolume/100f;
            }

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Left = e.X + this.Left - MouseDownLocation.X;
                this.Top = e.Y + this.Top - MouseDownLocation.Y;
            }

        }
        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //create a new tab and attach a new song list
            var lastIndex = this.tabControl1.TabCount - 1;
            if (this.tabControl1.GetTabRect(lastIndex).Contains(e.Location))
            {
                this.tabControl1.TabPages.Insert(lastIndex, "새선곡");

                this.tabControl1.SelectedIndex = lastIndex;
                //
                newlstSongs = new ListView();
                newlstSongs.Alignment = System.Windows.Forms.ListViewAlignment.Default;
                newlstSongs.AllowDrop = true;
                newlstSongs.AutoArrange = false;
                newlstSongs.BackColor = System.Drawing.SystemColors.InactiveBorder;
                newlstSongs.Dock = System.Windows.Forms.DockStyle.Fill;
                newlstSongs.Font = new System.Drawing.Font("Segoe UI", 8F);
                newlstSongs.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
                newlstSongs.FullRowSelect = true;
                newlstSongs.GridLines = true;
                newlstSongs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
                newlstSongs.HideSelection = false;
                newlstSongs.Location = new System.Drawing.Point(3, 3);
                newlstSongs.Name = "lstSongs" + lastIndex;
                newlstSongs.ShowItemToolTips = true;
                newlstSongs.Size = new System.Drawing.Size(259, 207);
                //newlstSongs.TabIndex = 19;
                newlstSongs.UseCompatibleStateImageBehavior = false;
                newlstSongs.View = System.Windows.Forms.View.Details;
                //
                newlstSongs.MouseClick += new MouseEventHandler(newlstSongs_MouseClick);
                newlstSongs.Columns.Add("순번");
                newlstSongs.Columns.Add("곡명");
                newlstSongs.Columns.Add("길이");
                newlstSongs.Columns[0].Width = 25;
                newlstSongs.Columns[1].Width = 283 - 90;
                newlstSongs.Columns[2].Width = 60;
                //
                this.tabControl1.TabPages[lastIndex].Name = "newPage" + lastIndex;
                this.tabControl1.TabPages[lastIndex].ControlAdded += new ControlEventHandler(tabPage_ControlAdded);
                this.tabControl1.TabPages[lastIndex].Controls.Add(newlstSongs);
                //
                SetHeight(newlstSongs, 30);
            }

        }

        public Control GetControlByName(Control ParentCntl, string NameToSearch)
        {
            if (ParentCntl.Name == NameToSearch)
                return ParentCntl;

            foreach (Control ChildCntl in ParentCntl.Controls)
            {
                Control ResultCntl = GetControlByName(ChildCntl, NameToSearch);
                if (ResultCntl != null)
                    return ResultCntl;
            }
            return null;
        }



        private void tabPage_ControlAdded(object sender, EventArgs e)
        {
            var lst = sender as TabPage;
            if (lst != null)
            {
                
                if (lst.Name == "newPage1")
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(path + @"\playlist2.dat", Encoding.UTF8);
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        playlist2.Add(line);
                    }
                    file.Close();

                    for (int i = 0; i < playlist2.Count; i++)
                    {
                        string result = Path.GetFileName(playlist2[i]);
                        // delete mp3 from filename
                        result = result.Replace(".mp3", "");
                        result = result.Replace(".flac", "");
                        string[] item = { (i + 1).ToString(), result, "00:00" };
                        //
                        var ctn = lst.Controls[0];
                        if (ctn != null)
                        {
                            ListView tmp = (ListView)lst.Controls[0];
                            tmp.Items.Add(new ListViewItem(item));
                        }
                        
                    }
                }
            }
        }
        

        private void newlstSongs_HandleCreated(object sender, EventArgs e)
        {
            //this is important
            var lst = sender as ListView;
            if (lst != null)
            {
                if (lst.Name == "lstSongs1")
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(path + @"\playlist2.dat", Encoding.UTF8);
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        playlist2.Add(line);
                    }
                    file.Close();

                    for (int i = 0; i < playlist2.Count; i++)
                    {
                        string result = Path.GetFileName(playlist[i]);
                        // delete mp3 from filename
                        result = result.Replace(".mp3", "");
                        result = result.Replace(".flac", "");
                        string[] item = { (i + 1).ToString(), result, "00:00" };
                        lst.Items.Add(new ListViewItem(item));
                    }
                }
            }

        }

        private void newlstSongs_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
        
            for (int i=0; i<this.tabControl1.TabPages.Count ; i++)
            {
                if (this.tabControl1.GetTabRect(i).Contains(e.X, e.Y)){
                    if (e.Button == MouseButtons.Right)
                    {
                       //get tab page number i
                    }
                }
            }
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        ContextMenu m = new ContextMenu();
                        MenuItem m1 = new MenuItem("편집");
                        m1.Click += (senders, es) => {
                            equalizerForm.Show();
                        };
                        MenuItem m3 = new MenuItem("삭제");
                        //m3.Checked = isTitle;

                        m3.Click += (senders, es) => {
                            
                        };

                        MenuItem m2 = new MenuItem("탭추가");
                        m2.MenuItems.Add(new MenuItem("기본"));
                        m2.MenuItems.Add(new MenuItem("파스텔"));
                        m2.Click += (senders, es) => {
                            //equalizerForm.Show();
                        };

                        MenuItem m5 = new MenuItem("플레이리스트 내보내기");
                        m5.Click += (senders, es) => {
                            SaveFileDialog saveFileDialog = new SaveFileDialog();

                            saveFileDialog.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
                            saveFileDialog.FilterIndex = 1;
                            saveFileDialog.FileName = "playlist.dat"; // 기본 파일 이름 설정

                            DialogResult result = saveFileDialog.ShowDialog();

                            // Process input if the user clicked OK
                            if (result == DialogResult.OK)
                            {
                                try
                                {
                                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                                    {

                                        // 데이터 작성
                                        foreach (string tmp in playlist)
                                        {
                                            writer.Write(tmp);
                                            writer.WriteLine();
                                        }
                                    }
                                    MessageBox.Show("파일로 내보내기 완료!");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("오류 발생: " + ex.Message);
                                }
                            }

                        };

                        MenuItem m_close = new MenuItem("닫기(C)");
                        m_close.Click += (senders, es) => {
                            playerClose();
                        };
                        m.MenuItems.Add(m1);
                        m.MenuItems.Add(m2);
                        m.MenuItems.Add(m3);
                        m.MenuItems.Add(m5);
                        m.MenuItems.Add(m_close);
                        m.Show(this, new Point(e.X, e.Y+150));
                        //places the menu at the pointer position
                    }
                    break;
            }
        }


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            

            switch (e.Button)
            {
                case MouseButtons.Left:
                    MouseDownLocation = e.Location;
                    break;
                case MouseButtons.Right:
                    {
                        ContextMenu m = new ContextMenu();
                        MenuItem m1 = new MenuItem("이퀄라이저");
                        m1.Click += (senders, es) => {
                            equalizerForm.Show();
                        };
                        MenuItem m3 = new MenuItem("창 가장자리 보이기");
                        m3.Checked = isTitle;
                        m3.Click += (senders, es) => {
                            if (m3.Checked == true)
                            {
                                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                                isTitle = false;
                            } else
                            {
                                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                                isTitle = true;
                            }
                        };

                        MenuItem m2 = new MenuItem("스킨");
                        MenuItem mm1 = new MenuItem("기본");
                        MenuItem mm2 = new MenuItem("파스텔");
                        if (defaultskin)
                        {
                            mm1.Checked = true;
                        } else mm2.Checked = true;
                        
                        mm1.Click += (senders, es) =>
                        {
                            this.BackColor = System.Drawing.Color.White;
                            this.Refresh();
                            defaultskin = true;
                            mm1.Checked = true;
                            mm2.Checked = false;
                            
                        };
                        mm2.Click += (senders, es) =>
                        {
                            //MessageBox.Show("eee");
                            this.BackColor = System.Drawing.Color.AliceBlue;
                            this.Refresh();
                            mm2.Checked = true;
                            defaultskin = false;
                            mm1.Checked = defaultskin;
                        };
                        m2.MenuItems.Add(mm1);
                        m2.MenuItems.Add(mm2);
                   
                        MenuItem m4 = new MenuItem("닫기(C)");
                        
                        m4.Click += (senders, es) => {
                            playerClose();
                        };
                        MenuItem m5 = new MenuItem("파일이 존재하지 않는 목록 삭제");
                        m5.Click += (senders, es) => {
                            List<string> newplaylist = new List<string>();
                            for (int i = 0; i < playlist.Count; i++)
                            {
                                string result = Path.GetFileName(playlist[i]);
                                
                                if (File.Exists(playlist[i]))
                                {
                                    newplaylist.Add(playlist[i]);
                                    
                                }
                            }
                            playlist.Clear();
                            for (int i = 0; i < newplaylist.Count; i++)
                            {
                                playlist.Add(newplaylist[i]);
                            }
                            
                            UpdatePlaylist();
                        };
                        MenuItem m6 = new MenuItem("선택 삭제");

                        m.MenuItems.Add(m1);
                        m.MenuItems.Add(m2);
                        m.MenuItems.Add(m3);
                        m.MenuItems.Add(m5);
                        m.MenuItems.Add(m6);
                        m.MenuItems.Add(m4);
                        m.Show(this, new Point(e.X, e.Y));//places the menu at the pointer position
                    }
                    break;
            }
        }

    }
}
