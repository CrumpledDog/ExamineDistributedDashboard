# Examine Distributed Dashboard for Umbraco

Early stage of development

This package adds a dashboard in the developer section of Umbraco that allows the manual triggering of Examine Index Rebuilds on specific server with a specific processId (this is needed for Azure Web App Slots). 

This package was created for Load Balanced Umbraco installations where a particular index has become corrupt and requires rebuilding.

There is also a mechanism to request that all servers log the status of all Indexes by writing to log4net. I recommend using the [AzureLogger](https://github.com/CrumpledDog/Umbraco-AzureLogger) package to make it easy to see all logs from all servers.


