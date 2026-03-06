#pragma once
#include "GrimTypes.h"
#include "Logger.h"

namespace VTableDispatch
{
	typedef void(__fastcall* pGetUIDisplayText)(
		void*                             /* this            */,
		const GAME::Character*            /* myCharacter     */,
		std::vector<GAME::GameTextLine>*  /* text            */,
		bool                              /* includeSetBonus */
		);

	// Candidates in priority order. We take the first one that exists.
	// ItemEquipment is tried first as it is the most common item type.
	// All share the same signature and calling one on any Item subclass
	// is safe -- virtual dispatch is not needed because we are calling
	// the exported implementation directly, not through the vtable.
	//
	// IMPORTANT: this only works correctly if the item passed to Call()
	// is actually of the matching subclass (or a subclass of it). Since
	// all relevant items are Item subclasses and the base Item export is
	// last in the list as a fallback, this is always safe.
	static const char* const k_candidates[] = {
		"?GetUIDisplayText@Item@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z",
		"?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z",
		"?GetUIDisplayText@ItemRelic@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z",
		"?GetUIDisplayText@ItemArtifact@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z",
	};

	static pGetUIDisplayText s_fn = nullptr;
	static bool              s_available = false;
	static bool              s_disabled = false;

	// ---------------------------------------------------------------------------
	// Init
	// Call once at the top of EnableHook(), before any Detours calls.
	// ---------------------------------------------------------------------------
	static void Init()
	{
		if (s_available || s_disabled)
			return;

		HMODULE hGame = ::GetModuleHandleW(L"game.dll");
		if (!hGame)
		{
			LogToFile(LogLevel::FATAL, "VTableDispatch::Init - game.dll not loaded, disabling.");
			s_disabled = true;
			return;
		}

		for (const char* name : k_candidates)
		{
			void* addr = ::GetProcAddress(hGame, name);
			if (addr)
			{
				s_fn = reinterpret_cast<pGetUIDisplayText>(addr);
				s_available = true;
				LogToFile(LogLevel::INFO,
					std::string("VTableDispatch::Init - using export: ") + name);
				return;
			}
		}

		LogToFile(LogLevel::FATAL,
			"VTableDispatch::Init - no known GetUIDisplayText exports found. Disabling.");
		s_disabled = true;
	}

	static bool IsAvailable() { return s_available && !s_disabled; }

	// ---------------------------------------------------------------------------
	// Call
	// ---------------------------------------------------------------------------
	static void Call(
		void* item,
		const GAME::Character* character,
		std::vector<GAME::GameTextLine>* lines,
		bool                             includeSetBonus)
	{
		if (!item || !s_available || s_disabled || !s_fn)
			return;

		s_fn(item, character, lines, includeSetBonus);
	}

} // namespace VTableDispatch


/*
IDA Pro:

I found the ItemEquipment constructor, then inside it i navigated to vftable (also found via shift+f4 and search for ItemEquipment)
Then you discard everything before dq offset, so we see its line / offset = 135
(0x180703D18 - 0x1807038E0) / 8 = 0x438 / 8 = 0x87 = 135


.rdata:00000001807038E0 ??_7ItemEquipment@GAME@@6BObject@1@@ dq offset ?GetStaticClassInfo@ItemEquipment@GAME@@SAAEBVRTTI_ClassInfo@2@XZ
.rdata:00000001807038E0                                         ; DATA XREF: GAME::ItemEquipment::ItemEquipment(void)+21o
.rdata:00000001807038E0                                         ; GAME::ItemEquipment::~ItemEquipment(void)+1Co ...
.rdata:00000001807038E0                                         ; GAME::ItemEquipment::GetStaticClassInfo(void)
.rdata:00000001807038E8                 dq offset ?Load@ItemEquipment@GAME@@UEAAXAEBVLoadTable@2@@Z ; GAME::ItemEquipment::Load(GAME::LoadTable const &)
.rdata:00000001807038F0                 dq offset ?CalculateMemoryUsage@Actor@GAME@@MEBAIXZ ; GAME::Actor::CalculateMemoryUsage(void)
.rdata:00000001807038F8                 dq offset ?CalculateAllocatedMemory@Actor@GAME@@MEBAIXZ ; GAME::Actor::CalculateAllocatedMemory(void)
.rdata:0000000180703900                 dq offset sub_180314380
.rdata:0000000180703908                 dq offset ?Read@Actor@GAME@@UEAAXAEAVBinaryReader@2@@Z ; GAME::Actor::Read(GAME::BinaryReader &)
.rdata:0000000180703910                 dq offset ?Write@Actor@GAME@@UEBAXAEAVBinaryWriter@2@@Z ; GAME::Actor::Write(GAME::BinaryWriter &)
.rdata:0000000180703918                 dq offset ?IsSavedByEditor@Entity@GAME@@UEBA_NXZ ; GAME::Entity::IsSavedByEditor(void)
.rdata:0000000180703920                 dq offset ?CanBePlacedInEditor@Entity@GAME@@UEBA_NXZ ; GAME::Entity::CanBePlacedInEditor(void)
.rdata:0000000180703928                 dq offset ?LoadWithoutScripts@Entity@GAME@@UEAAXAEBVLoadTable@2@@Z ; GAME::Entity::LoadWithoutScripts(GAME::LoadTable const &)
.rdata:0000000180703930                 dq offset ?PreLoad@ItemEquipment@GAME@@UEAA_N_N@Z ; GAME::ItemEquipment::PreLoad(bool)
.rdata:0000000180703938                 dq offset ?AppendDetailMapData@Item@GAME@@UEAAXAEAV?$vector@UMinimapGameNugget@GAME@@@mem@@@Z ; GAME::Item::AppendDetailMapData(mem::vector<GAME::MinimapGameNugget> &)
.rdata:0000000180703940                 dq offset ?IsInWorld@Entity@GAME@@UEBA_NXZ ; GAME::Entity::IsInWorld(void)
.rdata:0000000180703948                 dq offset ?OnAddToWorld@Entity@GAME@@UEAAXXZ ; GAME::Entity::OnAddToWorld(void)
.rdata:0000000180703950                 dq offset ?OnRemoveFromWorld@Entity@GAME@@UEAAXXZ ; GAME::Entity::OnRemoveFromWorld(void)
.rdata:0000000180703958                 dq offset ?OnUnload@Entity@GAME@@UEAAXXZ ; GAME::Entity::OnUnload(void)
.rdata:0000000180703960                 dq offset ?OnAddToLevel@Actor@GAME@@UEAAXPEAVLevel@2@@Z ; GAME::Actor::OnAddToLevel(GAME::Level *)
.rdata:0000000180703968                 dq offset ?OnRemoveFromLevel@Actor@GAME@@UEAAXPEAVLevel@2@@Z ; GAME::Actor::OnRemoveFromLevel(GAME::Level *)
.rdata:0000000180703970                 dq offset ?OnMoveInLevel@Actor@GAME@@UEAAXPEAVLevel@2@@Z ; GAME::Actor::OnMoveInLevel(GAME::Level *)
.rdata:0000000180703978                 dq offset ?OnDestroy@Actor@GAME@@UEAAXXZ ; GAME::Actor::OnDestroy(void)
.rdata:0000000180703980                 dq offset ?ForcedUpdate@Entity@GAME@@UEAAXH@Z ; GAME::Entity::ForcedUpdate(int)
.rdata:0000000180703988                 dq offset ?AddAttachedEntitiesToScene@Entity@GAME@@UEAAXAEAVGraphicsScene@2@PEBVFrustum@2@@Z ; GAME::Entity::AddAttachedEntitiesToScene(GAME::GraphicsScene &,GAME::Frustum const *)
.rdata:0000000180703990                 dq offset ?AddToScene@Actor@GAME@@UEAAXAEAVGraphicsScene@2@PEBVFrustum@2@@Z ; GAME::Actor::AddToScene(GAME::GraphicsScene &,GAME::Frustum const *)
.rdata:0000000180703998                 dq offset ?GetPhysicsMesh@Actor@GAME@@UEBAPEBVPhysicsMeshBase@2@XZ ; GAME::Actor::GetPhysicsMesh(void)
.rdata:00000001807039A0                 dq offset ?GetCollisionBox@Actor@GAME@@UEBA_NAEAVOBBox@2@@Z ; GAME::Actor::GetCollisionBox(GAME::OBBox &)
.rdata:00000001807039A8                 dq offset ?SetDynamicObstacle@Entity@GAME@@UEAAX_N@Z ; GAME::Entity::SetDynamicObstacle(bool)
.rdata:00000001807039B0                 dq offset ?UpdateBoundingBox@Actor@GAME@@UEAAXXZ ; GAME::Actor::UpdateBoundingBox(void)
.rdata:00000001807039B8                 dq offset ?WarmUpStart@SkillActivated@GAME@@UEAA_NAEAVCharacter@2@@Z ; GAME::SkillActivated::WarmUpStart(GAME::Character &)
.rdata:00000001807039C0                 dq offset ?DynamicPathingOccluder@Entity@GAME@@UEBA?AW4DynamicOccluderType@12@XZ ; GAME::Entity::DynamicPathingOccluder(void)
.rdata:00000001807039C8                 dq offset ?GetNumHitBoxes@Actor@GAME@@UEBAIXZ ; GAME::Actor::GetNumHitBoxes(void)
.rdata:00000001807039D0                 dq offset ?GetHitBox@Actor@GAME@@UEBA?AUHitBox@GraphicsMesh@2@HAEBVSkeletalPose@2@@Z ; GAME::Actor::GetHitBox(int,GAME::SkeletalPose const &)
.rdata:00000001807039D8                 dq offset ?GetHitBox@Actor@GAME@@UEBA?AVOBBox@2@H@Z ; GAME::Actor::GetHitBox(int)
.rdata:00000001807039E0                 dq offset ?GetCenterOfMass@Actor@GAME@@UEBA?AVVec3@2@XZ ; GAME::Actor::GetCenterOfMass(void)
.rdata:00000001807039E8                 dq offset ?SetPhysicsSimulation@Entity@GAME@@UEAAXW4PhysicsSimulation@2@@Z ; GAME::Entity::SetPhysicsSimulation(GAME::PhysicsSimulation)
.rdata:00000001807039F0                 dq offset ?CollisionCallback@Item@GAME@@UEAAXAEBUCollisionData@2@@Z ; GAME::Item::CollisionCallback(GAME::CollisionData const &)
.rdata:00000001807039F8                 dq offset ?CrowdAgentUpdate@Entity@GAME@@UEAAXHAEAUCrowdAgentParams@CROWD@@@Z ; GAME::Entity::CrowdAgentUpdate(int,CROWD::CrowdAgentParams &)
.rdata:0000000180703A00                 dq offset ?CrowdAgentMoved@Entity@GAME@@UEAAXAEBUCrowdAgentData@CROWD@@@Z ; GAME::Entity::CrowdAgentMoved(CROWD::CrowdAgentData const &)
.rdata:0000000180703A08                 dq offset ?GetIntersection@Actor@GAME@@UEBAXAEBVRay@2@AEAVIntersection@2@W4PhysicsSurface@2@_N@Z ; GAME::Actor::GetIntersection(GAME::Ray const &,GAME::Intersection &,GAME::PhysicsSurface,bool)
.rdata:0000000180703A10                 dq offset ?GetIntersection@Entity@GAME@@UEBAXAEBVRay@2@MAEAVIntersection@2@W4PhysicsSurface@2@@Z ; GAME::Entity::GetIntersection(GAME::Ray const &,float,GAME::Intersection &,GAME::PhysicsSurface)
.rdata:0000000180703A18                 dq offset ?CheckLOS@Actor@GAME@@UEBA_NAEBVRay@2@M@Z ; GAME::Actor::CheckLOS(GAME::Ray const &,float)
.rdata:0000000180703A20                 dq offset ?InitialUpdate@Item@GAME@@UEAAXXZ ; GAME::Item::InitialUpdate(void)
.rdata:0000000180703A28                 dq offset ?SetTransparency@Actor@GAME@@UEAAXAEBVFrustum@2@@Z ; GAME::Actor::SetTransparency(GAME::Frustum const &)
.rdata:0000000180703A30                 dq offset ?SetTransparent@Actor@GAME@@UEAAXXZ ; GAME::Actor::SetTransparent(void)
.rdata:0000000180703A38                 dq offset ?IsStatic@Entity@GAME@@UEBA_NXZ ; GAME::Entity::IsStatic(void)
.rdata:0000000180703A40                 dq offset ?Attach@Actor@GAME@@UEAAXPEAVEntity@2@AEBVCoords@2@AEBVName@2@_N33@Z ; GAME::Actor::Attach(GAME::Entity *,GAME::Coords const &,GAME::Name const &,bool,bool,bool)
.rdata:0000000180703A48                 dq offset ?Attach@Actor@GAME@@UEAAXPEAVEntity@2@AEBVCoords@2@PEBD_N33@Z ; GAME::Actor::Attach(GAME::Entity *,GAME::Coords const &,char const *,bool,bool,bool)
.rdata:0000000180703A50                 dq offset ?GetAttachedCoords@Actor@GAME@@UEBA?AVWorldCoords@2@AEBVName@2@@Z ; GAME::Actor::GetAttachedCoords(GAME::Name const &)
.rdata:0000000180703A58                 dq offset ?GetAttachedCoordsInRegion@Actor@GAME@@UEBA?AVWorldCoords@2@AEBVName@2@_N@Z ; GAME::Actor::GetAttachedCoordsInRegion(GAME::Name const &,bool)
.rdata:0000000180703A60                 dq offset ?GetBoneCoordsInRegion@Actor@GAME@@UEBA?AVWorldCoords@2@AEBVName@2@_N@Z ; GAME::Actor::GetBoneCoordsInRegion(GAME::Name const &,bool)
.rdata:0000000180703A68                 dq offset ?DeleteOnEnteringUnloadedLevel@Entity@GAME@@UEBA_NXZ ; GAME::Entity::DeleteOnEnteringUnloadedLevel(void)
.rdata:0000000180703A70                 dq offset ?ShouldServerSpawn@Item@GAME@@UEBA_NH@Z ; GAME::Item::ShouldServerSpawn(int)
.rdata:0000000180703A78                 dq offset ?CreateSpawnNetPacket@Entity@GAME@@UEAAPEAVNetPacket@2@XZ ; GAME::Entity::CreateSpawnNetPacket(void)
.rdata:0000000180703A80                 dq offset ?WriteReplicationData@Item@GAME@@UEAAXAEAVNetPacketOutBuffer@2@@Z ; GAME::Item::WriteReplicationData(GAME::NetPacketOutBuffer &)
.rdata:0000000180703A88                 dq offset ?ReadReplicationData@Item@GAME@@UEAAXAEAVNetPacketInBuffer@2@@Z ; GAME::Item::ReadReplicationData(GAME::NetPacketInBuffer &)
.rdata:0000000180703A90                 dq offset ?ApplyReplicationData@Item@GAME@@UEAAXXZ ; GAME::Item::ApplyReplicationData(void)
.rdata:0000000180703A98                 dq offset ?GetExtents@Actor@GAME@@UEBAMXZ ; GAME::Actor::GetExtents(void)
.rdata:0000000180703AA0                 dq offset ?GetRadius@Actor@GAME@@UEBAMXZ ; GAME::Actor::GetRadius(void)
.rdata:0000000180703AA8                 dq offset ?ShouldSaveState@Item@GAME@@UEBA_NXZ ; GAME::Item::ShouldSaveState(void)
.rdata:0000000180703AB0                 dq offset ?SetSaveState@Entity@GAME@@UEAAX_N@Z ; GAME::Entity::SetSaveState(bool)
.rdata:0000000180703AB8                 dq offset ?SaveState@Item@GAME@@UEBAXAEAVBinaryWriter@2@@Z ; GAME::Item::SaveState(GAME::BinaryWriter &)
.rdata:0000000180703AC0                 dq offset ?RestoreState@Item@GAME@@UEAAXAEAVBinaryReader@2@W4Restoration@Entity@2@@Z ; GAME::Item::RestoreState(GAME::BinaryReader &,GAME::Entity::Restoration)
.rdata:0000000180703AC8                 dq offset ?RegisterAnimationCallback@Actor@GAME@@UEAAXAEBVName@2@@Z ; GAME::Actor::RegisterAnimationCallback(GAME::Name const &)
.rdata:0000000180703AD0                 dq offset ?SetVisibility@Actor@GAME@@UEAAX_N@Z ; GAME::Actor::SetVisibility(bool)
.rdata:0000000180703AD8                 dq offset ?SetVisibility@Actor@GAME@@UEAAXW4Visibility@2@@Z ; GAME::Actor::SetVisibility(GAME::Visibility)
.rdata:0000000180703AE0                 dq offset ?GetVisibility@Actor@GAME@@UEBA?AW4Visibility@2@XZ ; GAME::Actor::GetVisibility(void)
.rdata:0000000180703AE8                 dq offset ?HasParentsUniqueID@Entity@GAME@@UEBA_NXZ ; GAME::Entity::HasParentsUniqueID(void)
.rdata:0000000180703AF0                 dq offset ?LogInfo@Entity@GAME@@UEBAXXZ ; GAME::Entity::LogInfo(void)
.rdata:0000000180703AF8                 dq offset ?IncludeInMinimap@Item@GAME@@UEBA_NXZ ; GAME::Item::IncludeInMinimap(void)
.rdata:0000000180703B00                 dq offset ?UseExistingObjectForRestore@Entity@GAME@@UEBA_NXZ ; GAME::Entity::UseExistingObjectForRestore(void)
.rdata:0000000180703B08                 dq offset ?GetCollisionShape@Actor@GAME@@UEBA?AW4PhysicsShape@2@XZ ; GAME::Actor::GetCollisionShape(void)
.rdata:0000000180703B10                 dq offset ?Validate@HotSlotOption@GAME@@UEAA?AW4ValidationResult@12@XZ ; GAME::HotSlotOption::Validate(void)
.rdata:0000000180703B18                 dq offset ?GetPhysicsMass@Actor@GAME@@UEBAMXZ ; GAME::Actor::GetPhysicsMass(void)
.rdata:0000000180703B20                 dq offset ?GetPhysicsFriction@Actor@GAME@@UEBAMXZ ; GAME::Actor::GetPhysicsFriction(void)
.rdata:0000000180703B28                 dq offset ?GetPhysicsRestitution@Actor@GAME@@UEBAMXZ ; GAME::Actor::GetPhysicsRestitution(void)
.rdata:0000000180703B30                 dq offset ?UsePathPositionAsHome@ControllerMonsterStatePatrol@GAME@@MEBA_NXZ ; GAME::ControllerMonsterStatePatrol::UsePathPositionAsHome(void)
.rdata:0000000180703B38                 dq offset ?GetIsPartOfLevel@Entity@GAME@@UEBA_NXZ ; GAME::Entity::GetIsPartOfLevel(void)
.rdata:0000000180703B40                 dq offset ?UpdatePunctuation@Entity@GAME@@UEAAXXZ ; GAME::Entity::UpdatePunctuation(void)
.rdata:0000000180703B48                 dq offset ?BillboardPunctuation@Entity@GAME@@UEAAXXZ ; GAME::Entity::BillboardPunctuation(void)
.rdata:0000000180703B50                 dq offset ?WriteSimulationInformation@Entity@GAME@@UEBAXXZ ; GAME::Entity::WriteSimulationInformation(void)
.rdata:0000000180703B58                 dq offset ?FastUpdate@ItemEquipment@GAME@@UEAAXH@Z ; GAME::ItemEquipment::FastUpdate(int)
.rdata:0000000180703B60                 dq offset ?OnMoved@Entity@GAME@@UEAAXAEBVVec3@2@00@Z ; GAME::Entity::OnMoved(GAME::Vec3 const &,GAME::Vec3 const &,GAME::Vec3 const &)
.rdata:0000000180703B68                 dq offset ?OnReachedMovementGoal@Entity@GAME@@UEAAXXZ ; GAME::Entity::OnReachedMovementGoal(void)
.rdata:0000000180703B70                 dq offset ?OnPathFailed@Entity@GAME@@UEAAXXZ ; GAME::Entity::OnPathFailed(void)
.rdata:0000000180703B78                 dq offset ?IsCopy@Entity@GAME@@UEBA_NXZ ; GAME::Entity::IsCopy(void)
.rdata:0000000180703B80                 dq offset ?ExecuteEventHook@Entity@GAME@@UEAAXIII@Z ; GAME::Entity::ExecuteEventHook(uint,uint,uint)
.rdata:0000000180703B88                 dq offset ?UseStoredOrientation@Entity@GAME@@UEBA_NXZ ; GAME::Entity::UseStoredOrientation(void)
.rdata:0000000180703B90                 dq offset ?GetStoredOrientation@Entity@GAME@@UEAA?AVCoords@2@XZ ; GAME::Entity::GetStoredOrientation(void)
.rdata:0000000180703B98                 dq offset ?UpdateSelf@ItemEquipment@GAME@@UEAAXH@Z ; GAME::ItemEquipment::UpdateSelf(int)
.rdata:0000000180703BA0                 dq offset ?CreatePathObstacles@Actor@GAME@@MEAAXXZ ; GAME::Actor::CreatePathObstacles(void)
.rdata:0000000180703BA8                 dq offset ?RemovePathObstacles@Actor@GAME@@MEAAXXZ ; GAME::Actor::RemovePathObstacles(void)
.rdata:0000000180703BB0                 dq offset ?IsCharacter@Actor@GAME@@UEAA_NXZ ; GAME::Actor::IsCharacter(void)
.rdata:0000000180703BB8                 dq offset ?PlayAnimation@Actor@GAME@@UEAAXHPEBVGraphicsAnim@2@_NMH@Z ; GAME::Actor::PlayAnimation(int,GAME::GraphicsAnim const *,bool,float,int)
.rdata:0000000180703BC0                 dq offset ?GetAlternateMeshName@ItemEquipment@GAME@@UEBA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@XZ ; GAME::ItemEquipment::GetAlternateMeshName(void)
.rdata:0000000180703BC8                 dq offset ?SetScale@Actor@GAME@@UEAAXAEBVVec3@2@@Z ; GAME::Actor::SetScale(GAME::Vec3 const &)
.rdata:0000000180703BD0                 dq offset ?SetScale@Actor@GAME@@UEAAXM@Z ; GAME::Actor::SetScale(float)
.rdata:0000000180703BD8                 dq offset ?SetHighlight@Actor@GAME@@UEAAX_N@Z ; GAME::Actor::SetHighlight(bool)
.rdata:0000000180703BE0                 dq offset ?LoadLite@Actor@GAME@@UEAAXAEBVLoadTable@2@_N@Z ; GAME::Actor::LoadLite(GAME::LoadTable const &,bool)
.rdata:0000000180703BE8                 dq offset ?TweakPose@Actor@GAME@@UEAAXXZ ; GAME::Actor::TweakPose(void)
.rdata:0000000180703BF0                 dq offset ?GetNormal@Actor@GAME@@UEBAXAEBVRay@2@AEAVVec3@2@@Z ; GAME::Actor::GetNormal(GAME::Ray const &,GAME::Vec3 &)
.rdata:0000000180703BF8                 dq offset ?EnableOutline@Actor@GAME@@UEAAX_N@Z ; GAME::Actor::EnableOutline(bool)
.rdata:0000000180703C00                 dq offset ?GetGameDescription@Item@GAME@@UEBA?AV?$basic_string@GU?$char_traits@G@std@@V?$allocator@G@2@@std@@_N0@Z ; GAME::Item::GetGameDescription(bool,bool)
.rdata:0000000180703C08                 dq offset ?UsePathPositionAsHome@ControllerMonsterStatePatrol@GAME@@MEBA_NXZ ; GAME::ControllerMonsterStatePatrol::UsePathPositionAsHome(void)
.rdata:0000000180703C10                 dq offset ?Enqueue@Actor@GAME@@UEAA_NPEAVActorConfigCommand@2@@Z ; GAME::Actor::Enqueue(GAME::ActorConfigCommand *)
.rdata:0000000180703C18                 dq offset ?LocalEnqueue@Actor@GAME@@UEAA_NPEAVActorConfigCommand@2@@Z ; GAME::Actor::LocalEnqueue(GAME::ActorConfigCommand *)
.rdata:0000000180703C20                 dq offset ?GetPose@Actor@GAME@@UEAAXAEAVSkeletalPose@2@@Z ; GAME::Actor::GetPose(GAME::SkeletalPose &)
.rdata:0000000180703C28                 dq offset ?SetPose@Actor@GAME@@UEAAXAEBVSkeletalPose@2@@Z ; GAME::Actor::SetPose(GAME::SkeletalPose const &)
.rdata:0000000180703C30                 dq offset ?GetMeshInstance@Actor@GAME@@UEBAPEAVGraphicsMeshInstance@2@XZ ; GAME::Actor::GetMeshInstance(void)
.rdata:0000000180703C38                 dq offset ?SendCameraShakeEvent@Actor@GAME@@UEAAXHM@Z ; GAME::Actor::SendCameraShakeEvent(int,float)
.rdata:0000000180703C40                 dq offset ?ShouldCastShadows@Actor@GAME@@UEBA_NXZ ; GAME::Actor::ShouldCastShadows(void)
.rdata:0000000180703C48                 dq offset ?GetAmbientHighlight@Actor@GAME@@UEBAMXZ ; GAME::Actor::GetAmbientHighlight(void)
.rdata:0000000180703C50                 dq offset ?GeometryBusStop@Actor@GAME@@UEAAXAEBVABBox@2@AEAV?$vector@UVertex@GeometryBusAdvanced@GAME@@@mem@@AEAV?$vector@UFace@GeometryBusAdvanced@GAME@@@5@M@Z ; GAME::Actor::GeometryBusStop(GAME::ABBox const &,mem::vector<GAME::GeometryBusAdvanced::Vertex> &,mem::vector<GAME::GeometryBusAdvanced::Face> &,float)
.rdata:0000000180703C58                 dq offset ?GeometryBusStop@Actor@GAME@@UEAAXAEBVABBox@2@AEAV?$vector@USimpleFace@GeometryBus@GAME@@@mem@@@Z ; GAME::Actor::GeometryBusStop(GAME::ABBox const &,mem::vector<GAME::GeometryBus::SimpleFace> &)
.rdata:0000000180703C60                 dq offset ?Detach@Actor@GAME@@UEAAXPEAVEntity@2@@Z ; GAME::Actor::Detach(GAME::Entity *)
.rdata:0000000180703C68                 dq offset ?OnTeleported@Actor@GAME@@UEAAXXZ ; GAME::Actor::OnTeleported(void)
.rdata:0000000180703C70                 dq offset ?SetParentLevel@Actor@GAME@@UEAAXI@Z ; GAME::Actor::SetParentLevel(uint)
.rdata:0000000180703C78                 dq offset ?UseAlternateMesh@ItemEquipment@GAME@@UEBA_N_N@Z ; GAME::ItemEquipment::UseAlternateMesh(bool)
.rdata:0000000180703C80                 dq offset ?ForcePoseUpdate@Actor@GAME@@UEAAXXZ ; GAME::Actor::ForcePoseUpdate(void)
.rdata:0000000180703C88                 dq offset ?AnimationCallback@Actor@GAME@@MEAA_NAEBVName@2@@Z ; GAME::Actor::AnimationCallback(GAME::Name const &)
.rdata:0000000180703C90                 dq offset ?PreAnimationUpdate@Actor@GAME@@MEAAXH@Z ; GAME::Actor::PreAnimationUpdate(int)
.rdata:0000000180703C98                 dq offset ?ShouldRenderForScene@Actor@GAME@@MEBA_NAEBVGraphicsScene@2@@Z ; GAME::Actor::ShouldRenderForScene(GAME::GraphicsScene const &)
.rdata:0000000180703CA0                 dq offset ?CreatePrimaryCursorHandler@Item@GAME@@UEAAPEAVCursorHandler@2@PEBVCharacter@2@@Z ; GAME::Item::CreatePrimaryCursorHandler(GAME::Character const *)
.rdata:0000000180703CA8                 dq offset ?TrackableTotalTime@Skill@GAME@@UEBA?BHI@Z ; GAME::Skill::TrackableTotalTime(uint)
.rdata:0000000180703CB0                 dq offset ?InitializeItem@ItemEquipment@GAME@@UEAAXXZ ; GAME::ItemEquipment::InitializeItem(void)
.rdata:0000000180703CB8                 dq offset ?UsePathPositionAsHome@ControllerMonsterStatePatrol@GAME@@MEBA_NXZ ; GAME::ControllerMonsterStatePatrol::UsePathPositionAsHome(void)
.rdata:0000000180703CC0                 dq offset ?IsOfInterest@Item@GAME@@UEBA_NXZ ; GAME::Item::IsOfInterest(void)
.rdata:0000000180703CC8                 dq offset ?CanBePlacedInTransferStash@Item@GAME@@UEBA_NXZ ; GAME::Item::CanBePlacedInTransferStash(void)
.rdata:0000000180703CD0                 dq offset ?PlayDropSound@Item@GAME@@UEAAXXZ ; GAME::Item::PlayDropSound(void)
.rdata:0000000180703CD8                 dq offset ?PlayDropSoundWorld@Item@GAME@@UEAAXW4PhysicsSurface@2@@Z ; GAME::Item::PlayDropSoundWorld(GAME::PhysicsSurface)
.rdata:0000000180703CE0                 dq offset ?SetDropSoundToPlay@Item@GAME@@UEAAXXZ ; GAME::Item::SetDropSoundToPlay(void)
.rdata:0000000180703CE8                 dq offset ?GetBitmap@ItemEquipment@GAME@@UEBAPEBVGraphicsTexture@2@XZ ; GAME::ItemEquipment::GetBitmap(void)
.rdata:0000000180703CF0                 dq offset ?GetBitmapName@ItemEquipment@GAME@@UEBAXAEAV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@@Z ; GAME::ItemEquipment::GetBitmapName(std::basic_string<char,std::char_traits<char>,std::allocator<char>> &)
.rdata:0000000180703CF8                 dq offset ?GetUIBitmapText@Item@GAME@@UEBAXAEAV?$basic_string@GU?$char_traits@G@std@@V?$allocator@G@2@@std@@_N@Z ; GAME::Item::GetUIBitmapText(std::basic_string<ushort,std::char_traits<ushort>,std::allocator<ushort>> &,bool)
.rdata:0000000180703D00                 dq offset ?GetUIBitmapOverlay@ItemEquipment@GAME@@UEBAPEBVGraphicsTexture@2@XZ ; GAME::ItemEquipment::GetUIBitmapOverlay(void)
.rdata:0000000180703D08                 dq offset ?GetSymbolBitmapName@Item@GAME@@UEBA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@XZ ; GAME::Item::GetSymbolBitmapName(void)
.rdata:0000000180703D10                 dq offset ?GetSimpleDescription@Item@GAME@@UEBA?AV?$basic_string@GU?$char_traits@G@std@@V?$allocator@G@2@@std@@XZ ; GAME::Item::GetSimpleDescription(void)
.rdata:0000000180703D18                 dq offset ?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z ; GAME::ItemEquipment::GetUIDisplayText(GAME::Character const *,mem::vector<GAME::GameTextLine> &,bool)



For Game::ItemRelic, we did the same.
Found:
.rdata:00000001807052C8 ??_7ItemRelic@GAME@@6BObject@1@@ dq offset ?GetStaticClassInfo@ItemRelic@GAME@@SAAEBVRTTI_ClassInfo@2@XZ
.rdata:00000001807052C8                                         ; DATA XREF: GAME::ItemRelic::ItemRelic(void):loc_1803167E1o
.rdata:00000001807052C8                                         ; GAME::ItemRelic::~ItemRelic(void)+1Co ...
.rdata:00000001807052C8                                         ; GAME::ItemRelic::GetStaticClassInfo(void)
.rdata:00000001807052D0                 dq offset ?Load@ItemRelic@GAME@@UEAAXAEBVLoadTable@2@@Z ; GAME::ItemRelic::Load(GAME::LoadTable const &)
....
.rdata:0000000180705700                 dq offset ?GetUIDisplayText@ItemRelic@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z ; GAME::ItemRelic::GetUIDisplayText(GAME::Character const *,mem::vector<GAME::GameTextLine> &,bool)

(0x180705700 - 0x1807052C8) / 8 = 0x438 / 8 = 0x87 = 135






On regular v1.2:
.rdata:0000000180635400 ??_7ItemEquipment@GAME@@6BObject@1@@ dq offset ?GetStaticClassInfo@ItemEquipment@GAME@@SAAEBVRTTI_ClassInfo@2@XZ
.rdata:0000000180635400                                         ; DATA XREF: GAME::ItemEquipment::ItemEquipment(void)+10o
.rdata:0000000180635400                                         ; GAME::ItemEquipment::~ItemEquipment(void)+1Co ...
.rdata:0000000180635400                                         ; GAME::ItemEquipment::GetStaticClassInfo(void)
.rdata:0000000180635408                 dq offset ?Load@ItemEquipment@GAME@@UEAAXAEBVLoadTable@2@@Z ; GAME::ItemEquipment::Load(GAME::LoadTable const &)
.rdata:0000000180635820                 dq offset ?GetUIDisplayText@ItemEquipment@GAME@@UEBAXPEBVCharacter@2@AEAV?$vector@UGameTextLine@GAME@@@mem@@_N@Z ; GAME::ItemEquipment::GetUIDisplayText(GAME::Character const *,mem::vector<GAME::GameTextLine> &,bool)


(0x180635820 - 0x180635408) / 8 = 0x418 / 8 = 0x83 = 131
*/