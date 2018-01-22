
export default interface IMod {
  label: string;
  mod: string;
  path: string;
  isEnabled: boolean;
  isSelected?: boolean;
  isHardcore: boolean;
}

export const getDefaultSelection = (mods: IMod[]) => {
  return mods.filter((m) => m.isSelected)[0]
    || mods.filter((m) => m.mod === '' && !m.isHardcore)[0]
    || mods.filter((m) => m.mod === '' && m.isHardcore)[0]
    || (mods.length > 0 ? mods[0] : undefined);
};
