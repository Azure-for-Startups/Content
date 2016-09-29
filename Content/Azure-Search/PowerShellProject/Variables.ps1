#
# Variables.ps1 defines variables used by another scripts
#

$datasourceName = "logset-ds"
$indexName = "logset-idx"
$indexerName = "logset-idxr"
$searchServiceUrl = "_SERVICE_URL_"
$searchAdminKey = "_ADMIN_KEY_"
$blobContainer = "_CONTAINER_NAME_"
$storageAccountName = "_ACCOUNT_NAME_"
$storageAccountKey = "_ACCOUNT_KEY_"
$pathPrefix = ""

$authHeader = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$authHeader.Add("api-key", $searchAdminKey)
$apiVersion = "api-version=2015-02-28-Preview"
