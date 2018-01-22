import * as React from 'react';
import ItemStat from './ItemStat';
import { ISkill } from '../interfaces/ISkill';

interface Props {
  skill: ISkill;
  keyPrefix: string;
}
class Skill extends React.Component<Props, object> {

  render() {
    const skill = this.props.skill;
    const prefix = this.props.keyPrefix;

    const headerStats = skill.headerStats.map((stat) =>
      <ItemStat label={stat.label} extras={stat.extras} key={'skill-stat-head-' + prefix + stat.label} />
    );

    const bodyStats = skill.bodyStats.map((stat) =>
      <ItemStat label={stat.label} extras={stat.extras} key={'skill-stat-body-' + prefix + stat.label} />
    );

    const petStats = skill.petStats.map((stat) =>
      <ItemStat label={stat.label} extras={stat.extras} key={'skill-stat-pets-' + prefix + stat.label} />
    );

    return (
      <div className="skill">
        <span className="skill-header">Grants Skill:</span> {skill.name} {skill.level && skill.level > 0 ? `(Level ${skill.level})` : ''}
        <div>
          <ul>
            {headerStats}
          </ul>

          <ul>
            {bodyStats}
          </ul>

          {petStats.length > 0 ? (
            <div>
              <div className="pet-header">Bonus to All Pets:</div>
              {petStats}
            </div>
          ) : ''
          }

          <span className="skill-trigger">{skill.trigger}</span>
        </div>
      </div>
    );
  }
}

export default Skill;
