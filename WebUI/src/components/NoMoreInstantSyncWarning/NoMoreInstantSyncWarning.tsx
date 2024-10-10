import {h} from "preact";
import {PureComponent} from "preact/compat";
import styles from "./NoMoreInstantSyncWarning.css";

interface Props {
  close: () => void;
}

class NoMoreInstantSyncWarning extends PureComponent<Props, object> {
  state = {
    isHidden: false,
  }

  render() {
    if (this.state.isHidden) {
      return null;
    }
    return (
      <div className={styles.outer}>
        <div className={styles.large + " " + styles.large + " " + styles.black + " " + styles.border + " " + styles.panel + " " + styles.container}>
          <img src="/static/header.jpg" class={styles.img}/>
          <br/>
          <p class={styles.text}>Congratulations! <br/>
            <br/>
            You have more than 300 items stored in Item Assistant.<br/>
            When you first started using Item Assistant, it automatically updated your item list every time you looted something new. But as your collection grows, that approach become less and less practical.<br/>
            <br/>
            From now on, the item list won’t update automatically with new loot. To see new items, you’ll need to perform a search. If you just want to refresh the list, double-click any checkbox on the left.<br/>
            <br/>
            This change will help keep your item view stable, without jumping around for a single new item being added to a list of thousands.<br/>
          </p>
          <br/>
          <p className={styles.btn} onClick={ () => this.props.close()}>I understand</p>
        </div>
      </div>
    );
  }
}

// <span className={styles.button + " " + styles.large + " " + styles.topright} onClick={() => this.setState({isHidden: true})}>×</span>
export default NoMoreInstantSyncWarning;
