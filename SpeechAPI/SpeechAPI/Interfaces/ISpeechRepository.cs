using Microsoft.CognitiveServices.Speech.Audio;

namespace SpeechAPI.Interfaces
{
    public interface ISpeechRepository
    {
        Task<string> TextToSpeechAsync(string text, string caminho);
        Task<string> SpeakToTextAsync(string audio);
    }
}
