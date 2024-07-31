import { isEmbedded } from '../integration/integration';

interface IntegrationInterface {
  getTranslationStrings(): { [index: string]: string };
}
declare let core: IntegrationInterface;


// Applies translations provided by the parent application
class EmbeddedTranslator {
  static defaults: { [index: string]: string } = {
    'app.tab.items': 'Items',
    'app.tab.collections': 'Collections',
    'app.tab.crafting': 'Crafting',
    'app.tab.help': 'Help',
    'app.tab.components': 'Components',
    'app.tab.discord': 'Help / Discord',
    'items.label.noItemsFound': 'No items found',
    'items.label.youCanCraftThisItem': 'You can craft this item',
    'items.label.cloudOk': 'This item has been backed up to the cloud',
    'items.label.cloudError': 'This item has not been backed up to the cloud',
    'items.label.cloudOk.hw': 'This item has been flown up to the clouds',
    'items.label.cloudError.hw': 'This item has not been flown up to the clouds',
    'items.label.cloudOk.easter': 'The easter bunny has safely stored this item away',
    'items.label.cloudError.easter': 'The easter bunny has not yet found this egg',
    'item.buddies.singular': '«{0}» also has this item.',
    'item.buddies.singularOnly': '«{0}» has this item.',
    'item.buddies.plural': 'Several of your Buddies have this item',
    'item.buddies.watermark': 'Buddyitem',
    'item.buddies.tooltip': 'One of your buddies has this item',
    'item.genericstats.warning': 'You are seeing the generic stats for this item.\nSee the help tab for more information',
    'items.label.singleRare': 'This is a MI with a green affix',
    'items.label.doubleRare': 'This is a MI with two green affixes!',
    'item.label.bonusToAllPets': 'Bonus to All Pets:',
    'item.label.grantsSkill': 'Grants Skill: ',
    'item.label.grantsSkillLevel': 'Level {0}',
    'item.label.levelRequirement': 'Level Requirement: {0}',
    'item.label.levelRequirementAny': 'Any',
    'item.label.transferSingle': 'Transfer',
    'item.label.transferCompareSingle': 'Compare & Transfer',
    'item.label.transferAll': 'Transfer All',
    'crafting.header.recipeName': 'Crafting recipe for {0}',
    'crafting.header.currentlyLacking': 'You are currently lacking:',
    'item.augmentPurchasable': 'You may be able to purchase this augment from {0}',
    'app.copyToClipboard': 'Copy to clipboard',
    'item.label.setbonus': 'Set:',
    'item.label.noMoreItems': 'No more items',
    'item.label.setConsistsOf': 'This set consists of the following items: ',
    'button.loadmoreitems': 'Load more items',
    'items.displaying': 'Displaying {0}',
    'collections.filter.owned': 'Owned only',
    'collections.filter.missing': 'Missing only',
    'collections.filter.purple': 'Purple only',
    'collections.h2': 'Experimental feature',
    'collections.ingress1': 'If you like or dislike this feature, complain on discord or the forum.',
    'collections.ingress2': 'This feature will get deprecated or improved pending user feedback.',
    'notification.clearall': 'Clear all',
    'app.error.grimnotparsed': 'Getting started\n\nThe first step in using Item Assistant is parsing the Grim Dawn database..\n\nGo to the "Grim Dawn" tab and parse the game.\nIf the game path is not already detected, simply start Grim Dawn and it should get found automatically.',
  };


  static translation = {} as { [index: string]: string };

  public translate(id: string): string {
    // Fetch data from host
    if (Object.getOwnPropertyNames(EmbeddedTranslator.translation).length === 0 && typeof core !== 'undefined') {
      console.debug("Fetching translation strings");
      const d = core.getTranslationStrings();
      EmbeddedTranslator.translation = typeof d === 'string' ? JSON.parse(d) : d;
    }

    if (isEmbedded && EmbeddedTranslator.translation.hasOwnProperty(id) && EmbeddedTranslator.translation[id] !== '') {
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
