# PROJECT HANDOFF — Roguelike 2D Unity

> **Dashboard bàn giao giữa các phiên chat.**
> File này chỉ giữ trạng thái hiện tại + việc tiếp theo. Chi tiết kiến trúc xem `PROJECT_ARCHITECTURE.md`; lịch sử từng bước/test/bug xem `PROJECT_HISTORY.md`.
>
> **Cập nhật gần nhất: 2026-10-16.**

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
Tuần 6 - Thứ 2      DONE  — Enemy Architecture
Tuần 6 - Thứ 3      DONE  — Enemy Ranged

COMPLETED: 27 / 50 task
CURRENT: Tuần 6 - Thứ 4 — Damage System
STATUS: Đang làm
```

## Bổ sung sau Tuần 4 - Thứ 6 — Minimap Update — PLAY TEST PASS

Đã hoàn thành và người dùng đã Play Test xác nhận phần update Minimap.

Phạm vi đã hoàn thành:

```text
Room layout position của Edgar
        ↓
Chuẩn hóa thành các mức tọa độ X/Y cách đều trên Minimap
        ↓
CreateRoomIcon() đặt icon theo roomIconSpacing
        ↓
CreateRoomConnections() chạy sau khi toàn bộ icon đã có vị trí
        ↓
SetCurrentRoom()
        ↓
dịch RoomIconContainer + ConnectionContainer
        ↓
Current Room nằm tại tâm MinimapRoot
```

Đã chốt:
- Khoảng cách Room icon trên Minimap không còn phụ thuộc trực tiếp vào khoảng trống/kích thước layout Edgar.
- `roomIconSpacing` là biến Inspector để tuning khoảng cách cố định theo cả trục X và Y; giá trị Play Test hiện tại là `70`.
- Connection được tạo sau Room icon và dùng cùng tọa độ Minimap đã chuẩn hóa.
- `RectMask2D` trên `MinimapRoot` cắt phần icon/connection nằm ngoài vùng UI.
- Recenter chỉ dịch hai UI container, không build lại RoomContext hoặc graph Edgar.

Regression đã xác nhận:

```text
✓ Room lớn/nhỏ vẫn có khoảng cách Minimap theo spacing cố định.
✓ Connection nối đúng vị trí sau khi icon được tạo.
✓ Current Room luôn nằm ở tâm MinimapRoot khi Player đổi Room.
✓ Phần Minimap vượt ngoài MinimapRoot bị ẩn bởi RectMask2D.
✓ Regenerate nhiều seed không giữ icon/connection/reference cũ.
✓ Console không có lỗi đỏ trong test cuối.
```

## Tuần 6 - Thứ 2 — Enemy Architecture — DONE

Đã Play Test regression sau refactor.

```text
EnemyAI
├── Idle
├── Chase
└── Attack
        ↓
EnemyAttackBehaviour
├── MeleeEnemyAttack
└── RangedEnemyAttack
```

Đã chốt:
- `EnemyData` tiếp tục giữ stat/config của Enemy.
- `EnemyAI` giữ detection, state và Chase; không ôm chi tiết Attack cụ thể.
- Attack được tách thành `EnemyAttackBehaviour` dạng `abstract MonoBehaviour` để Unity Inspector có thể tham chiếu component cụ thể.
- `MeleeEnemyAttack` giữ contact detection, cooldown, telegraph và melee damage.
- `EnemyAI` gọi `CanAttack()` để quyết định vào Attack State và gọi `UpdateAttack()` khi đang Attack.
- `ResetAttack()` được gọi khi Enemy chết/disable/reuse để giữ pooling contract.
- Chưa tạo `IEnemyMovement` / `IEnemyAttack`; interface chỉ được cân nhắc khi có nhu cầu chung lớn hơn thực tế hiện tại.
- Chưa cài A* vì chưa có bằng chứng direct chase thường xuyên kẹt trong dungeon.

## Edgar — Multiple LevelGraph Variants — DONE (bổ sung kiến trúc)

**Người dùng đã Play Test và xác nhận flow random LevelGraph hoạt động đúng.**

Phạm vi bổ sung này không tạo task roadmap mới; đây là điều chỉnh kiến trúc cho Edgar Free hiện tại.

### Cấu hình hiện tại
```text
FloorEdgarConfig
├── FloorData
└── List<LevelGraph>
    ├── LevelGraph A
    ├── LevelGraph B
    └── LevelGraph C
```

- `FloorEdgarConfig` dùng `List<LevelGraph>` thay cho một `LevelGraph` duy nhất.
- Mỗi `LevelGraph` có thể có số room khác nhau.
- `FloorData.RoomCount` không còn được `FloorEdgarConfig` dùng để validate số room của Edgar Graph.
- Số room thực tế của dungeon layout lấy trực tiếp từ `LevelGraph.Rooms.Count`.
- `EdgarDungeonGenerator` random chọn một `LevelGraph` trước khi gọi `DungeonGeneratorGrid2D.Generate()`.
- Room Template vẫn được cấu hình thủ công trên từng node của từng LevelGraph trong Edgar Graph Editor.
- Không sửa source Edgar và không thêm Manager/framework mới.

### Runtime flow
```text
FloorEdgarConfig.List<LevelGraph>
        ↓
EdgarDungeonGenerator
        ↓
Random chọn 1 LevelGraph
        ↓
FixedLevelGraphConfig.LevelGraph
        ↓
DungeonGeneratorGrid2D.Generate()
```

### Test đã xác nhận
```text
✓ Nhiều LevelGraph được cấu hình trong FloorEdgarConfig.
✓ Random chọn Graph trước Generate.
✓ Các Graph không cần cùng số room.
✓ Console không có lỗi trong test cuối.
```

## Tuần 6 - Thứ 3 — Enemy Ranged — DONE

Đã Play Test toàn bộ flow Ranged và regression pooling.

```text
EnemyAI
    ↓
RangedEnemyAttack.CanAttack()
    ↓
Player trong Attack Range
    ↓
Attack State
    ↓
Telegraph trên Visual
    ↓
EnemyProjectilePool.GetProjectile()
    ↓
Projectile.Initialize()
    ↓
Player / Environment collision
    ↓
ReturnToPool
```

Đã hoàn thành:
- Tạo `RangedEnemyAttack` dùng chung `EnemyAttackBehaviour` với Melee.
- Có `attackRange` riêng, tách khỏi `detectionRange`.
- Telegraph bằng DOTween trên child `Visual`; không tween Rigidbody2D root.
- Có `FirePoint` và tính hướng bắn từ Enemy tới Player.
- Projectile được lấy từ `ProjectilePool`, không Instantiate/Destroy mỗi lần bắn.
- `ProjectilePool` được cấp cho Ranged Enemy bằng runtime injection; không kéo Scene reference cứng vào prefab.
- `EnemyData.AttackDamage` được dùng làm nguồn damage cho Ranged projectile và đã test thay đổi damage theo data.
- Enemy death giữa telegraph không bắn projectile muộn.
- Enemy Pool reuse reset được Attack state/cooldown/coroutine/tween.
- Projectile Pool reuse hoạt động đúng.

Regression đã xác nhận:
- Idle → Chase → Attack đúng theo Detection/Attack Range.
- Telegraph → bắn projectile đúng hướng.
- Projectile trúng Player gây damage đúng và ReturnToPool.
- Projectile trúng Environment ReturnToPool.
- Cooldown hoạt động.
- Enemy chết giữa telegraph không gây damage/bắn projectile sau death.
- Enemy reuse có thể Attack lại sạch.
- Projectile reuse không giữ state cũ.
- Console không có lỗi đỏ trong test cuối task.

## Bổ sung sau Tuần 6 — Enemy Pool hỗ trợ nhiều loại Enemy — DONE

Đã Play Test sau khi mở rộng và dọn cleanup legacy.

```text
EnemySpawnPoint.EnemyPrefab
        ↓
RoomController.GetEnemy(requestedEnemyPrefab, ...)
        ↓
EnemyPool tìm pool con theo Enemy prefab
        ↓
EnemyHealth / EnemyAI reset state
        ↓
Enemy runtime được parent vào Room
```

Đã chốt:
- Một `EnemyPool` Scene object quản lý nhiều `ObjectPool<EnemyHealth>` nội bộ.
- Mỗi `EnemyPoolEntry` tương ứng với một Enemy prefab và capacity riêng.
- Mỗi `EnemySpawnPoint` có thể chọn một Enemy prefab khác nhau.
- `EnemyPool` tracking pool sở hữu từng Enemy đang rented để release đúng pool con.
- `EnemyPool` đã xoá field default Enemy và overload `GetEnemy(...)` cũ sau khi flow mới được Play Test.
- `EnemyData` tiếp tục là stat/config được Enemy prefab tham chiếu; không dùng làm lựa chọn spawn tại SpawnPoint.

Regression đã xác nhận:

```text
✓ Một Room spawn được nhiều loại Enemy từ các SpawnPoint khác nhau.
✓ Enemy spawn đúng vị trí và đúng prefab đã cấu hình.
✓ Enemy chết được release về đúng pool con.
✓ Enemy reuse reset state đúng.
✓ ReleaseAllRentedEnemies() vẫn cleanup được nhiều loại Enemy khi đổi Floor.
✓ Console không có lỗi đỏ sau cleanup legacy.
```

Đây là bổ sung kiến trúc cho Object Pooling, không tạo task roadmap mới và không làm tăng tổng số task DONE.

---

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

# 5. Current Task

## Tuần 6 — Thứ 4: Damage System
**ĐANG LÀM — chưa bắt đầu sửa code trong task này.**

Theo V4.1:
> Bổ sung `DamageCalculator` cho critical/knockback khi thật sự cần. Chuẩn hóa `DamageInfo` để truyền `amount`, `source`, `knockback`; tránh nhồi nhiều tham số rời vào `TakeDamage`.

Mục tiêu trước mắt:

```text
Review damage flow hiện tại
        ↓
Player Projectile → ProjectileCollision → EnemyHealth
Enemy Contact    → DamageSource / Enemy Attack → PlayerHealth
        ↓
Xác định dữ liệu damage đang truyền bằng int rời
        ↓
Chốt DamageInfo tối thiểu
        ↓
Đánh giá DamageCalculator chỉ nếu critical/knockback thật sự cần
        ↓
Refactor từng đường damage
        ↓
Play Test regression
```

Ranh giới bắt buộc:
- Không tạo `DamageCalculator` chỉ để có thêm class nếu chưa có công thức cần tính.
- Không đưa critical/knockback vào gameplay nếu task hiện tại chưa chứng minh cần.
- `DamageInfo` phải có tối thiểu `amount`, `source`, `knockback` theo roadmap; có thể để giá trị mặc định khi mechanic chưa dùng.
- Không phá `EnemyHealth.OnDamaged` / `OnDeath`, `PlayerHealth.OnHealthChanged` / `OnDeath`.
- Không phá projectile/enemy pooling.
- Không để damage logic phụ thuộc HUD.

---

# 6. First Next Step cho chat tiếp theo

Trước khi viết `DamageInfo`, cần đọc và vẽ **damage flow hiện tại** của các đường sau:

```text
Player Projectile
→ ProjectileCollision
→ EnemyHealth.TakeDamage(...)

Enemy Contact / Melee
→ MeleeEnemyAttack
→ PlayerHealth.TakeDamage(...)

Enemy Projectile
→ EnemyProjectileCollision / ProjectileCollision hiện tại
→ PlayerHealth.TakeDamage(...)
```

Cần xác định rõ:
1. Class nào là nơi tạo damage request.
2. Class nào là nơi nhận damage.
3. Damage hiện đang truyền những dữ liệu gì ngoài `amount`.
4. `source` nên là object/reference nào ở từng trường hợp.
5. Knockback hiện chưa có hay đã có một phần ở code current.

Sau khi review xong mới chốt cấu trúc `DamageInfo`. **Chưa tự viết DamageCalculator trước bước review này.**

---

# 7. Những thứ quan trọng CHƯA làm

Không tự giả định đã có:
- `DamageInfo` / `DamageCalculator` hoàn chỉnh.
- DOTween pop/hút pickup ngoài world.
- Player nhặt Item lớn / Item Effects/stat modifier.
- NextFloor hoàn chỉnh, floor index và RunProgress.
- Enemy Spawn Marker Tile (mới là hướng cải tiến về sau).
- A* Pathfinding.
- Boss system hoàn chỉnh / Boss sequence orchestration.
- Save / Audio / Main Menu / Pause / GameOver.
- `IEnemyMovement` / `IEnemyAttack` interface; hiện chưa cần thêm tầng interface riêng.

Đã có và đã Play Test:
- Physical Coin/Heart/Key/Bomb pickup.
- Pickup update `PlayerResources` / `PlayerHealth` và ReturnToPool.
- Item pedestal world reward theo Reward/Boss Item Pool.
- FloorExit cơ bản sau Boss Clear.
- Gameplay Marker Tilemap cho Small Pickup / Item Reward / FloorExit.
- Loot Data weighted random cho pickup nhỏ và Item Pool.
- Enemy/Drop/DamageText pooling.
- Room lifecycle + Room transition + Cinemachine + Minimap.
- Enemy Architecture State Machine tối thiểu.
- Enemy Ranged + Projectile Pool integration.
- Prototype Next Floor trong cùng Gameplay scene: FloorExit → cleanup runtime pool → Edgar generate Floor config tiếp theo → rebuild Room/Minimap → Player về Start Room mới.

## Prototype Next Floor — CODE COMPLETE — UNITY PLAY TEST PASS

Phạm vi prototype đã xác nhận:

```text
Boss clear
    ↓
FloorExit spawn trong Boss Room
    ↓
Player chạm FloorExit
    ↓
FloorTransitionPrototype chọn FloorEdgarConfig kế tiếp
    ↓
Enemy / Projectile / Drop đang rented được trả về pool
    ↓
Edgar xóa Generated Level cũ và generate lại
    ↓
EdgarDungeonPostProcessing rebuild Room, Door, Minimap và runtime references
    ↓
RoomTransitionController đưa Player về PlayerSpawnPoint của Start Room mới
```

Đã thêm/điều chỉnh contract prototype:

- `EnemyPool`, `ProjectilePool`, `DropPool` tracking object đang rented và có hàm `ReleaseAllRented...()` trước khi đổi Floor.
- `FloorExit` được nối qua `FloorExitSpawner` tới `FloorTransitionPrototype` bằng runtime injection.
- `FloorExit` và `ItemPedestal` được parent vào Room runtime để Edgar cleanup cùng dungeon cũ.
- `EdgarDungeonGenerator` nhận `FloorEdgarConfig` mới và generate lại trong cùng scene.
- `FloorTransitionPrototype` giữ danh sách Floor config theo thứ tự và floor index tạm thời.

Giới hạn prototype:

- Chưa có `RunProgress` production.
- Chưa có Branch Point, Surface/DeepEarth route hoặc SelectedRoute.
- Chưa có save/load progression, End Run hoặc loading/fade transition.
- Đây chưa phải task `Boss Flow` hoàn chỉnh của Tuần 7.

---

# 8. Known Issues / Blocker hiện tại

**Không có blocker đã biết.**

Prototype Next Floor đã Play Test PASS. Roadmap chính thức vẫn giữ `NextFloor hoàn chỉnh / floor index / RunProgress` ở Tuần 7; không đánh dấu task roadmap này DONE chỉ từ prototype.

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
