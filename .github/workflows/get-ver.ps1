# Verify valid semver, and provide it along with an AssemblyVersion-compatible string as env vars.

# Set to the value provided by github.ref
param([string]$ghRef)

# refs/tags/v1.2.3 or refs/heads/branch — extract semver token from tag refs
$tagPart = $ghRef
if ($ghRef -match 'refs/tags/(.+)$') {
    $tagPart = $Matches[1].Trim()
}
if ($tagPart.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
    $tagPart = $tagPart.Substring(1)
}

$semVerRegex = "^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$"
$matchInfo = [regex]::Match($tagPart, $semVerRegex)

if (!($matchInfo.Success)) {
    Write-Host "Could not find valid semver within ref. string. Given: $ghRef (parsed: $tagPart)"
    Exit 1
}

$verRes = "VER={0}.{1}.{2}" -f $matchInfo.Groups["major"], $matchInfo.Groups["minor"], $matchInfo.Groups["patch"]
$semVerRes = "SEMVER=" + $matchInfo.Value
$prGroup = $matchInfo.Groups["prerelease"]
$isPrerelease = $prGroup.Success -and -not [string]::IsNullOrEmpty($prGroup.Value)
$isPr = "ISPRERELEASE=" + $isPrerelease.ToString().ToLower()

echo $verRes >> $env:GITHUB_OUTPUT
echo $semVerRes >> $env:GITHUB_OUTPUT
echo $isPr >> $env:GITHUB_OUTPUT

Write-Host "Result: $verRes, $semVerRes, $isPr"
