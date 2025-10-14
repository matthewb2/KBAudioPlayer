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
    public partial class Form1

    {
        void UpdatePlaylist()
        {
            lstSongs.Items.Clear();

            for (int i = 0; i < playlist.Count; i++)
            {
                string result = Path.GetFileName(playlist[i]);
                result = result.Replace(".mp3", "");
                result = result.Replace(".flac", "");
                //if file exist
                if (File.Exists(playlist[i]))
                {
                    TagLib.File f = TagLib.File.Create(playlist[i], TagLib.ReadStyle.Average);
                    System.TimeSpan duration = f.Properties.Duration;
                    string songlength = duration.ToString("mm':'ss");
                    string[] item = { (i + 1).ToString(), result, songlength };
                    lstSongs.Items.Add(new ListViewItem(item));
                }
                else
                {
                    string[] item = { (i + 1).ToString(), "!" + result, "" };
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
            File.WriteAllText(path + @"\playlist.dat", string.Empty);
            FileStream fs = File.Open(path + @"\playlist.dat", FileMode.Open);
            //write again
            StreamWriter writer = new StreamWriter(fs, Encoding.UTF8);
            {
                foreach (string tmp in playlist)
                {
                    writer.WriteLine(tmp);
                }

            }
            writer.Dispose();
            writer.Close();
            WriteXML();

        }


        private void playerClose()
        {
            try
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
                        waveOut.Stop();
                        waveOut.Dispose();
                    }

                }
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
                WriteJson(getPlayIndex(item_current));
                timer1.Stop();
            }

            catch (Exception ex)
            {
                MessageBox.Show("종료 중 오류: " + ex.Message);
            }
            finally
            {
                Application.Exit();

                // 혹시 백그라운드 스레드가 살아 있으면 강제 종료
                Environment.Exit(0);


            }

        }


        [System.Runtime.InteropServices.DllImport("user32", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]

        private static extern bool ShowScrollBar(IntPtr hwnd, int wBar, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool bShow);

        private void HideHorizontalScrollBar()
        {
            ShowScrollBar(lstSongs.Handle, 0, false);

        }


        private int getIndexFromSelect(string current)
        {
            // add escape character for regular match
            for (int i = 0; i <= playlist.Count - 1; i++)
            {
                string name = playlist[i];
                //name = Path.GetFileName(name);
                /*
                FileInfo fi = new FileInfo(name);
                string ext = fi.Extension;
                name = name.Replace(ext, "");
				Console.WriteLine(name+ " "+current);
				*/
                if (current.Contains(name))
                {
                    return i;
                }
            }
            return -1;
        }

        public void WriteXML()
        {
            Employee emp = new Employee();
            emp.Volume = (float)volumeSlider.Value / 100;
            emp.Height = this.ClientSize.Height;

            string path2 = path + @"\pref.xml";
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(emp.GetType());

            System.IO.FileStream file = System.IO.File.Create(path2);

            writer.Serialize(file, emp);
            file.Close();

        }


        private void PlayAudio(string name)
        {
            if (File.Exists(name))
            {
                string result = Path.GetFileName(name);
                result = result.Replace(".mp3", "");
                FileInfo fi = new FileInfo(name);

                if (isPaused == true)
                {
                    playTime.Start();
                    reader = new AudioFileReader(name);
                    equalizer = new Equalizer(reader, bands);
                    equalizerForm.setEqualizer(bands, equalizer, waveOut);
                    waveOut.Stop();
                    waveOut.Init(equalizer);
                    waveOut.Play();
                    label1.Text = Path.GetFileName(result);
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
                        label1.Text = Path.GetFileName(result);
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
                        duration = soundSource.GetLength();
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
            }
            else
            {
                if (_playback.IsPlaying()) _playback.Stop();
                playTime.Stop();

            }

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


        // Set the slider's value. If the value has changed,
        // display the value tooltip.
        private void SetValue(float value)
        {
            // Make sure the new value is within bounds.
            if (value < MinimumValue) value = MinimumValue;
            if (value > MaximumValue) value = MaximumValue;
            // See if the value has changed.
            if (SliderValue == value) return;
            SliderValue = value;
            picSlider.Refresh();

        }


        void WriteJson(int index)
        {
            // JSON 파일 경로
            string filePath = "pref.json";

            // Person 객체 생성
            Person person = new Person
            {
                Name = "John",
                Volume = (int)(volumeSlider.Value),
                City = "New York",
                selLists = index
            };
            string jsonData = JsonConvert.SerializeObject(person, Newtonsoft.Json.Formatting.Indented);

            // JSON 데이터 파일에 쓰기
            File.WriteAllText(filePath, jsonData);
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

    }
}
