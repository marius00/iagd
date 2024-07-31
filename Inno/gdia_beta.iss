#define ApplicationVersion GetFileVersion('..\installer\IAGrim_beta.exe')
#define ProductVersion GetStringFileInfo('..\installer\IAGrim_beta.exe', 'ProductVersion')

[Setup]
AppVerName=Grim Dawn Item Assistant
AppName=Grim Dawn Item Assistant (c) EvilSoft
VersionInfoVersion={#ApplicationVersion}
AppId=gdia
DefaultDirName={code:DefDirRoot}\GD Item Assistant
Uninstallable=Yes
OutputDir=..\Installer
SetupIconFile=gd.ico


[Tasks]
Name: desktopicon; Description: "Create a &desktop icon"; GroupDescription: "Icons:"
Name: starticon; Description: "Create a &startmenu icon"; GroupDescription: "Icons:"


[Icons]
Name: "{commonprograms}\GD Item Assistant"; Filename: "{app}\\IAGrim.exe"; Tasks: starticon
Name: "{commondesktop}\GD Item Assistant"; Filename: "{app}\\IAGrim.exe"; Tasks: desktopicon


[Files]
Source: "..\IAGrim\bin\Release\*"; Excludes: "*.pdb,ndp472-kb4054531-web.exe,vc2015_redist.x64.exe,vcredist_x86.exe,2010sp1_vcredist_x86.exe,2015rc3_vc_redist.x86.exe"; DestDir: "{app}"; Flags: overwritereadonly replacesameversion recursesubdirs createallsubdirs touch ignoreversion
Source: "readme.txt"; DestDir: "{app}";

[Run]
Filename: "{app}\IAGrim.exe"; Description: "Launch GD Item Assistant"; Flags: postinstall nowait
 


[Setup]
UseSetupLdr=yes
DisableProgramGroupPage=yes
DiskSpanning=no
AppVersion={#ApplicationVersion}
VersionInfoProductTextVersion={#ApplicationVersion}
PrivilegesRequired=admin
DisableWelcomePage=Yes
ArchitecturesInstallIn64BitMode=x64
AlwaysShowDirOnReadyPage=Yes
DisableDirPage=No
OutputBaseFilename=GDItemAssistantBeta-{#ApplicationVersion}
InfoAfterFile=readme.txt

[InstallDelete]
Type: files; Name: "{app}\SQLite.Interop.dll"


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

