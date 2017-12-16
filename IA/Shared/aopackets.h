#pragma once

namespace AO {

#pragma pack(push, 1)

    struct AoObjectId
    {
        unsigned int low;
        unsigned int high;
    };

    /** 
    * SHIFT F9 info: (adjuster inside HR8)
    *
    * 15.7.3:
    *	Pos: 109.4,149.4,115.0, PF: 3027
    *	Zone: 17, Area: Omni-1 Trade Backyard 8, CharID: 50000:522643323
    *	ServerID: 3104 UTC: Wed Dec 22 16:11:08 2004
    *	Pf Proxy: Model=51100:3027 GroupSelector=0 Subgroup=0 RunningPlayfield=40016:3027
    *	Suppression field at 100%. No fighting possible.
    *	Version: 15.7.3_EP1. MapID: 15.7.3_08.12.2004_14.21.41
    *
    * 16.2.2:
    *  Pos: 109.9,149.8,115.0, PF: 3027
    *  109.9 149.8 y 115.0 3027
    *  Zone: 17, Area: Omni-1 Trade Backyard 8, CharID: 50000:522643323
    *  ServerID: 3064 UTC: Sat Apr 15 12:24:30 2006
    *   51100:3027/0/0/40016:3027
    *  Suppression field at 100%. No fighting possible.
    *  Version: 16.2.2_EP1. MapID: 16.2.2.0_22.03.2006_14.03.16
    *
    * 17.2.0:
    *  Extended location information:
    *  - 107.4, 147.5, 115.0 (107.4 147.5 y 115.0 3027)
    *  - Pf Proxy: Model=51100:3027 GS=0 SG=0 R=3027, resource: 3027
    *  - zone: 17, area: "Omni-1 Trade Backyard 8"
    *  Server id: 1114, character id: 50000:522643323, time: 2007-03-18 17:13:49 (UTC)
    *  Version: 17.2.0_EP1, map id: 17.2.0_14.03.2007_15.23.14, build: 46091.
    *
    * (From RK2)
    *  Extended location information:
    *  - 850.8, 789.1, 38.0 (850.8 789.1 y 38.0 1923801)
    *  - Pf Proxy: Model=51102:4582 GS=3 SG=0 R=1923801, resource: 4582
    *  - zone: 971, area: "ICC Shuttleport"
    *  Server id: 3149, character id: 50000:854759930, time: 2007-02-11 20:12:41 (UTC)
    *  Version: 17.1.1_EP1, map id: 17.1.1_08.02.2007_15.57.44, build: 45960.
    */
    struct Header
    {
        unsigned int	headerid;	// 0xDFDF000A
        unsigned short	unknown1;	// 0x0001		protocol version?
        unsigned short	msgsize;	//				size incl header
        unsigned int	serverid;	// 0x00000C20	ServerID
        unsigned int	charid;		// 0x1F26E77B	== Adjuster

        unsigned int	msgid;		//

        // Target of message?
        AoObjectId		target;		// 0x0000C350	dimension? (50000:<charid>)
    };

	/*struct ClientHeader
    {
		unsigned int	headerid;	// 0x0000000A
		unsigned short	unknown1;	// 0x0000		protocol version?
        unsigned short	msgsize_not_filled;	//				size incl header
		unsigned int	clientid;	// 0x67a6de6a	ClientID?         
        unsigned int	unknown2;	//??ex 00 00 00 02 or 00 00 0b ed  on logoff
		unsigned int	msgid;
        AoObjectId		charId;		// 0x0000C350	(50000:<charid>)
    };*/

	struct InvItemId
	{
		unsigned short	unknown4;//00 00 (part of fromType probably)
		unsigned short	type;//00 68 for inventory etc..
		unsigned short	containerTempId;//the temporary id of the container
		unsigned short	itemSlotId;//The slot id of the item in its container/inventory.
	};

	struct MoveData
	{
		unsigned char	unknown1;//00

		InvItemId		fromItem;

		//unsigned short	unknown2;//00 00 (part of fromType maybe)
		//unsigned short	fromType;
		//unsigned short	fromContainerTempId;// If backpack: the slot id in the inventory that holds the container
		//unsigned short	fromItemSlotId;//The slot id of the item in its container/inventory.
		AoObjectId		toContainer;
		unsigned short	unknown3; //00
		unsigned short	targetSlot; //6f (invalid) except when moving to wear window.
	};

	/*struct MoveOperation
	{
		ClientHeader	header;
		MoveData		moveData;
	};*/

	struct ItemMoved
	{
		Header			header;
		MoveData		moveData;
	};

	/*struct OpenBackpackOperation
	{
		ClientHeader header;
		unsigned char	unknown1;//01
		unsigned int	unknown2;//00 00 00 00
		unsigned int	counter;
		unsigned int	unknown3;//00 00 00 03
		unsigned int	openedbeforeCount; //00 00 00 03
		AoObjectId		owner;//?
		InvItemId		backPack;
		//unsigned short	unknown4;//00 00 (part of fromType probably)
		//unsigned short	fromType;//68 for inventory
		//unsigned short	unknown5;//00 00 (fromContainerTempId probably, but we dont open a backpack from within another backpack)
		//unsigned short	containerTempId;//temp id (temporary)

		//01 00 00 00 00 00 00 00 1b 00 00 00 03 00 00 00 01 00 00 c3 50 67 a6 de 6a 00 00 00 68 00 00 00 43
	};*/

	struct CharacterAction
	{
		//MSG_CHAR_OPERATION
		Header header;
		//00 00 00 00 70 00 00 00 00 00 00 00 68 00 00 00 4e 00 01 3f 5d 00 01 3f 5c 00 00
		unsigned char	unknown1;//01
		unsigned int	operationId;//00 00 00 70 
		unsigned int	unknown3;//00 00 00 00
		InvItemId		itemToDelete;
		AoObjectId		itemId;
		unsigned short	zeroes2; //00 00
	};

	struct GiveToNPC
	{
		Header header;
		unsigned char	unknown1;//00
		unsigned short	operationId;//02??
		AoObjectId		npcID;//??
		unsigned int	direction;//00 00 00 00 if giving, 00 00 00 01 if removing an item
		unsigned int	unknown3;//00 00 00 00
		unsigned int	unknown4;//00 00 00 00
		InvItemId		invItemGiven;
	};

	struct EndNPCTrade
	{
		Header header;
		unsigned char	unknown1;//00
		unsigned short	operationId;//02??
		AoObjectId		npcID;//??
		unsigned int	operation;//00 00 00 00 if closing, 00 00 00 01 if accepting
		unsigned int	unknown2;//00 00 00 00
	};

	struct NPCTradeAcceptBase
	{
		Header header;
		unsigned char	unknown1;//00
		unsigned short	operationId;//02??
		AoObjectId		npcID;//??
		unsigned int	itemCount;
	};

	struct NPCTradeRejectedItem
	{
		AoObjectId		itemid;
		unsigned int	ql;
		unsigned int	unknown1;//49 96 02 d2
	};

	struct NPCTradeAcceptEnd
	{
		unsigned int	unknown1;//00 00 00 00
	};

	struct ServerCharacterAction
	{
		//MSG_CHAR_OPERATION
		Header header;
		//00 00 00 00 70 00 00 00 00 00 00 00 68 00 00 00 4e 00 01 3f 5d 00 01 3f 5c 00 00
		unsigned char	unknown1;//01
		unsigned int	operationId;//00 00 00 70 
		unsigned int	unknown3;//00 00 00 00
		InvItemId		itemToDelete;
		AoObjectId		itemId;
		unsigned short	zeroes2; //00 00
	};

	struct TradeTransaction
	{
		Header header;
		//00 00 00 00 01 05 00 00 c3 50 67 a6 de 6a 00 00 00 68 00 00 00 54
		unsigned char	unknown1;//01
		unsigned int	unknown2;//00 00 00 01 
		unsigned char	operationId;//01=accept, 02=decline,03=?start?, 04=commit,05=add item,06=remove item
		AoObjectId		fromId;
		InvItemId		itemToTrade;
	};

	struct PartnerTradeItem
	{
		//MSG_PARTNER_TRADE
		Header header;
		//00 00 01 d1 35 00 01 d1 35 00 00 00 01 00 00 00 01|00 00 00 55|00 00 00 68 00 00 00 4f |00 00 00 00 00 00 00 00 
		unsigned char	unknown1;//01
		AoObjectId		itemid;
		unsigned int	ql;
		unsigned short	flags;
		unsigned short	stack;
		unsigned int	operationId;//55=Add, 56=remove
		InvItemId		partnerInvItem;
		InvItemId		myInvItemNotUsed; //

	};

	struct BoughtItemFromShop
	{
		Header			header;
		//df df 00 0a 00 01 00 2d 00 00 0b c4 67 a6 de 6a 05 2e 2f 0c 00 00 c3 50 67 a6 de 6a 00 00 00 54 69 00 00 54 69 00 00 00 01 00 00 01 90 
		unsigned char	unknown1;//00
		AoObjectId		itemid;
		unsigned int	ql;
		unsigned short	flags;
		unsigned short	stack;
	};

	struct Backpack
	{
		//TODO: This is probably a key/value container with the bp props.
		//Being lazy and using the order I have seen on ~100 messages.
		//If someone buys a backpack and gets the wrong backpack name before zoneing, there is a bug here!

		Header          header;
		//*00 00 00 00 0b 00 00 c3 50 44 bb 17 ae 
		unsigned char	unknown1; //0x00
		unsigned int	unknown2; //0x00 00 00 0b  always (rk1 and 2)
		AoObjectId		ownerId;
		unsigned int	zone; //id of the zone you're in
		unsigned int	unCons_f424f;
		unsigned int	unZeroes1;
		unsigned char	operationId; //65=bought bp, 01=opened bp, 0e=?, what is got bp in trade?
		unsigned char	invSlot; //or 6f "find a spot" if you buy one
		unsigned int	unknMass;//1b 97 gives short msg (1009*7) , 1f 88 (1009*8) gives long. probably number of key/val pairs following.
		
		unsigned int		unZeroes2;
		unsigned int	flags; //seems to be a bitwise bp flag (probably unique, nodrop etc)

		unsigned int		unknownKey17;//always 0x17!
		unsigned int	itemKey1;

		unsigned int		unknownKey2bd;//always 0x2bd!
		unsigned int	ql;

		unsigned int		unknownKey2be;//always 0x2be!
		unsigned int	itemKeyLow;//I guess

		unsigned int		unknownKey2bf;//always 0x2bf!
		unsigned int	itemKeyHigh;//I guess

		unsigned int		unknownKey19c;//always 0x19c!
		unsigned int		unknown01;//always 0x01!

		//after this we got two different sizes.. based on unknMass I think
	};

    struct Container
    {
        Header          header;
        unsigned int	unknown1;	// 0x01000000 object type?
        unsigned char	numslots;	// 0x15 for a backpack
        unsigned int	unknown2;	// 0x00000003 ??? introduced after 16.1 or so..
        unsigned int	mass;	      // mass? seems related to number of items in backpack. 1009 + 1009*numitems
    };

    struct ContItem						// Size 0x20
    {
        unsigned int	index;
        unsigned short	flags;	// 0x0021 ?? 1=normal item? 2=backpack 32 = nodrop? 34=nodrop bp?
        unsigned short	stack;
        AoObjectId      containerid;
        AoObjectId		itemid;		// low + high id
        unsigned int	ql;
        unsigned int	nullval;
    };

    struct ContEnd
    {
        AoObjectId		containerId;
        unsigned int	tempContainerId;	// Virtual id of the container
        unsigned int	nullval;
    };

    struct Bank                      // size 17
    {
        Header         header;
        char           unknown;
        unsigned int   mass;          // 1009*items+1009
    };

    struct AoString
    {
        unsigned char   strLen;         // Number of bytes allocated to 'str'
        const char      str;            // array of strLen number of bytes.
    };

    struct MobInfo
    {
        Header         header;
        char           unknown[42];
        AoString       characterName;
    };

#pragma pack(pop)

};	// namespace
