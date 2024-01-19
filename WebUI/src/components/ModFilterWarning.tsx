import {h} from "preact";
import {PureComponent} from "preact/compat";
import translate from "../translations/EmbeddedTranslator";
import styles from "./ModFilterWarning.css";

interface Props {
  numOtherItems: number;
}

class ModFilterWarning extends PureComponent<Props, object> {
  state = {
    isHidden: false,
  }
  render() {
  if (this.state.isHidden) {
    return null;
  }
  return (
  <div className={styles.outer}>
        <div className={styles.large +" "+ styles.large +" "+ styles.yellow +" "+ styles.border +" "+ styles.panel + " " + styles.container}>
          <span className={styles.button +" "+ styles.large +" "+ styles.topright} onClick={() => this.setState({isHidden: true})}>×</span>
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
