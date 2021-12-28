using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace IAGrim.UI.Misc {
    // See [MessageType.h] from the C++ project in IA\ItemAssistantHook
    // Must be identical
    public enum MessageType {
        TYPE_OPEN_PRIVATE_STASH = 1,
        TYPE_OPEN_CLOSE_TRANSFER_STASH = 2,
        TYPE_REPORT_WORKER_THREAD_LAUNCHED = 3,

        // Movement
        TYPE_ControllerRequestNpcAction = 7,
        TYPE_ControllerRequestMoveAction = 8,

        TYPE_CloudGetNumFiles = 11,
        TYPE_CloudRead = 12,
        TYPE_CloudWrite = 13,
        TYPE_GameInfo_IsHardcore = 20,
        TYPE_GameInfo_SetModName = 21,
        TYPE_Stash_Item_BasicInfo = 25,
        TYPE_InventorySack_Sort = 26,

        TYPE_ERROR_HOOKING_PRIVATE_STASH = 37,
        TYPE_ERROR_HOOKING_TRANSFER_STASH = 38,
        TYPE_SAVE_TRANSFER_STASH = 39,
        TYPE_ERROR_HOOKING_SAVETRANSFER_STASH = 40,


        TYPE_DISPLAY_CRAFTER = 41,

        TYPE_ERROR_HOOKING_GENERIC = 44,
        TYPE_GameInfo_IsHardcore_via_init = 47,

        TYPE_SUCCESS_HOOKING_GENERIC = 52,
        TYPE_Display_Transmute = 55,
        TYPE_CAN_USE_DISMANTLE = 56,
        TYPE_ITEMSEEDDATA_EQ = 71,
        TYPE_ITEMSEEDDATA_REL = 72,
        TYPE_ITEMSEEDDATA_BASE = 73,
        TYPE_ITEMSEEDDATA_PLAYERID = 74,
        TYPE_ITEMSEEDDATA_PLAYERID_ERR_NOGAME = 62,
        TYPE_ITEMSEEDDATA_PLAYERID_ERR_NOITEM = 63,
        TYPE_ITEMSEEDDATA_PLAYERID_DEBUG_RECV = 64,
    };
}