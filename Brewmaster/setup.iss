[Setup]
AppId=brewmaster
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
Name: "{group}\Uninstall Brewmaster"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Brewmaster"; Filename: "{app}\Brewmaster.exe"

[Run]
Filename: "{app}\Brewmaster.exe"; Description: "Launch Brewmaster now"; Flags: postinstall nowait skipifsilent
