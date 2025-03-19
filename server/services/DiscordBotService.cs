using DSharpPlus;

namespace DStorage.Services {
  public class DiscordBotService : BackgroundService {

    public DiscordClient? discordClient { get; private set; }
    private readonly IConfiguration configuration;

    public DiscordBotService(IConfiguration configuration) {
      this.configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
      var discordClientBuilder = DiscordClientBuilder.CreateDefault(configuration.GetConnectionString("DiscordToken") ?? "", DiscordIntents.All);
      this.discordClient = discordClientBuilder.Build();

      // Will most likely throw if token is invalid 
      await this.discordClient.ConnectAsync();
      await Task.Delay(-1, cancellationToken); 
    }
  }
}