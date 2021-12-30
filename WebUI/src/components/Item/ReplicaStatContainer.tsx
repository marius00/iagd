import {h} from "preact";
import {PureComponent} from "preact/compat";
import {IReplicaRow} from "../../interfaces/IReplicaRow";
import ReplicaStat from "./ReplicaStat";

interface Props {
  rows: IReplicaRow[];
  id: string;
  tooltip: string;
}

class ReplicaStatContainer extends PureComponent<Props, object> {
  render() {
    if (this.props.rows === null || this.props.rows.length === 0)
      return null;

    console.log(this.props);
    // TODO: Need something additional for the key..

    // @ts-ignore: replaceAll does exist.
    const tooltip = this.props.tooltip.replaceAll('\n', '<br/>');

    return (
      <p className="replica" data-tip={tooltip}>
        {this.props.rows.map((row, idx) => <ReplicaStat {...row} key={this.props.id + idx} /> )}
      </p>
    );
  }
}

export default ReplicaStatContainer;
