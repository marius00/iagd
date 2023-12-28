#pragma once
#include <string>
#include <iostream>
#include <sstream>
#include <vector>
#include <Windows.h>
#include <fstream>

#include "Exports.h"


void* GetProcAddressOrLogToFile(const wchar_t* dll, char* procAddress, bool skipLog = false);

namespace GAME
{
	struct Vec3
	{
		float x;
		float y;
		float z;
	};
	struct Rect
	{
		float top;
		float right;
		float bottom;
		float left;
	};

	struct Color
	{
		float r;
		float g;
		float b;
		float a;
	};

	struct ItemReplicaInfo
	{
		unsigned int id;
		std::basic_string<char, std::char_traits<char>, std::allocator<char> > baseRecord;
		std::basic_string<char, std::char_traits<char>, std::allocator<char> > prefixRecord;
		std::basic_string<char, std::char_traits<char>, std::allocator<char> > suffixRecord;
		unsigned int seed;
		std::basic_string<char, std::char_traits<char>, std::allocator<char> > modifierRecord;
		std::basic_string<char, std::char_traits<char>, std::allocator<char> > materiaRecord;
		std::basic_string<char, std::char_traits<char>, std::allocator<char> > relicBonus;
		unsigned int relicSeed;
		std::basic_string<char, std::char_traits<char>, std::allocator<char> > enchantmentRecord;
		unsigned int enchantmentLevel;
		unsigned int enchantmentSeed;
		std::basic_string<char, std::char_traits<char>, std::allocator<char> > transmuteRecord;
		unsigned int var1;
		Vec3 velocity;
		unsigned int owner;
		unsigned int stackSize;
		unsigned int visiblePlayerId;
		unsigned int droppedPlayerId;
	};
	struct Object { void* dummy; };
	struct Item { void* dummy; };
	struct ItemEquipment { void* dummy; };
	struct GraphicsTexture { void* dummy; };
	struct Player { void* dummy; };
	struct GameEngine { void* dummy; };
	struct GameInfo { void* dummy; };
	struct Engine { void* dummy; };
	struct ObjectManager { void* dummy; };
	struct InventorySack { void* dummy; };
	struct Character { void* dummy; };

	enum GameTextClass
	{
		GameTextClass_Default = 0x0,
		GameTextClass_ItemBanner = 0x1,
		GameTextClass_ItemNameCommon = 0x2,
		GameTextClass_ItemNameMagical = 0x3,
		GameTextClass_ItemNameRare = 0x4,
		GameTextClass_ItemNameEpic = 0x5,
		GameTextClass_ItemNameLegendary = 0x6,
		GameTextClass_ItemNameBroken = 0x7,
		GameTextClass_ItemNamePotion = 0x8,
		GameTextClass_ItemNameRelic = 0x9,
		GameTextClass_ItemNameEnchantment = 0xA,
		GameTextClass_ItemNameQuest = 0xB,
		GameTextClass_ItemNameArtifactFormula = 0xC,
		GameTextClass_ItemNameArtifact = 0xD,
		GameTextClass_ItemNameScroll = 0xE,
		GameTextClass_ItemNameLore = 0xF,
		GameTextClass_ItemDescription = 0x10,
		GameTextClass_ItemBaseStats = 0x11, // 17
		GameTextClass_ItemBonuses = 0x12, // 18
		GameTextClass_ItemRequirements = 0x13,
		GameTextClass_ItemSetName = 0x14,
		GameTextClass_ItemSetDescription = 0x15,
		GameTextClass_ItemSetComponentNotAvailable = 0x16,
		GameTextClass_ItemSetComponentAvailable = 0x17,
		GameTextClass_ItemSetComponentTitle = 0x18,
		GameTextClass_ItemSetBonuses = 0x19,
		GameTextClass_ItemSetBonusesNotAvailable = 0x1A,
		GameTextClass_ItemRelicName = 0x1B,
		GameTextClass_ItemRelicNameDisabled = 0x1C,
		GameTextClass_ItemRelicNumber = 0x1D,
		GameTextClass_ItemRelicDescription = 0x1E,
		GameTextClass_ItemRelicBonus = 0x1F, // 31
		GameTextClass_ItemRelicCompleteTitle = 0x20,
		GameTextClass_ItemDirections = 0x21,
		GameTextClass_ItemSkillHeading = 0x22, // 34
		GameTextClass_ItemSkillName = 0x23, // 35
		GameTextClass_ItemSkillNameSet = 0x24,
		GameTextClass_ItemSkillDescription = 0x25,
		GameTextClass_ItemSkillDescriptionSet = 0x26,
		GameTextClass_ItemSkillStats = 0x27,
		GameTextClass_ItemPetSkillStats = 0x28,
		GameTextClass_ItemEnchantmentName = 0x29,
		GameTextClass_ItemEnchantmentStats = 0x2A,
		GameTextClass_ItemEnchantmentStatsNotAvailable = 0x2B,
		GameTextClass_SkillName = 0x2C,
		GameTextClass_SkillDescription = 0x2D,
		GameTextClass_SkillLevelTitles = 0x2E,
		GameTextClass_SkillStatsCurrent = 0x2F,
		GameTextClass_SkillStatsNext = 0x30,
		GameTextClass_SkillRequirements = 0x31,
		GameTextClass_SkillRequirementsNotMet = 0x32,
		GameTextClass_PetSkillNameCurrent = 0x33,
		GameTextClass_PetSkillNameNext = 0x34,
		GameTextClass_PetSkillStatsCurrent = 0x35,
		GameTextClass_PetSkillStatsNext = 0x36,
		GameTextClass_ArtifactFormulaTitle = 0x37,
		GameTextClass_ArtifactFormulaDescription = 0x38,
		GameTextClass_ArtifactFormulaReagents = 0x39, // 57
		GameTextClass_ArtifactFormulaReagentAvailable = 0x3A, // 58
		GameTextClass_ArtifactFormulaReagentNotAvailable = 0x3B, // 59
		GameTextClass_ArtifactFormulaCostAvailable = 0x3C,
		GameTextClass_ArtifactFormulaCostNotAvailable = 0x3D,
		GameTextClass_ArtifactTitle = 0x3E,
		GameTextClass_ArtifactDescription = 0x3F,
		GameTextClass_ArtifactClass = 0x40,
		GameTextClass_ArtifactBonusTitle = 0x41,
		GameTextClass_ArtifactBonus = 0x42,
		GameTextClass_ArtifactCraft = 0x43,
		GameTextClass_PetBonusTitle = 0x44,
		GameTextClass_PetBonus = 0x45, // 69
		GameTextClass_PetBonusNextTitle = 0x46,
		GameTextClass_PetBonusNext = 0x47,
		GameTextClass_EffectHeading = 0x48,
		GameTextClass_EffectName = 0x49,
		GameTextClass_EffectDescription = 0x4A,
		GameTextClass_EffectStatsCurrent = 0x4B,
		GameTextClass_EffectStatsNext = 0x4C,
		GameTextClass_Debug = 0x4D,
		GameTextClass_GrayStats1 = 0x4E,
		GameTextClass_GrayStats2 = 0x4F,
		GameTextClass_GrayStats3 = 0x50,
	};

	// If this changes, just look up the "GAME::GameTextLine::GameTextLine" export
	struct GameTextLine
	{
		GameTextClass textClass;
		std::wstring text;
		bool needsProcessing;
		GraphicsTexture* leadingIcon;
#ifdef PLAYTEST
		float _iconScale;
#endif
	};

	std::wstring Serialize(GAME::ItemReplicaInfo replica);
	std::vector<std::string> GetNextLineAndSplitIntoTokens(std::istream& str);
	GAME::ItemReplicaInfo* Deserialize(std::vector<std::string> tokens);
	std::wstring GameTextLineToString(std::vector<GameTextLine>& gameTextLines);
}

// ?GetItemReplicaInfo@Item@GAME@@UEBAXAEAUItemReplicaInfo@2@@Z
// void GAME::Item::GetItemReplicaInfo(struct GAME::ItemReplicaInfo &)
//
typedef void (__thiscall* ItemGetItemReplicaInfo)(void* This, GAME::ItemReplicaInfo& info);


typedef GAME::Item* (__fastcall* pCreateItem)(GAME::ItemReplicaInfo* info);
static auto fnCreateItem = pCreateItem(GetProcAddressOrLogToFile(L"game.dll", "?CreateItem@Item@GAME@@SAPEAV12@AEBUItemReplicaInfo@2@@Z"));

typedef GAME::ObjectManager* (__fastcall* pGetObjectManager)();
static auto fnGetObjectManager = pGetObjectManager(GetProcAddressOrLogToFile(L"engine.dll", "?Get@?$Singleton@VObjectManager@GAME@@@GAME@@SAPEAVObjectManager@2@XZ"));

typedef void(__fastcall* pDestroyObjectEx)(GAME::ObjectManager*, GAME::Object* object, const char* file, int line);
static auto fnDestroyObjectEx = pDestroyObjectEx(GetProcAddressOrLogToFile(L"engine.dll","?DestroyObjectEx@ObjectManager@GAME@@QEAAXPEAVObject@2@PEBDH@Z"));

typedef void(__fastcall* pItemEquipmentGetUIDisplayText)(GAME::ItemEquipment*, GAME::Character* myCharacter, std::vector<GAME::GameTextLine>* text);
static auto fnItemEquipmentGetUIDisplayText = pItemEquipmentGetUIDisplayText(GetProcAddressOrLogToFile(L"game.dll", "?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@@Z"));
static auto fnItemGetItemReplicaInfo = ItemGetItemReplicaInfo(GetProcAddressOrLogToFile(L"game.dll", GET_ITEM_REPLICAINFO));

typedef GAME::Player* (__fastcall* pGetMainPlayer)(GAME::GameEngine*);
static auto fnGetMainPlayer = pGetMainPlayer(GetProcAddressOrLogToFile(L"game.dll", "?GetMainPlayer@GameEngine@GAME@@QEBAPEAVPlayer@2@XZ"));


typedef void(__fastcall* pGetModNameArg)(GAME::GameInfo* gi, std::wstring* str);
static auto fnGetModNameArg = pGetModNameArg(GetProcAddressOrLogToFile(L"engine.dll", "?GetModName@GameInfo@GAME@@QEAAXAEAV?$basic_string@GU?$char_traits@G@std@@V?$allocator@G@2@@std@@@Z"));


typedef int(__fastcall* pGetGameInfoMode)(GAME::GameInfo* gi);
static auto fnGetGameInfoMode = pGetGameInfoMode(GetProcAddressOrLogToFile(L"engine.dll", "?GetMode@GameInfo@GAME@@QEBAIXZ"));

typedef std::wstring const& (__fastcall* pGetModName)(GAME::GameInfo* gi);
static auto fnGetModName = pGetModName(GetProcAddressOrLogToFile(L"engine.dll", "?GetModName@GameInfo@GAME@@QEBAAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@XZ"));


typedef GAME::GameInfo* (__fastcall* pGetGameInfo)(GAME::Engine* ge);
static auto fnGetGameInfo = pGetGameInfo(GetProcAddressOrLogToFile(L"engine.dll", "?GetGameInfo@Engine@GAME@@QEAAPEAVGameInfo@2@XZ"));

typedef GAME::GameInfo* (__fastcall* pPlayDropSound)(GAME::Item* ge);
static auto fnPlayDropSound = pPlayDropSound(GetProcAddressOrLogToFile(L"game.dll", "?PlayDropSound@Item@GAME@@UEAAXXZ"));

typedef GAME::GameInfo* (__fastcall* pShowCinematicText)(GAME::Engine* engine, const std::wstring* header, const std::wstring* content, int CinematicTextType, GAME::Color* color);
static auto fnShowCinematicText = pShowCinematicText(GetProcAddressOrLogToFile(L"engine.dll", "?ShowCinematicText@Engine@GAME@@QEAAXAEBV?$basic_string@GU?$char_traits@G@std@@V?$allocator@G@2@@std@@0W4CinematicTextType@2@AEBVColor@2@@Z"));


typedef bool(__thiscall* IsGameLoadingPtr)(void* This);
typedef bool(__thiscall* IsGameWaitingPtr)(void* This, bool);
typedef bool(__thiscall* SortInventorySackPtr)(void* This, int);
extern IsGameLoadingPtr IsGameLoading;
extern IsGameLoadingPtr IsGameEngineOnline;
extern IsGameWaitingPtr IsGameWaiting;
extern SortInventorySackPtr SortInventorySack;


// 
// 
//typedef unsigned __int64(__fastcall* pGetSelectedTransferSackNumber)(GAME::GameEngine*);
//static auto fnGetSelectedTransferSackNumber = pGetSelectedTransferSackNumber(GetProcAddressOrLogToFile((L"game.dll", "?GetSelectedTransferSackNumber@GameEngine@GAME@@QEBAIXZ"));


typedef void* (__fastcall* pGetPlayerTransfer)(GAME::GameEngine*);
static auto fnGetPlayerTransfer = pGetPlayerTransfer(GetProcAddressOrLogToFile(L"game.dll", "?GetPlayerTransfer@GameEngine@GAME@@QEAAAEAV?$vector@PEAVInventorySack@GAME@@@mem@@XZ"));



typedef bool(__fastcall* pGetHardcore)(GAME::GameInfo*);

GAME::GameEngine* fnGetGameEngine();
GAME::Engine* fnGetEngine(bool skipLog = false);

bool fnGetHardcore(GAME::GameInfo* gameInfo, bool skipLog = false);