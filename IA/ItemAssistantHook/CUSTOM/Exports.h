#pragma once

#if defined(_AMD64_)
#define REQUEST_MOVE_ACTION_IDLE "?RequestMoveAction@ControllerPlayerStateIdle@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_MOVETO "?RequestMoveAction@ControllerPlayerStateMoveTo@GAME@@MEAAX_N0AEBVWorldVec3@2@@Z"
#else
#define REQUEST_MOVE_ACTION_IDLE "?RequestMoveAction@ControllerPlayerStateIdle@GAME@@MAEX_N0ABVWorldVec3@2@@Z"
#define REQUEST_MOVE_ACTION_MOVETO "?RequestMoveAction@ControllerPlayerStateMoveTo@GAME@@MAEX_N0ABVWorldVec3@2@@Z"
#endif