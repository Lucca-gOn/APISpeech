using NAudio.Wave;

namespace SpeechAPI.Services
{
    public class ConvertToWav
    {
        public static async Task<string> ConvertWav(string caminhoOriginal, string caminhoConversao)
        {
            using (var reader = new MediaFoundationReader(caminhoOriginal))
            {
                using (var writer = new WaveFileWriter(caminhoConversao, reader.WaveFormat))
                {
                    reader.CopyTo(writer);
                    return caminhoConversao;
                }
            }
        }
    }
}
