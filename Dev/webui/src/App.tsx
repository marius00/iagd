import * as React from 'react';
import './App.css';
import IItem from './interfaces/IItem';
import MockItemsButton from './containers/MockItemsButton';
import { isEmbedded } from './integration/integration';
import ItemContainer from './containers/ItemContainer'

export interface ApplicationState {
  items: IItem[];
  isLoading: boolean;
}

const StoreContext = React.createContext({items: [], isLoading: true} as ApplicationState);


class App extends React.PureComponent<{}, object> {
  state = {
    items: [],
    isLoading: true
  } as ApplicationState;

  componentDidMount() {
    // @ts-ignore: setItems doesn't exist on window
    window.setItems = (data: any) => {
      const items = typeof data === 'string' ? JSON.parse(data) : data;

      this.setState({
        isLoading: false,
        items: items
      });
    };

    // @ts-ignore: setItems doesn't exist on window
    window.addItems = (data: any) => {
      const items = typeof data === 'string' ? JSON.parse(data) : data;

      this.setState({
        isLoading: false,
        items: {...this.state.items, items}
      });
    };

    // @ts-ignore: setIsLoading doesn't exist on window
    window.setIsLoading = (isLoading: boolean) => {
      this.setState({
        isLoading: isLoading
      });
    };
  }

  // Used primarily for setting mock items for testing
  setItems(items: IItem[]) {
    this.setState({items: items, isLoading: false})
  }

  // reduceItemCount will reduce the number of items displayed, generally after an item has been transferred out.
  reduceItemCount(url: object[], numItems: number) {
    let items = [...this.state.items];
    var item = items.filter(e => e.url.join(':') === url.join(':'))[0];
    if (item !== undefined) {
      item.numItems -= numItems;
      this.setState({items: items});
      console.log(item);
    } else {
      console.warn("Attempted to reduce item count, but item could not be found", url);
    }
  }

  render() {
    return (
      <div className="App">
        <StoreContext.Provider value={this.state}>
          <header className="">
          </header>

          {!isEmbedded ? <MockItemsButton onClick={(items) => this.setItems(items)} /> : ''}

          <ItemContainer items={this.state.items} isLoading={this.state.isLoading} onItemReduce={(url, numItems) => this.reduceItemCount(url, numItems)} />
        </StoreContext.Provider>
      </div>
    );
  }
};

export default App;
