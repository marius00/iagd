import * as React from 'react';
import 'react-select/dist/react-select.css';

interface Props {
  label: string;
  extras?: string;
}

class ItemStat extends React.PureComponent<Props, object> {
  render() {
    if (this.props.extras) {
      return (
        <li>
          <a data-tip={this.props.extras} className="skill-trigger">{this.props.label}</a>
        </li>
      );
    } else {
      return (
        <li>
          {this.props.label}
        </li>
      );
    }
  }
}

export default ItemStat;
