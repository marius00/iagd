import {h} from "preact";
import {PureComponent} from "preact/compat";
import translate from "../translations/EmbeddedTranslator";
import styles from "./EasterEgg.css";

interface Props {
  close: () => void;
}

class EasterEgg extends PureComponent<Props, {}> {
  render() {
    return Math.random() < 0.5 ? this.render2() : this.render3();
  }
  render3() {
    return (
      <div className={styles.center + " " + styles.yellowmodal}>
      <h1>Free item limit reached!</h1>
      <p>
        Your free item limit has been reached.<br/>
      <br/>
        To continue using Item Assistant, purchase the full version and unlock unlimited items.
        <br/>
      <br/>
        <p className={styles.btnSubscribe} onClick={() => this.props.close()}>Subscribe now for only $19.99/mo</p>
      </p>
    </div>);
  }

render2() {
  return (
    <div className={styles.center + " " + styles.yellowmodal}>
    <h1>Your free trial has expired!</h1>
    <br/>
    <p>
      Your free trial of Item Assistant has expired.<br/>
    <br/>
      To continue using Item Assistant, please purchase the full version.
      <br/>
    <br/>
      <p className={styles.btnSubscribe} onClick={() => this.props.close()}>Unlock now for only $69.95</p>
    </p>
  </div>);
}
}

export default EasterEgg;
