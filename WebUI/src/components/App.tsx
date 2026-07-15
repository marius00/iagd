import {h} from 'preact';
import Header from './header';
import Help from "../containers/help/Help";
import IItem from "../interfaces/IItem";
import ICollectionItem from "../interfaces/ICollectionItem";
import {PureComponent} from "preact/compat";
import {isEmbedded, requestCollectionData, requestMoreItems, signalReady} from "../integration/integration";
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
import {IReplicaRow} from "../interfaces/IReplicaRow";
import GdSeasonError from "./GdSeasonError";

interface ApplicationState {
  items: IItem[][];
  itemLookupMap: Map<number, number>;
  isLoading: boolean;
  activeTab: number;
  collectionItems: ICollectionItem[];
  collectionIsHardcore: boolean;
  itemAggregate: IItemAggregateRow[];
  isDarkMode: boolean;
  helpSearchFilter: string;
  numItems: number;
  numItemsApproximate: boolean;
  hasMore: boolean;
  showBackupCloudIcon: boolean;
  notifications: NotificationMessage[];
  hideItemSkills: boolean;
  isGrimParsed: boolean;
  isFirstRun: boolean;
  showModFilterWarning: number;
  hasShownModFilterWarning: boolean;
  easterEggMode: boolean;
  gdSeasonError: boolean;
}

interface IOMessage {
  type: IOMessageType;
  data: any;
}

interface IOMessageStateChange {
  type: IOMessageStateChangeType;
  value: boolean;
}

interface IOMessageCloudIconStateChange {
  ids: number[];
}

interface IOMessageSetReplicaStats {
  id: number;
  replicaStats: IReplicaRow[];
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
  UpdateCloudIconStatus,
  UpdateItemStats,
}

enum IOMessageStateChangeType {
  ShowCloudIcon,
  DarkMode,
  HideItemSkills,
  GrimDawnIsParsed,
  FirstRun,
  EasterEggMode,
  IsLoading,
  GdSeasonError,
}

interface IOMessageSetItems {
  replaceExistingItems: boolean;
  items: IItem[][];
  numItemsFound: number;
  numItemsApproximate: boolean;
  hasMore: boolean;
}


class App extends PureComponent<object, object> {
  delayedUpdateTimer: any;
  delayedMessageQueue = [] as IOMessage[];

  state = {
    items: [],
    itemLookupMap: new Map<number,number>(),
    isLoading: true,
    activeTab: 0,
    collectionItems: [],
    collectionIsHardcore: false,
    itemAggregate: [],
    isDarkMode: false,
    helpSearchFilter: '',
    numItems: 0,
    numItemsApproximate: false,
    hasMore: false,
    showBackupCloudIcon: true,
    notifications: [],
    hideItemSkills: false,
    isGrimParsed: true,
    isFirstRun: false,
    showModFilterWarning: 0,
    hasShownModFilterWarning: false,
    easterEggMode: false,
    gdSeasonError: false,
  } as ApplicationState;

  componentDidMount() {
    // Mock data for not embedded / dev mode
    if (!isEmbedded) {
      this.setState({collectionItems: MockCollectionItemData});
    }

    signalReady()

    // Things such as real item stats and cloud sync status gets aggregated and updated every few seconds.
    // This is not critical to display realtime, and we may have hundreds of events per second during syncs
    if (!this.delayedUpdateTimer) {
      this.delayedUpdateTimer = setInterval(() => {
        const messages = [...this.delayedMessageQueue];
        this.delayedMessageQueue = [];
        if (messages.length === 0) {
          // Prevent state changes when empty
          return;
        }
        console.log("Queued messages:", messages);

        const items = [...this.state.items];
        for (let i = 0; i < messages.length; i++) {
          const message = messages[i];
          switch (message.type) {
            case IOMessageType.UpdateCloudIconStatus: {
              const playerItemIds = (message.data as IOMessageCloudIconStateChange).ids;
              for (let pidIdx = 0; pidIdx < playerItemIds.length; pidIdx++) {
                const playerItemId = playerItemIds[pidIdx];
                if (this.state.itemLookupMap.has(playerItemId)) {
                  const loc = this.state.itemLookupMap.get(playerItemId) as number;

                  for (let idx = 0; idx < this.state.items[loc].length; idx++) {
                    if (this.state.items[loc][idx].uniqueIdentifier.startsWith("PI/" + playerItemId)) {

                      // console.log("Successfully(?) marked PI " + playerItemId + " as having a cloud backup");
                      const subItems = [...items[loc]] as IItem[];
                      subItems[idx].hasCloudBackup = true;
                      items[loc] = subItems;
                    }
                  }
                }
              }
            }
              break;

            case IOMessageType.UpdateItemStats: {
              // Obs! When finding the subindex, if it's not === 0, there is no reason to re-render the view is there?
              // Gotta test if the comparison window will work if we don't re-render on a subindex change. -- Will require a 'isDirty' state to see if we call setState or not..
              const payload = message.data as IOMessageSetReplicaStats;
              const playerItemId = payload.id;
              if (this.state.itemLookupMap.has(playerItemId)) {
                const loc = this.state.itemLookupMap.get(playerItemId) as number;

                for (let idx = 0; idx < this.state.items[loc].length; idx++) {
                  if (this.state.items[loc][idx].uniqueIdentifier.startsWith("PI/" + playerItemId)) {

                    const subItems = [...items[loc]] as IItem[];
                    subItems[idx].replicaStats = payload.replicaStats;
                    subItems[idx].bodyStats = [];
                    subItems[idx].headerStats = [];
                    subItems[idx].petStats = [];
                    items[loc] = subItems;
                  }
                }
              }
            }
              break;
          } // switch
        } // for

        this.setState({items: items});
      }, 6 * 1000);
    }

    // Show a notification message such as "Item transferred" or "Too close to stash"
    // @ts-ignore: showMessage doesn't exist on window
    const showMessage = (s: any) => {
      const notifications = [...this.state.notifications]
      while (notifications.length >= 8) {
        notifications.shift();
      }

      const id = "" + Math.random();
      notifications.push({
        message: s.message,
        type: s.type,
        id: id
      });

      // If IA has focus, we don't need to keep these messages
      if (s.fade === "true") {
        setTimeout(() => {
          const notifications = [...this.state.notifications].filter(n => n.id !== id);
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
    window.message = (message: IOMessage) => {
      console.log(message, 'existing state:', this.state);
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

        case IOMessageType.UpdateItemStats:
        case IOMessageType.UpdateCloudIconStatus:
          this.delayedMessageQueue.push(message);
          break;

        case IOMessageType.SetItems: {
          const data = message.data as IOMessageSetItems;

          if (data.replaceExistingItems) {
            window.scrollTo(0, 0);
            const isFirstRun = this.state.isFirstRun && data.numItemsFound === 0;

            const lookupMap = this.calculateItemLocations(data.items, 0, undefined);
            this.setState({
              isLoading: false,
              items: data.items,
              numItems: data.numItemsFound || 0,
              numItemsApproximate: data.numItemsApproximate,
              hasMore: data.hasMore,
              isFirstRun: isFirstRun,
              itemLookupMap: lookupMap,
            });
          } else {
            const items = [...this.state.items];
            this.setState({
              isLoading: false,
              items: items.concat(data.items),
              hasMore: data.hasMore,
              // The first page may defer the exact total (shown as "1000+"); when C# later computes it
              // during pagination it sends numItemsFound >= 0 on an append to update the displayed count,
              // at which point the total is exact and the "+" is dropped.
              ...(data.numItemsFound >= 0 ? { numItems: data.numItemsFound, numItemsApproximate: false } : {}),
              itemLookupMap: this.calculateItemLocations(data.items, items.length, this.state.itemLookupMap),
            });
          }

          // If a search completes while the Collection tab is open, refresh it too (it's query-filtered).
          if (data.replaceExistingItems && this.state.activeTab === App.COLLECTION_TAB) {
            requestCollectionData();
          }

          console.log("Item state is now", this.state.items);
        }
          break;

        case IOMessageType.SetCollectionItems:
          this.setState({
            collectionItems: message.data.items,
            collectionIsHardcore: message.data.isHardcore
          });
          break;

        case IOMessageType.SetAggregateItemData: {
          const data = message.data;
          const itemAggregate = typeof data === 'string' ? JSON.parse(data) : data;
          console.log('Item Aggregate:', itemAggregate);
          this.setState({
            itemAggregate: itemAggregate
          });
        }
          break;

        case IOMessageType.SetState: {
          const data = message.data as IOMessageStateChange;
          switch (data.type) {
            // TODO: This could be a lookup map.. enum => state value..
            case IOMessageStateChangeType.ShowCloudIcon:
              /*this.setState({
                showBackupCloudIcon: data.value
              });*/
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

            case IOMessageStateChangeType.GdSeasonError:
              this.setState({
                gdSeasonError: true,
              });
              break;


            case IOMessageStateChangeType.FirstRun:
              this.setState({
                isFirstRun: data.value,
                isLoading: false,
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


  // Tab index 1 is the Collection view. Its data is fetched on demand (not on every search),
  // so request it whenever the user switches to that tab.
  static readonly COLLECTION_TAB = 1;

  setActiveTab = (idx: number) => {
    if (idx === App.COLLECTION_TAB) {
      requestCollectionData();
    }
    this.setState({activeTab: idx});
  }

  // Used primarily for setting mock items for testing
  setItems(items: IItem[]) {
    this.setState({items: items, isLoading: false});
  }

  // Creates a [playerItemId => idxPosition] map so we know where a given playerItem is located
  // This gets us to the correct row in the outer array, eliminating at least O(n) complexity in lookups.
  calculateItemLocations = (items: IItem[][], offset: number, lookupMap?: Map<number, number>): Map<number, number> => {
    const regex = /PI\/(\d+)\/.*/;

    let result = new Map<number, number>();
    if (lookupMap) {
      result = lookupMap;
    }

    for (let i = 0; i < items.length; i++) {
      for (let m = 0; m < items[i].length; m++) {
        const pid = items[i][m].uniqueIdentifier.match(regex);
        if (pid?.length === 2) {
          result.set(parseInt(pid[1]), i + offset);
        }
      }
    }

    return result;
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
    const notifications = [...this.state.notifications];

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
    if (this.state.gdSeasonError) {
      return <GdSeasonError close={() => this.setState({gdSeasonError: false})}/>;
    }


    return (
      <div className={'App ' + (this.state.isDarkMode ? 'App-dark' : 'App-Light')}>
        <Header activeTab={this.state.activeTab} setActiveTab={this.setActiveTab}/>

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
          numItemsApproximate={this.state.numItemsApproximate}
          hasMore={this.state.hasMore}
          isLoading={this.state.isLoading}
          onItemReduce={(url, transferAll) => this.reduceItemCount(url, transferAll)}
          onRequestMoreItems={() => this.requestMoreItems()}
          collectionItems={this.state.collectionItems}
          isDarkMode={this.state.isDarkMode}
          hideItemSkills={this.state.hideItemSkills}
          requestUnknownItemHelp={() => this.setState({helpSearchFilter: 'UnknownItem', activeTab: 2})}
        />}
        {this.state.activeTab === 1 && <CollectionItemContainer items={this.state.collectionItems} aggregate={this.state.itemAggregate} isHardcore={this.state.collectionIsHardcore}/>}
        {this.state.activeTab === 2 && <Help searchString={this.state.helpSearchFilter} onSearch={(v: string) => this.setState({helpSearchFilter: v})}/>}

        <NotificationContainer notifications={this.state.notifications} onClose={this.closeNotification}/>
      </div>
    );
  }
};


export default App;
