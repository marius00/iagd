import {IReplicaRow} from "../../interfaces/IReplicaRow";

/*
 * Row types, as emitted by the game's tooltip renderer. The same visual role is numbered
 * differently depending on nesting (item / component / granted skill / set), so each of these is a
 * set of numbers rather than a single one. See ReplicaStat.css for the full map.
 */

/** An empty row, used by the game to separate sections */
export const BLANK = [0, 1];

/** "Granted Skills". 24 is also a pet header, so see isGrantedSkillHeader */
export const GRANTED_SKILL_HEADER = [24, 34, 36];

/** "Counterblow (15% Chance on Block)" */
export const SKILL_NAME = [37, 38];

/** "+3 to Hellfire" */
export const SKILL_BONUS = [18, 19, 79, 81];

/** The set name, which is the first row of an item's set info */
export const SET_NAME = [21];

/**
 * Rows that always begin a new section: requirements, set info, pet bonuses, completion bonus and
 * the ctrl hint. None of them occur inside a granted skill, so they bound a skipped skill block.
 */
export const SECTION_START = [20, 21, 22, 23, 25, 35, 65, 67, 68, 70];

/** Set info sits at the foot of an item, with only the requirements and the ctrl hint after it */
export const AFTER_SET_INFO = [20, 35];

export function isBlank(row: IReplicaRow) {
  return BLANK.includes(row.type);
}

/**
 * The game reuses the header row types for pet bonuses and pet abilities ("Bonus to All Pets",
 * "Crab Spirit Abilities:"), so a header only opens a granted skill when a skill name follows it.
 * Keyed off the row types rather than the header text, which is translated.
 */
export function isGrantedSkillHeader(rows: IReplicaRow[], idx: number) {
  if (!GRANTED_SKILL_HEADER.includes(rows[idx].type)) {
    return false;
  }

  const next = rows.slice(idx + 1).find(row => !isBlank(row));
  return next !== undefined && SKILL_NAME.includes(next.type);
}

export function isSkillBooster(row: IReplicaRow) {
  if (!SKILL_BONUS.includes(row.type))
    return false;

  // +1...+5, most likely a skill. A bit naive. No idea how it'll work with russian/japanese/etc..
  for (let i = 1; i <= 5; i++) {
    if (row.text.startsWith(`+${i} `)) {
      return true;
    }
  }

  return false;
}

/**
 * Builds the "should this row be hidden" predicate for one item, to be called once per row in order.
 *
 * A granted skill runs from its header to the second blank line -- the first ends the skill
 * description, the second ends its stats. Set info runs from the set name to the requirements at
 * the foot of the item. Both also stop at any row that starts the next section, so a block laid out
 * differently than expected can't swallow the rest of the item.
 */
export function createSectionSkipper(rows: IReplicaRow[], hideGrantedSkill: boolean, hideSetBonus: boolean) {
  let skipping: 'skill' | 'set' | null = null;
  let blankLines = 0;

  return (row: IReplicaRow, idx: number): boolean => {
    if (skipping === 'skill') {
      if (SECTION_START.includes(row.type)) {
        skipping = null;
      }
      else {
        if (isBlank(row) && ++blankLines >= 2) {
          skipping = null;
        }

        return true;
      }
    }
    else if (skipping === 'set') {
      if (AFTER_SET_INFO.includes(row.type)) {
        skipping = null;
      }
      else {
        return true;
      }
    }

    if (hideGrantedSkill && isGrantedSkillHeader(rows, idx)) {
      skipping = 'skill';
      blankLines = 0;
      return true;
    }

    if (hideSetBonus && SET_NAME.includes(row.type)) {
      skipping = 'set';
      return true;
    }

    return false;
  };
}
