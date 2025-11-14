# Game Hub Api using ASP.NET Web API

- Web API to power the [ReactWeb](https://github.com/LuisMSuarez/React-GameHub) project.
- Usage of Dependency Injection
- Automated tests using XUnit and Moq.
- Caching of requests in LRU cache (in-memory) to avoid consumption costs, rate limiting, and latency on downstream apis 
- Secrets loading from Azure Keyvault
- Custom Content filter to remove explicit or inappropriate content
- Integration with OpenAI api for game content filter and game recommendations
- Integration with Azure AI cognitive services for translation of game descriptions
- CI/CD github action to run build, execute tests, build and publish container images and deploy to Azure App Service
- Dockerfile for packaging to a container image
