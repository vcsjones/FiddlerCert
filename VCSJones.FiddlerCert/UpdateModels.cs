using System.Runtime.Serialization;

namespace VCSJones.FiddlerCert
{
    [DataContract]
    public class Release
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "assets")]
        public ReleaseAsset[] Assets { get; set; }

        [DataMember(Name = "html_url")]
        public string HtmlUrl { get; set; }
    }

    [DataContract]
    public class ReleaseAsset
    {
        [DataMember(Name = "browser_download_url")]
        public string BrowserDownloadUrl { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

}
