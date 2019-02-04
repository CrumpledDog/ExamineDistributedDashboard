using System;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Examine.Providers;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Web.Search;
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

        private ExamineIndexerModel CreateModel(BaseIndexProvider indexer)
        {
            var indexerModel = new ExamineIndexerModel()
            {
                IndexCriteria = indexer.IndexerData,
                Name = indexer.Name
            };

            var props = TypeHelper.CachedDiscoverableProperties(indexer.GetType(), mustWrite: false)
                                  //ignore these properties
                                  .Where(x => new[] { "IndexerData", "Description", "WorkingFolder" }.InvariantContains(x.Name) == false)
                                  .OrderBy(x => x.Name);

            foreach (var p in props)
            {
                var val = p.GetValue(indexer, null);
                if (val == null)
                {
                    // Do not warn for new new attribute that is optional
                    if (string.Equals(p.Name, "DirectoryFactory", StringComparison.InvariantCultureIgnoreCase) == false)
                        LogHelper.Warn<ExamineManagementApiController>("Property value was null when setting up property on indexer: " + indexer.Name + " property: " + p.Name);

                    val = string.Empty;
                }
                indexerModel.ProviderProperties.Add(p.Name, val.ToString());
            }

            var luceneIndexer = indexer as LuceneIndexer;
            if (luceneIndexer != null)
            {
                indexerModel.IsLuceneIndex = true;

                if (luceneIndexer.IndexExists())
                {
                    Exception indexError;
                    indexerModel.IsHealthy = luceneIndexer.IsHealthy(out indexError);

                    if (indexerModel.IsHealthy == false)
                    {
                        //we cannot continue at this point
                        indexerModel.Error = indexError.ToString();
                        return indexerModel;
                    }

                    indexerModel.DocumentCount = luceneIndexer.GetIndexDocumentCount();
                    indexerModel.FieldCount = luceneIndexer.GetIndexFieldCount();
                    indexerModel.IsOptimized = luceneIndexer.IsIndexOptimized();
                    indexerModel.DeletionCount = luceneIndexer.GetDeletedDocumentsCount();
                }
                else
                {
                    indexerModel.DocumentCount = 0;
                    indexerModel.FieldCount = 0;
                    indexerModel.IsOptimized = true;
                    indexerModel.DeletionCount = 0;
                }
            }
            return indexerModel;
        }

        public override void RefreshAll()
        {
            // method to get all indexes is already used by the Examine dashboard so we use that instead of reinventing the wheel

            var allIndexers = ExamineManager.Instance.IndexProviderCollection.Select(CreateModel).OrderBy(x =>
            {
                //order by name , but strip the "Indexer" from the end if it exists
                return x.Name.TrimEnd("Indexer");
            });

            foreach (var index in allIndexers)
            {
                LogHelper.Info<ExamineStatusLogRefresher>($"[{Environment.MachineName} - {System.Diagnostics.Process.GetCurrentProcess().Id}] Index: {index.Name} - Healthy: {index.IsHealthy} - Count: {index.DocumentCount}");
            }
        }
    }
}
