import {h} from "preact";
import {PureComponent} from "preact/compat";
import styles from "./GdSeasonError.css";

interface Props {
  close: () => void;
}

class GdSeasonError extends PureComponent<Props, {}> {
  render() {
    return (
      <div className={styles.center + " " + styles.yellowmodal}>
      <h2>Grim League detected</h2>
      <p>
        The use of Item Assistant is not permitted when playing Grim League.<br/>
      <br/>
        You can safely keep Item Assistant running, IA will not interfer with the game as long as Grim League is running.
        <br/>
      <br/>
        <p className={styles.btnClose} onClick={() => this.props.close()}>Close</p>
      </p>
    </div>);
  }
}

export default GdSeasonError;
