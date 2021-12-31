import {h} from "preact";
import {PureComponent} from "preact/compat";
import {IReplicaRow} from "../../interfaces/IReplicaRow";


/**
 * Renders a single replica stat row
 * Responsible for parsing ^K ^S ^M color codes and row "type" into css classes
 */
class ReplicaStat extends PureComponent<IReplicaRow, object> {
  render() {
    const { text, type } = this.props;

    let result = '';
    for (let idx = text.length - 1; idx >= 0; idx--) {
      const c = text.charAt(idx);

      // Wrap the resulting text with classes for color-coding
      if (c === '^') {
        result = `<span class="replica-letter-${text.charAt(idx+1)}">` + result.substr(1) + "</span>";
      } else {
        result = c + result; // TODO: No need to do it in a loop, take the diff since last "halt"
      }


    }
    return <p class={"replica-type-" + type} dangerouslySetInnerHTML={{__html: result}}></p>;
  }
}

export default ReplicaStat;
