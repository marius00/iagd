import {h} from 'preact';
import Header from './header';
import Help from "../containers/help/Help";
import IItem from "../interfaces/IItem";
import ICollectionItem from "../interfaces/ICollectionItem";
import {PureComponent} from "preact/compat";
import {isEmbedded, requestMoreItems} from "../integration/integration";
import MockCollectionItemData from "../mock/MockCollectionItemData";
import Spinner from "./Spinner";
import '../style/App.css';
import MockItemsButton from "./LoadMockItemsButton";
import CharacterListContainer from "../containers/CharacterListContainer";
import ItemContainer from "../containers/ItemContainer";
import CollectionItemContainer from "../containers/CollectionItemContainer";
import NotificationContainer, {NotificationMessage} from "./NotificationComponent";

interface ApplicationState {
  items: IItem[][];
  isLoading: boolean;
  activeTab: number;
  collectionItems: ICollectionItem[];
  isDarkMode: boolean;
  helpSearchFilter: string;
  numItems: number;
  showBackupCloudIcon: boolean;
  notifications: NotificationMessage[];
  hideItemSkills: boolean;
}

class App extends PureComponent<object, object> {
  state = {
    items: [],
    isLoading: true,
    activeTab: 0,
    collectionItems: [],
    isDarkMode: false,
    helpSearchFilter: '',
    numItems: 0,
    showBackupCloudIcon: true,
    notifications: [],
    hideItemSkills: false
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

    // Toggle "hide skills" from C# container
    // @ts-ignore
    window.setHideItemSkills = (hideItemSkills: boolean) => {
      this.setState({
        hideItemSkills: hideItemSkills
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

    // @ts-ignore:
    window.setOnlineBackupsEnabled = (enabled: boolean) => {
      this.setState({
        showBackupCloudIcon: enabled
      });
    };

    // Show a notification message such as "Item transferred" or "Too close to stash"
    // @ts-ignore: showMessage doesn't exist on window
    window.showMessage = (input: string) => {
      let s = JSON.parse(input);

      let notifications = [...this.state.notifications]
      while (notifications.length >= 15) {
        notifications.shift();
      }

      let id = "" + Math.random();
      notifications.push({
        message: s.message,
        type: s.type,
        id: id
      });

      // If IA has focus, we don't need to keep these messages
      if (s.fade === "true") {
        setTimeout(() => {
          let notifications = [...this.state.notifications].filter(n => n.id !== id);
          this.setState({
            notifications: notifications
          });

        }, 3500);
      }

      this.setState({
        notifications: notifications
      });
    };
  }


  // Used primarily for setting mock items for testing
  setItems(items: IItem[]) {
    this.setState({items: items, isLoading: false});
  }

  itemSort(a: IItem, b: IItem) {
    if (a.type !== b.type) {
      // Highest number wins (PlayerItem is id 2, BudddyItem is id 1, so playeritems gets first)
      return b.type - a.type;
    }

    // Don't care, as long as its consistent.
    return a.uniqueIdentifier < b.uniqueIdentifier;
  }

  /**
   * Find the index of a given item
   */
  findIndex(item: IItem) {
    const items = this.state.items;
    for (let idx = 0; idx < items.length; idx++) {
      // Assumes there are no empty arrays
      if (items[idx][0].mergeIdentifier === item.mergeIdentifier) {
        return idx;
      }
    }

    return -1;
  }

  reduceItemCount(item: IItem, transferAll: boolean) {
    const itemIdx = this.findIndex(item);
    if (itemIdx === -1) {
      console.log("Something went terribly horribly wrong locating item idx for", item);
      return;
    }

    let itemArray;
    if (transferAll) {
      // Filter out all playeritems
      itemArray = [...this.state.items[itemIdx]].filter(m => m.type !== 2);
    } else {
      // Filter out specific item
      itemArray = [...this.state.items[itemIdx]].filter(m => m.uniqueIdentifier !== item.uniqueIdentifier);
    }


    if (itemArray.length === 0) {
      const stateItems = [...this.state.items];
      stateItems.splice(itemIdx, 1);
      this.setState({items: stateItems});
    } else {
      const stateItems = [...this.state.items];
      stateItems[itemIdx] = itemArray;
      this.setState({items: stateItems});
    }
  }

  requestMoreItems() {
    console.log('More items it wantssssss?');
    this.setState({isLoading: true});
    requestMoreItems();
    // TODO: Fix this weird loop? This one will request more items.. which will end up in a call from C# to window.addItems().. is that how we wanna do this?
  }

  closeNotification = (id?: string) => {
    let notifications = [...this.state.notifications];

    if (id) {
      this.setState({
        notifications: notifications.filter(n => n.id !== id)
      });
    } else {

      this.setState({
        notifications: []
      });
    }
  }


  render() {
    return (
      <div className={'App ' + (this.state.isDarkMode ? 'App-dark' : 'App-Light')}>
        <Header
          activeTab={this.state.activeTab}
          setActiveTab={(idx: number) => this.setState({activeTab: idx})}
          showVideoGuide={this.state.items.length <= 100}
        />
        {this.state.isLoading && isEmbedded && <Spinner/>}


        {this.state.activeTab === 0 && !isEmbedded ? <MockItemsButton onClick={(items) => this.setItems(items)}/> : ''}
        {this.state.activeTab === 3 && <CharacterListContainer/>}

        {this.state.activeTab === 0 && <ItemContainer
            showBackupCloudIcon={this.state.showBackupCloudIcon}
            items={this.state.items}
            numItems={this.state.numItems}
            isLoading={this.state.isLoading}
            onItemReduce={(url, transferAll) => this.reduceItemCount(url, transferAll)}
            onRequestMoreItems={() => this.requestMoreItems()}
            collectionItems={this.state.collectionItems}
            isDarkMode={this.state.isDarkMode}
            hideItemSkills={this.state.hideItemSkills}
            requestUnknownItemHelp={() => this.setState({helpSearchFilter: 'UnknownItem', activeTab: 2})}
        />}
        {this.state.activeTab === 1 && <CollectionItemContainer items={this.state.collectionItems}/>}
        {this.state.activeTab === 2 && <Help searchString={this.state.helpSearchFilter}
                                             onSearch={(v: string) => this.setState({helpSearchFilter: v})}/>}

        <NotificationContainer notifications={this.state.notifications} onClose={this.closeNotification}/>
      </div>
    );
  }
};


export default App;
