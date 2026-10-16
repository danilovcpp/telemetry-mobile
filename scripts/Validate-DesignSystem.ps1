param([string]$ProjectPath = (Join-Path $PSScriptRoot '../TelemetryMobile'))
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $ProjectPath).Path
$xamlNamespace = 'http://schemas.microsoft.com/winfx/2009/xaml'
$keys = @{}
$sourceFiles = @(Get-ChildItem -LiteralPath $root -Recurse -File | Where-Object {
    $_.Extension -eq '.xaml' -and $_.FullName -notmatch '[\\/](bin|obj)[\\/]'
})
foreach ($file in $sourceFiles) {
    [xml]$document = Get-Content -LiteralPath $file.FullName -Raw
    foreach ($element in $document.SelectNodes('//*')) {
        $key = $element.GetAttribute('Key', $xamlNamespace)
        if ($key) {
            if ($keys.ContainsKey($key)) { throw "Duplicate resource '$key': $($file.FullName), $($keys[$key])" }
            $keys[$key] = $file.FullName
        }
    }
}
foreach ($file in $sourceFiles) {
    $text = Get-Content -LiteralPath $file.FullName -Raw
    foreach ($match in [regex]::Matches($text, '\{(?:StaticResource|DynamicResource)\s+([^},\s]+)')) {
        if (-not $keys.ContainsKey($match.Groups[1].Value)) { throw "Missing resource '$($match.Groups[1].Value)' in $($file.FullName)" }
    }
    foreach ($match in [regex]::Matches($text, 'Source="([^"{}]+\.xaml)"')) {
        if (-not (Test-Path -LiteralPath (Join-Path $file.DirectoryName $match.Groups[1].Value))) { throw "Missing merged dictionary in $($file.FullName)" }
    }
    foreach ($match in [regex]::Matches($text, '(?:Source|LeadingIcon|TrailingIcon|Icon)="([a-z_]+\.png)"')) {
        $image = Join-Path $root ('Resources/Images/' + $match.Groups[1].Value)
        if (-not ((Test-Path -LiteralPath $image) -or (Test-Path -LiteralPath ([IO.Path]::ChangeExtension($image, '.svg'))))) { throw "Missing image '$image'" }
    }
}
function Get-DictionaryKeys([string]$Path, [string[]]$Visited = @()) {
    $absolutePath = (Resolve-Path -LiteralPath $Path).Path
    if ($absolutePath -in $Visited) { throw "Cyclic dictionary dependency: $absolutePath" }
    [xml]$dictionary = Get-Content -LiteralPath $absolutePath -Raw
    foreach ($element in $dictionary.SelectNodes('//*')) {
        $key = $element.GetAttribute('Key', $xamlNamespace)
        if ($key) { $key }
        if ($element.LocalName -eq 'ResourceDictionary' -and $element.HasAttribute('Source')) {
            Get-DictionaryKeys (Join-Path (Split-Path $absolutePath) $element.GetAttribute('Source')) ($Visited + $absolutePath)
        }
    }
}
foreach ($file in $sourceFiles | Where-Object { $_.FullName -match '[\\/]Resources[\\/]Styles[\\/]' }) {
    $localKeys = @(Get-DictionaryKeys $file.FullName)
    foreach ($match in [regex]::Matches((Get-Content -LiteralPath $file.FullName -Raw), '\{StaticResource\s+([^},\s]+)')) {
        if ($match.Groups[1].Value -notin $localKeys) {
            throw "StaticResource '$($match.Groups[1].Value)' is outside the dictionary scope: $($file.FullName). Merge its token dictionary explicitly."
        }
    }
}
$localeKeys = @{}
foreach ($locale in @('', '.en', '.de')) {
    [xml]$resx = Get-Content -LiteralPath (Join-Path $root "Resources/Localization/AppResources$locale.resx") -Raw
    $names = @($resx.root.data | ForEach-Object { $_.name } | Sort-Object)
    if (@($names | Select-Object -Unique).Count -ne $names.Count) { throw "Duplicate localization key: $locale" }
    $localeKeys[$locale] = $names
    if ($locale -and (Compare-Object $localeKeys[''] $names)) { throw "Locale key mismatch: $locale" }
}
foreach ($file in $sourceFiles) {
    foreach ($match in [regex]::Matches((Get-Content -LiteralPath $file.FullName -Raw), '\{loc:Translate\s+([^}]+)\}')) {
        if ($match.Groups[1].Value -notin $localeKeys['']) { throw "Unknown translation '$($match.Groups[1].Value)'" }
    }
}
Write-Output "Validated $($sourceFiles.Count) XAML files, $($keys.Count) unique resource keys, images and $($localeKeys[''].Count) translations in RU/EN/DE."
