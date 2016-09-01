#
# CreareAzureSearchChain.ps1 requests index statistics
#

. $PSScriptRoot\Variables.ps1

#Requesting index statistics
$result = Invoke-WebRequest -UseBasicParsing -Uri "$searchServiceUrl/indexes/$indexName/stats?$apiVersion" -Method Get -Headers $authHeader
$result.Content | ConvertFrom-Json | ConvertTo-Json
