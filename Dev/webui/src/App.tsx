import * as React from 'react';
import './App.css';
import IItem from './interfaces/IItem';
import MockItemsButton from './containers/MockItemsButton';
import { isEmbedded, requestMoreItems } from './integration/integration';
import ItemContainer from './containers/ItemContainer';
import { store } from 'react-notifications-component';
import ReactNotification from 'react-notifications-component';
import 'react-notifications-component/dist/theme.css';


export interface ApplicationState {
  items: IItem[];
  isLoading: boolean;
}

const StoreContext = React.createContext({items: [], isLoading: true} as ApplicationState);

// TODO: infiscroll
// TODO: Collection tab
// TODO: Tabs [and maybe improve discord link, and merge in help tab?]
// TODO: Dark mode
// TODO: Tooltips broken inside IA?
// TODO: Prevent multiple clicks on transfer? or non-issue?
// TODO: Crafting support??
// TODO: A no more matches message when scrolling too far?
// TODO: A commit redoing all the damn bracket styles in C# -- do this last.. sigh.

class App extends React.PureComponent<{}, object> {
  state = {
    items: [],
    isLoading: true
  } as ApplicationState;

  componentDidMount() {
    // Set the items to show
    // @ts-ignore: setItems doesn't exist on window
    window.setItems = (data: any) => {
      const items = typeof data === 'string' ? JSON.parse(data) : data;

      this.setState({
        isLoading: false,
        items: items
      });
    };

    // Add more items (typically scrolling)
    // @ts-ignore: setItems doesn't exist on window
    window.addItems = (data: any) => {
      const items = [...this.state.items];
      const newItems = typeof data === 'string' ? JSON.parse(data) : data;

      this.setState({
        isLoading: false,
        items: items.concat(newItems)
      });
    };

    // Start showing the loading spinner
    // @ts-ignore: setIsLoading doesn't exist on window
    window.setIsLoading = (isLoading: boolean) => {
      this.setState({
        isLoading: isLoading
      });
    };

    // Show a notification message such as "Item transferred" or "Too close to stash"
    // @ts-ignore: showMessage doesn't exist on window
    window.showMessage = (input: string) => {
      let s = JSON.parse(input);
      ShowMessage(s.message, s.type);
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
    } else {
      console.warn("Attempted to reduce item count, but item could not be found", url);
    }
  }

  requestMoreItems() {
    console.log("More items it wantssssss?");
    this.setState({isLoading: true});
    requestMoreItems();
    // TODO: Fix this weird loop? This one will request more items.. which will end up in a call from C# to window.addItems().. is that how we wanna do this?
  }

  render() {
    return (
      <div className="App">
        <ReactNotification />
        <StoreContext.Provider value={this.state}>
          <header className="">
          </header>

          {!isEmbedded ? <MockItemsButton onClick={(items) => this.setItems(items)} /> : ''}

          <ItemContainer
            items={this.state.items}
            isLoading={this.state.isLoading}
            onItemReduce={(url, numItems) => this.reduceItemCount(url, numItems)}
            onRequestMoreItems={() => this.requestMoreItems()}
          />
        </StoreContext.Provider>
      </div>
    );
  }
};

function ShowMessage(message: string, type: 'success' | 'danger' | 'info' | 'default' | 'warning') {
  store.addNotification({
    title: type,
    message: message,
    type: type,
    insert: "top",
    container: "bottom-center",
    animationIn: ["animate__animated", "animate__fadeIn"],
    animationOut: ["animate__animated", "animate__fadeOut"],
    dismiss: {
      duration: 2500,
      onScreen: true
    }
  });
}

export default App;
