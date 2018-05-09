import { IStat } from './IStat';
import { ISkill } from './ISkill';
import IItemType from './IItemType';

export default interface IItem {
  baseRecord: string;
  icon: string;
  quality: string;
  name: string;
  socket: string;
  level: number;
  url: Array<number | string>;
  numItems: number;
  type: IItemType;
  buddies: string[];
  hasRecipe: boolean;
  greenRarity: number;
  headerStats: IStat[];
  bodyStats: IStat[];
  petStats: IStat[];
  skill: ISkill | null;
  hasCloudBackup?: boolean; // TODO: Should be mandatory, optional due to test data not including it
  slot?: string; // TODO: See above
  extras?: string | undefined;
}

