using System;
using Newtonsoft.Json;

namespace Our.Umbraco.ExamineDistributedDashboard.Models
{
    public class RebuildRequest
    {
        [JsonProperty("requestId")]
        public Guid  RequestId { get; set; }

        [JsonProperty("targetMachineName")]
        public string TargetMachineName { get; set; }

        [JsonProperty("targetProcessId")]
        public string TargetProcessId { get; set; }

        [JsonProperty("targetIndexName")]
        public string TargetIndexName { get; set; }
    }
}
