using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PushMeNow.AspNet5.Models
{
    public class Message : Registration
    {
        public Message()
        {
        }

        public Message(Registration registration)
            : base(registration)
        {
        }

        [JsonProperty("text")]
        [Required]
        public string Text { get; set; }
    }
}
