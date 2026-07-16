import {h} from "preact";
import {PureComponent} from "preact/compat";
import styles from "./NumericFilterBanner.module.css";
import {darkFilterScreenshot, lightFilterScreenshot} from "./NumericFilterBanner.images";

interface Props {
  close: () => void;
}

class NumericFilterBanner extends PureComponent<Props, object> {
  render() {
    return (
      <div className={styles.backdrop}>
        <div className={styles.modal}>
          <h2>Advanced filtering</h2>
          <p>
            You can now filter your items on the numeric value of a stat, not just whether the item has it at all.<br/>
            <br/>
            Hover a stat in the filter panel on the left and click the funnel button that appears, then pick a
            comparison such as "&ge; 30". Only items whose value matches will be shown.
          </p>

          <img className={styles.screenshot + " " + styles.lightScreenshot} src={lightFilterScreenshot}
               alt="The funnel button next to a stat, and the value filter dialog it opens"/>
          <img className={styles.screenshot + " " + styles.darkScreenshot} src={darkFilterScreenshot}
               alt="The funnel button next to a stat, and the value filter dialog it opens"/>

          <br/>
          <p className={styles.btnConfirm} onClick={() => this.props.close()}>Got it</p>
        </div>
      </div>);
  }
}

export default NumericFilterBanner;
