
// tslint:disable-next-line
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
}
declare let core: IntegrationInterface;

export function transferItem(url: object[], numItems: number): TransferResult {
  const id = url.join(';');
  if (isEmbedded) {
    var response = JSON.parse(core.transferItem(url, 1));
    return {success: response.success, numTransferred: response.numTransferred};
  } else {
    console.debug('Transfer Single', id);
    return {success: true, numTransferred: numItems};
  }
}

export function setClipboard(text: string): void {
  core.setClipboard(text);
}

