import {h} from "preact";
import {PureComponent} from "preact/compat";
import IItem from "../../interfaces/IItem";
import styles from "./ItemComparer.css";
import Item, {getUniqueId} from "./Item";
import ReplicaItem from "./ReplicaItem";
import ICollectionItem from "../../interfaces/ICollectionItem";

interface Props {
  item: IItem[];
  transferSingle: (item: IItem) => void;
  onClose: () => void;
  showBackupCloudIcon: boolean;
  getItemName: (baseRecord: string) => ICollectionItem; // TODO: Deprecated?
}

class ItemComparer extends PureComponent<Props, object> {
  componentDidMount() {
    setTimeout(() => document.addEventListener("click", this.handleOutsideClick, false), 500);
  }

  componentWillUnmount() {
    document.removeEventListener("click", this.handleOutsideClick, false);
  }

  handleOutsideClick = (e: any) => {
    const base = this.base;

    if (base) {
      const isOutsideModal = !base.contains(e.target);
      if (isOutsideModal)
        this.props.onClose();
    }
  };

  render() {
    const items = this.props.item;

    return (
      <div className={styles.itemComparer + " itemComparer"}>
        <header className={styles.itemHeader}>
          <h2>Item Comparison</h2>
          <span className={styles.closeButton} onClick={() => this.props.onClose()}>X</span>
        </header>
        <div className={styles.itemList}>
          {items.map((item) =>
            <ReplicaItem
              item={item}
              key={'cmp-' + getUniqueId(item)}
              transferSingle={(item: IItem) => this.props.transferSingle(item)}
              getItemName={this.props.getItemName}
              showBackupCloudIcon={this.props.showBackupCloudIcon}
            />
          )}
        </div>
      </div>
    );
  }
}

export default ItemComparer;
