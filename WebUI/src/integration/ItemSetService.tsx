import { getItemSetAssociations } from './integration';

interface ItemSetAssociation {
  baseRecord: string;
  setName: string;
}


// Both directions are indexed up front. These are read once per item per render, so a scan over the
// association list here shows up directly in the cost of drawing a page of results.
const setNameByRecord = new Map<string, string>();
const recordsBySetName = new Map<string, string[]>();
let isLoaded = false;

function load() {
  if (isLoaded) {
    return;
  }
  isLoaded = true;

  const dataset = JSON.parse(getItemSetAssociations()) as ItemSetAssociation[];
  for (const entry of dataset) {
    // First one wins, matching the previous filter(..)[0] lookup.
    if (!setNameByRecord.has(entry.baseRecord)) {
      setNameByRecord.set(entry.baseRecord, entry.setName);
    }

    const members = recordsBySetName.get(entry.setName);
    if (members) {
      members.push(entry.baseRecord);
    } else {
      recordsBySetName.set(entry.setName, [entry.baseRecord]);
    }
  }
}

// Returns the set name or undefined
export default function GetSetName(baseRecord: string): string | undefined {
  load();
  return setNameByRecord.get(baseRecord);
}

// Returns the items in a given set, or an empty list
export function GetSetItems(setName: string | undefined): string[] {
  if (setName === undefined) {
    return [];
  }

  load();
  return recordsBySetName.get(setName) ?? [];
}
