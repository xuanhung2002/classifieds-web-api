using Classifieds.Data.DTOs.WatchListDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Services.IServices
{
    public interface IWatchListService
    {
        Task AddWatchPost(AddWatchPostDto dto);
    }
}
