import './Help.css';
import {openUrl} from '../../integration/integration';
import {Fragment, h} from "preact";
import {PureComponent} from "preact/compat";

function toNumberedList(s: string) {
    return <ol>
        {s.trim().split('\n').map(e => <li key={e}>{e}</li>)}
    </ol>;
}

enum IHelpEntryType {
    Informational, Help
}

interface IHelpEntry {
    title: string;
    tag: string;
    body: () => any;
    type: IHelpEntryType;
}

const typicalParseDbMessage = <Fragment>{toNumberedList(`
    Close Grim Dawn
    Open IA
    Click the "Grim Dawn" tab inside IA
    Select "Vanilla" (or Forgotten Gods/AoM)
    Click "Load Database"
    `)}
    <i>If this did not resolve your issue, try restarting IA.</i>
</Fragment>;

const helpEntries = [
    {
        title: `IA deposits items, but they never show up in-game`,
        tag: 'ExpansionsDisabledItemsMissing',
        body: () => <div>
            IA is looting items just fine, depositing just fine, but they never show up in-game? <br/>
            Most likely you have the wrong setting on the "mod filter" in the upper right corner. <br/>
            The mod filter might be set to "no expansions", while you're playing with expansions or similar.<br/><br/>
            This filter lets you use IA even when you own expansions that you've disabled in order to play with friends.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `IA says "Stash: Error"`,
        tag: 'StashError',
        body: () => <div>
            There is an issue detecting the status of your in-game stash. <br/>
            This often indicates an issue injecting the IA dll into the game due to permission issues. <br/>
            Try running IA as administrator.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `There is an error with the path..`,
        tag: 'PathError',
        body: () => <div>
            Item Assistant has encountered an error injecting the item monitoring code into Grim Dawn<br/>
            <br/>
            There appears to be an error in the path/folder where IA is installed.. <br/>
			Try installing IA in a different folder, or running IA as administrator. (If that fails, ask for help on discord) <br/>
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `IA says "Stash: NOT64BIT"`,
        tag: 'No32Bit',
        body: () => <div>
            As of version 1.3 of Item Assistant, only 64 bit Grim Dawn is supported. <br/>
            If you do not wish to run 64 bit version of Grim Dawn, you'll need to downgrade to version 1.2 of Item Assistant <br/>
            The version is available on the website as the "32-bit version" <br/> <br/>
            To ensure that you always run the 64-bit version, you can add "/x64" as a launch option in steam.
            <img src="./x64steam.png"/> <br/>
            You do not need to do any conversions on your stash. Just run 64 bit grim dawn.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `IA says "Generic stat"`,
        tag: 'GenericStatNoReplica',
        body: () => <div>
            As of version 1.3 of Item Assistant, the program has support for displaying the real stats of items.<br/>
            If an item says "Generic stats", it's a warning that Item Assistant does not yet know the true stats of this item <br/><br/>
            IA will automatically start fetching the real stats of items while you are playing Grim Dawn.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `IA returns items immediately`,
        tag: 'ItemClassificationReturn',
        body: () => <div>
            If you're encountering the error "Deposited item back in-game, did not pass item classification.", it most likely means you need to parse the database. <br/>
            IA will return any items it cannot identify, since you'd be unable to search for them based on stats. <br/> <br/>
            {typicalParseDbMessage}
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `IA shows me "Unknown Item"!??`,
        tag: 'UnknownItem',
        body: () => typicalParseDbMessage,
        type: IHelpEntryType.Help
    },
    {
        title: `My item names are missing`,
        tag: 'MissingItemNames',
        body: () => typicalParseDbMessage,
        type: IHelpEntryType.Help
    },
    {
        title: `My item are named funny stuff like gdxTag0134`,
        tag: 'NeedParseGdxTag',
        body: () => typicalParseDbMessage,
        type: IHelpEntryType.Help
    },
    {
        title: `IA says my items are unidentified??`,
        tag: 'NotLootingUnidentified',
        body: () => typicalParseDbMessage,
        type: IHelpEntryType.Help
    },
    {
        title: `My icons are missing!`,
        tag: 'MissingIcons',
        body: () => <div>
            Icons are parsed by IA in the background, so it may take a while for them to show up.<br/>
            It may require a restart for IA to fully complete the icon parsing.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `When will IA be updated for the latest GD patch?`,
        tag: 'GrimDawnUpdated',
        body: () => <div>
            Item Assistant rarely requires any update, though you may need to parse the database again. <br/><br/>
            {typicalParseDbMessage}
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `Will IA give me back exactly the same item, with the same stats?`,
        tag: 'ReproduceStats',
        body: () => <div>
            IA will never modify any of your items, and will always reproduce them exactly as it found them. <br/>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `What does it mean to use IA on multiple PCs?`,
        tag: 'MultiplePcs',
        body: () => <div>
            The "<i>Multiple PC</i>" checkbox in Item Assistant is an addition to the backup system.<br/>
            If you have enabled backups by logging in with an e-mail, it's possible to synchronize your items between
            multiple PCs in ~realtime.<br/><br/>
            This can useful if you play Grim Dawn on several PCs, or if you share your stash with a loved one, as you'll
            be using the same IA stash across installs.<br/><br/>

            The checkbox under settings makes IA sync items more aggressively, opting for near-instant syncronization
            instead of low bandwidth usage.
            IA will not synchronize your characters, only your items.<br/>
            <br/>
            <b>Obs:</b> You will need to restart IA after toggling this feature.
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Why does IA show me items I don't have?`, // TODO: Link to this on recipe and buddy items?
        tag: 'ShowingItemsIDontHave',
        body: () => <div>
            Item Assistant lets you show recipes and augments as items, as a reminder that you can obtain them.<br/>
            <br/>
            If you don't wish IA to display these items: <br/>
            Go to settings and disable "Show recipes as items" and "Show augments as items". <br/><br/>
            It's merely a "convenience feature" to have IA list things you 'might' have or be able to make. <br/>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `I can't find my items!`,
        tag: 'CantFindItemsMod',
        body: () => <div>
            {toNumberedList(`
      Restart IA
      Make sure you've selected the right mod (upper right corner, most likely "No Mod")`)}

            Typically it's the mod selection that causes items to not be found, as IA supports separating items from
            both mods and softcore/hardcore.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `How/Where do I find my log files?`,
        tag: 'FindLogFiles',
        body: () => <div>
            {toNumberedList(`
      Click the "Settings" tab
      Click "View Logs"
      The most recent log is "log.txt"`)}
            The log folder can also be found at <i>%appdata%\..\local\evilsoft\iagd\</i>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `How/Where do I find my backups?`,
        tag: 'FindBackups',
        body: () => <div>
            {toNumberedList(`
      Click the "Settings" tab
      Click "View Backups"`)}

            This folder contains both daily backups from Item Assistant, as well as all your previous shared stash
            files.<br/>
            It is <span className="attention">highly recommended</span> that you enable additional backups. <br/>
            Harddrive failures happens. Viruses happens. Reinstalling windows and forgetting to copy IA
            happens. <br/><br/>
            <span className="attention">Use additional backup methods!</span>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Where do I find userdata.db / Where does IA store items?`,
        tag: 'FindUserdataDb',
        body: () => <div>
            {toNumberedList(`
      Click the "Settings" tab
      Click "View Logs"
      Go to the "data" folder`)}
            The data folder can also be found at <i>%appdata%\..\local\evilsoft\iagd\data</i>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Why is dropbox/onedrive/gdrive backups disabled?`,
        tag: 'BackupAutodetectDisabled',
        body: () => <div>
            Item Assistant does not have any direct integration with these services.<br/>
            The way IA creates backups to say Dropbox is by detecting the installation folder, and placing a zip file
            there with the daily backup.<br/>
            The same can be achieved by choosing "custom" and simply navigating to the folder of your cloud
            provider.<br/>
            <br/>
            If the icon is disabled, it means IA was not able to automatically detect where you have the cloud sync
            installed, and you will have to enable it manually via custom backups.
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `I disabled cloud saving and all my characters are gone!`,
        tag: 'DisabledCloudsaveFreakout',
        body: () => <div>
            {toNumberedList(`
        Don't freak out
        This was not IA
        This is expected`)}
            Look up a guide online on disabling cloud saving.<br/>
            When cloud saves are disabled, Grim Dawn looks for your characters in a <i>different folder</i>.<br/><br/>
            You simply need to <a
            href="https://forums.crateentertainment.com/t/how-to-move-your-saves-from-steam-cloud-to-grim-dawns-default-location/28921">copy
            them to a new folder</a>,
            and everything will be right in the world.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `Could not create SSL/TLS secure channel`,
        tag: 'SSLAntiVirusIssues',
        body: () => <div>
            "The request was aborted: Could not create SSL/TLS secure channel." <br/>
            If you're running into this error message in the logs, your anti virus is most likely blocking online backups and automatic backups. <br/><br/>
            <a href={"https://social.msdn.microsoft.com/Forums/vstudio/en-US/9e0bbf83-78ae-4f5c-9ebb-dbb75c928929/problems-could-not-create-ssl-tls-secure-channel?forum=csharpgeneral"}>Kaspersky anti-virus in particular</a> does a man-in-the-middle attack on network traffic, causing the encryption (SSL) verification to fail.
            <br/><br/>
            Another cause can be outdated TLS on Windows 7. If you are running windows 7, <a href={"https://stackoverflow.com/questions/70674832/windows-7-could-not-create-ssl-tls-secure-channel-system-net-webexception"}>see this link</a>
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `How do I backup my items?`,
        tag: 'HowDoIBackup',
        body: () => <div>
            <b>IA can automatically backup your items to your favorite cloud provider, or to a custom folder (for
                example on your desktop or a USB stick)</b><br/>
            <br/>
            To backup your items to a custom folder:<br/>

            <ol>
                <li>Click the checkbox next to the "Custom" button</li>
                <li>Click "Custom" button to select the folder</li>
                <li>Click "Backup Now" to create the backup immediately</li>
            </ol>
            The backup will also include your Grim Dawn characters and stash files.

            <br/><br/><br/>
            <b>If you just want to quickly move your items to another computer:</b>
            <ol>
                <li>Click the "Settings" tab</li>
                <li>Click "View Logs"</li>
                <li>Go into "data"</li>
                <li>Copy "userdata.db"</li>
            </ol>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `How do I restore from a backup?`,
        tag: 'RestoreBackup',
        body: () => <div>

            <b>There are two ways you may have backed up IA:</b><br/>
            <br/>
            <b>If you backed up IA via cloud backups or to a custom folder:</b><br/>

            <ol>
                <li>Unzip the <i>export.ias</i> file from your backup</li>
                <li>Open IA</li>
                <li>Click the "Settings" tab</li>
                <li>Click "Import/Export"</li>
                <li>Import / IAS</li>
                <li>Import the export.ias file</li>
            </ol>
            You will also find your Grim Dawn characters under the 'save' directory in the backup file.<br/>
            <br/>
            <b>If you backed up IA manually just copying <i>userdata.db</i></b><br/>
            <ol>
                <li>Open IA</li>
                <li>Click the "Settings" tab</li>
                <li>Click "View Logs"</li>
                <li>Enter the "data" folder</li>
                <li>
                    <b>Close IA</b>
                </li>
                <li>Copy <i>"userdata.db" into the data folder</i></li>
            </ol>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Item Assistant says some files are missing, and that Avast may be to blame!?`,
        tag: 'Avasted',
        body: () => <div>
            The way Item Assistant detects if your stash is open or closed, is by injecting code into Grim Dawn.<br/>
            Some super paranoid anti virus / anti malware programs tends to handle this by simply deleting Item
            Assistant.<br/>
            They never tell you, so you never realize what the cause is.<br/><br/>
            The main culprit here is Avast, but other anti virus programs has been known to sometimes freak out as
            well. <br/>
            The only way to notice is by going into the logs of the anti virus and see that it has interferred without
            notifying you.<br/><br/>
            In order to continue using IA, you must whitelist it in whatever god forsaken anti virus program you're
            using, and then completely reinstall IA.<br/>
            If you are reading this message, your "anti" virus program has already wiped out half the things IA needs to
            keep working properly.<br/><br/>
            Good luck, don't blame me. The problem is not IA.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `I want to delete all my items! Start from scratch!`,
        tag: 'StartFromZero',
        body: () => <div>
            {toNumberedList(`
      Go to backups and click "Delete backups" (if using online backups)
      Go to settings and click "View data".
      Close IA
      Delete the file called "userdata.db"
      Watch the world burn
      Open IA and cry, as you see all your items forever gone.
      `)}
            <br/>

            If your items start slowly and magically reappearing, you forgot step 1.<br/>
            Start at step 1.
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Does this tool support Hardcore? Can I play both Hardcore and softcore?`,
        tag: 'SupportsHardcore',
        body: () => <div>
            Yes! Your items are separated by mod and hardcore/softcore. <br/>
            In the upper right corner of IA you can select which items you which to display.
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Settings: What is show recipes as items?`,
        tag: 'ShowRecipesAsItems',
        body: () => <div>
            <b>Show recipes as items will include your recipes when searching for items</b><br/>
            Some players are more interested in what is available, in both items and what they can make<br/>
            And this option lets them see craftable items as well as their own.
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Settings: What is show augments as items?`,
        tag: 'ShowAugmentsAsItems',
        body: () => <div>
            <b>Show augments as items will include purchaseable augments when searching for items</b><br/>
            Some players are more interested in what is available, in both items and what they can purchase
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Settings: What is buddy items?`,
        tag: 'BuddyItems',
        body: () => <div>
            Buddy items lets you see all of your friends items.<br/>
            This can be useful if you're playing with a friend / spouse / etc <br/>
            As you can both search and see the items the other person has, but not access them yourself.
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `BuddyItems: What is a buddy id?`,
        tag: 'WhatIsBuddyId',
        body: () => <div>
            When logging into online backups, a random number is assigned to your account.<br/>
            This number can then be sent to your buddies, to let them search your items without needing to know your
            e-mail.
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `BuddyItems: What is a buddy nickname?`,
        tag: 'WhatIsBuddyNickname',
        body: () => <div>
            The buddy nickname is a text label to help you identify which buddy this is.<br/>
            This field can contain anything, but it cannot be empty.
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Settings: What is regular updates?`,
        tag: 'RegularUpdates',
        body: () => <div>
            With regular updates you'll occationally be notified of a new version of IA. <br/>
            You'll always get the latest version, but it may take a couple of weeks before you're notified. <br/>
            <b>Recommended for those who don't wish to be bothered with frequent updates.</b>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Settings: What is experimental updates?`,
        tag: 'ExperimentalUpdates',
        body: () => <div>
            With experimental updates you'll always get the latest version of IA. <br/>
            This can include very minor bugfixes, and may during periods mean daily updates. <br/>
            <b>Recommended for those who always wish to be on the latest version, and don't mind upgrading
                frequently.</b>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `Cannot find the Grim Dawn installation`,
        tag: 'CannotFindGrimdawn',
        body: () => <div>
            IA will automatically attempt to detect Grim Dawn the following ways:
            <ul>
                <li>By checking if Grim Dawn is currently running</li>
                <li>By reading the steam config</li>
                <li>By reading the registry</li>
                <li>By looking for GOG Galaxy</li>
            </ul>
            <b>The easiest way to get IA to detect Grim Dawn, is to simply start the game while IA is running.</b>
        </div>,
        type: IHelpEntryType.Informational
    },
    {
        title: `How does online backups work?`,
        tag: 'OnlineBackups',
        body: () => <div>
            <h2>IA has built in support for backing up your items to the cloud.</h2>
            To enable automatic backup of your items, all you need to do is input your e-mail address, and then input
            the one-time code which will be sent to you. <br/>
            <br/>
            Any time you loot an item it will be synced up to the cloud. If anything happens and you need to reinstall
            IA, all you need to do is login with the same e-mail address, and IA
            will automagically download all of your items again.
            <br/>
            This functionality can also be used to share items between multiple computers, using the "Using multiple
            PCs" setting.
        </div>
    },
    {
        title: `Not enough stash tabs`,
        tag: 'NotEnoughStashTabs',
        body: () => <div>
            IA requires a <u>minimum</u> of two <u>shared</u> stash tabs to work. <br/>
            One tab to loot/remove items from, and one tab to deposit/add items. <br/>
            By default IA will loot any items you place in the <u>last</u> shared tab, and deposit to the <u>second to
            last</u><br/><br/>
            If you already have 2 or more tabs, and are seeing this error, you most likely bought <u>personal
            stash</u> tabs, not <u>shared stash</u> tabs.<br/>
            <img src="./smuggler.png"/> <br/>
            If you are seeing this icon, you are viewing your <u>private stash</u>, not your shared stash.
        </div>
    },
    {
        title: `Can I transfer/store items on a different harddrive?`,
        tag: 'StoreOnDifferentDisk',
        body: () => <div>
            Sorta..<br/><br/>

            <ol>
                <li>Close IA</li>
                <li>Open "%appdata%\..\local\evilsoft"</li>
                <li>Copy the folder "IAGD" into "D:\mystuff\"</li>
                <li>Rename the folder to "IAGD-backup"</li>
                <li>Open command line (win+r on keyboard, then "run" and enter)</li>
                <li>"cd %appdata%\..\local\evilsoft"</li>
                <li>"mklink /j iagd d:\stuff\iagd"</li>
                <li>Start IA</li>
                <li>If everything works fine, delete the "IAGD-backup" folder.</li>
            </ol>
            This creates a type of shortcut that is transparant to IA. The items/etc will be on "D:\mystuff" but IA can
            keep reading it from "%appdata%\..\local\evilsoft"
        </div>
    },
    {
        title: `IA is looting items, but they never get removed in-game!`,
        tag: 'WindowsAntiRansomwareIssue',
        body: () => <div>
            Item assistant is looting items, they never vanish in-game, and it cannot transfer items back into the
            game? <br/>
            The most common cause here is the anti-ransomware protection in Windows 10.<br/>
            If you have recently enabled this on the <i>my documents</i> folder, it's most likely preventing Item
            Assistant from writing to files under <i>my games</i>, which is under my
            documents. <br/><br/>

            If this help entry opened automatically for you in item assistant, something is preventing Item Assistant
            from writing it's changes.
        </div>
    },
    {
        title: `Can I use this with a 4K monitor?`,
        tag: '4K',
        body: () => <div>
            Item Assistant may not function correctly with 4K monitors and UI scaling. <br/>
            Right click Item Assistant, got to properties and then Compatability mode. <br/>
            <img src="./4k.png"/>
        </div>
    },
    {
        title: `Why does IA ask for my e-mail?`,
        tag: 'email-whiners',
        body: () => <div>
            IA requires your e-mail for identifying you for online backups. You can opt-out by clicking the "Opt out of online features" on the online tab. <br/>
            I absolutely could not care less about your e-mail, and the backend code is open source just like IA. Don't like it? Simply opt-out of online backups. <br/>
            <span style="display: none;">email, e-mail, privacy, need, login.</span>
        </div>
    },
    /*
    {
      title: `aaaaaaaaaaaaaaaaaaa`,
      tag: 'UnknownItem',
      body: () => <div>
        bbbbbbbbbbbbbbbbbbbbbbb
      </div>
    },

  */
] as IHelpEntry[];

// Converts a JSX element tree to an array of text (extract text content from <div>mytext</div> for example)
function elementToText(arg: any) {
    const children = arg?.children || [];

    if (typeof children === 'string') {
        return [children];
    }

    let result = [] as string[];
    for (const idx in children) {
        const child = children[idx];

        if (typeof child === 'string') {
            result.push(child);
        } else if (child) {
            result = result.concat(elementToText(child.props));
        }
    }

    return result;
}

// Checks if a target string contains all the words in a search string, ala "mystrike LIKE %word%word%word%"
const contains = (target: string, what: string) => {
    const args = what.split(' ');
    for (const idx in args) {
        const arg = args[idx];
        if (target.toLowerCase().indexOf(arg.toLowerCase()) === -1) {
            return false;
        }
    }

    return true;
};

interface Props {
    searchString: string;
    onSearch: (s: string) => void;
}

class Help extends PureComponent<Props, object> {
    renderHelpEntry(entry: IHelpEntry) {

        const onSelectTag = (tag: string | undefined) => {
            if (tag) {
                this.props.onSearch(tag);
            }
        };

        return (
            <div className="container" key={entry.tag}>
                <div className="header" data-helptag={entry.tag} onClick={() => onSelectTag(entry.tag)}>
                    {entry.title}
                    {entry.type === IHelpEntryType.Informational &&
                    <span className="informational">
            Informational
          </span>}
                    {entry.type === IHelpEntryType.Help &&
                    <span className="needhelp">
            Help
          </span>}
                </div>
                <div className="content">
                    {entry.body()}
                </div>
            </div>
        );
    }

    filteredEntries() {
        if (this.props.searchString.trim() === '') {
            return helpEntries.map(e => this.renderHelpEntry(e));
        }

        // Convert a JSX element to searchable text
        const toSearchableText = (elem: IHelpEntry) => [elem.title, elem.tag]
            .concat(elementToText(elem.body().props))
            .join(' ');

        // Filter items
        return helpEntries
            .filter(s => contains(toSearchableText(s), this.props.searchString))
            .map(e => this.renderHelpEntry(e));
    }

    render() {
        return <div className="help">
            <div className="form-group">
                <h2>Search:</h2>
                <input type="text" className="form-control" placeholder="Search.."
                       onInput={(e: any) => this.props.onSearch(e.target.value)} value={this.props.searchString}/>
            </div>
            {this.filteredEntries()}

            <h2>Still could not find what you were looking for?</h2>
            <a href="#" onClick={() => openUrl('https://discord.gg/5wuCPbB')} target="_blank" rel="noreferrer">Try the
                IA discord!</a>
        </div>;
    }
}

export default Help;
