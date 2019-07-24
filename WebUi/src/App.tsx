import * as React from 'react';
import { Provider, Store } from 'react-redux';
import { Tab, TabList, TabPanel, Tabs } from 'react-tabs';
import { ApplicationState } from './types/index';
import MagicButton from './containers/MagicButton';
import { MockStore } from 'redux-mock-store';
import ItemContainer from './containers/ItemContainer';
import { isEmbedded } from './constants/index';
import 'react-tabs/style/react-tabs.css'; // scss and less also available
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
            <Tabs className="tab-control">
              <div className="tab-list">
                <TabList>
                  <Tab>{translate('app.tab.items')}</Tab>
                  <Tab disabled={true}><a onClick={() => this.openUrl('http://dev.dreamcrash.org/enchantments/')}>{translate('app.tab.components')}</a></Tab>
                  {translate('app.tab.videoGuide').length > 0 &&
                    <Tab disabled={true}><a onClick={() => this.openUrl(translate('app.tab.videoGuideUrl'))}>{translate('app.tab.videoGuide')}</a></Tab>
                  }
                  <Tab disabled={true}><a onClick={() => this.openUrl('https://discord.gg/vJHp4y')}>{translate('app.tab.discord')}</a></Tab>
                </TabList>
              </div>

              <TabPanel>
                {!isEmbedded ? <MagicButton label="Load mock items"/> : ''}
                <ItemContainer/>
              </TabPanel>

              <TabPanel/>
              <TabPanel/>
              <TabPanel/>
            </Tabs>


          </div>
          <NotificationContainer/>
        </div>
      </Provider>
    );
  }
}

export default App;
