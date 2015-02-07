using System;
using System.Collections;
using System.IO;
using System.Net;
using DD.CiBuildLight.Netduino.Model;
using Json.NETMF;

namespace DD.CiBuildLight.Netduino.IO
{
    public class BuildStatusManager
    {
        public static BuildStatus GetBuildStatus()
        {
            try
            {
                var request =
                    (HttpWebRequest) WebRequest.Create("http://okcsharpbuildwebapi.azurewebsites.net/api/buildstatus");

                request.Accept = "application/json";
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    using (var dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        using (var reader = new StreamReader(dataStream))
                        {
                            // Read the content. 
                            var responseFromServer = reader.ReadToEnd();
                            var jsonResponse = JsonSerializer.DeserializeString(responseFromServer) as Hashtable;
                            if (jsonResponse != null)
                            {
                                return new BuildStatus
                                {
                                    Status = jsonResponse["status"] is bool && (bool) jsonResponse["status"],
                                    BuildDef = jsonResponse["builddef"] as string,
                                    CommunicationError =
                                        jsonResponse["communicationerror"] is bool &&
                                        (bool) jsonResponse["communicationerror"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return new BuildStatus
            {
                Status = false,
                CommunicationError = true
            };
        }
    }
}