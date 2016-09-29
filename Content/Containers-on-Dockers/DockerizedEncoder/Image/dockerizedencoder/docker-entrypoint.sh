#!/bin/bash

if [ "$5" = '' ]; then
    echo "Usage:"
    echo " $0 SourceUrl StorageAccount StorageKey ContainerName TargetGUID ExpirationEpoch"
    echo "    SourceUrl: URI from the source file will be downloaded"
    echo "    StorageAccount: storage account name to store a results"
    echo "    StorageKey: storage account access key"
    echo "    ContainerName: blob container name"
    echo "    TargetGUID: alphanumeric unique id of target"
    echo "    ExpirationEpoch: result file expiration at given seconds since epoch UTC"
    exit 1
fi

SourceUrl=$1
export AZURE_STORAGE_ACCOUNT=$2
export AZURE_STORAGE_ACCESS_KEY=$3
export ContainerName=$4
TargetGUID=$5
Expiration=$6

IntermediateName=${TargetGUID}_Intermediate_${Expiration}
TargetName=${TargetGUID}_Success_${Expiration}
ErrorName=${TargetGUID}_Error_${Expiration}
SourceFile=/tmp/`basename $SourceUrl`
Zero=/tmp/zero
>$Zero

function cecho()
{
    echo -e "\x1B[32m$@\x1B[0m"
}

function result()
{
    if [ $1 ]; then
        cecho Uloading an Error file ...
        azure storage blob upload $Zero "$ContainerName" "$ErrorName"
    else
        cecho Uploading a Success file ...
        azure storage blob upload "$SourceFile" "$ContainerName" "$TargetName"
    fi

    cecho Deleting intermediate state marker file...
    azure storage blob delete "$ContainerName" "$IntermediateName"

    cecho Finished
    exit 0
}

cecho Writing intermediate state marker file...
azure storage blob upload $Zero "$ContainerName" "$IntermediateName"
if [ $? -ne 0 ]; then
    echo -e "\x1B[31mContainer access error. Terminated.\x1B[0m"
    exit 1
fi

cecho Removing expired files...
export Now=`date +%s`
azure storage blob list "$ContainerName" --json | awk ' /'name'/ {print $2}' | sed -e "s/[\",]//g" | awk -F "_" ' { if ($3 < ENVIRON["Now"]) { print "azure storage blob delete " ENVIRON["ContainerName"] " " $0 } } ' > /tmp/deleteExpired.sh
bash /tmp/deleteExpired.sh

cecho Downloading source file...
rm $SourceFile &> /dev/null
wget $SourceUrl -O $SourceFile
if [ $? -ne 0 ]; then
    echo -e "\x1B[31mSource file download error\x1B[0m"
    result 1
fi

cecho Dummy encoding with heavy CPU load for 120+ seconds...
End=$((SECONDS+120))
while [ $SECONDS -lt $End ]; do
    dd if=/dev/zero of=/dev/null bs=1K count=10M &> /dev/null
done

cecho This is as designed: 20% of runs will produce an error result
result "$(((RANDOM % 100)+1)) -lt 20"
