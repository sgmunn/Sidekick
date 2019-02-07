using System.IO;
using Newtonsoft.Json;

namespace Sidekick
{
    public class Artifact
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonIgnore]
        public string FileName
        {
            get
            {
                return Path.GetFileName(this.Url);
            }
        }

        [JsonIgnore]
        public string FileExtension
        {
            get
            {
                return Path.GetExtension(this.Url);
            }
        }
    }
}
