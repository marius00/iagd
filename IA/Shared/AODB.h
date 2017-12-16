#pragma once

#include <list>
#include <map>
#include <vector>

#define AODB_MAX_NAME_LEN		104
#define AODB_ITEM_EFF_MAXVALS	10

// Different datatypes to load from the original AO database.
enum ResourceType
{
    AODB_TYP_UNKNOWN    = 0x00000000,
    AODB_TYP_ITEM		= 0x000f4254,
    AODB_TYP_TEXTURE	= 0x000f6954,
    AODB_TYP_ICON		= 0x000f6958,
    AODB_TYP_PF			= 0x000f4241,
    AODB_TYP_PFGROUND	= 0x000f4249,
    AODB_TYP_OBJMODEL	= 0x000f424d,	// unsure: object model data? (.biff filenames)
    AODB_TYP_NANO	    = 0x000fde85,
};

// Weapon slots
enum WeaponSlot
{
    AODB_SLOT1_HUD1		= 0x0002,
    AODB_SLOT1_HUD2		= 0x8000,
    AODB_SLOT1_HUD3		= 0x0004,
    AODB_SLOT1_UTILS1	= 0x0008,
    AODB_SLOT1_UTILS2	= 0x0010,
    AODB_SLOT1_UTILS3	= 0x0020,
    AODB_SLOT1_RHAND	= 0x0040,
    AODB_SLOT1_BELT		= 0x0080,
    AODB_SLOT1_LHAND	= 0x0100,
    AODB_SLOT1_DECK1	= 0x0200,
    AODB_SLOT1_DECK2	= 0x0400,
    AODB_SLOT1_DECK3	= 0x0800,
    AODB_SLOT1_DECK4	= 0x1000,
    AODB_SLOT1_DECK5	= 0x2000,
    AODB_SLOT1_DECK6	= 0x4000,
};

// Armor slots
enum ArmorSlot
{
    AODB_SLOT2_NECK			= 0x0002,
    AODB_SLOT2_HEAD			= 0x0004,
    AODB_SLOT2_BACK			= 0x0008,
    AODB_SLOT2_RSHOULDER	= 0x0010,
    AODB_SLOT2_BODY			= 0x0020,
    AODB_SLOT2_LSHOULDER	= 0x0040,
    AODB_SLOT2_RARM			= 0x0080,
    AODB_SLOT2_HANDS		= 0x0100,
    AODB_SLOT2_LARM			= 0x0200,
    AODB_SLOT2_RWRIST		= 0x0400,
    AODB_SLOT2_LEGS			= 0x0800,
    AODB_SLOT2_LWRIST		= 0x1000,
    AODB_SLOT2_RFINGER		= 0x2000,
    AODB_SLOT2_FEET			= 0x4000,
    AODB_SLOT2_LFINGER		= 0x8000,
};

// Implant slots
enum ImplantSlot
{
    AODB_SLOT3_EYE		= 0x0002,
    AODB_SLOT3_HEAD		= 0x0004,
    AODB_SLOT3_EAR		= 0x0008,
    AODB_SLOT3_RARM		= 0x0010,
    AODB_SLOT3_BODY		= 0x0020,
    AODB_SLOT3_LARM		= 0x0040,
    AODB_SLOT3_RWRIST	= 0x0080,
    AODB_SLOT3_WAIST	= 0x0100,
    AODB_SLOT3_LWRIST	= 0x0200,
    AODB_SLOT3_RHAND	= 0x0400,
    AODB_SLOT3_LEGS		= 0x0800,
    AODB_SLOT3_LHAND	= 0x1000,
    AODB_SLOT3_FEET		= 0x4000,
};

// Item type
enum ItemType
{
    AODB_ITEM_MISC      = 0,
    AODB_ITEM_WEAPON    = 1,
    AODB_ITEM_ARMOR     = 2,
    AODB_ITEM_IMPLANT   = 3,
    AODB_ITEM_TEMPLATE  = 4,
    AODB_ITEM_SPIRIT    = 5,
};

// Professions
enum Profession
{
    AODB_PROF_SOLDIER           = 1,
    AODB_PROF_MARTIALARTIST		= 2,
    AODB_PROF_ENGINEER			= 3,
    AODB_PROF_FIXER				= 4,
    AODB_PROF_AGENT				= 5,
    AODB_PROF_ADVENTURER		= 6,
    AODB_PROF_TRADER			= 7,
    AODB_PROF_BUREAUCRAT		= 8,
    AODB_PROF_ENFORCER			= 9,
    AODB_PROF_DOCTOR			= 10,
    AODB_PROF_NANOTECHNICIAN	= 11,
    AODB_PROF_METAPHYSICIST		= 12,
    AODB_PROF_KEEPER			= 14,
    AODB_PROF_SHADE				= 15,
};

// Item flags
enum ItemFlag
{
    FLAG_VISIBLE            = 0x00000001,
    FLAG_MOD_DESC           = 0x00000002,
    FLAG_MOD_NAME           = 0x00000004,
    FLAG_CAN_BE_TEMPLATE    = 0x00000008,
    FLAG_TURN_ON_USE        = 0x00000010,
    FLAG_HAS_MULTIPLE_COUNT = 0x00000020,
    FLAG_LOCKED             = 0x00000040,
    FLAG_OPEN               = 0x00000080,
    FLAG_SOCIAL_ARMOR       = 0x00000100,
    FLAG_TELL_COLLISION     = 0x00000200,
    FLAG_NO_SELECTION_IND   = 0x00000400,
    FLAG_USE_EMPTY_DESTRUCT = 0x00000800,
    FLAG_STATIONARY         = 0x00001000,
    FLAG_REPULSIVE          = 0x00002000,
    FLAG_DEFAULT_TARGET     = 0x00004000,
    FLAG_TEXTURE_OVERRIDE   = 0x00008000,
    FLAG_BUFF_NANO          = 0x00010000,
    FLAG_HAS_ANIMATION      = 0x00020000,
    FLAG_HAS_ROTATION       = 0x00040000,
    FLAG_WANT_COLLISION     = 0x00080000,
    FLAG_WANT_SIGNALS       = 0x00100000,
    FLAG_HAS_SENT_FIRTS_IIR = 0x00200000,
    FLAG_HAS_ENERGY         = 0x00400000,
    FLAG_MIRROR_IN_LEFT_HAND= 0x00800000,
    FLAG_NOT_CLAN           = 0x01000000,
    FLAG_NOT_OMNI           = 0x02000000,
    FLAG_NO_DROP            = 0x04000000,
    FLAG_UNIQUE             = 0x08000000,
    FLAG_CAN_BE_ATTACKED    = 0x10000000,
    FLAG_DISABLE_FALLING    = 0x20000000,
    FLAG_HAS_DAMAGE         = 0x40000000,
    FLAG_DISABLE_COLLISION  = 0x80000000,
};

enum ItemProperty
{
    PROP_CARRY              = 0x00000001,
    PROP_UNKNOWN_1          = 0x00000002,
    PROP_WEAR               = 0x00000004,
    PROP_USE                = 0x00000008,
    PROP_CONFIRM_USE        = 0x00000010,
    PROP_EAT                = 0x00000020,
    PROP_TUTORCHIP          = 0x00000040,
    PROP_UNKNOWN_7          = 0x00000080,
    PROP_BREAK_AND_ENTER    = 0x00000100,
    PROP_STACKABLE          = 0x00000200,
    PROP_NO_AMMO            = 0x00000400,
    PROP_BURST              = 0x00000800,
    PROP_FLING_SHOT         = 0x00001000,
    PROP_FULL_AUTO          = 0x00002000,
    PROP_AIMED_SHOT         = 0x00004000,
    PROP_BOW_SPECIAL        = 0x00008000,
    PROP_UNKNOWN_16         = 0x00010000,
    PROP_SNEAK_ATTACK       = 0x00020000,
    PROP_FAST_ATTACK        = 0x00040000,
    PROP_DISARM_TRAPS       = 0x00080000,
    PROP_AUTO_SELECT        = 0x00100000,
    PROP_APPLY_FRIENDLY     = 0x00200000,
    PROP_APPLY_HOSTILE      = 0x00400000,
    PROP_APPLY_SELF         = 0x00800000,
    PROP_NO_SPLIT           = 0x01000000,
    PROP_BRAWL              = 0x02000000,
    PROP_DIMACH             = 0x04000000,
    PROP_ENABLE_ATTRACTORS  = 0x08000000,
    PROP_OK_WITH_SOCIAL_ARMOR= 0x10000000,
    PROP_PARRY              = 0x20000000,
    PROP_PARRIABLE          = 0x40000000,
    PROP_APPLY_TARGET       = 0x80000000,
};

// Structures
struct ao_item_req
{
    int id;
    int type;
    int attribute;
    int count;
    int op;
    int opmod;
};

struct ao_item_effect
{
    ao_item_effect();

    int hook;
    int type;
    std::list<ao_item_req> reqs;
    int hits;
    int delay;
    int target;
    std::vector<int> values;
    std::string text;
};

struct ao_item
{
    ao_item();

    // aodb
    //unsigned int hash;
    char metatype;
    int  aoid;
    //int  patch;
    int  flags;
    int  props;
    int  ql;
    int  icon;
    std::string name;
    std::string description;

    // aodb_item
    int type;
    int slot;
    int defslot;
    int value;
    int tequip;

    // aodb_nano
    int crystal;
    int ncu;
    int nanocost;
    int school;
    int strain;
    int duration;

    // aodb_weapon
    int multim;
    int multir;
    int tburst;
    int tfullauto;
    int clip;
    int dcrit;
    int atype;
    int mareq;

    // both
    int tattack;
    int trecharge;
    int dmax;
    int dmin;
    int range;
    int dtype;
    int initskill;
    std::map<int,int> attmap;
    std::map<int,int> defmap;

    // other tables
    std::map<int, int> other;
    std::list<ao_item_req>  reqs;
    std::list<ao_item_effect>  effs;
};
