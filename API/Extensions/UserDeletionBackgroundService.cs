using Business.Abstract;
using Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace API.Extensions
{
    public class UserDeletionBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UserDeletionBackgroundService> _logger;

        public UserDeletionBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<UserDeletionBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UserDeletionBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    var thresholdDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-15);

                    var usersToDelete = (await userService.GetAllUsers())
                        .Where(u => u.DeletionRequestedAt != null
                        && u.DeletionRequestedAt <= thresholdDate)
                        .ToList();

                    foreach (var user in usersToDelete)
                    {
                        string archiveFolder = Path.Combine("Private", "Archives");
                        if (!Directory.Exists(archiveFolder))
                            Directory.CreateDirectory(archiveFolder);

                        string fileName = $"{user.Id}_{user.DeletionRequestedAt.Value:yyyyMMdd}.json";
                        string filePath = Path.Combine(archiveFolder, fileName);

                        var jsonData = JsonSerializer.Serialize(user);
                        await File.WriteAllTextAsync(filePath, jsonData, stoppingToken);

                        var result = await userService.DeleteUser(user);
                        if (result.Succeeded)
                            _logger.LogInformation($"User {user.Id} deleted and archived.");
                        else
                            _logger.LogWarning($"Failed to delete user {user.Id}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in UserDeletionBackgroundService");
                }

                // Test için 25 ms değil, gerçek kullanımda örneğin 1 gün
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }

            _logger.LogInformation("UserDeletionBackgroundService stopped.");
        }

    }
}
