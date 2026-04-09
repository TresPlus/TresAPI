using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace API.Extensions
{
  public class FileStorage
  {
    private readonly IWebHostEnvironment _env;

    public FileStorage(IWebHostEnvironment env)
    {
      _env = env;
    }

    // ------------------------------------------------------
    // GLOBAL SAVE METHOD (her türlü dosya için)
    // ------------------------------------------------------
    public async Task<string> SaveFileAsync(
        IFormFile file,
        string[] pathSegments,
        bool generateGuidName = true)
    {
      var root = _env.WebRootPath;

      // pathSegments → ["Images", "Profiles", "2025", "January"]
      var folder = Path.Combine(root, Path.Combine(pathSegments));

      if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

      string fileName = generateGuidName
          ? Guid.NewGuid().ToString() + Path.GetExtension(file.FileName)
          : file.FileName;

      var fullPath = Path.Combine(folder, fileName);

      using (var stream = new FileStream(fullPath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      // web yolu geri dönülüyor
      return "/" + string.Join("/", pathSegments) + "/" + fileName; 
    }

    // ------------------------------------------------------
    // PROFIL RESMI KAYDETME - RESIZE + PAD
    // ------------------------------------------------------
    public async Task<string> SaveProfilePicture(IFormFile file)
    {
      string[] path = new[] { "Images", "Profiles" };

      var root = _env.WebRootPath;
      var folder = Path.Combine(root, Path.Combine(path));

      if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

      var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
      var fullPath = Path.Combine(folder, fileName);

      using (var image = await Image.LoadAsync(file.OpenReadStream()))
      {
        image.Mutate(x => x.Resize(new ResizeOptions
        {
          Size = new Size(96, 96),
          Mode = ResizeMode.Pad
        }));

        await image.SaveAsync(fullPath);
      }

      return "/" + string.Join("/", path) + "/" + fileName;
    }
  }
}
