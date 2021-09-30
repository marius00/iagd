import { IStat } from './IStat';

export interface ISkill {
  name: string;
  description: string;
  level?: number;

  petStats: IStat[];
  headerStats: IStat[];
  bodyStats: IStat[];

  trigger?: string | null;
}
