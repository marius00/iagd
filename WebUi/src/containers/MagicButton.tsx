import Button from '../components/Button';
import { AnyAction, setLoadingStatus, setMockItems } from '../actions';
import { ApplicationState } from '../types/index';
import { connect, Dispatch } from 'react-redux';
import IItem from '../interfaces/IItem';

interface MagicButtonProps {
  label: string;
}

interface MagicButtonState {
  count: number;
  items: IItem[];
  label: string;
}

interface MagicButtonDispatch {
  onClick: () => void;
}

export const addTodo3 = (text: string): IItem[] => {
  const dummyData = [
    {
      baseRecord: 'records\/items\/gearaccessories\/rings\/c008_ring.dbr',
      icon: 'c08_ring_aetherlords_signet.tex.png',
      quality: 'Blue',
      name: 'Mock data  ',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearaccessories\/rings\/c008_ring.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [
        't.o',
        'tjommi'
      ],
      hasRecipe: true,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+36% Aether Damage'
        },
        {
          label: '+22 Spirit'
        },
        {
          label: '+240 Health'
        },
        {
          label: '+1.5 Energy Regenerated per second'
        },
        {
          label: '15% Aether Resistance'
        },
        {
          label: '+1 to Albrecht\'s Aether Ray',
          extras: 'Tier 6 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Arcane Blast',
        description: 'A surge of arcane energies erupts from you, damaging all nearby enemies.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '4 Seconds Skill Recharge'
          },
          {
            label: '4.75 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '22044-34291 Aether Damage'
          },
          {
            label: '22044 Fire Damage'
          },
          {
            label: '+15% Weapon Damage'
          }
        ],
        trigger: '10% Chance on attacking'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/rings\/c037_ring.dbr',
      icon: 'c37_ring_aetherstorm_seal.tex.png',
      quality: 'Blue',
      name: 'Aetherstorm Seal',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearaccessories\/rings\/c037_ring.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: true,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+55% Lightning Damage'
        },
        {
          label: '+60% Electrocute Damage'
        },
        {
          label: '+30 Spirit'
        },
        {
          label: '+6% Casting Speed'
        },
        {
          label: '22% Chaos Resistance'
        },
        {
          label: '15% Vitality Resistance'
        },
        {
          label: '+2 to Raging Tempest',
          extras: 'Tier 5 Shaman skill'
        },
        {
          label: '+2 to Devastation',
          extras: 'Tier 9 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/waist\/c003_waist.dbr',
      icon: 'c02_waist_alchemistsbelt.tex.png',
      quality: 'Blue',
      name: 'Alchemist\'s Belt',
      socket: '',
      level: 29,
      url: [
        0,
        'records\/items\/gearaccessories\/waist\/c003_waist.dbr',
        '',
        '',
        ''
      ],
      numItems: 5,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '25 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+22% Acid Damage'
        },
        {
          label: '+40% Constitution'
        },
        {
          label: '+16 Spirit'
        },
        {
          label: '32% Acid Resistance'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Homebrewed Potion',
        description: 'Drink deep and feel all better.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '10 Second Duration'
          },
          {
            label: '30 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '+140 Health\/s'
          },
          {
            label: '+15% Energy Regenerated per second'
          },
          {
            label: '2000 Health Restored'
          },
          {
            label: '15% Health Restored'
          }
        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearshoulders\/c017_shoulder.dbr',
      icon: 'c017_shoulder.tex.png',
      quality: 'Blue',
      name: 'Alchemist\'s Mantle',
      socket: '',
      level: 58,
      url: [
        0,
        'records\/items\/gearshoulders\/c017_shoulder.dbr',
        '',
        '',
        ''
      ],
      numItems: 5,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '616 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+38% Vitality Damage'
        },
        {
          label: '+46% Poison Damage'
        },
        {
          label: '28% Vitality Resistance'
        },
        {
          label: '38% Acid Resistance'
        },
        {
          label: '+3 to Vile Eruption',
          extras: 'Tier 8 Occultist skill'
        },
        {
          label: '+2 to Merciless Repertoire',
          extras: 'Tier 7 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/guns1h\/c102_gun1h.dbr',
      icon: 'c102_gun1h.tex.png',
      quality: 'Blue',
      name: 'Aldur\'s Inquisition',
      socket: '',
      level: 25,
      url: [
        0,
        'records\/items\/gearweapons\/guns1h\/c102_gun1h.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '20% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '5 Elemental Damage'
        },
        {
          label: '25-45 Physical Damage'
        },
        {
          label: '+28% Elemental Damage'
        },
        {
          label: '+8% Attack Speed'
        },
        {
          label: '+8 Defensive Ability'
        },
        {
          label: '15% Physical Damage converted to Fire'
        },
        {
          label: '+2 to Artifact Handling',
          extras: 'Tier 8 Inquisitor skill'
        },
        {
          label: '+2 to Fire Strike',
          extras: 'Tier 1 Demolitionist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Elemental Seal',
        description: 'Creates an arcane seal on the ground imbued with destructive force.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '6 Second Duration'
          },
          {
            label: '5 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '14724 Elemental Damage'
          },
          {
            label: '3.5 Meter Radius'
          }
        ],
        trigger: '10% Chance on attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/rings\/c019a_ring_alkamos.dbr',
      icon: 'b04_ringalkamos.tex.png',
      quality: 'Blue',
      name: 'Alkamos\' Dread',
      socket: '',
      level: 30,
      url: [
        0,
        'records\/items\/gearaccessories\/rings\/c019a_ring_alkamos.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '1-6 Cold Damage'
        },
        {
          label: '+28% Cold Damage'
        },
        {
          label: '+3% Offensive Ability'
        },
        {
          label: '+4% Total Speed'
        },
        {
          label: '12% Elemental Resistance'
        },
        {
          label: '+2 to Execution',
          extras: 'Tier 9 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/melee2h\/c011_blunt2h.dbr',
      icon: 'c011_blunt2h.tex.png',
      quality: 'Blue',
      name: 'Alvarick\'s Rebuke',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearweapons\/melee2h\/c011_blunt2h.dbr',
        '',
        '',
        ''
      ],
      numItems: 7,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '2-30 Lightning Damage'
        },
        {
          label: '160-284 Physical Damage'
        },
        {
          label: '+68% Lightning Damage'
        },
        {
          label: '+25 Defensive Ability'
        },
        {
          label: '+22 Spirit'
        },
        {
          label: '45% Physical Damage converted to Lightning'
        },
        {
          label: '+1 to All Skills in Shaman'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Lightning Nova',
        description: 'Unleashes a blast of lightning energy, damaging all nearby enemies.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '1 Seconds Skill Recharge'
          },
          {
            label: '4 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '3348-28347 Lightning Damage'
          }
        ],
        trigger: '10% Chance on attacking'
      }
    },
    {
      baseRecord: 'records\/items\/gearshoulders\/c019_shoulder.dbr',
      icon: 'c019_shoulder.tex.png',
      quality: 'Blue',
      name: 'Amarastan Pauldrons',
      socket: '',
      level: 75,
      url: [
        0,
        'records\/items\/gearshoulders\/c019_shoulder.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '871 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+32% Physical Damage'
        },
        {
          label: '+22% Pierce Damage'
        },
        {
          label: '+20 Defensive Ability'
        },
        {
          label: '+8% Chance to Avoid Projectiles'
        },
        {
          label: '+66 Offensive Ability'
        },
        {
          label: '15% Bleeding Resistance'
        },
        {
          label: '+1 to Cadence',
          extras: 'Tier 1 Soldier skill'
        },
        {
          label: '+2 to Lethal Assault',
          extras: 'Tier 4 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/blunt1h\/c002_blunt.dbr',
      icon: 'c002_blunt.tex.png',
      quality: 'Blue',
      name: 'Anarchy',
      socket: '',
      level: 19,
      url: [
        0,
        'records\/items\/gearweapons\/blunt1h\/c002_blunt.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '10% Chance of 36-122 Chaos Damage'
        },
        {
          label: '38-68 Physical Damage'
        },
        {
          label: '2-6 Pierce Damage'
        },
        {
          label: '+30% Chaos Damage'
        },
        {
          label: '+14 Cunning'
        },
        {
          label: '15% Physical Damage converted to Chaos'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/necklaces\/c031_necklace.dbr',
      icon: 'c31_necklace_ancestral_ward.tex.png',
      quality: 'Blue',
      name: 'Ancestral Ward',
      socket: '',
      level: 65,
      url: [
        0,
        'records\/items\/gearaccessories\/necklaces\/c031_necklace.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+550 Health'
        },
        {
          label: '60% Bleeding Resistance'
        },
        {
          label: '28% Elemental Resistance'
        },
        {
          label: '60% Acid Resistance'
        },
        {
          label: '+2 to Mogdrogen\'s Pact',
          extras: 'Tier 2 Shaman skill'
        },
        {
          label: '+2 to Tenacity of the Boar',
          extras: 'Tier 4 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/guns1h\/c016_gun1h.dbr',
      icon: 'c016_gun1h.tex.png',
      quality: 'Blue',
      name: 'Apothecary\'s Injector',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearweapons\/guns1h\/c016_gun1h.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '20% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '40-50 Physical Damage'
        },
        {
          label: '+30% to All Damage'
        },
        {
          label: '+24 Defensive Ability'
        },
        {
          label: '+102 Health'
        },
        {
          label: '+16% Casting Speed'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/medals\/c011_medal.dbr',
      icon: 'c11_medal_menhirs_sign.tex.png',
      quality: 'Blue',
      name: 'Apothecary\'s Sign',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearaccessories\/medals\/c011_medal.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+30 Spirit'
        },
        {
          label: '+2 Energy Regenerated per second'
        },
        {
          label: '5% Bleeding Resistance'
        },
        {
          label: '15% Vitality Resistance'
        },
        {
          label: '24% Acid Resistance'
        },
        {
          label: '+2 to Menhir\'s Bulwark',
          extras: 'Tier 9 Soldier skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearhands\/c013_hands.dbr',
      icon: 'c013_hands.tex.png',
      quality: 'Blue',
      name: 'Apothecary\'s Touch',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearhands\/c013_hands.dbr',
        '',
        '',
        ''
      ],
      numItems: 3,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '194 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+15 Spirit'
        },
        {
          label: '+8 Health\/s'
        },
        {
          label: '+10% Health\/s'
        },
        {
          label: '15% Elemental Resistance'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Apothecary\'s Touch',
        description: 'Training in the art of field medicine enables you to soothe the wounds of nearby allies.',
        level: 2,
        petStats: [

        ],
        headerStats: [
          {
            label: '8 Seconds Skill Recharge'
          },
          {
            label: '404 Energy Cost'
          },
          {
            label: '5 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '1887 Health Restored'
          },
          {
            label: '34% Health Restored'
          }
        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/rings\/c012_ring.dbr',
      icon: 'c12_ring_menhirs_wisdom.tex.png',
      quality: 'Blue',
      name: 'Apothecary\'s Wisdom',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearaccessories\/rings\/c012_ring.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+20% Constitution'
        },
        {
          label: '+32 Spirit'
        },
        {
          label: '+10% Health\/s'
        },
        {
          label: '18% Bleeding Resistance'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/axe1h\/c014_axe.dbr',
      icon: 'c014_axe.tex.png',
      quality: 'Blue',
      name: 'Asterkarn Cleaver',
      socket: '',
      level: 67,
      url: [
        0,
        'records\/items\/gearweapons\/axe1h\/c014_axe.dbr',
        '',
        '',
        ''
      ],
      numItems: 4,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '10% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '10 Cold Damage'
        },
        {
          label: '82-94 Physical Damage'
        },
        {
          label: '+70% Cold Damage'
        },
        {
          label: '+50% Physical Damage'
        },
        {
          label: '+320 Health'
        },
        {
          label: '6% Physical Resistance'
        },
        {
          label: '15% Physical Damage converted to Cold'
        },
        {
          label: '+2 to Night\'s Chill',
          extras: 'Tier 4 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Chillmane Breath',
        description: 'Chill your enemies to the core with the breath of a Chillmane Yeti.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '2 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '519-636 Cold Damage'
          },
          {
            label: '+15% Weapon Damage'
          }
        ],
        trigger: '5% Chance on attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearshoulders\/c002_shoulder.dbr',
      icon: 'c002_shoulder.tex.png',
      quality: 'Blue',
      name: 'Astral Mantle',
      socket: '',
      level: 23,
      url: [
        0,
        'records\/items\/gearshoulders\/c002_shoulder.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '127 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+19% Elemental Damage'
        },
        {
          label: '+21 Spirit'
        },
        {
          label: '+15% Light Radius'
        },
        {
          label: '+20% Energy Regenerated per second'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Lightning Orb',
        description: 'Retaliates with an orb of lightning launched in the attacker\'s direction, damaging and stunning all in its path.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '1 Seconds Skill Recharge'
          },
          {
            label: '1 Projectile'
          }
        ],
        bodyStats: [
          {
            label: '4536-33780 Lightning Damage'
          },
          {
            label: '1 Meter Radius'
          },
          {
            label: '100% Chance to pass through Enemies'
          },
          {
            label: '1 Projectile'
          },
          {
            label: 'Stun target for 1 Seconds'
          }
        ],
        trigger: '100% Chance when hit by a critical'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/necklaces\/c014_necklace.dbr',
      icon: 'c14_necklace_avarice_of_androneus.tex.png',
      quality: 'Blue',
      name: 'Avarice of Androneus',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearaccessories\/necklaces\/c014_necklace.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+1 to All Skills'
        },
        {
          label: '+15 Cunning'
        },
        {
          label: '+15 Spirit'
        },
        {
          label: '+15 Physique'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/medals\/c010_medal.dbr',
      icon: 'c10_medal_badge_of_the_crimson_company.tex.png',
      quality: 'Blue',
      name: 'Badge of the Crimson Company',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearaccessories\/medals\/c010_medal.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+36% Pierce Damage'
        },
        {
          label: '+40% Bleeding Damage'
        },
        {
          label: '+4% Attack Speed'
        },
        {
          label: '+40 Cunning'
        },
        {
          label: '40% Bleeding Resistance'
        },
        {
          label: '+2 to Anatomy of Murder',
          extras: 'Tier 5 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/rings\/d008_ring.dbr',
      icon: 'd08_ring_band_of_the_eternal_haunt.tex.png',
      quality: 'Epic',
      name: 'Band of the Eternal Haunt',
      socket: '',
      level: 65,
      url: [
        0,
        'records\/items\/gearaccessories\/rings\/d008_ring.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+50% Aether Damage'
        },
        {
          label: '+50% Cold Damage'
        },
        {
          label: '+50% Frost Damage'
        },
        {
          label: '+26 Defensive Ability'
        },
        {
          label: '+5% Casting Speed'
        },
        {
          label: '26% Chaos Resistance'
        },
        {
          label: '34% Vitality Resistance'
        },
        {
          label: '3% to Maximum Vitality Resistance'
        },
        {
          label: '+2 to Olexra\'s Flash Freeze',
          extras: 'Tier 2 Arcanist skill'
        },
        {
          label: '+2 to Callidor\'s Tempest',
          extras: 'Tier 2 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Eternal Haunt',
        description: 'Inflict an tormenting haunt upon your foe that spreads among your foes.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '6 Second Duration'
          },
          {
            label: '6 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '119 Aether Damage'
          },
          {
            label: '79 Cold Damage'
          }
        ],
        trigger: '10% Chance on attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/necklaces\/c012_necklace.dbr',
      icon: 'c12_necklace_bane_of_nuram\'siin.tex.png',
      quality: 'Blue',
      name: 'Bane of Nuram\'Siin',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearaccessories\/necklaces\/c012_necklace.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '6 Cold Damage'
        },
        {
          label: '+40% Cold Damage'
        },
        {
          label: '22% Energy Absorption From Enemy Spells'
        },
        {
          label: '+24 Spirit'
        },
        {
          label: '+35 Offensive Ability'
        },
        {
          label: '+15% Damage to Chthonic'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Bane of Nuram\'Siin',
        description: 'Ancient word of power which freezes the target solid in a prison of ice.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '6.5 Seconds Skill Recharge'
          },
          {
            label: '866 Energy Cost'
          }
        ],
        bodyStats: [
          {
            label: '1282 Cold Damage'
          },
          {
            label: '284-1238 Lightning Damage'
          }
        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/melee2h\/d007_sword2h.dbr',
      icon: 'd007_sword2h.tex.png',
      quality: 'Epic',
      name: 'Bane of the Winter King',
      socket: '',
      level: 75,
      url: [
        0,
        'records\/items\/gearweapons\/melee2h\/d007_sword2h.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '248-270 Cold Damage'
        }
      ],
      bodyStats: [
        {
          label: '34-42 Cold Damage'
        },
        {
          label: '+160% Cold Damage'
        },
        {
          label: '+160% Frost Damage'
        },
        {
          label: '+15% Attack Speed'
        },
        {
          label: '+5% Health'
        },
        {
          label: '+15% Casting Speed'
        },
        {
          label: '+1 to All Skills in Nightblade'
        },
        {
          label: '+1 to All Skills in Shaman'
        }
      ],
      petStats: [
        {
          label: '+55% to All Damage'
        },
        {
          label: '+8% Health'
        },
        {
          label: '25% Elemental Resistance'
        }
      ],
      greenRarity: 0,
      skill: {
        name: 'Heart of the Winter King',
        description: 'The winter king was a savage, yet just ruler, who fiercely defended his homeland besides his loyal' +
        ' white wolf, whom he called father. The spirit of the great king still dwells within his mighty blade. ' +
        'This ability must be toggled to maintain its effect.',
        level: 1,
        petStats: [

        ],
        headerStats: [

        ],
        bodyStats: [
          {
            label: '40-70 Cold Damage'
          },
          {
            label: '15% Chance of +400% Cold Damage'
          },
          {
            label: '33% Cold Resistance'
          },
          {
            label: 'Increases Armor by 36%'
          }
        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/focus\/c006_focus.dbr',
      icon: 'c006_focus.tex.png',
      quality: 'Blue',
      name: 'Banshee\'s Misery',
      socket: '',
      level: 34,
      url: [
        0,
        'records\/items\/gearweapons\/focus\/c006_focus.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '3-5 Cold Damage'
        },
        {
          label: '+60% Cold Damage'
        },
        {
          label: '+4% Spirit'
        },
        {
          label: '+3.5 Energy Regenerated per second'
        },
        {
          label: '+22 Offensive Ability'
        },
        {
          label: '20% Cold Resistance'
        },
        {
          label: '-16% Skill Cooldown Reduction'
        },
        {
          label: '+2 to Necrotic Edge',
          extras: 'Tier 4 Necromancer skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Fear',
        description: 'Strike fear into the hearts of your enemies.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '1 Seconds Skill Recharge'
          }
        ],
        bodyStats: [

        ],
        trigger: ' '
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/focus\/c006_focus.dbr',
      icon: 'c006_focus.tex.png',
      quality: 'Blue',
      name: 'Banshee\'s Misery',
      socket: '(Chilled Steel)',
      level: 34,
      url: [
        0,
        'records\/items\/gearweapons\/focus\/c006_focus.dbr',
        '',
        '',
        'records\/items\/materia\/compa_chilledsteel.dbr'
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '3-5 Cold Damage'
        },
        {
          label: '+60% Cold Damage'
        },
        {
          label: '+4% Spirit'
        },
        {
          label: '+3.5 Energy Regenerated per second'
        },
        {
          label: '+22 Offensive Ability'
        },
        {
          label: '20% Cold Resistance'
        },
        {
          label: '-16% Skill Cooldown Reduction'
        },
        {
          label: '+2 to Necrotic Edge',
          extras: 'Tier 4 Necromancer skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Fear',
        description: 'Strike fear into the hearts of your enemies.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '1 Seconds Skill Recharge'
          }
        ],
        bodyStats: [

        ],
        trigger: ' '
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/blunt1h\/d002_blunt.dbr',
      icon: 'd002_blunt.tex.png',
      quality: 'Epic',
      name: 'Beacon of the Winter\'s Veil',
      socket: '',
      level: 58,
      url: [
        0,
        'records\/items\/gearweapons\/blunt1h\/d002_blunt.dbr',
        '',
        '',
        ''
      ],
      numItems: 3,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '13-108 Physical Damage'
        },
        {
          label: '120 Frost Damage over 3 Seconds'
        },
        {
          label: '+66% Cold Damage'
        },
        {
          label: '+66% Frost Damage'
        },
        {
          label: '+42 Offensive Ability'
        },
        {
          label: '+8% Crit Damage'
        },
        {
          label: '15% Physical Damage converted to Cold'
        },
        {
          label: '+3 to Night\'s Chill',
          extras: 'Tier 4 Nightblade skill'
        },
        {
          label: '+3 to Absolute Zero',
          extras: 'Tier 5 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Winter\'s Veil',
        description: 'Orbs of jagged ice swirl around you in a freezing whirlwind.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '1 Seconds Skill Recharge'
          },
          {
            label: '1 Projectile'
          }
        ],
        bodyStats: [
          {
            label: '360-514 Cold Damage'
          },
          {
            label: '0.3 Meter Radius'
          },
          {
            label: '100% Chance to pass through Enemies'
          },
          {
            label: '1 Projectile'
          },
          {
            label: '+15% Weapon Damage'
          }
        ],
        trigger: '15% Chance on attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/necklaces\/c039_necklace.dbr',
      icon: 'c39_necklace_bindingemeraldofmogdrogen.tex.png',
      quality: 'Blue',
      name: 'Binding Emerald of Mogdrogen',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearaccessories\/necklaces\/c039_necklace.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+20 Defensive Ability'
        },
        {
          label: '51% Bleeding Resistance'
        },
        {
          label: '5% to Maximum Bleeding Resistance'
        },
        {
          label: '+2 to Summon Briarthorn',
          extras: 'Tier 3 Shaman skill'
        },
        {
          label: '+2 to Ground Slam',
          extras: 'Tier 5 Shaman skill'
        }
      ],
      petStats: [
        {
          label: '+50% to All Damage'
        },
        {
          label: '20% Vitality Resistance'
        }
      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/focus\/c008_focus.dbr',
      icon: 'c008_focus.tex.png',
      quality: 'Blue',
      name: 'Black Grimoire of Og\'Napesh',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearweapons\/focus\/c008_focus.dbr',
        '',
        '',
        ''
      ],
      numItems: 3,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+45% Vitality Damage'
        },
        {
          label: '+21 Spirit'
        },
        {
          label: '+3.8 Energy Regenerated per second'
        },
        {
          label: '+20 Offensive Ability'
        },
        {
          label: '+10% Casting Speed'
        },
        {
          label: '-16% Skill Cooldown Reduction'
        },
        {
          label: '+2 to Soul Harvest',
          extras: 'Tier 7 Necromancer skill'
        }
      ],
      petStats: [
        {
          label: '+18% to All Damage'
        }
      ],
      greenRarity: 0,
      skill: {
        name: 'Summon Revenant of Og\'Napesh',
        description: 'Channel the dark writings of Og\'Napesh to summon his most foul revenant back into the world of ' +
        'the living. Only one revenant can be summoned at any one time. The revenant scales with Pet Bonuses.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '60 Seconds Skill Recharge'
          },
          {
            label: '1226 Energy Cost'
          }
        ],
        bodyStats: [

        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/blunt1h\/c013_blunt.dbr',
      icon: 'c013_blunt.tex.png',
      quality: 'Blue',
      name: 'Black Hand of Sanar\'Siin',
      socket: '',
      level: 60,
      url: [
        0,
        'records\/items\/gearweapons\/blunt1h\/c013_blunt.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '48-148 Physical Damage'
        },
        {
          label: '+48% Chaos Damage'
        },
        {
          label: '+4% Offensive Ability'
        },
        {
          label: '15% Physical Damage converted to Chaos'
        },
        {
          label: '+1 to Summon Briarthorn',
          extras: 'Tier 3 Shaman skill'
        },
        {
          label: '+1 to Summon Hellhound',
          extras: 'Tier 3 Occultist skill'
        }
      ],
      petStats: [
        {
          label: '+30% to All Damage'
        },
        {
          label: '+5% Defensive Ability'
        },
        {
          label: '+5% Offensive Ability'
        }
      ],
      greenRarity: 0,
      skill: {
        name: 'Conjure Devourer',
        description: 'Pull a Chthonian Devourer from the depths of the void to protect you. The devourer scales with Pet Bonuses.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '5 Seconds Skill Recharge'
          }
        ],
        bodyStats: [

        ],
        trigger: ' '
      }
    },
    {
      baseRecord: 'records\/items\/gearhands\/c010_hands.dbr',
      icon: 'c010_hands.tex.png',
      quality: 'Blue',
      name: 'Blacksteel Gauntlets',
      socket: '',
      level: 67,
      url: [
        0,
        'records\/items\/gearhands\/c010_hands.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '798 Armor'
        }
      ],
      bodyStats: [
        {
          label: '182 Pierce Retaliation'
        },
        {
          label: '+65 Defensive Ability'
        },
        {
          label: '+18 Physique'
        },
        {
          label: '5% Physical Resistance'
        },
        {
          label: '+50% Total Retaliation Damage'
        },
        {
          label: '+2 to Counter Strike',
          extras: 'Tier 8 Soldier skill'
        },
        {
          label: '+2 to Vindictive Flame',
          extras: 'Tier 3 Demolitionist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/guns2h\/c006_gun2h.dbr',
      icon: 'c006_gun2h.tex.png',
      quality: 'Blue',
      name: 'Blackwood Arbalest',
      socket: '',
      level: 14,
      url: [
        0,
        'records\/items\/gearweapons\/guns2h\/c006_gun2h.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '7-12 Cold Damage'
        },
        {
          label: '7-12 Fire Damage'
        },
        {
          label: '7-12 Lightning Damage'
        },
        {
          label: '35% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '7 Elemental Damage'
        },
        {
          label: '39-54 Physical Damage'
        },
        {
          label: '+33% Elemental Damage'
        },
        {
          label: '+1 Energy Regenerated per second'
        },
        {
          label: '+2 to Panetti\'s Replicating Missile',
          extras: 'Tier 1 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/blunt1h\/c005_blunt.dbr',
      icon: 'c005_blunt.tex.png',
      quality: 'Blue',
      name: 'Blackwood Wand',
      socket: '',
      level: 25,
      url: [
        0,
        'records\/items\/gearweapons\/blunt1h\/c005_blunt.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '24-40 Aether Damage'
        }
      ],
      bodyStats: [
        {
          label: '7 Aether Damage'
        },
        {
          label: '+15% Aether Damage'
        },
        {
          label: '+13 Spirit'
        },
        {
          label: '+1 to All Skills in Occultist'
        },
        {
          label: '+1 to All Skills in Arcanist'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/waist\/d002_waist.dbr',
      icon: 'd02_waist_blade_breaker_sash.tex.png',
      quality: 'Epic',
      name: 'Blade Breaker Sash',
      socket: '',
      level: 58,
      url: [
        0,
        'records\/items\/gearaccessories\/waist\/d002_waist.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '66 Armor'
        }
      ],
      bodyStats: [
        {
          label: '6 Pierce Damage'
        },
        {
          label: '+44% Cold Damage'
        },
        {
          label: '+35% Pierce Damage'
        },
        {
          label: '+44% Frost Damage'
        },
        {
          label: '+15 Defensive Ability'
        },
        {
          label: '+4% Offensive Ability'
        },
        {
          label: '22% Bleeding Resistance'
        },
        {
          label: '5% to Maximum Bleeding Resistance'
        },
        {
          label: '+7% Crit Damage'
        },
        {
          label: '+1 to All Skills in Nightblade'
        },
        {
          label: '+1 to Belgothian\'s Shears',
          extras: 'Tier 2 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearhands\/c003_hands.dbr',
      icon: 'c003_hands.tex.png',
      quality: 'Blue',
      name: 'Bladedancer\'s Handguards',
      socket: '',
      level: 23,
      url: [
        0,
        'records\/items\/gearhands\/c003_hands.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '148 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+16% Physical Damage'
        },
        {
          label: '+28 Cunning'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Revenge',
        description: 'A swift jab with your weapon. Requires a melee weapon.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '1 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '+175% Weapon Damage'
          }
        ],
        trigger: '20% Chance when hit by a melee attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearlegs\/c013_legs.dbr',
      icon: 'c013_legs.tex.png',
      quality: 'Blue',
      name: 'Bladeguard Leggings',
      socket: '',
      level: 58,
      url: [
        0,
        'records\/items\/gearlegs\/c013_legs.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '776 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+25% Physical Damage'
        },
        {
          label: '+20% Pierce Damage'
        },
        {
          label: '+2% Defensive Ability'
        },
        {
          label: '+8% Chance to Avoid Melee Attacks'
        },
        {
          label: '+5% Movement Speed'
        },
        {
          label: '23% Pierce Resistance'
        },
        {
          label: '+1 to Markovian\'s Advantage',
          extras: 'Tier 1 Soldier skill'
        },
        {
          label: '+1 to Belgothian\'s Shears',
          extras: 'Tier 2 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/shields\/c016_shield.dbr',
      icon: 'c016_shield.tex.png',
      quality: 'Blue',
      name: 'Blazeguard Arbiter',
      socket: '',
      level: 65,
      url: [
        0,
        'records\/items\/gearweapons\/shields\/c016_shield.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '32% Chance to Block 980 Damage (100% Absorption)'
        }
      ],
      bodyStats: [
        {
          label: '11 Fire Damage'
        },
        {
          label: '118 Physical Damage'
        },
        {
          label: '+50% Fire Damage'
        },
        {
          label: '0.9 second Block Recovery'
        },
        {
          label: '+40 Defensive Ability'
        },
        {
          label: '44% Fire Resistance'
        },
        {
          label: '44% Lightning Resistance'
        },
        {
          label: '+2 to Blast Shield',
          extras: 'Tier 7 Demolitionist skill'
        },
        {
          label: '+2 to Temper',
          extras: 'Tier 5 Demolitionist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Ring of Everlasting Flame',
        description: 'Bathe yourself in incinerating flames that burn all around you to cinders, but leave you unharmed.',
        level: 8,
        petStats: [

        ],
        headerStats: [
          {
            label: '3 Second Duration'
          },
          {
            label: '3 Seconds Skill Recharge'
          },
          {
            label: '4 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '980 Fire Damage'
          },
          {
            label: '2010 Burn Damage over 2 Seconds'
          }
        ],
        trigger: '33% Chance on a critical attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/melee2h\/c021_axe2h.dbr',
      icon: 'c021_axe2h.tex.png',
      quality: 'Blue',
      name: 'Blessed Cleaver of Mogdrogen',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearweapons\/melee2h\/c021_axe2h.dbr',
        '',
        '',
        ''
      ],
      numItems: 4,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '235-408 Physical Damage'
        },
        {
          label: '+66% to All Damage'
        },
        {
          label: '+6% Offensive Ability'
        },
        {
          label: '+5% Total Speed'
        },
        {
          label: '+2 to Ground Slam',
          extras: 'Tier 5 Shaman skill'
        },
        {
          label: '+2 to Emboldening Presence',
          extras: 'Tier 8 Shaman skill'
        }
      ],
      petStats: [
        {
          label: '+80% to All Damage'
        },
        {
          label: '+8% Attack Speed'
        },
        {
          label: '+8% Health'
        }
      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/blunt1h\/c007_blunt.dbr',
      icon: 'c007_blunt.tex.png',
      quality: 'Blue',
      name: 'Blessed Torch',
      socket: '',
      level: 45,
      url: [
        0,
        'records\/items\/gearweapons\/blunt1h\/c007_blunt.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '48-65 Physical Damage'
        },
        {
          label: '72-38 Burn Damage over 3 Seconds'
        },
        {
          label: '+44% Burn Damage'
        },
        {
          label: '+33% Light Radius'
        },
        {
          label: '+30 Offensive Ability'
        },
        {
          label: '32% Fire Resistance'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Ring of Everlasting Flame',
        description: 'Bathe yourself in incinerating flames that burn all around you to cinders, but leave you unharmed.',
        level: 8,
        petStats: [

        ],
        headerStats: [
          {
            label: '3 Second Duration'
          },
          {
            label: '3 Seconds Skill Recharge'
          },
          {
            label: '4 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '980 Fire Damage'
          },
          {
            label: '2010 Burn Damage over 2 Seconds'
          }
        ],
        trigger: '33% Chance on a critical attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/necklaces\/c021_necklace.dbr',
      icon: 'c21_necklace_blightshard_amulet.tex.png',
      quality: 'Blue',
      name: 'Blightshard Amulet',
      socket: '',
      level: 24,
      url: [
        0,
        'records\/items\/gearaccessories\/necklaces\/c021_necklace.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '20 Poison Damage over 5 Seconds'
        },
        {
          label: '+15% Poison Damage'
        },
        {
          label: '+15 Offensive Ability'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Summon Blighted Rift Scourge',
        description: 'Summon a Blighted Rift Scourge from the depths to follow your every command. ' +
        'Only one rift scourge can be summoned at any one time. The rift scourge scales with Pet Bonuses.',
        level: 2,
        petStats: [

        ],
        headerStats: [
          {
            label: '60 Seconds Skill Recharge'
          },
          {
            label: '800 Energy Cost'
          }
        ],
        bodyStats: [

        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/focus\/d005_focus.dbr',
      icon: 'd005_focus.tex.png',
      quality: 'Epic',
      name: 'Blood Orb of Ch\'thon',
      socket: '',
      level: 65,
      url: [
        0,
        'records\/items\/gearweapons\/focus\/d005_focus.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+92% Chaos Damage'
        },
        {
          label: '+64% Vitality Damage'
        },
        {
          label: '+64% Vitality Decay Damage'
        },
        {
          label: '+68% Energy Regenerated per second'
        },
        {
          label: '+17% Casting Speed'
        },
        {
          label: '24% Chaos Resistance'
        },
        {
          label: '+4% Crit Damage'
        },
        {
          label: '-16% Skill Cooldown Reduction'
        },
        {
          label: '+1 to All Skills in Occultist'
        },
        {
          label: '+1 to Bloody Pox',
          extras: 'Tier 2 Occultist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Blood Rite',
        description: 'The dark blood of Ch\'thon corrupts all that it touches, even the arcane.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '330 Energy Cost'
          }
        ],
        bodyStats: [
          {
            label: '100% Elemental Damage converted to Chaos'
          }
        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/medals\/d005_medal.dbr',
      icon: 'd05_medal_blood_sigil_of_ch\'thon.tex.png',
      quality: 'Epic',
      name: 'Blood Sigil of Ch\'Thon',
      socket: '',
      level: 65,
      url: [
        0,
        'records\/items\/gearaccessories\/medals\/d005_medal.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+48% Chaos Damage'
        },
        {
          label: '+48% Fire Damage'
        },
        {
          label: '+48% Burn Damage'
        },
        {
          label: '+58 Offensive Ability'
        },
        {
          label: '22% Chaos Resistance'
        },
        {
          label: '30% Fire Resistance'
        },
        {
          label: '+18% Damage to Human'
        },
        {
          label: '+3 to Destruction',
          extras: 'Tier 6 Occultist skill'
        },
        {
          label: '+2 to Brimstone',
          extras: 'Tier 9 Demolitionist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Fire and Brimstone',
        description: 'Become empowered with the cataclysmic power of fire and brimstone.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '6 Second Duration'
          },
          {
            label: '10 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '42-154 Chaos Damage'
          },
          {
            label: '72-128 Fire Damage'
          }
        ],
        trigger: '15% Chance on attacking'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/waist\/c009_waist.dbr',
      icon: 'c09_waist_bloodbathed_links.tex.png',
      quality: 'Blue',
      name: 'Bloodbathed Links',
      socket: '',
      level: 45,
      url: [
        0,
        'records\/items\/gearaccessories\/waist\/c009_waist.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '48 Armor'
        }
      ],
      bodyStats: [
        {
          label: '5% Chance of 375 Bleeding Damage over 3 Seconds'
        },
        {
          label: '+42% Bleeding Damage'
        },
        {
          label: '+30 Physique'
        },
        {
          label: '+2 to Laceration',
          extras: 'Tier 7 Soldier skill'
        },
        {
          label: '+1 to Blood of Dreeg',
          extras: 'Tier 4 Occultist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/swords1h\/c017_sword.dbr',
      icon: 'c017_bloodbornsabre01.tex.png',
      quality: 'Blue',
      name: 'Bloodborn Sabre',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearweapons\/swords1h\/c017_sword.dbr',
        '',
        '',
        ''
      ],
      numItems: 3,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '30% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '62-78 Physical Damage'
        },
        {
          label: '8 Pierce Damage'
        },
        {
          label: '+46% Pierce Damage'
        },
        {
          label: '+40% Bleeding Damage'
        },
        {
          label: '+30 Offensive Ability'
        },
        {
          label: '+2 to Circle of Slaughter',
          extras: 'Tier 8 Nightblade skill'
        },
        {
          label: '+1 to Whirling Death',
          extras: 'Tier 6 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Ring of Steel',
        description: 'In the blink of an eye, hundreds of phantasmal blades encircle you at lethal speed, cutting down adjacent foes.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '5 Seconds Skill Recharge'
          },
          {
            label: '3.75 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '18456 Pierce Damage'
          },
          {
            label: '+25% Weapon Damage'
          },
          {
            label: 'Stun target for 1 Seconds'
          }
        ],
        trigger: '2% Chance when Hit by Melee Attacks'
      }
    },
    {
      baseRecord: 'records\/items\/gearfeet\/c006_feet.dbr',
      icon: 'c006_feet.tex.png',
      quality: 'Blue',
      name: 'Bloodhound Greaves',
      socket: '',
      level: 30,
      url: [
        0,
        'records\/items\/gearfeet\/c006_feet.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '166 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+15% Bleeding Damage'
        },
        {
          label: '+2% Chance to Avoid Projectiles'
        },
        {
          label: '+15 Cunning'
        },
        {
          label: '+11% Movement Speed'
        },
        {
          label: '35% Slow Resistance'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/swords1h\/c006_sword.dbr',
      icon: 'c006_bloodreapersclaw.tex.png',
      quality: 'Blue',
      name: 'Bloodreaper\'s Claw',
      socket: '',
      level: 30,
      url: [
        0,
        'records\/items\/gearweapons\/swords1h\/c006_sword.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '45% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '40-62 Physical Damage'
        },
        {
          label: '30 Bleeding Damage over 3 Seconds'
        },
        {
          label: '+15% Bleeding Damage'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/axe1h\/c004_axe.dbr',
      icon: 'c004_axe.tex.png',
      quality: 'Blue',
      name: 'Bloodreaper\'s Cleaver',
      socket: '',
      level: 30,
      url: [
        0,
        'records\/items\/gearweapons\/axe1h\/c004_axe.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '35-86 Physical Damage'
        },
        {
          label: '+26% Physical Damage'
        },
        {
          label: '+20 Offensive Ability'
        },
        {
          label: '3% of Attack Damage converted to Health'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearhead\/c009_head.dbr',
      icon: 'c009_head.tex.png',
      quality: 'Blue',
      name: 'Bloodreaper\'s Cowl',
      socket: '',
      level: 30,
      url: [
        0,
        'records\/items\/gearhead\/c009_head.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '198 Armor'
        }
      ],
      bodyStats: [
        {
          label: '132-67 Bleeding Retaliation over 3 Seconds'
        },
        {
          label: '+35 Cunning'
        },
        {
          label: '+2 to Heart Seeker',
          extras: 'Tier 4 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/medals\/c004_medal.dbr',
      icon: 'c03_medal_bloodreapersmark.tex.png',
      quality: 'Blue',
      name: 'Bloodreaper\'s Mark',
      socket: '',
      level: 30,
      url: [
        0,
        'records\/items\/gearaccessories\/medals\/c004_medal.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '15 Bleeding Damage over 3 Seconds'
        },
        {
          label: '+17% Pierce Damage'
        },
        {
          label: '+12% Bleeding Damage'
        },
        {
          label: '3% of Attack Damage converted to Health'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/melee2h\/c007_axe2h.dbr',
      icon: 'c007_axe2h.tex.png',
      quality: 'Blue',
      name: 'Boneshard Lacerator',
      socket: '',
      level: 30,
      url: [
        0,
        'records\/items\/gearweapons\/melee2h\/c007_axe2h.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '95-270 Physical Damage'
        },
        {
          label: '72 Bleeding Damage over 3 Seconds'
        },
        {
          label: '+75% Bleeding Damage'
        },
        {
          label: '+10% Crit Damage'
        },
        {
          label: '5% of Attack Damage converted to Health'
        },
        {
          label: '+2 to Feral Hunger',
          extras: 'Tier 3 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearlegs\/d002_legs.dbr',
      icon: 'd002_legs.tex.png',
      quality: 'Epic',
      name: 'Boneweave Leggings',
      socket: '(Scaled Hide)',
      level: 50,
      url: [
        0,
        'records\/items\/gearlegs\/d002_legs.dbr',
        '',
        '',
        'records\/items\/materia\/compa_scalyhide.dbr'
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '947 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+35% Vitality Damage'
        },
        {
          label: '+35% Vitality Decay Damage'
        },
        {
          label: '+25 Defensive Ability'
        },
        {
          label: '+18 Health\/s'
        },
        {
          label: '+15% Health\/s'
        },
        {
          label: '20% Chaos Resistance'
        },
        {
          label: '28% Vitality Resistance'
        },
        {
          label: '+2 to Sigil of Consumption',
          extras: 'Tier 3 Occultist skill'
        },
        {
          label: '+2 to Wendigo Totem',
          extras: 'Tier 5 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Sigil of Consumption',
        description: 'Create an occult sigil on the ground that consumes the life-force of enemies caught within its power, ' +
        'returning a portion of the damage dealt as health to you.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '6 Second Duration'
          },
          {
            label: '3 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '202 Vitality Damage'
          },
          {
            label: '40% of Attack Damage converted to Health'
          },
          {
            label: '3.2 Meter Radius'
          }
        ],
        trigger: ' '
      }
    },
    {
      baseRecord: 'records\/items\/gearfeet\/c003_feet.dbr',
      icon: 'c003_feet.tex.png',
      quality: 'Blue',
      name: 'Boots of Unseeing Swiftness',
      socket: '',
      level: 25,
      url: [
        0,
        'records\/items\/gearfeet\/c003_feet.dbr',
        '',
        '',
        ''
      ],
      numItems: 4,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '125 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+4% Chance to Avoid Projectiles'
        },
        {
          label: '+4% Chance to Avoid Melee Attacks'
        },
        {
          label: '+20% Movement Speed'
        },
        {
          label: '+22 Physique'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Interruption',
        description: 'The immense speed of these boots requires complete focus on every step taken. Even the slightest distraction could result in a misstep.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '3 Second Duration'
          },
          {
            label: '3 Seconds Skill Recharge'
          }
        ],
        bodyStats: [

        ],
        trigger: '100% Chance when hit'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/necklaces\/c029_necklace.dbr',
      icon: 'c29_necklace_bramblewoodamulet.tex.png',
      quality: 'Blue',
      name: 'Bramblewood Amulet',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearaccessories\/necklaces\/c029_necklace.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+28% Lightning Damage'
        },
        {
          label: '72 Pierce Retaliation'
        },
        {
          label: '+200 Health'
        },
        {
          label: '20% Acid Resistance'
        },
        {
          label: '+1 to All Skills in Shaman'
        }
      ],
      petStats: [
        {
          label: '+15% to All Damage'
        }
      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearhands\/c008_hands.dbr',
      icon: 'c008_hands.tex.png',
      quality: 'Blue',
      name: 'Brawler\'s Gloves',
      socket: '',
      level: 41,
      url: [
        0,
        'records\/items\/gearhands\/c008_hands.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '211 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+18% Physical Damage'
        },
        {
          label: '+45% Bleeding Damage'
        },
        {
          label: '+45% Internal Trauma Damage'
        },
        {
          label: '+44 Offensive Ability'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Knockout',
        description: 'Take your opponent out of the fight with a swift and precise strike. Activates off of default melee weapon attacks.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '30% Chance to be Used'
          }
        ],
        bodyStats: [
          {
            label: '1086 Bleeding Damage over 2 Seconds'
          },
          {
            label: '1086 Internal Trauma Damage over 2 Seconds'
          },
          {
            label: '+200% Weapon Damage'
          }
        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/guns2h\/c021_gun2h.dbr',
      icon: 'c021_gun2h.tex.png',
      quality: 'Blue',
      name: 'Brimstone Repeater',
      socket: '',
      level: 30,
      url: [
        0,
        'records\/items\/gearweapons\/guns2h\/c021_gun2h.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '30% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '85-124 Physical Damage'
        },
        {
          label: '48 Burn Damage over 3 Seconds'
        },
        {
          label: '+32% Attack Speed'
        },
        {
          label: '45% Physical Damage converted to Fire'
        },
        {
          label: '+2 to Fire Strike',
          extras: 'Tier 1 Demolitionist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearshoulders\/c015_shoulder.dbr',
      icon: 'c015_shoulder.tex.png',
      quality: 'Blue',
      name: 'Brimstone Shoulderguard',
      socket: '',
      level: 30,
      url: [
        0,
        'records\/items\/gearshoulders\/c015_shoulder.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '198 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+16% Fire Damage'
        },
        {
          label: '+28 Spirit'
        },
        {
          label: '+90 Health'
        },
        {
          label: '+2 to Explosive Strike',
          extras: 'Tier 3 Demolitionist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/guns1h\/c015_gun1h.dbr',
      icon: 'c015_gun1h.tex.png',
      quality: 'Blue',
      name: 'Burrwitch Peacekeeper',
      socket: '',
      level: 65,
      url: [
        0,
        'records\/items\/gearweapons\/guns1h\/c015_gun1h.dbr',
        '',
        '',
        ''
      ],
      numItems: 4,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '30% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '50-64 Physical Damage'
        },
        {
          label: '+65% Physical Damage'
        },
        {
          label: '+10% Attack Speed'
        },
        {
          label: '+2% Defensive Ability'
        },
        {
          label: '+30 Cunning'
        },
        {
          label: '5% Physical Resistance'
        },
        {
          label: '+1 to All Skills in Soldier'
        },
        {
          label: '+2 to Vigor',
          extras: 'Tier 5 Inquisitor skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/axe1h\/d001_axe.dbr',
      icon: 'd001_axe.tex.png',
      quality: 'Epic',
      name: 'Butcher of Burrwitch',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearweapons\/axe1h\/d001_axe.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '20% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '42-82 Physical Damage'
        },
        {
          label: '120 Bleeding Damage over 3 Seconds'
        },
        {
          label: '+65% Bleeding Damage'
        },
        {
          label: '+20% to All Damage'
        },
        {
          label: '+55 Offensive Ability'
        },
        {
          label: '+25% Damage to Human'
        },
        {
          label: '+2 to Primal Bond',
          extras: 'Tier 9 Shaman skill'
        },
        {
          label: '+2 to Blood Pact',
          extras: 'Tier 8 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Devour',
        description: 'Upon landing a vicious blow, devour some of your foe\'s exposed life essence.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '0.5 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '430 Bleeding Damage over 2 Seconds'
          },
          {
            label: '300% of Attack Damage converted to Health'
          },
          {
            label: '+55% Weapon Damage'
          }
        ],
        trigger: '100% Chance on a critical attack (target enemy)'
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/focus\/c018_focus.dbr',
      icon: 'c018_focus.tex.png',
      quality: 'Blue',
      name: 'Callidor\'s Codex',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearweapons\/focus\/c018_focus.dbr',
        '',
        '',
        ''
      ],
      numItems: 4,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+98% Aether Damage'
        },
        {
          label: '+44% Fire Damage'
        },
        {
          label: '+98% Burn Damage'
        },
        {
          label: '+65% Energy Regenerated per second'
        },
        {
          label: '+28 Offensive Ability'
        },
        {
          label: '+18% Casting Speed'
        },
        {
          label: '-16% Skill Cooldown Reduction'
        },
        {
          label: '+4 to Inferno',
          extras: 'Tier 5 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearshoulders\/c021_shoulder.dbr',
      icon: 'c021_shoulder.tex.png',
      quality: 'Blue',
      name: 'Callidor\'s Mantle',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearshoulders\/c021_shoulder.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '736 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+46% Fire Damage'
        },
        {
          label: '+46% Burn Damage'
        },
        {
          label: '+44 Spirit'
        },
        {
          label: '+35 Offensive Ability'
        },
        {
          label: '20% Vitality Resistance'
        },
        {
          label: '+2 to Reckless Power',
          extras: 'Tier 9 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/necklaces\/c038_necklace.dbr',
      icon: 'c38_necklace_callidors_shard.tex.png',
      quality: 'Blue',
      name: 'Callidor\'s Shard',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearaccessories\/necklaces\/c038_necklace.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+60% Aether Damage'
        },
        {
          label: '+220 Health'
        },
        {
          label: '+4 Energy Regenerated per second'
        },
        {
          label: '46% Aether Resistance'
        },
        {
          label: '5% to Maximum Aether Resistance'
        },
        {
          label: '30% Fire Damage converted to Aether'
        },
        {
          label: '+2 to Callidor\'s Tempest',
          extras: 'Tier 2 Arcanist skill'
        },
        {
          label: '+2 to Inferno',
          extras: 'Tier 5 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/necklaces\/c026_necklace.dbr',
      icon: 'c26_necklace_cerulean_shard.tex.png',
      quality: 'Blue',
      name: 'Cerulean Shard',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearaccessories\/necklaces\/c026_necklace.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+15% Lightning Damage'
        },
        {
          label: '+35% Electrocute Damage'
        },
        {
          label: '+2.2 Energy Regenerated per second'
        },
        {
          label: '+42 Offensive Ability'
        },
        {
          label: '+2 to Stun Jacks',
          extras: 'Tier 1 Demolitionist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Lightning Nova',
        description: 'Unleashes a blast of lightning energy, damaging all nearby enemies.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '1 Seconds Skill Recharge'
          },
          {
            label: '4 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '3348-28347 Lightning Damage'
          }
        ],
        trigger: '10% Chance on attacking'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/waist\/c011_waist.dbr',
      icon: 'c11_waist_chains_of_torment.tex.png',
      quality: 'Blue',
      name: 'Chains of Anguish',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearaccessories\/waist\/c011_waist.dbr',
        '',
        '',
        ''
      ],
      numItems: 4,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '48 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+80% to All Damage'
        },
        {
          label: '+4% Total Speed'
        },
        {
          label: '+10% Crit Damage'
        },
        {
          label: '8% of Attack Damage converted to Health'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Anguish',
        description: 'The cursed chains torment you with visions of your own demise.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '5 Second Duration'
          },
          {
            label: '60 Seconds Skill Recharge'
          }
        ],
        bodyStats: [

        ],
        trigger: '5% Chance on attacking'
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/melee2h\/c016_axe2h.dbr',
      icon: 'c016_axe2h.tex.png',
      quality: 'Blue',
      name: 'Champion of the Light',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearweapons\/melee2h\/c016_axe2h.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '10% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '238-385 Physical Damage'
        },
        {
          label: '144 Electrocute Damage over 3 Seconds'
        },
        {
          label: '+113% Lightning Damage'
        },
        {
          label: '+120% Electrocute Damage'
        },
        {
          label: '42-624 Lightning Retaliation'
        },
        {
          label: '+20% Attack Speed'
        },
        {
          label: '+30% Total Retaliation Damage'
        },
        {
          label: '45% Physical Damage converted to Lightning'
        },
        {
          label: '+2 to Oak Skin',
          extras: 'Tier 6 Shaman skill'
        },
        {
          label: '+2 to Counter Strike',
          extras: 'Tier 8 Soldier skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/focus\/c012_focus.dbr',
      icon: 'c012_focus.tex.png',
      quality: 'Blue',
      name: 'Channeling Orb of the Covenant',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearweapons\/focus\/c012_focus.dbr',
        '',
        '',
        ''
      ],
      numItems: 5,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+70% Chaos Damage'
        },
        {
          label: '+65% Energy Regenerated per second'
        },
        {
          label: '+32 Offensive Ability'
        },
        {
          label: '24% Elemental Resistance'
        },
        {
          label: '-16% Skill Cooldown Reduction'
        },
        {
          label: '+2 to Hellfire',
          extras: 'Tier 7 Occultist skill'
        },
        {
          label: '+2 to Infernal Breath',
          extras: 'Tier 9 Occultist skill'
        }
      ],
      petStats: [
        {
          label: '+55% to All Damage'
        },
        {
          label: '+10% Offensive Ability'
        }
      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearlegs\/d011_legs.dbr',
      icon: 'd011_legs.tex.png',
      quality: 'Epic',
      name: 'Chausses of Barbaros',
      socket: '',
      level: 75,
      url: [
        0,
        'records\/items\/gearlegs\/d011_legs.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '1316 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+46% Physical Damage'
        },
        {
          label: '+32% Bleeding Damage'
        },
        {
          label: '+46% Internal Trauma Damage'
        },
        {
          label: '+18 Defensive Ability'
        },
        {
          label: '+4% Chance to Avoid Melee Attacks'
        },
        {
          label: '+7 Health\/s'
        },
        {
          label: '+96 Offensive Ability'
        },
        {
          label: '12% Chaos Resistance'
        },
        {
          label: '3% Physical Resistance'
        },
        {
          label: '+3 to Oleron\'s Rage',
          extras: 'Tier 9 Soldier skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Battlecry',
        description: 'Blows inflicted upon you throw you into a frenzy, emboldening your allies.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '10 Second Duration'
          },
          {
            label: '15 Seconds Skill Recharge'
          },
          {
            label: '18 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '+225% to All Damage'
          },
          {
            label: '+18% Total Speed'
          },
          {
            label: 'Increases Armor by 35%'
          }
        ],
        trigger: '25% Chance when hit'
      }
    },
    {
      baseRecord: 'records\/items\/geartorso\/d009_torso.dbr',
      icon: 'd009_torso.tex.png',
      quality: 'Epic',
      name: 'Chestguard of Justice',
      socket: '',
      level: 68,
      url: [
        0,
        'records\/items\/geartorso\/d009_torso.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '1205 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+25% Fire Damage'
        },
        {
          label: '+48 Defensive Ability'
        },
        {
          label: '+4% Defensive Ability'
        },
        {
          label: '16% Chaos Resistance'
        },
        {
          label: '42% Cold Resistance'
        },
        {
          label: '42% Fire Resistance'
        },
        {
          label: '+2 to Rending Force',
          extras: 'Tier 4 Soldier skill'
        },
        {
          label: '+2 to Internal Trauma',
          extras: 'Tier 6 Soldier skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/geartorso\/c018_torso.dbr',
      icon: 'c018_torso.tex.png',
      quality: 'Blue',
      name: 'Chestguard of Perdition',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/geartorso\/c018_torso.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '398 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+19% Acid Damage'
        },
        {
          label: '125 Acid Retaliation'
        },
        {
          label: '+185 Health'
        },
        {
          label: '20% Bleeding Resistance'
        },
        {
          label: '12% Vitality Resistance'
        },
        {
          label: 'Increases Armor by 4%'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/geartorso\/c018_torso.dbr',
      icon: 'c018_torso.tex.png',
      quality: 'Blue',
      name: 'Chestguard of Perdition',
      socket: '(Rotten Heart)',
      level: 40,
      url: [
        0,
        'records\/items\/geartorso\/c018_torso.dbr',
        '',
        '',
        'records\/items\/materia\/compa_rottenheart.dbr'
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '398 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+19% Acid Damage'
        },
        {
          label: '125 Acid Retaliation'
        },
        {
          label: '+185 Health'
        },
        {
          label: '20% Bleeding Resistance'
        },
        {
          label: '12% Vitality Resistance'
        },
        {
          label: 'Increases Armor by 4%'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/swords1h\/c011_sword.dbr',
      icon: 'c011_chillblaze.tex.png',
      quality: 'Blue',
      name: 'Chillblaze',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearweapons\/swords1h\/c011_sword.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '22% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '67-74 Physical Damage'
        },
        {
          label: '54 Frost Damage over 3 Seconds'
        },
        {
          label: '+40% Cold Damage'
        },
        {
          label: '+50% Frost Damage'
        },
        {
          label: '+36 Offensive Ability'
        },
        {
          label: '15% Physical Damage converted to Cold'
        },
        {
          label: '+2 to Rune of Hagarrad',
          extras: 'Tier 2 Inquisitor skill'
        },
        {
          label: '+2 to Biting Cold',
          extras: 'Tier 4 Inquisitor skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/guns1h\/c020_gun1h.dbr',
      icon: 'c020_gun1h.tex.png',
      quality: 'Blue',
      name: 'Chillborer',
      socket: '',
      level: 75,
      url: [
        0,
        'records\/items\/gearweapons\/guns1h\/c020_gun1h.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '15% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '18-40 Physical Damage'
        },
        {
          label: '141 Frost Damage over 3 Seconds'
        },
        {
          label: '+73% Cold Damage'
        },
        {
          label: '+80% Frost Damage'
        },
        {
          label: '+15% Casting Speed'
        },
        {
          label: '+2 to Olexra\'s Flash Freeze',
          extras: 'Tier 2 Arcanist skill'
        },
        {
          label: '+2 to Star Pact',
          extras: 'Tier 9 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Sky Fragment',
        description: 'Brings down the sky upon your enemies.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '1.5 Seconds Skill Recharge'
          },
          {
            label: '2 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '20113 Cold Damage'
          },
          {
            label: '20113 Lightning Damage'
          },
          {
            label: '2 Meter Radius'
          }
        ],
        trigger: '5% Chance on attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearshoulders\/c018_shoulder.dbr',
      icon: 'c018_shoulder.tex.png',
      quality: 'Blue',
      name: 'Chillmane Mantle',
      socket: '',
      level: 66,
      url: [
        0,
        'records\/items\/gearshoulders\/c018_shoulder.dbr',
        '',
        '',
        ''
      ],
      numItems: 3,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '739 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+42% Cold Damage'
        },
        {
          label: '+30% Frost Damage'
        },
        {
          label: '+290 Health'
        },
        {
          label: '+36 Offensive Ability'
        },
        {
          label: '40% Cold Resistance'
        },
        {
          label: '20% Pierce Resistance'
        },
        {
          label: '+2 to Trozan\'s Sky Shard',
          extras: 'Tier 3 Arcanist skill'
        },
        {
          label: '+2 to Dual Blades',
          extras: 'Tier 1 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Howling Wind',
        description: 'Surrounds you with a blistering wind that chills your enemies to the bone.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '6 Second Duration'
          },
          {
            label: '20 Seconds Skill Recharge'
          },
          {
            label: '3.5 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '7866 Cold Damage'
          },
          {
            label: '13680 Frost Damage over 2 Seconds'
          }
        ],
        trigger: '15% Chance when hit'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/rings\/c023_ring.dbr',
      icon: 'c21_ring_chillsurge_ring.tex.png',
      quality: 'Blue',
      name: 'Chillsurge Ring',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearaccessories\/rings\/c023_ring.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+30% Cold Damage'
        },
        {
          label: '+20 Spirit'
        },
        {
          label: '+20 Offensive Ability'
        },
        {
          label: '25% Cold Resistance'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Flash Freeze',
        description: 'A flash of pure cold freezes nearby enemies. This spell freezes enemies for its duration.' +
        ' Enemies resistant to freeze effects will suffer a reduced duration.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '3 Second Duration'
          },
          {
            label: '8 Seconds Skill Recharge'
          },
          {
            label: '7 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '399 Cold Damage'
          },
          {
            label: '406 Frost Damage over 2 Seconds'
          }
        ],
        trigger: '100% Chance when Hit by Melee Attacks'
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/waist\/c008_waist.dbr',
      icon: 'c08_waist_chthonian_thread_sash.tex.png',
      quality: 'Blue',
      name: 'Chthonian Thread Sash',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearaccessories\/waist\/c008_waist.dbr',
        '',
        '',
        ''
      ],
      numItems: 4,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '45 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+30% Chaos Damage'
        },
        {
          label: '+21% Vitality Damage'
        },
        {
          label: '+5% Casting Speed'
        },
        {
          label: '22% Chaos Resistance'
        },
        {
          label: '32% Fire Resistance'
        },
        {
          label: '+2 to Destruction',
          extras: 'Tier 6 Occultist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Lifestealer Nova',
        description: 'A surge of necrotic energies rips the life from nearby enemies and transfers it to you.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '3 Seconds Skill Recharge'
          },
          {
            label: '6 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '22144 Vitality Damage'
          },
          {
            label: '100% of Attack Damage converted to Health'
          }
        ],
        trigger: '33% Chance on a critical attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearhead\/c029_head.dbr',
      icon: 'c029_head.tex.png',
      quality: 'Blue',
      name: 'Circlet of Burning Rage',
      socket: '',
      level: 65,
      url: [
        0,
        'records\/items\/gearhead\/c029_head.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '629 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+36% Fire Damage'
        },
        {
          label: '+65% Burn Damage'
        },
        {
          label: '+3.4 Energy Regenerated per second'
        },
        {
          label: '+36 Offensive Ability'
        },
        {
          label: '15% Chaos Resistance'
        },
        {
          label: '29% Fire Resistance'
        },
        {
          label: '+2 to Blackwater Cocktail',
          extras: 'Tier 2 Demolitionist skill'
        },
        {
          label: '+2 to Thermite Mine',
          extras: 'Tier 7 Demolitionist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Blackwater Cocktail',
        description: 'Blackwater Cocktails were concocted when more conventional explosives ran low. ' +
        'These improvised fire bombs leave a self-fueled blaze upon impact and are particularly effective against tightly packed enemies.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '4 Second Duration'
          },
          {
            label: '0.5 Seconds Skill Recharge'
          },
          {
            label: '1 Projectile'
          }
        ],
        bodyStats: [
          {
            label: '42-78 Fire Damage'
          },
          {
            label: '192 Burn Damage over 3 Seconds'
          },
          {
            label: '5 Meter Radius'
          },
          {
            label: '1 Projectile'
          }
        ],
        trigger: '5% Chance on attack'
      }
    },
    {
      baseRecord: 'records\/items\/geartorso\/c016_torso.dbr',
      icon: 'c016_torso.tex.png',
      quality: 'Blue',
      name: 'Cloth of Unspeakable Invocation',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/geartorso\/c016_torso.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '459 Armor'
        }
      ],
      bodyStats: [
        {
          label: '1-8 Chaos Damage'
        },
        {
          label: '+35% Chaos Damage'
        },
        {
          label: '+20% Vitality Damage'
        },
        {
          label: '+36 Spirit'
        },
        {
          label: '+5.8 Energy Regenerated per second'
        },
        {
          label: '+25% Energy Regenerated per second'
        },
        {
          label: '+2 to Doom Bolt',
          extras: 'Tier 8 Occultist skill'
        },
        {
          label: '+2 to Decomposition',
          extras: 'Tier 5 Necromancer skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/focus\/d003_focus.dbr',
      icon: 'd003_focus.tex.png',
      quality: 'Epic',
      name: 'Codex of Lies',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearweapons\/focus\/d003_focus.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+75% Aether Damage'
        },
        {
          label: '+75% Chaos Damage'
        },
        {
          label: '+4.5 Energy Regenerated per second'
        },
        {
          label: '+10% Crit Damage'
        },
        {
          label: '-16% Skill Cooldown Reduction'
        },
        {
          label: '+2 to Curse of Frailty',
          extras: 'Tier 1 Occultist skill'
        },
        {
          label: '+3 to Albrecht\'s Aether Ray',
          extras: 'Tier 6 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Mark of the Deceiver',
        description: 'Mark your foes with a debilitating curse that overwhelms their senses.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '5 Second Duration'
          },
          {
            label: '3 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '103 Aether Damage'
          },
          {
            label: '103 Chaos Damage'
          }
        ],
        trigger: '10% Chance on attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/caster\/c009_scepter.dbr',
      icon: 'c009_scepter001.tex.png',
      quality: 'Blue',
      name: 'Coldsnap',
      socket: '',
      level: 40,
      url: [
        0,
        'records\/items\/gearweapons\/caster\/c009_scepter.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '40-43 Cold Damage'
        }
      ],
      bodyStats: [
        {
          label: '+76% Cold Damage'
        },
        {
          label: '+64% Frost Damage'
        },
        {
          label: '+1.4 Energy Regenerated per second'
        },
        {
          label: '+24 Offensive Ability'
        },
        {
          label: '+2 to Absolute Zero',
          extras: 'Tier 5 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearhands\/d009_hands.dbr',
      icon: 'd009_hands.tex.png',
      quality: 'Epic',
      name: 'Colossal Grasp',
      socket: '',
      level: 75,
      url: [
        0,
        'records\/items\/gearhands\/d009_hands.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '1012 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+30% Physical Damage'
        },
        {
          label: '+30% Internal Trauma Damage'
        },
        {
          label: '+78 Defensive Ability'
        },
        {
          label: '+9 Health\/s'
        },
        {
          label: '+15% Health\/s'
        },
        {
          label: '+36 Physique'
        },
        {
          label: '5% Physical Resistance'
        },
        {
          label: '+3 to Decorated Soldier',
          extras: 'Tier 6 Soldier skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Colossal Might',
        description: 'Mastery of shield combat has honed you into a lethal and unstoppable weapon. This is a shield passive bonus.',
        level: 1,
        petStats: [

        ],
        headerStats: [

        ],
        bodyStats: [
          {
            label: '15% Chance of 170-231 Physical Damage'
          },
          {
            label: '+150% Physical Damage'
          },
          {
            label: '+300% Internal Trauma Damage'
          },
          {
            label: '+8% Attack Speed'
          }
        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/waist\/c010_waist.dbr',
      icon: 'c10_waist_cord_of_the_ancestor.tex.png',
      quality: 'Blue',
      name: 'Cord of the Ancestor',
      socket: '',
      level: 45,
      url: [
        0,
        'records\/items\/gearaccessories\/waist\/c010_waist.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '50 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+30% Cold Damage'
        },
        {
          label: '+45% Vitality Damage'
        },
        {
          label: '+33% Constitution'
        },
        {
          label: '50% Energy Absorption From Enemy Spells'
        },
        {
          label: '+19 Spirit'
        },
        {
          label: '+15 Offensive Ability'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/axe1h\/c012_axe.dbr',
      icon: 'c012_axe.tex.png',
      quality: 'Blue',
      name: 'Corpse Desecrator',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearweapons\/axe1h\/c012_axe.dbr',
        '',
        '',
        ''
      ],
      numItems: 3,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '10 Vitality Damage'
        },
        {
          label: '60-98 Physical Damage'
        },
        {
          label: '48 Vitality Decay Damage over 3 Seconds'
        },
        {
          label: '+55% Vitality Damage'
        },
        {
          label: '+45% Vitality Decay Damage'
        },
        {
          label: '+36 Offensive Ability'
        },
        {
          label: '+15% Damage to Beastkin'
        },
        {
          label: '15% Physical Damage converted to Vitality'
        },
        {
          label: '+2 to Bone Harvest',
          extras: 'Tier 2 Necromancer skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearhead\/c022_head.dbr',
      icon: 'c022_head.tex.png',
      quality: 'Blue',
      name: 'Cowl of the Paragon',
      socket: '',
      level: 65,
      url: [
        0,
        'records\/items\/gearhead\/c022_head.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '627 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+40% Aether Damage'
        },
        {
          label: '+44% Elemental Damage'
        },
        {
          label: '+33 Spirit'
        },
        {
          label: '+3.3 Energy Regenerated per second'
        },
        {
          label: '+35 Offensive Ability'
        },
        {
          label: '26% Vitality Resistance'
        },
        {
          label: '+1 to All Skills in Arcanist'
        },
        {
          label: '+1 to Panetti\'s Replicating Missile',
          extras: 'Tier 1 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/shields\/c007_shield.dbr',
      icon: 'c007_shield.tex.png',
      quality: 'Blue',
      name: 'Crest of the Black Legion',
      socket: '',
      level: 42,
      url: [
        0,
        'records\/items\/gearweapons\/shields\/c007_shield.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '32% Chance to Block 550 Damage (100% Absorption)'
        }
      ],
      bodyStats: [
        {
          label: '98 Physical Damage'
        },
        {
          label: '+15% Physical Damage'
        },
        {
          label: '154 Pierce Retaliation'
        },
        {
          label: '0.8 second Block Recovery'
        },
        {
          label: '+30 Defensive Ability'
        },
        {
          label: '31% Fire Resistance'
        },
        {
          label: '11% Physical Resistance'
        },
        {
          label: '+2 to Decorated Soldier',
          extras: 'Tier 6 Soldier skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/faction\/booster\/boosthostile_outlaw_c01.dbr',
      icon: 'faction_outlaws_wantedposter01.tex.png',
      quality: 'Blue',
      name: 'Cronley\'s Warrant',
      socket: '',
      level: 0,
      url: [
        0,
        'records\/items\/faction\/booster\/boosthostile_outlaw_c01.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [

      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/axe1h\/c001_axe.dbr',
      icon: 'c001_axe.tex.png',
      quality: 'Blue',
      name: 'Cruel Edge',
      socket: '',
      level: 14,
      url: [
        0,
        'records\/items\/gearweapons\/axe1h\/c001_axe.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '15% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '35-44 Physical Damage'
        },
        {
          label: '10% Chance of +200% Physical Damage'
        },
        {
          label: '+18 Offensive Ability'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/focus\/c002_focus.dbr',
      icon: 'c002_focus.tex.png',
      quality: 'Blue',
      name: 'Crushing Will',
      socket: '',
      level: 29,
      url: [
        0,
        'records\/items\/gearweapons\/focus\/c002_focus.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '5 Elemental Damage'
        },
        {
          label: '+40% Elemental Damage'
        },
        {
          label: '+14 Spirit'
        },
        {
          label: '+3 Energy Regenerated per second'
        },
        {
          label: '+20% Casting Speed'
        },
        {
          label: '-16% Skill Cooldown Reduction'
        },
        {
          label: '+1 to Iskandra\'s Elemental Exchange',
          extras: 'Tier 1 Arcanist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/swords1h\/d002_sword.dbr',
      icon: 'd002_sword.tex.png',
      quality: 'Epic',
      name: 'Crystallum',
      socket: '',
      level: 58,
      url: [
        0,
        'records\/items\/gearweapons\/swords1h\/d002_sword.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '32% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '2-18 Lightning Damage'
        },
        {
          label: '65-106 Physical Damage'
        },
        {
          label: '+60% Lightning Damage'
        },
        {
          label: '+60% Electrocute Damage'
        },
        {
          label: '+18% Attack Speed'
        },
        {
          label: '10% Elemental Resistance'
        },
        {
          label: '18% Lightning Resistance'
        },
        {
          label: '15% Physical Damage converted to Lightning'
        },
        {
          label: '+2 to Storm Touched',
          extras: 'Tier 7 Shaman skill'
        },
        {
          label: '+2 to Stormcaller\'s Pact',
          extras: 'Tier 9 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Chain Lightning',
        description: 'Immense electric force emanates from you, cascading to additional enemies.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '2 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '259-851 Lightning Damage'
          },
          {
            label: '100% Chance of affecting up to 5 targets within 6 Meters'
          }
        ],
        trigger: '10% Chance on attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/guns2h\/c003_gun2h.dbr',
      icon: 'c003_gun2h.tex.png',
      quality: 'Blue',
      name: 'Curse of Burrwitch',
      socket: '',
      level: 25,
      url: [
        0,
        'records\/items\/gearweapons\/guns2h\/c003_gun2h.dbr',
        '',
        '',
        ''
      ],
      numItems: 3,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '30% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '4-22 Chaos Damage'
        },
        {
          label: '90-107 Physical Damage'
        },
        {
          label: '+40% Chaos Damage'
        },
        {
          label: '+45 Offensive Ability'
        },
        {
          label: '45% Physical Damage converted to Chaos'
        },
        {
          label: '+3 to Curse of Frailty',
          extras: 'Tier 1 Occultist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/shields\/d008_shield.dbr',
      icon: 'd008_shield.tex.png',
      quality: 'Epic',
      name: 'Dawnbreaker\'s Duty',
      socket: '',
      level: 68,
      url: [
        0,
        'records\/items\/gearweapons\/shields\/d008_shield.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '35% Chance to Block 1044 Damage (100% Absorption)'
        }
      ],
      bodyStats: [
        {
          label: '35 Lightning Damage'
        },
        {
          label: '100 Physical Damage'
        },
        {
          label: '480 Lightning Retaliation'
        },
        {
          label: '1 second Block Recovery'
        },
        {
          label: '+780 Health'
        },
        {
          label: '24% Elemental Resistance'
        },
        {
          label: '+42% Total Retaliation Damage'
        },
        {
          label: '-6% Skill Cooldown Reduction'
        },
        {
          label: '15% Physical Damage converted to Lightning'
        },
        {
          label: '+2 to War Cry',
          extras: 'Tier 5 Soldier skill'
        },
        {
          label: '+2 to Tenacity of the Boar',
          extras: 'Tier 4 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Lightning Nova',
        description: 'Lightning energy surges within you before erupting in deadly force.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '1.8 Seconds Skill Recharge'
          },
          {
            label: '4.75 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '357-830 Lightning Damage'
          }
        ],
        trigger: '15% Chance on attacking'
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/blunt1h\/d006_blunt.dbr',
      icon: 'd006_blunt.tex.png',
      quality: 'Epic',
      name: 'Dawnbreaker\'s Sledge',
      socket: '',
      level: 68,
      url: [
        0,
        'records\/items\/gearweapons\/blunt1h\/d006_blunt.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '9-20 Lightning Damage'
        },
        {
          label: '24-202 Physical Damage'
        },
        {
          label: '+5% Health'
        },
        {
          label: '+25% Shield Damage Blocked'
        },
        {
          label: '+35% Total Retaliation Damage'
        },
        {
          label: '15% Physical Damage converted to Lightning'
        },
        {
          label: '+2 to Savagery',
          extras: 'Tier 1 Shaman skill'
        },
        {
          label: '+2 to Heart of the Wild',
          extras: 'Tier 4 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Lightning Bolt',
        description: 'A touch of Ultos\' wrath rips through the sky to shock your foes.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '2.5 Seconds Skill Recharge'
          },
          {
            label: '2 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '259-1235 Lightning Damage'
          },
          {
            label: 'Stun target for 1 Seconds'
          }
        ],
        trigger: '15% Chance on attack'
      }
    },
    {
      baseRecord: 'records\/items\/gearshoulders\/c014_shoulder.dbr',
      icon: 'c014_shoulder.tex.png',
      quality: 'Blue',
      name: 'Dawnguard Epaulets',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearshoulders\/c014_shoulder.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '1068 Armor'
        }
      ],
      bodyStats: [
        {
          label: '42-424 Lightning Retaliation'
        },
        {
          label: '+220 Health'
        },
        {
          label: '33% Elemental Resistance'
        },
        {
          label: '5% Physical Resistance'
        },
        {
          label: '+33% Total Retaliation Damage'
        },
        {
          label: '+2 to Tenacity of the Boar',
          extras: 'Tier 4 Shaman skill'
        },
        {
          label: '+2 to Counter Strike',
          extras: 'Tier 8 Soldier skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearhands\/c015_hands.dbr',
      icon: 'c015_hands.tex.png',
      quality: 'Blue',
      name: 'Dawnguard Gauntlets',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearhands\/c015_hands.dbr',
        '',
        '',
        ''
      ],
      numItems: 7,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '947 Armor'
        }
      ],
      bodyStats: [
        {
          label: '42-424 Lightning Retaliation'
        },
        {
          label: '+220 Health'
        },
        {
          label: '26% Bleeding Resistance'
        },
        {
          label: '5% Physical Resistance'
        },
        {
          label: '+3 to Savagery',
          extras: 'Tier 1 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearhead\/c023_head.dbr',
      icon: 'c023_head.tex.png',
      quality: 'Blue',
      name: 'Dawnguard Helm',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/gearhead\/c023_head.dbr',
        '',
        '',
        ''
      ],
      numItems: 7,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '1068 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+46% Lightning Damage'
        },
        {
          label: '42-424 Lightning Retaliation'
        },
        {
          label: '+265 Health'
        },
        {
          label: '24% Elemental Resistance'
        },
        {
          label: '5% Physical Resistance'
        },
        {
          label: '+2 to Savagery',
          extras: 'Tier 1 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/geartorso\/c021_torso.dbr',
      icon: 'c021_torso.tex.png',
      quality: 'Blue',
      name: 'Dawnguard Plate',
      socket: '',
      level: 72,
      url: [
        0,
        'records\/items\/geartorso\/c021_torso.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '1218 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+46% Lightning Damage'
        },
        {
          label: '42-424 Lightning Retaliation'
        },
        {
          label: '+350 Health'
        },
        {
          label: '35% Bleeding Resistance'
        },
        {
          label: '30% Acid Resistance'
        },
        {
          label: 'Increases Armor by 6%'
        },
        {
          label: '+2 to Military Conditioning',
          extras: 'Tier 2 Soldier skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/shields\/c009_shield.dbr',
      icon: 'c009_shield.tex.png',
      quality: 'Blue',
      name: 'Dawnshard Bulwark',
      socket: '',
      level: 75,
      url: [
        0,
        'records\/items\/gearweapons\/shields\/c009_shield.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '32% Chance to Block 868 Damage (100% Absorption)'
        }
      ],
      bodyStats: [
        {
          label: '10 Elemental Damage'
        },
        {
          label: '127 Physical Damage'
        },
        {
          label: '+45% Elemental Damage'
        },
        {
          label: '320-405 Fire Retaliation'
        },
        {
          label: '0.9 second Block Recovery'
        },
        {
          label: '+25% Light Radius'
        },
        {
          label: '+44 Physique'
        },
        {
          label: '38% Elemental Resistance'
        },
        {
          label: '-12% Skill Cooldown Reduction'
        },
        {
          label: '+2 to Temper',
          extras: 'Tier 5 Demolitionist skill'
        },
        {
          label: '+2 to Aura of Censure',
          extras: 'Tier 9 Inquisitor skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Solar Flare',
        description: 'Blinding light erupts from you, confounding your foes.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '3 Seconds Skill Recharge'
          },
          {
            label: '6 Meter Radius'
          }
        ],
        bodyStats: [

        ],
        trigger: '10% Chance when blocking'
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/blunt1h\/d001_blunt.dbr',
      icon: 'd001_blunt.tex.png',
      quality: 'Epic',
      name: 'Death Omen',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearweapons\/blunt1h\/d001_blunt.dbr',
        '',
        '',
        ''
      ],
      numItems: 3,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '9 Vitality Damage'
        },
        {
          label: '54-146 Physical Damage'
        },
        {
          label: '+55% Vitality Damage'
        },
        {
          label: '+55% Vitality Decay Damage'
        },
        {
          label: '+12% Attack Speed'
        },
        {
          label: '+300 Health'
        },
        {
          label: '6% of Attack Damage converted to Health'
        },
        {
          label: '15% Physical Damage converted to Vitality'
        },
        {
          label: '+2 to Execution',
          extras: 'Tier 9 Nightblade skill'
        },
        {
          label: '+3 to Curse of Frailty',
          extras: 'Tier 1 Occultist skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Mark of Death',
        description: 'Mark your foe for an imminent death.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '5 Second Duration'
          },
          {
            label: '2 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '257 Vitality Damage'
          },
          {
            label: '20 Life Reduction Damage'
          },
          {
            label: '20% Reduction to Enemy\'s Health'
          }
        ],
        trigger: ' '
      }
    },
    {
      baseRecord: 'records\/items\/gearaccessories\/necklaces\/d008_necklace.dbr',
      icon: 'd08_necklace_deathbound_amethyst.tex.png',
      quality: 'Epic',
      name: 'Deathbound Amethyst',
      socket: '',
      level: 65,
      url: [
        0,
        'records\/items\/gearaccessories\/necklaces\/d008_necklace.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+48% Cold Damage'
        },
        {
          label: '+36% Vitality Damage'
        },
        {
          label: '+48% Frost Damage'
        },
        {
          label: '+36% Vitality Decay Damage'
        },
        {
          label: '+6% Casting Speed'
        },
        {
          label: '20% Cold Resistance'
        },
        {
          label: '5% to Maximum Cold Resistance'
        },
        {
          label: '15% Vitality Resistance'
        },
        {
          label: '5% to Maximum Vitality Resistance'
        },
        {
          label: '10% Physical Damage converted to Vitality'
        },
        {
          label: '+3 to Heart Seeker',
          extras: 'Tier 4 Nightblade skill'
        },
        {
          label: '+2 to Bone Harvest',
          extras: 'Tier 2 Necromancer skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Deathbound Might',
        description: 'Become empowered by the souls you seek to claim.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '6 Second Duration'
          },
          {
            label: '10 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '44 Cold Damage'
          },
          {
            label: '32-57 Vitality Damage'
          },
          {
            label: '+300% Cold Damage'
          },
          {
            label: '+300% Vitality Damage'
          },
          {
            label: '+50% Energy Regenerated per second'
          }
        ],
        trigger: '15% Chance on attacking'
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/guns1h\/d006_gun1h.dbr',
      icon: 'd006_gun1h.tex.png',
      quality: 'Epic',
      name: 'Deathdealer\'s Sidearm',
      socket: '',
      level: 75,
      url: [
        0,
        'records\/items\/gearweapons\/guns1h\/d006_gun1h.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '33% Armor Piercing'
        }
      ],
      bodyStats: [
        {
          label: '52-72 Physical Damage'
        },
        {
          label: '+70% Physical Damage'
        },
        {
          label: '+70% Internal Trauma Damage'
        },
        {
          label: '+13% Attack Speed'
        },
        {
          label: '22% Pierce Resistance'
        },
        {
          label: '5% of Attack Damage converted to Health'
        },
        {
          label: '+1 to All Skills in Soldier'
        },
        {
          label: '+1 to Cadence',
          extras: 'Tier 1 Soldier skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Exterminate',
        description: 'End your foe with a well-aimed shot.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '10% Chance to be Used'
          },
          {
            label: '5 Seconds Skill Recharge'
          },
          {
            label: '1 Projectile'
          }
        ],
        bodyStats: [
          {
            label: '33 Life Reduction Damage'
          },
          {
            label: '214-305 Physical Damage'
          },
          {
            label: '33% Reduction to Enemy\'s Health'
          },
          {
            label: '1 Projectile'
          },
          {
            label: '+300% Weapon Damage'
          }
        ],
        trigger: null
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/focus\/c017_focus.dbr',
      icon: 'c017_focus.tex.png',
      quality: 'Blue',
      name: 'Deathlord\'s Tome',
      socket: '',
      level: 75,
      url: [
        0,
        'records\/items\/gearweapons\/focus\/c017_focus.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '+100% Vitality Damage'
        },
        {
          label: '+304 Health'
        },
        {
          label: '+5.3 Energy Regenerated per second'
        },
        {
          label: '25% Vitality Resistance'
        },
        {
          label: '+8% Crit Damage'
        },
        {
          label: '5% of Attack Damage converted to Health'
        },
        {
          label: '-16% Skill Cooldown Reduction'
        },
        {
          label: '+2 to Sigil of Consumption',
          extras: 'Tier 3 Occultist skill'
        },
        {
          label: '+2 to Wendigo Totem',
          extras: 'Tier 5 Shaman skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Lifestealer Nova',
        description: 'A surge of necrotic energies rips the life from nearby enemies and transfers it to you.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '3 Seconds Skill Recharge'
          },
          {
            label: '6 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '22144 Vitality Damage'
          },
          {
            label: '100% of Attack Damage converted to Health'
          }
        ],
        trigger: '33% Chance on a critical attack'
      }
    },
    {
      baseRecord: 'records\/items\/geartorso\/d016_torso.dbr',
      icon: 'd016_torso.tex.png',
      quality: 'Epic',
      name: 'Deathmarked Jacket',
      socket: '',
      level: 75,
      url: [
        0,
        'records\/items\/geartorso\/d016_torso.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '1071 Armor'
        }
      ],
      bodyStats: [
        {
          label: '8 Cold Damage'
        },
        {
          label: '+44% Cold Damage'
        },
        {
          label: '+44% Frost Damage'
        },
        {
          label: '+46 Offensive Ability'
        },
        {
          label: '21% Chaos Resistance'
        },
        {
          label: '25% Pierce Resistance'
        },
        {
          label: '+20% Damage to Human'
        },
        {
          label: '+2 to Shadow Strike',
          extras: 'Tier 3 Nightblade skill'
        },
        {
          label: '+2 to Nidalla\'s Justifiable Ends',
          extras: 'Tier 6 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearlegs\/d003_legs.dbr',
      icon: 'd003_legs.tex.png',
      quality: 'Epic',
      name: 'Deathwhisper Leggings',
      socket: '',
      level: 58,
      url: [
        0,
        'records\/items\/gearlegs\/d003_legs.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '994 Armor'
        }
      ],
      bodyStats: [
        {
          label: '+38% Cold Damage'
        },
        {
          label: '+30% Pierce Damage'
        },
        {
          label: '+38% Frost Damage'
        },
        {
          label: '+4% Chance to Avoid Melee Attacks'
        },
        {
          label: '+26 Offensive Ability'
        },
        {
          label: '+5% Movement Speed'
        },
        {
          label: '12% Chaos Resistance'
        },
        {
          label: '3% Physical Resistance'
        },
        {
          label: '+2 to Amarasta\'s Blade Burst',
          extras: 'Tier 1 Nightblade skill'
        },
        {
          label: '+2 to Lethal Assault',
          extras: 'Tier 4 Nightblade skill'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Blade Burst',
        description: 'Foes who dare strike you will face Amarasta\'s wrath.',
        level: 1,
        petStats: [

        ],
        headerStats: [
          {
            label: '3 Seconds Skill Recharge'
          },
          {
            label: '4 Meter Radius'
          }
        ],
        bodyStats: [
          {
            label: '228-362 Pierce Damage'
          },
          {
            label: '430 Frost Damage over 2 Seconds'
          },
          {
            label: '+25% Weapon Damage'
          }
        ],
        trigger: '100% Chance when hit'
      }
    },
    {
      baseRecord: 'records\/items\/gearweapons\/shields\/c001_shield.dbr',
      icon: 'c001_shield.tex.png',
      quality: 'Blue',
      name: 'Defender of Devil\'s Crossing',
      socket: '',
      level: 17,
      url: [
        0,
        'records\/items\/gearweapons\/shields\/c001_shield.dbr',
        '',
        '',
        ''
      ],
      numItems: 1,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [
        {
          label: '26% Chance to Block 225 Damage (100% Absorption)'
        }
      ],
      bodyStats: [
        {
          label: '60 Physical Damage'
        },
        {
          label: '0.6 second Block Recovery'
        },
        {
          label: '+35 Defensive Ability'
        },
        {
          label: 'Increases Armor by 10%'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: null
    },
    {
      baseRecord: 'records\/items\/gearweapons\/blunt1h\/c009_blunt.dbr',
      icon: 'c009_blunt.tex.png',
      quality: 'Blue',
      name: 'Defiance',
      socket: '',
      level: 50,
      url: [
        0,
        'records\/items\/gearweapons\/blunt1h\/c009_blunt.dbr',
        '',
        '',
        ''
      ],
      numItems: 2,
      type: 2,
      buddies: [

      ],
      hasRecipe: false,
      headerStats: [

      ],
      bodyStats: [
        {
          label: '10 Aether Damage'
        },
        {
          label: '1-21 Chaos Damage'
        },
        {
          label: '62-114 Physical Damage'
        },
        {
          label: '+40% Aether Damage'
        },
        {
          label: '+40% Chaos Damage'
        },
        {
          label: '+25 Defensive Ability'
        },
        {
          label: '+290 Health'
        }
      ],
      petStats: [

      ],
      greenRarity: 0,
      skill: {
        name: 'Aether Ward',
        description: 'Embrace the powers of the great aether, gaining resistance to the arcane and a strengthened resolve.',
        level: 0,
        petStats: [

        ],
        headerStats: [
          {
            label: '8 Second Duration'
          },
          {
            label: '56 Seconds Skill Recharge'
          }
        ],
        bodyStats: [
          {
            label: '30% Aether Resistance'
          },
          {
            label: '100% Reduced Petrify Duration'
          },
          {
            label: '100% Reduced Entrapment Duration'
          }
        ],
        trigger: '30% Chance when hit'
      }
    }
  ];

  return dummyData;
};

export function mapStateToProps({items}: ApplicationState, {label}: MagicButtonProps): MagicButtonState {
  return {
    count: 1,
    items: addTodo3(''),
    label: label
  };
}

export function mapDispatchToProps(dispatch: Dispatch<AnyAction>): MagicButtonDispatch {
  return {
    onClick: () => {
      dispatch(setLoadingStatus(true));
      setTimeout(() => dispatch(setMockItems(JSON.stringify(addTodo3('')))), 750);
    }
  };
}

export default connect<MagicButtonState, MagicButtonDispatch, MagicButtonProps>(mapStateToProps, mapDispatchToProps)(Button);
