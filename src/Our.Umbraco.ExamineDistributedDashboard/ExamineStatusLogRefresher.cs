using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Web.WebServices;

namespace Our.Umbraco.ExamineDistributedDashboard
{
    // this refresher will log the status and count of every indexer and can also be used to trigger a rebuild of a specific index on a specifc server with a specific processid (for Azure slot support)
    public class ExamineStatusLogRefresher : JsonCacheRefresherBase<ExamineStatusLogRefresher>
    {
        public static Guid Id => new Guid("c70924f5-220d-48f6-8184-97ad7c98a50f");

        protected override ExamineStatusLogRefresher Instance => this;

        public override Guid UniqueIdentifier => Id;

        public override string Name => "ExamineStatusLogRefresher";

        public override void RefreshAll()
        {
            // method to get all indexes is already used by the Examine dashboard so we use that instead of reinventing the wheel
            var allIndexers = new ExamineManagementApiController().GetIndexerDetails();
            foreach (var index in allIndexers)
            {
                LogHelper.Info<ExamineStatusLogRefresher>($"[{Environment.MachineName} - {System.Diagnostics.Process.GetCurrentProcess().Id}] Index: {index.Name} - Healthy: {index.IsHealthy} - Count: {index.DocumentCount}");
            }
        }
    }
}
