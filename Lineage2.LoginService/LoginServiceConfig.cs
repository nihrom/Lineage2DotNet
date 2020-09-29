using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.LoginService
{
    public class LoginServiceConfig
    {
        [JsonProperty(PropertyName = "host")]
        public string Host { get; set; }

        [JsonProperty(PropertyName = "port")]
        public int LoginPort { get; set; }

        [JsonProperty(PropertyName = "gsport")]
        public int GsPort { get; set; }

        [JsonProperty(PropertyName = "autocreate")]
        public bool AutoCreate { get; set; }
    }
}
