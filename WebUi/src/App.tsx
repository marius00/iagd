import * as React from 'react';
import { Provider, Store } from 'react-redux';
import { ApplicationState } from './types/index';
import MagicButton from './containers/MagicButton';
import { MockStore } from 'redux-mock-store';
import ItemContainer from './containers/ItemContainer';
import { isEmbedded } from './constants/index';
import './App.css';
import { requestInitialItems } from './actions';
import NotificationContainer from './containers/NotificationContainer';
import translate from './translations/EmbeddedTranslator';

export interface Props {
  store: Store<ApplicationState> | MockStore<ApplicationState>;
}

class App extends React.PureComponent<Props, object> {
  loading: boolean = false;

  constructor(props: Props) {
    super(props);
    this.props.store.dispatch(requestInitialItems());
  }

  openUrl(url: string) {
    if (isEmbedded) {
      document.location.href = url;
    } else {
      window.open(url);
    }
  }

  render() {
    const {store} = this.props;

    return (
      <Provider store={store}>
        <div className="App wrapper">
          <div id="content">
            <div className="header">
                <a onClick={() => this.openUrl('http://dev.dreamcrash.org/enchantments/')}>{translate('app.tab.components')}</a>
                {translate('app.tab.videoGuide').length > 0 &&
                  <a onClick={() => this.openUrl(translate('app.tab.videoGuideUrl'))}>{translate('app.tab.videoGuide')}</a>
                }
                <a onClick={() => this.openUrl('https://discord.gg/5wuCPbB')}>{translate('app.tab.discord')}</a>
            </div>

            {!isEmbedded ? <MagicButton label="Load mock items"/> : ''}
            <ItemContainer/>


          </div>
          <NotificationContainer/>
        </div>
      </Provider>
    );
  }
}

export default App;
