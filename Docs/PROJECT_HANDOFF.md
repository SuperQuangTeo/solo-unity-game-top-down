# PROJECT HANDOFF — Roguelike 2D Unity

> **Dashboard bàn giao giữa các phiên chat.**
> File này chỉ giữ trạng thái hiện tại + việc tiếp theo. Chi tiết kiến trúc xem `PROJECT_ARCHITECTURE.md`; lịch sử từng bước/test/bug xem `PROJECT_HISTORY.md`.
>
> **Cập nhật gần nhất: 2026-09-16.**

---

# 1. Bộ tài liệu chuẩn

```text
Kế hoạch Roguelike 2D V4.1 - UPDATED 2026-09-16.xlsx
→ Roadmap chính thức: thứ tự task bắt buộc.

PROJECT_HANDOFF.md
→ Trạng thái hiện tại: đang ở đâu, task nào tiếp theo.

PROJECT_ARCHITECTURE.md
→ Kiến trúc hiện tại: module, folder, data flow, plugin, contract kỹ thuật.

PROJECT_HISTORY.md
→ Nhật ký: task DONE, từng bước đã làm, bug, regression đã test.
```

Quy tắc tiến độ:
- Bám đúng thứ tự task trong V4.1.
- Không tự nhảy task hoặc tạo Manager/framework lớn khi chưa thật sự cần.
- Nếu roadmap/dependency có điểm chưa hợp lý thì trao đổi trước khi sửa.
- Chỉ đánh dấu DONE sau khi người dùng Play Test và xác nhận.

---

# 2. Current Progress

```text
Tuần 1              DONE
Tuần 2 - Thứ 2      DONE
Tuần 2 - Thứ 3      DONE
Tuần 2 - Thứ 4      DONE
Tuần 2 - Thứ 5      DONE
Tuần 2 - Thứ 6      DONE
Tuần 3 - Thứ 2      DONE
Tuần 3 - Thứ 3      DONE
Tuần 3 - Thứ 4      DONE
Tuần 3 - Thứ 5      DONE
Tuần 3 - Thứ 6      DONE
Tuần 4 - Thứ 2      DONE  — Edgar Setup & Prototype
Tuần 4 - Thứ 3      DONE  — Edgar Room Templates
Tuần 4 - Thứ 4      DONE  — Edgar Graph & Floor Data
Tuần 4 - Thứ 5      DONE  — Edgar Runtime Integration
Tuần 4 - Thứ 6      DONE  — Minimap từ Edgar Data
Tuần 5 - Thứ 2      DONE  — Room Lifecycle + Edgar
Tuần 5 - Thứ 3      DONE  — Room Transition + Cinemachine
Tuần 5 - Thứ 4      DONE  — Object Pooling
Tuần 5 - Thứ 5      DONE  — Loot Data
Tuần 5 - Thứ 6      DONE  — Loot & Floor Flow (scope đã chốt)

COMPLETED: 25 / 50 task
```

## Tuần 5 - Thứ 6 — trạng thái cuối đã Play Test

Phạm vi người dùng **chủ động chốt làm**:

```text
1. Small Pickup spawn ngoài world
2. Player nhặt Coin / Heart / Key / Bomb
4. Item lớn spawn pedestal từ Item Pool
5. FloorExit cơ bản sau Boss Clear
```

Phần **chủ động hoãn**:

```text
DOTween pop / hút pickup
→ visual polish, chưa ưu tiên

NextFloor hoàn chỉnh / floor index / progression
→ chưa làm ở Tuần 5
→ sẽ nối với RunProgress / Boss Flow ở Tuần 7
```

### Small Pickup runtime

```text
RoomController.OnRoomCleared
        ↓
RoomLootRoller
        ↓
RoomClearLootTable.RollLoot()
        ↓
LootEntry
        ↓
RoomLootDropSpawner
        ↓
SmallPickupMarkerTile
        ↓
DropPool.GetDrop()
        ↓
PooledDrop.Configure(RewardType, Amount)
```

Đã xác nhận:
- `None` → không spawn object.
- Coin / Heart / Key / Bomb → spawn đúng cell marker.
- `PooledDrop` dùng một collider chung cho các sprite pickup ở giai đoạn hiện tại.
- `DropPool` vẫn chỉ quản lý reuse; không chứa luật reward.
- `EdgarDungeonPostProcessing` inject `DropPool` runtime cho `RoomLootDropSpawner`, giống pattern `EnemyPool`.

### Pickup collect

```text
Player chạm PooledDrop
        ↓
PooledDropPickup
        ↓
Coin → PlayerResources.AddCoins()
Key  → PlayerResources.AddKeys()
Bomb → PlayerResources.AddBombs()
Heart → PlayerHealth.TryHeal()
        ↓
áp dụng thành công
        ↓
PooledDrop.ReturnToPool()
```

Quy tắc Heart:
- `1 HP = nửa Heart` theo HUD hiện tại.
- Full HP → `TryHeal()` trả `false` → Heart **không bị tiêu thụ**.
- Thiếu HP → heal thành công → HUD cập nhật qua `OnHealthChanged` → Drop về pool.
- `Amount > 1` đã test truyền xuyên `LootEntry → PooledDrop → PlayerResources/PlayerHealth` đúng.

### Item lớn

```text
Reward Room
→ RewardItemPool
→ ItemRewardRoller
→ RoomItemRewardSpawner
→ ItemRewardMarkerTile
→ ItemPedestal

Boss Room
→ Boss clear
→ BossItemPool
→ ItemRewardRoller
→ RoomItemRewardSpawner
→ ItemRewardMarkerTile
→ ItemPedestal
```

Đã xác nhận:
- Reward Room chỉ roll item thuộc Reward Pool.
- Boss Room chỉ roll item thuộc Boss Pool.
- Item cùng pool dùng Weight.
- Item chỉ roll một lần mỗi Room.
- `ItemPedestal` hiện chỉ hiển thị `ItemData`; **Item Effects chưa làm**.

### FloorExit cơ bản

```text
Boss chưa clear
→ chưa có FloorExit

Boss clear
→ FloorExitSpawner
→ FloorExitMarkerTile
→ spawn FloorExit prefab đúng tâm cell

Player chạm FloorExit
→ FloorExit.OnPlayerEnteredExit
→ hiện mới phát tín hiệu / log
→ chưa NextFloor
```

`FloorExit.prefab` vẫn cần vì Marker Tile chỉ giữ **vị trí**, còn prefab giữ `Collider2D + FloorExit.cs` và interaction gameplay.

---

# 3. Thay đổi kiến trúc lớn — Gameplay Marker Tilemap

Đây là thay đổi lớn nhất cuối Tuần 5 và phải được ưu tiên khi đọc tài liệu cũ.

## Trước đây

```text
RewardSpawnPoint (Empty Transform)
→ vị trí pickup / item

FloorExitSpawnPoint (Empty Transform)
→ vị trí FloorExit
```

## Hiện tại

Hai marker Transform trên đã được loại bỏ khỏi flow hiện tại.

```text
GameplayMarkers (Tilemap)
├── SmallPickupMarkerTile
├── ItemRewardMarkerTile
└── FloorExitMarkerTile
        ↓
RoomMarkerTilemap
        ↓
Tilemap.GetCellCenterWorld(cell)
        ↓
World Position chính xác theo Grid
```

Quy tắc:
- Marker Tile **không phải child GameObject** và không phải gameplay object.
- Marker là `TileBase` asset được paint vào `GameplayMarkers` Tilemap.
- Marker chỉ trả lời: **"spawn ở cell nào?"**
- GameObject/prefab thật vẫn trả lời: **"gameplay làm gì?"**
- `GameplayMarkers` renderer có thể ẩn lúc runtime; data Tilemap vẫn đọc được.
- Edgar di chuyển Room runtime thì marker đi cùng Room và vẫn lấy đúng world position.

Đã migrate sang Marker Tile:

```text
Small Pickup
Item Pedestal
FloorExit
```

Chưa migrate:

```text
EnemySpawnPoint
PlayerSpawnPoint
```

Người dùng đã xác nhận **có thể cân nhắc Enemy Spawn Marker Tile về sau**, nhưng chưa refactor vì Enemy spawning hiện đang ổn và không thuộc task hiện tại.

`RewardSpawnPoint.cs` và `FloorExitSpawnPoint.cs` không còn thuộc kiến trúc current sau cleanup.

---

# 4. Current runtime reward flow

```text
COMBAT ROOM CLEAR
        ↓
RoomController.OnRoomCleared
        ↓
RoomLootRoller
        ↓
RoomClearLootTable
        ↓
None / Coin / Heart / Key / Bomb
        ↓
RoomLootDropSpawner
        ↓
SmallPickupMarkerTile
        ↓
DropPool / PooledDrop
        ↓
PooledDropPickup
        ↓
PlayerResources / PlayerHealth
```

```text
REWARD ROOM
        ↓
RoomContext.ItemPoolData = RewardItemPool
        ↓
ItemRewardRoller
        ↓
RoomItemRewardSpawner
        ↓
ItemRewardMarkerTile
        ↓
ItemPedestal
```

```text
BOSS CLEAR
        ↓
RoomController.OnRoomCleared
        ├── RoomItemRewardSpawner
        │       ↓
        │   BossItemPool
        │       ↓
        │   ItemRewardMarkerTile
        │       ↓
        │   ItemPedestal
        │
        └── FloorExitSpawner
                ↓
            FloorExitMarkerTile
                ↓
            FloorExit prefab
```

Không có `BossRewardManager` ở giai đoạn hiện tại. Hai listener độc lập vẫn đơn giản. Chỉ cân nhắc coordinator/manager khi Boss Clear có nhiều bước phụ thuộc thứ tự (camera focus, delay, chest, victory sequence, input lock...).

---

# 5. Next Task

## Tuần 6 — Thứ 2: Enemy Architecture
**CHƯA BẮT ĐẦU.**

Theo V4.1:

> Refactor `EnemyAI` cơ bản thành State Machine vừa đủ `Idle / Chase / Attack`. Tách state logic khỏi visual; `EnemyData` tiếp tục giữ stat. Chỉ tách Movement/Attack thành component/interface khi behavior thứ hai thực sự xuất hiện. A* Pathfinding vẫn là decision gate, không cài nếu direct chase vẫn đủ tốt.

Mục tiêu kế tiếp:

```text
EnemyAI hiện tại
├── detection
├── chase
├── contact attack
├── telegraph
├── cooldown
├── death/reset pooling
└── runtime Player reference
        ↓
review trách nhiệm hiện tại
        ↓
chốt State Machine tối thiểu
        ↓
Idle
Chase
Attack
        ↓
không phá EnemyPool / ResetForReuse / death event
```

Ranh giới bắt buộc:
- Không tạo Behavior Tree/framework lớn.
- Không tạo `IEnemyMovement` / `IEnemyAttack` chỉ để "đẹp" nếu chưa có implementation thứ hai.
- Không kéo Enemy Ranged của Thứ 3 lên trước khi State Machine cơ bản đã test.
- Không cài A* nếu chưa chứng minh direct chase thường xuyên kẹt trong dungeon thực tế.
- Pooling contract phải giữ nguyên: Enemy reuse phải reset state/state machine sạch.

---

# 6. First Next Step cho chat tiếp theo

Trước khi sửa code Enemy Architecture, cần đọc code current:

```text
EnemyAI.cs
EnemyData.cs
EnemyHealth.cs
EnemyPool.cs
```

Sau đó làm theo thứ tự nhỏ:

```text
1. Vẽ flow EnemyAI hiện tại.
2. Chỉ ra state nào đang ẩn trong các bool hiện có.
3. Chốt enum/state tối thiểu Idle / Chase / Attack.
4. Refactor từng state một, giữ behavior cũ.
5. Play Test regression trước khi tách thêm component/interface.
```

Không coi task DONE cho tới khi người dùng Play Test Enemy cơ bản sau refactor.

---

# 7. Những thứ quan trọng CHƯA làm

Không tự giả định đã có:
- DOTween pop/hút pickup ngoài world.
- Player nhặt Item lớn / Item Effects/stat modifier.
- NextFloor hoàn chỉnh, floor index và RunProgress.
- Enemy State Machine nâng cao / Enemy Ranged.
- DamageInfo / DamageCalculator hoàn chỉnh.
- Enemy Spawn Marker Tile (mới là hướng có thể làm sau).
- Large Room camera bounds/follow riêng.
- A* Pathfinding.
- Boss system hoàn chỉnh.
- Save / Audio / Main Menu / Pause / GameOver.
- Player prefab hóa đã được xác nhận hoàn tất.

Đã có và đã Play Test:
- Physical Coin/Heart/Key/Bomb pickup.
- Pickup update `PlayerResources` / `PlayerHealth` và ReturnToPool.
- Item pedestal world reward theo Reward/Boss Item Pool.
- FloorExit cơ bản sau Boss Clear.
- Gameplay Marker Tilemap cho Small Pickup / Item Reward / FloorExit.
- Loot Data weighted random cho pickup nhỏ và Item Pool.
- Enemy/Drop/DamageText pooling.
- Room lifecycle + Room transition + Cinemachine + Minimap.

---

# 8. Known Issues / Blocker hiện tại

**Không có blocker đã biết.**

Ghi chú Edgar Editor cũ:
- Từng xuất hiện `SerializedObjectNotCreatableException` từ Custom Inspector Edgar sau compile.
- Lock/unlock Inspector rồi compile lại đã hết; runtime bình thường.
- Không sửa source Edgar nếu lỗi không tái diễn ổn định.

---

# 9. Quy trình cập nhật tài liệu

Sau mỗi task lớn hoặc trước khi đổi chat:
1. `PROJECT_HANDOFF.md` → Current Progress / Next Task / First Next Step / blocker.
2. `PROJECT_HISTORY.md` → ghi task DONE, flow, test pass, bug/thay đổi thiết kế đáng nhớ.
3. `PROJECT_ARCHITECTURE.md` → cập nhật khi contract/module/data flow thay đổi.
4. Excel V4.1 → cập nhật trạng thái task và tổng tiến độ; nếu scope roadmap thay đổi phải ghi rõ phần deferred, không giả định đã làm.
