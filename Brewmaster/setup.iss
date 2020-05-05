[Setup]
AppName=Brewmaster
OutputBaseFilename=BrewmasterSetup
AppVersion={#version}
WizardStyle=modern
DefaultDirName={autopf}\Brewmaster
DefaultGroupName=Brewmaster
UninstallDisplayIcon={app}\Brewmaster.exe
Compression=lzma2
SolidCompression=yes
OutputDir=bin\Installer
PrivilegesRequiredOverridesAllowed=dialog

[Files]
Source: "bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Brewmaster"; Filename: "{app}\Brewmaster.exe"