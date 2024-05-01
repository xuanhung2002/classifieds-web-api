using Classifieds.Data.DTOs.BidDtos;
using Classifieds.Data.Entities;
using Classifieds.Repository;
using Classifieds.Services.IServices;

namespace Classifieds.Services.Services
{
    public class BidService : IBidService
    {
        private readonly IDBRepository _repository;

        public BidService(IDBRepository repository)
        {
            _repository = repository;
        }
        public async Task CreateBidAsync(CreateBidRequest request, Guid userId)
        {
            var post = await _repository.FindForUpdateAsync<Post>(s => s.Id == request.PostId);
            if (post == null)
            {
                throw new Exception("Post is not existed");
            }
            if(post.AuctionStatus == Data.Models.AuctionStatus.Closed)
            {
                throw new Exception("Post's auction is closed");
            }
            
            Bid bid = new Bid
            {
                PostId = request.PostId,
                BidderId = userId,
                Amount = request.Amount,
                BidTime = DateTime.UtcNow
            };
            post.CurrentAmount = bid.Amount;
            post.CurrentBidderId = userId;
            await _repository.AddAsync(bid);
            await _repository.UpdateAsync(post);
        }
    }
}
