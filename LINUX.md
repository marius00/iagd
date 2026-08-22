# Item Assistant on Linux

Item Assistant is a Windows program. It runs on Linux under Wine, with one important rule:

> **Item Assistant must run inside Grim Dawn's own Proton prefix.**

Everything below is a consequence of that rule. If you install Item Assistant into its own Wine
prefix, or with a tool that makes a new prefix for it, it will start but it will never see your
game — no loot will be captured and no items can be transferred.

---

## Why one prefix

A Wine prefix is not just a folder. It is a whole simulated Windows session, with its own process
list and its own windows. Two prefixes are two separate machines that happen to share a disk.

Item Assistant works by injecting a small DLL into the running game and then exchanging files with
it. Every part of that — finding the game's window, opening its process, injecting the DLL — only
works between programs in the **same** prefix. There is no way around it.

So Item Assistant is launched *into* Grim Dawn's prefix. Both programs are then in one session, and
everything works the same way it does on Windows.

---

## Before you start

You need:

1. **Grim Dawn installed through Steam, with Proton enabled**, and **started at least once**. That
   first start is what creates the prefix everything else goes into.
2. **protontricks**. Install it from your distribution's packages, or as a Flatpak:
   ```bash
   flatpak install com.github.Matoking.protontricks
   ```

Grim Dawn's Steam app ID is **219990**. You will use that number in every command.

> **Flatpak users:** replace `protontricks-launch` in every command below with
> `flatpak run --command=protontricks-launch com.github.Matoking.protontricks`.
> You may also need `flatpak override --user --filesystem=/path/to/your/steam/library
> com.github.Matoking.protontricks` if your games are not in the default location.

---

## Install

Download these three installers on the Linux side first, then run each one **into the prefix**.

### 1. The .NET Desktop Runtime

Item Assistant is a .NET program. Get the **Windows x64 Desktop Runtime** installer (not the SDK,
not the ASP.NET one) from <https://dotnet.microsoft.com/download/dotnet/10.0>, then:

```bash
protontricks-launch --appid 219990 windowsdesktop-runtime-10.0.x-win-x64.exe
```

### 2. The WebView2 runtime

This is what draws the item list. Get the **Evergreen Standalone Installer (x64)** from
<https://developer.microsoft.com/microsoft-edge/webview2/>, then:

```bash
protontricks-launch --appid 219990 MicrosoftEdgeWebView2Setup.exe
```

Installing WebView2 on the Linux side, or into a different prefix, does not work. It must go into
this prefix.

### 3. Item Assistant

Get the installer from <https://grimdawn.evilsoft.net>, then:

```bash
protontricks-launch --appid 219990 GDItemAssistant.exe
```

The installer usually puts Item Assistant here:

```
<steam library>/steamapps/compatdata/219990/pfx/drive_c/users/steamuser/AppData/Local/IAGD/
```

If it is not there, look in `.../pfx/drive_c/Program Files/IAGD/` instead — the installer chooses
between the two depending on how Wine reports your user account.

If you do not know where your Steam library is, the common locations are `~/.steam/steam` and
`~/.local/share/Steam`. This will find it:

```bash
find ~ -type d -name 219990 -path '*compatdata*' 2>/dev/null
```

---

## Run it

```bash
protontricks-launch --appid 219990 ~/.steam/steam/steamapps/compatdata/219990/pfx/drive_c/users/steamuser/AppData/Local/IAGD/IAGrim.exe
```

Adjust the path if your library is elsewhere. It is worth saving this as a shell script or a
desktop shortcut, because you will run it every session.

Then start Grim Dawn as usual. You do not need to do anything else — Item Assistant finds the game
on its own.

**The hook attaches about 45 seconds after the game starts.** This is deliberate. Attaching while
the game is still building its world can crash it, so Item Assistant waits until the game is
definitely past that point. You will not lose any loot: nothing is captured before you are in the
game anyway.

---

## Check that it worked

Item Assistant can describe its own state. Run:

```bash
protontricks-launch --appid 219990 ~/.steam/steam/steamapps/compatdata/219990/pfx/drive_c/users/steamuser/AppData/Local/IAGD/IAGrim.exe --diagnose
```

This writes `iagd-diagnostics.txt` next to the settings file and opens it. It works even while
Item Assistant is already running, and even if the program is too broken to start normally.

In a healthy install you should see:

| Line | Expected |
|---|---|
| `Running under Wine` | `yes` |
| `Launched by Proton` | `yes` |
| `Game folder (Proton)` | a path ending in `steamapps\common\Grim Dawn` |
| `Runtime version` (WebView2) | a version number, not `(none)` |
| `ItemAssistantHook_x64.dll` | `present, version ...` |
| `Install #1` | your Grim Dawn folder, `database present` |
| `isRunningInWine` | `True` |

The same report is written into the normal log at every start, so if you send a log file it is
already included.

---

## When something is wrong

Run `--diagnose` first. Almost every problem below is visible in that report.

**The program does not start at all, or closes immediately.**
The .NET Desktop Runtime is probably not installed in this prefix. Repeat install step 1. Check
that you downloaded the *Desktop* runtime for *x64* — the plain runtime does not include the
Windows Forms parts Item Assistant needs.

**A message says the WebView2 runtime is missing.**
Repeat install step 2, using `protontricks-launch`. If you installed WebView2 some other way, it
went into the wrong place.

**The window opens but the item list stays empty.**
WebView2 is installed but did not start. The error message names its cache folder — delete that
folder and start Item Assistant again.

**No loot is captured.**
Check these lines in the report, in this order:
- `isRunningInWine` must be `True`. If it says `not set`, start Item Assistant once and close it.
- `ItemAssistantHook_x64.dll` must be `present`. If it says `MISSING`, reinstall.
- `Install #1` must show your Grim Dawn folder. If it says `none`, Steam did not tell Item
  Assistant where the game is; set the folder by hand in Settings.
- `Injection markers` should be `1` while the game is running. `0` means the hook has not attached
  — wait a full minute after the game starts, then check again.
- `Aborted attaches` counting up is normal while the game is loading or on the character screen. It
  is not an error.

**A message says the install is corrupted.**
The hook DLL is older than the program expects. This happens when an update could not overwrite a
file. Close Grim Dawn, reinstall Item Assistant, and start the game afterwards.

---

## What is not supported

- **Anything other than Steam plus Proton.** The one-prefix rule still applies to a GOG or Lutris
  install, so the same approach works in principle, but nothing here has been checked against one.
- **Item Assistant in its own prefix.** See the top of this file. It cannot work.
- **Steam rebuilding the prefix.** Some Proton updates and a "verify files" can reset it, which
  removes the .NET and WebView2 runtimes. Repeat install steps 1 and 2 if things stop working after
  a Steam update.

Linux support is newer and much less tested than Windows. If something does not work, please open
an issue and **attach the `--diagnose` output** — it answers most of the questions that would
otherwise take several rounds of messages.
