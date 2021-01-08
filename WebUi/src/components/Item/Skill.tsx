import * as React from 'react';
import ItemStat from './ItemStat';
import { ISkill } from '../../interfaces/ISkill';
import translate from '../../translations/EmbeddedTranslator';
import { statToString } from '../../interfaces/IStat';

interface Props {
  skill: ISkill;
  keyPrefix: string;
}

class Skill extends React.PureComponent<Props, object> {

  render() {
    const skill = this.props.skill;
    const prefix = this.props.keyPrefix;

    const headerStats = skill.headerStats.map((stat) =>
      <ItemStat {...stat} key={`skill-stat-head-${prefix}-${statToString(stat)}`.replace(' ', '_')} />
    );

    const bodyStats = skill.bodyStats.map((stat) =>
      <ItemStat {...stat} key={`skill-stat-body-${prefix}-${statToString(stat)}`.replace(' ', '_')} />
    );

    const petStats = skill.petStats.map((stat) =>
      <ItemStat {...stat} key={`skill-stat-pets-${prefix}-${statToString(stat)}`.replace(' ', '_')} />
    );

    return (
      <div className="skill">
        <span className="skill-header">
          {translate('item.label.grantsSkill')}
        </span>
        <span className="skill-content">{skill.name} {skill.level && skill.level > 0 ? `(${translate('item.label.grantsSkillLevel', String(skill.level))})` : ''}</span>
        <div>
          <ul>
            {headerStats}
          </ul>

          <ul>
            {bodyStats}
          </ul>

          {petStats.length > 0 ? (
            <div>
              <div className="pet-header">{translate('item.label.bonusToAllPets')}</div>
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
