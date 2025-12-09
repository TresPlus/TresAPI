namespace API.Dtos
{
  public class RegisterDto
  {
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public IFormFile ProfilePicture { get; set; }
  }

  public class LoginDto
  {
    public string EmailOrUserName { get; set; }
    public string Password { get; set; }
  }
}
