#pragma once

#define REQUEST_MOVE_ACTION_IDLE "?RequestMoveAction@ControllerPlayerStateIdle@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_MOVETO "?RequestMoveAction@ControllerPlayerStateMoveTo@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_LONG_IDLE "?RequestMoveAction@ControllerPlayerStateLongIdle@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_MOVE_AND_SKILL "?RequestMoveAction@ControllerPlayerStateMoveAndUseSkill@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_MOVE_ACTOR "?RequestMoveAction@ControllerPlayerStateMoveToActorBase@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_MOVE_TO_SKILL "?RequestMoveAction@ControllerPlayerStateMoveToUseSkill@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_PICKUP "?RequestMoveAction@ControllerPlayerStatePickupItem@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_TALK_TO_NPC "?RequestMoveAction@ControllerPlayerStateTalkToNpc@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_SKILL "?RequestMoveAction@ControllerPlayerStateUseSkill@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"

#define REQUEST_ROTATE_ACTION_IDLE "?RequestRotateAction@ControllerPlayerStateIdle@GAME@@MEAAXAEBVWorldVec3@2@@Z"
#define REQUEST_ROTATE_ACTION_LONG_IDLE "?RequestRotateAction@ControllerPlayerStateLongIdle@GAME@@MEAAXAEBVWorldVec3@2@@Z"

#define GET_TRANSFER_SACK "?GetTransferSack@GameEngine@GAME@@QEAAPEAVInventorySack@2@H@Z"
#define SET_TRANSFER_OPEN "?SetTransferOpen@GameEngine@GAME@@QEAAX_N@Z"
#define GAMEINFO_CONSTRUCTOR_ARGS "??0GameInfo@GAME@@QEAA@AEBV01@@Z"
#define GAMEINFO_CONSTRUCTOR "??0GameInfo@GAME@@QEAA@XZ"
#define SET_IS_HARDCORE "?SetHardcore@GameInfo@GAME@@QEAAX_N@Z"
#define SORT_INVENTORY "?Sort@InventorySack@GAME@@QEAA_NI@Z"
#define GET_HARDCORE "?GetHardcore@GameInfo@GAME@@QEBA_NXZ"
#define GET_GAME_INFO "?GetGameInfo@Engine@GAME@@QEAAPEAVGameInfo@2@XZ"
#define SAVE_TRANSFER_STASH "?SaveTransferStash@GameEngine@GAME@@QEAAXXZ"
#define REQUEST_NPC_ACTION_IDLE "?RequestNpcAction@ControllerPlayerStateIdle@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_MOVETO "?RequestNpcAction@ControllerPlayerStateMoveTo@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_JUMP_TO_SKILL "?RequestNpcAction@ControllerPlayerStateJumpToUseSkill@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_LONG_IDLE "?RequestNpcAction@ControllerPlayerStateLongIdle@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_MOVE_AND_SKILL "?RequestNpcAction@ControllerPlayerStateMoveAndUseSkill@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_MOVE_TO_ACTOR "?RequestNpcAction@ControllerPlayerStateMoveToActorBase@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_MOVE_TO_NPC "?RequestNpcAction@ControllerPlayerStateMoveToNpc@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_MOVE_TO_SKILL "?RequestNpcAction@ControllerPlayerStateMoveToUseSkill@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
#define REQUEST_NPC_ACTION_PICKUP_ITEM "?RequestNpcAction@ControllerPlayerStatePickupItem@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"
//#define REQUEST_NPC_ACTION_SLEEP "?RequestNpcAction@ControllerPlayerStateSleep@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z" // Crash
#define REQUEST_NPC_ACTION_TALK_TO_NPC "?RequestNpcAction@ControllerPlayerStateTalkToNpc@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"


#define GET_PLAYER_ITEM_COUNT_IN_STASHES "?GetItemCountInStashes@Player@GAME@@UEBAHAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEBV?$vector@V?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@@mem@@1AEAV?$vector@I@6@_N3@Z"

#define NPC_TRANSMUTE_ON_PLAYER_INTERACT "?OnPlayerInteract@NpcTransmuter@GAME@@UEAAXI_N0@Z"
#define NPC_ENCHANTER_ON_PLAYER_INTERACT "?OnPlayerInteract@NpcEnchanter@GAME@@UEAAXI_N0@Z"
#define CAN_USE_DISMANTLE "?MainPlayerCanUseDismantle@GameEngine@GAME@@QEBA_NXZ"

#define CLOUD_WRITE "?CloudWrite@Steamworks@GAME@@QEBA_NAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@PEBXI_N@Z"
#define CLOUD_READ "?CloudRead@Steamworks@GAME@@QEBA_NAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@PEAXI@Z"
#define CLOUD_GET_NUM_FILES "?CloudGetNumFiles@Steamworks@GAME@@QEBAIXZ"

#define GET_PRIVATE_STASH "?GetPrivateStash@Player@GAME@@QEAAAEAV?$vector@PEAVInventorySack@GAME@@@mem@@XZ"

#define SAVE_MANAGER_DIRECTREAD "?DirectRead@SaveManager@GAME@@QEAA_NAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEAPEAXAEAI_N3@Z"
#define LOAD_PLAYER_TRANSFER "?LoadPlayerTransfer@GameEngine@GAME@@QEAAXXZ"
#define READ_PLAYER_TRANSFER "?ReadPlayerTransfer@GameEngine@GAME@@QEAA_NAEAVCheckedReader@2@@Z"
#define EXPERIMENTAL_HOOK "?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@@Z"

#define GET_ITEM_REPLICAINFO "?GetItemReplicaInfo@Item@GAME@@UEBAXAEAUItemReplicaInfo@2@@Z"