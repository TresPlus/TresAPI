using System.ComponentModel.DataAnnotations;

namespace Entities
{
  public class Model3D
  {
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    // Dosya meta
    public string OriginalFileName { get; set; } = null!;
    public string StoragePath { get; set; } = null!;
    public string ContentType { get; set; } = null!; // mime type
    public string Format { get; set; } = null!; // glb, obj, usdz, fbx vb.
    public long FileSize { get; set; }
    public string? ChecksumSha256 { get; set; }

    // Gizlilik / paylaşım
    public bool IsPublic { get; set; } = false;
    public bool IsUnlisted { get; set; } = false;

    // Versiyonlama
    public int CurrentVersionNumber { get; set; } = 1;

    // Denormalize edilmiş metrikler (performans için)
    public long ViewCount { get; set; } = 0;
    public long DownloadCount { get; set; } = 0;
    public long LikeCount { get; set; } = 0;
    public long CommentCount { get; set; } = 0;

    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // İlişkiler
    public ICollection<ModelVersion> Versions { get; set; } = new List<ModelVersion>();
    public ICollection<ModelConvertedFile> ConvertedFiles { get; set; } = new List<ModelConvertedFile>();
    public ICollection<ModelTag> ModelTags { get; set; } = new List<ModelTag>();
    public ModelSettings? Settings { get; set; }

    // Sosyal / paylaşım
    public ICollection<ModelLike> Likes { get; set; } = new List<ModelLike>();
    public ICollection<ModelComment> Comments { get; set; } = new List<ModelComment>();
    public ICollection<ModelShareLink> ShareLinks { get; set; } = new List<ModelShareLink>();
    public ICollection<ModelReport> Reports { get; set; } = new List<ModelReport>();

    [Timestamp]
    public byte[]? RowVersion { get; set; }
  }
}
