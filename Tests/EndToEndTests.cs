using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests
{
    public class EndToEndTests : IClassFixture<WebApplicationFactory<mx_csharp.Startup>>
    {

        private readonly WebApplicationFactory<mx_csharp.Startup> _factory;
        private readonly Assembly _assembly;

        public EndToEndTests(WebApplicationFactory<mx_csharp.Startup> factory)
        {
            _factory = factory;
            _assembly = Assembly.GetExecutingAssembly();
        }

        [Fact]
        public void Respond_with_status_200_and_the_highest_bid_returned_the_bidders()
        {
            // GIVEN
            var client = _factory.CreateClient();
            Stream stream = _assembly.GetManifestResourceStream("Tests.Fixtures.requestFromClient.json");
            StreamReader reader = new StreamReader(stream);
            string request = reader.ReadToEnd();
            var httpContent = new StringContent(request, Encoding.UTF8, "application/json");

            // WHEN
            var actualResponse = client.PostAsync("http://localhost:5000/ads", httpContent).Result;

            // THEN
            Assert.NotNull(actualResponse);
            Assert.Equal(HttpStatusCode.OK, actualResponse.StatusCode);

            stream = _assembly.GetManifestResourceStream("Tests.Fixtures.responseWithBids.json");
            reader = new StreamReader(stream);
            string expectedResponseString = removeWhitespaces(reader.ReadToEnd());
            string actualResponseString = removeWhitespaces(actualResponse.Content.ReadAsStringAsync().Result);
            Assert.Equal(expectedResponseString, actualResponseString);
        }

        [Fact]
        public void Respond_with_status_204_and_emty_body_if_all_bidders_didnt_send_any_bids()
        {
            // GIVEN
            var client = _factory.CreateClient();
            Stream stream = _assembly.GetManifestResourceStream("Tests.Fixtures.requestFromClient.json");
            StreamReader reader = new StreamReader(stream);
            string request = reader.ReadToEnd();
            var httpContent = new StringContent(request, Encoding.UTF8, "application/json");

            // WHEN
            var actualResponse = client.PostAsync("http://localhost:5000/ads", httpContent).Result;

            // THEN
            Assert.Equal(HttpStatusCode.NoContent, actualResponse.StatusCode);
        }

        private string removeWhitespaces(string inputString)
        {
            return Regex.Replace(inputString, @"\t|\n|\r|\s+", "");
        }
       
    }
}
