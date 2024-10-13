import {h} from "preact";
import {PureComponent} from "preact/compat";
import IItem from "../../interfaces/IItem";
import styles from "./ItemComparer.css";
import Item, {getUniqueId} from "./Item";
import ReplicaItem from "./ReplicaItem";
import ICollectionItem from "../../interfaces/ICollectionItem";
import {Collection} from 'react-virtualized';

interface Props {
  item: IItem[];
  transferSingle: (item: IItem) => void;
  onClose: () => void;
  showBackupCloudIcon: boolean;
  getItemName: (baseRecord: string) => ICollectionItem; // TODO: Deprecated?
}

class ItemComparer extends PureComponent<Props, object> {
  _columnYMap: any;


  constructor() {
    super();

    this._columnYMap = [];
    this._cellSizeAndPositionGetter = this._cellSizeAndPositionGetter.bind(this,);
    this.renderCell = this.renderCell.bind(this,);
  }

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
    return (
      <div className={styles.itemComparer + " itemComparer"}>
        <header className={styles.itemHeader}>
          <h2>Item Comparison</h2>
          <span className={styles.closeButton} onClick={() => this.props.onClose()}>X</span>
        </header>
        <Collection
          cellCount={3}
          cellRenderer={this.renderCell}
          width={window.innerWidth * .8}
          height={window.innerHeight * .6}
          cellSizeAndPositionGetter={this._cellSizeAndPositionGetter}
        />
      </div>
    );
  }


  _cellSizeAndPositionGetter(args: any) {
    const list = this.props.item;

    const index = args.index;
    const columnPosition = index % 3;
    const datum = list[index % list.length];
    console.log(index, columnPosition, datum);

    // Poor man's Masonry layout; columns won't all line up equally with the bottom.
    const height = 620;
    const width = 615;
    const x = columnPosition * (3 + width);
    const y = (height + 3) * Math.floor(index/3); //this._columnYMap[columnPosition] || 3;

    this._columnYMap[columnPosition] = y + height + 3;

    return {
      height,
      width,
      x,
      y,
    };
  }

  renderCell(args: any) {
    const index = args.index as number;
    const isScrolling = args.isScrolling as boolean;
    const key = args.key as any; // int?
    const style = args.style as any;


    const item = this.props.item[index];
    console.log('rendering:', index, isScrolling, key, style, item)
    if (!item) {
      return null;
    }

    return <ReplicaItem
      style={style}
      item={item}
      key={'cmp-' + getUniqueId(item)}
      transferSingle={(item: IItem) => this.props.transferSingle(item)}
      getItemName={this.props.getItemName}
      showBackupCloudIcon={this.props.showBackupCloudIcon}
    />;
  }
}

export default ItemComparer;
