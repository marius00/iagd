import {h} from 'preact';
import Header from './header';
import Help from "../containers/help/Help";
import IItem from "../interfaces/IItem";
import ICollectionItem from "../interfaces/ICollectionItem";
import {PureComponent, Suspense, lazy} from "preact/compat";
import {dismissNumericFilterBanner, isEmbedded, requestCollectionData, requestMoreItems, signalReady} from "../integration/integration";
import MockCollectionItemData from "../mock/MockCollectionItemData";
import Spinner from "./Spinner";
import '../style/App.css';
import MockItemsButton from "./LoadMockItemsButton";
import CharacterListContainer from "../containers/CharacterListContainer";
import ItemContainer from "../containers/ItemContainer";
import CollectionItemContainer from "../containers/CollectionItemContainer";
import NotificationContainer, {NotificationMessage} from "./NotificationComponent";
import GrimNotParsed from "./GrimNotParsed";
import ModFilterWarning from "./ModFilterWarning";
import FirstRunHelpThingie from "./FirstRunHelpThingie";
import IItemAggregateRow from "../interfaces/IItemAggregateRow";
import {IReplicaRow} from "../interfaces/IReplicaRow";
import GdSeasonError from "./GdSeasonError";
import NumericFilterBanner from "./NumericFilterBanner";

// Split into its own chunk: a once-a-year gag should not put its stylesheet (a ~94 KB embedded image) into
// the bundle on every launch. If the chunk fails to load the gag is simply skipped, which is fine.
const EasterEgg = lazy(() => import("./EasterEgg"));

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
  modFilterWarningDismissed: boolean;
  easterEggMode: boolean;
  gdSeasonError: boolean;
  showNumericFilterBanner: boolean;
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
  ShowNumericFilterBanner,
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
    modFilterWarningDismissed: false,
    easterEggMode: false,
    gdSeasonError: false,
    showNumericFilterBanner: false,
  } as ApplicationState;

  componentDidMount() {
    // Mock data for not embedded / dev mode
    if (import.meta.env.DEV && !isEmbedded) {
      this.setState({collectionItems: MockCollectionItemData});
    }

    signalReady()

    // The set-bonus tooltip ("This set consists of the following items:") resolves each set
    // member's name and owned-count against collectionItems. That data is otherwise only fetched
    // when the Collection tab is opened, so request it up front — otherwise the tooltip on the
    // Items tab renders every member as an unresolved "0x" with no name.
    //
    // Held back until the browser is idle: it is thousands of rows to serialize, ship and parse, it is
    // only needed once the user hovers a set item, and doing it inline competes with the first paint.
    if (isEmbedded) {
      const idle = (window as any).requestIdleCallback;
      if (idle) {
        idle(() => requestCollectionData(), {timeout: 5000});
      } else {
        setTimeout(() => requestCollectionData(), 1000);
      }
    }

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
        const items = [...this.state.items];

        // Item is a PureComponent, so an updated item has to be a new object for the change to be picked up;
        // editing the existing one in place leaves its identity untouched and the row keeps its stale contents.
        const replaceItem = (loc: number, idx: number, replacement: IItem) => {
          const subItems = [...items[loc]] as IItem[];
          subItems[idx] = replacement;
          items[loc] = subItems;
        };

        for (let i = 0; i < messages.length; i++) {
          const message = messages[i];
          switch (message.type) {
            case IOMessageType.UpdateCloudIconStatus: {
              const playerItemIds = (message.data as IOMessageCloudIconStateChange).ids;
              for (let pidIdx = 0; pidIdx < playerItemIds.length; pidIdx++) {
                const playerItemId = playerItemIds[pidIdx];
                if (this.state.itemLookupMap.has(playerItemId)) {
                  const loc = this.state.itemLookupMap.get(playerItemId) as number;

                  for (let idx = 0; idx < items[loc].length; idx++) {
                    if (items[loc][idx].uniqueIdentifier.startsWith("PI/" + playerItemId)) {
                      replaceItem(loc, idx, {...items[loc][idx], hasCloudBackup: true});
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

                for (let idx = 0; idx < items[loc].length; idx++) {
                  if (items[loc][idx].uniqueIdentifier.startsWith("PI/" + playerItemId)) {
                    replaceItem(loc, idx, {
                      ...items[loc][idx],
                      replicaStats: payload.replicaStats,
                      bodyStats: [],
                      headerStats: [],
                      petStats: [],
                    });
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
      // Deliberately not logging the message or state here: this fires for every inbound message (hundreds
      // per second during a sync) and each call pins the whole item list in the console's retained buffer.
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
          // Shown per search (SetItems clears it again), but once the user has dismissed it we stay
          // quiet for the rest of the session rather than re-nagging on every subsequent search.
          if (!this.state.modFilterWarningDismissed) {
            this.setState({
              showModFilterWarning: message.data as number,
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
              // The warning belongs to a single search. C# sends SetItems first and only then decides
              // whether to raise it, so clearing here keeps a stale banner from outliving its search.
              showModFilterWarning: 0,
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

            case IOMessageStateChangeType.ShowNumericFilterBanner:
              this.setState({
                showNumericFilterBanner: data.value,
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
    this.setState({isLoading: true});
    requestMoreItems();
    // TODO: Fix this weird loop? This one will request more items.. which will end up in a call from C# to window.addItems().. is that how we wanna do this?
  }

  closeModFilterWarning = () => {
    this.setState({showModFilterWarning: 0, modFilterWarningDismissed: true});
  }

  closeNumericFilterBanner = () => {
    this.setState({showNumericFilterBanner: false});
    dismissNumericFilterBanner();
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
      return <Suspense fallback={null}><EasterEgg close={() => this.setState({easterEggMode: false})}/></Suspense>;
    }
    if (this.state.gdSeasonError) {
      return <GdSeasonError close={() => this.setState({gdSeasonError: false})}/>;
    }


    return (
      <div className={'App ' + (this.state.isDarkMode ? 'App-dark' : 'App-light')}>
        <Header activeTab={this.state.activeTab} setActiveTab={this.setActiveTab}/>

        {this.state.activeTab === 0 && !this.state.isGrimParsed && <GrimNotParsed/>}
        {this.state.activeTab === 0 && this.state.isGrimParsed && this.state.isFirstRun && <FirstRunHelpThingie/>}
        {this.state.isLoading && isEmbedded && <Spinner/>}


        {import.meta.env.DEV && this.state.activeTab === 0 && !isEmbedded ? <MockItemsButton onClick={(items) => this.setItems(items)}/> : ''}
        {this.state.activeTab === 3 && <CharacterListContainer/>}

        {this.state.activeTab === 0 && this.state.showModFilterWarning > 0 && <ModFilterWarning numOtherItems={this.state.showModFilterWarning} close={this.closeModFilterWarning}/>}
        {this.state.activeTab === 0 && this.state.showNumericFilterBanner && <NumericFilterBanner close={this.closeNumericFilterBanner}/>}
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
