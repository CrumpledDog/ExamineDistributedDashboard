using System;
using System.Web.Http;
using Newtonsoft.Json;
using Our.Umbraco.ExamineDistributedDashboard.Models;
using Umbraco.Web.Cache;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.ExamineDistributedDashboard.Controllers
{
    public class ExamineDistributedDashboardController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public string ServersCheckIn()
        {
            DistributedCache.Instance.RefreshAll(ExamineStatusLogRefresher.Id);
            return $"Exmaine Cache message sent to all servers";
        }

        [HttpPost]
        [UmbracoAuthorize]
        public string SendRebuildRequest(string targetMachineName, string targetProcessId, string targetIndexName)
        {
            var thisRequestId = Guid.NewGuid();

            var thisRequest = new RebuildRequest()
            {
                RequestId = thisRequestId,
                TargetProcessId = targetProcessId,
                TargetIndexName = targetIndexName,
                TargetMachineName = targetMachineName
            };

            var json = JsonConvert.SerializeObject(thisRequest);

            DistributedCache.Instance.RefreshByJson(ExamineStatusLogRefresher.Id, json);
            return $"Rebuild request sent with RequestId: {thisRequestId}";
        }
    }
}
