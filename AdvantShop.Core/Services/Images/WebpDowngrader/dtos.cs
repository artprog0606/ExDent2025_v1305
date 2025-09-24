using System;
using Newtonsoft.Json;

namespace AdvantShop.Images.WebpDowngrader
{
    public class RequestDto
    {
        [JsonProperty("imageUri")]
        public Uri ImageUri { get; set; }
    }
    
    public class ResponseDto
    {
        public bool Ok { get; set; }
        public byte[] Image { get; set; }
        public string Cause { get; set; }
    }
}