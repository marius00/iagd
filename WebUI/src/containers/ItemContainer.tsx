import {h} from "preact";
import Item, { getUniqueId } from '../components/Item/Item';
import IItem from '../interfaces/IItem';
import './ItemContainer.css';
import './ReplicaStat.css';
import ReactTooltip from 'react-tooltip';
import translate from '../translations/EmbeddedTranslator';
import { setClipboard, transferItem } from '../integration/integration';
import OnScrollLoader from '../components/OnScrollLoader';
import ICollectionItem from '../interfaces/ICollectionItem';
import {PureComponent} from "preact/compat";
import ItemComparer from "../components/Item/ItemComparer";
import IItemType from "../interfaces/IItemType";

interface Props {
  items: IItem[][];
  numItems?: number;
  numItemsApproximate?: boolean;
  hasMore?: boolean;
  isLoading: boolean;
  onItemReduce(item: IItem, transferAll: boolean): void;
  onRequestMoreItems(): void;
  collectionItems: ICollectionItem[];
  isDarkMode: boolean;
  requestUnknownItemHelp: () => void;
  showBackupCloudIcon: boolean;
  hideItemSkills: boolean;
}


class ItemContainer extends PureComponent<Props, object> {
  state = {
    isComparing: false,
    compareItem: '',
  }

  // The collection list findByRecord is currently indexed against, compared by identity.
  private indexedCollection: ICollectionItem[] | null = null;

  // Only ever one rebuild pending. Previously every update queued another, and each one walks the DOM.
  private tooltipRebuildTimer: ReturnType<typeof setTimeout> | null = null;

  private static readonly MissingCollectionItem: ICollectionItem =
    {baseRecord: "", name: "", icon: "", numOwnedSc: 0, numOwnedHc: 0, quality: ''};

  // TODO: The state should maybe say if these are NEW or MODIFIED items, to support transferring multiple items?
  componentWillReceiveProps(nextProps: any, nextState: any) {
    if (this.state.isComparing) {
      this.setState({isComparing: false});
    }
  }

  // Arrow-bound so each item keeps the same handler identity across renders. Item is a PureComponent, and
  // rebuilding these inline per render made its shallow prop compare fail every time, re-rendering the
  // entire list on any state change.
  transferSingleWrapper = (item: IItem[]) => {
    // Switch to comparison dialogue
    if (item.length > 1) {
      this.setState({
        isComparing: true,
        compareItem: item[0].mergeIdentifier,
      });
    } else {
      // Only one item
      this.transferSingle(item[0]);
    }
  }

  transferSingle = (item: IItem) => {
    const id = item.uniqueIdentifier + '/-/-/-';
    const url = (id.split('/') as any) as object[];
    const r = transferItem(url, false);
    if (r.success) {
      this.props.onItemReduce(item, false);
    }
  }

  transferAll = (item: IItem[]) => {
    const url = (item[0].url as any) as object[];
    const r = transferItem(url, true);
    if (r.success) {
      this.props.onItemReduce(item[0], true); // Don't particularly matter which we reduce when doing transferAll
    }
  }

  requestUnknownItemHelp = () => this.props.requestUnknownItemHelp();

  componentDidUpdate(props: Props) {
    this.scheduleTooltipRebuild();
  }

  componentWillUnmount() {
    if (this.tooltipRebuildTimer !== null) {
      clearTimeout(this.tooltipRebuildTimer);
      this.tooltipRebuildTimer = null;
    }
  }

  private scheduleTooltipRebuild() {
    if (this.tooltipRebuildTimer !== null) {
      clearTimeout(this.tooltipRebuildTimer);
    }

    this.tooltipRebuildTimer = setTimeout(() => {
      this.tooltipRebuildTimer = null;
      ReactTooltip.rebuild(); // TODO: This seems like a stupid way to solve tooltip issues.
    }, 1250);
  }


  getClipboardContent() {
    const colors: {[index: string]:string} = { Epic: 'DarkOrchid', Blue: 'RoyalBlue', Green: 'SeaGreen', Unknown: '', Yellow: 'Yellow' };

    const entries = this.props.items.map(item => {
      const name = item[0].name.replace('"', '');
      return `[URL="https://grimdawn.evilsoft.net/search/?query=${name}"][COLOR="${colors[item[0].quality]}"]${item[0].name}[/COLOR][/URL]`;
    });

    return entries.join('\n');
  }

  /**
   * Keeps the base-record index, and the lookup handed to each Item, in step with the collection list.
   *
   * The handler identity has to change with the collection and not otherwise. Item is a PureComponent, so a
   * handler that never changes would leave every set-bonus tooltip showing its pre-collection "0x" placeholder
   * once the collection arrives, while one that changes every render re-renders the whole list on any update.
   */
  private ensureCollectionIndex() {
    const collectionItems = this.props.collectionItems;
    if (this.indexedCollection === collectionItems) {
      return;
    }

    // The tooltip resolves every member of every set on screen through here, so scanning the collection
    // (thousands of entries) per member per item dominated the cost of rendering a page of results.
    const index = new Map<string, ICollectionItem>();
    for (const entry of collectionItems) {
      // First one wins, as the previous linear scan did; a record can carry more than one classification row.
      if (!index.has(entry.baseRecord)) {
        index.set(entry.baseRecord, entry);
      }
    }

    this.indexedCollection = collectionItems;
    this.findByRecord = (baseRecord: string) => index.get(baseRecord) ?? ItemContainer.MissingCollectionItem;
  }

  private findByRecord: (baseRecord: string) => ICollectionItem =
    () => ItemContainer.MissingCollectionItem;

  handleClick = () => {
    this.setState({
      isComparing: !this.state.isComparing
    });
  };

  render() {
    this.ensureCollectionIndex();

    const items = this.props.items;
    // Prefer the explicit hasMore signal from C# (robust across DB-page boundaries and the pre/post
    // stack-merge count mismatch); fall back to the count comparison only when it wasn't provided.
    const canLoadMoreItems = this.props.hasMore !== undefined
      ? this.props.hasMore
      : (this.props.numItems !== undefined ? this.props.numItems > items.length : true);

    let comparingItem = [] as IItem[];
    if (this.state.isComparing) {
      for (let idx = 0; idx < items.length; idx++) {
        if (items[idx][0].mergeIdentifier === this.state.compareItem) {
          comparingItem = items[idx];
          break;
        }
      }
    }


    if (items.length > 0) {
      let numItemsDisplayed = 0;
      for (const group of items) {
        for (const item of group) {
          if (item.type === IItemType.Player) {
            numItemsDisplayed++;
          }
        }
      }

      const renderItem = (group: IItem[]) => (
        <Item
          items={group}
          key={getUniqueId(group[0])}
          transferAll={this.transferAll}
          transferSingle={this.transferSingleWrapper}
          getItemName={this.findByRecord}
          requestUnknownItemHelp={this.requestUnknownItemHelp}
          showBackupCloudIcon={this.props.showBackupCloudIcon}
          hideItemSkills={this.props.hideItemSkills}
        />
      );
      return (
        <div class="items">
          <div class="clipboard-container">
            {<div class="clipboard-link" onClick={() => setClipboard(this.getClipboardContent())}>
              {translate('app.copyToClipboard')}
            </div>}
            {/* Append "+" when the exact total is still deferred (result was capped and the real COUNT
                hasn't been computed yet), so a capped result reads e.g. "64/1000+". */}
            <div>{translate('items.displaying', numItemsDisplayed + '/' + this.props.numItems + (this.props.numItemsApproximate ? '+' : ''))}</div>
          </div>

          {this.state.isComparing && <ItemComparer
              item={comparingItem}
              onClose={this.handleClick}
              getItemName={this.findByRecord}
              showBackupCloudIcon={this.props.showBackupCloudIcon}
              transferSingle={this.transferSingle}
          />}

          {items.map(renderItem)}

          {canLoadMoreItems && <button onClick={this.props.onRequestMoreItems} className="load-more-items">{translate('button.loadmoreitems')}</button>}
          {canLoadMoreItems && <OnScrollLoader onTrigger={this.props.onRequestMoreItems} isLoading={this.props.isLoading} />}
          <ReactTooltip html={true} type={this.props.isDarkMode ? 'dark' : 'light'} />
        </div>
      );
    }
    else {
      if (this.props.isLoading)
        return null;

      return (
        <div class="no-items-found">
          {translate('items.label.noItemsFound')}
        </div>
      );
    }
  }

}

export default ItemContainer;
