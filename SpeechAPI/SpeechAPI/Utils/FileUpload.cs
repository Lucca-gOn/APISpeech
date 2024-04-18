using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SpeechAPI.Utils
{
    public class FileUpload
    {
        [NotMapped]
        [JsonIgnore]
        public IFormFile ArquivoAudio { get; set; }
    }
}
