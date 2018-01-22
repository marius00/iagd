import {
  ACTION_INCREMENT,
  ACTION_SET_ITEMS,
  ACTION_ADD_ITEMS,
  SET_MOCK_DATA,
  SET_CLASSES,
  REQUEST_CLASSES,
  SET_MODS,
  REQUEST_MODS,
  SET_SELECTED_MOD,
  SET_LOADING_STATUS
} from '../constants';

import IProfession from '../interfaces/IProfession';
import IMod from '../interfaces/IMod';
import IItem from '../interfaces/IItem';

export interface IncrementCounter {
  type: ACTION_INCREMENT;
}

export interface SetItems {
  type: ACTION_SET_ITEMS;
  items: IItem[];
}

export interface AddItems {
  type: ACTION_ADD_ITEMS;
  items: IItem[];
}

export interface SetMockItems {
  type: SET_MOCK_DATA;
  items: string;
}

export interface SetClasses {
  type: SET_CLASSES;
  classes: IProfession[];
}

export interface SetMods {
  type: SET_MODS;
  mods: IMod[];
}

export interface RequestClasses {
  type: REQUEST_CLASSES;
}

export interface RequestMods {
  type: REQUEST_MODS;
}
export interface SetSelectedMod {
  type: SET_SELECTED_MOD;
  mod: IMod;
}

export interface SetLoadingStatus {
  type: SET_LOADING_STATUS,
  status: boolean
}

export type AnyAction = IncrementCounter | SetItems | AddItems | SetMockItems | SetClasses | RequestClasses | SetMods | RequestMods | SetSelectedMod | SetLoadingStatus;

export function setItems(items: IItem[]): SetItems {
  console.log('Dispatching a request for setting items');
  console.log(`setting ${items.length} items`);
  return {
    type: ACTION_SET_ITEMS,
    items: items
  };
}

export function addItems(items: IItem[]): AddItems {
  console.log('Dispatching a request for adding items');
  return {
    type: ACTION_ADD_ITEMS,
    items: items
  };
}

export function setMockItems(items: string): SetMockItems {
  console.log('Dispatching a request for setting mock items');
  return {
    type: SET_MOCK_DATA,
    items: items
  };
}

export function setClasses(classes: IProfession[]): SetClasses {
  console.log('Dispatching a request for setting classes');
  return {
    type: SET_CLASSES,
    classes: classes
  };
}

export function setMods(mods: IMod[]): SetMods {
  console.log('Dispatching a request for setting mods');
  return {
    type: SET_MODS,
    mods: mods
  };
}

export function requestClasses(): RequestClasses {
  console.log('Dispatching a request for class updates');
  return {
    type: REQUEST_CLASSES
  };
}

export function requestMods(): RequestMods {
  console.log('Dispatching a request for mod updates');
  return {
    type: REQUEST_MODS
  };
}

export function setSelectedMod(mod: IMod): SetSelectedMod {
  console.log('Dispatching a request to set the selected mod');
  return {
    type: SET_SELECTED_MOD,
    mod: mod
  };
}

export function setLoadingStatus(status: boolean): SetLoadingStatus {
  console.log('Dispatching a request to set the loading status to', status);
  return {
    type: SET_LOADING_STATUS,
    status: status
  };
}

