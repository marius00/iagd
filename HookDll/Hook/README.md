# ItemAssistantHook.dll

## Purpose
* Detect in-game stash status (open/closed)
* Instantly loot items as they are placed in the stash
* Detect steam cloud being active
* Fetch real information about items (on mouse hover, and on-request from the main IA application)


## Ignoring this DLL
The easiest way to obtain this DLL is by simply copying it from a recent installation of IAGrim.  
Unless you plan to actively develop the DLL, there is no need to compile it yourself.


## Prerequisites
* Boost 1.64 x64
* Visual Studio
* Build tools "Visual Studio 2015 (v140)" (Included in the latest VS installers, search for v140)
* SDK 10.0.22000.0 (Included in the latest VS installers)

## Compilation 
The DLL uses C++ and Assembly.

Boost is assumed to be located at $(BOOST), so lib files at $(BOOST)\lib64-msvc-14.0


# Flow
The application continously searches for a window/HWND with the class "GDIAWindowClass" and will send messages to said window if it exists.  
The DLL proxies a series of calls inside Grim Dawn, such as "Walk to NPC" to detect the location of the player, and the smuggler.  
For locating items, it proxies the "place item in stash bag" call and returns without forwarding it to Grim Dawn.

Item looted using the DLL are stored as CSV files, which will get picked up by IAGD.  
IAGD uses the same method of transferring items for the DLL to read.  

# Instaloot / real stats
The most relevant class for looking into both instaloot and real stats from items is InventorySack_AddItem