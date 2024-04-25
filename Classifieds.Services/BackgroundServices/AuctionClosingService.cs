using Classifieds.Data;
using Classifieds.Data.Entities;
using Classifieds.Data.Models;
using Classifieds.Repository;
using Microsoft.Extensions.Hosting;

namespace Classifieds.Services.BackgroundServices
{
    public class AuctionClosingService : BackgroundService
    {
        //private readonly IDBRepository _repository;
        private readonly DataContext _context;

        public AuctionClosingService(/*IDBRepository repository,*/ DataContext context)
        {
            //_repository = repository;
            _context = context;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var currentTime = DateTime.UtcNow;
                    var nextExecutionTime = GetNextExecutionTime(currentTime); // Lấy thời điểm tiếp theo cần thực hiện kiểm tra

                    // Tính thời gian cần chờ trước khi thực hiện tiếp theo
                    var delayTime = nextExecutionTime - currentTime;
                    await Task.Delay(delayTime, stoppingToken);

                    //var autions = _repository.GetSetAsTracking<Post>(s => s.PostType == PostType.Auction && s.AuctionStatus == AuctionStatus.Opening && s.AuctionStatus == AuctionStatus.Opening && s.EndTime <= currentTime);
                    var autions = _context.Posts.Where(s => s.PostType == PostType.Auction && s.AuctionStatus == AuctionStatus.Opening && s.AuctionStatus == AuctionStatus.Opening && s.EndTime <= currentTime);

                    if (autions != null)
                    {
                        foreach (var auction in autions)
                        {
                            auction.AuctionStatus = AuctionStatus.Closed;
                            _context.Posts.Update(auction);
                            //signalR
                        }
                        await _context.SaveChangesAsync();
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

        private DateTime GetNextExecutionTime(DateTime currentTime)
        {
            // Tính thời gian tiếp theo cần kiểm tra (ví dụ: mỗi phút)
            return currentTime.AddSeconds(30);
        }
    }
}
