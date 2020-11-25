import * as React from 'react';
import Item from '../components/Item/Item';
import IItem from '../interfaces/IItem';
import './ItemContainer.css';
import ReactTooltip from 'react-tooltip';
import Spinner from '../components/Spinner';
import translate from '../translations/EmbeddedTranslator';
import { setClipboard, transferItem } from '../integration/integration';
import OnScrollLoader from '../components/OnScrollLoader';

interface Props {
  items: IItem[];
  isLoading: boolean;
  onItemReduce(url: object[], numItems: number): void;
  onRequestMoreItems(): void;
}

class ItemContainer extends React.PureComponent<Props, object> {
  transferSingle(url: object[]) {
    var r = transferItem(url, 1);
    if (r.success) {
      this.props.onItemReduce(url, r.numTransferred);
    }
  }

  transferAll(url: object[]) {
    var r = transferItem(url, 99999);
    if (r.success) {
      this.props.onItemReduce(url, r.numTransferred);
    }
  }

  componentDidUpdate(props: Props) {
    setTimeout(() => ReactTooltip.rebuild(), 1250); // TODO: This seems like a stupid way to solve tooltip issues.
  }

  getClipboardContent() {
    const colors: {[index: string]:string} = { Epic: 'DarkOrchid', Blue: 'RoyalBlue', Green: 'SeaGreen', Unknown: '', Yellow: 'Yellow' };
    const entries = this.props.items.map(item => {
      const name = item.name.replace('"', '');
      return `[URL="http://www.grimtools.com/db/search?query=${name}"][COLOR="${colors[item.quality]}"]${item.name}[/COLOR][/URL]`;
    });

    return entries.join('\n');
  }

  render() {
    const items = this.props.items;

    if (items.length > 0) {
      return (
        <div className="items">
          {this.props.isLoading && <Spinner />}
          <span className="clipboard-link" onClick={() => setClipboard(this.getClipboardContent())}>
            {translate('app.copyToClipboard')}
          </span>

          {items.map((item) =>
            <Item
              item={item}
              key={'item-' + item.url.join(':')}
              transferAll={(url: object[]) => this.transferAll(url)}
              transferSingle={(url: object[]) => this.transferSingle(url)}
            />
          )}
          <ReactTooltip />
          <OnScrollLoader onTrigger={this.props.onRequestMoreItems} />

        </div>
      );
    }
    else {
      return (
        <div className="no-items-found">
          {translate('items.label.noItemsFound')}
        </div>
      );
    }
  }

}

export default ItemContainer;
