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
import GrimNotParsed from "./GrimNotParsed";
import EasterEgg from "./EasterEgg";
import ModFilterWarning from "./ModFilterWarning";
import FirstRunHelpThingie from "./FirstRunHelpThingie";
import IItemAggregateRow from "../interfaces/IItemAggregateRow";
import NoMoreInstantSyncWarning from "./NoMoreInstantSyncWarning/NoMoreInstantSyncWarning";

interface ApplicationState {
  items: IItem[][];
  isLoading: boolean;
  activeTab: number;
  collectionItems: ICollectionItem[];
  itemAggregate: IItemAggregateRow[];
  isDarkMode: boolean;
  helpSearchFilter: string;
  numItems: number;
  showBackupCloudIcon: boolean;
  notifications: NotificationMessage[];
  hideItemSkills: boolean;
  isGrimParsed: boolean;
  isFirstRun: boolean;
  showModFilterWarning: number;
  hasShownModFilterWarning: boolean;
  easterEggMode: boolean;
  showNoMoreInstantSyncWarning: boolean;
}

interface IOMessage {
  type: IOMessageType;
  data: any;
}

interface IOMessageStateChange {
  type: IOMessageStateChangeType;
  value: boolean;
}

enum IOMessageType {
  ShowHelp,
  ShowMessage,
  ShowCharacterBackups,
  SetState,
  SetAggregateItemData,
  SetItems,
  SetCollectionItems,
  ShowModFilterWarning,
}

enum IOMessageStateChangeType {
  ShowCloudIcon,
  DarkMode,
  HideItemSkills,
  GrimDawnIsParsed,
  FirstRun,
  EasterEggMode,
  IsLoading,
  ShowNoMoreInstantSyncWarning,
}

interface IOMessageSetItems {
  replaceExistingItems: boolean;
  items: IItem[][];
  numItemsFound: number;
}

class App extends PureComponent<object, object> {
  state = {
    items: [],
    isLoading: true,
    activeTab: 0,
    collectionItems: [],
    itemAggregate: [],
    isDarkMode: false,
    helpSearchFilter: '',
    numItems: 0,
    showBackupCloudIcon: true,
    notifications: [],
    hideItemSkills: false,
    isGrimParsed: true,
    isFirstRun: false,
    showModFilterWarning: 0,
    hasShownModFilterWarning: false,
    easterEggMode: false,
    showNoMoreInstantSyncWarning: false,
  } as ApplicationState;

  componentDidMount() {
    // Mock data for not embedded / dev mode
    if (!isEmbedded) {
      this.setState({collectionItems: MockCollectionItemData});
    }

    // Show a notification message such as "Item transferred" or "Too close to stash"
    // @ts-ignore: showMessage doesn't exist on window
    const showMessage = (s: any) => {
      let notifications = [...this.state.notifications]
      while (notifications.length >= 8) {
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

    // @ts-ignore: message doesn't exist on window
    window.message = (input: string) => {
      let message = JSON.parse(input) as IOMessage;

      switch (message.type) {
        case IOMessageType.ShowCharacterBackups:
          this.setState({
            activeTab: 3,
          });
          break;

        case IOMessageType.ShowHelp:
          this.setState({
            activeTab: 2,
            helpSearchFilter: message.data as string,
            isLoading: false,
          });
          break;

        case IOMessageType.ShowMessage:
          showMessage(message.data);
          break;

        case IOMessageType.ShowModFilterWarning:
          // Enable "is first run" tutorial window
          if (!this.state.hasShownModFilterWarning) {
            this.setState({
              showModFilterWarning: message.data as number,
              hasShownModFilterWarning: true,
            });
          }
          break;

        case IOMessageType.SetItems: {
          let data = message.data as IOMessageSetItems;

          if (data.replaceExistingItems) {
            window.scrollTo(0, 0);
            let isFirstRun = this.state.isFirstRun && data.numItemsFound === 0;
            this.setState({
              isLoading: false,
              items: data.items,
              numItems: data.numItemsFound || 0,
              isFirstRun: isFirstRun,
            });
          } else {
            const items = [...this.state.items];
            this.setState({
              isLoading: false,
              items: items.concat(data.items)
            });
          }

          console.log("Item state is now", this.state.items);
        }
          break;

        case IOMessageType.SetCollectionItems:
          this.setState({
            collectionItems: message.data
          });
          break;

        case IOMessageType.SetAggregateItemData: {
          let data = message.data;
          const itemAggregate = typeof data === 'string' ? JSON.parse(data) : data;
          console.log('Item Aggregate:', itemAggregate);
          this.setState({
            itemAggregate: itemAggregate
          });
        }
          break;

        case IOMessageType.SetState: {
          let data = message.data as IOMessageStateChange;
          switch (data.type) {
            // TODO: This could be a lookup map.. enum => state value..
            case IOMessageStateChangeType.ShowCloudIcon:
              this.setState({
                showBackupCloudIcon: data.value
              });
              break;
            case IOMessageStateChangeType.GrimDawnIsParsed:
              this.setState({
                isGrimParsed: data.value,
                isLoading: false,
              });
              break;

            case IOMessageStateChangeType.EasterEggMode:
              this.setState({
                easterEggMode: true,
              });
              break;


            case IOMessageStateChangeType.ShowNoMoreInstantSyncWarning:
              this.setState({
                showNoMoreInstantSyncWarning: true,
              });
              break;

            case IOMessageStateChangeType.FirstRun:
              this.setState({
                isFirstRun: true,
              });
              break;
            case IOMessageStateChangeType.HideItemSkills:
              this.setState({
                hideItemSkills: data.value
              });
              break;

            case IOMessageStateChangeType.DarkMode:
              this.setState({
                isDarkMode: data.value
              });
              break;

            case IOMessageStateChangeType.IsLoading:
              this.setState({
                isLoading: data.value
              });
              break;

          }
        }
          break;
      }
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
    if (this.state.easterEggMode) {
      return <EasterEgg close={() => this.setState({easterEggMode: false})}/>;
    }

    if (this.state.showNoMoreInstantSyncWarning) {
      return <NoMoreInstantSyncWarning close={() => this.setState({showNoMoreInstantSyncWarning: false})}/>;
    }

    return (
      <div className={'App ' + (this.state.isDarkMode ? 'App-dark' : 'App-Light')}>
        <Header activeTab={this.state.activeTab} setActiveTab={(idx: number) => this.setState({activeTab: idx})}/>

        {this.state.activeTab === 0 && !this.state.isGrimParsed && <GrimNotParsed/>}
        {this.state.activeTab === 0 && this.state.isGrimParsed && this.state.isFirstRun && <FirstRunHelpThingie/>}
        {this.state.isLoading && isEmbedded && <Spinner/>}


        {this.state.activeTab === 0 && !isEmbedded ? <MockItemsButton onClick={(items) => this.setItems(items)}/> : ''}
        {this.state.activeTab === 3 && <CharacterListContainer/>}

        {this.state.activeTab === 0 && this.state.showModFilterWarning > 0 && <ModFilterWarning numOtherItems={this.state.showModFilterWarning}/>}
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
        {this.state.activeTab === 1 && <CollectionItemContainer items={this.state.collectionItems} aggregate={this.state.itemAggregate}/>}
        {this.state.activeTab === 2 && <Help searchString={this.state.helpSearchFilter} onSearch={(v: string) => this.setState({helpSearchFilter: v})}/>}

        <NotificationContainer notifications={this.state.notifications} onClose={this.closeNotification}/>
      </div>
    );
  }
};


export default App;
