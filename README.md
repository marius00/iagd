# Grim Dawn Item Assistant

For a binary download, see https://www.nexusmods.com/grimdawn/mods/43/



## Issues running this project?


## Dependencies missing
This project uses Nuget for external dependencies. To reinstall these, simply run `Update-Package -reinstall` in the package manager console.



Error    CS0246    The type or namespace name 'AutoUpdaterDotNET' could not be found (are you missing a using directive or an assembly reference?)    IAGrim    X:\Y\Z\iagd\IAGrim\UI\MainWindow.cs    13    Active

This means you're missing the AutoUpdater.NET.dll file that IA uses for automatic updates.
It can be obtained in two ways:
* Simply copying it from an existing IA install
* Compiling the "Auto updater" project, which is held separately from the IA solution. (SLN)
Place this file under IAGrim\bin\Release



## ItemAssistantHook.dll
IAGrim also depends on ItemAssistantHook.dll for detecting the status of various things ingame.

[More information in the DLL readme](HookDll/README.md)

## The web interface
HowTo:
* cd WebUI
* npm install
* npm run build
* deploy.cmd


# Trouble understanding the code?

## The Web View
The Web view is written in React and Typescript.
It can run both Embedded inside IAGrim, and stand-alone in a normal browser for debugging purposes.
After first cloning the project, the developer should run "npm install" to download node packages.

## IAGrim and the Web View (React)
The user interface is made in native C# (left side bar, top tabs) with CefSharp (Chromium) to run an embedded web view.
The web view is written using React and Typescript.

**The web view communicates with IAGrim in the following scenarios:**
* [1] Transfer items
* [2] Scrolling (delay load more items)
* [2] Load items on startup
* [2] Load recipes

1) This method uses regular links, like transfer://itemid, these are then caught by IA and passed on to the appropriate service.
2) This method uses the "magic" global variable called data, which is exposed into the embedded webview by Chromium (via JSWrapper)
   The data object contains both data and functions which can be called.
   Ex: Functions which are implemented in IAGrim, so that the Webview can call "data.RequestMoreItems" when infinite scrolling etc.
       The functions are async, and IAGrim will respond by calling a different callback in the data object.

**IA communicates with the Web View in the following ways:**
* Update items
* Display messages
* Update recipes
* Etc..

IAGrim exclusively communicates with the Web UI via the "data" object.
The web UI will have updated the data object with pointers to its own methods (ala   data.globalAddItems = addItems;)
So that IAGrim calls the appropriate code.


## SQL
Database operations are done by the classes located in the *Dao*Impl* classes.
These classes are then wrapped in a "Repo" named counterpart implementing the same interface as the *Impl* class.
The repo-class is responsible for executing all the methods using the ThreadExecuter class.

The ThreadExecuter is a class which contains 1 dedicated thread for running SQL operations.
All calls are run async, but uses waiting to run blocking for the calling function.

This ensures that all database transactions are run on a single thread, which is imperative when running with an SQLite backend.
(There are older versions of IA which uses PostgreSQL, which do not wrap the calls using the ThreadExecuter)


## Monitoring transfer files (Grim Dawn integration)
The StashFileMonitor class is responsible for monitoring the Grim Dawn savegame folder for file changes.
Once a change to transfer.gs? has been detected, it will notify the StashManager about the change.
The StashManager is responsible for verifying that the player has left the stash area (prevent the player from re-opening the stash while IA is using it) and that the stash is indeed closed.

## Message Processors
IAGrim uses a DLL Proxy which it injects into the game process for the purposes of "spying" on some information about the current game state.
Such as: If the stash is open, the player position, if cloud saving is enabled.
The goal is to ensure that it is indeed safe to transfer items, before any item transfers are attempted by IA.

Messages are passed from the C++ dll via WINAPI SendMessage, IA is listening for these messages using a window created with a specific class name.


## Item Searches
Item searches are done for owned items, recipe items*, buddy items* and augments*.
The search result is then merged stored.
Stats delay loaded and applied to items as the user scrolls.

*) If configured to be included.


## Recognizing item stats
In order for IA to recognize item stats, it needs to parse the Grim Dawn item database. (item templates)
IA will parse the database and "cache" the results in the IA database for lookups during search.
If a player item does not match any known template (different mod, not loaded AoM, other..), the item will simply be displayed as "Unknown Item"
Loading items is only done for **visual purposes** and to **enable search**.
Even if an item is unknown, the item will be replicated in-game with its original stats.


## Translations
Translations are handled in two ways:
For the web ui, they are passed in via the JSWrapper object.
For the C# Forms user interface, the "tag" property is used to detect the appropriate translation string.
Default language is located in EnglishLanguage.cs, IA looks for tags_ia.txt in GD translation files.




## Restoring items from old backup to new one:
SELECT 

id_playeritem,baserecord,PrefixRecord,SuffixRecord,ModifierRecord,null,
Seed,MateriaRecord,RelicCompletionRecord,RelicSeed,null,
0,0,MateriaCombines,StackCount,
null,null,null,null,0,null,null,0,null,null,null,null,null,null

FROM `playeritem` WHERE id_itemowner = 41025536


UPDATE PlayerItem SET enchantmentrecord = NULL where enchantmentrecord = 0;
UPDATE PlayerItem SET azuuid_v2 = NULL where azuuid_v2 = 'NULL';
UPDATE PlayerItem SET azpartition_v2 = NULL where azpartition_v2 = 'NULL';
UPDATE PlayerItem SET Mod = NULL where Mod = 'NULL';
UPDATE PlayerItem SET TransmuteRecord = NULL where TransmuteRecord = 'NULL';