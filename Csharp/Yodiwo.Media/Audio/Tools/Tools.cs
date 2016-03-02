using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.IO;

namespace Yodiwo.Media.Audio
{
    public static class Tools
    {
        public static IMp3FrameDecompressor CreateFrameDecompressor(Mp3Frame frame)
        {
            WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate,
                                                        frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                                                        frame.FrameLength,
                                                        frame.BitRate
                                                        );
            return new AcmMp3FrameDecompressor(waveFormat);
        }

        //------------------------------------------------------------------------------------------------------------------------

        public static void WriteHeader(Stream stream, int length, int audioSampleRate, int audioBitsPerSample, int audioChannels)
        {
            //http://www.topherlee.com/software/pcm-tut-wavformat.html
            using (BinaryWriter bw = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                bw.Write(new char[4] { 'R', 'I', 'F', 'F' });
                int fileSize = 36 + length;
                bw.Write(fileSize);
                bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                bw.Write((int)16);
                bw.Write((short)1);
                bw.Write((short)audioChannels);
                bw.Write((int)audioSampleRate);
                bw.Write((int)(audioSampleRate * ((audioBitsPerSample * audioChannels) / 8)));
                bw.Write((short)((audioBitsPerSample * audioChannels) / 8));
                bw.Write((short)audioBitsPerSample);
                bw.Write(new char[4] { 'd', 'a', 't', 'a' });
                bw.Write((int)length);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
    }

    public class AudioFormat
    {
        public int samplerate;
        public int bitsperchannel;
        public int channels;
        public AudioFormat(int sr, int bpc, int c)
        {
            this.samplerate = sr;
            this.bitsperchannel = bpc;
            this.channels = c;
        }
    }
}
