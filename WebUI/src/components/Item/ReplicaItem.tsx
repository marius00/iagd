import {h} from "preact";
import ItemStat from './ItemStat';
import IItem from '../../interfaces/IItem';
import Skill from './Skill';
import { isEmbedded, openUrl } from '../../integration/integration';
// @ts-ignore: Missing @types
import translate from '../../translations/EmbeddedTranslator';
import IItemType from '../../interfaces/IItemType';
import GetSetName, { GetSetItems } from '../../integration/ItemSetService';
import ICollectionItem from '../../interfaces/ICollectionItem';
import { IStat, statToString } from '../../interfaces/IStat';
import ItemCornerContainer from './ItemCornerContainer';
import { v4 as uuidv4 } from 'uuid';
import {PureComponent} from "preact/compat";
import ReplicaStatContainer from "./ReplicaStatContainer";
import styles from './ReplicaItem.css';

interface Props {
  item: IItem;
  transferSingle: (item: IItem) => void;
  getItemName: (baseRecord: string) => ICollectionItem;
  showBackupCloudIcon: boolean;
}

export function getUniqueId(item: IItem): string {
  if (item.uniqueIdentifier) return item.uniqueIdentifier;
  else {
    console.warn("Could not find unique identifier for item, defaulting to uuid", item);
    return uuidv4();
  }
}

/**
 * Comparison item
 */
class ReplicaItem extends PureComponent<Props, object> {
  stripColorCodes(initialString: any | string): string {
    return initialString?.replaceAll(/{?\^.}?/g, '');
  }

  renderBuddyItem(item: IItem) {
    if (item.type === IItemType.Buddy) {
      return (
        <div className="buddy-item-mix">
          &nbsp;{translate('item.buddies.singularOnly', item.extras)}
        </div>
      );
    }

    return null;
  }

  translateQualityToClass(quality: string): string {
    return `item-quality-${quality.toLowerCase()}`;
  }
  
  getSetItemTooltip(setName: string|undefined, isHardcore: boolean): string {
    const getNumItems = (item: ICollectionItem) => isHardcore ? item.numOwnedHc : item.numOwnedSc;

    if (setName !== undefined && isEmbedded) {
      const addFont = (numOwned: number, s: string) => (numOwned <= 0) ? `<font color="red">${s}</font>` : s;
      let setItemsList = GetSetItems(setName)
        .map(this.props.getItemName)
        .map(entry => addFont(
          getNumItems(entry),
          `${("  " + getNumItems(entry)).substr(-2,2)}x ${this.stripColorCodes(entry.name)}`))
        .join("<br>");

      setItemsList = `<b>${translate('item.label.setConsistsOf')}</b><br><br>` + setItemsList;
      return setItemsList;
    }

    return "";
  }

  statToString(stat: IStat) {
    return stat.text
      .replace("{0}", stat.param0)
      .replace("{1}", stat.param1)
      .replace("{2}", stat.param2)
      .replace("{3}", stat.param3)
      .replace("{4}", stat.param4)
      .replace("{5}", stat.param5)
      .replace("{6}", stat.param6);
  }

  renderIcon() {
    const item = this.props.item;
    let icon = (item.icon && item.icon.length) > 0 ? item.icon : 'weapon1h_focus02a.tex.png';
    if (!isEmbedded) // Online items stores icons separately
      icon = `http://static.iagd.evilsoft.net/img/${icon}`;

    return <div className="item-icon-container" data-tip={item.slot}>
      <div className={'item-icon-background item-icon-'+ item.quality.toLowerCase()} />
      <img src={icon} className={"item-icon" }/>
    </div>;
  }

  render() {
    const item = this.props.item;
    const name = item.name.length > 0 ? this.stripColorCodes(item.name) : 'Unknown';
    const socket = item.socket.replace(" ", "");
    const { type } = item;

    const headerStats = item.headerStats.map((stat) =>
      <ItemStat {...stat} key={`stat-head-${getUniqueId(item)}-${socket}-${statToString(stat)}`.replace(' ', '_')} />
    );

    const bodyStats = item.bodyStats.map((stat) =>
      <ItemStat {...stat} key={`stat-body-${getUniqueId(item)}-${socket}-${statToString(stat)}`.replace(' ', '_')} />
    );

    const petStats = item.petStats.map((stat) =>
      <ItemStat {...stat} key={`stat-pets-${getUniqueId(item)}-${socket}-${statToString(stat)}`.replace(' ', '_')} />
    );

    const setName = GetSetName(item.baseRecord);
    let setItemsList = this.getSetItemTooltip(setName, item.isHardcore);

    const miText = item.isMonsterInfrequent ? ' / MI' : '';
    return (
      <div className={"item"}>
        {this.renderIcon()}
        <div className="text">
          <div>
            <span>
              <a className={this.translateQualityToClass(item.quality)}>{name}</a>
            </span>
              {item.greenRarity === 3 ? <span className="cursor-help supergreen" data-tip={translate('items.label.tripleGreen')}> (TripleRare{miText})</span> : ''}
              {item.greenRarity === 2 ? <span className="cursor-help supergreen" data-tip={translate('items.label.doubleGreen')}> (DoubleRare{miText})</span> : ''}
          </div>
          {item.socket && item.socket.length > 0 &&
          <span className="item-socket-label">{item.socket}</span>
          }

          {item.replicaStats && <ReplicaStatContainer rows={item.replicaStats} id={getUniqueId(item)} skills={item.bodyStats} hideGrantedSkill hideSetBonus /> }
          <ul className="headerStats">
            {headerStats}
          </ul>

          <br/>

          <ul className="bodystats">
            {!item.replicaStats && bodyStats}
          </ul>

          {petStats.length > 0 ? (
            <div className="pet-stats">
              <div className="pet-header">{translate('item.label.bonusToAllPets')}</div>
              {petStats}
            </div>
          ) : ''
          }

          {setName !== undefined && <div><br />
            <span className="set-name">{translate('item.label.setbonus')}</span> <span className="set-name" data-tip={setItemsList}>{setName}</span></div>}

          {item.skill ? <Skill skill={item.skill} keyPrefix={getUniqueId(item)}/> : ''}

        </div>
        {this.renderBuddyItem(item)}

        <div className="level">
          <p>{translate('item.label.levelRequirement', item.level > 1 ? String(item.level) : translate('item.label.levelRequirementAny'))}</p>
        </div>

        {type === IItemType.Player ?
          <div className="link-container">
            <a onClick={() => this.props.transferSingle(item)}>{translate('item.label.transferSingle')}</a>
          </div>
          : ''
        }

        {type === IItemType.Buddy && <div className={styles.watermarkContainer}>
          <p className={styles.watermark}>{translate('item.buddies.watermark')}</p>
        </div>}
        {type === IItemType.Player && !item.replicaStats && <div className={styles.watermarkContainer}>
            <p className={styles.watermark}>{translate('item.genericstats.warning')}</p>
        </div>}
      </div>
    );
  }
}

export default ReplicaItem;
