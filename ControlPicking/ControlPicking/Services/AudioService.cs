using System.IO;

namespace ControlPicking.Services
{

    public class AudioService
    {

        public static void Sound(string _filepath)
        {
            var assembly = typeof(AudioService).Assembly;
            Stream audiostream = assembly.GetManifestResourceStream(_filepath);
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(audiostream);
            audio.Play();
        }

    }
}
