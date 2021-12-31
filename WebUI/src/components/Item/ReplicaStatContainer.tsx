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
      if (isSkipping && row.type === 0 /* Newline */)
        numWhitespaces++;

      if (isSkipping && numWhitespaces < 2) // Second whitespace generally ends the skill description.
        return true;

      if (this.props.hideGrantedSkill && (row.type === 34) /* Start of skill */) {
        isSkipping = true;
        return true;
      }

      return false;
    }

    let hasShownSkills = false;
    return (
      <p className="replica">
        {rows.map((row, idx) => {
          // Skip skill information
          if (shouldSkip(row)) {
            return null;
          }

          if (!this.isSkillBooster(row)) {
            return <ReplicaStat {...row} key={id + idx}/>
          }
          // We have our own skill descriptions, superior to that of the replica rows.
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
