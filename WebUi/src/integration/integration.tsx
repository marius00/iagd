// tslint:disable-next-line
import MockItemSetData from '../mock/MockItemSetData';

declare abstract class cefSharp {
}

export const isEmbedded = typeof cefSharp === 'object';

export interface TransferResult {
  success: boolean;
  numTransferred: number;
}

interface IntegrationInterface {
  transferItem(id: object[], numItems: number): string;

  setClipboard(text: string): void;

  requestMoreItems(): void;

  getItemSetAssociations(): string;

  getFeatureSuggestion(): string;

  markFeatureSuggestionSeen(feature: string): void;
}

declare let core: IntegrationInterface;

export function transferItem(url: object[], numItems: number): TransferResult {
  const id = url.join(';');
  if (isEmbedded) {
    var response = JSON.parse(core.transferItem(url, numItems));
    return {success: response.success, numTransferred: response.numTransferred};
  } else {
    console.debug('Transfer Single', id);
    return {success: true, numTransferred: numItems};
  }
}

export function setClipboard(text: string): void {
  core.setClipboard(text);
}

export function requestMoreItems(): void {
  if (isEmbedded) {
    core.requestMoreItems();
  } else {
    console.debug('It wants itemsss doesss itssss? no more have it doessssss');
  }
}

export function getItemSetAssociations(): string {
  if (isEmbedded) {
    return core.getItemSetAssociations();
  } else {
    return JSON.stringify(MockItemSetData);
  }
}


export function openUrl(url: string) {
  if (isEmbedded) {
    document.location.href = url;
  } else {
    window.open(url, '_blank');
  }
}

var hasSeenMockFeature = false;

export function getFeatureSuggestion() {
  if (isEmbedded) {
    return core.getFeatureSuggestion();
  } else {
    const f = hasSeenMockFeature ? '' : (Math.random() < 0.5 ? 'SetBonus' : 'CollectionsTab');
    console.log('Feature:', f);
    return f;
  }
}

export function markFeatureSuggestionSeen(feature: string) {
  if (isEmbedded) {
    return core.markFeatureSuggestionSeen(feature);
  } else {
    hasSeenMockFeature = true;
  }
}
