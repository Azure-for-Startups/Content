#
# CreareAzureSearchChain.ps1 requests indexer status
#

. $PSScriptRoot\Variables.ps1

#Requesting indexer status
$result = Invoke-WebRequest -UseBasicParsing -Uri "$searchServiceUrl/indexers/$indexerName/status?$apiVersion" -Method Get -Headers $authHeader
$result.Content | ConvertFrom-Json | ConvertTo-Json
