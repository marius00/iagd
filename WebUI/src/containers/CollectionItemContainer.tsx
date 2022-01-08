import {h} from "preact";
import './ItemContainer.css';
import ReactTooltip from 'react-tooltip';
import ICollectionItem from '../interfaces/ICollectionItem';
import './CollectionItemContainer.css';
import { openUrl } from '../integration/integration';
import {PureComponent} from "preact/compat";
import translate from "../translations/EmbeddedTranslator";

interface Props {
  items: ICollectionItem[];
}

class CollectionItemContainer extends PureComponent<Props, object> {
  state = {
    onlyGreen: false,
    onlyRed: false
  };

  constructor(props: Props) {
    super(props);
  }

  stripColorCodes(initialString: any|string): string {
    return initialString.replaceAll(/{?\^.}?/g, '');
  }

  openItemSite(item: ICollectionItem) {
    let url = `https://grimdawn.evilsoft.net/search/?query=${this.stripColorCodes(item.name)}`;
    openUrl(url);
  }

  render() {
    const items = this.props.items;

    // TODO: Use both numOwnedSc and numOwnedHc (if support is ever added)
    const filterItems = (item: ICollectionItem) => {
      if (this.state.onlyGreen && item.numOwnedSc < 1) {
        return false;
      } else if (this.state.onlyRed && item.numOwnedSc > 0) {
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
        <h2>{translate('collections.h2')}</h2>
        <p>
          {translate('collections.ingress1')}<br/>
          {translate('collections.ingress2')}
        </p>
        <div className={'toggleContainer'}>

          <div className={'sliderContainer'}>
            <label className="switch">
              <input type="checkbox" id={'cbOnlyGreen'} onChange={swapGreen} />
              <span className="slider round"/>
            </label>

            <label htmlFor={'cbOnlyGreen'} className={'sliderLabel'}>{translate('collections.filter.owned')}</label>
          </div>

          <div className={'sliderContainer'}>
            <label className="switch">
              <input type="checkbox" id={'cbOnlyRed'} onChange={swapRed} />
              <span className="slider round"/>
            </label>
            <label htmlFor={'cbOnlyRed'} className={'sliderLabel'}>{translate('collections.filter.missing')}</label>
          </div>
        </div>

        {items.filter(filterItems).map((item) =>
          <a className={'collectionItem'} onClick={() => this.openItemSite(item)} key={'collected-' + item.baseRecord} data-tip={(item.numOwnedSc > 0 ? `${this.stripColorCodes(item.name)} (x${item.numOwnedSc})` : this.stripColorCodes(item.name))}>
            <div className={(item.numOwnedSc > 0 ? 'collected' : 'uncollected') + ' imageContainer'}>
              <img src={item.icon} />
            </div>
            <div className={'textContainer'}>
              {this.stripColorCodes(item.name)}
            </div>
          </a>
        )}
      </div>
    );
  }
}

export default CollectionItemContainer;
