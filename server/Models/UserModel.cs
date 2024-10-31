namespace Server.Models {
  public class UserModel {
    public long Id { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Secret { get; set; } = "";
  }
}

// id INTEGER PRIMARY KEY AUTO_INCREMENT
// username TEXT
// password TEXT 
// secret TEXT
