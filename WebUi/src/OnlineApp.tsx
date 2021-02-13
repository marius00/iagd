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
  hasMoreItems: boolean;
  isHardcore: boolean;
}


interface Props {
  url: string;
}

function getIsHardcore(): boolean {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get('hc') === '1';
}

class OnlineApp extends React.PureComponent<Props, object> {
  state = {
    items: [],
    isLoading: false,
    isDarkMode: true,
    offset: 0,
    search: '',
    errorMessage: '',
    hasMoreItems: false,
    isHardcore: getIsHardcore(),
  } as ApplicationState;

  componentDidMount() {
    this.search('');
  }

  setItems(items: IItem[]) {
    var joined = [...this.state.items].concat(items);
    this.setState({
      items: joined,
      isLoading: false,
      offset: this.state.items.length + items.length,
      hasMoreItems: items.length > 0
    });
  }

  requestMoreItems() {
    if (this.state.hasMoreItems) {
      this.setState({isLoading: true});
      this.fetchItems(this.state.search, this.state.offset);
    }
  }

  getId() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('id');
  }


  search(text: string) {
    this.setState({isLoading: true, items: [], offset: 0, search: text, hasMoreItems: true});
    this.fetchItems(text, 0);
  }

  fetchItems(text: string, offset: number) {
    var self = this;
    fetch(`${this.props.url}/search?id=${this.getId()}&offset=${offset}&search=${encodeURIComponent(text.toLowerCase())}&hc=${this.state.isHardcore?1:0}`, {
        method: 'GET',
        headers: {'Accept': 'application/json'},
      }
    )
      .then(r => r.json().then(data => ({status: r.status, statusText: r.statusText, ok: r.ok, body: data})))
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

  toggleHardcoreMode() {
    const url = window.document.location.href.replace(`hc=${this.state.isHardcore ? 1 : 0}`, `hc=${this.state.isHardcore ? 0 : 1}`);
    window.history.replaceState('', '', url);
    this.setState({isHardcore: !this.state.isHardcore});
  }

  render() {
    if (!this.getId())
      return <h1 className="missing-id">404 - These are not the drones you are looking for..</h1>;

    return (
      <div className={'web App ' + (this.state.isDarkMode ? 'App-dark' : 'App-Light')}>
        <header>
          <h1>Items for #{this.getId()}</h1>
          {this.state.isLoading && <Spinner/>}

          <WebSearchContainer onSearch={(s) => this.search(s)}/>

          <div className="darkmode-toggle">
            <div className={'sliderContainer noselect'}>
              <label className="switch">
                <input type="checkbox" checked={this.state.isHardcore} onChange={() => this.toggleHardcoreMode()}/>
                <span className="slider round"/>
                <span className="sc">SC</span>
                <span className="hc">HC</span>
              </label>
            </div>

            <div className={'sliderContainer'}>
              <label className="switch">
                <input type="checkbox" checked={this.state.isDarkMode} onChange={() => this.setState({isDarkMode: !this.state.isDarkMode})}/>
                <span className="slider round"/>
                <LightMode className="lightmode"/>
                <DarkMode className="darkmode"/>
              </label>
            </div>
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
        {!this.state.hasMoreItems && <h2 className="no-more-items">No more items found</h2>}

        <div className="new-feature-promoter"></div>
      </div>
    );
  }
};

export default OnlineApp;
