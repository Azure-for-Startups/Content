using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PushMeNow.Client.WindowsPhone.Models
{
    public class UserInfo
    {
        private string _name;
        private string _salt;

        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                int saltLength = 7;
                do
                {
                    ++saltLength;
                    _salt = Guid.NewGuid().ToString("N").Substring(0, saltLength);
                } while (AsTag().Contains("="));
            }
        }

        [JsonProperty("s")]
        public string Salt
        {
            get { return _salt; }
        }

        public string AsTag()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
        }
    }
}
