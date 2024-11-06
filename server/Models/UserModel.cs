namespace Server.Models {
  public class UserModel {
    public long Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Secret { get; set; } = "";
  
    public ICollection<FileModel> Files { get; } = new List<FileModel>();
  }
}

