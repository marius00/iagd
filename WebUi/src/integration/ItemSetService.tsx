import { getItemSetAssociations } from './integration';

interface ItemSetAssociation {
  baseRecord: string;
  setName: string;
}


let reverseLookup: { [index: string]: string[] } = {};

// Returns the set name or undefined
export default function GetSetName(baseRecord: string): string | undefined {
  let dataset = JSON.parse(getItemSetAssociations()) as Array<ItemSetAssociation>;

  for (let idx in dataset) {
    const entry = dataset[idx];
    if (reverseLookup.hasOwnProperty(entry.setName)) {
      reverseLookup[entry.setName] = reverseLookup[entry.setName].concat(entry.baseRecord);
    } else {
      reverseLookup[entry.setName] = [entry.baseRecord];
    }
  }

  const elems = dataset.filter(elem => elem.baseRecord === baseRecord);
  if (elems.length > 0) {
    return elems[0].setName;
  }

  return undefined;
}

// Returns the items in a given set or undefined
export function GetSetItems(setName: string | undefined): string[] {
  if (setName !== undefined) {
    return reverseLookup[setName];
  }

  return [];
}