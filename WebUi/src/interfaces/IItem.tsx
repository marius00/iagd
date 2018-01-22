import { IStat } from './IStat';
import { ISkill } from './ISkill';

export default interface IItem {
  baseRecord: string;
  icon: string;
  quality: string;
  name: string;
  socket: string;
  level: number;
  url: Array<number | string>;
  numItems: number;
  type: number;
  buddies: string[];
  hasRecipe: boolean;
  greenRarity: number;
  headerStats: IStat[];
  bodyStats: IStat[];
  petStats: IStat[];
  skill: ISkill | null;
}

