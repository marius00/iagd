#include "stdafx.h"
#include "AOItemParser.h"
#include <boost/assign.hpp>
#include <boost/algorithm/string.hpp>

using namespace boost::assign;
using namespace boost::algorithm;

#define NEXT(v)     v = *((unsigned int*)p); p += 4;
#define NEXTV(v)    unsigned int v; NEXT(v);
#define REMAINING   (bufSize-(p-pBuffer))

static std::map<unsigned short, unsigned char> s_effectkeys = map_list_of
(0x0a,   4) // AO: Random modify [attribute, min, max, ac]
(0x0b,   5) // ??: SL: unknown, shadowlands, unsure of size or purpose
(0x14,   3) // AO: set attribute [attribute, value]
(0x16,   5) // AO: temporary skill modify [skill, modifier, ?, ?, time]
(0x18,   4) // AO: teleport [x,y,z,playfield]
(0x1b,   0) // AO: upload nano [id] (we cheat here (count=0), the value is gotten by pre-effect check)
(0x21,   3) // ??: SL: unknown, unsure of size or purpose
(0x22,   3) // AO: set value [?, key, value]
(0x24,   3) // AO: add skill [value, key]
(0x26,   7) // AO: some graphics effect crap, 7 bytes maybe
(0x28,   4) // AO: save character.. unknown values (4?)
(0x29,   3) // AO: lock skill ?, key, value
(0x2b,   2) // AO: headmesh, unknown, meshid
(0x2d,   2) // AO: backmesh, unknown, meshid
(0x2e,   2) // ??: unknown, 2 bytes likely
(0x2f,   3) // AO: texture, texid, slot?, unknown
(0x34,   0) // AO: text (chat) length, text
(0x35,   2) // AO: modify skill, key, value
(0x3b,   1) // AO: execute nano (self?), id
(0x3e,   0) // AO: text (unknown) length, text
(0x3f,   2) // AO: attractor mesh
(0x41,   0) // AO: text (floating) length, text
(0x44,   2) // AO: change shape, ? ?
(0x47,  10) // AO: summon mob, ? ? ? ?
(0x48,   8) // AO: summon item, ? ? ? ?
(0x49,   4) // ??: unknown, 4 bytes
(0x4a,   1) // AO: execute nano (team?), id
(0x4b,   2) // AO: change status
(0x4c,   2) // AO: restrict, ? ?
(0x4d,   0) // AO: next head model
(0x4e,   0) // AO: previous head model
(0x51,   5) // AO: area effect, key, min, max, type, range
(0x53,   4) // ??: unknown, 4 bytes most likely
(0x54,   4) // ??: unknown, 4 bytes
(0x56,   1) // ??: SL: unknown, unsure of size or purpose
(0x57,   3) // AO: change vision, ? ? ?
(0x5a,   6) // ??: SL: unknown, unsure of size or purpose
(0x5b,   7) // AO: teleport, ? playfield ? ? ?
(0x5e,   0) // AO: refresh model
(0x5f,   2) // AO: area nano, id, range
(0x61,   2) // AO: cast nano, id, chance to cast
(0x64,   0) // AO: open bank
(0x6c,   1) // AO: equip monster weapon, 4byteid
(0x70,	 0)	// ??: Unknown text string
(0x71,   3) // AO: remove nanos under x ncu [ncumax, school, count]
(0x73,   1) // AO: script [id]
(0x75,   0) // AO: Create OR enter apartment
(0x76,   2) // AO: set value key, value
(0x7b,   0) // AO: text (unknown) length, text
(0x7d,   5) // AO: taunt, 0,0,taunt value
(0x7e,   0) // AO: pacify
(0x81,   0) // AO: fear
(0x82,   0) // ??: unknown, zero.. in 12.80 nano data
(0x84,   9) // AO: random spawn item, [chance, id, ql] 9 bytes likely
(0x86,   0) // AO: wipe hate list
(0x87,   0) // AO: charm
(0x88,   0) // AO: daze, zero arguments, only in 'daze spell test'
(0x8a,   0) // AO: Destroy item
(0x8c,   0) // AO: text (dual) len, text, len, text
(0x8d,   1) // AO: Organization type .. 1 byte MAYBE
(0x8e,   0) // AO: text (??) 0, len, text
(0x91,   1) // AO: Create OR enter apartment .. 1 byte MAYBE
(0x92,   0) // AO: zero content, enable flight
(0x93,   2) // AO: Set flag, attribute, bit
(0x94,   2) // AO: Enable feature.. name change gate has it
(0x96,   4) // ??: unknown, could be 4 bytes, or less.. at least 2. present in 11.00 data
(0x98,   0) // AO: warp to last save
(0xa1,   0) // ??: unknown
(0xa2,   0) // AO: summon selected player
(0xa3,   0) // AO: summon team members
(0xaa,   2) // AO: Resistance to nano [strain, percentage]
(0xac,   0) // SL: Save character
(0xae,   0) // ??: unknown, 0, only name giver has it
(0xaf,  16) // AO: Spawn pet, 4byteid, ?, ?
(0xb5,   0) // ??: unknown, zero content
(0xb7,   2) // NW: Org advantage
(0xb9,   2) // AO: Reduce nano strain length, strain, seconds
(0xba,   0) // NW: Shield disabler, notum wars shite, zero content
(0xbd,   1) // AO: Warp pets to user
(0xbe,   4) // SL: Add action
(0xc0,   2) // SL: Modify attribute by percentage
(0xc1,   5) // SL: "Drain hit", attribute, minimum, maximum, armor-attribute, recover %
(0xc3,   3) // SL: Lock perk
(0xc5,   2) // 18.5.3: Set Faction - AOID 297262
(0xc7,   1) // ??: "Monster Sit useitem" - AOID 283566
(0xc8,  11) // 
(0xc9,   2) // ??: SL: unknown
(0xcc,   4) // SL: "Special hit", attribute, minimum, maximum, armor-attribute
(0xce,   2) // 18.5.0 : ?? - AOID 287559 
(0xd4,  13) // ??: SL: unknown
(0xd6,   1) // ??: SL: unknown
(0xd8,   0) // SL: Set anchor
(0xd9,   0) // SL: Recall to anchor
(0xda,   0) // ??: SL: 15.0.6-ep1 text
(0xdd,   8) // ??: ControlHate-Self <unknown... x8>
(0xe4,   9) // 
(0xe5,   0) // 
(0xe6,   4) // 
(0xe7,   4) // 
(0xe8,   2) // ??: AddDefProc-Self <%chance, AOID>
(0xe9,   0) // ??: AOID = 283789
(0xea,   3) // ??: SpawnQuest-Self <quest-id, unknown, unknown>
(0xeb,   2) //
(0xec,   1) // ??: PlayfieldNano-Self <AOID>
(0xed,   1) // ??: SolveQuest-Self <quest-id>
(0xee,   3) // ??: KnockBack-Self <unknown, unknown, unknown> / KnockBack-Target?
(0xef,   0) // 17.10.0: ??
(0xf0,   0) // 18.0.0 : ?? AOID = 278587
(0xf1,   6) // 18.1.0 : ?? Instanced City gate?
(0xf2,   0) // 18.0.1 : ?? 
(0xf3,   3) // 18.1.0 : ?? Instanced City Guest Key Generator
(0xf4,   1) // 18.0.0 : ?? AOID = 280162
(0xf5,   2) // 18.5.0 : ?? AOID = 287559
(0xf6,   2) // 18.5.0 : ?? AOID = 296265
(0xf7,   1) // 18.5.x : Change Gender - AOID = 296265
(0xf8,	 1) // 18.3.x : ?? AOID = 202260
(0xfa,   1) // 18.4.6 : CastNano? AOID = 290265
(0xfc,   0) // Text string
(0xff,   0) // 18.5.0 : string + number - AOID = 292479
;

AOItemParser::AOItemParser(char* pBuffer, unsigned int bufSize)
{
    this->aoid = *((unsigned int*)pBuffer);
	if (this->aoid == 245890) {
		int x = 9;
	}

    char *p = pBuffer + 0x20;
    bool head = true;
    unsigned int ftype = 0; // we need bigger scope for this variable, as it is used for catching lingering functions

    while(p + 4 < pBuffer+bufSize)
    {
        unsigned int key, val;
			
        if(head)
        {
            NEXT(key);
            NEXT(val);
            //debugf(DEBUG_VERBOSE, "In head, got key 0x%x, val 0x%x\n", key, val);

            switch(key)
            {
            case 0x00 :
                //debugf(DEBUG_INFO, "item->flags = %d\n", val);
                this->flags = val;
                break;

            case 0x08 :
                //debugf(DEBUG_INFO, "item->duration = %d\n", val);
                this->duration = val;
                break;

            case 0x17 :
                //debugf(DEBUG_INFO, "property count for item : %d\n", val);
                break;

            case 0x1e :
                //debugf(DEBUG_INFO, "item->props = %d\n", val);
                this->props = val;
                break;

            case 0x36 :
                //debugf(DEBUG_INFO, "item->ql = %d\n", val);
                this->ql = val;
                break;

            case 0x4a :
                //debugf(DEBUG_INFO, "item->value = %d\n", val);
                this->value = val;
                break;

            case 0x4b :
                //debugf(DEBUG_INFO, "item->strain = %d\n", val);
                this->strain = val;
                break;

            case 0x4c :
                //debugf(DEBUG_INFO, "item->type = %d\n", val);
                this->type = val;
                break;

            case 0x4f :
                //debugf(DEBUG_INFO, "item->icon = %d\n", val);
                this->icon = val;
                break;

            case 0x58 :
                //debugf(DEBUG_INFO, "item->defslot = %d\n", val);
                this->defslot = val;
                break;

            case 0x64 :
                //debugf(DEBUG_INFO, "item->mareq = %d\n", val);
                this->mareq = val;
                this->other[key] = val;
                break;

            case 0x65 :
                //debugf(DEBUG_INFO, "item->multim = %d\n", val);
                this->multim = val;
                this->other[key] = val;
                break;

            case 0x86 :
                //debugf(DEBUG_INFO, "item->multir = %d\n", val);
                this->multir = val;
                this->other[key] = val;
                break;

            case 0xd2 :
                //debugf(DEBUG_INFO, "item->trecharge = %d\n", val);
                this->trecharge = val;
                break;

            case 0xd3 :
                //debugf(DEBUG_INFO, "item->tequip = %d\n", val);
                this->tequip = val;
                break;

            case 0xd4 :
                //debugf(DEBUG_INFO, "item->clip = %d\n", val);
                this->clip = val;
                break;

            case 0x11c :
                //debugf(DEBUG_INFO, "item->dcrit = %d\n", val);
                this->dcrit = val;
                break;

            case 0x11d :
                //debugf(DEBUG_INFO, "item->dmax = %d\n", val);
                this->dmax = val;
                break;

            case 0x11e :
                //debugf(DEBUG_INFO, "item->dmin = %d\n", val);
                this->dmin = val;
                break;

            case 0x11f :
                //debugf(DEBUG_INFO, "item->range = %d\n", val);
                this->range = val;
                break;

            case 0x126 :
                //debugf(DEBUG_INFO, "item->tattack = %d\n", val);
                this->tattack = val;
                break;

            case 0x12a :
                //debugf(DEBUG_INFO, "item->slot = %d\n", val);
                this->slot = val;
                break;

            case 0x176 :
                //debugf(DEBUG_INFO, "item->tburst = %d\n", val);
                this->tburst = val;
                this->other[key] = val;
                break;

            case 0x177 :
                //debugf(DEBUG_INFO, "item->tfullauto = %d\n", val);
                this->tfullauto = val;
                this->other[key] = val;
                break;

            case 0x195 :
                //debugf(DEBUG_INFO, "item->school = %d\n", val);
                this->school = val;
                break;

            case 0x197 :
                //debugf(DEBUG_INFO, "item->nanocost = %d\n", val);
                this->nanocost = val;
                break;

            case 0x1a4 :
                //debugf(DEBUG_INFO, "item->atype = %d\n", val);
                this->atype = val;
                break;

            case 0x1b4 :
                //debugf(DEBUG_INFO, "item->dtype = %d\n", val);
                this->dtype = val;
                break;

            case 0x1b8 :
                //debugf(DEBUG_INFO, "item->initskill = %d\n", val);
                this->initskill = val;
                this->other[key] = val;
                break;

            case 0x15 :
                if(val == 0x21)
                {
                    unsigned short lname, linfo;
                    lname = *((unsigned short*)p); p += 2;
                    linfo = *((unsigned short*)p); p += 2;
                    try
                    {
                        this->name = std::string(p, lname);
                    }
                    catch (std::exception &/*e*/)
                    {
                        throw std::exception("Error extracting item name string.");
                    }
                    try
                    {
                        this->description = std::string(p+lname, linfo);
                    }
                    catch (std::exception &/*e*/)
                    {
                        throw std::exception("Error extracting item description string.");
                    }
                    p += lname + linfo;
                    head = false;
                    //debugf(DEBUG_INFO, "Name: %s\n", item->name);
                    // printf("ITEM: %06d %s\n", item->aoid, item->name);
                }
                else
                {
                    this->other[key] = val;
                }
                break;

            default :
                this->other[key] = val;
                break;
            }
        }
        else
        {
            NEXT(key);

            //debugf(DEBUG_VERBOSE, "In body, got key 0x%x\n", key);

            if(key == 0x0)
            {
                continue;
            }
            if(key == 0x2)
            {
                NEXT(ftype);
                NEXTV(count);
                count >>= 10;
                //debugf(DEBUG_INFO, "item: have %d effects for a function of type 0x%x\n", count, ftype);
                while(count--)
                {
                    int fkey = 0;
                    while(fkey == 0) {
                        NEXT(fkey);
                    }
                    if((fkey & 0xcf00) != 0xcf00) {
                        //LOG("While parsing item " << this->aoid << ": Unknown functionkey " << fkey << " found.");
                        //debugf(DEBUG_VERBOSE, "In body, got key 0x%x\n", key);
                        ////assert(false);
                        return;
                    }
                    p = ParseFunctions(p, REMAINING, ftype, fkey & 0xff);
                    if(p == NULL)
                        return; /* Failed, stop item parsing */
                }
            }
            else if(key == 0x16)
            {
                NEXT(val);
                if(val != 0x24) {
                    ////assert(false);
                    return;
                }
                NEXTV(count);
                count >>= 10;
                while(count--)
                {
                    NEXTV(rhook);
                    NEXTV(subcount);
                    subcount >>= 10;
                    p = ParseRequirements(p, REMAINING, this->reqs, subcount, rhook);
                }
            }
            else if(key == 0x4)
            {
                NEXT(val);
                if(val != 0x4) {
                    ////assert(false);
                    return;
                    //DATAFAIL(key);
                }
                NEXTV(count);
                count >>= 10;
                while(count--)
                {
					if (aoid == 99302) {
						int x = 9;
					}
                    std::map<int,int> m;
                    NEXTV(fkey);
                    NEXTV(subcount);
                    subcount >>= 10;
                    while(subcount--)
                    {
                        NEXTV(subkey);
                        NEXTV(subval);
                        m[subkey] = subval;
                    }
                    if(fkey == 0xc){
                        this->attmap = m;
                    }
                    else if(fkey == 0xd) {
                        this->defmap = m;
                    }
                    else if (fkey == 0x03) {
                        //Logger::instance().log(STREAM2STR("AOID " << this->aoid << "had a skillmap, but it was not att or def (0x03)"));
                    }
                    else {
                        //Logger::instance().log(STREAM2STR("AOID " << this->aoid << "had a skillmap, but it was not att or def (0x" << std::hex << fkey << ")"));
                        ////assert(false);
                        //debugf(DEBUG_INFO, "item had a skillmap, but it was not att or def (0x%x)\n", fkey);
                    }
                }
            }
            else if(key == 0x6)
            {
                NEXT(val);
                if(val != 0x1b) {
                    ////assert(false);
                    return;
                    //DATAFAIL(key);
                }
                NEXTV(count);
                count >>= 10;
                while(count--)
                {
                    NEXTV(fkey); // no idea what these are.. usually there's fkey:0xc, fkey:0xd
                    NEXTV(fval); // and this one is always zero
                }
            }
            else if(key == 0xe || key == 0x14)
            {
                NEXT(val);
                NEXTV(count1);
                count1 >>= 10;
                while(count1--)
                {
                    NEXT(val);
                    NEXTV(count2);
                    count2 >>= 10;
                    while(count2--)
                        NEXT(val);
                }
            }
            else if(key == 0x17)
            {
                // gah, stop parsing item here, it's most likely full of shit
                //NEXT(val);
                //NEXTV(count);
                //p += 17 * (count >> 10); // ignore this.. shop inventory afaik
                return;
            }
            else if((key & 0xcf00) == 0xcf00) // lingering function? 
            {
                //debugf(DEBUG_INFO-1, "Found lingering function 0x%x in item %d\n", key, item->aoid);
                p = ParseFunctions(p, REMAINING, ftype, key & 0xff);
                if(p == NULL) {
                    ////assert(false);
                    return;
                    //PARSEFAIL(); /* Failed, stop item parsing */
                }
            }
            else
            {
                // Unknown key found.
                //Logger::instance().log(STREAM2STR("Unknown key found while parsing item database. AOID = " << this->aoid << " KEY = 0x" << std::hex << key));
                //////assert(false);
                //return;
                //DATAFAIL(key);
            }
        }
    }
}


char* AOItemParser::ParseFunctions(char* pBuffer, unsigned int bufSize, unsigned int ftype, unsigned int key)
{
    char *p = pBuffer;
    unsigned int xxx;

    ao_item_effect eff;
    eff.hook   = ftype;
    eff.type   = key;
    //memset(eff.values, 0, sizeof eff.values);

    //debugf(DEBUG_INFO, "Item function 0xcf%02x\n", key);

    NEXT(xxx); 
    //if(xxx != 0) debugf(DEBUG_SERIOUS+1, "%d xxx == %d\n", __LINE__, xxx);
    NEXT(xxx); 
    //if(xxx != 4) debugf(DEBUG_SERIOUS+1, "%d xxx == %d\n", __LINE__, xxx);
    // p += 4*2; // always 0, 4

    NEXTV(cnt);
    if(cnt > 0)
    {
        //debugf(DEBUG_INFO, "Item function got requirements, %d.\n", cnt);
        p = ParseRequirements(p, REMAINING, eff.reqs, cnt, -1);
    }

    NEXT(eff.hits);
    NEXT(eff.delay);
    NEXT(eff.target);
    NEXT(xxx);
    //if(xxx != 9) debugf(DEBUG_SERIOUS+1, "%d xxx == %d\n", __LINE__, xxx);
    // p += 4; // always 9
    //eff.valuecount = 0;

    switch(key)
    {
        /* hack for nano uploads */
    case 0x1b : // upload nano [id]
        eff.target = 2;
        NEXTV(v);
        eff.values.push_back(v);
        //NEXT(eff.values[eff.valuecount++]);
        if (contains(this->name, "Nano Crystal") ||
            contains(this->name, "NanoCrystal") ||
            contains(this->name, ": Startup Crystal - "))
        {
            //nanomap[eff.values[0]].aoid = this->aoid;
            //nanomap[eff.values[0]].ql   = this->ql;
        }
        break;

        /* hack for various text-displaying effects */
    case 0x34 : // chat text, length, text
        p = ParseString(p, REMAINING, eff.text);
        if(key == 0x34) {
            p += 4*5; // skip five
        }
        break;

    case 0x3e : // unknown1 text, length, text
        p = ParseString(p, REMAINING, eff.text);
        break;

    case 0x41 : // floating text, length, text
        p = ParseString(p, REMAINING, eff.text);
        p += 4;   // skip one (MKF: This is probably the number of seconds the text should be visible for?)
        break;

	case 0x70:	// Unknown text. Looks like named event or something?
		{
			p = ParseString(p, REMAINING, eff.text);
			p += 4;	// Skip
		}
		break;

	case 0x7b : // unknown2 text, length, text
        p = ParseString(p, REMAINING, eff.text);
        p += 4*2; // skip two
        break;

    case 0x8c : // dual text, len, text, len, text
        {
            std::string t1, t2;
            p = ParseString(p, REMAINING, t1);
            p = ParseString(p, REMAINING, t2);
            eff.text = t1;
            eff.text += ", ";
            eff.text += t2;
            p += 4; // skip one, probably a timeout value.
        }
        break;

    case 0x8e : // text, len, text
        p = ParseString(p, REMAINING, eff.text);
        break;

    case 0xda : // shadowlands 15.0.6-ep1 text
        p = ParseString(p, REMAINING, eff.text);
        p += 4;   // skip one
        break;

    case 0xe5:  // Unknown text, looks like debug/comment from AO devs.
        // "## Alien_Event_Lander_02 Float StartX 476.0 Float StartZ 90.0 #"
        // "## Unicorn_Event_Lander_02 Float StartX 207.0 Float StartZ 441.0 Float StartY 36.0 #"
        {
            unsigned int v;
            NEXT(v);
            eff.values.push_back(v);    // StartX
            NEXT(v);
            eff.values.push_back(v);    // StartZ
            NEXT(v);
            eff.values.push_back(v);    // StartY
            p = ParseString(p, REMAINING, eff.text);
        }
        break;

    case 0xfc:  // Textstring. Looks like something related to the 10th anniversary? 
        // "The Desert Rider has arrived! Hurry to coordinates..."
        p = ParseString(p, REMAINING, eff.text);
        break;

    case 0xff:  // Textstring followed by number
        p = ParseString(p, REMAINING, eff.text);
        NEXT(v);
        eff.values.push_back(v);
        break;
    }

    std::map<unsigned short, unsigned char>::const_iterator ekey = s_effectkeys.find(key);
    if(ekey == s_effectkeys.end())
    {
        ////assert(false);
        //debugf(DEBUG_SERIOUS, "UNKNOWN FUNCTION <0x%x> <ITEM: %d>\n", key, item->aoid);
        return NULL;
    }
    for(cnt=0; cnt<ekey->second; cnt++) {
        NEXTV(v);
        eff.values.push_back(v);
        //NEXT(eff.values[eff.valuecount++]);
    }

    this->effs.push_back(eff);

    return p;
}


char* AOItemParser::ParseString(char *pBuffer, unsigned int bufSize, std::string &outText)
{
    char *p = pBuffer;
    NEXTV(tlen);

    if (tlen == 0) {
        return p;
    }

    if(tlen > bufSize) {
        ////assert(false);
        //debugf(DEBUG_SERIOUS, "Buffer underrun trying to get %d bytes of text!\n", tlen);
        outText.clear();
        return pBuffer + bufSize;
    }
    //debugf(DEBUG_INFO, "Getting %d bytes of text\n", tlen);

    // If it is a zero terminated string, ignore the tlen
    if (*(p + tlen-1) == 0) {
        outText = p;
    }
    else {
        outText = std::string(p, tlen);
    }
    p += tlen;

    return p;
}


char* AOItemParser::ParseRequirements(char *pBuffer, unsigned int bufSize, std::list<ao_item_req> &reqlist, unsigned int cnt, int rhook)
{
    ao_item_req req;
    char *p = pBuffer, opmod=0, havereq=0, reqid=0;

    //debugf(DEBUG_INFO, "Parsing %d requirements of type %d\n", cnt, rhook);
    while(cnt--)
    {
        if(p+4*3 > pBuffer+bufSize)
        {
            //debugf(DEBUG_SERIOUS, "Buffer underrun while parsing requirements!\n");
            return pBuffer+bufSize;
        }

        NEXTV(skill);
        NEXTV(count);
        NEXTV(op);
        //debugf(DEBUG_INFO, "Got requirement, skill %d, count %d, operator %d\n", skill, count, op);
        if(skill == 0 && count == 0)
        {
            if(op == 0x12) {
                opmod |= op;
            }
            else {
                req.opmod |= opmod|op;
            }
        }
        else {
            if(havereq) {
                reqlist.push_back(req);
            } 
            else {
                havereq = 1;
            }
            req.id = reqid++;
            req.type = rhook;
            req.attribute = skill;
            req.count = count;
            req.op = op;
            req.opmod = 0;
        }
    }
    if(havereq) {
        reqlist.push_back(req);
    }

    return p;
}
