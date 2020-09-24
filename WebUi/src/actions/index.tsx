import {
  ACTION_SET_ITEMS,
  ACTION_ADD_ITEMS,
  SET_MOCK_DATA,
  SET_LOADING_STATUS,
  REQUEST_INITIAL_ITEMS, SET_MOCK_COLLECTION_DATA, ACTION_SET_COLLECTIONITEMS
} from '../constants';

import IItem from '../interfaces/IItem';
import { NotificationLevel, show } from 'react-notification-system-redux';
import { Action } from 'redux';
import ICollectionItem from '../interfaces/ICollectionItem';

export interface SetItems {
  type: ACTION_SET_ITEMS;
  items: IItem[];
}

export interface SetCollectionItems {
  type: ACTION_SET_COLLECTIONITEMS;
  items: ICollectionItem[];
}

export interface AddItems {
  type: ACTION_ADD_ITEMS;
  items: IItem[];
}

export interface SetMockItems {
  type: SET_MOCK_DATA;
  items: string;
}

export interface SetMockCollectionItems {
  type: SET_MOCK_COLLECTION_DATA;
  items: string;
}

export interface SetLoadingStatus {
  type: SET_LOADING_STATUS;
  status: boolean;
}

export interface RequestInitialItems {
  type: REQUEST_INITIAL_ITEMS;
}

export type AnyAction = SetItems | AddItems | SetMockItems | SetLoadingStatus | RequestInitialItems | SetMockCollectionItems | SetCollectionItems;

export function setItems(items: IItem[]): SetItems {
  console.log('Dispatching a request for setting items');
  console.log(`setting ${items.length} items`);
  return {
    type: ACTION_SET_ITEMS,
    items: items
  };
}

export function setCollectionItems(items: ICollectionItem[]): SetCollectionItems {
  return {
    type: ACTION_SET_COLLECTIONITEMS,
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

export function setMockCollectionItems(items: string): SetMockCollectionItems {
  console.log('Dispatching a request for setting mock collection items');
  return {
    type: SET_MOCK_COLLECTION_DATA,
    items: items
  };
}

export function setLoadingStatus(status: boolean): SetLoadingStatus {
  console.log('Dispatching a request to set the loading status to', status);
  return {
    type: SET_LOADING_STATUS,
    status: status
  };
}

export function requestInitialItems(): RequestInitialItems {
  console.log('Dispatching a request to load initial items');
  return {
    type: REQUEST_INITIAL_ITEMS
  };
}

export function showMessage(message: string, level: NotificationLevel, url: string | undefined): Action {
  const action = url !== undefined ? {
    label: 'Click here for more info',
    callback: function () {
      window.open(url);
    }
  } : undefined;

  return show(
    {
      message: message,
      autoDismiss: 5.0,
      dismissible: false,
      position: 'bc',
      action: action
    },
    level
  );
}