#
# CreareAzureSearchChain.ps1 schedules indexer run to rescan blob container
#

. $PSScriptRoot\Variables.ps1

#Scheduling indexer run to rescan blob container
$result = Invoke-WebRequest -UseBasicParsing -Uri "$searchServiceUrl/indexers/$indexerName/run?$apiVersion" -Method Post -Headers $authHeader
$result.StatusDescription
