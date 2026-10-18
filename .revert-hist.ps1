param()
$ErrorActionPreference = 'SilentlyContinue'
$out = @()
$hist = "$env:APPDATA\Code\User\History"
$targets = @(
  'Components\App.razor',
  'Components\Pages\Home.razor',
  'Components\Sections\Tech\TechSection.razor',
  'Components\Shared\AnalyticsHeader.razor'
)
$map = @{}
Get-ChildItem $hist -Recurse -Filter entries.json | ForEach-Object {
  try {
    $j = Get-Content $_.FullName -Raw | ConvertFrom-Json
    if ($j.resource) { $map[$j.resource.Replace('%3A', ':').ToLower()] = @{ Dir = $_.Directory.FullName; E = $j.entries } }
  } catch {}
}
foreach ($f in $targets) {
  $uri = ([Uri](Resolve-Path $f).Path).AbsoluteUri.ToLower()
  $hit = $map[$uri]
  $out += "=== $f"
  if (-not $hit) { $out += "  sin historial"; continue }
  foreach ($e in $hit.E) {
    $ts = [DateTimeOffset]::FromUnixTimeMilliseconds($e.timestamp).ToLocalTime().ToString('dd/MM HH:mm:ss')
    $path = Join-Path $hit.Dir $e.id
    $exists = Test-Path $path
    $len = if ($exists) { (Get-Item $path).Length } else { -1 }
    $out += "  $ts $($e.id) exists=$exists len=$len"
  }
}
$out | Out-File "$env:TEMP\histdet.txt" -Encoding utf8
"OK"
