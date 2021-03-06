import { IStat } from './IStat';
import { ISkill } from './ISkill';
import IItemType from './IItemType';

export default interface IItem {
  uniqueIdentifier: string;
  baseRecord: string;
  icon: string;
  quality: string;
  name: string;
  socket: string;
  level: number;
  url: Array<number | string>;
  numItems: number;
  initialNumItems: number;
  type: IItemType;
  buddies: string[];
  hasRecipe: boolean;
  greenRarity: number;
  headerStats: IStat[];
  bodyStats: IStat[];
  petStats: IStat[];
  skill?: ISkill | null;
  hasCloudBackup?: boolean;
  slot?: string;
  extras?: string | undefined;
  isMonsterInfrequent?: boolean;
}

