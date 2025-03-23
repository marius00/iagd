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

#define SET_TRANSFER_OPEN "?SetTransferOpen@GameEngine@GAME@@QEAAX_N@Z"
#define GAMEINFO_CONSTRUCTOR_ARGS "??0GameInfo@GAME@@QEAA@AEBV01@@Z"
#define GAMEINFO_CONSTRUCTOR "??0GameInfo@GAME@@QEAA@XZ"
#define SET_IS_HARDCORE "?SetHardcore@GameInfo@GAME@@QEAAX_N@Z"
#define GET_HARDCORE "?GetHardcore@GameInfo@GAME@@QEBA_NXZ"
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
#define REQUEST_NPC_ACTION_TALK_TO_NPC "?RequestNpcAction@ControllerPlayerStateTalkToNpc@GAME@@MEAAX_N0AEBVWorldVec3@2@PEBVNpc@2@@Z"


#define GET_PLAYER_ITEM_COUNT_IN_STASHES "?GetItemCountInStashes@Player@GAME@@UEBAHAEBV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEBV?$vector@V?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@@mem@@1AEAV?$vector@I@6@_N3@Z"

#define CAN_USE_DISMANTLE "?MainPlayerCanUseDismantle@GameEngine@GAME@@QEBA_NXZ"

#define GET_PRIVATE_STASH "?GetPrivateStash@Player@GAME@@QEAAAEAV?$vector@PEAVInventorySack@GAME@@@mem@@XZ"

#define ITEM_EQUIPMENT_GETUIDISPLAYTEXT "?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z"
#define GET_ITEMARTIFACT_GETUIDISPLAYTEXT "?GetUIDisplayText@ItemArtifact@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z"

#define GET_ITEM_REPLICAINFO "?GetItemReplicaInfo@Item@GAME@@UEBAXAEAUItemReplicaInfo@2@@Z"
