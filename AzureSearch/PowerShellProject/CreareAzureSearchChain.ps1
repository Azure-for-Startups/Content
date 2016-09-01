#
# CreareAzureSearchChain.ps1 creates or recreates Azure Blob Data Source, Index and Indexer
#

. $PSScriptRoot\Variables.ps1

#Deleting indexer, datasource and index if already exists. Ignoring errors
try { $result = Invoke-WebRequest -UseBasicParsing -Uri "$searchServiceUrl/indexers/$indexerName\?$apiVersion" -Method Delete -Headers $authHeader } catch {}
try { $result = Invoke-WebRequest -UseBasicParsing -Uri "$searchServiceUrl/datasources/$datasourceName\?$apiVersion" -Method Delete -Headers $authHeader } catch {}
try { $result = Invoke-WebRequest -UseBasicParsing -Uri "$searchServiceUrl/indexes/$indexName\?$apiVersion" -Method Delete -Headers $authHeader } catch {}

#Creating data source
"Blob data source ---"
$dataSourceBody = @"
{
    "name" : "$datasourceName",
    "type" : "azureblob",
    credentials : { "connectionString" : "DefaultEndpointsProtocol=https;AccountName=$storageAccountName;AccountKey=$storageAccountKey" },
    container : { "name" : "$blobContainer", query : "$pathPrefix" }
}
"@
$result = Invoke-WebRequest -UseBasicParsing -Uri "$searchServiceUrl/datasources?$apiVersion" -Method Post -Body $dataSourceBody -ContentType "application/json" -Headers $authHeader
$result.StatusDescription

#Creating index
"Index ---"
$indexBody = @"
{
   "name":"$indexName",
   "fields":[
      {
         "name":"id",
         "type":"Edm.String",
         "key":true,
         "searchable":false
      },
      {
         "name":"path",
         "type":"Edm.String",
         "key":false,
         "searchable":true,
         "filterable":true,
         "sortable":true,
         "facetable":true
      },
      {
         "name":"content",
         "type":"Edm.String",
         "searchable":true,
         "filterable":false,
         "sortable":false,
         "facetable":false,
         "analyzer":"my_analyzer",
         "retrievable": false
      },
      {
         "name":"modified",
         "type":"Edm.DateTimeOffset",
         "key":false,
         "searchable":false,
         "filterable":true,
         "sortable":true,
         "facetable":true
      }
   ],
   "analyzers":[
      {
         "name":"my_analyzer",
         "@odata.type":"#Microsoft.Azure.Search.CustomAnalyzer",
         "tokenizer":"my_tokenizer",
         "tokenFilters":[
            "my_camelCase",
            "lowercase"
         ]
      }
   ],
   "tokenizers":[
      {
         "name":"my_tokenizer",
         "@odata.type":"#Microsoft.Azure.Search.PatternTokenizer",
         "pattern":"([a-zA-Z0-9]+)",
         "group": 1
      }
   ],
   "tokenFilters":[
      {
         "name":"my_camelCase",
         "@odata.type":"#Microsoft.Azure.Search.PatternCaptureTokenFilter",
         "patterns":[
            "(.+?((?<!^)(?=[A-Z][a-z])|$))"
         ],
         "preserveOriginal":true
      }
   ],
   "corsOptions":{
      "allowedOrigins":[
         "*"
      ]
   }
}
"@
$result = Invoke-WebRequest -UseBasicParsing -Uri "$searchServiceUrl/indexes?$apiVersion" -Method Post -Body $indexBody -ContentType "application/json" -Headers $authHeader
$result.StatusDescription

#Creating indexer
"Indexer ---"
$indexerBody = @"
{
  "name" : "$indexerName",
  "dataSourceName" : "$datasourceName",
  "targetIndexName" : "$indexName",
  "schedule" : { "interval" : "PT1H" },
  "fieldMappings" : [
    { "sourceFieldName" : "metadata_storage_path", "targetFieldName" : "id" },
    { "sourceFieldName" : "metadata_storage_path", "targetFieldName" : "path" },
    { "sourceFieldName" : "metadata_storage_last_modified", "targetFieldName" : "modified" },
    { "sourceFieldName" : "content", "targetFieldName" : "content" }
  ],
  "parameters" : {
      "maxFailedItems" : 1000000, 
      "maxFailedItemsPerBatch" : 1000000,
      "base64EncodeKeys": true,
      "configuration" : { 
          "indexedFileNameExtensions" : ".csv,.log" 
      }
  }
}
"@
$result = Invoke-WebRequest -UseBasicParsing -Uri "$searchServiceUrl/indexers?$apiVersion" -Method Post -Body $indexerBody -ContentType "application/json" -Headers $authHeader
$result.StatusDescription
