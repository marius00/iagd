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
            Item Assistant only supports the 64 bit version of Grim Dawn. <br/>
            To ensure that you always run the 64-bit version, you can add "/x64" as a launch option in steam.
            <img src="./x64steam.png"/> <br/>
            If you believe you are already running Grim Dawn x64, you are mistaken. Add the launch parameters. You'll see (x64) in the bottom right corner in-game.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `IA says "Base stats"`,
        tag: 'GenericStatNoReplica',
        body: () => <div>
            When an item says "Base stats", IA is not yet able to <u>display</u> the true stats of this item. <br/>
            Although Item Assistant is always able to exactly reproduce your item, displaying the true stats inside IA requires some assistance from Grim Dawn <br/>
            As you are playing the game, IA will automatically start fetching the real display stats of your items. <br/>
            While this is happening, IA will display the base values for items.<br/><br/>
            Transferring the item back into the game will always reproduce the exact item you put in.
        </div>,
        type: IHelpEntryType.Help
    },
    {
        title: `IA returns items immediately`,
        tag: 'ItemClassificationReturn',
        body: () => <div>
            If you're encountering the error "Deposited item back in-game, did not pass item classification.", it most likely means you need to parse the database. <br/>
            IA will return any items it cannot identify, since you'd be unable to search for them based on stats. <br/> <br/>
            It might also be a DLL injection issue, check the log for messages regarding injection issues.
            <br/>
            {typicalParseDbMessage}
        </div>,
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
      title: `What is the settings "delay when searching?"`,
      tag: 'DelayWhenSearching',
      body: () => <div>
        The "<i>delay when searching</i>" checkbox in Item Assistant will introduce a small delay when using the left-side checkboxes.<br/>
        This may help improve performance when you have many (100k) items, or are running on a lower end PC.
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
        title: `Settings: What is th e difference between regular and experimental updates?`,
        tag: 'RegularUpdates',
        body: () => <div>
            With regular updates you'll occationally be notified of a new version of IA. <br/>
            You'll always get the latest version, but it may take a couple of weeks before you're notified. <br/>
            <b>Recommended for those who don't wish to be bothered with frequent updates.</b>
			<br/><br/>
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
        title: `Can I store items on a different harddrive?`,
        tag: 'StoreOnDifferentDisk',
        body: () => <div>
            Sorta..<br/><br/>
            This is very niche / power user, but it's possible.. <br/>
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
        title: `Can I use this with a 4K monitor?`,
        tag: '4K',
        body: () => <div>
            Item Assistant may not function correctly with 4K monitors and UI scaling. <br/>
            Right click Item Assistant, got to properties and then Compatibility mode. <br/>
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
    {
        title: `Why can't I find translations / IA is only in English?`,
        tag: 'translations',
        body: () => <div>
            With the release of Grim Dawn v1.2 <i>Crate</i> changed how translations works with Grim Dawn.<br/>
            Item Assistant expects translations to work the way they used to in v1.0 and v1.1 <br/>
            <br/>
            If you wish to run IAGD with a different language than English, you'll need to follow the steps used to translate Grim Dawn back with v1.1 <br/>
            This involves finding the community zip file for your language, downloading that and placing it in a folder inside Grim Dawn.<br/>
            <br/>
            This is out of scope of IAGD itself, you can google guides on how to install translations for v1.1<br/><br/>
            Not all translations has full support for IAGD.
        </div>
    },
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
