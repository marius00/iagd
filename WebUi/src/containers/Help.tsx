import * as React from 'react';
import './Help.css';
import { openUrl } from '../integration/integration';

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

const typicalParseDbMessage = <>{toNumberedList(`
    Close Grim Dawn
    Open IA
    Click the "Grim Dawn" tab inside IA
    Select "Vanilla" (or Forgotten Gods/AoM)
    Click "Load Database"
    Click "Update Item Stats"
    `)}
  <i>If this did not resolve your issue, try restarting IA.</i>
</>;

const helpEntries = [
  {
    title: `IA says "Stash: Error"`,
    tag: 'StashError',
    body: () => <div>
      Try running IA as administrator.
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
    title: `When will IA be updated for the latest GD patch? I cannot see some stats`,
    tag: 'GrimDawnUpdated',
    body: () => <div>
      Item Assistant rarely requires any update, you just need to parse the database again. <br/><br/>
      {typicalParseDbMessage}
    </div>,
    type: IHelpEntryType.Help
  },
  {
    title: `Will IA give me back exactly the same item, with the same stats?`,
    tag: 'ReproduceStats',
    body: () => <div>
      IA will never modify any of your items, and will always reproduce them exactly as it found them. <br/>
      Even if merging items is enabled, your items will remain unchanged. <b>Always.</b> <br/> <br/>

      However, the stats displayed in IA will vary from what you see in-game, as IA is unable to display the correct stats itself.
    </div>,
    type: IHelpEntryType.Informational
  },
  {
    title: `What does it mean to use IA on multiple PCs?`,
    tag: 'MultiplePcs',
    body: () => <div>
      The "<i>Multiple PC</i>" checkbox in Item Assistant is an addition to the backup system.<br/>
      If you have enabled backups by logging in with an e-mail, it's possible to synchronize your items between multiple PCs in ~realtime.<br/><br/>
      This can useful if you play Grim Dawn on several PCs, or if you share your stash with a loved one, as you'll be using the same IA stash across installs.<br/><br/>

      The checkbox under settings makes IA sync items more aggressively, opting for near-instant syncronization instead of low bandwidth usage.
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
    title: `What does it mean for IA to delete duplicates?`,
    tag: 'DeleteDuplicates',
    body: () => <div>
      Sometimes bugs can happen which causes an <u>exact duplicate</u> of an item to be spawned. <br/>
      This is an item with the exact same stats etc, it's not an item that was actually found, but was <u>spawned due to a bug.</u><br/><br/>


      Enabling this option under settings will cause IA to simply delete these bugged items, instead of just warning you.<br/>
      IA will not loot items it detects to have been spawned due to a duplication bug.
    </div>,
    type: IHelpEntryType.Informational
  },
  {
    title: `I can't find my items!`,
    tag: 'CantFindItemsMod',
    body: () => <div>
      {toNumberedList(`
      Restart IA
      Make sure you've selected the right mod (upper right corner, most likely Vanilla)`)}

      Typically it's the mod selection that causes items to not be found, as IA supports separating items from both mods and softcore/hardcore.
    </div>,
    type: IHelpEntryType.Help
  },
  {
    title: `IA is not looting my items!`,
    tag: 'CantFindItemsMod2', // TODO: Why is this duplicated?
    body: () => <div>
      {toNumberedList(`
      Place items in your stash
      Walk away from your stash area
      IA should now have looted these items
      The status bar in IA should indicate it just looted some items`)}

      <b className="attention">If this did not resolve your issue, please try running IA as administrator.</b> <br/>
      In some cases IA requires additional permissions in order to run properly.
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

      This folder contains both daily backups from Item Assistant, as well as all your previous shared stash files.<br/>
      It is <span className="attention">highly recommended</span> that you enable additional backups. <br/>
      Harddrive failures happens. Viruses happens. Reinstalling windows and forgetting to copy IA happens. <br/><br/>
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
      The way IA creates backups to say Dropbox is by detecting the installation folder, and placing a zip file there with the daily backup.<br/>
      The same can be achieved by choosing "custom" and simply navigating to the folder of your cloud provider.<br/>
      <br/>
      If the icon is disabled, it means IA was not able to automatically detect where you have the cloud sync installed, and you will have to enable it manually via custom backups.
    </div>,
    type: IHelpEntryType.Informational
  },
  {
    title: `How do i restore a transfer stash? (lost items from tab1 / tab2 for example)`,
    tag: 'RestoreTransferStash',
    body: () => <div>
      {toNumberedList(`
      Close Grim Dawn (or at least go to the main menu)
      In IA go to settings
      Click "View Logs"
      Enter the "backup" folder
      You're looking for a file named transfer-something, look at the timestamps to find the correct one
      Once you think you've found the correct transfer copy, go to "my documents"\\"my games"\\"grim dawn"\\"save"
      Find transfer.gst and rename it to transfer-renamed.gst
      Copy the transfer file from the backup folder into the save\\ folder
      Rename the copied file to "transfer.gst"
      Open Grim dawn and go pick up your items from the stash
      Close grim dawn
      Delete the copied file, and rename the transfer-renamed.gst to transfer.gst to restore your stash`)}
      <br/>
      IA keeps the last 1000 copies of your transfer stash, so you may need to guess which file you need based on the timestamp.<br/>
      Remember: only you know when this happened.
    </div>,
    type: IHelpEntryType.Informational
  },

  {
    title: `IA says cloud saving is enabled`,
    tag: 'CloudSavesEnabled',
    body: () => <div>
      For Grim Dawn, cloud saving has to be disabled in <u>two</u> different locations.
      {toNumberedList(`In steam
        Inside Grim Dawn itself, under "Settings"`)}
      Grim Dawn uses a different save directory with cloud saving disabled.<br/>
      After disabling cloud saving you still need to copy your characters to the correct directory.<br/>
    </div>,
    type: IHelpEntryType.Help
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
      You simply need to copy them to a new folder, and everything will be right in the world.
    </div>,
    type: IHelpEntryType.Help
  },
  {
    title: `How do I backup my items?`,
    tag: 'HowDoIBackup',
    body: () => <div>
      <b>IA can automatically backup your items to your favorite cloud provider, or to a custom folder (for example on your desktop or a USB stick)</b><br/>
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
      Some super paranoid anti virus / anti malware programs tends to handle this by simply deleting Item Assistant.<br/>
      They never tell you, so you never realize what the cause is.<br/><br/>
      The main culprit here is Avast, but other anti virus programs has been known to sometimes freak out as well. <br/>
      The only way to notice is by going into the logs of the anti virus and see that it has interferred without notifying you.<br/><br/>
      In order to continue using IA, you must whitelist it in whatever god forsaken anti virus program you're using, and then completely reinstall IA.<br/>
      If you are reading this message, your "anti" virus program has already wiped out half the things IA needs to keep working properly.<br/><br/>
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
    title: `Can I move items from Hardcore to Softcore? (or vice versa)`,
    tag: 'SoftcoreToHardcore',
    body: () => <div>
      By default this is not enabled. <br/>
      However, if you enable the "<i>Transfer to any mod</i>" under settings, it is possible to transfer items anywhere you'd like.
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
    title: `Settings: What is secure transfers?`,
    tag: 'SecureTransfers',
    body: () => <div>
      <b>Disabling the <i>Secure Transfers</i> option lets IA transfer items even when the stash status is open or unknown</b><br/>
      This is an override provided for extreme cases where IA cannot detect the Grim Dawn stash status.<br/><br/>
      If secure transfers is disabled, <u>it is up to the user to make sure the stash is closed</u>, or risk losing items during transfer.<br/><br/>
      <span className="attention">This is an emergency option only.</span>
    </div>,
    type: IHelpEntryType.Informational
  },
  {
    title: `Settings: What is transfer to any mod?`,
    tag: 'TransferToAnyMod',
    body: () => <div>
      <b>Transfer to any mod lets you transfer items between the regular campaign and your mods.</b><br/>
      This is not enabled by default, as it could be considered cheating by some.
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
      <b>Recommended for those who always wish to be on the latest version, and don't mind upgrading frequently.</b>
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
      To enable automatic backup of your items, all you need to do is input your e-mail address, and then input the one-time code which will be sent to you. <br/>
      <br/>
      Any time you loot an item it will be synced up to the cloud. If anything happens and you need to reinstall IA, all you need to do is login with the same e-mail address, and IA will automagically download all of your items again.
      <br/>
      This functionality can also be used to share items between multiple computers, using the "Using multiple PCs" setting.
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
  {
    title: `aaaaaaaaaaaaaaaaaaa`,
    tag: 'UnknownItem',
    body: () => <div>
      bbbbbbbbbbbbbbbbbbbbbbb
    </div>
  },
  {
    title: `aaaaaaaaaaaaaaaaaaa`,
    tag: 'UnknownItem',
    body: () => <div>
      bbbbbbbbbbbbbbbbbbbbbbb
    </div>
  },
  {
    title: `aaaaaaaaaaaaaaaaaaa`,
    tag: 'UnknownItem',
    body: () => <div>
      bbbbbbbbbbbbbbbbbbbbbbb
    </div>
  },
  {
    title: `aaaaaaaaaaaaaaaaaaa`,
    tag: 'UnknownItem',
    body: () => <div>
      bbbbbbbbbbbbbbbbbbbbbbb
    </div>
  },
  {
    title: `aaaaaaaaaaaaaaaaaaa`,
    tag: 'UnknownItem',
    body: () => <div>
      bbbbbbbbbbbbbbbbbbbbbbb
    </div>
  },
  {
    title: `aaaaaaaaaaaaaaaaaaa`,
    tag: 'UnknownItem',
    body: () => <div>
      bbbbbbbbbbbbbbbbbbbbbbb
    </div>
  },
  {
    title: `aaaaaaaaaaaaaaaaaaa`,
    tag: 'UnknownItem',
    body: () => <div>
      bbbbbbbbbbbbbbbbbbbbbbb
    </div>
  },
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

class Help extends React.PureComponent<Props, object> {
  renderHelpEntry(entry: IHelpEntry) {
    return (
      <div className="container" key={entry.tag}>
        <div className="header" data-helptag={entry.tag}>
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
      return helpEntries.map(this.renderHelpEntry);
    }

    // Convert a JSX element to searchable text
    const toSearchableText = (elem: IHelpEntry) => [elem.title, elem.tag]
      .concat(elementToText(elem.body().props))
      .join(' ');

    // Filter items
    return helpEntries
      .filter(s => contains(toSearchableText(s), this.props.searchString))
      .map(this.renderHelpEntry);
  }

  render() {
    return <div className="help">
      <div className="form-group">
        <h2>Search:</h2>
        <input type="text" className="form-control" placeholder="Search.." onChange={(e) => this.props.onSearch(e.target.value)} value={this.props.searchString}/>
      </div>
      {this.filteredEntries()}

      <h2>Still could not find what you were looking for?</h2>
      <a href="#" onClick={() => openUrl("https://discord.gg/5wuCPbB")} target="_blank" rel="noreferrer">Try the IA discord!</a>
    </div>;
  }
}

export default Help;