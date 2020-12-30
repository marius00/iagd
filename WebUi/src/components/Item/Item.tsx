import * as React from 'react';
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


interface Props {
  item: IItem;
  transferSingle: (x: any) => void;
  transferAll: (x: any) => void;
  getItemName: (baseRecord: string) => ICollectionItem;
  requestUnknownItemHelp: () => void;
}

class Item extends React.PureComponent<Props, object> {
  openItemSite() {
    openUrl(`http://www.grimtools.com/db/search?src=itemassistant&query=${this.props.item.name}`);
  }

  renderBuddyItem(item: IItem) {
    if (item.type === IItemType.Buddy) {
      if (item.buddies.length === 1) {
        return (
          <div className="buddy-item-mix">
            &nbsp;{translate('item.buddies.singularOnly', item.buddies[0])}
          </div>
        );
      } else {
        return (
          <div className="buddy-item-mix" data-bind="attr: {title: buddies.join()}">
            <a data-tip={'&laquo;' + item.buddies.length + '&raquo; of your Buddies also have this item.'} />
          </div>
        );
      }
    }

    return null;
  }

  translateQualityToClass(quality: string): string {
    return `item-quality-${quality.toLowerCase()}`;
  }


  getSetItemTooltip(setName: string|undefined): string {
    if (setName !== undefined && isEmbedded) {
      const addFont = (numOwned: number, s: string) => (numOwned <= 0) ? `<font color="red">${s}</font>` : s;
      var setItemsList = GetSetItems(setName)
        .map(this.props.getItemName)
        .map(entry => addFont(entry.numOwned, `${("  " + entry.numOwned).substr(-2,2)}x ${entry.name}`))
        .join("<br>");

      setItemsList = `<b>${translate('item.label.setConsistsOf')}</b><br><br>` + setItemsList;
      return setItemsList;
    }

    return "";
  }

  // TODO:
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

    return <div className="item-icon-container">
      <div className={'item-icon-background item-icon-'+ item.quality.toLowerCase()} />
      <img src={icon} className={"item-icon" } data-tip={item.slot}/>
    </div>;
  }

  render() {
    const item = this.props.item;
    const name = item.name.length > 0 ? item.name : 'Unknown';
    const socket = item.socket.replace(" ", "");



    const headerStats = item.headerStats.map((stat) =>
      <ItemStat {...stat} key={'stat-head-' + item.url.join(':') + socket + statToString(stat)} />
    );

    const bodyStats = item.bodyStats.map((stat) =>
      <ItemStat {...stat} key={'stat-body-' + item.url.join(':') + socket + statToString(stat)} />
    );

    const petStats = item.petStats.map((stat) =>
      <ItemStat {...stat} key={'stat-pets-' + item.url.join(':') + socket + statToString(stat)} />
    );

    const setName = GetSetName(item.baseRecord);
    var setItemsList = this.getSetItemTooltip(setName);

    const mainClasses = [
      "item",
      (item.numItems <= 0 && item.type === IItemType.Player ?' item-disabled':"")
    ];

    const miText = item.isMonsterInfrequent ? ' / MI' : '';
    return (
      <div className={mainClasses.join(" ")}>
        {this.renderIcon()}
        <div className="text">
          <div>
            <span>
              <a onClick={() => this.openItemSite()} className={this.translateQualityToClass(item.quality)}>{name}</a>
            </span>
              {item.greenRarity === 3 ? <span className="cursor-help supergreen" data-tip={translate('items.label.tripleGreen')}> (3{miText})</span> : ''}
              {item.greenRarity === 2 ? <span className="cursor-help supergreen" data-tip={translate('items.label.doubleGreen')}> (2{miText})</span> : ''}
          </div>
          {item.socket && item.socket.length > 0 &&
          <span className="item-socket-label">{item.socket}</span>
          }

          <ul className="headerStats">
            {headerStats}
          </ul>

          <br/>

          <ul className="bodystats">
            {bodyStats}
          </ul>

          {petStats.length > 0 ? (
            <div className="pet-stats">
              <div className="pet-header">{translate('item.label.bonusToAllPets')}</div>
              {petStats}
            </div>
          ) : ''
          }

          {setName !== undefined && <div><br />
            <span className="set-name">{translate('item.label.setbonus')}</span> <span className="set-name" data-feature="SetBonus" data-tip={setItemsList}>{setName}</span></div>}

          {item.skill ? <Skill skill={item.skill} keyPrefix={item.url.join(':')}/> : ''}

        </div>
        {item.buddies.length > 0 ? this.renderBuddyItem(item) : ''}

        {item.hasRecipe && item.type !== IItemType.Recipe ?
          <span className="informative">
            <a data-tip={translate('items.label.youCanCraftThisItem')}>
              <div className="recipe-item-corner">
                <img className="cursor-help" src="static\recipe.png"/>
              </div>
            </a>
          </span>
          : ''
        }

        <ItemCornerContainer {...item} />

        {item.hasRecipe && item.type === IItemType.Recipe ?
          <div className="recipe-item">
            <img src="static\recipe.png"/>
            <span className="craft-link">
              &nbsp;<span>{translate('items.label.youCanCraftThisItem')}</span>
            </span>
          </div>
          : ''
        }

        <div className="level">
          <p>{translate('item.label.levelRequirement', item.level > 1 ? String(item.level) : translate('item.label.levelRequirementAny'))}</p>
        </div>

        {item.initialNumItems > 1 && item.numItems >= 1 && item.type === IItemType.Player && isEmbedded ?
          <div className="link-container-all">
            <a onClick={() => this.props.transferAll(item.url)}>{translate('item.label.transferAll')} ({item.numItems})</a>
          </div>
          : ''
        }

        {item.numItems >= 1 && item.type === IItemType.Player && isEmbedded ?
          <div className="link-container">
            <a onClick={() => this.props.transferSingle(item.url)}>{translate('item.label.transferSingle')}</a>
          </div>
          : ''
        }

        {item.numItems <= 0 && item.type === IItemType.Player ?
          <div className="link-container no-more-items">
            {translate('item.label.noMoreItems')}
          </div>
          : ''
        }

        {item.type === IItemType.Player && this.props.item.numItems > 50 && <div className="unknownitem"><a onClick={() => this.props.requestUnknownItemHelp()}>You may be experiencing issues.. Click here for more information.</a></div>}
      </div>
    );
  }
}

export default Item;
