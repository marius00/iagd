import {h} from "preact";
import {PureComponent} from "preact/compat";
import translate from "../translations/EmbeddedTranslator";
import styles from "./FirstRunHelpThingie.css";

class FirstRunHelpThingie extends PureComponent<{}, {}> {
  render() {
    return (
      <div className={styles.center}>
      <h2>Welcome to Grim Dawn Item Assistant</h2>
      <p>
        It seems you have already parsed the Grim Dawn database, so you're ready to get started. <br/>
        <br/>
        <br/>
        Step 1: Start Grim Dawn<br/>
        Step 2: Walk to the smuggler<br/>
        Step 3: Open the shared stash tab..<br/>
        <br/>
        This is where most people get it wrong, so make sure you're opening the SHARED stash, and not the PRIVATE stash.<br/>
        Remember there are two types of stashes at the smuggler.<br/>
        <br/>
        With the shared stash open, make sure you own at least two tabs. If you only own one, you need to purchase another before you can use IA.<br/>
        <br/>
        Once you have at least two stash tabs, simply place an item in the last tab, and watch it disappear from the game.<br/>
        <br/>
        <br/>
        The item will not immediately appear in Item Assistant, rather, you need to search for items to refresh the item view.<br/>
        The easiest way to do this, having just one item, is to just select and unselect any checkbox on the left side.<br/>
        <br/>
        The item should now appear inside Item Assistant (and this little walktrough, forever gone.. never to be seen again..)
        <br/>
        <br/>
        Should you run into any issues, you can usually find the answer on the help tab.

      </p>
    </div>);
  }
}

export default FirstRunHelpThingie;
