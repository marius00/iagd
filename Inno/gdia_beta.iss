#define ApplicationVersion GetFileVersion('..\installer\IAGrim_beta.exe')
#define ProductVersion GetStringFileInfo('..\installer\IAGrim_beta.exe', 'ProductVersion')

[Setup]
AppVerName=Grim Dawn Item Assistant
AppName=Grim Dawn Item Assistant (c) EvilSoft
VersionInfoVersion={#ApplicationVersion}
AppId=gdia
DefaultDirName={code:DefDirRoot}\IAGD
Uninstallable=Yes
OutputDir=..\Installer
SetupIconFile=gd.ico


[Tasks]
Name: desktopicon; Description: "Create a &desktop icon"; GroupDescription: "Icons:"
Name: starticon; Description: "Create a &startmenu icon"; GroupDescription: "Icons:"


[Icons]
Name: "{commonprograms}\IAGD"; Filename: "{app}\\IAGrim.exe"; Tasks: starticon
Name: "{commondesktop}\IAGD"; Filename: "{app}\\IAGrim.exe"; Tasks: desktopicon
; Resets the window position / minimize to tray settings, for when IA starts off-screen or hidden in the tray.
Name: "{commonprograms}\IAGD (safemode)"; Filename: "{app}\IAGrim.exe"; Parameters: "--safe-mode"; Comment: "Start IAGD with the window position and tray settings reset"; Tasks: starticon


; Packaging must fail loudly rather than ship an install without dllver.txt: IAGD reads it on startup to
; detect a hook DLL left behind by an update that could not overwrite it. It is written by the IAGrim
; build (see IAGrim.csproj) and by HookDll\copy.cmd; missing here means neither has run against this
; output folder, so the release would go out with no version check at all.
#if !FileExists(AddBackslash(SourcePath) + "..\IAGrim\bin\Release\net10.0-windows\win-x64\dllver.txt")
  #error dllver.txt is missing from IAGrim\bin\Release\net10.0-windows\win-x64 - rebuild IAGrim (or run HookDll\copy.cmd) before packaging.
#endif
[Files]
Source: "..\IAGrim\bin\Release\net10.0-windows\win-x64\*"; Excludes: "*.pdb"; DestDir: "{app}"; Flags: overwritereadonly replacesameversion recursesubdirs createallsubdirs touch ignoreversion
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

