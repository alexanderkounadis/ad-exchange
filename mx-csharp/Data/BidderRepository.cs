using mx_csharp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace mx_csharp.Data
{
    public class BidderRepository : IBidderRepository
    {
        public List<Bidder> GetBidders()
        {
            using (StreamReader sr = new StreamReader("bidders.json"))
            {
                string json = sr.ReadToEnd();
                List<Bidder> bidders = JsonConvert.DeserializeObject<List<Bidder>>(json);
                return bidders;
            }  
        }
    }
}
