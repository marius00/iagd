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
    this.setState({...this.state, items: items, isLoading: false});
  }

  render() {
    return (
      <div className="App">
        <StoreContext.Provider value={this.state}>
          <header className="">
          </header>

          {!isEmbedded ? <MockItemsButton onClick={(items) => this.setItems(items)} /> : ''}

          <ItemContainer items={this.state.items} isLoading={this.state.isLoading} />
        </StoreContext.Provider>
      </div>
    );
  }
};

export default App;
