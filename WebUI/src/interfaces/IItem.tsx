import { IStat } from './IStat';
import { ISkill } from './ISkill';
import IItemType from './IItemType';
import {IReplicaRow} from "./IReplicaRow";

export default interface IItem {
  uniqueIdentifier: string;
  mergeIdentifier: string;
  baseRecord: string;
  icon: string;
  quality: string;
  name: string;
  socket: string;
  level: number;
  url: Array<number | string>;
  type: IItemType;
  hasRecipe: boolean;
  greenRarity: number;
  headerStats: IStat[];
  bodyStats: IStat[];
  petStats: IStat[];
  skill?: ISkill | null;
  hasCloudBackup?: boolean;
  extras?: string | undefined;
  isMonsterInfrequent?: boolean;
  isHardcore: boolean;
  replicaStats: IReplicaRow[];
}

