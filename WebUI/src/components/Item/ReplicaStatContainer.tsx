import {h} from "preact";
import {PureComponent} from "preact/compat";
import {IReplicaRow} from "../../interfaces/IReplicaRow";
import ReplicaStat from "./ReplicaStat";
import {IStat, statToString} from "../../interfaces/IStat";
import ItemStat from "./ItemStat";
import {createSectionSkipper, isBlank, isSkillBooster} from "./replicaSections";

interface Props {
  rows: IReplicaRow[];
  id: string;
  skills: IStat[];
  hideGrantedSkill: boolean;
  hideSetBonus: boolean;
}

/**
 * Renders all the replica stats for an item
 */
class ReplicaStatContainer extends PureComponent<Props, object> {
  render() {
    const {rows, id, skills} = this.props;
    if (rows === null || rows.length === 0)
      return null;

    const bodyStats = skills.map((stat) =>
      <ItemStat {...stat} key={`stat-body-${id}-${statToString(stat)}`.replace(' ', '_')}/>
    );

    const shouldSkip = createSectionSkipper(rows, this.props.hideGrantedSkill, this.props.hideSetBonus);

    let setSkillStage = 0;

    let hasShownSkills = false;
    return (
      <p className="replica">
        {rows.map((row, idx) => {
          // Skip granted skill and set information
          if (shouldSkip(row, idx)) {
            return null;
          }

          if (row.type === 80 /* Set skill, e.g. Secrets of the Guardian (50% Chance on Critical Attack) */) {
            // All rows of this skill's info have type 80, so these rows are rendered as plain black text (in dark mode)

            // With the following "state machine", "80" types are replaced with more appropriate ones:
            // setSkillStage=0 => render with type=37 // Skill name
            // setSkillStage=1 => render with type=39 // Skill description
            // setSkillStage=2 => render with type=19 // Skill stat row
            // setSkillStage=3 => render with type=19
            // setSkillStage=4 => render with type=19
            // etc...
            let replicaStat;

            if (setSkillStage === 0) {
              replicaStat = <ReplicaStat {...row} key={id + idx} type={37}/>
            } else if (setSkillStage === 1) {
              replicaStat = <ReplicaStat {...row} key={id + idx} type={39}/>
            } else if (setSkillStage > 1) {
              replicaStat = <ReplicaStat {...row} key={id + idx} type={19}/>
            }

            ++setSkillStage;

            return replicaStat;
          }

          if (isBlank(row) /* Newline */) {
            return <br/>;
          }

          if (!isSkillBooster(row)) {
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
