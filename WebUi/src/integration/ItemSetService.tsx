import { getItemSetAssociations } from './integration';

interface ItemSetAssociation {
  baseRecord: string;
  setName: string;
}


let dataset = [] as Array<ItemSetAssociation>;

// Returns the set name or undefined
export default function GetSetName(baseRecord: string): string | undefined {
  if (dataset.length === 0) {
    dataset = JSON.parse(getItemSetAssociations());
    console.log('Parsed item sets into', dataset);
  }

  const elems = dataset.filter(elem => elem.baseRecord === baseRecord);
  if (elems.length > 0) {
    return elems[0].setName;
  }

  return undefined;
}