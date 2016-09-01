@echo off
set AZCOPY="c:\Program Files (x86)\Microsoft SDKs\Azure\AzCopy\AzCopy.exe"
set ACCOUNT=_ACCOUNT_NAME_
set KEY=_ACCOUNT_KEY_
set CONTAINER=_CONTAINER_NAME_

%AZCOPY% /Dest:https://%ACCOUNT%.blob.core.windows.net/%CONTAINER% /Source:.\logset /Z:. /DestKey:%KEY% /S /XO /NC:64
pause
