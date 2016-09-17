using System;
using System.Text;
using Newtonsoft.Json;

namespace PushMeNow.AspNet5.Models
{
    enum RegistrationType
    {
        Windows,
        Apple,
        Gcm,
    }

    public class Registration
    {
        public Registration()
        { }

        public Registration(Registration other)
        {
            RegistrationType = other.RegistrationType;
            Tags = other.Tags;
        }

        [JsonProperty("registration_type")]
        public string RegistrationType { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        public string Name
        {
            get
            {
                try
                {
                    return
                        JsonConvert.DeserializeObject<UserInfo>(Encoding.UTF8.GetString(Convert.FromBase64String(Tags)))
                            .Name;
                }
                catch
                {
                    return "Unknown";
                }
            }
        }
    }
}
