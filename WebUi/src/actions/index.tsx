import {
  ACTION_SET_ITEMS,
  ACTION_ADD_ITEMS,
  SET_MOCK_DATA,
  SET_LOADING_STATUS,
  REQUEST_INITIAL_ITEMS
} from '../constants';

import IItem from '../interfaces/IItem';
import { NotificationLevel, show } from 'react-notification-system-redux';
import { Action } from 'redux';

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

export interface SetLoadingStatus {
  type: SET_LOADING_STATUS;
  status: boolean;
}

export interface RequestInitialItems {
  type: REQUEST_INITIAL_ITEMS;
}

export type AnyAction = SetItems | AddItems | SetMockItems | SetLoadingStatus | RequestInitialItems;

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

export function showMessage(message: string, level: NotificationLevel): Action {
  return show(
    {
      message: message,
      autoDismiss: 3.5,
      dismissible: false,
      position: 'bc'
    },
    level
  );
}