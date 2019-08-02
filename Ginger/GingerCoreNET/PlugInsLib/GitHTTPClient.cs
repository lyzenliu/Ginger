#region License
/*
Copyright © 2014-2019 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Amdocs.Ginger.CoreNET.PlugInsLib
{
    class GitHTTPClient
    {

        public static async Task<string> GetResponseString(string url)
        {
            using (var client = new HttpClient())
            {
                // Simulate a browser header                
                //client.DefaultRequestHeaders.Add("User-Agent", GingerUtils.OSHelper.Current.UserAgent);
                client.DefaultRequestHeaders.Add("User-Agent", "Ginger-App");
                

                var result = client.GetAsync(url).Result;

                if (result.IsSuccessStatusCode)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    return json;
                }
                else
                {
                    return "Error: " + result.ReasonPhrase;
                }
            }
        }

        internal static T GetJSON<T>(string url)
        {
            T t = default(T);            
            string packagesjson = GetResponseString(url).Result;
            if (packagesjson.Contains("Error: Forbidden"))
            {
                return t;
            }
            T list = JsonConvert.DeserializeObject<T>(packagesjson);
            return list;
        }
    }
}
