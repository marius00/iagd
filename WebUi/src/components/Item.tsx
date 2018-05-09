import * as React from 'react';
import 'react-select/dist/react-select.css';
import ItemStat from './ItemStat';
import IItem from '../interfaces/IItem';
import Skill from './Skill';
import * as ReactTooltip from 'react-tooltip';
import { isEmbedded } from '../constants/index';
import * as Guid from 'guid';
import translate from '../translations/EmbeddedTranslator';
import IItemType from '../interfaces/IItemType';

const buddyIcon = require('./img/buddy.png');
const recipeIcon = require('./img/recipe.png');
const cloudErrIcon = require('./img/cloud-err.png');
const cloudOkIcon = require('./img/cloud-ok.png');
const purchaseableItem = require('./img/gold-coins-sm.png');

interface Props {
  item: IItem;
  transferSingle: (x: any) => void;
  transferAll: (x: any) => void;
}

class Item extends React.Component<Props, object> {

  openItemSite() {
    if (isEmbedded) {
      document.location.href = `http://www.grimtools.com/db/search?query=${this.props.item.name}`;
    } else {
      window.open(`http://www.grimtools.com/db/search?query=${this.props.item.name}`);
    }
  }

  renderBuddyItem(item: IItem) {
    const tooltipId = Guid.raw();
    if (item.type === IItemType.Buddy) {
      if (item.buddies.length === 1) {
        return (
          <div className="buddy-item-mix">
            &nbsp;&laquo; {translate('item.buddies.singular', item.buddies[0])}
          </div>
        );
      } else {
        return (
          <div className="buddy-item-mix" data-bind="attr: {title: buddies.join()}">
            <a data-tip="true" data-for={tooltipId}>
              &laquo;{item.buddies.length}&raquo; of your Buddies also have this item.
            </a>

            <ReactTooltip id={tooltipId}><span>{translate('item.buddies.plural', item.buddies.join('\n'))}</span>
            </ReactTooltip>
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
    const buddyTooltip = Guid.raw();

    // purchaseableItem

    const showCloudOkIcon = item.type === IItemType.Player && item.hasCloudBackup;
    const showCloudErrorIcon = item.type === IItemType.Player && !item.hasCloudBackup;
    const showSingularBuddyItemIcon = item.type !== IItemType.Buddy && item.buddies.length === 1;
    const showPluralBuddyItemIcon = item.type !== IItemType.Buddy && item.buddies.length > 1;
    const showRecipeIcon = item.hasRecipe && item.type !== 0;
    const showAugmentationIcon = item.type === IItemType.Augmentation;

    let augmentationTooltip = Guid.raw();

    return (
      <div className="recipe-item-corner">
        {showAugmentationIcon &&
        <span>
          <img
            className="cursor-help"
            src={purchaseableItem}
            data-tip="true"
            data-for={augmentationTooltip}
          />

          <ReactTooltip id={augmentationTooltip}><span>{translate('item.augmentPurchasable', item.extras)}</span></ReactTooltip>
        </span>
        }

        {showCloudOkIcon &&
          <img
            className="cursor-help"
            src={cloudOkIcon}
            data-tip="true"
            data-for="cloud-ok-tooltip"
          />
        }

        {showCloudErrorIcon &&
        <img
          className="cursor-help"
          src={cloudErrIcon}
          data-tip="true"
          data-for="cloud-err-tooltip"
        />
        }

        {showSingularBuddyItemIcon &&
        <span>
            <img
              className="cursor-help"
              src={buddyIcon}
              data-tip="true"
              data-for={buddyTooltip}
            />
            <ReactTooltip id={buddyTooltip}>
              <span>{translate('item.buddies.singular', item.buddies[0])}</span>
            </ReactTooltip>
          </span>
        }

        {showPluralBuddyItemIcon &&
        <span>
            <img
              className="cursor-help"
              src={buddyIcon}
              data-tip="true"
              data-for={buddyTooltip}
            />
            <ReactTooltip id={buddyTooltip}>
              <span>{translate('item.buddies.plural', item.buddies.join('\n'))}</span>
            </ReactTooltip>
            </span>
        }

        {showRecipeIcon &&
        <span data-tip="true" data-for="you-can-craft-this-item-tooltip">
          <img
            className="cursor-help"
            data-bind="click: function(item) { jumpToCraft(item.baseRecord); }"
            src={recipeIcon}
          />
          </span>
        }
      </div>
    );
  }

  render() {
    const item = this.props.item;
    const icon = item.name.length > 0 ? item.icon : 'weapon1h_focus02a.tex.png';
    const name = item.name.length > 0 ? item.name : 'Unknown';

    const headerStats = item.headerStats.map((stat) =>
      <ItemStat label={stat.label} extras={stat.extras} key={'stat-head-' + item.url.join(':') + stat.label}/>
    );

    const bodyStats = item.bodyStats.map((stat) =>
      <ItemStat label={stat.label} extras={stat.extras} key={'stat-body-' + item.url.join(':') + stat.label}/>
    );

    const petStats = item.petStats.map((stat) =>
      <ItemStat label={stat.label} extras={stat.extras} key={'stat-pets-' + item.url.join(':') + stat.label}/>
    );

    const itemLogoSlotTooltip = Guid.raw();
    return (
      <div className="item">
        <span>
          <img src={icon} className="item-icon" data-tip="true" data-for={itemLogoSlotTooltip}/>
          <ReactTooltip id={itemLogoSlotTooltip}><span>{item.slot}</span>
          </ReactTooltip>
        </span>
        <div className="text">
          <h3 className="item-name-header">
            <span>
              <a onClick={() => this.openItemSite()} className={this.translateQualityToClass(item.quality)}>{name}</a>
            </span>
            {item.greenRarity === 3 ? <span className="cursor-help" data-tip="true" data-for="triple-green-tooltip">(+2)</span> : ''}
            {item.greenRarity === 2 ? <span className="cursor-help" data-tip="true" data-for="double-green-tooltip">(+1)</span> : ''}
          </h3>
          {item.socket && item.socket.length > 0 &&
          <span className="item-socket-label">{item.socket}</span>
          }

          <ul>
            {headerStats}
          </ul>

          <ul className="bodystats">
            {bodyStats}
          </ul>

          {petStats.length > 0 ? (
            <div>
              <div className="pet-header">{translate('item.label.bonusToAllPets')}</div>
              {petStats}
            </div>
          ) : ''
          }

          {item.skill ? <Skill skill={item.skill} keyPrefix={item.url.join(':')}/> : ''}

        </div>
        {item.buddies.length > 0 ? this.renderBuddyItem(item) : ''}

        {item.hasRecipe && item.type !== IItemType.Recipe ?
          <span>
            <a data-tip="true" data-for="you-can-craft-this-item-tooltip">
              <div className="recipe-item-corner">
                <img className="cursor-help" src={recipeIcon}/>
              </div>
            </a>
          </span>
          : ''
        }

        {this.renderCornerContainer(item)}

        {item.hasRecipe && item.type === IItemType.Recipe ?
          <div className="recipe-item">
            <img src={recipeIcon}/>
            <span className="craft-link" data-bind="click: function(item) { jumpToCraft(item.baseRecord); }">
              &nbsp;<span>{translate('items.label.youCanCraftThisItem')}</span>
            </span>
          </div>
          : ''
        }

        <div className="level">
          <p>{translate('item.label.levelRequirement', item.level > 1 ? String(item.level) : translate('item.label.levelRequirementAny'))}</p>
        </div>

        {item.numItems > 1 && item.type === IItemType.Recipe ?
          <div className="link-container-all">
            <a onClick={() => this.props.transferAll(item.url)}>{translate('item.label.transferAll')} ({item.numItems})</a>
          </div>
          : ''
        }

        {item.type === IItemType.Player ?
          <div className="link-container">
            <a onClick={() => this.props.transferSingle(item.url)}>{translate('item.label.transferSingle')}</a>
          </div>
          : ''
        }
      </div>
    );
  }
}

export default Item;
