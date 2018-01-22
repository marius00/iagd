import IItem from '../interfaces/IItem';
import IProfession from '../interfaces/IProfession';
import IMod from '../interfaces/IMod';

export interface GlobalState {
    clickCounter: number;
    items: IItem[];
    isLoading: boolean;
    classes: IProfession[];
    mods: IMod[];
    selectedMod: IMod;
}
