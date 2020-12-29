import * as React from 'react';
import { ApplicationState } from '../OnlineApp';
import IItem from '../interfaces/IItem';
import { isEmbedded } from '../integration/integration';

export interface Props {
  onSearch: (text: string) => void;
}

class WebSearchContainer extends React.PureComponent<Props, object> {
  state = {
    search: '',
  };

  handleKeyDown(event: any) {
    if (event.key === 'Enter') {
      this.props.onSearch(this.state.search);
    }
    else if (event.key === 'Escape') {
      this.setState({search: ''});
    }
  }

  render() {
    return (
      <div>
        <input
          type="text"
          className="form-control"
          placeholder="Search.."
          onChange={(e) => this.setState({search: e.target.value})}
          value={this.state.search}
          onKeyDown={(e) => this.handleKeyDown(e)}
        />
        <button onClick={() => this.props.onSearch(this.state.search)} className="btn btn-primary">Search</button>
      </div>
    );
  }
}

export default WebSearchContainer;