$ErrorActionPreference = 'SilentlyContinue'
$out = @()
$hist = "$env:APPDATA\Code\User\History"
$targets = @(
  @{ p = 'Components\App.razor'; ids = @('MTz0.razor') },
  @{ p = 'Components\Sections\Tech\TechSection.razor'; ids = @('DhlW.razor', 'El8F.razor') },
  @{ p = 'Components\Shared\AnalyticsHeader.razor'; ids = @('SP8g.razor', 'zfQN.razor') }
)
$map = @{}
Get-ChildItem $hist -Recurse -Filter entries.json | ForEach-Object {
  try { $j = Get-Content $_.FullName -Raw | ConvertFrom-Json; if ($j.resource) { $map[$j.resource.Replace('%3A', ':').ToLower()] = $_.Directory.FullName } } catch {}
}
function H($t) { if ($null -eq $t) { return 'null' } return [BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash([Text.Encoding]::UTF8.GetBytes(($t -replace "`r`n", "`n").TrimEnd()))).Replace('-', '').Substring(0, 12) }
foreach ($t in $targets) {
  $f = $t.p
  $uri = ([Uri](Resolve-Path $f).Path).AbsoluteUri.ToLower()
  $dir = $map[$uri]
  $cur = Get-Content $f -Raw
  $head = git show "HEAD:$f" | Out-String
  $out += "=== $f  cur=$(H $cur) head=$(H $head)"
  foreach ($id in $t.ids) {
    $path = Join-Path $dir $id
    if (Test-Path $path) { $c = Get-Content $path -Raw; $out += "   $id => $(H $c)" } else { $out += "   $id => missing" }
  }
}
$out | Out-File "$env:TEMP\cmp2.txt" -Encoding utf8
"OK"
