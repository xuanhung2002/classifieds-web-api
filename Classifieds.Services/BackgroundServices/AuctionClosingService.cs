using Classifieds.Data;
using Classifieds.Data.Entities;
using Classifieds.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1.Esf;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Classifieds.Services.BackgroundServices
{
    public class AuctionClosingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AuctionClosingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
       {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                        var currentTime = DateTime.UtcNow;
                        Console.WriteLine("Execute auto closing, current time =>> " + currentTime);

                        // Tính toán thời gian cho phút tiếp theo
                        var nextMinute = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute + 1, 0);

                        // Tính thời gian chờ để bắt đầu từ phút tiếp theo
                        var delayTime = nextMinute - currentTime;
                        await Task.Delay(delayTime, stoppingToken);

                        // Thực hiện công việc kiểm tra và đóng các cuộc đấu giá
                        var auctionsToClose = await dbContext.Posts
                            .Where(s => s.PostType == PostType.Auction &&
                                        s.AuctionStatus == AuctionStatus.Opening &&
                                        s.EndTime <= nextMinute)
                            .ToListAsync(stoppingToken);

                        if (auctionsToClose.Any())
                        {
                            foreach (var auction in auctionsToClose)
                            {
                                auction.AuctionStatus = AuctionStatus.Closed;
                                dbContext.Posts.Update(auction);
                                // Cần thêm logic để thông báo cho người dùng (ví dụ: sử dụng SignalR)
                            }

                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Task was canceled, stop the service
                    break;
                }
                catch (Exception ex)
                {
                    // Log and continue
                }
            }
        }
    }
}
