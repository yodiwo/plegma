using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNIVERSAL
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;
#endif

namespace Yodiwo.Tools
{
    public static class Speech
    {

#if UNIVERSAL
        public static async void SpeakText(string TTS)
        {
            try
            {
                var ttssynthesizer = new SpeechSynthesizer();

                //Set the Voice/Speaker
                using (var Speaker = new SpeechSynthesizer())
                {
                    Speaker.Voice = (SpeechSynthesizer.AllVoices.First(x => x.Gender == VoiceGender.Female));
                    ttssynthesizer.Voice = Speaker.Voice;
                }

                var ttsStream = await ttssynthesizer.SynthesizeTextToStreamAsync(TTS);

                //play the speech
                MediaElement media = new MediaElement();
                media.SetSource(ttsStream, " ");
            }
            catch (Exception ex) { }
        }
#endif

    }
}
