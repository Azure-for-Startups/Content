using System;
using Newtonsoft.Json;

namespace PushMeNow.AspNet5.Models
{
    public class UserInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
