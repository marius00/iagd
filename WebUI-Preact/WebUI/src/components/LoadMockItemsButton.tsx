import {h} from "preact";
import IItem from '../interfaces/IItem';
import MockCollectionItemData from '../mock/MockCollectionItemData';
import {PureComponent} from "preact/compat";

/* tslint:disable */
export const mockDataItems = (text: string): IItem[] => {
  // @ts-ignore
  return MockCollectionItemData;
};


interface Props {
  onClick: (items: IItem[]) => void;
}

class MockItemsButton extends PureComponent<Props, object> {
  render() {
    return (
      <button className="button noselect"  onClick={() => this.props.onClick(mockDataItems(""))}>Load mock items</button>
    );
  }
};

export default MockItemsButton;