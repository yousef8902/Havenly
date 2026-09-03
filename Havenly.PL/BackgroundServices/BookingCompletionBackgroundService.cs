using System;
using System.Threading;
using System.Threading.Tasks;
using Havenly.BLL.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Havenly.PL.BackgroundServices
{
    public class BookingCompletionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingCompletionBackgroundService> _logger;

        public BookingCompletionBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<BookingCompletionBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BookingCompletionBackgroundService has started.");

            // Wait a few seconds after startup before the first run
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    var completedCount = await bookingService.ProcessAutomaticCheckoutsAsync();
                    if (completedCount > 0)
                    {
                        _logger.LogInformation("Automatically completed {Count} booking(s) whose checkout date passed, and sent review invitation emails.", completedCount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while executing BookingCompletionBackgroundService.");
                }

                // Check every 3 minutes
                await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
            }
        }
    }
}
