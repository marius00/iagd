import IItem from '../interfaces/IItem';
import ICollectionItem from '../interfaces/ICollectionItem';

export interface ApplicationState {
  clickCounter: number;
  items: IItem[];
  collectionItems: ICollectionItem[];
  isLoading: boolean;
}


export interface GlobalReducerState {
  setItemReducer: ApplicationState;
  notifications: Notification[];
}
