$token = ":iMvdH9sDj0zHvTFRmr6YGZLUOqIVhqO0CbhlditjOZs="

$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(($token)))

$headers = @{Authorization=("Basic {0}" -f $base64AuthInfo)}
$text = Get-Content .\solution.json -Raw 

Invoke-RestMethod -Headers $headers -ContentType "application/json" -Method Post -Uri https://davar.icfpcontest.org/teams/4/solutions -Body $text