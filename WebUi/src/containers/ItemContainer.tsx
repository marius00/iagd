import * as React from 'react';
import Item from '../components/Item';
import IItem from '../interfaces/IItem';
import { connect } from 'react-redux';
import './ItemContainer.css';
import * as ReactTooltip from 'react-tooltip';
import { isEmbedded } from '../constants';
import Spinner from '../components/Spinner';
import OnScrollLoader from './InfiniteItemLoader';
import { GlobalReducerState } from '../types';
import translate from '../translations/EmbeddedTranslator';

interface Props {
  items: IItem[];
  isLoading: boolean;
}

class ItemContainer extends React.Component<Props, object> {

  constructor(props: Props) {
    super(props);
    this.props = props;
  }

  transferSingle(url: object[]) {
    const id = url.join(';');
    if (isEmbedded) {
      document.location.href = `transfer://single/${id}`;
    } else {
      console.log('Transfer Single', id);
    }
  }

  transferAll(url: object[]) {
    const id = url.join(';');
    if (isEmbedded) {
      document.location.href = `transfer://all/${id}`;
    } else {
      console.log('Transfer All', id);
    }

  }

  render() {
    const items = this.props.items;
    const classes = items.length > 1 ? 'items' : 'items single-item';

    if (this.props.isLoading) {
      return <Spinner />;
    }
    else if (items.length > 0) {
      return (
        <div className={classes}>

          {items.map((item) =>
            <Item
              item={item}
              key={'item-' + item.url.join(':')}
              transferAll={(url) => this.transferAll(url)}
              transferSingle={(url) => this.transferSingle(url)}
            />
          )}

          <ReactTooltip id="you-can-craft-this-item-tooltip">
            <span>{translate('items.label.youCanCraftThisItem')}</span>
          </ReactTooltip>
          <ReactTooltip id="cloud-ok-tooltip">
            <span>{translate('items.label.cloudOk')}</span>
          </ReactTooltip>
          <ReactTooltip id="cloud-err-tooltip">
            <span>{translate('items.label.cloudError')}</span>
          </ReactTooltip>
          <ReactTooltip id="triple-green-tooltip">
            <span>{translate('items.label.tripleGreen')}</span>
          </ReactTooltip>
          <ReactTooltip id="double-green-tooltip">
            <span>{translate('items.label.doubleGreen')}</span>
          </ReactTooltip>


          <OnScrollLoader />
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

export function mapStateToProps(state: GlobalReducerState): Props {
  return {
    items: state.setItemReducer.items,
    isLoading: state.setItemReducer.isLoading
  };
}

export default connect<Props>(mapStateToProps)(ItemContainer);
