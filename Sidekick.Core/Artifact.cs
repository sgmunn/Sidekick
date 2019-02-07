using System.IO;
using System.Threading.Tasks;
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

    public static class ArtifactDownloader
    {
        public static async Task<string> DownloadFromUrlAsync(string url)
        {
            var downloader = new Downloader(ConsoleLog.Log, "artifacts");
            var localFile = await downloader.Download(url);
            return localFile[0];
        }
    }
}
