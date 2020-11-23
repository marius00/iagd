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

  setItems(items: IItem[]) {
    this.setState({...this.state, items: items, isLoading: false});
    console.log('Set items called', this.state);
  }

  render() {
    return (
      <div className="App">
        <StoreContext.Provider value={this.state}>
          <header className="">
          </header>

          -{!isEmbedded ? <MockItemsButton onClick={(items) => this.setItems(items)} /> : ''}-

          <ItemContainer items={this.state.items} isLoading={this.state.isLoading} />
        </StoreContext.Provider>
      </div>
    );
  }
};

export default App;
