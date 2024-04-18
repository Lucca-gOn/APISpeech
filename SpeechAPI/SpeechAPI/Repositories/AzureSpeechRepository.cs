using SpeechAPI.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace SpeechAPI.Repositories
{
    public class AzureSpeechRepository : ISpeechRepository
    {
        private readonly SpeechConfig _speechConfig;

        public AzureSpeechRepository(IConfiguration configuration)
        {
            _speechConfig = SpeechConfig.FromSubscription(
                configuration["AzureSpeechSubscriptionKey"],
                configuration["AzureSpeechRegion"]
            );
        }

        public async Task<string> SpeakToTextAsync(string audio)
        {
            try
            {
                var audioConfig = AudioConfig.FromWavFileInput(audio);

                using (var recognizer = new SpeechRecognizer(_speechConfig, "pt-BR", audioConfig))
                {
                    var result = await recognizer.RecognizeOnceAsync();


                    switch (result.Reason)
                    {
                        case ResultReason.RecognizedSpeech:
                            return $"Texto identificado: \n\n '{result.Text}'";
                        case ResultReason.NoMatch:
                            return "Não foi possível reconhecer o discurso.";
                        default:
                            return "Ocorreu um erro durante o reconhecimento de fala.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Erro ao converter áudio para texto: {ex.Message}";
            }
        }

        public async Task<string> TextToSpeechAsync(string text, string caminho)
        {
            try
            {
                var audioConfig = AudioConfig.FromWavFileOutput(caminho);

                using (var sintetizador = new SpeechSynthesizer(_speechConfig, audioConfig))
                {
                    await sintetizador.SpeakTextAsync(text);

                    return ($"Arquivo de áudio salvo em: {caminho}");
                }
            }
            catch (Exception ex)
            {
                return $"Erro ao converter texto para áudio: {ex.Message}";
            }
        }
    }
}
