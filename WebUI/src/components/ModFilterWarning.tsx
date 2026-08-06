import {h} from "preact";
import {PureComponent} from "preact/compat";
import translate from "../translations/EmbeddedTranslator";
import styles from "./ModFilterWarning.module.css";

interface Props {
  numOtherItems: number;
  close: () => void;
}

// Dismissal is owned by App, not this component: it is unmounted whenever the user leaves the search
// tab, so local state would forget the dismissal and the warning would pop back up on return.
class ModFilterWarning extends PureComponent<Props, object> {
  render() {
  return (
  <div className={styles.outer}>
        <div className={styles.large +" "+ styles.large +" "+ styles.yellow +" "+ styles.border +" "+ styles.panel + " " + styles.container}>
          <span className={styles.button +" "+ styles.large +" "+ styles.topright} onClick={() => this.props.close()}>×</span>
          <h3>Warning!</h3>
          <p>You have an additional {this.props.numOtherItems} items which were filtered out due to the mod filter.</p>
          <p>If you are having trouble finding your items, check the mod filter drop down in the top right corner.</p>
          <p>This is used to differentiate between softcore and hardcore stashes, as well as items from various mods.</p>
        </div>
        </div>
    );
  }
}

export default ModFilterWarning;
