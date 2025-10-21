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

        private IWavePlayer waveOut = new WaveOutEvent();
        private ISoundOut soundOut;
        //
        private Equalizer equalizer;
        private EqualizerBand[] bands;
        //
        private Boolean isPaused = false;
        private Boolean isPlay = false;
        private int timeElapsed = 0;
        private TimeSpan duration;
        private int currentNum = 0;

        private int NumLEDS = 20;
        //
        private RealTimePlayback _playback;
        private static string path = null;
        private float SliderValue = 0.0f;
        private const float MinimumValue = 0.0f;
        private const float MaximumValue = 1.0f;
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
        const int WM_NCLBUTTONDOWN = 0x00A1;
        ListView newlstSongs;


        public Form1()
        {
            InitializeComponent();
            path = Directory.GetCurrentDirectory();
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //this.Load += Form1_Load;
            this.Closing += Form1_Closing;
            this.Resize += Form1_Resize;
            //this.MouseClick += Form1_MouseClick;
            //this.MouseDown += Form1_MouseDown;
            //this.MouseMove += Form1_MouseMove;

            tabControl1.HandleCreated += new EventHandler(tabControl1_HandleCreated);
            tabControl1.MouseClick += new MouseEventHandler(tabControl1_MouseClick);
            tabControl1.MouseDown += new MouseEventHandler(tabControl1_MouseDown);

            volumeSlider.ValueChanged += new System.EventHandler(volumeSlider_ValueChanged);
            //volumeSlider.MouseUp += new MouseEventHandler(volumeSlider_MouseUp);
            volumeSlider.MouseWheel += new MouseEventHandler(volumeSlider_MouseWheel);
            volumeSlider.MouseHover += new System.EventHandler(volumeSlider_MouseHover);

            lstSongs.MouseDoubleClick += new MouseEventHandler(lstSongs_MouseDoubleClick);
            //lstSongs.MouseClick += new MouseEventHandler(lstSongs_MouseClick);
            lstSongs.DragDrop += new System.Windows.Forms.DragEventHandler(lstSongs_DragDrop);
            lstSongs.DragEnter += new System.Windows.Forms.DragEventHandler(lstSongs_DragEnter);
            lstSongs.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(lstSongs_ItemDrag);
            lstSongs.DragOver += new DragEventHandler(lstSongs_DragOver);
            //lstSongs.SizeChanged += new System.EventHandler(lstSongs_SizeChanged);

            // volume spectrum
            this.peakMeterCtrl1.SetRange(40, 70, 100);
            this.peakMeterCtrl1.Start(33);
            

            lstSongs.SelectedIndexChanged += (s, e) => Console.WriteLine("SelectedIndexChanged");
            lstSongs.MouseClick += (s, e) => Console.WriteLine("MouseClick");
            lstSongs.Click += (s, e) => Console.WriteLine("Click");


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
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;
            this.Location = new Point(area.Width - this.Size.Width - 200, area.Height - this.ClientSize.Height - 200);
            ToolTip addtoolTip = new ToolTip();
            addtoolTip.AutoPopDelay = 5000;
            addtoolTip.InitialDelay = 1000;
            addtoolTip.ReshowDelay = 500;
            addtoolTip.ShowAlways = true;
            addtoolTip.IsBalloon = true;
            addtoolTip.SetToolTip(this.btnAdd, "추가");
            tabPage1.Text = "선곡1";
            tabPage2.Text = "+";



            lstSongs.Columns.Add("순번", 25);
            lstSongs.Columns.Add("곡명", 163);
            lstSongs.Columns.Add("길이", 60);
            //lstSongs.Columns[0].Width = 25;
            //lstSongs.Columns[1].Width = 183;
            //lstSongs.Columns[2].Width = -2;
            //
            SetHeight(lstSongs, 32);
            
            // 1. ListView의 View 속성이 Details인지 확인
            if (lstSongs.View == View.Details)
            {
                // 2. 마지막 컬럼의 인덱스 가져오기 (0부터 시작)
                int lastColumnIndex = lstSongs.Columns.Count - 1;

                if (lastColumnIndex >= 0)
                {                    
                    // 마지막 컬럼의 너비를 ListView의 너비에 맞추어 설정 (Fill 역할)
                    //lstSongs.Columns[lastColumnIndex].Width = -2; // -2는 ListView의 너비에 맞춰 자동 조정하도록 지시
                }
            }
            // 2. 가로 스크롤바 강제 숨김
            //ListViewScrollHelper.HideHorizontalScrollBar(lstSongs);

            this.Load += Form1_Load;

        }

        // 이 메서드는 lstSongs의 크기가 변경될 때마다 실행됩니다.
        private void lstSongs_SizeChanged(object sender, EventArgs e)
        {
            // 1. 컬럼 너비를 조정하여 가로 스크롤바가 생기는 것을 방지하는 로직을 여기에 넣습니다.
            int lastColumnIndex = lstSongs.Columns.Count - 1;

            if (lastColumnIndex >= 0)
            {
                // 마지막 컬럼이 남은 공간을 채우도록 설정
                lstSongs.Columns[lastColumnIndex].Width = -2;
            }

            // 2. 만약 -2 설정으로 해결되지 않았다면, API 호출 코드를 여기에 추가하여
            //    가로 스크롤바를 강제로 숨깁니다.
            ListViewScrollHelper.HideHorizontalScrollBar(lstSongs);
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
            playlist.RemoveAt(item_index);
            playlist.Insert(targetIndex, item_current);
            UpdatePlaylist();
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
            tabControl1.Size = new Size(control.Size.Width - 20, control.Size.Height - (450 - 208));
            btnToTop.Location = new Point(4, control.Size.Height - (470 - 389));
            btnUp.Location = new Point(40, control.Size.Height - (470 - 389));
            btnDown.Location = new Point(76, control.Size.Height - (470 - 389));
            btnToBottom.Location = new Point(112, control.Size.Height - (470 - 389));
            btnDelete.Location = new Point(146, control.Size.Height - (470 - 389));
            btnAdd.Location = new Point(242, control.Size.Height - (470 - 389));           

        }
        // set focus to volume slider when click form
        void Form1_MouseClick(object sender, MouseEventArgs e)
        {
           // volumeSlider.Focus();
        }


        void lstSongs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstSongs.SelectedItems.Count == 0 || lstSongs.SelectedIndices.Count == 0)
                return; // 선택된 항목이 없으면 바로 종료

            playTime.Start();
            this._playback = new RealTimePlayback();
            this._playback.Start();

            var item = lstSongs.SelectedItems[0];
            item_current = item.SubItems[1].Text;
            Debug.WriteLine(item_current);
            if (item_current != null)
            {
                label1.Text = Path.GetFileName(item_current);
                //
                int index = lstSongs.SelectedIndices[0];
                if (index >= 0)
                {
                    waveOut.Stop();
                    PlayAudio(playlist[index]);
                    duration = reader.TotalTime;
                    label3.Text = duration.ToString(@"mm\:ss");
                    item_current = playlist[index];
                    label1.Text = Path.GetFileName(item_current);
							
					isPlay = true;				

                }
            }
            // change play button icon to pause icon
            if (isPlay)
            {
                button1.Image = new Bitmap(@"res\control-pause-icon.png");

            }

        }


        private void picSlider_MouseDown(object sender, MouseEventArgs e)
        {
            MouseIsDown = true;
            SliderValue = XtoValue(e.X);

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
            SendMessage(this.tabControl1.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
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

        private float ValueToX(float value)
        {
            return (picSlider.ClientSize.Width - 1) *
                (value - MinimumValue) / (float)(MaximumValue - MinimumValue);
        }

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
        private void playTime_Tick(object sender, EventArgs e)
        {
            FillMeterData();

            //플레이어가 재생 중일 때
            // change play button icon to pause icon
            if (isPlay)
            {
                button1.Image = new Bitmap(@"res\control-pause-icon.png");
                label1.Text = Path.GetFileName(item_current);

            }

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
            
            cur = Path.GetFileName(cur);
            FileInfo fi = new FileInfo(cur);
			
            if (fi.Extension == ".flac")
            {

                TimeSpan tmp2 = soundSource.GetPosition();
                TimeSpan totalTime = soundSource.GetLength();
                label2.Text = tmp2.ToString(@"mm\:ss");
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
                    // 재생이 파일 끝까지 도달했을 때
                    if (reader.CurrentTime == reader.TotalTime)
                    {
                        waveOut.Stop();
                        Console.WriteLine(item_current);
                        int index = getIndexFromSelect(item_current);
                        Console.WriteLine(index.ToString());
                        PlayAudio(playlist[index + 1]);
                        Console.WriteLine(playlist[index + 1]);
                        // update current song
                        item_current = playlist[index + 1];
                        label1.Text = Path.GetFileName(item_current);
                        lstSongs.Items[index].Selected = false;
                        lstSongs.Items[index+1].Selected = true;
                        
                    }
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
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
            if (playlist.Count>0)
                UpdatePlaylist();
            /*
            string filePath = "pref.json";
            // JSON 파일 읽기
            string jsonData = File.ReadAllText(filePath);
            // JSON 데이터 파싱
            Person person = JsonConvert.DeserializeObject<Person>(jsonData);

            Debug.WriteLine(person.Volume);
            volumeSlider.Value = person.Volume;
            currentNum = person.selLists;

            if (currentNum != -1)
                lstSongs.Items[currentNum].Selected = true;
            */
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
            var item = lstSongs.SelectedItems[0];
            string select = item.SubItems[1].Text;
        }

        // Move the selected item to the top of the list (index 0).
        private void btnToTop_Click(object sender, EventArgs e)
        {
            var item = lstSongs.SelectedItems[0];
            item_current = item.SubItems[1].Text;
            int index = getIndexFromSelect(item_current);
            string tmp = playlist[index];
            playlist.RemoveAt(index);
            playlist.Insert(0, tmp);
            UpdatePlaylist();
            lstSongs.Items[0].Selected = true;

        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var item = lstSongs.SelectedItems[0];
            item_current = item.SubItems[1].Text;
            int index = getIndexFromSelect(item_current);

            string[] items = { };
            if (index > 0)
            {
                string tmp = playlist[index];
                playlist.RemoveAt(index);
                playlist.Insert(index - 1, tmp);
                UpdatePlaylist();
                lstSongs.Items[index - 1].Selected = true;
            }
        }

        // Move the selected item down one position.
        private void btnDown_Click(object sender, EventArgs e)
        {
            var item = lstSongs.SelectedItems[0];
            item_current = item.SubItems[1].Text;
            int index = getIndexFromSelect(item_current);
            if (index < lstSongs.Items.Count - 1)
            {
                string tmp = playlist[index];
                playlist.RemoveAt(index);
                playlist.Insert(index + 1, tmp);
                //
                UpdatePlaylist();
                lstSongs.Items[index + 1].Selected = true;
            }
        }

        private void btnToBottom_Click(object sender, EventArgs e)
        {
            var item = lstSongs.SelectedItems[0];
            item_current = item.SubItems[1].Text;
            int index = getIndexFromSelect(item_current);
            string tmp = playlist[index];
            playlist.RemoveAt(index);
            playlist.Add(tmp);
            UpdatePlaylist();
            lstSongs.Items[playlist.Count - 1].Selected = true;
        }


        // 재생 버튼 이벤트 핸들러
        private void button1_Click(object sender, EventArgs e)
        {            
            // 일시정지 구현
            if (isPlay)
            {
                VolumeFadeOut();
                waveOut.Pause();
                if (reader != null)
                    this._playback.Stop();
                playTime.Stop();
                isPaused = true;
                isPlay = false;
                button1.Image = new Bitmap(@"res\control-right-icon.png");

            }
            else if (isPaused || item_current !=null)
            {
                playTime.Start();
                waveOut.Volume = (float)(volumeSlider.Value / 100);
                waveOut.Play();
                if (reader != null)
                    this._playback.Start();
                isPaused = false;
                isPlay = true;
            }

        }

        private void VolumeFadeOut()
        {
            //플레이어를 종료하기 전에 볼륨을 페이드 아웃
            //fade out volune until stop player
            for (int i = 0; i < 10; i++)
            {
                if (waveOut.Volume > 0.1f)
                {
                    waveOut.Volume -= .1f;
                }
                else break;
                Thread.Sleep(300);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isPlay = false;
            button1.Image = new Bitmap(@"res\control-right-icon.png");
            timer1.Interval = 100;
            timer1.Start();
            //restore cover image
            pictureBox2.Image = new Bitmap(@"res\Music-Folder-icon.png");
            SliderValue = 0;
            picSlider.Refresh();
            label2.Text = "00:00";
            //플레이어를 종료하기 전에 볼륨을 페이드 아웃
            VolumeFadeOut();
            if (reader != null)
                this._playback.Stop();
            playTime.Stop();

        }


        private void volumeSlider_ValueChanged(object sender, System.EventArgs e)
        {
            if (item_current != null)
            {
                string cur = null;

                if (getPlayIndex(item_current) == -1)
                {
                    cur = playlist[currentNum];
                }
                else cur = playlist[getPlayIndex(item_current)];

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
        
        private void btnAdd_Click(object sender, EventArgs e)
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
            
        }


        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            playerClose();

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
                        volumeSlider.Value += 1;
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
                        volumeSlider.Value -= 1;
                    }
                    catch { }
                }
            }
        }


        private void btnNext_Click(object sender, EventArgs e)
        {
            playTime.Stop();
            waveOut.Stop();

            var item = lstSongs.SelectedItems[0];
			string current;
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
                        item_current = playlist[index + 1];
                        label1.Text = Path.GetFileName(item_current);
                        currentNum++;
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstSongs.SelectedIndices.Count > 0)
            {

                //
                var item = lstSongs.SelectedItems[0];
                
                item_current = item.SubItems[1].Text;
                //Console.WriteLine(item_current);
                int index = getPlayIndex(item_current);
                //Console.WriteLine(index);
                if (index != -1)
                {
                    
                    lstSongs.BeginUpdate();
                    lstSongs.Items.RemoveAt(index);
                    //lstSongs.Items.Add(new ListViewItem("새 아이템"));
                    

                    lstSongs.EndUpdate();
                    playlist.RemoveAt(index);
                }
                //
                //UpdatePlaylist();
                //lstSongs.Items[index].Selected = true;
                //lstSongs.Items[index].Focused = true;
                //lstSongs.Items[index].EnsureVisible();
                //save list
                SavePlayList();
            }

        }

        private void speakerButton_Click(object sender, EventArgs e)
        {

            //
            int tmpVolume = (int)volumeSlider.Value;
            speakerButton.Image.Dispose();
            if (speakerButton.Tag.ToString() == "cb.png")
            {
                speakerButton.Tag = "cg.png";
                speakerButton.Image = new Bitmap(@"res\cg.png");
                tmpVolume = (int)volumeSlider.Value;
                waveOut.Volume = 0;
            }
            else
            {
                speakerButton.Tag = "cb.png";
                this.speakerButton.Image = global::KBAudioPlayer.Properties.Resources._62789_speaker_medium_volume_icon;
                waveOut.Volume = (float)tmpVolume / 100f;
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

            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                if (this.tabControl1.GetTabRect(i).Contains(e.X, e.Y))
                {
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
                        m.Show(this, new Point(e.X, e.Y + 150));
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
                            }
                            else
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
                        }
                        else mm2.Checked = true;

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


                        MenuItem m_about = new MenuItem("정보(A)");
                        m_about.Click += (senders, es) => {
                            About about = new About();
                            about.Show();
                        };


                        m.MenuItems.Add(m1);
                        m.MenuItems.Add(m2);
                        m.MenuItems.Add(m3);
                        m.MenuItems.Add(m5);
                        m.MenuItems.Add(m6);
                        m.MenuItems.Add(m_about);
                        m.MenuItems.Add(m4);
                        m.Show(this, new Point(e.X, e.Y));//places the menu at the pointer position
                    }
                    break;
            }
        }

    }
}
