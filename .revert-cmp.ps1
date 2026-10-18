$ErrorActionPreference = 'SilentlyContinue'
$out = @()
$hist = "$env:APPDATA\Code\User\History"
$files = git diff --name-only
$map = @{}
Get-ChildItem $hist -Recurse -Filter entries.json | ForEach-Object {
  try {
    $j = Get-Content $_.FullName -Raw | ConvertFrom-Json
    if ($j.resource) { $map[$j.resource.Replace('%3A', ':').ToLower()] = @{ Dir = $_.Directory.FullName; E = $j.entries } }
  } catch {}
}
$thr = 1791414000000  # 07/10 23:00 UTC = 20:00 local (inicio posible de la sesion)
foreach ($f in $files) {
  $mt = (Get-Item $f).LastWriteTime
  if ($mt -lt (Get-Date '2026-10-07 20:00')) { continue }   # solo archivos de hoy
  $head = git show "HEAD:$f" | Out-String
  $uri = ([Uri](Resolve-Path $f).Path).AbsoluteUri.ToLower()
  $hit = $map[$uri]
  $firstTxt = ''; $firstTs = 'none'; $preTxt = ''; $preTs = 'none'
  if ($hit -and $hit.E.Count -gt 0) {
    $sorted = @($hit.E | Sort-Object timestamp)
    $fE = $sorted[0]
    $fp = Join-Path $hit.Dir $fE.id
    if (Test-Path $fp) { $firstTxt = Get-Content $fp -Raw }
    $firstTs = [DateTimeOffset]::FromUnixTimeMilliseconds($fE.timestamp).ToLocalTime().ToString('dd/MM HH:mm:ss')
    $pE = @($sorted | Where-Object { $_.timestamp -lt $thr }) | Select-Object -Last 1
    if ($pE) { $pp = Join-Path $hit.Dir $pE.id; if (Test-Path $pp) { $preTxt = Get-Content $pp -Raw }; $preTs = [DateTimeOffset]::FromUnixTimeMilliseconds($pE.timestamp).ToLocalTime().ToString('dd/MM HH:mm:ss') }
  }
  $cur = Get-Content $f -Raw
  $norm = { param($t) ($t -replace "`r`n", "`n").TrimEnd() }
  $eq = { param($a, $b) (& $norm $a) -ceq (& $norm $b) }
  $flags = @()
  if ($head -and (& $eq $head $firstTxt)) { $flags += 'HEAD=firstEntry' }
  if ($preTxt -and (& $eq $head $preTxt)) { $flags += 'HEAD=preSession' }
  if ($preTxt -and (& $eq $preTxt $firstTxt)) { $flags += 'pre=firstEntry' }
  $out += ("{0} | mt={1} | first={2} pre={3} | {4}" -f $f, $mt.ToString('dd/MM HH:mm'), $firstTs, $preTs, ($flags -join ','))
}
$out | Out-File "$env:TEMP\cmp.txt" -Encoding utf8
"OK"
