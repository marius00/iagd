import * as React from 'react';
import 'react-select/dist/react-select.css';
import * as ReactTooltip from 'react-tooltip';
import * as Guid from 'guid';

interface Props {
  label: string;
  extras?: string;
}

class ItemStat extends React.Component<Props, object> {
  render() {
    const tooltipId = Guid.raw();
    if (this.props.extras) {
      return (
        <li>
          <a data-tip="true" data-for={tooltipId} className="skill-trigger">{this.props.label}</a>
          <ReactTooltip id={tooltipId}>
            <span>{this.props.extras}</span>
          </ReactTooltip>
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
