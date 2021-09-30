import {h} from "preact";
import { IStat } from '../../interfaces/IStat';
import {PureComponent} from "preact/compat";


function statToString(text: string, stat: IStat) {
  return text
    .replace('{0}', stat.param0)
    .replace('{1}', stat.param1)
    .replace('{2}', stat.param2)
    .replace('{3}', stat.param3)
    .replace('{4}', stat.param4)
    .replace('{5}', stat.param5)
    .replace('{6}', stat.param6);
}


// TODO: Color differentiation on {4}
class ItemStat extends PureComponent<IStat, object> {
  render() {
    if (this.props.text === '') {
      return null;
    }

    if (this.props.extras) {
      let text = statToString(this.props.text.replace('{3}', ' '), this.props);
      let modifier = text.substr(0, text.indexOf(' '));
      let label = text.substr(text.indexOf(' ') + 1);

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
      const text = statToString(this.props.text, this.props);
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
