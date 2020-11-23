import * as React from 'react';
import Item from '../components/Item';
import IItem from '../interfaces/IItem';
import './ItemContainer.css';
import ReactTooltip from 'react-tooltip';
import { isEmbedded } from '../integration/integration';
import Spinner from '../components/Spinner';
import translate from '../translations/EmbeddedTranslator';

interface Props {
  items: IItem[];
  isLoading: boolean;
}

class ItemContainer extends React.PureComponent<Props, object> {

  constructor(props: Props) {
    super(props);
  }

  transferSingle(url: object[]) {
    const id = url.join(';');
    if (isEmbedded) {
      document.location.href = `transfer://single/${id}`; // TODO: Redo
    } else {
      console.log('Transfer Single', id);
    }
  }

  transferAll(url: object[]) {
    const id = url.join(';');
    if (isEmbedded) {
      document.location.href = `transfer://all/${id}`; // TODO: Redo
    } else {
      console.log('Transfer All', id);
    }
  }

  copyToClipboard(stuff: string) {
    // TODO: REDO, move to integration
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

    if (this.props.isLoading) {
      return <Spinner/>;
    }
    else if (items.length > 0) {
      return (
        <div className="items">
          <span className="clipboard-link" onClick={() => this.copyToClipboard(this.getClipboardContent())}>
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
