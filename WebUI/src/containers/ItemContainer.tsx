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

interface Props {
  items: IItem[][];
  numItems?: number;
  isLoading: boolean;
  onItemReduce(item: IItem, transferAll: boolean): void;
  onRequestMoreItems(): void;
  collectionItems: ICollectionItem[];
  isDarkMode: boolean;
  requestUnknownItemHelp: () => void;
  showBackupCloudIcon: boolean;
}


class ItemContainer extends PureComponent<Props, object> {
  state = {
    isComparing: false,
    compareItem: '',
  }

  transferSingleWrapper(item: IItem[]) {
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

  transferSingle(item: IItem) {
    const id = item.uniqueIdentifier + '/-/-/-';
    const url = (id.split('/') as any) as object[];
    const r = transferItem(url, false);
    if (r.success) {
      this.props.onItemReduce(item, false);
    }
  }

  transferAll(item: IItem[]) {
    const url = (item[0].url as any) as object[];
    const r = transferItem(url, true);
    if (r.success) {
      this.props.onItemReduce(item[0], true); // Don't particulary matter which we reduce when doing transferAll
    }
  }

  componentDidUpdate(props: Props) {
    setTimeout(() => ReactTooltip.rebuild(), 1250); // TODO: This seems like a stupid way to solve tooltip issues.
  }

  /*
  getClipboardContent() {
    const colors: {[index: string]:string} = { Epic: 'DarkOrchid', Blue: 'RoyalBlue', Green: 'SeaGreen', Unknown: '', Yellow: 'Yellow' };
    const entries = this.props.items.map(item => {
      const name = item.name.replace('"', '');
      return `[URL="http://www.grimtools.com/db/search?query=${name}"][COLOR="${colors[item.quality]}"]${item.name}[/COLOR][/URL]`;
    });

    return entries.join('\n');
  }*/

  // TODO: A O(1) lookup would be preferable
  findByRecord(baseRecord: string): ICollectionItem {
    for (let idx in this.props.collectionItems) {
      const entry = this.props.collectionItems[idx];
      if (entry.baseRecord === baseRecord) {
        return entry;
      }
    }

    return {baseRecord: "", name: "", icon: "", numOwnedSc: 0, numOwnedHc: 0};
  }

  handleClick = () => {
    this.setState({
      isComparing: !this.state.isComparing
    });
  };

  render() {
    const items = this.props.items;
    const canLoadMoreItems = this.props.numItems !== undefined ? this.props.numItems > items.length : true;

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
      return (
        <div class="items">
          <div class="clipboard-container">
            {/*<div class="clipboard-link" onClick={() => setClipboard(this.getClipboardContent())}>
              {translate('app.copyToClipboard')}
            </div>*/}
            <div>{translate('items.displaying', items.length + '/' + this.props.numItems)}</div>
          </div>

          {this.state.isComparing && <ItemComparer
              item={comparingItem}
              onClose={this.handleClick}
              getItemName={(baseRecord:string) => this.findByRecord(baseRecord)}
              showBackupCloudIcon={this.props.showBackupCloudIcon}
              transferSingle={(item: IItem) => this.transferSingle(item)}
          />}

          {items.map((item) =>
            <Item
              item={item}
              key={'item-' + getUniqueId(item[0])}
              transferAll={(item: IItem[]) => this.transferAll(item)}
              transferSingle={(item: IItem[]) => this.transferSingleWrapper(item)}
              getItemName={(baseRecord:string) => this.findByRecord(baseRecord)}
              requestUnknownItemHelp={this.props.requestUnknownItemHelp}
              showBackupCloudIcon={this.props.showBackupCloudIcon}
            />
          )}

          {canLoadMoreItems && <button onClick={this.props.onRequestMoreItems} className="load-more-items">Load more items</button>}
          {canLoadMoreItems && <OnScrollLoader onTrigger={this.props.onRequestMoreItems} />}
          <ReactTooltip html={true} type={this.props.isDarkMode ? 'light' : 'dark'} />
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
