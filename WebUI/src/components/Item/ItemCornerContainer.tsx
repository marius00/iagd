import {h} from "preact";
import IItem from '../../interfaces/IItem';
import { isEmbedded } from '../../integration/integration';
import translate from '../../translations/EmbeddedTranslator';
import IItemType from '../../interfaces/IItemType';
import {PureComponent} from "preact/compat";


interface Props {
  showBackupCloudIcon: boolean;
  onClickBuddyIcon: () => void;
  items: IItem[];
}


function getEaster(Y: number) {
  let C = Math.floor(Y/100);
  let N = Y - 19*Math.floor(Y/19);
  let K = Math.floor((C - 17)/25);
  let I = C - Math.floor(C/4) - Math.floor((C - K)/3) + 19*N + 15;
  I = I - 30*Math.floor((I/30));
  I = I - Math.floor(I/28)*(1 - Math.floor(I/28)*Math.floor(29/(I + 1))*Math.floor((21 - N)/11));
  let J = Y + Math.floor(Y/4) + I + 2 - C + Math.floor(C/4);
  J = J - 7*Math.floor(J/7);
  let L = I - J;
  let M = 3 + Math.floor((L + 40)/44);
  let D = L + 28 - 31*Math.floor(M/4);

  return new Date(Y, M - 1, D);
}

function isItEaster(): boolean {
  const easterSunday = getEaster(new Date().getFullYear());
  const startOfEaster = new Date(easterSunday.getFullYear(), easterSunday.getMonth(), easterSunday.getDate() - 7);
  return new Date() >= startOfEaster && new Date <= easterSunday;
}

function getCloudIcon(isOk: boolean) {
  const isHalloween = new Date().getMonth() == 9 && new Date().getDate() >= 24;
  const isEaster = isItEaster();

  let suffix = '';
  if (isHalloween) {
    suffix = '-hw';
    console.log("Switching to halloween cloud icons");
  } else if (isEaster) {
    suffix = '-easter';
      console.log("Switching to easter cloud icons");
  }

  return isOk ? `static/cloud-ok${suffix}.png` : `static/cloud-err${suffix}.png`;
}

function getCloudLabel(isOk: boolean) {
  const isHalloween = new Date().getMonth() == 9 && new Date().getDate() >= 24;
  const isEaster = isItEaster();

  let suffix = '';
  if (isHalloween) {
    suffix = '.hw';
  } else if (isEaster) {
    suffix = '.easter';
  }

  return isOk ? `items.label.cloudOk${suffix}` : `items.label.cloudError${suffix}`;
}

class ItemCornerContainer extends PureComponent<Props, object> {
  render() {
    const buddyItems = this.props.items.filter(m => m.type === IItemType.Buddy);

    // Amazingly all of the old logic works pretty well, since buddy items are always LAST in the list, if items[0] is type buddy, then we only have buddy items.
    const item = this.props.items[0];
    const showBackupCloudIcon = this.props.showBackupCloudIcon;
    const showCloudOkIcon = item.type === IItemType.Player && item.hasCloudBackup && isEmbedded && showBackupCloudIcon;
    const showCloudErrorIcon = item.type === IItemType.Player && !item.hasCloudBackup && isEmbedded && showBackupCloudIcon;
    const showSingularBuddyItemIcon = item.type !== IItemType.Buddy && buddyItems.length === 1;
    const showPluralBuddyItemIcon = item.type !== IItemType.Buddy && buddyItems.length > 1;
    const showRecipeIcon = item.hasRecipe && item.type !== 0;
    const showAugmentationIcon = item.type === IItemType.Augmentation;
    const onClickBuddyIcon = this.props.onClickBuddyIcon;

    const cloudIconOk = getCloudIcon(true);
    const cloudIconErr = getCloudIcon(false);
    const cloudLabelOk = getCloudLabel(true);
    const cloudLabelError = getCloudLabel(false);

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
          src={cloudIconOk}
          data-tip={translate(cloudLabelOk)}
          alt={"Synced to the cloud"}
        />
        }

        {showCloudErrorIcon &&
        <img
          className="cursor-help"
          src={cloudIconErr}
          data-tip={translate(cloudLabelError)}
          alt={"Not synced to the cloud"}
        />
        }

        {showSingularBuddyItemIcon &&
        <span onClick={() => onClickBuddyIcon()}>
            <img
              className="cursor-help"
              src="static/buddy.png"
              data-tip={translate('item.buddies.singular', buddyItems[0].extras)}
              alt={translate('item.buddies.tooltip')}
            />

          </span>
        }

        {showPluralBuddyItemIcon &&
        <span>
            <img
              className="cursor-help"
              src="static/buddy.png"
              data-tip={translate('item.buddies.plural')}
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
