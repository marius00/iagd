import * as React from 'react';
import IItem from '../../interfaces/IItem';
import { isEmbedded } from '../../integration/integration';
import translate from '../../translations/EmbeddedTranslator';
import IItemType from '../../interfaces/IItemType';


interface Props {
    showBackupCloudIcon: boolean;
}
type IItemWithshowBackupCloudIcon = Props & IItem;

class ItemCornerContainer extends React.PureComponent<IItemWithshowBackupCloudIcon, object> {
  render() {
    const item = {...this.props};
    const showBackupCloudIcon = item.showBackupCloudIcon;
    const showCloudOkIcon = item.type === IItemType.Player && item.hasCloudBackup && isEmbedded && showBackupCloudIcon;
    const showCloudErrorIcon = item.type === IItemType.Player && !item.hasCloudBackup && isEmbedded && showBackupCloudIcon;
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
}

export default ItemCornerContainer;