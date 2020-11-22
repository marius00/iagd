import * as React from 'react';
import 'react-select/dist/react-select.css';
import ItemStat from './ItemStat';
import IItem from '../interfaces/IItem';
import Skill from './Skill';
import { isEmbedded } from '../constants/index';
import { Textfit } from 'react-textfit';
import translate from '../translations/EmbeddedTranslator';
import IItemType from '../interfaces/IItemType';
import GetSetName from '../logic/ItemSetLookupService';

const buddyIcon = require('./img/buddy.png');
const recipeIcon = require('./img/recipe.png');
const cloudErrIcon = require('./img/cloud-err.png');
const cloudOkIcon = require('./img/cloud-ok.png');
const purchasableItem = require('./img/gold-coins-sm.png');

interface Props {
  item: IItem;
  transferSingle: (x: any) => void;
  transferAll: (x: any) => void;
}

class Item extends React.PureComponent<Props, object> {
  openItemSite() {
    let url = `http://www.grimtools.com/db/search?query=${this.props.item.name}`;
    if (this.props.item.numItems > 50) {
      console.log('Unknown item, redirecting to help page.');
      window.open('http://grimdawn.dreamcrash.org/ia/help.html?q=UnknownItem');
    } else if (isEmbedded) {
      document.location.href = url;
    } else {
      window.open(url);
    }
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
            src={purchasableItem}
            data-tip={translate('item.augmentPurchasable', item.extras)}
          />

        </span>
        }

        {showCloudOkIcon &&
          <img
            className="cursor-help"
            src={cloudOkIcon}
            data-tip={translate('items.label.cloudOk')}
          />
        }

        {showCloudErrorIcon &&
        <img
          className="cursor-help"
          src={cloudErrIcon}
          data-tip={translate('items.label.cloudError')}
        />
        }

        {showSingularBuddyItemIcon &&
        <span>
            <img
              className="cursor-help"
              src={buddyIcon}
              data-tip={translate('item.buddies.singular', item.buddies[0])}
            />

          </span>
        }

        {showPluralBuddyItemIcon &&
        <span>
            <img
              className="cursor-help"
              src={buddyIcon}
              data-tip={translate('item.buddies.plural', item.buddies.join('\n'))}
            />

            </span>
        }

        {showRecipeIcon &&
        <span data-tip={translate('items.label.youCanCraftThisItem')}>
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
    const icon = (item.icon && item.icon.length) > 0 ? item.icon : 'weapon1h_focus02a.tex.png';
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

    const setName = GetSetName(item.baseRecord);

    return (
      <div className="item">
        <span>
          <img src={icon} className="item-icon" data-tip={item.slot}/>
        </span>
        <div className="text">
          <Textfit mode="multi" max={15} min={10}>
            <span>
              <a onClick={() => this.openItemSite()} className={this.translateQualityToClass(item.quality)}>{name}</a>
            </span>
              {item.greenRarity === 3 ? <span className="cursor-help" data-tip={translate('items.label.tripleGreen')}>(+2)</span> : ''}
              {item.greenRarity === 2 ? <span className="cursor-help" data-tip={translate('items.label.doubleGreen')}>(+1)</span> : ''}
          </Textfit>
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

          {setName !== undefined && <div><br />{translate('item.label.setbonus')} <span className="set-name">{setName}</span></div>}

          {item.skill ? <Skill skill={item.skill} keyPrefix={item.url.join(':')}/> : ''}

        </div>
        {item.buddies.length > 0 ? this.renderBuddyItem(item) : ''}

        {item.hasRecipe && item.type !== IItemType.Recipe ?
          <span>
            <a data-tip={translate('items.label.youCanCraftThisItem')}>
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

        {item.numItems > 1 && item.type === IItemType.Player ?
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

        {item.type === IItemType.Player && this.props.item.numItems > 50 && <div className="unknownitem"><a onClick={() => this.openItemSite()}>You may be experiencing issues.. Click here for more information.</a></div>}
      </div>
    );
  }
}

export default Item;
