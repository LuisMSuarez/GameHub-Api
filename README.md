# Game Hub Api using ASP.NET Web API

- Web API to power the [ReactWeb](https://github.com/LuisMSuarez/React-GameHub) project.
- Usage of Dependency Injection
- Automated tests using XUnit and Moq.
- Caching of requests in LRU cache (in-memory) 
- Secrets loading from Azure Keyvault
- Custom Content filter to remove explicit or inappropriate content
- RAWG api provider using HttpClient
- Integration with Azure AI cognitive services for translation of game descriptions
- CI/CD github action to run build, execute tests, build and publish container images and deploy to Azure App Service
