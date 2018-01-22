
export const ACTION_INCREMENT = 'ACTION_INCREMENT';
export type ACTION_INCREMENT = typeof ACTION_INCREMENT;

export const ACTION_ADD_ITEMS = 'ACTION_ADD_ITEMS';
export type ACTION_ADD_ITEMS = typeof ACTION_ADD_ITEMS;

export const ACTION_SET_ITEMS = 'ACTION_SET_ITEMS';
export type ACTION_SET_ITEMS = typeof ACTION_SET_ITEMS;

export const SET_MOCK_DATA = 'SET_MOCK_DATA';
export type SET_MOCK_DATA = typeof SET_MOCK_DATA;

export const SET_CLASSES = 'SET_CLASSES';
export type SET_CLASSES = typeof SET_CLASSES;

export const REQUEST_CLASSES = 'REQUEST_CLASSES';
export type REQUEST_CLASSES = typeof REQUEST_CLASSES;

export const SET_MODS = 'SET_MODS';
export type SET_MODS = typeof SET_MODS;

export const REQUEST_MODS = 'REQUEST_MODS';
export type REQUEST_MODS = typeof REQUEST_MODS;

export const SET_SELECTED_MOD = 'SET_SELECTED_MOD';
export type SET_SELECTED_MOD = typeof SET_SELECTED_MOD;

export const SET_LOADING_STATUS = 'SET_LOADING_STATUS';
export type SET_LOADING_STATUS = typeof SET_LOADING_STATUS;

declare abstract class cefsharp_CreatePromise {
}

export const isEmbedded = typeof cefsharp_CreatePromise === 'function';
