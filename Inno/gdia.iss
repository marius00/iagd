#define ApplicationVersion GetFileVersion('..\installer\IAGrim.exe')

[Setup]
AppVerName=Grim Dawn Item Assistant
AppName=Grim Dawn Item Assistant (c) EvilSoft
VersionInfoVersion={#ApplicationVersion}
AppId=gdia
DefaultDirName={code:DefDirRoot}\Grim Dawn Item Assistant
Uninstallable=Yes
OutputDir=..\Installer
SetupIconFile=gd.ico


[Tasks]
Name: desktopicon; Description: "Create a &desktop icon"; GroupDescription: "Icons:"
Name: starticon; Description: "Create a &startmenu icon"; GroupDescription: "Icons:"


[Icons]
Name: "{commonprograms}\Grim Dawn Item Assistant"; Filename: "{app}\\iagrim.exe"; Tasks: starticon
Name: "{commondesktop}\Grim Dawn Item Assistant"; Filename: "{app}\\iagrim.exe"; Tasks: desktopicon


[Files]
Source: "..\IAGrim\bin\Release\*"; Excludes: "*.pdb, *.exe.config"; DestDir: "{app}"; Flags: overwritereadonly replacesameversion recursesubdirs createallsubdirs touch ignoreversion


[Run]
Filename: "{app}\dotNetFx45_Full_setup.exe"; Parameters: "/passive /showrmui /norestart"; Description: "Install .NET 4.5"; Flags: postinstall unchecked runascurrentuser
Filename: "{app}\vcredist_x86.exe"; Parameters: "/install /quiet /norestart"; Description: "Install VC++ Redistributable 2013 (x86)"; Flags: runhidden runascurrentuser
Filename: "{app}\2010sp1_vcredist_x86.exe"; Parameters: "/install /quiet /norestart"; Description: "Install VC++ Redistributable 2010 SP1 (x86)"; Flags: runhidden runascurrentuser

Filename: "{app}\iagrim.exe"; Description: "Launch Grim Dawn Item Assistant"; Flags: postinstall nowait


[Setup]
UseSetupLdr=yes
DisableProgramGroupPage=yes
DiskSpanning=no
AppVersion={#ApplicationVersion}
PrivilegesRequired=admin
DisableWelcomePage=Yes
ArchitecturesInstallIn64BitMode=x64
AlwaysShowDirOnReadyPage=Yes
DisableDirPage=No
OutputBaseFilename=GDItemAssistant

[UninstallDelete]
Type: filesandordirs; Name: {app}

[Languages]
Name: eng; MessagesFile: compiler:Default.isl

[Code]
function IsRegularUser(): Boolean;
begin
Result := not (IsAdminLoggedOn or IsPowerUserLoggedOn);
end;

function DefDirRoot(Param: String): String;
begin
if IsRegularUser then
Result := ExpandConstant('{localappdata}')
else
Result := ExpandConstant('{pf}')
end;

