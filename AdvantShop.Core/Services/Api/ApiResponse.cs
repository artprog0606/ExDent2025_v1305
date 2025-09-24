using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Api
{
    public interface IApiResponse
    {
    }

    public class BaseApiResponse : IApiResponse
    {
        public BaseApiResponse()
        {
        }

        [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
        public string Errors { get; set; }

        [JsonProperty("warnings", NullValueHandling = NullValueHandling.Ignore)]
        public string Warnings { get; set; }
    }

    public class ApiResponse : BaseApiResponse
    {
        public ApiResponse(){}
        public ApiResponse(ApiStatus status, string errors)
        {
            Status = status;
            Errors = errors;
        }

        [JsonProperty("status"), JsonConverter(typeof(StringEnumConverter))]
        public ApiStatus Status { get; set; }
    }

    public class ApiError : ApiResponse
    {
        public ApiError(string errors) : base(ApiStatus.Error, errors)
        {
        }
    }

    public enum ApiStatus
    {
        [EnumMember(Value = "ok")]
        Ok,

        [EnumMember(Value = "error")]
        Error
    }
}