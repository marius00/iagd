cmake_minimum_required(VERSION 3.29)
project(Hook)

set(CMAKE_CXX_STANDARD 14)

include_directories(.)

add_executable(Hook
    BaseMethodHook.cpp
    BaseMethodHook.h
    CanUseDismantle.cpp
    CanUseDismantle.h
    CloudGetNumFiles.cpp
    CloudGetNumFiles.h
    CloudRead.cpp
    CloudRead.h
    CloudWrite.cpp
    CloudWrite.h
    DataQueue.cpp
    DataQueue.h
    dllmain.cpp
    Exports.h
    GameEngineUpdate.cpp
    GameEngineUpdate.h
    GetPrivateStash.cpp
    GetPrivateStash.h
    GrimTypes.cpp
    GrimTypes.h
    HookLog.cpp
    HookLog.h
    InventorySack_AddItem.cpp
    InventorySack_AddItem.h
    Logger.h
    MessageType.h
    NpcDetectionHook.cpp
    NpcDetectionHook.h
    OnDemandSeedInfo.cpp
    OnDemandSeedInfo.h
    resource.h
    SaveTransferStash.cpp
    SaveTransferStash.h
    SetHardcore.cpp
    SetHardcore.h
    SettingsReader.cpp
    SettingsReader.h
    SetTransferOpen.cpp
    SetTransferOpen.h
    StateRequestMoveAction.cpp
    StateRequestMoveAction.h
    StateRequestNpcAction.cpp
    StateRequestNpcAction.h
    stdafx.cpp
    stdafx.h
    Vec3f.h)

target_include_directories(detours PRIVATE ../Detours-master/src)
set(SOURCES ../Detours-master/src/detours.h)
add_library(detours SHARED ../Detours-master/x64/detours.lib)