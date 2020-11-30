import * as React from 'react';
import './ItemContainer.css';
import ReactTooltip from 'react-tooltip';
import ICollectionItem from '../interfaces/ICollectionItem';
import './CollectionItemContainer.css';
import { openUrl } from '../integration/integration';

interface Props {
  items: ICollectionItem[];
}

class CollectionItemContainer extends React.PureComponent<Props, object> {
  state = {
    onlyGreen: false,
    onlyRed: false
  };

  constructor(props: Props) {
    super(props);
  }

  openItemSite(item: ICollectionItem) {
    let url = `http://www.grimtools.com/db/search?query=${item.name}`;
    openUrl(url);
  }

  render() {
    const items = this.props.items;

    const filterItems = (item: ICollectionItem) => {
      if (this.state.onlyGreen && item.numOwned < 1) {
        return false;
      } else if (this.state.onlyRed && item.numOwned > 0) {
        return false;
      }

      return true;
    };

    const swapGreen = () => {
      this.setState({onlyGreen: !this.state.onlyGreen});
      setTimeout(() => ReactTooltip.rebuild(), 250);
    };

    const swapRed = () => {
      this.setState({onlyRed: !this.state.onlyRed});
      setTimeout(() => ReactTooltip.rebuild(), 250);
    };

    return (
      <div className="collectionItems">
        <h2>Experimental feature</h2>
        <p>
          This feature was added as Proof-of-concept at request from a user.<br/>
          It may get improved or removed in the future, depending on usage.
        </p>
        <div className={'toggleContainer'}>

          <div className={'sliderContainer'}>
            <label className="switch">
              <input type="checkbox" id={'cbOnlyGreen'} onChange={swapGreen} />
              <span className="slider round"/>
            </label>

            <label htmlFor={'cbOnlyGreen'} className={'sliderLabel'}>Owned only</label>
          </div>

          <div className={'sliderContainer'}>
            <label className="switch">
              <input type="checkbox" id={'cbOnlyRed'} onChange={swapRed} />
              <span className="slider round"/>
            </label>
            <label htmlFor={'cbOnlyRed'} className={'sliderLabel'}>Missing only</label>
          </div>
        </div>

        {items.filter(filterItems).map((item) =>
          <a className={'collectionItem'} onClick={() => this.openItemSite(item)} key={'collected-' + item.baseRecord} data-tip={(item.numOwned > 0 ? `${item.name} (x${item.numOwned})` : item.name)}>
            <div className={(item.numOwned > 0 ? 'collected' : 'uncollected') + ' imageContainer'}>
              <img src={item.icon} />
            </div>
            <div className={'textContainer'}>
            {item.name}
            </div>
          </a>
        )}
      </div>
    );
  }
}

export default CollectionItemContainer;
