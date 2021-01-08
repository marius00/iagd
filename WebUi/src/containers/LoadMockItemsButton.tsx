import * as React from 'react';
import IItem from '../interfaces/IItem';
import MockCollectionItemData from '../mock/MockCollectionItemData';

/* tslint:disable */
export const mockDataItems = (text: string): IItem[] => {
  // @ts-ignore
  return MockCollectionItemData;
};


interface Props {
  onClick: (items: IItem[]) => void;
}

class MockItemsButton extends React.PureComponent<Props, object> {
  render() {
    return (
      <button className="button noselect"  onClick={() => this.props.onClick(mockDataItems(""))}>Load mock items</button>
    );
  }
};

export default MockItemsButton;