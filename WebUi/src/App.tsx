import * as React from 'react';
import { Provider, Store } from 'react-redux';
import './App.css';
import { GlobalState } from './types/index';
import MagicButton from './containers/MagicButton';
import FilterView from './containers/FilterView';
import { MockStore } from 'redux-mock-store';
import ItemContainer from './containers/ItemContainer';
import { requestClasses, requestMods } from './actions';
import CornerMenu from './components/CornerMenu/CornerMenu';
import { isEmbedded } from './constants/index';

export interface Props {
  store: Store<GlobalState> | MockStore<GlobalState>;
}

class App extends React.Component<Props, object> {
  loading: boolean = false;

  constructor(props: Props) {
    super(props);
    this.props = props;
    this.props.store.dispatch(requestClasses());
    this.props.store.dispatch(requestMods());
  }

  /*
  [16:11:53] <[nd]> position: absolute;top: 0; right: 0; bottom: 0; left: 0;
  [16:11:58] <[nd]> should in essence take care of it for you
  [16:12:08] <evil> and when the div is empty and gets a height of 1px?
  [16:12:10] <[nd]> but will need tweaking to get it to be right for your situation
  [16:12:14] <[nd]> exactly
  [16:12:20] <[nd]> its parent needs to be something sensible
  [16:12:39] <[nd]> maybe an element with min-width: 100vw; min-height: 100vh;
   */

  render() {
    const { store } = this.props;

    return (
      <Provider store={store}>
        <div className="App wrapper">
          <nav className="row" id="sidebar">
            <FilterView isEmbedded={isEmbedded} />
          </nav>
          <div id="content">
            <CornerMenu />
            {!isEmbedded ? <MagicButton label="Load mock items" /> : ''}
            <ItemContainer />
          </div>
        </div>
      </Provider>
    );
  }

}

export default App;
