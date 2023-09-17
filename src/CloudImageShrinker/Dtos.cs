using Newtonsoft.Json;

namespace CloudImageShrinkerUWP
{
    public class RootObject
    {
        public string _odata_context { get; set; }
        public int _odata_count { get; set; }
        public string _odata_nextLink { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        [JsonProperty(PropertyName = "@microsoft.graph.downloadUrl")]
        public string DownloadUrl { get; set; }
        public string createdDateTime { get; set; }
        public string cTag { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public string lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public string webUrl { get; set; }
        public Reactions reactions { get; set; }
        public CreatedBy createdBy { get; set; }
        // public LastModifiedBy lastModifiedBy { get; set; }
        public ParentReference parentReference { get; set; }
        public File file { get; set; }
        public FileSystemInfo fileSystemInfo { get; set; }
        public Image image { get; set; }
        public Photo photo { get; set; }
        public Location location { get; set; }
        public PendingOperations pendingOperations { get; set; }
        public Video video { get; set; }
        public Thumbnails[] thumbnails { get; set; }

    }

    public class Reactions
    {
        public int commentCount { get; set; }
    }

    public class CreatedBy
    {
        // public Application application { get; set; }
        public User user { get; set; }
        public MobileMediaBackupKey mobileMediaBackupKey { get; set; }
    }

    // public class Application
    // {
    //     public string displayName { get; set; }
    //     public string id { get; set; }
    // }

    public class User
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }

    public class MobileMediaBackupKey
    {
        public string _odata_type { get; set; }
        public string id { get; set; }
    }

    // public class LastModifiedBy
    // {
    //     public Application application { get; set; }
    //     public User user { get; set; }
    //     public MobileMediaBackupKey1 mobileMediaBackupKey { get; set; }
    // }


    

    public class Thumbnails
    {
        public string id { get; set; }
        public Thumbnail large { get; set; }
        public Thumbnail medium { get; set; }
        public Thumbnail small { get; set; }
    }

    public class Thumbnail
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    
    public class MobileMediaBackupKey1
    {
        public string _odata_type { get; set; }
        public string id { get; set; }
    }

    public class ParentReference
    {
        public string driveId { get; set; }
        public string driveType { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
    }

    public class File
    {
        public string mimeType { get; set; }
        public Hashes hashes { get; set; }
    }

    public class Hashes
    {
        public string quickXorHash { get; set; }
        public string sha1Hash { get; set; }
        public string sha256Hash { get; set; }
    }

    public class FileSystemInfo
    {
        public string createdDateTime { get; set; }
        public string lastModifiedDateTime { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Photo
    {
        public string cameraMake { get; set; }
        public string cameraModel { get; set; }
        public double exposureDenominator { get; set; }
        public double exposureNumerator { get; set; }
        public double focalLength { get; set; }
        public double fNumber { get; set; }
        public int iso { get; set; }
        public int orientation { get; set; }
        public string takenDateTime { get; set; }
    }

    public class Location
    {
        public double altitude { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class PendingOperations
    {
        public PendingContentUpdate pendingContentUpdate { get; set; }
    }

    public class PendingContentUpdate
    {
        public string queuedDateTime { get; set; }
    }

    public class Video
    {
        public int bitrate { get; set; }
        public int duration { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int audioBitsPerSample { get; set; }
        public int audioChannels { get; set; }
        public string audioFormat { get; set; }
        public int audioSamplesPerSecond { get; set; }
        public string fourCC { get; set; }
        public double frameRate { get; set; }
    }
}