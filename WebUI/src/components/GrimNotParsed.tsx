import {h} from "preact";
import {PureComponent} from "preact/compat";
import translate from "../translations/EmbeddedTranslator";
import styles from "./GrimNotParsed.css";

class GrimNotParsed extends PureComponent<{}, {}> {
  render() {
    return (
      <div className={styles.center}>
      {translate('app.error.grimnotparsed')}
    </div>);
  }
}

export default GrimNotParsed;
