import * as React from 'react';
import './App.css';
import './OnlineApp.css';
import 'react-notifications-component/dist/theme.css';
import IItem from './interfaces/IItem';
import ItemContainer from './containers/ItemContainer';
import Spinner from './components/Spinner';
import WebSearchContainer from './containers/WebSearchContainer';
import DarkMode from '@material-ui/icons/NightsStay';
import LightMode from '@material-ui/icons/WbSunny';


export interface ApplicationState {
  items: IItem[];
  isLoading: boolean;
  isDarkMode: boolean;
  search: string;
  errorMessage: string;
  offset: number;
}


interface Props {
  url: string;
}
class OnlineApp extends React.PureComponent<Props, object> {
  state = {
    items: [],
    isLoading: false,
    isDarkMode: true,
    offset: 0,
    search: '',
    errorMessage: '',
  } as ApplicationState;


  setItems(items: IItem[]) {
    var joined = [...this.state.items].concat(items);
    console.log('joined:', joined);
    this.setState({
      items: joined,
      isLoading: false,
      offset: this.state.items.length + items.length
    });
  }

  requestMoreItems() {
    console.log('More items it wantssssss?');
    this.setState({isLoading: true});
    this.fetchItems(this.state.search, this.state.offset);
  }

  getId() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('id');
  }

  search(text: string) {
    this.setState({isLoading: true, items: [], offset: 0, search: text});
    this.fetchItems(text, 0);
  }

  fetchItems(text: string, offset: number) {
    var self = this;
    // TODO: Url encode
    fetch(`${this.props.url}/buddy?id=${this.getId()}&offset=${offset}&search=${encodeURIComponent(text.toLowerCase())}`, {
        method: 'GET',
        headers: {'Accept': 'application/json'},
      }
    )
      .then(r =>  r.json().then(data => ({status: r.status, statusText: r.statusText, ok: r.ok, body: data})))
      .then((response) => {
        if (!response.ok) {
          console.log(response);
          throw Error(`Got response ${response.body.msg}`);
        }
        return response;
      })
      .then((response) => response.body)
      .then((json) => {
        this.setItems(json);
      })
      .catch((error) => {
        console.warn(error);
        self.setState({isLoading: false, errorMessage: error.toString()});
      });
  }

  render() {
    return (
      <div className={'web App ' + (this.state.isDarkMode ? 'App-dark' : 'App-Light')}>
        <header>
          <h1>Items for #{this.getId()}</h1>
          {this.state.isLoading && <Spinner/>}

          <WebSearchContainer onSearch={(s) => this.search(s)}/>

          <div className="darkmode-toggle" onClick={() => this.setState({isDarkMode: !this.state.isDarkMode})}>
            {this.state.isDarkMode && <LightMode/>}
            {!this.state.isDarkMode && <DarkMode/>}
          </div>
        </header>

        <div className="content">
          {this.state.errorMessage && <h2 className="error">{this.state.errorMessage}</h2>}
          <ItemContainer
            items={this.state.items}
            isLoading={this.state.isLoading}
            onItemReduce={(url, numItems) => {
            }}
            onRequestMoreItems={() => this.requestMoreItems()}
            collectionItems={[]}
            isDarkMode={this.state.isDarkMode}
            requestUnknownItemHelp={() => this.setState({helpSearchFilter: 'UnknownItem', activeTab: 3})}
          />
        </div>

        <div className="new-feature-promoter"></div>
      </div>
    );
  }
};

export default OnlineApp;
