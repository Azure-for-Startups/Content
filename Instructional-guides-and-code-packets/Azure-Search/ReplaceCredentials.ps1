$files = Get-ChildItem $PSScriptRoot -Include ("Variables.ps1","settings.js","uploadLogset.cmd") -Recurse 
foreach ($file in $files)
{
    (Get-Content $file.PSPath) |
    Foreach-Object { 
        $_ `
        -creplace "_SERVICE_URL_"   , "_SERVICE_URL_" `
        -creplace "_ADMIN_KEY_"     , "_ADMIN_KEY_" `
        -creplace "_ACCOUNT_NAME_"  , "_ACCOUNT_NAME_" `
        -creplace "_ACCOUNT_KEY_"   , "_ACCOUNT_KEY_" `
        -creplace "_CONTAINER_NAME_", "_CONTAINER_NAME_" `
        -creplace "_QUERY_KEY_"     , "_QUERY_KEY_" `
    } |
    Set-Content $file.PSPath -Encoding UTF8
}