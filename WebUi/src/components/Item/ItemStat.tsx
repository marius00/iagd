import * as React from 'react';
import { IStat } from '../../interfaces/IStat';


function statToString(stat: IStat) {
  return stat.text
    .replace('{0}', stat.param0)
    .replace('{1}', stat.param1)
    .replace('{2}', stat.param2)
    .replace('{3}', stat.param3)
    .replace('{4}', stat.param4)
    .replace('{5}', stat.param5)
    .replace('{6}', stat.param6);
}

// Removes the 3rd parameter, typically skill name.
function statToStringSkip3(stat: IStat) {
  return stat.text
    .replace('{0}', stat.param0)
    .replace('{1}', stat.param1)
    .replace('{2}', stat.param2)
    .replace('{3}', ' ')
    .replace('{4}', stat.param4)
    .replace('{5}', stat.param5)
    .replace('{6}', stat.param6);
}

class ItemStat extends React.PureComponent<IStat, object> {
  render() {
    if (this.props.text === '') {
      return null;
    }

    if (this.props.extras) {
      const text = statToStringSkip3(this.props);
      const modifier = text.substr(0, text.indexOf(' '));
      const label = text.substr(text.indexOf(' ') + 1);

      // TODO: We have a tooltip.. that means we got a skill in {3}
      return (
        <li>
          <a data-tip={this.props.extras} className="skill-trigger">
            <span className="modifier">{modifier}</span>&nbsp;
            <span className="label">{label}</span>
            <span className="modified-skill">{this.props.param3}</span>
          </a>
        </li>
      );
    } else {
      const text = statToString(this.props);
      const modifier = text.substr(0, text.indexOf(' '));
      const label = text.substr(text.indexOf(' ') + 1);
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
