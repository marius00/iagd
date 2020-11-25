import * as React from 'react';

interface Props {
  label: string;
  extras?: string;
}

class ItemStat extends React.PureComponent<Props, object> {
  render() {
    if (this.props.label === '') {
      return null;
    }

    const modifier = this.props.label.substr(0, this.props.label.indexOf(' '));
    const label = this.props.label.substr(this.props.label.indexOf(' ') + 1);

    if (this.props.extras) {
      return (
        <li>
          <a data-tip={this.props.extras} className="skill-trigger">
            <span className="modifier">{modifier}</span>&nbsp;
            <span className="label">{label}</span>
            </a>
        </li>
      );
    } else {
      return (
        <li>
          <span className="modifier">{modifier}</span>&nbsp;
          <span className="label">{label}</span>
        </li>
      );
    }
  }
}

export default ItemStat;
