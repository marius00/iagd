using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc {

    // See [MessageType.h] from the C++ project in IA\ItemAssistantHook
    // Must be identical
    public enum MessageType {
        TYPE_OPEN_PRIVATE_STASH = 1,
        TYPE_OPEN_CLOSE_TRANSFER_STASH = 2,
        TYPE_REPORT_WORKER_THREAD_LAUNCHED = 3,
        TYPE_ControllerPlayerStateIdleRequestMoveAction = 4,

        //TYPE_ControllerPlayerStateIdleRequestInteractableAction = 5,
        TYPE_ControllerPlayerStateIdleRequestNpcAction = 6,

        TYPE_ControllerPlayerStateMoveToRequestNpcAction = 7,
        TYPE_ControllerPlayerStateMoveToRequestMoveAction = 8,
        TYPE_InventorySack_AddItem = 9,
        TYPE_InventorySack_AddItem_Vec2 = 10,
        TYPE_CloudGetNumFiles = 11,
        TYPE_CloudRead = 12,
        TYPE_CloudWrite = 13,
        TYPE_DamageAttributeAbs_AddDamageToAccumulator = 14,
        TYPE_Nabbed_ItemReplicaInfo = 15,
        TYPE_GameEngine_GetTransferSack = 16,
        TYPE_HookUnload = 17,
        TYPE_GameEngine_AddItemToTransfer_01 = 18,
        TYPE_GameEngine_AddItemToTransfer_02 = 19,
        TYPE_GameInfo_IsHardcore = 20,
        TYPE_GameInfo_SetModName = 21,
        TYPE_Custom_AddItem = 22,
        TYPE_Custom_AddItemFailed = 23,
        TYPE_ERROR_NO_MOD_NAME = 24,
        TYPE_Stash_Item_BasicInfo = 25,
        TYPE_InventorySack_Sort = 26,
        TYPE_Custom_AddItemSucceeded = 27,
        TYPE_WorldSpawnItem = 28,
        TYPE_LOG01 = 29,
        TYPE_LOG02 = 30,
        TYPE_HookedCombatManager_ApplyDamage = 31,
        TYPE_HookedCombatManager_ApplyDamage_Exit = 32,
        TYPE_LOG02_ = 33,

        TYPE_DEBUG_DefenseAttribute_Jitter = 34,
        TYPE_DEBUG_CharAttribute_AddJitter = 35,
        TYPE_DEBUG_CharAttributeStore_Equipment_Load = 36,

        TYPE_ERROR_HOOKING_PRIVATE_STASH = 37,
        TYPE_ERROR_HOOKING_TRANSFER_STASH = 38,
        TYPE_SAVE_TRANSFER_STASH = 39,
        TYPE_ERROR_HOOKING_SAVETRANSFER_STASH = 40,


        TYPE_DISPLAY_CRAFTER = 41,
        TYPE_DISPLAY_CARAVAN = 42,
        TYPE_DISPLAY_ENCHANTER = 43,

        TYPE_ERROR_HOOKING_GENERIC = 44,
        TYPE_RequestRestrictedSack = 45,
        TYPE_REPORT_WORKER_THREAD_EXPERIMENTAL_LAUNCHED = 46,
        TYPE_GameInfo_IsHardcore_via_init = 47,
        TYPE_GameInfo_IsHardcore_via_init_2 = 48,

        TYPE_DetectedStashToLootFrom = 49,
    };
}