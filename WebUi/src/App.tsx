import * as React from 'react';
import './App.css';
import 'react-notifications-component/dist/theme.css';
import IItem from './interfaces/IItem';
import MockItemsButton from './containers/MockItemsButton';
import { isEmbedded, requestMoreItems } from './integration/integration';
import ItemContainer from './containers/ItemContainer';
import ReactNotification, { store } from 'react-notifications-component';
import Tabs from './components/Tabs/Tabs';
import CollectionItemContainer from './containers/CollectionItemContainer';
import ICollectionItem from './interfaces/ICollectionItem';
import MockCollectionItemData from './mock/MockCollectionItemData';
import NewFeaturePromoter from './components/NewFeaturePromoter';
import Spinner from './components/Spinner';
import Help from './containers/Help';


export interface ApplicationState {
  items: IItem[];
  isLoading: boolean;
  activeTab: number;
  collectionItems: ICollectionItem[];
  isDarkMode: boolean;
  helpSearchFilter: string;
}


// TODO: Am I actually using this store??
const StoreContext = React.createContext({items: [], isLoading: true, activeTab: 0, collectionItems: [], isDarkMode: false, helpSearchFilter: ''} as ApplicationState);

// TODO: Help Tab, how does online backups work? -- maybe split it into "generic info" and "freakout questions"
// See code: TODO: Possible to get skill stuff on this? Tooltip + color
// TODO: Font color red on Set: bonus tag is freaking terrible in light mode (dark tooltip)
// TODO: Dark mode
// TODO: Basic tab-usage analytics, determine if collection or crafting is being used
// TODO: Move help tab into WebUI? "[?]" links can trigger a modal on the help page. Add more screenshots.
// TODO: A commit redoing all the damn bracket styles in C# -- do this last.. sigh.
// TODO: Ensure loading works correctly.. no duplicates, in expected order..
// TODO: Look into the duplicate key errors -- seems to be for items with&without a component. dupe item but with comp.
// TODO: Loot button if stash file path is network \\
// TODO: Onboarding => new user/returning => import flow vs parse flow, explain how it works, nag about backups, explain misc functionality.
// TODO: Icon next to transfer links?
// TODO: Maybe a UI for restoring old stash files? Would only be accessible via the help page.. unlikely but think on it.

// Trivial
// TODO: Prevent multiple clicks on transfer? or non-issue?
// TODO: Reset the 'statusLabel' text once in a while, may show 'error' until the end of time.
// TODO: A no more matches message when scrolling too far?


class App extends React.PureComponent<{}, object> {
  state = {
    items: [],
    isLoading: true,
    activeTab: 0,
    collectionItems: [],
    isDarkMode: false,
    helpSearchFilter: '',
  } as ApplicationState;

  componentDidMount() {
    // Set the items to show
    // @ts-ignore: setItems doesn't exist on window
    window.setItems = (data: any) => {
      const items = typeof data === 'string' ? JSON.parse(data) : data;

      this.setState({
        isLoading: false,
        items: items
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

    // Show the help screen
    // @ts-ignore: setIsLoading doesn't exist on window
    window.showHelp = (tag: string) => {
      this.setState({
        activeTab: 3,
        helpSearchFilter: tag
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
    console.log('swappy', this.state.isDarkMode);
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
    return (
      <div className={"App " + (this.state.isDarkMode ? "App-dark" : "")}>
        {this.state.isLoading && isEmbedded && <Spinner />}
        <ReactNotification/>
        <StoreContext.Provider value={this.state}>
          <div className={'container'}>
            <Tabs
              activeTab={this.state.activeTab}
              setActiveTab={(idx: number) => this.setState({activeTab: idx})}
              isDarkMode={this.state.isDarkMode}
              toggleDarkmode={() => this.toggleDarkmode()}
              showVideoGuide={this.state.items.length <= 100}
            />
            <div id="myTabContent" className="tab-content" aria-live="polite">
            </div>
          </div>
          {this.state.activeTab === 0 && !isEmbedded ? <MockItemsButton onClick={(items) => this.setItems(items)}/> : ''}

          <div style={this.state.activeTab === 0 ? {display: 'block'} : {display: 'none'}}>
            <ItemContainer
              items={this.state.items}
              isLoading={this.state.isLoading}
              onItemReduce={(url, numItems) => this.reduceItemCount(url, numItems)}
              onRequestMoreItems={() => this.requestMoreItems()}
              collectionItems={this.state.collectionItems}
              isDarkMode={this.state.isDarkMode}
            />
          </div>

          {this.state.activeTab === 1 && <CollectionItemContainer items={this.state.collectionItems} />}

          {this.state.activeTab === 2 && <div className="legacy-crafting">
            <h2>Legacy functionality</h2>
            <p>This used to be a part of IA, automagically detecting which components you already had. <br/>
              If you'd like to see to see the functionality return, give a shout out!</p>
            <iframe title="Crafting tab" src="https://items.dreamcrash.org/ComponentAssembler?record=d009_relic.dbr"
                    style={{width: '100%', height: '100%', overflow: 'hidden', position: 'absolute'}} scrolling="yes" frameBorder={0}/>
          </div>}

          {this.state.activeTab === 3 && <Help searchString={this.state.helpSearchFilter} onSearch={(v: string) => this.setState({helpSearchFilter: v})} />}

        </StoreContext.Provider>

        <NewFeaturePromoter />
      </div>
    );
  }

};

function ShowMessage(message: string, type: 'success' | 'danger' | 'info' | 'default' | 'warning') {
  store.addNotification({
    title: type,
    message: message,
    type: type,
    insert: 'top',
    container: 'bottom-center',
    animationIn: ['animate__animated', 'animate__fadeIn'],
    animationOut: ['animate__animated', 'animate__fadeOut'],
    dismiss: {
      duration: 2500,
      onScreen: true
    }
  });
}

export default App;
