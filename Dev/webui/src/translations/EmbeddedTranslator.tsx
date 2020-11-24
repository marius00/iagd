import { isEmbedded } from '../integration/integration';

interface IntegrationInterface {
  getTranslationStrings(): { [index: string]: string };
}
declare let core: IntegrationInterface;


// Applies translations provided by the parent application
class EmbeddedTranslator {
  static defaults: { [index: string]: string } = {
    'app.tab.items': 'Items',
    'app.tab.crafting': 'Crafting',
    'app.tab.components': 'Components',
    'app.tab.videoGuide': 'Video Guide',
    'app.tab.videoGuideUrl': 'https://www.twitch.tv/videos/210592694',
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
    'app.copyToClipboard': 'Copy to clipboard',
    'item.label.setbonus': 'Set:',
    'item.label.noMoreItems': 'No more items'
  };


  static translation = {} as { [index: string]: string };

  public translate(id: string): string {
    // Fetch data from host
    if (Object.getOwnPropertyNames(EmbeddedTranslator.translation).length === 0 && typeof core !== 'undefined') {
      const d = core.getTranslationStrings();
      EmbeddedTranslator.translation = typeof d === 'string' ? JSON.parse(d) : d;
    }

    if (isEmbedded && EmbeddedTranslator.translation.hasOwnProperty(id)) {
      return EmbeddedTranslator.translation[id];
    } else if (EmbeddedTranslator.defaults.hasOwnProperty(id)) {
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