
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
    console.debug("It wants itemsss doesss itssss? no more have it doessssss");
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
    window.open(url, "_blank");
  }
}