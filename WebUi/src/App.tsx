import * as React from 'react';
import './App.css';
import 'react-notifications-component/dist/theme.css';
import IItem from './interfaces/IItem';
import MockItemsButton from './containers/LoadMockItemsButton';
import { isEmbedded, requestMoreItems } from './integration/integration';
import ItemContainer from './containers/ItemContainer';
import ReactNotification, { store } from 'react-notifications-component';
import Tabs from './components/Tabs/Tabs';
import CollectionItemContainer from './containers/CollectionItemContainer';
import ICollectionItem from './interfaces/ICollectionItem';
import MockCollectionItemData from './mock/MockCollectionItemData';
import NewFeaturePromoter from './components/NewFeaturePromoter';
import Spinner from './components/Spinner';
import Help from './containers/help/Help';
import CharacterListContainer from './containers/CharacterListContainer';


export interface ApplicationState {
  items: IItem[];
  isLoading: boolean;
  activeTab: number;
  collectionItems: ICollectionItem[];
  isDarkMode: boolean;
  helpSearchFilter: string;
  numItems: number;
}


// ** Reports of the user interface freezing while searching, viable to stick it into its own thread? May be too complex to do right..
// TODO: See C#: "What's this?" May be the cause of all the "no stats found" shit.. does it ever DO anything?
// See code: TODO: Possible to get skill stuff on this? Tooltip + color
// TODO: Font color red on Set: bonus tag is freaking terrible in light mode (dark tooltip)
// TODO: Loot button if stash file path is network \\
// TODO: Onboarding => new user/returning => import flow vs parse flow, explain how it works, nag about backups, explain misc functionality.
// TODO: Icon next to transfer links?
// TODO: Maybe a UI for restoring old stash files? Would only be accessible via the help page.. unlikely but think on it.

// Trivial
// TODO: Prevent multiple clicks on transfer? or non-issue?
// TODO: Reset the 'statusLabel' text once in a while, may show 'error' until the end of time.


/*
Does buddy items use a lot of memory atm?


   at Newtonsoft.Json.Utilities.StringBuffer.Append(sIArrayPool`1 bufferPool, Char[] buffer, Int32 startIndex, Int32 count)
   at Newtonsoft.Json.JsonTextReader.ReadStringIntoBuffer(Char quote)
   at Newtonsoft.Json.JsonTextReader.ParseString(Char quote, ReadType readType)
   at Newtonsoft.Json.JsonTextReader.ParseValue()
   at Newtonsoft.Json.JsonTextReader.Read()
   at Newtonsoft.Json.Linq.JContainer.ReadContentFrom(JsonReader r, JsonLoadSettings settings)
   at Newtonsoft.Json.Linq.JContainer.ReadTokenFrom(JsonReader reader, JsonLoadSettings options)
   at Newtonsoft.Json.Linq.JObject.Load(JsonReader reader, JsonLoadSettings settings)
   at Newtonsoft.Json.Linq.JObject.Parse(String json, JsonLoadSettings settings)
   at IAGrim.BuddyShare.Synchronizer.DownloadBuddyItems(List`1 buddies)
   at IAGrim.BuddyShare.BuddyBackgroundThread.bw_DoWork(Object sender, DoWorkEventArgs e)
* */

class App extends React.PureComponent<{}, object> {
  state = {
    items: [],
    isLoading: true,
    activeTab: 0,
    collectionItems: [],
    isDarkMode: false,
    helpSearchFilter: '',
    numItems: 0,
  } as ApplicationState;

  componentDidMount() {
    // Set the items to show
    // @ts-ignore: setItems doesn't exist on window
    window.setItems = (data: any, numItems: number) => {
      const items = typeof data === 'string' ? JSON.parse(data) : data;
      console.log(data, numItems);
      console.log('==========>', numItems);
      window.scrollTo(0, 0);

      this.setState({
        isLoading: false,
        items: items,
        numItems: numItems || 0
      });
    };

    // Add more items (typically scrolling)
    // @ts-ignore: setItems doesn't exist on window
    window.addItems = (data: any) => {
      const items = [...this.state.items];
      const newItems = typeof data === 'string' ? JSON.parse(data) : data;

      this.setState({
        isLoading: false,
        items: items.concat(newItems)
      });
    };

    // Set collection items
    // @ts-ignore: setCollectionItems doesn't exist on window
    window.setCollectionItems = (data: any) => {
      const collectionItems = typeof data === 'string' ? JSON.parse(data) : data;
      this.setState({
        collectionItems: collectionItems
      });
    };

    // Mock data for not embedded / dev mode
    if (!isEmbedded) {
      this.setState({collectionItems: MockCollectionItemData});
    }

    // Start showing the loading spinner
    // @ts-ignore: setIsLoading doesn't exist on window
    window.setIsLoading = (isLoading: boolean) => {
      this.setState({
        isLoading: isLoading
      });
    };

    // Toggle dark mode from C# container
    // @ts-ignore
    window.setIsDarkmode = (isDarkMode: boolean) => {
      this.setState({
        isDarkMode: isDarkMode
      });
    };

    // Show the help screen
    // @ts-ignore: setIsLoading doesn't exist on window
    window.showHelp = (tag: string) => {
      this.setState({
        activeTab: 2,
        helpSearchFilter: tag
      });
    };

    // Show the help screen
    // @ts-ignore: setIsLoading doesn't exist on window
    window.showCharacterBackups = () => {
      this.setState({
        activeTab: 3,
      });
    };

    // Show a notification message such as "Item transferred" or "Too close to stash"
    // @ts-ignore: showMessage doesn't exist on window
    window.showMessage = (input: string) => {
      let s = JSON.parse(input);
      ShowMessage(s.message, s.type);
    };
  }

  // Used primarily for setting mock items for testing
  setItems(items: IItem[]) {
    this.setState({items: items, isLoading: false});
  }

  // Will eventually be moved into C# once the form supports darkmode
  toggleDarkmode() {
    this.setState({isDarkMode: !this.state.isDarkMode});
  }

  // reduceItemCount will reduce the number of items displayed, generally after an item has been transferred out.
  reduceItemCount(url: object[], numItems: number) {
    let items = [...this.state.items];
    var item = items.filter(e => e.url.join(':') === url.join(':'))[0];
    if (item !== undefined) {
      item.numItems -= numItems;
      this.setState({items: items});
    } else {
      console.warn('Attempted to reduce item count, but item could not be found', url);
    }
  }

  requestMoreItems() {
    console.log('More items it wantssssss?');
    this.setState({isLoading: true});
    requestMoreItems();
    // TODO: Fix this weird loop? This one will request more items.. which will end up in a call from C# to window.addItems().. is that how we wanna do this?
  }


  render() {
    console.log('tab', this.state.activeTab);
    return (
      <div className={'App ' + (this.state.isDarkMode ? 'App-dark' : 'App-Light')}>
        {this.state.isLoading && isEmbedded && <Spinner/>}
        <ReactNotification/>
        <div className={'container'}>
          <Tabs
            activeTab={this.state.activeTab}
            setActiveTab={(idx: number) => this.setState({activeTab: idx})}
            isDarkMode={this.state.isDarkMode}
            showVideoGuide={this.state.items.length <= 100}
          />
          <div id="myTabContent" className="tab-content" aria-live="polite">
          </div>
        </div>
        {this.state.activeTab === 0 && !isEmbedded ? <MockItemsButton onClick={(items) => this.setItems(items)}/> : ''}
        {this.state.activeTab === 3 && <CharacterListContainer />}

        {this.state.activeTab === 0 && <ItemContainer
            items={this.state.items}
            numItems={this.state.numItems}
            isLoading={this.state.isLoading}
            onItemReduce={(url, numItems) => this.reduceItemCount(url, numItems)}
            onRequestMoreItems={() => this.requestMoreItems()}
            collectionItems={this.state.collectionItems}
            isDarkMode={this.state.isDarkMode}
            requestUnknownItemHelp={() => this.setState({helpSearchFilter: 'UnknownItem', activeTab: 2})}
          />}

        {this.state.activeTab === 1 && <CollectionItemContainer items={this.state.collectionItems}/>}
        {this.state.activeTab === 2 && <Help searchString={this.state.helpSearchFilter} onSearch={(v: string) => this.setState({helpSearchFilter: v})}/>}


        <NewFeaturePromoter/>
      </div>
    );
  }

};

function ShowMessage(message: string, type: 'success' | 'danger' | 'info' | 'default' | 'warning') {
  let duration = 2500;
  if (type === 'danger') {
    duration = 4500;
  }

  store.addNotification({
    message: message,
    type: type,
    insert: 'top',
    container: 'bottom-center',
    animationIn: ['animate__animated', 'animate__fadeIn'],
    animationOut: ['animate__animated', 'animate__fadeOut'],
    dismiss: {
      duration: duration,
      onScreen: true
    }
  });
}

export default App;
