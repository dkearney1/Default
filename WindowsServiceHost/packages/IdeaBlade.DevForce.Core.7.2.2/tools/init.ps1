param($installPath, $toolsPath, $package, $project)

# The init script is run: 1) once when package first installed to solution, 2) every time the solution is opened

function IsSetupRequired([string] $minVersion)
{
  $setupRequired = $true
  
  # Check registry to see if setup required.  Only needed if not installed or version is prior to rtw
  $os=Get-WMIObject win32_operatingsystem
  if ($os.OSArchitecture -eq "64-bit") 
  {
    $IBKey = "hklm:\SOFTWARE\Wow6432Node\IdeaBlade2012\Version"
  } else 
  {
    $IBKey = "hklm:\SOFTWARE\IdeaBlade2012\Version"
  }

  if (Test-Path $IBKey)
  {
    $ver = (Get-ItemProperty -Path $IBKey -Name '(default)').'(default)'
    $currVersion = $ver.replace(".", "") -as [int]
    $reqVersion = $minVersion.replace(".", "") -as [int]
    $setupRequired = $currVersion -lt $reqVersion
  } 

  return $setupRequired
}

function IsExtensionNeeded([string] $extensionId, [string] $minVersion)
{
   $registryRoot = $DTE.RegistryRoot
   $extensionsTypesPath = "hkcu:\$registryRoot\ExtensionManager\ExtensionTypes"

   if (! (Test-Path $extensionsTypesPath)) { 
     return $true
   }

   $ext = Get-Item -Path $extensionsTypesPath | Select-Object -ExpandProperty Property | Where-Object {$_ -like $extensionId}
   if ($ext -ne $null) {
      $extarr = $ext.split(",")
      $extVersion = $extarr[1].replace(".", "") -as [int]

      $reqVersion = $minVersion.replace(".", "") -as [int]       
      return $extVersion -lt $reqVersion
   } else {
     return $true
   }
}

# --------------------------------------------------------------------------------------------
# 7.0.2.0 was first non-beta release.  
# 7.1.0.0 was last change requiring template extension re-install.
# 7.2.2.0 was last change requiring edmx extension re-install.

$setupInstallRequired = IsSetupRequired "7.0.2.0"
$edmvsixInstallRequired = IsExtensionNeeded "DF2012_IdeaBladeDesigner,*" "7.2.2.0"
$tempvsixInstallRequired = IsExtensionNeeded "DevForce2012Templates,*" "7.1.0.0"

$vsVersion = $DTE.RegistryRoot.substring($DTE.RegistryRoot.LastIndexOf('\')+1)
$extensionVsix = [string]::Format("IdeaBlade.VisualStudio.OM.Designer.{0}.vsix", $vsVersion)
$templatesVsix = [string]::Format("IdeaBlade.VisualStudio.TemplateInstaller.{0}.vsix", $vsVersion)


if ($edmvsixInstallRequired -or $tempvsixInstallRequired -or $setupInstallRequired)
{
    $VSPath = [System.IO.Path]::GetDirectoryName($DTE.FileName)
    $installer = [System.IO.Path]::Combine($VSPath, "VSIXInstaller.exe") 
    $extensionVsixPath = [System.IO.Path]::Combine($toolsPath, $extensionVsix)
    $templatesVsixPath = [System.IO.Path]::Combine($toolsPath, $templatesVsix)

    if ($edmvsixInstallRequired) {
      $process = [System.Diagnostics.Process]::Start($installer, "`"$extensionVsixPath`"")
      $process.WaitForExit()
    }
    if ($tempvsixInstallRequired) {
      $process = [System.Diagnostics.Process]::Start($installer, "`"$templatesVsixPath`"")
      $process.WaitForExit()
    }
    if ($edmvsixInstallRequired -or $tempvsixInstallRequired) {
      [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") | Out-Null
      [Windows.Forms.MessageBox]::Show("You must restart Microsoft Visual Studio in order for the changes to take effect.", 
        "Extensions and Updates", [Windows.Forms.MessageBoxButtons]::OK, 
        [System.Windows.Forms.MessageBoxIcon]::Information) | Out-Null
    }
    
    if ($setupInstallRequired) { 
      $toolsSetup = [System.IO.Path]::Combine($toolsPath, "setup.exe")
      [System.Diagnostics.Process]::Start($toolsSetup)
    }
}
else
{
    { "DevForce 2012 Tools are already installed." }
}
