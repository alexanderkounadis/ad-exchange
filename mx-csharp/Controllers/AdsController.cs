using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using mx_csharp.Data;
using mx_csharp.Models;
using Newtonsoft.Json;

namespace mx_csharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly IBidderRepository _repo;
        public AdsController(IBidderRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AdResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] AdRequest request)
        {
            List<Bidder> bidders = _repo.GetBidders();
            List<AdResponse> result = await GetBids(bidders, request);
            AdResponse temp = new AdResponse();
            temp = result.OrderBy(res => res.Bid.Price).FirstOrDefault();
            return Ok(temp);
        }

        private async Task<List<AdResponse>> GetBids(List<Bidder> bidders, AdRequest request)
        {
            List<AdResponse> returnList = new List<AdResponse>();
            var downloads = bidders.Select(bidder => TryDownloadAsync(bidder.Uri, request));
            Task<HttpResponseMessage>[] downloadTasks = downloads.Where(d => d.IsCompletedSuccessfully).ToArray();
            HttpResponseMessage[] responses = await Task.WhenAll(downloadTasks);
            foreach(HttpResponseMessage r in responses)
            {
                string stringData = r.Content.ReadAsStringAsync().Result;
                AdResponse response = JsonConvert.DeserializeObject<AdResponse>(stringData);
                returnList.Add(response);
            }
            return returnList;
        }

        private async Task<HttpResponseMessage> TryDownloadAsync(string uri, AdRequest request)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                return await httpClient.PostAsJsonAsync(uri, request);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

    }
}
