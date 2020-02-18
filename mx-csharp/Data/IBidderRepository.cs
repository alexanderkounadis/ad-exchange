using mx_csharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mx_csharp.Data
{
    public interface IBidderRepository
    {
        List<Bidder> GetBidders();
    }
}
