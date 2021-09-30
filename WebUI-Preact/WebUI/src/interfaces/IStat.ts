
export interface IStat {
  text: string;
  param0: string;
  param1: string;
  param2: string;
  param3: string;
  param4: string;
  param5: string;
  param6: string;
  extras?: string;
}


export function statToString(stat: IStat) {
  return stat.text
    .replace("{0}", stat.param0)
    .replace("{1}", stat.param1)
    .replace("{2}", stat.param2)
    .replace("{3}", stat.param3)
    .replace("{4}", stat.param4)
    .replace("{5}", stat.param5)
    .replace("{6}", stat.param6);
}