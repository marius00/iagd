import IItem from '../interfaces/IItem';

export interface ApplicationState {
  clickCounter: number;
  items: IItem[];
  isLoading: boolean;
}


export interface GlobalReducerState {
  setItemReducer: ApplicationState;
  notifications: Notification[];
}
