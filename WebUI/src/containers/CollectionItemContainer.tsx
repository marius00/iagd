import {Fragment, h} from "preact";
import './ItemContainer.css';
import ReactTooltip from 'react-tooltip';
import ICollectionItem from '../interfaces/ICollectionItem';
import './CollectionItemContainer.css';
import { openUrl } from '../integration/integration';
import {PureComponent} from "preact/compat";
import translate from "../translations/EmbeddedTranslator";
import IItemAggregateRow from "../interfaces/IItemAggregateRow";

interface Props {
  items: ICollectionItem[];
  aggregate: IItemAggregateRow[];
}

class CollectionItemContainer extends PureComponent<Props, object> {
  state = {
    onlyGreen: false,
    onlyRed: true,
    onlyPurple: true
  };

  constructor(props: Props) {
    super(props);
  }

  stripColorCodes(initialString: any|string): string {
    return initialString?.replaceAll(/{?\^.}?/g, '') || '';
  }

  openItemSite(item: ICollectionItem) {
    let url = `https://grimdawn.evilsoft.net/search/?query=${this.stripColorCodes(item.name)}`;
    openUrl(url);
  }

  renderItemAggregate() {

    let sum = {
      blue: 0,
        green: 0,
        green2: 0,
        green3: 0,
        epic: 0,
    }
    const table = {} as any;
    for (let i = 0; i < this.props.aggregate.length; i++) {
      const obj = this.props.aggregate[i];
      let key = obj.translatedSlot;
      if (!table.hasOwnProperty(key)) {
        table[key] = {
          blue: 0,
          green: 0,
          green2: 0,
          green3: 0,
          epic: 0,
        }
      }
      // @ts-ignore
      table[key][obj.quality.toLowerCase()] = obj.num;
      sum[obj.quality.toLowerCase()] += obj.num;
    }

    table['Sum'] = sum; // TODO: Translate support
    console.log('prelim', table);

    // TODO: Translate support
    return <table className={'aggregate-table'}>
      <tr>
        <th>Slot</th>
        <th>Epic</th>
        <th>Blue</th>
        <th>Green</th>
        <th>Green (DoubleRare)</th>
        <th>Green (TripleRare)</th>
      </tr>
      {
        Object.keys(table).map((a, b, c) => {
          return <tr>
            <th>{a}</th>
            <td>{table[a].epic}</td>
            <td>{table[a].blue}</td>
            <td>{table[a].green}</td>
            <td>{table[a].green2}</td>
            <td>{table[a].green3}</td>
          </tr>
        })
      }
    </table>;
  }

  renderFilters() {
    const swapGreen = () => {
      const redState = !this.state.onlyGreen ? false : this.state.onlyRed;
      this.setState({onlyGreen: !this.state.onlyGreen, onlyRed: redState});
      setTimeout(() => ReactTooltip.rebuild(), 250);
    };

    const swapRed = () => {
      const greenState = !this.state.onlyRed ? false : this.state.onlyGreen;
      this.setState({onlyRed: !this.state.onlyRed, onlyGreen: greenState});
      setTimeout(() => ReactTooltip.rebuild(), 250);
    };

    const swapPurple = () => {
      this.setState({onlyPurple: !this.state.onlyPurple});
      setTimeout(() => ReactTooltip.rebuild(), 250);
    };



    return (
      <Fragment>

        <h2>{translate('collections.h2')}</h2>
        <p>
          {translate('collections.ingress1')}<br/>
          {translate('collections.ingress2')}
        </p>
        <div className={'toggleContainer'}>

          <div className={'sliderContainer'}>
            <label className="switch">
              <input type="checkbox" id={'cbOnlyGreen'} onChange={swapGreen} checked={this.state.onlyGreen} />
              <span className="slider round"/>
            </label>

            <label htmlFor={'cbOnlyGreen'} className={'sliderLabel'}>{translate('collections.filter.owned')}</label>
          </div>

          <div className={'sliderContainer'}>
            <label className="switch">
              <input type="checkbox" id={'cbOnlyRed'} onChange={swapRed} checked={this.state.onlyRed} />
              <span className="slider round"/>
            </label>
            <label htmlFor={'cbOnlyRed'} className={'sliderLabel'}>{translate('collections.filter.missing')}</label>
          </div>

          <div className={'sliderContainer'}>
            <label className="switch">
              <input type="checkbox" id={'cbPurpleOnly'} onChange={swapPurple} checked={this.state.onlyPurple} />
              <span className="slider round"/>
            </label>
            <label htmlFor={'cbPurpleOnly'} className={'sliderLabel'}>{translate('collections.filter.purple')}</label>
          </div>

        </div>
      </Fragment>

    );
  }

  render() {
    //const items = [this.props.items[0], this.props.items[1]];
    const items = this.props.items;
    const isMock = this.props.items && this.props.items.length > 0 && this.props.items[0].baseRecord === "mock item";

    // TODO: Use both numOwnedSc and numOwnedHc (if support is ever added)
    const filterItems = (item: ICollectionItem) => {
      if (this.state.onlyGreen && item.numOwnedSc < 1) {
        return false;
      } else if (this.state.onlyRed && item.numOwnedSc > 0) {
        return false;
      } else if (this.state.onlyPurple && item.quality !== 'Legendary') {
        return false;
      }

      return true;
    };

    const toQuality = (item: ICollectionItem) => {
      if (item.quality === 'Legendary')
        return 'epic';
      else if (item.quality === 'Epic')
        return 'blue';
      else if (item.quality === 'Rare')
        return 'green';
      else
        return 'white';
    };


    return (
      <div className="collectionItems">
        {this.renderFilters()}
        {this.renderItemAggregate()}
        <div className="collectionContainer">
          {items.filter(filterItems).map((item) =>
            <a className={'collectionItem'} onClick={() => this.openItemSite(item)} key={'collected-' + item.baseRecord} data-tip={(item.numOwnedSc > 0 ? `${this.stripColorCodes(item.name)} (x${item.numOwnedSc})` : this.stripColorCodes(item.name))}>

              {/* TODO: Icon background color */}
              <div className={(item.numOwnedSc > 0 ? 'collected' : 'uncollected') + ' imageContainer' + (' item-icon-' + toQuality(item))}>
                <span className="helper"></span>
                <img src={(isMock ? "assets/":"") + item.icon} className={"item-icon" }/>
              </div>

              <p className={`item-quality-${toQuality(item)}`}>{this.stripColorCodes(item.name)}</p>
            </a>
          )}
        </div>
      </div>
    );
  }
}

export default CollectionItemContainer;
