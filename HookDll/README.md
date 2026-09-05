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
* SDK 10.0.22000.0 (Included in the latest VS installers) (or newer?)

## Compilation 
The DLL uses C++ and Assembly.

[Boost](https://sourceforge.net/projects/boost/files/boost-binaries/1.78.0/) is assumed to be located at $(BOOST), so lib files at $(BOOST)\lib64-msvc-14.0


# Flow
The application continously searches for a window/HWND with the class "GDIAWindowClass" and will send messages to said window if it exists.  
The DLL proxies a series of calls inside Grim Dawn, such as "Walk to NPC" to detect the location of the player, and the smuggler.  
For locating items, it proxies the "place item in stash bag" call and returns without forwarding it to Grim Dawn.

Item looted using the DLL are stored as CSV files, which will get picked up by IAGD.  
IAGD uses the same method of transferring items for the DLL to read.  

# Instaloot / real stats
The most relevant class for looking into both instaloot and real stats from items is InventorySack_AddItem


# The DLLs
* InventorySack_AddItem: Instaloot and real stats
* OnDemandSeedInfo: Runs a named pipe and listens for requests from IA to generate real stats for items
* ItemRelicSeedInfo: Passively listens for seed info for relics
* EquipmentSeedInfo: Passively listens for seed info for equipment
The remaining hooks are mostly simple proxies used to glean information, such as opening the stash or walking away from the stash.


# Messages
| Message								| Purpose/meaning		| Payload			|
| --------------------------------------|:---------------------:|------------------:|
| TYPE_OPEN_PRIVATE_STASH				| Stash open			| 1 byte			|
| TYPE_OPEN_CLOSE_TRANSFER_STASH		| Stash open/closed		| 1 byte			|
| TYPE_SAVE_TRANSFER_STASH				| Stash closed			| None				|
| TYPE_DISPLAY_CRAFTER					| Crafting/unsafe IO	| None				|
| TYPE_CAN_USE_DISMANTLE				| Crafting/unsafe IO	| None				|
| TYPE_Display_Transmute				| Crafting/unsafe IO	| None				|

## Compiling detours 64 bit
VSPATH...\VC\bin\amd64\vcvars64.bat
nmake clean
nmake -D DETOURS_TARGET_PROCESSOR=X64


## Debugging the DLL
The DLL logs to %appdata%\..\local\evilsoft\iagd\iagd_hook.log