namespace NetLynk
{
    using Types;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Timers;

    public class NetLynk
    {
        private Timer projectUpdateTimer;
        private Project cachedProject;

        public NetLynk(string apiAddress, int port, string authToken)
        {
            this.ApiAddress = apiAddress;
            this.ApiPort = port;
            this.Token = authToken;
        }

        public event EventHandler ProjectUpdated;

        /// <summary>
        /// Gets Api Address
        /// </summary>
        public string ApiAddress { get; }

        /// <summary>
        /// Gets Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets Api Port
        /// </summary>
        public int ApiPort { get; set; }

        public string ApiBase => $"{ApiAddress}:{ApiPort}/{Token}";

        /// <summary>
        /// Gets Widgets
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetAnalogPinValue(int Pin)
        {
            var str = await RunWebRequest($"get/D{Pin}", "GET");
            var value = JsonConvert.DeserializeObject(str);

            return 0;
        }

        /// <summary>
        /// Gets Widgets
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetDigitalPinValue(int Pin)
        {
            var str = await RunWebRequest($"get/D{Pin}", "GET");
            var value = JsonConvert.DeserializeObject<JArray>(str);

            var result = false; 

            if(value.First.Value<string>() == "1")
            {
                result = true;
            }
            else if(value.First.Value<string>() == "0")
            {
                result = false;
            }
            else
            {
                throw new Exception("Not a valid value");
            }

            return result;
        }

        /// <summary>
        /// Sets Widgets
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetDigitalPinValue(int Pin, bool val)
        {
            var dict = new Dictionary<string, string> { { "value", val ? "1" : "0" } };
            var str = await RunWebRequest($"update/D{Pin}", "GET", dict);

            return true;
        }

        /// <summary>
        /// Gets Project 
        /// </summary>
        /// <returns></returns>
        public async Task<Project> GetProject(bool doContinuousUpdate = true, int updateSeconds = 5)
        {
            if (doContinuousUpdate)
            {
                this.cachedProject = await UpdateProject();
                this.StartUpdates(updateSeconds);
            }
            else
            {
                return await UpdateProject();
            }

            return this.cachedProject;
        }

        /// <summary>
        /// Cancels update interval
        /// </summary>
        public void CancelUpdates()
        {
            this.projectUpdateTimer.Stop();
            this.projectUpdateTimer.Dispose();
        }

        /// <summary>
        /// Starts update interval with given seconds
        /// </summary>
        /// <param name="seconds"></param>
        public void StartUpdates(int seconds)
        {
            if (this.projectUpdateTimer != null)
            {
                this.projectUpdateTimer.Stop();
                this.projectUpdateTimer.Dispose();
                this.projectUpdateTimer = null;
            }

            this.projectUpdateTimer = new Timer(seconds * 1000);
            this.projectUpdateTimer.Elapsed += (a, e) => {
                Task.Run(async () =>
                {
                    var project = await UpdateProject();
                    var projectStr = JsonConvert.SerializeObject(project);
                    var cachedProjectStr = JsonConvert.SerializeObject(cachedProject);

                    if(cachedProjectStr != projectStr)
                    {
                        this.cachedProject.Update(project);
                        this.ProjectUpdated?.Invoke(this, new EventArgs());
                    }
                });
            };

            this.projectUpdateTimer.Start();
        }

        /// <summary>
        /// Pauses updates
        /// </summary>
        public void PauseUpdates()
        {
            this.projectUpdateTimer.Stop();
        }

        private async Task<Project> UpdateProject()
        {
            var projectStr = await RunWebRequest("project", "GET");
            var project = JsonConvert.DeserializeObject<Project>(projectStr);
            foreach (var widget in project.Widgets)
            {
                widget.Project = project;
                widget.Lynk = this;
            }

            return project;
        }

        private async Task<string> RunWebRequest(string action, string method, Dictionary<string, string> parameters = null)
        {
            var queryString = parameters != null ? GenerateQueryString(parameters) : string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{ApiBase}/{action}{queryString}");
            request.Method = method;
            request.ContentType = "application/json";

            //Get the response
            WebResponse wr = await request.GetResponseAsync();
            Stream receiveStream = wr.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            var content = reader.ReadToEnd();

            this.CheckResult(content);
            return content;
        }

        private void CheckResult(string str)
        {
            if (str.Contains("Requested pin"))
            {
                throw new Exception(str);
            }

            if (str.Contains("Wrong pin"))
            {
                throw new Exception(str);
            }

            if (str.Contains("Invalid token."))
            {
                throw new Exception(str);
            }
        }

        private string GenerateQueryString(Dictionary<string, string> dict)
        {
            return string.Format("?{0}",
                        string.Join("&",
                            dict.Select(kvp =>
                                string.Format("{0}={1}", kvp.Key, kvp.Value))));
        }
    }
}
