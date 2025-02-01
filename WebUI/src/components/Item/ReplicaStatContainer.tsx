import {h} from "preact";
import {PureComponent} from "preact/compat";
import {IReplicaRow} from "../../interfaces/IReplicaRow";
import ReplicaStat from "./ReplicaStat";
import {IStat, statToString} from "../../interfaces/IStat";
import ItemStat from "./ItemStat";

interface Props {
  rows: IReplicaRow[];
  id: string;
  skills: IStat[];
  hideGrantedSkill: boolean;
  hideSetBonus: boolean; /* 20-26 */
}

/**
 * Renders all the replica stats for an item
 */
class ReplicaStatContainer extends PureComponent<Props, object> {
  isSkillBooster(row: IReplicaRow) {
    if (row.type !== 18 && row.type !== 79)
      return false;

    // +1...+5, most likely a skill. A bit naive. No idea how it'll work with russian/japanese/etc..
    for (let i = 1; i <= 5; i++) {
      if (row.text.startsWith(`+${i} `)) {
        return true;
      }
    }

    return false;
  }

  render() {
    const {rows, id, skills} = this.props;
    if (rows === null || rows.length === 0)
      return null;

    const bodyStats = skills.map((stat) =>
      <ItemStat {...stat} key={`stat-body-${id}-${statToString(stat)}`.replace(' ', '_')}/>
    );

    // "Hack" to skip all the rows for a skill
    let numWhitespaces = 0;
    let isSkipping = false;
    const shouldSkip = (row: IReplicaRow) => {
      // Set bonus -- Maybe replace "20" with "Set bonus hidden" or similar?
      if (this.props.hideSetBonus && (row.type >= 20 && row.type <= 26)) {
        return true;
      }

      if (isSkipping && numWhitespaces < 2) // Second whitespace generally ends the skill description.
        return true;

      if (this.props.hideGrantedSkill && (row.type === 34) /* Start of skill */) {
        isSkipping = true;
        return true;
      }

      return false;
    }

    let setSkillStage = 0;

    let hasShownSkills = false;
    return (
      <p className="replica">
        {rows.map((row, idx) => {
          // Skip skill information
          if (shouldSkip(row)) {
            return null;
          }

          if (row.type === 80 /* Set skill, e.g. Secrets of the Guardian (50% Chance on Critical Attack) */) {
            // All rows of this skill's info have type 80, so these rows are rendered as plain black text (in dark mode)

            // With the following "state machine", "80" types are replaced with more appropriate ones:
            // setSkillStage=0 => render with type=23 // Skill name
            // setSkillStage=1 => render with type=21 // Skill description
            // setSkillStage=2 => render with type=40 // Skill stat row
            // setSkillStage=3 => render with type=40
            // setSkillStage=4 => render with type=40
            // etc...
            let replicaStat;

            if (setSkillStage === 0) {
              replicaStat = <ReplicaStat {...row} key={id + idx} type={23}/>
            } else if (setSkillStage === 1) {
              replicaStat = <ReplicaStat {...row} key={id + idx} type={21}/>
            } else if (setSkillStage > 1) {
              replicaStat = <ReplicaStat {...row} key={id + idx} type={40}/>
            }

            ++setSkillStage;

            return replicaStat;
          }

          if (row.type === 0 /* Newline */) {
            return <br/>;
          }

          if (!this.isSkillBooster(row)) {
            return <ReplicaStat {...row} key={id + idx}/>
          }
          // "+1 to all skills in Oathkeeper" will not be included in the "skills" array, so just render it normally.
          // Setting render type to 18 to skip underline and cursor
          else if (!skills.some(skill => row.text.includes(skill.param3))) {
            return <ReplicaStat text={row.text} type={18} key={id + idx}/>
          }
          // Render "+N to SomeSkill", We have our own skill descriptions, superior to that of the replica rows.
          else if (!hasShownSkills) {
            hasShownSkills = true;
            return bodyStats;
          }
        })}
      </p>
    );
  }
}

export default ReplicaStatContainer;
