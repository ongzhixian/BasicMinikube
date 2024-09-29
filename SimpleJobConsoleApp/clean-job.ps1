# 

# Get list of pods that are Completed (Succeeded)
Write-Host 'Remove Completed pods'
kubectl get pod -o jsonpath='{range .items[*]}{@.metadata.name}|{@.status.phase}{"\n"}{end}' `
| Select-String -Pattern '(.+)\|(.+)' `
| Where-Object { $_.Matches[0].Groups[1].Value -like 'test*' -and $_.Matches[0].Groups[2].Value -like 'Succ*' } `
| ForEach-Object { kubectl delete pod $_.Matches[0].Groups[1].Value }


Write-Host 'Remove Completed jobs'
kubectl get job -o jsonpath='{range .items[*]}{@.metadata.name}|{@.status.succeeded}{"\n"}{end}' `
| Select-String -Pattern '(.+)\|(.*)' `
| Where-Object { $_.Matches[0].Groups[1].Value -like 'test*' -and $_.Matches[0].Groups[2].Value -like '1' } `
| ForEach-Object { kubectl delete job $_.Matches[0].Groups[1].Value }