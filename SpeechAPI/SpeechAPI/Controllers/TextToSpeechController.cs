using Microsoft.AspNetCore.Mvc;
using SpeechAPI.Domain;
using SpeechAPI.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SpeechAPI.Controllers
{
    [ApiController]
    [Route("api/texttospeech")]
    public class TextToSpeechController : ControllerBase
    {
        private readonly ISpeechRepository _speechRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TextToSpeechController(ISpeechRepository speechRepository, IWebHostEnvironment webHostEnvironment)
        {
            _speechRepository = speechRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> PostTextToSpeech(TextDomain texto)
        {
            if (texto == null || string.IsNullOrWhiteSpace(texto.Texto))
            {
                return BadRequest("O texto para conversão é necessário.");
            }

            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "audios");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var fileName = $"{Guid.NewGuid()}.wav"; 
                var fullPath = Path.Combine(filePath, fileName);

                var result = await _speechRepository.TextToSpeechAsync(texto.Texto, fullPath);

                if (result.StartsWith("Erro"))
                {
                    return BadRequest(result);
                }

                var fileUrl = $"{Request.Scheme}://{Request.Host}/audios/{fileName}";
                return Ok(new { audioUrl = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
