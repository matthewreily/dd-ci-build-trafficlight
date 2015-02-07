using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DD.CiBuildLight.Web.Models;
using Newtonsoft.Json;

namespace DD.CiBuildLight.Web.Controllers
{
    public class BuildStatusController : ApiController
    {
        #region Public Methods

        // GET: api/BuildStatus
        public async Task<BuildStatus> Get()
        {
            _baseUrl = String.Format(_baseUrl, Account);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                //Set alternate credentials
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", AltUsername, AltPassword))));


                //Get a list of build definitions
                var responseBody = await GetAsync(client, _baseUrl + "build/builds");
                var response = JsonConvert.DeserializeObject<Rootobject>(responseBody);

                var buildStatus = new BuildStatus();
                if (response != null && response.value != null && response.value.Any())
                {
                    var builds = response.value.OrderByDescending(v => v.finishTime);
                    buildStatus.status = builds.First().status != "failed";
                    buildStatus.builddef = builds.First().definition.name;
                }
                else
                {
                    buildStatus.communicationerror = true;
                }

                return buildStatus;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static async Task<String> GetAsync(HttpClient client, String apiUrl)
        {
            var responseBody = String.Empty;

            try
            {
                using (var response = client.GetAsync(apiUrl + ApiVersion).Result)
                {
                    response.EnsureSuccessStatusCode();
                    responseBody = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return responseBody;
        }

        #endregion Private Methods

        #region Private Fields

        //Your visual studio account name
        private const String Account = "{account}";
        //Api version query parameter
        private const String ApiVersion = "?api-version=1.0";

        private static String _baseUrl =
            "https://okcsharp.visualstudio.com/DefaultCollection/OKCSharp.NetduinoBuildLight/_apis/";

        #endregion Private Fields

        // Get the alternate credentials that you'll use to access the Visual Studio Online account.

        #region secret

        private const String AltPassword = "Jesus1114";
        private const String AltUsername = "matthewreily";

        #endregion
    }
}