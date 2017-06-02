using System;
using System.Threading.Tasks;
using Examine;
using Newtonsoft.Json;
using Our.Umbraco.ExamineDistributedDashboard.Models;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Our.Umbraco.ExamineDistributedDashboard
{
    public class ExamineDistributedDashboardEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            CacheRefresherBase<ExamineStatusLogRefresher>.CacheUpdated += CacheRefresherBase_ExamineStatusLogRefresher_CacheUpdated;

        }
        private static void CacheRefresherBase_ExamineStatusLogRefresher_CacheUpdated(ExamineStatusLogRefresher sender,
            CacheRefresherEventArgs e)
        {
            var rawPayLoad = (string)e.MessageObject;
            var payload = JsonConvert.DeserializeObject<RebuildRequest>(rawPayLoad);

            if (System.Diagnostics.Process.GetCurrentProcess().Id.ToString().InvariantEquals(payload.TargetProcessId) && Environment.MachineName.InvariantEquals(payload.TargetMachineName))
            {
                // this is the target machine
                Task.Run(() =>
                {
                    ExamineManager.Instance.IndexProviderCollection[payload.TargetIndexName].RebuildIndex();
                    LogHelper.Info<ExamineDistributedDashboardEvents>($"[{Environment.MachineName} - {System.Diagnostics.Process.GetCurrentProcess().Id}] Running:PostRebuildIndex({payload.TargetIndexName}) Async - RequestId: {payload.RequestId}");
                });
            }
        }
    }
}
