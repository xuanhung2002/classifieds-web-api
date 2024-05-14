using Classifieds.Data.DTOs.BidDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.IServices
{
    public interface IBidService
    {
        Task<BidDto?> CreateBidAsync(CreateBidRequest request, Guid userId);
        Task<List<BidDto>> GetBidsOfPost(Guid postId);
    }
}
