using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace KBAudioPlayer
{
    public class RealTimePlayback 
    {
        private WasapiCapture _capture;
        private object _lock;
        private int _fftPos;
        private int _fftLength;
        private Complex[] _fftBuffer;
        private float[] _lastFftBuffer;
        private float[] meters1=null;

        private bool _fftBufferAvailable;
        private int _m;
        private Boolean isPlay = false;

        public RealTimePlayback()
        {
            this._lock = new object();

            this._capture = new WasapiLoopbackCapture();
            this._capture.DataAvailable += this.DataAvailable;

            this._m = (int)Math.Log(this._fftLength, 2.0);
            this._fftLength = 2048; // 44.1kHz.
            this._fftBuffer = new Complex[this._fftLength];
            this._lastFftBuffer = new float[this._fftLength];
            this.meters1 = new float[this._fftLength];
        }

        public WaveFormat Format
        {
            get
            {
                return this._capture.WaveFormat;
            }
        }

        private float[] ConvertByteToFloat(byte[] array, int length)
        {
            int samplesNeeded = length / 4;
            float[] floatArr = new float[samplesNeeded];

            for (int i = 0; i < samplesNeeded; i++)
            {
                floatArr[i] = BitConverter.ToSingle(array, i * 4);
            }

            return floatArr;
        }

        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            // Convert byte[] to float[].
            float[] data = ConvertByteToFloat(e.Buffer, e.BytesRecorded);

            // For all data. Skip right channel on stereo (i += this.Format.Channels).
            for (int i = 0; i < data.Length; i += this.Format.Channels)
            {
                this._fftBuffer[_fftPos].X = (float)(data[i] * FastFourierTransform.HannWindow(_fftPos, _fftLength));
                this._fftBuffer[_fftPos].Y = 0;
                this._fftPos++;

                if (this._fftPos >= this._fftLength)
                {
                    this._fftPos = 0;

                    // NAudio FFT implementation.
                    FastFourierTransform.FFT(true, this._m, this._fftBuffer);

                    // Copy to buffer.
                    lock (this._lock)
                    {
                        for (int c = 0; c < this._fftLength; c++)
                        {
                            this._lastFftBuffer[c] = this._fftBuffer[c].X;
                            this.meters1[c] = this._fftBuffer[c].X;
                        }

                        this._fftBufferAvailable = true;
                    }
                }
            }
            //print out
            for (int c = 0; c < this._fftLength; c++)
            {
                this._lastFftBuffer[c] = this._fftBuffer[c].X;
                //Debug.Write(this._fftBuffer[c].X);
                //Debug.Write(":");

            }
            //Debug.WriteLine("");
            //
            

        }

        public float[] GetFFTData()
        {
            return this.meters1;
        }

        public void Start()
        {
            this._capture.StartRecording();
            this.isPlay = true;
        }

        public void Stop()
        {
            this._capture.StopRecording();
            this.isPlay = false;
        }

        public bool GetFFTData(float[] fftDataBuffer)
        {
            lock (this._lock)
            {
                // Use last available buffer.
                if (this._fftBufferAvailable)
                {
                    this._lastFftBuffer.CopyTo(fftDataBuffer, 0);
                    this._fftBufferAvailable = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int GetFFTFrequencyIndex(int frequency)
        {
            int index = (int)(frequency / (this.Format.SampleRate / this._fftLength / this.Format.Channels));
            return index;
        }

        public bool IsPlaying()
        {
            return isPlay;
        }
        
    }
}
