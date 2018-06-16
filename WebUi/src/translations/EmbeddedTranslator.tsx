import { isEmbedded } from '../constants';

/* tslint:disable */
declare abstract class data {
  public static translation: {[id: string] : string};
}

class EmbeddedTranslator {
  static tagTranslation = {
    'app.tab.items': 'iatag_html_tab_header_items',
    'app.tab.crafting': 'iatag_html_tab_header_crafting',
    'app.tab.components': 'iatag_html_tab_header_components',
    'app.tab.discord': 'iatag_html_tab_header_discord',
    'items.label.noItemsFound': 'iatag_html_items_no_items',
    'items.label.youCanCraftThisItem': 'iatag_html_items_youcancraftthisitem',
    'items.label.cloudOk': 'iatag_html_cloud_ok',
    'items.label.cloudError': 'iatag_html_cloud_ok',
    'item.buddies.singular': 'iatag_html_items_buddy_alsohasthisitem1',
    'item.buddies.plural': 'iatag_html_items_buddy_alsohasthisitem3',
    'item.buddies.singularOnly': 'iatag_html_items_buddy_alsohasthisitem4',
    'items.label.doubleGreen': 'iatag_html_items_affix2',
    'items.label.tripleGreen': 'iatag_html_items_affix3',
    'item.label.bonusToAllPets': 'iatag_html_bonustopets',
    'item.label.grantsSkill': 'iatag_html_items_grantsskill',
    'item.label.grantsSkillLevel': 'iatag_html_items_level',
    'item.label.levelRequirement': 'iatag_html_levlerequirement',
    'item.label.levelRequirementAny': 'iatag_html_any',
    'item.label.transferSingle': 'iatag_html_transfer',
    'item.label.transferAll': 'iatag_html_transferall',
    'crafting.header.recipeName': 'iatag_html_badstate_title', // TODO:
    'crafting.header.currentlyLacking': 'iatag_html_crafting_lacking',
    'item.augmentPurchasable': 'iatag_html_augmentation_item',
    'app.copyToClipboard': 'iatag_html_copytoclipboard'
  };
  static defaults = {
    'app.tab.items': 'Items',
    'app.tab.crafting': 'Crafting',
    'app.tab.components': 'Components',
    'app.tab.discord': 'Help / Discord',
    'items.label.noItemsFound': 'No items found',
    'items.label.youCanCraftThisItem': 'You can craft this item',
    'items.label.cloudOk': 'This item has been backed up to the cloud',
    'items.label.cloudError': 'This item has not been backed up to the cloud',
    'item.buddies.singular': '«{0}» also has this item.',
    'item.buddies.singularOnly': '«{0}» has this item.',
    'item.buddies.plural': 'These buddies also have this item: {0}',
    'items.label.doubleGreen': 'This item has two green affixes',
    'items.label.tripleGreen': 'This item has three green affixes! (Rare!)',
    'item.label.bonusToAllPets': 'Bonus to All Pets:',
    'item.label.grantsSkill': 'Grants Skill: ',
    'item.label.grantsSkillLevel': 'Level {0}',
    'item.label.levelRequirement': 'Level Requirement: {0}',
    'item.label.levelRequirementAny': 'Any',
    'item.label.transferSingle': 'Transfer',
    'item.label.transferAll': 'Transfer All',
    'crafting.header.recipeName': 'Crafting recipe for {0}',
    'crafting.header.currentlyLacking': 'You are currently lacking:',
    'item.augmentPurchasable': 'You may be able to purchase this augment from {0}',
    'app.copyToClipboard': 'Copy to clipboard'
  };

  public translate(id: string): string {
    let iaTag = EmbeddedTranslator.tagTranslation[id];
    if (isEmbedded && data.translation.hasOwnProperty(iaTag)) {
      if (data.translation[iaTag]) {
        return data.translation[iaTag];
      }
    } else if (EmbeddedTranslator.defaults.hasOwnProperty(id)){
      return EmbeddedTranslator.defaults[id];
    }

    return id;
  }
}
const t = new EmbeddedTranslator();
function translate(id: string, arg1?: string, arg2?: string, arg3?: string): string {
  let translation = t.translate(id);

  if (arg1) {
    if (translation.indexOf('{0}') === -1) {
      console.warn(`Could not find {0} in tag ${id}`);
    }
    translation = translation.replace('{0}', arg1);
  }
  if (arg2) {
    if (translation.indexOf('{1}') === -1) {
      console.warn(`Could not find {1} in tag ${id}`);
    }
    translation = translation.replace('{1}', arg2);
  }
  if (arg3) {
    if (translation.indexOf('{2}') === -1) {
      console.warn(`Could not find {2} in tag ${id}`);
    }
    translation = translation.replace('{2}', arg3);
  }

  return translation;
}
export default translate;