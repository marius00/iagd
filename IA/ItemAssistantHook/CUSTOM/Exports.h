#pragma once

#if defined(_AMD64_)
#define REQUEST_MOVE_ACTION_IDLE "?RequestMoveAction@ControllerPlayerStateIdle@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_MOVETO "?RequestMoveAction@ControllerPlayerStateMoveTo@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define GET_TRANSFER_SACK "?GetTransferSack@GameEngine@GAME@@QEAAPEAVInventorySack@2@H@Z"
#define SET_TRANSFER_OPEN "?SetTransferOpen@GameEngine@GAME@@QEAAX_N@Z"
#define GAMEINFO_CONSTRUCTOR_ARGS "??0GameInfo@GAME@@QEAA@AEBV01@@Z"
#define GAMEINFO_CONSTRUCTOR "??0GameInfo@GAME@@QEAA@XZ"
#define SET_IS_HARDCORE "?SetHardcore@GameInfo@GAME@@QEAAX_N@Z"
#define SORT_INVENTORY "?Sort@InventorySack@GAME@@QEAA_NI@Z"
#define GET_HARDCORE "?GetHardcore@GameInfo@GAME@@QEBA_NXZ"
#define GET_GAME_INFO "?GetGameInfo@Engine@GAME@@QEAAPEAVGameInfo@2@XZ"
#define SAVE_TRANSFER_STASH "?SaveTransferStash@GameEngine@GAME@@QEAAXXZ"
#define REQUEST_NPC_ACTION "?RequestNpcAction@ControllerPlayerStateIdle@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_MOVETO "?RequestNpcAction@ControllerPlayerStateMoveTo@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define GET_PLAYER_ITEM_COUNT_IN_STASHES "?GetItemCountInStashes@Player@GAME@@UEBAHAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEBV?$vector@V?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@@mem@@1AEAV?$vector@I@6@_N3@Z"

#define CLOUD_WRITE "?CloudWrite@Steamworks@GAME@@QEBA_NAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@PEBXI_N@Z"
#define CLOUD_READ "?CloudRead@Steamworks@GAME@@QEBA_NAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@PEAXI@Z"
#define CLOUD_GET_NUM_FILES "?CloudGetNumFiles@Steamworks@GAME@@QEBAIXZ"

#define GET_PRIVATE_STASH "?GetPrivateStash@Player@GAME@@QEAAAEAV?$vector@PEAVInventorySack@GAME@@@mem@@XZ"
#else
#define REQUEST_MOVE_ACTION_IDLE "?RequestMoveAction@ControllerPlayerStateIdle@GAME@@MAEX_N0ABVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_MOVETO "?RequestMoveAction@ControllerPlayerStateMoveTo@GAME@@MAEX_N0ABVWorldVec3@2@@Z"
#define GET_TRANSFER_SACK "?GetTransferSack@GameEngine@GAME@@QAEPAVInventorySack@2@H@Z"
#define SET_TRANSFER_OPEN "?SetTransferOpen@GameEngine@GAME@@QAEX_N@Z"
#define GAMEINFO_CONSTRUCTOR_ARGS "??0GameInfo@GAME@@QAE@ABV01@@Z"
#define GAMEINFO_CONSTRUCTOR "??0GameInfo@GAME@@QAE@XZ"
#define SET_IS_HARDCORE "?SetHardcore@GameInfo@GAME@@QAEX_N@Z"
#define SORT_INVENTORY "?Sort@InventorySack@GAME@@QAE_NI@Z"
#define GET_HARDCORE "?GetHardcore@GameInfo@GAME@@QBE_NXZ"
#define GET_GAME_INFO "?GetGameInfo@Engine@GAME@@QAEPAVGameInfo@2@XZ"
#define SAVE_TRANSFER_STASH "?SaveTransferStash@GameEngine@GAME@@QAEXXZ"
#define REQUEST_NPC_ACTION "?RequestNpcAction@ControllerPlayerStateIdle@GAME@@MAEX_N0ABVWorldVec3@2@PBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_MOVETO "?RequestNpcAction@ControllerPlayerStateMoveTo@GAME@@MAEX_N0ABVWorldVec3@2@PBVNpc@2@@Z"
#define GET_PLAYER_ITEM_COUNT_IN_STASHES "?GetItemCountInStashes@Player@GAME@@UBEHABV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@ABV?$vector@V?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@@mem@@1AAV?$vector@I@6@_N3@Z"

#define CLOUD_WRITE "?CloudWrite@Steamworks@GAME@@QBE_NABV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@PBXI_N@Z"
#define CLOUD_READ "?CloudRead@Steamworks@GAME@@QBE_NABV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@PAXI@Z"
#define CLOUD_GET_NUM_FILES "?CloudGetNumFiles@Steamworks@GAME@@QBEIXZ"
#define GET_PRIVATE_STASH "?GetPrivateStash@Player@GAME@@QAEAAV?$vector@PAVInventorySack@GAME@@@mem@@XZ"
#endif