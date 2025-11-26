16414fc7-90e7-46e0-b1f8-dc80f48bb456 is the managed identity of the app service running in .net
00000000-0000-0000-0000-000000000002 is the roleid for Cosmos DB Built-in Data Contributor data plane role

this role is not visible in the azure portal and must be set using the azure cli

winget install --id Microsoft.AzureCLI --exact

az login
az cosmosdb sql role assignment create --account-name document-storage --resource-group Common --scope "/" --principal-id 16414fc7-90e7-46e0-b1f8-dc80f48bb456 --role-definition-id 00000000-0000-0000-0000-000000000002
az cosmosdb sql role assignment list --account-name document-storage --resource-group Common
