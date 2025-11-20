// tslint:disable-next-line
import MockItemSetData from '../mock/MockItemSetData';

declare abstract class cefSharp {
}


declare abstract class chrome {
    static webview: any;
}

export const isEmbedded = typeof chrome !== "undefined" && typeof chrome.webview.hostObjects.sync.core === 'function';

export interface TransferResult {
    success: boolean;
}

interface IntegrationInterface {
    TransferItem(id: object[], transferAll: boolean): string;

    SetClipboard(text: string): void;

    RequestMoreItems(): void;

    GetItemSetAssociations(): string;

    GetBackedUpCharacters(): string;
    GetCharacterDownloadUrl(character: string): string;
    OpenURL(url: string): void;
    SignalReady(): void;

    GetTranslationStrings(): { [index: string]: string };
}


let core = isEmbedded ? chrome.webview.hostObjects.sync.core as IntegrationInterface : {} as IntegrationInterface;

export function transferItem(url: object[], transferAll: boolean): TransferResult {
  const id = url.join(';');
  if (isEmbedded) {
    const response = JSON.parse(core.TransferItem(url, transferAll));
    return {success: response.success};
  } else {
    console.debug('Transfer Single', id);
    return {success: true};
  }
}

export function setClipboard(text: string): void {
  console.debug("Setting clipboard text");
  core.SetClipboard(text);
}

export function requestMoreItems(): void {
  if (isEmbedded) {
    console.debug("Requesting more items");
    core.RequestMoreItems();
  } else {
    console.debug('It wants itemsss doesss itssss? no more have it doessssss');
  }
}


let itemSetAssociationsCache = '';
export function getItemSetAssociations(): string {
  if (isEmbedded) {
    if (itemSetAssociationsCache !== '')
      return itemSetAssociationsCache;

    console.debug("Requesting item set associations");
    itemSetAssociationsCache = core.GetItemSetAssociations();
    return itemSetAssociationsCache;
  } else {
    return JSON.stringify(MockItemSetData);
  }
}


export function openUrl(url: string): void {
    core.OpenURL(url)
}

export function signalReady(): void {
    console.log("Notifying IAGD that we're ready")
    core.SignalReady()
}

export interface CharacterListDto {
  name: string;
  createdAt: string;
  updatedAt: string;
}

export function getBackedUpCharacters(): CharacterListDto[] {
  if (isEmbedded) {
    return JSON.parse(core.GetBackedUpCharacters());
  }

  return [{"name":"_Burn","createdAt":"2021-02-14T13:46:37.332325Z","updatedAt":"2021-02-15T16:41:28.098545Z"},{"name":"_Fog","createdAt":"2021-02-14T13:46:44.096884Z","updatedAt":"2021-02-15T16:41:28.543658Z"},{"name":"_HC Joe","createdAt":"2021-02-14T13:46:49.376661Z","updatedAt":"2021-02-15T16:41:28.929276Z"},{"name":"_Mist","createdAt":"2021-02-14T13:46:50.954797Z","updatedAt":"2021-02-15T16:41:29.559928Z"},{"name":"_Oaf","createdAt":"2021-02-14T13:46:52.057302Z","updatedAt":"2021-02-15T16:41:30.102216Z"},{"name":"_Ogor","createdAt":"2021-02-14T13:46:53.818669Z","updatedAt":"2021-02-15T16:41:30.530022Z"},{"name":"_Prison","createdAt":"2021-02-14T13:46:54.475951Z","updatedAt":"2021-02-15T16:41:31.139391Z"},{"name":"_Spirit","createdAt":"2021-02-14T13:46:54.878489Z","updatedAt":"2021-02-15T16:41:31.562969Z"},{"name":"_Stick","createdAt":"2021-02-14T13:46:55.375027Z","updatedAt":"2021-02-15T16:41:31.981983Z"},{"name":"_test","createdAt":"2021-02-14T13:46:55.735678Z","updatedAt":"2021-02-15T16:41:32.335665Z"},{"name":"_The Fireman","createdAt":"2021-02-14T13:46:56.116189Z","updatedAt":"2021-02-15T16:41:32.695291Z"},{"name":"_The Houndmaster of Yir","createdAt":"2021-02-14T13:46:56.534847Z","updatedAt":"2021-02-15T16:41:33.104993Z"},{"name":"_Tool","createdAt":"2021-02-14T13:46:56.915763Z","updatedAt":"2021-02-15T16:41:33.511978Z"},{"name":"_Worf","createdAt":"2021-02-14T13:46:57.458046Z","updatedAt":"2021-02-15T16:41:34.880836Z"},{"name":"_Xzipnkiron","createdAt":"2021-02-14T13:46:57.89902Z","updatedAt":"2021-02-15T16:41:36.204445Z"},{"name":"__Fog","createdAt":"2021-02-14T13:46:58.274755Z","updatedAt":"2021-02-15T16:41:36.616057Z"},{"name":"__HC Joe","createdAt":"2021-02-14T13:46:58.574766Z","updatedAt":"2021-02-15T16:41:37.059381Z"},{"name":"__test","createdAt":"2021-02-14T13:46:58.913527Z","updatedAt":"2021-02-15T16:41:37.449426Z"},{"name":"__Worf","createdAt":"2021-02-14T13:46:59.458262Z","updatedAt":"2021-02-15T16:41:37.903103Z"},{"name":"Joe","createdAt":"2021-02-15T15:50:29.190968Z","updatedAt":"2021-02-15T20:12:01.177349Z"}];
}


export interface CharacterUrlRequest {
  url: string|undefined;
}


export function getCharacterDownloadUrl(character: string): CharacterUrlRequest {
  if (isEmbedded) {
    return JSON.parse(core.GetCharacterDownloadUrl(character));
  }
  return {'url': undefined};
}

export function getTranslationStrings(): ReturnType<IntegrationInterface['GetTranslationStrings']> {
  if (isEmbedded) {
    const d = core.GetTranslationStrings();
    return typeof d === 'string' ? JSON.parse(d) : d;
  }
  return {};
}
