import * as React from 'react';
import ItemStat from './ItemStat';
import IItem from '../../interfaces/IItem';
import Skill from './Skill';
import { openUrl } from '../../integration/integration';
// @ts-ignore: Missing @types
import { Textfit } from 'react-textfit';
import translate from '../../translations/EmbeddedTranslator';
import IItemType from '../../interfaces/IItemType';
import GetSetName, { GetSetItems } from '../../integration/ItemSetService';
import ICollectionItem from '../../interfaces/ICollectionItem';


interface Props {
  item: IItem;
  transferSingle: (x: any) => void;
  transferAll: (x: any) => void;
  getItemName: (baseRecord: string) => ICollectionItem;
  requestUnknownItemHelp: () => void;
}

class Item extends React.PureComponent<Props, object> {
  openItemSite() {
    openUrl(`http://www.grimtools.com/db/search?query=${this.props.item.name}`);
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

  renderCornerContainer(item: IItem) {
    const showCloudOkIcon = item.type === IItemType.Player && item.hasCloudBackup;
    const showCloudErrorIcon = item.type === IItemType.Player && !item.hasCloudBackup;
    const showSingularBuddyItemIcon = item.type !== IItemType.Buddy && item.buddies.length === 1;
    const showPluralBuddyItemIcon = item.type !== IItemType.Buddy && item.buddies.length > 1;
    const showRecipeIcon = item.hasRecipe && item.type !== 0;
    const showAugmentationIcon = item.type === IItemType.Augmentation;

    return (
      <div className="recipe-item-corner">
        {showAugmentationIcon &&
        <span>
          <img
            className="cursor-help"
            src="static/gold-coins-sm.png"
            data-tip={translate('item.augmentPurchasable', item.extras)}
            alt="You can purchase this item"
          />

        </span>
        }

        {showCloudOkIcon &&
          <img
            className="cursor-help"
            src="static/cloud-ok.png"
            data-tip={translate('items.label.cloudOk')}
            alt={"Synced to the cloud"}
          />
        }

        {showCloudErrorIcon &&
        <img
          className="cursor-help"
          src="static/cloud-err.png"
          data-tip={translate('items.label.cloudError')}
          alt={"Not synced to the cloud"}
        />
        }

        {showSingularBuddyItemIcon &&
        <span>
            <img
              className="cursor-help"
              src="static/buddy.png"
              data-tip={translate('item.buddies.singular', item.buddies[0])}
              alt={"One of your buddies has this item"}
            />

          </span>
        }

        {showPluralBuddyItemIcon &&
        <span>
            <img
              className="cursor-help"
              src="static/buddy.png"
              data-tip={translate('item.buddies.plural', item.buddies.join('\n'))}
              alt={"Several of your buddies has this item"}
            />

            </span>
        }

        {showRecipeIcon &&
        <span data-tip={translate('items.label.youCanCraftThisItem')}>
          <img
            className="cursor-help"
            data-bind="click: function(item) { jumpToCraft(item.baseRecord); }"
            src="static\recipe.png"
            alt={"You can create this item (recipe)"}
          />
          </span>
        }
      </div>
    );
  }

  getSetItemTooltip(setName: string|undefined, quality: string): string {
    if (setName !== undefined && quality === 'Epic') { // We don't support blues yet, not available on collection page.
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

  render() {
    const item = this.props.item;
    const icon = (item.icon && item.icon.length) > 0 ? item.icon : 'weapon1h_focus02a.tex.png';
    const name = item.name.length > 0 ? item.name : 'Unknown';
    const socket = item.socket.replace(" ", "");

    const headerStats = item.headerStats.map((stat) =>
      <ItemStat label={stat.label} extras={stat.extras} key={'stat-head-' + item.url.join(':') + socket + stat.label}/>
    );

    const bodyStats = item.bodyStats.map((stat) =>
      <ItemStat label={stat.label} extras={stat.extras} key={'stat-body-' + item.url.join(':') + socket + stat.label}/>
    );

    const petStats = item.petStats.map((stat) =>
      <ItemStat label={stat.label} extras={stat.extras} key={'stat-pets-' + item.url.join(':') + socket + stat.label}/>
    );

    const setName = GetSetName(item.baseRecord);
    var setItemsList = this.getSetItemTooltip(setName, item.quality);

    const mainClasses = [
      "item",
      (item.numItems <= 0 && item.type === IItemType.Player ?' item-disabled':"")
    ];

    return (
      <div className={mainClasses.join(" ")}>
        <img src={icon} className={"item-icon item-icon-" + item.quality.toLowerCase()} data-tip={item.slot}/>
        <div className="text">
          <Textfit mode="multi" max={15} min={10}>
            <span>
              <a onClick={() => this.openItemSite()} className={this.translateQualityToClass(item.quality)}>{name}</a>
            </span>
              {item.greenRarity === 3 ? <span className="cursor-help supergreen" data-tip={translate('items.label.tripleGreen')}> (3)</span> : ''}
              {item.greenRarity === 2 ? <span className="cursor-help supergreen" data-tip={translate('items.label.doubleGreen')}> (2)</span> : ''}
          </Textfit>
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

        {this.renderCornerContainer(item)}

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

        {item.initialNumItems > 1 && item.numItems >= 1 && item.type === IItemType.Player ?
          <div className="link-container-all">
            <a onClick={() => this.props.transferAll(item.url)}>{translate('item.label.transferAll')} ({item.numItems})</a>
          </div>
          : ''
        }

        {item.numItems >= 1 && item.type === IItemType.Player ?
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
