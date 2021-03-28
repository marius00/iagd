import * as React from 'react';
import './App.css';
import 'react-notifications-component/dist/theme.css';
import Help from './containers/help/Help';

export interface ApplicationState {
  isDarkMode: boolean;
  helpSearchFilter: string;
}

class OnlineHelpPage extends React.PureComponent<{}, object> {
  state = {
    isDarkMode: false,
    helpSearchFilter: '',
  } as ApplicationState;

  componentDidMount() {
    const urlParams = new URLSearchParams(window.location.search);
    const helpParam = urlParams.get('q');
    if (helpParam) {
      this.setState({helpSearchFilter: helpParam});
    }
  }

  onSearchArgUpdate(v: string) {
    let url;
    if (window.location.href.indexOf('?') === -1) {
      url = window.location.href;
    } else {
      url = window.location.href.substring(0, window.location.href.indexOf('?'));
    }
    this.setState({helpSearchFilter: v});
    window.history.replaceState('', '', `${url}?q=${encodeURIComponent(v)}`);
  }

  render() {
    return (
      <div className={'App ' + (this.state.isDarkMode ? 'App-dark' : 'App-Light')}>
        <Help searchString={this.state.helpSearchFilter} onSearch={(v: string) => this.onSearchArgUpdate(v)}/>
      </div>
    );
  }

};

export default OnlineHelpPage;
