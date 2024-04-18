using Microsoft.AspNetCore.Mvc;
using SpeechAPI.Interfaces;
using SpeechAPI.Services;
using SpeechAPI.Utils;

namespace SpeechAPI.Controllers
{
    [ApiController]
    [Route("api/speechtotext")]
    public class SpeechToTextController : ControllerBase
    {
        private readonly ISpeechRepository _speechRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SpeechToTextController(ISpeechRepository speechRepository, IWebHostEnvironment webHostEnvironment)
        {
            _speechRepository = speechRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> PostAudio([FromForm] FileUpload file)
        {
            if (file?.ArquivoAudio == null || file.ArquivoAudio.Length == 0)
            {
                return BadRequest("Nenhum arquivo de áudio foi enviado");
            }

            // Verificação adicionada para garantir que o WebRootPath não seja nulo
            var webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            try
            {
                var filePath = Path.Combine(webRootPath, "audios");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.ArquivoAudio.FileName);
                var fullPath = Path.Combine(filePath, fileName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.ArquivoAudio.CopyToAsync(fileStream);
                }

                string wavFilePath = fullPath;
                if (Path.GetExtension(file.ArquivoAudio.FileName).ToLowerInvariant() != ".wav")
                {
                    // Chamada síncrona alterada para assíncrona
                    wavFilePath = await ConvertToWav.ConvertWav(fullPath, Path.ChangeExtension(fullPath, ".wav"));
                }

                var textResult = await _speechRepository.SpeakToTextAsync(wavFilePath);

                return Ok(new { Texto = textResult });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
