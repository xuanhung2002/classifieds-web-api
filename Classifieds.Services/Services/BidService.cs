using AutoMapper;
using Classifieds.Data.DTOs.BidDtos;
using Classifieds.Data.Entities;
using Classifieds.Data.Enums;
using Classifieds.Repository;
using Classifieds.Services.IServices;

namespace Classifieds.Services.Services
{
    public class BidService : IBidService
    {
        private readonly IDBRepository _repository;
        private readonly IMapper _mapper;

        public BidService(IDBRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<BidDto?> CreateBidAsync(CreateBidRequest request, Guid userId)
        {
            var post = await _repository.FindForUpdateAsync<Post>(s => s.Id == request.PostId);
            if (post == null)
            {
                throw new Exception("Post is not existed");
            }
            if(post.AuctionStatus == AuctionStatus.Closed)
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
            var entity = await _repository.AddAsync(bid);
            await _repository.UpdateAsync(post);
            if(entity != null)
            {
                return _mapper.Map<BidDto>(entity);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<BidDto>> GetBidsOfPost(Guid postId)
        {
            var bids = await _repository.GetAsync<Bid, BidDto>(s => _mapper.Map<BidDto>(s),s => s.PostId == postId); 
            return bids;

        }
    }
}
