$scaleSetName = "swarm-agent-89BF2E5Fvmss-0"
$resourceGroupName = "DockerizedEncoder"

$vmss = Get-AzureRmVmss -ResourceGroupName $resourceGroupName -VMScaleSetName $scaleSetName 
Stop-AzureRmVmss -InstanceId * -ResourceGroupName $resourceGroupName -VMScaleSetName $scaleSetName
Start-AzureRmVmss -InstanceId * -ResourceGroupName $resourceGroupName -VMScaleSetName $scaleSetName

