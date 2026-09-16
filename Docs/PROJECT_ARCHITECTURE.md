# PROJECT ARCHITECTURE — Roguelike 2D Unity

> Tài liệu kiến trúc và quy ước kỹ thuật hiện tại của project.
> Đọc file này khi cần hiểu **project được tổ chức như thế nào, module nào chịu trách nhiệm gì, data/event đi qua đâu, plugin được dùng ra sao**.
> Không cần đọc toàn bộ file này mỗi khi mở chat mới. Trạng thái làm việc hiện tại nằm trong `PROJECT_HANDOFF.md`.

---

## Cách dùng nhanh

- Cần biết **đang làm task nào** → đọc `PROJECT_HANDOFF.md`.
- Cần biết **task cũ đã làm/test gì** → đọc `PROJECT_HISTORY.md`.
- Cần biết **kiến trúc, folder, module, plugin, quy tắc code** → đọc file này.
- Roadmap và thứ tự task chính thức vẫn lấy từ file Excel V4.1.

---

# 3. PlayerData hiện tại

`PlayerData` là ScriptableObject dùng để chứa các thông số cấu hình Player.

Asset hiện tại:
```text
Assets/Data/Player/DefaultPlayerData.asset
```

## Movement
- `MoveSpeed`
- `Acceleration`
- `Deceleration`

Giá trị test:
```text
Move Speed   = 5
Acceleration = 20
Deceleration = 25
```

Lưu ý:
- `MoveSpeed` đã dùng thật.
- `Acceleration` và `Deceleration` mới chỉ được khai báo trong data, chưa có logic thật.

## Dash
- `DashSpeed`
- `DashDuration`
- `DashCooldown`

Giá trị test:
```text
Dash Speed    = 12
Dash Duration = 0.2
Dash Cooldown = 0.6
```

Quy ước hiện tại:
```text
Dash Duration + Dash Cooldown
= thời gian từ lúc bắt đầu Dash đến lần Dash tiếp theo
```

## Shooting
- `ProjectileSpeed`
- `FireInterval`
- `ProjectileSpawnOffset`
- `ProjectileLifetime`

Giá trị test:
```text
Projectile Speed        = 10
Fire Interval           = 0.25
Projectile Spawn Offset = 0.5
Projectile Lifetime     = 3
```

## Health
- `MaxHealth`
- `InvincibilityDuration`

Giá trị test:
```text
Max Health              = 6
Invincibility Duration  = 0.5
```

Quy tắc:
- `PlayerData.MaxHealth` là cấu hình gốc, không bị trừ trực tiếp khi Player nhận damage.
- `PlayerHealth.currentHealth` mới là HP runtime thay đổi trong lúc chơi.
- `InvincibilityDuration` quy định khoảng bất tử ngắn sau một hit hợp lệ.

---

# 4. PlayerController hiện tại

Trách nhiệm:
> Chỉ đọc Input System và chuyển ý định của người chơi sang module tương ứng.

```text
Move Input  → PlayerController → PlayerMovement
Dash Input  → PlayerController → PlayerDash
Shoot Input → PlayerController → PlayerShooter
```

Quy tắc:
- Move là input liên tục → đọc mỗi frame.
- Dash là hành động một lần → dùng InputAction `performed`.
- Dash event đăng ký bằng `+=` trong `OnEnable`.
- Hủy đăng ký bằng `-=` trong `OnDisable`.

---

# 5. PlayerMovement hiện tại

Trách nhiệm:
> Là nơi duy nhất điều khiển chuyển động vật lý của Rigidbody2D Player.

Không để `PlayerDash` trực tiếp di chuyển Rigidbody2D.

```text
PlayerMovement.FixedUpdate()

Nếu không Dash:
    MoveNormally()

Nếu đang Dash:
    MoveDuringDash()
```

PlayerDash chỉ gọi:
```text
StartDash(direction, speed)
StopDash()
```

Lý do:
- Tránh nhiều script cùng giành quyền điều khiển một Rigidbody2D.
- Dễ debug movement.
- Physics tập trung tại một nơi.

---

# 6. PlayerDash hiện tại

Trách nhiệm:
- Kiểm tra có được Dash không.
- Kiểm tra hướng Dash.
- Quản lý Dash duration.
- Quản lý cooldown.
- Gọi `PlayerMovement.StartDash()`.
- Gọi `PlayerMovement.StopDash()`.
- Chạy hiệu ứng Visual khi Dash.

Luồng:
```text
Space
 ↓
PlayerController
 ↓
PlayerDash.TryDash()
 ↓
Kiểm tra cooldown
 ↓
Lấy CurrentMoveDirection
 ↓
StartDash()
 ↓
chờ DashDuration
 ↓
StopDash()
 ↓
chờ DashCooldown
 ↓
cho Dash lại
```

Hiện tại:
- Không có input hướng → không Dash.
- Chưa lưu `lastMoveDirection`.
- Không tự thêm chức năng đứng yên vẫn Dash theo hướng cuối.

---

# 7. DOTween trên Player

Hiệu ứng Dash dùng `DOPunchScale()` trên:
```text
Player/Visual
```

Không áp dụng lên root `Player`.

Trước khi tạo tween mới có dùng `DOKill()` để tránh nhiều tween cùng sửa scale Visual.

Quy tắc chung:
> DOTween dùng cho visual/game feel/UI. Physics gameplay chính ưu tiên Rigidbody2D và logic vật lý.

---

# 8. Tuần 2 — Thứ 4: Player Combat
**DONE.**

Đã hoàn thành và test:
- `PlayerShooter.cs`
- `Projectile.cs`
- `ProjectilePool.cs`
- `ProjectileCollision.cs`
- `PlayerProjectile.prefab`
- `FirePoint`
- Shooting data trong `PlayerData`
- Fire Interval
- Projectile Lifetime
- Projectile Pool bằng `UnityEngine.Pool.ObjectPool<T>`
- Projectile trả về pool khi hết lifetime.
- Projectile trả về pool khi va Layer hợp lệ.
- Layer không hợp lệ được bỏ qua.
- Projectile object được tái sử dụng, không tăng vô hạn khi bắn lâu.

---

# 9. PlayerShooter hiện tại

Trách nhiệm:
- Nhận `shootDirection` từ PlayerController.
- Kiểm tra FireInterval.
- Lấy Projectile từ ProjectilePool.
- Tính spawn position từ FirePoint.
- Initialize Projectile.

```text
Arrow Keys
   ↓
PlayerController
   ↓
PlayerShooter.SetShootDirection()
   ↓
TryShoot()
   ↓
kiểm tra FireInterval
   ↓
ProjectilePool.GetProjectile()
   ↓
tính spawnPosition
   ↓
Projectile.Initialize(direction, speed, lifetime)
```

FirePoint là GameObject con của Player, không phải script.

Spawn position:
```text
FirePoint Position
+
Shoot Direction × ProjectileSpawnOffset
```

---

# 10. Projectile Prefab hiện tại

```text
PlayerProjectile
├── Rigidbody2D
├── CircleCollider2D
├── Projectile
├── ProjectileCollision
└── Visual
    └── SpriteRenderer
```

Cấu hình:
- Rigidbody2D:
  - Dynamic
  - Gravity Scale = 0
  - Freeze Rotation Z
- CircleCollider2D:
  - Is Trigger = true
- Root giữ physics.
- Child Visual giữ SpriteRenderer.
- Projectile movement dùng Rigidbody2D.
- Không dùng DOTween để di chuyển Projectile gameplay chính.

---

# 11. Projectile hiện tại

Trách nhiệm:
- Lưu hướng bay.
- Lưu tốc độ.
- Di chuyển bằng Rigidbody2D trong FixedUpdate.
- Quản lý Lifetime.
- Biết Pool nào sở hữu nó.
- Tự ReturnToPool khi Lifetime hết.
- Reset state khi được Release.

State quan trọng:
- `moveDirection`
- `moveSpeed`
- `remainingLifetime`
- `isActive`
- `ownerPool`

---

# 12. ownerPool — quyết định kiến trúc quan trọng

Projectile có `ownerPool`.

Mục đích:
> Mỗi Projectile nhớ chính xác Pool nào đã tạo/quản lý nó.

Ví dụ tương lai:
```text
PlayerProjectilePool
EnemyProjectilePool
BossProjectilePool
```

Luồng:
```text
ProjectilePool.CreateProjectile()
    ↓
projectile.SetOwnerPool(this)
    ↓
Projectile ghi nhớ ownerPool
    ↓
Projectile dùng xong
    ↓
ownerPool.ReleaseProjectile(this)
```

Không dùng:
- Singleton cho ProjectilePool.
- `FindObjectOfType`.
- GameManager để quản lý pool đạn.

---

# 13. ProjectilePool hiện tại

Dùng:
```text
UnityEngine.Pool.ObjectPool<Projectile>
```

Callback:
```text
CreateProjectile
OnGetProjectile
OnReleaseProjectile
OnDestroyProjectile
```

Luồng:
```text
Get()
 ↓
Nếu Pool có Projectile → dùng lại
Nếu Pool rỗng → CreateProjectile() → Instantiate một lần
```

Khi Release:
```text
Projectile.Stop()
 ↓
SetActive(false)
 ↓
quay lại Pool
```

Quan trọng:
`defaultCapacity` không có nghĩa Unity tạo sẵn từng đó Projectile khi Play.

### Debug đã xác nhận — Stop Play Mode / ObjectPool.Clear()

Đã từng gặp:
```text
MissingReferenceException:
Projectile has been destroyed but you are still trying to access it
```

Stack trace chỉ tới:
```text
ProjectilePool.OnDestroyProjectile(...)
ObjectPool<T>.Clear()
PoolManager.Reset()
```

Nguyên nhân:
- Khi thoát Play Mode, Unity có thể đã Destroy Projectile trước.
- Sau đó ObjectPool tiếp tục Clear và callback `OnDestroyProjectile()` nhận reference Unity Object đã bị destroy.
- Truy cập `projectile.gameObject` lúc đó gây `MissingReferenceException`.

Cách đã sửa và test:
```text
OnDestroyProjectile(projectile)
↓
nếu projectile == null → return
↓
nếu còn tồn tại → Destroy(projectile.gameObject)
```

Không bỏ callback Destroy vì nó vẫn cần khi Pool vượt `maxSize` trong runtime.

---

# 14. ProjectileCollision hiện tại

Trách nhiệm:
> Lọc collision bằng LayerMask, gây damage cho Enemy nếu collider có `EnemyHealth`, sau đó yêu cầu Projectile ReturnToPool.

Dùng:
```text
LayerMask collisionLayers
```

Không hard-code bằng nhiều `CompareTag()`.

Luồng hiện tại:
```text
OnTriggerEnter2D
↓
kiểm tra Layer có nằm trong collisionLayers
↓
nếu không hợp lệ → bỏ qua
↓
TryGetComponent<EnemyHealth>()
↓
nếu có EnemyHealth → TakeDamage(damageAmount)
↓
Projectile.ReturnToPool()
```

Giá trị test:
```text
Damage Amount = 1
Collision Layers:
- Environment
- Enemy
```

Đã test:
```text
Environment Layer → không damage → Projectile ReturnToPool
Enemy Layer + EnemyHealth → damage đúng 1 lần → Projectile ReturnToPool
Layer ngoài CollisionLayers → bỏ qua
```

---

# 14A. Tuần 2 — Thứ 5: Combat Foundation
**DONE — người dùng đã chạy thử và xác nhận.**

Đã hoàn thành:
- Enemy Dummy.
- Layer `Enemy`.
- `EnemyData.cs` ScriptableObject.
- `DummyEnemyData` với `MaxHealth = 3` để test.
- `EnemyHealth.cs`.
- `TakeDamage(int damageAmount)`.
- `OnDamaged`.
- `OnDeath`.
- `isDead` để đảm bảo death chỉ phát một lần.
- Projectile gây damage thật qua `ProjectileCollision`.
- `EnemyHitFeedback.cs`.
- Hit flash bằng DOTween trên `SpriteRenderer` của child Visual.
- Dùng `DOKill()` trước tween mới và reset màu gốc.

### Enemy hierarchy tối thiểu hiện tại

```text
EnemyDummy                     Layer: Enemy
├── Rigidbody2D
├── Collider2D
├── EnemyHealth
├── EnemyHitFeedback
├── EnemyAI
└── Visual
    └── SpriteRenderer
```

`DamageSource` từng được gắn trên Enemy Dummy để test Player Health ở Tuần 2, nhưng đã được gỡ khỏi Enemy Dummy khi `EnemyAI` dùng `EnemyData.AttackDamage` làm nguồn contact damage thật.

### EnemyData hiện tại

```text
DummyEnemyData
├── MaxHealth = 3
├── MoveSpeed = 2
├── AttackDamage = 1
└── AttackCooldown = 1
```

Quy tắc:
```text
EnemyData.MaxHealth
→ configuration/static data

EnemyHealth.currentHealth
→ runtime state
```

Không mutate asset `EnemyData` khi Enemy bị trúng đòn.

### EnemyHealth hiện tại

State/contract quan trọng:
- `currentHealth`
- `isDead`
- `OnDamaged`
- `OnDeath`
- `CurrentHealth`

Luồng:
```text
TakeDamage()
↓
bỏ qua nếu đã chết hoặc damage <= 0
↓
trừ currentHealth
↓
clamp tối thiểu 0
↓
OnDamaged
↓
nếu HP <= 0 → Die()
↓
OnDeath chỉ một lần
```

### EnemyHitFeedback hiện tại

Trách nhiệm:
> Chỉ xử lý visual feedback khi Enemy nhận damage, không sửa HP/gameplay.

Dùng:
```text
SpriteRenderer.DOColor(hitColor, hitFlashDuration)
.SetLoops(2, LoopType.Yoyo)
```

Giá trị test:
```text
Hit Color = Red
Hit Flash Duration = 0.08
```

Luồng visual:
```text
OnDamaged
↓
DOKill tween cũ
↓
reset originalColor
↓
Normal → Hit Color → Normal
```

DOTween chỉ tác động `SpriteRenderer.color`, không tác động Collider/HP/physics.

---

# 14B. Tuần 2 — Thứ 6: Player Health
**DONE — người dùng đã chạy thử và xác nhận.**

Đã hoàn thành:
- Bổ sung `MaxHealth` và `InvincibilityDuration` vào `PlayerData`.
- `PlayerHealth.cs`.
- `DamageSource.cs` cho contact damage cơ bản.
- Khoảng bất tử ngắn sau khi Player nhận hit.
- `OnHealthChanged`.
- `OnDeath`.
- Test event bằng listener tạm, xác nhận event không phụ thuộc HUD.
- Test death chỉ phát một lần.

### PlayerHealth hiện tại

Giá trị test:
```text
MaxHealth = 6
InvincibilityDuration = 0.5
```

State/contract quan trọng:
- `currentHealth`
- `isInvincible`
- `isDead`
- `CurrentHealth`
- `MaxHealth`
- `OnHealthChanged(int currentHealth, int maxHealth)`
- `OnDeath`

Luồng damage:
```text
TakeDamage()
↓
nếu isDead → bỏ qua
↓
nếu isInvincible → bỏ qua
↓
nếu damage <= 0 → bỏ qua
↓
trừ HP
↓
OnHealthChanged
↓
HP <= 0 ?
├── Có → Die() → OnDeath
└── Không → bắt đầu InvincibilityCoroutine
             ↓
             isInvincible = true
             ↓
             chờ PlayerData.InvincibilityDuration
             ↓
             isInvincible = false
```

`OnHealthChanged` phát HP hiện tại + HP tối đa để HUD dùng ở task kế tiếp, nhưng `PlayerHealth` không reference HUD.

### DamageSource hiện tại

Trách nhiệm:
> Chỉ phát yêu cầu damage khi Player đang tiếp xúc; luật có nhận damage hay không do `PlayerHealth` quyết định.

Dùng:
```text
OnTriggerStay2D
↓
TryGetComponent<PlayerHealth>()
↓
playerHealth.TakeDamage(damageAmount)
```

Giá trị test:
```text
Damage Amount = 1
```

Việc dùng `OnTriggerStay2D()` giúp test trực tiếp invincibility window:
```text
chạm liên tục
↓
hit đầu mất HP
↓
các lần gọi trong 0.5 giây bị PlayerHealth chặn
↓
hết 0.5 giây mới nhận hit kế tiếp
```

---

# 14C. Tuần 3 — Thứ 2: HUD UI
**DONE — người dùng đã chạy thử và xác nhận.**

## Kiến trúc HUD hiện tại

```text
PlayerHealth
    │
    │ OnHealthChanged(currentHealth, maxHealth)
    ▼
HUDPresenter
    │
    │ UpdateHealth(currentHealth, maxHealth)
    ▼
HUDView
    │
    └── HeartContainer
          └── HeartUI(Clone) × số Heart cần thiết
```

Currency hiện tại đã dùng nguồn gameplay thật từ Vertical Slice:

```text
PlayerResources
    │
    │ OnCoinsChanged(currentCoins)
    ▼
HUDPresenter
    │
    │ UpdateCurrency(currentCoins)
    ▼
HUDView
    └── CoinText
```

### MVP tối giản đã chốt

- **Model / nguồn gameplay:** `PlayerHealth` và sau này là nguồn Run Currency thật.
- **Presenter:** `HUDPresenter` đăng ký/hủy đăng ký event và chuyển dữ liệu sang View.
- **View:** `HUDView` chỉ hiển thị Heart/Coin và chạy visual tween.
- Không để `HUDView` tự poll `PlayerHealth` trong `Update()`.
- Không để `PlayerHealth` reference UI.

### Heart HUD kiểu Isaac

Quy ước:
```text
1 HP = nửa trái tim
2 HP = 1 trái tim đầy
```

Ví dụ:
```text
6 HP → Full Full Full
5 HP → Full Full Half
4 HP → Full Full Empty
3 HP → Full Half Empty
2 HP → Full Empty Empty
1 HP → Half Empty Empty
0 HP → Empty Empty Empty
```

Số Heart được tính từ `maxHealth` runtime:
```text
requiredHeartCount = ceil(maxHealth / 2)
```

`HUDView` dùng:
- `heartContainer`
- `heartPrefab`
- `List<Image> heartImages`
- `List<Vector3> originalHeartScales`

Nếu số Heart hiện tại chưa đủ, `EnsureHeartCount()` gọi `CreateHeart()` và `Instantiate(heartPrefab, heartContainer)`.

Điểm quan trọng:
> `HUDView` không đọc `PlayerData.MaxHealth` trực tiếp. `PlayerHealth.MaxHealth` là nguồn runtime và được `HUDPresenter` truyền xuống View.

Nhờ vậy sau này nếu MaxHealth runtime tăng, HUD có thể sinh thêm Heart mà không phải gắn trước `Heart_04`, `Heart_05` trong Inspector.

### DOTween HUD

Heart thay đổi trạng thái:
```text
DOKill()
↓
reset original scale
↓
DOPunchScale()
```

CoinText thay đổi:
```text
DOKill()
↓
reset original scale
↓
DOPunchScale()
```

Tween chỉ tác động RectTransform scale của UI, không ảnh hưởng gameplay/physics.

### Coin / PlayerResources

Nguồn Coin gameplay hiện tại là `PlayerResources.cs`:
- giữ `currentCoins`, `currentKeys`, `currentBombs`;
- phát `OnCoinsChanged(int)`, `OnKeysChanged(int)`, `OnBombsChanged(int)`;
- HUD hiện chỉ lắng nghe `OnCoinsChanged`;
- `RoomReward` cộng Coin khi `RoomCleared`;
- không phải `CurrencyManager` hoặc `RunProgress`.

`TestCurrencySource` chỉ là công cụ test lịch sử của task HUD và không còn là nguồn gameplay.

Quyết định đã chốt:
> Chưa tạo RunProgress/ProgressionManager sớm. `PlayerResources` chỉ giữ runtime resource tối thiểu cho Player; ownership xuyên scene/run sẽ được đánh giá đúng roadmap ở Tuần 7.

# 15. Object Pooling — Tuần 5 Thứ 4 — DONE

Project dùng `UnityEngine.Pool.ObjectPool<T>`, không dùng pooling plugin ngoài và hiện **không có PoolManager/PoolRegistry chung** vì các pool riêng đã đủ đơn giản.

Pool hiện có:

```text
ProjectilePool
EnemyPool
DropPool
DamageTextPool
```

Nguyên tắc chung:

```text
Get từ Pool
↓
Reset state của lần sử dụng trước
↓
Gán position/reference runtime mới
↓
Gameplay/visual sử dụng object
↓
Release về Pool
↓
OnDisable/Kill tween/cleanup
↓
object inactive chờ lần Get tiếp theo
```

Phải reset khi phù hợp:
- health/death state;
- AI detection/contact/attack/cooldown;
- coroutine;
- Rigidbody2D velocity/angular velocity;
- DOTween cũ;
- màu/scale/rotation visual;
- event subscription thuộc owner cũ;
- Scene reference cần cấp lại khi reuse.

## Enemy Pool

```text
RoomController.PrepareRuntimeEnemies()
        ↓
EnemyPool.GetEnemy(spawnPosition, enemyContainer)
        ↓
EnemyHealth.ResetForReuse()
EnemyAI.ResetForReuse()
        ↓
parent vào Enemies của Room
đặt tại EnemySpawnPoint
        ↓
ConfigureSpawnedEnemy()
├── EnemyAI.SetPlayerTransform(playerTransform)
└── EnemyDeathCameraShake.SetCameraController(cameraController)
        ↓
RoomController.ActivateEnemies()
```

Enemy chết:

```text
EnemyHealth.OnDeath(EnemyHealth deadEnemy)
        ↓
RoomController.HandleEnemyDeath(deadEnemy)
        ↓
unsubscribe RoomController khỏi Enemy đó
aliveEnemyCount--
CheckRoomCleared()
        ↓
đợi sang frame kế tiếp
        ↓
EnemyPool.ReleaseEnemy(deadEnemy)
        ↓
SetActive(false)
OnDisable cleanup
parent trở lại EnemyPool
```

`EnemyHealth.OnDeath` truyền `EnemyHealth` vừa chết để RoomController biết chính xác object nào phải release.

`EnemyHealth.ResetForReuse()`:
- hồi `currentHealth = EnemyData.MaxHealth`;
- `isDead = false`.

`EnemyAI.ResetForReuse()`:
- `isDead = false`;
- reset detection/contact/attack;
- reset cooldown;
- dừng attack coroutine;
- `DOKill()` telegraph + trả scale gốc;
- reset Rigidbody2D velocity/angular velocity.

Combat Room cũ có Enemy đặt sẵn vẫn được giữ tương thích; chỉ Enemy runtime lấy từ `EnemyPool` mới được trả về pool.

## Drop Pool

`DropPool` + `PooledDrop` mới chịu trách nhiệm pooling contract, **chưa phải loot gameplay thật**.

```text
DropPool.GetDrop(position)
→ ResetForReuse
→ đặt position
→ Active
→ sử dụng
→ ReturnToPool / ReleaseDrop
→ inactive + parent về DropPool
```

`PooledDrop` reset:
- Kill tween;
- scale gốc;
- rotation gốc/identity.

Coin/Heart/Item, LootTable và pickup behavior vẫn thuộc task Loot tiếp theo.

## DamageText Pool

DamageText hiện là **TextMeshPro World Space**, không phải `TextMeshProUGUI` trong Screen Space Canvas.

```text
DamageTextPool.GetDamageText(worldPosition)
→ ResetForReuse
→ đặt World Position
→ Active
→ ShowDamage(value)
→ DOMove lên trên + DOFade về 0 cùng lúc trong Sequence
→ OnComplete ReturnToPool
```

Reset gồm:
- Kill Sequence/tween cũ;
- trả scale gốc;
- trả màu/alpha gốc.

Vì DamageText ở World Space nên khi nối gameplay thật sau này có thể dùng trực tiếp vị trí Enemy + offset; không cần đổi World → Screen coordinate.

## Regression/Profiler đã xác nhận

```text
✓ Enemy được reuse, không Create lại khi Pool còn object
✓ HP hồi MaxHealth khi reuse
✓ EnemyAI Chase/Attack lại bình thường
✓ hit feedback và camera shake vẫn hoạt động
✓ Room enemy count / Clear / Door không regression
✓ Drop reuse cùng instance và reset state đúng
✓ DamageText reuse cùng instance, fade/bay lên và tự Release đúng
✓ pooled object không giữ tween/event cũ
✓ Profiler không còn spike Instantiate/Destroy lớn do Enemy combat sau khi pool đã có object
✓ Console sạch trong test cuối
```

---

# 16. Scene / Core hiện tại

## Bootstrap Scene
Chứa:
```text
Bootstrap
GameManager
SceneLoader
LoadingScreen liên quan SceneLoader
```

Bootstrap là Scene số 0 trong Build Settings / Build Profiles.

`Bootstrap.cs` gọi `SceneLoader` để load scene tiếp theo.

## GameManager
- Singleton.
- `DontDestroyOnLoad`.
- Không nhét ProjectilePool, EnemyPool, Audio, UI... vào GameManager.
- Giữ trách nhiệm cấp cao.

## SceneLoader
Hiện có:
- Singleton Instance.
- `DontDestroyOnLoad`.
- chống duplicate Instance.
- `isLoading`.
- `LoadingScreen`.
- load scene async.
- `allowSceneActivation`.
- progress.
- Show/Hide LoadingScreen.

---

# 18. Folder structure hiện tại trong Unity

> **Đây là cấu trúc folder chuẩn hiện tại của project sau Tuần 5.**
> Khi tạo script/module mới, ưu tiên đặt đúng theo cấu trúc này.
> Không tự di chuyển hoặc tạo một cấu trúc folder khác nếu chưa trao đổi với người dùng.

```text
Scripts
├── Core
│   ├── Bootstrap.cs
│   ├── GameManager.cs
│   └── SceneLoader.cs
│
├── Data
│   ├── Config
│   ├── Enum
│   │   ├── RoomType.cs
│   │   └── LootRewardType.cs
│   └── ScriptableObjects
│       ├── Dungeon
│       │   └── FloorData.cs
│       ├── Item
│       │   ├── ItemData.cs
│       │   ├── ItemPoolEntry.cs
│       │   └── ItemPoolData.cs
│       ├── Loot
│       │   ├── LootEntry.cs
│       │   └── LootTable.cs
│       └── Player
│           └── PlayerData.cs
│
├── Events
│
├── Gameplay
│   ├── Boss
│   ├── Camera
│   │   ├── CameraController.cs
│   │   └── EnemyDeathCameraShake.cs
│   ├── Dungeon
│   │   ├── EdgarDungeonGenerator.cs
│   │   ├── EdgarDungeonPostProcessing.cs
│   │   ├── FloorEdgarConfig.cs
│   │   ├── FloorExit.cs
│   │   └── FloorExitSpawner.cs
│   ├── Enemy
│   │   ├── DamageSource.cs
│   │   ├── EnemyAI.cs
│   │   ├── EnemyData.cs
│   │   ├── EnemyHealth.cs
│   │   ├── EnemyHitFeedback.cs
│   │   └── EnemyPool.cs
│   ├── Item
│   │   ├── ItemPedestal.cs
│   │   ├── ItemRewardRoller.cs
│   │   └── RoomItemRewardSpawner.cs
│   ├── NPC
│   ├── Pickup
│   │   ├── DropPool.cs
│   │   ├── PooledDrop.cs
│   │   ├── PooledDropPickup.cs
│   │   └── RoomLootDropSpawner.cs
│   ├── Player
│   │   ├── PlayerController.cs
│   │   ├── PlayerController.inputactions
│   │   ├── PlayerDash.cs
│   │   ├── PlayerData.cs
│   │   ├── PlayerHealth.cs
│   │   ├── PlayerMovement.cs
│   │   ├── PlayerResources.cs
│   │   └── PlayerShooter.cs
│   ├── Projectile
│   │   ├── Projectile.cs
│   │   ├── ProjectileCollision.cs
│   │   └── ProjectilePool.cs
│   └── Room
│       ├── EnemySpawnPoint.cs
│       ├── PlayerSpawnPoint.cs
│       ├── RoomContext.cs
│       ├── RoomController.cs
│       ├── RoomDoor.cs
│       ├── RoomLootRoller.cs
│       ├── RoomMarkerTilemap.cs
│       ├── RoomReward.cs
│       ├── RoomTransitionController.cs
│       └── RoomTrigger.cs
│
├── Managers
├── Systems
├── Test
├── UI
│   ├── DamageText
│   │   ├── DamageText.cs
│   │   └── DamageTextPool.cs
│   ├── GameOver
│   ├── HUD
│   │   ├── HUDPresenter.cs
│   │   └── HUDView.cs
│   ├── Inventory
│   ├── LoadingScreen.cs
│   ├── MainMenu
│   ├── Minimap
│   │   ├── MinimapGenerator.cs
│   │   └── MinimapRoomIcon.cs
│   ├── Pause
│   └── Settings
└── Utilities
```

### Thay đổi folder/current scripts quan trọng sau Tuần 5

- `RewardSpawnPoint.cs` **đã bỏ khỏi current architecture** sau khi Item/Pickup chuyển sang Marker Tile.
- `FloorExitSpawnPoint.cs` **đã bỏ khỏi current architecture** sau khi FloorExit chuyển sang Marker Tile.
- `EnemySpawnPoint.cs` và `PlayerSpawnPoint.cs` vẫn còn dùng GameObject marker.
- `GameplayMarkers` là **Tilemap trong Room Template**, không phải folder script. `RoomMarkerTilemap.cs` đọc marker tile từ Tilemap này.

### Ý nghĩa phân nhóm hiện tại

- `Core`: Bootstrap, state cấp cao và scene loading.
- `Data`: enum/config/ScriptableObject, không chứa runtime state thay đổi của một run.
- `Gameplay/Dungeon`: adapter Edgar + FloorExit/floor-level flow.
- `Gameplay/Room`: Room lifecycle/context/transition/marker data.
- `Gameplay/Pickup`: physical small pickup + pooling/collect.
- `Gameplay/Item`: Item lớn/pedestal/roll orchestration; Item Effects chưa làm.
- `Managers`: chỉ thêm manager thật sự cần; hiện không có `BossRewardManager`.
- `UI`: hiển thị/feedback, không làm nguồn gameplay state.

---

# 19. Plugin / Package Roadmap đã chốt

## Đang dùng

### DOTween
Dùng cho:
- Fade.
- Punch.
- Scale.
- UI animation.
- Visual feedback.

Không dùng làm physics movement chính khi collision phụ thuộc Rigidbody2D.

### Unity Input System
Đã dùng cho Move, Shoot, Dash.

### Cinemachine 3.1.7
Đang dùng từ Tuần 3 - Thứ 5 cho:
- Camera follow Player nền tảng của Vertical Slice.
- Framing bằng Position Composer.
- Camera shake / Impulse.
- Camera theo Room kiểu Isaac sẽ triển khai đúng roadmap ở Tuần 5:
  - Normal Room ≈ 1 màn hình, không follow Player tự do.
  - Qua Door mới đổi current room/camera.
  - Large Room có thể follow nhưng phải giới hạn trong room bounds.

Kiến trúc:
```text
Gameplay / Room
      ↓
CameraController nhỏ
      ↓
Cinemachine
```

Không rải trực tiếp dependency Cinemachine vào Enemy/Combat/UI.

## Đang dùng — từ Tuần 4

### Edgar Free 2.1.0 — kiến trúc hoàn chỉnh sau Tuần 4

Môi trường đã Play Test:

```text
Unity 6000.5.2f1
Edgar Free 2.1.0
```

Lưu ý tương thích:
- Nhánh Git `#upm` cũ từng gây `CS0619` với `AssetDatabase.GetAssetPath(int)` trên Unity 6000.5.
- Không sửa trực tiếp `Library/PackageCache`.
- Project hiện dùng Edgar Free 2.1.0 import bằng `.unitypackage`; compile và generate sạch.

#### 19A. Edgar chịu trách nhiệm gì?

Edgar **chỉ** giải bài toán bố cục dungeon:

```text
Level Graph
    ↓
Chọn Room Template
    ↓
Tìm vị trí Room không overlap
    ↓
Tính Door / connection
    ↓
Tạo Corridor nếu cấu hình yêu cầu
```

Code project chịu trách nhiệm gameplay:

```text
RoomContext
RoomController
RoomDoor gameplay
Enemy / Health / Damage
Room Clear
Reward
Camera
Minimap state
Run progression
```

Ranh giới bắt buộc:

```text
Edgar API
   ↓
EdgarDungeonGenerator
+ EdgarDungeonPostProcessing
   ↓
RoomContext / RoomController
   ↓
Gameplay / UI
```

**Player, Combat và UI không gọi Edgar trực tiếp.**

Điều này giống một công ty thuê đơn vị bên ngoài vẽ sơ đồ căn nhà: Edgar quyết định phòng nào nằm ở đâu và nối với phòng nào; còn luật “vào phòng thì đóng cửa, đánh quái, nhận thưởng” vẫn là luật của game chúng ta.

---

#### 19B. Prototype Edgar — Tuần 4 Thứ 2

Scene prototype đã dùng để học plugin độc lập trước khi chạm gameplay thật:

```text
EdgarPrototype
├── Room Template cơ bản
├── Simple Door Mode cho room thường
├── CorridorHorizontal
├── CorridorVertical
├── PrototypeLevelGraph
└── Dungeon Generator (Grid2D)
```

Thiết lập corridor:
- `CorridorHorizontal` và `CorridorVertical` dùng Manual Door Mode.
- Mỗi corridor có đúng 2 door ở hai phía đối diện.
- `Use Corridors = ON`.

Kết quả đã test:
- graph generate đúng;
- room không overlap;
- corridor nối hợp lệ;
- Console không lỗi đỏ.

---

#### 19C. Room Template gameplay — Tuần 4 Thứ 3

Hierarchy chuẩn của tất cả gameplay Room Template:

```text
Room_*
├── Tilemaps
├── RoomTrigger
├── Doors
├── Enemies
├── EnemySpawnPoints
└── RewardSpawnPoint
```

Bộ template tối thiểu:

```text
Room_Start_01
Room_Combat_01
Room_Reward_01
Room_Boss_01
```

Quy tắc:
- Root giữ `RoomController`.
- Root giữ `RoomContext` từ giai đoạn Runtime Integration trở đi.
- `RoomTrigger` reference về `RoomController` của chính Room.
- `EnemySpawnPoint` / `RewardSpawnPoint` chỉ là marker Transform/component.
- Child `Doors` để rỗng trong prefab; Runtime Integration mới instantiate `RoomDoor.prefab` tại đúng Door Edgar thực sự sử dụng.
- Edgar generate đã được test giữ nguyên GameObject/component/reference nội bộ của Room Template.

Cấu hình marker hiện tại:

```text
Start
├── EnemySpawnPoint = 0
└── RewardSpawnPoint = inactive

Combat
├── EnemySpawnPoint = 3
└── RewardSpawnPoint = active

Reward
├── EnemySpawnPoint = 0
└── RewardSpawnPoint = active

Boss
├── EnemySpawnPoint = 1
└── RewardSpawnPoint = active
```

---

#### 19D. Floor Data + Level Graph — Tuần 4 Thứ 4

##### `RoomType`

Path:

```text
Assets/Scripts/Data/Enum/RoomType.cs
```

Giá trị hiện có:

```text
Start
Combat
Reward
Shop
Boss
```

`Shop` mới là loại dữ liệu chuẩn bị sẵn; **chưa có `Room_Shop_01` gameplay thật**.

##### `FloorData`

Path:

```text
Assets/Scripts/Data/ScriptableObjects/Dungeon/FloorData.cs
```

Trách nhiệm:

```text
FloorData
├── RoomCount
├── DifficultyLevel
├── RoomTypes[]
├── UseRandomSeed
└── FixedSeed
```

`FloorData` chỉ là **data/config**, không gọi Edgar API.

Asset Floor 01:

```text
Assets/Data/Dungeon/Floors/Floor_01_Data.asset

RoomCount       = 6
DifficultyLevel = 1
RoomTypes       = Start / Combat / Reward / Boss
UseRandomSeed   = true (có thể đổi khi test fixed seed)
FixedSeed       = 12345
```

##### Giới hạn Edgar Free cần nhớ

Edgar Free dùng **Fixed Level Graph**. Vì vậy `FloorData.RoomCount` hiện không tự sinh graph bằng code runtime.

Flow hiện tại là:

```text
FloorData.RoomCount
→ metadata / validation

LevelGraph asset
→ graph thật mà Edgar Free generate
```

Không tự viết graph generator riêng chỉ để thay tính năng PRO.

##### Level Graph Floor 01

Asset:

```text
Assets/Data/Dungeon/Edgar/Floor_01_LevelGraph.asset
```

Graph hiện tại:

```text
              Reward
                 |
Start — Combat_01 — Combat_02 — Combat_03 — Boss
```

Tổng:

```text
6 gameplay Room
5 graph connections
```

Mỗi graph node dùng Individual Room Template tương ứng.

##### `FloorEdgarConfig`

Path:

```text
Assets/Scripts/Gameplay/Dungeon/FloorEdgarConfig.cs
```

Asset:

```text
Assets/Data/Dungeon/Edgar/Floor_01_EdgarConfig.asset
```

Trách nhiệm:

```text
FloorEdgarConfig
├── FloorData
└── LevelGraph
```

Đây là **điểm ánh xạ tập trung** giữa dữ liệu project và asset Edgar.

Flow:

```text
FloorData
    +
LevelGraph Edgar
    ↓
FloorEdgarConfig
```

`OnValidate()` hiện kiểm tra tối thiểu:
- `FloorData.RoomCount` có khớp số node graph không;
- có `RoomType.Start` không;
- có `RoomType.Boss` không.

Mục đích của validation là phát hiện cấu hình sai ngay trong Editor, trước khi Play.

---

#### 19E. Runtime Integration — Tuần 4 Thứ 5

Đây là lớp cầu nối quan trọng nhất giữa Edgar và gameplay.

##### Tổng flow runtime

```text
Floor_01_Data
      ↓
Floor_01_EdgarConfig
      ↓
EdgarDungeonGenerator
      ↓
DungeonGeneratorGrid2D.Generate()
      ↓
Edgar sinh RoomInstance / Corridor
      ↓
EdgarDungeonPostProcessing.Run(level)
      ↓
RoomContext + RoomController + RoomDoor + SpawnPoint
      ↓
Gameplay/UI dùng dữ liệu project
```

##### `EdgarDungeonGenerator`

Path:

```text
Assets/Scripts/Gameplay/Dungeon/EdgarDungeonGenerator.cs
```

Trách nhiệm duy nhất:
1. kiểm tra reference;
2. lấy `FloorEdgarConfig`;
3. gán `LevelGraph` cho Edgar generator;
4. gán random/fixed seed;
5. gọi `Generate()`.

Flow:

```text
Start()
  ↓
GenerateDungeon()
  ↓
ValidateReferences()
  ↓
ApplyFloorConfiguration()
  ↓
DungeonGeneratorGrid2D.Generate()
```

Edgar generator được đặt:

```text
Generate On = Manually
```

để tránh Edgar tự generate một lần rồi adapter lại generate thêm lần nữa.

Seed flow:

```text
FloorData.UseRandomSeed
FloorData.FixedSeed
      ↓
EdgarDungeonGenerator
      ↓
DungeonGeneratorGrid2D.UseRandomSeed
DungeonGeneratorGrid2D.RandomGeneratorSeed
```

Đã test:
- random seed;
- fixed seed;
- regenerate nhiều lần trong cùng Play Mode.

##### `EdgarDungeonPostProcessing`

Path:

```text
Assets/Scripts/Gameplay/Dungeon/EdgarDungeonPostProcessing.cs
```

Kế thừa:

```text
DungeonGeneratorPostProcessingComponentGrid2D
```

Edgar gọi:

```text
Run(DungeonGeneratorLevelGrid2D level)
```

sau khi generation hoàn tất.

Post Processing được chia thành **2 pass** để dễ hiểu và tránh reference chưa sẵn sàng.

Pass 1 — setup từng gameplay Room:

```text
foreach RoomInstance
    ↓
IsCorridor?
├── Có  → bỏ qua gameplay setup
└── Không
       ↓
   tìm RoomTemplateInstance
       ↓
   RoomController
       ↓
   RoomContext
       ↓
   SetRoomId()
       ↓
   SetLayoutPosition()
       ↓
   CollectRoomSpawnPoints()
       ↓
   CreateGameplayDoors()
```

Pass 2 — setup logical connection:

```text
Gameplay Room A
    ↓ DoorInstance
ConnectedRoomInstance
    ↓
Có thể là Corridor
    ↓
đi qua Corridor
    ↓
Gameplay Room B
    ↓
RoomContext A.ConnectedRooms += RoomContext B
```

Sau khi cả hai pass xong:

```text
generatedRoomContexts
        ↓
MinimapGenerator.BuildMinimap()
```

---

#### 19F. `RoomContext` — dữ liệu runtime trung tâm của một Room

Path:

```text
Assets/Scripts/Gameplay/Room/RoomContext.cs
```

`RoomContext` trả lời câu hỏi:

> “Room runtime này là room nào, nằm ở đâu, đã ghé chưa, nối với room nào và có marker gameplay nào?”

Dữ liệu hiện tại:

```text
RoomContext
├── RoomId
├── RoomType
├── LayoutPosition
├── IsVisited
├── ConnectedRooms[]
├── EnemySpawnPoints[]
└── RewardSpawnPoint
```

##### `RoomId`

Post Processing gán ID runtime tuần tự cho gameplay Room:

```text
0, 1, 2, 3, 4, 5
```

Không được giả định:

```text
RoomId 0 == Start
```

Muốn tìm Start phải dùng:

```text
RoomType.Start
```

##### `RoomType`

Mỗi Room Template tự cấu hình type trong prefab:

```text
Room_Start_01  → Start
Room_Combat_01 → Combat
Room_Reward_01 → Reward
Room_Boss_01   → Boss
```

##### `LayoutPosition`

Nguồn:

```text
roomInstance.Position (Vector3Int của Edgar)
        ↓
lấy X/Y
        ↓
Vector2Int
        ↓
RoomContext.LayoutPosition
```

Đây là **tọa độ layout dungeon**, chưa phải tọa độ UI Minimap.

##### `IsVisited`

Runtime state:

```text
false → Player chưa ghé
true  → Player đã từng ghé
```

Khi một Room trở thành Current Room:

```text
SetCurrentRoom(room)
    ↓
room.MarkAsVisited()
```

##### `ConnectedRooms[]`

Chỉ chứa gameplay Room, không chứa Corridor Edgar.

Ví dụ:

```text
Combat_01.ConnectedRooms
├── Start
├── Combat_02
└── Reward
```

Dữ liệu hai chiều:

```text
A → B
B → A
```

##### Spawn Points

```text
EnemySpawnPoints[]
RewardSpawnPoint
```

Post Processing chỉ **thu thập reference**. Tuần 4 chưa instantiate enemy/reward tại marker; lifecycle/spawn thật đi đúng roadmap sau.

---

#### 19G. RoomDoor runtime từ Door Edgar

Room Template không đặt RoomDoor gameplay cứng sẵn ở mọi door candidate.

Lý do:
- Simple Door Position chỉ là **vị trí có thể** dùng;
- sau generation mới biết Door nào Edgar thực sự chọn.

Flow:

```text
roomInstance.Doors
      ↓
DoorInstanceGrid2D.DoorLine
      ↓
local grid cell
      ↓
Grid.GetCellCenterWorld()
      ↓
worldDoorPosition
      ↓
Instantiate(RoomDoor.prefab)
      ↓
parent = Room/Doors
      ↓
RoomController.SetRoomDoors(RoomDoor[])
```

Chỉ tạo RoomDoor cho gameplay Room, không tạo thêm ở Corridor.

Với Floor 01 hiện tại:

```text
5 graph connections
→ 10 gameplay-side door endpoints
→ 10 RoomDoor runtime
```

RoomDoor mặc định Unlock:

```text
Unlock() → collider disabled
Lock()   → collider enabled
```

Tuần 5 - Thứ 2 mới dùng danh sách Door runtime này vào lifecycle combat thực tế của các Room Edgar.

---

#### 19H. Logical connection: đi xuyên Corridor để tìm gameplay Room

Edgar có thể trả:

```text
Room A
  ↓
Corridor
  ↓
Room B
```

Khi đọc Door của Room A:

```text
doorInstance.ConnectedRoomInstance
```

đầu bên kia có thể là Corridor, không phải Room B.

Helper hiện tại làm:

```text
source Room A
    ↓
connected instance
    ↓
IsCorridor?
├── Không → trả gameplay Room ngay
└── Có
     ↓
   duyệt 2 Door của Corridor
     ↓
   bỏ Door quay về source Room A
     ↓
   Door còn lại
     ↓
   Room B
```

Sau đó chỉ lưu:

```text
RoomContext A ↔ RoomContext B
```

không đưa Corridor vào gameplay data.

---

#### 19I. Minimap từ Edgar Data — Tuần 4 Thứ 6

Minimap không dùng tính năng Minimap PRO của Edgar.

Flow kiến trúc:

```text
Edgar RoomInstance.Position / connection
        ↓
EdgarDungeonPostProcessing
        ↓
RoomContext
├── LayoutPosition
├── ConnectedRooms[]
├── IsVisited
└── RoomType
        ↓
MinimapGenerator
        ↓
UI Room Icon + Connection Line
```

Path script:

```text
Assets/Scripts/UI/Minimap/MinimapGenerator.cs
Assets/Scripts/UI/Minimap/MinimapRoomIcon.cs
```

Hierarchy UI:

```text
HUDCanvas
└── MinimapRoot
    ├── ConnectionContainer
    └── RoomIconContainer
```

Prefab:

```text
Assets/Prefabs/UI/Minimap/MinimapRoomIcon.prefab
Assets/Prefabs/UI/Minimap/MinimapConnection.prefab
```

##### Chuyển LayoutPosition sang UI Position

Start Room được lấy làm origin:

```text
relativePosition
= room.LayoutPosition - start.LayoutPosition
```

Sau đó:

```text
minimapPosition
= relativePosition × minimapScale
```

Cuối cùng:

```text
RectTransform.anchoredPosition = minimapPosition
```

Điều này tách rõ:

```text
Dungeon coordinate
≠
UI pixel coordinate
```

##### Minimap state

`MinimapGenerator` giữ một reference:

```text
currentRoom
```

Mỗi `MinimapRoomIcon` giữ reference tới đúng `RoomContext` mà nó đại diện.

Ưu tiên hiển thị:

```text
Current?
├── Có  → Current Color
└── Không
      ↓
   IsVisited?
   ├── Có  → Visited Color
   └── Không → Unvisited Color
```

`Current` không được lưu thành bool trên từng RoomContext để tránh nhiều Room cùng `isCurrent = true`.

##### Vẽ connection/corridor logic trên Minimap

Minimap hiển thị **logical connection**, không vẽ chính xác từng tile của Corridor ngoài world.

Ví dụ world:

```text
Room A → Corridor hình học → Room B
```

Minimap:

```text
[Room A] ───────── [Room B]
```

Để tránh tạo line hai lần vì `ConnectedRooms` hai chiều:

```text
chỉ tạo line khi
roomA.RoomId < roomB.RoomId
```

Connection UI tính từ hai minimap position:

```text
Direction = B - A
Length    = Direction.magnitude
MidPoint  = (A + B) / 2
Angle     = Atan2(Direction.y, Direction.x)
```

rồi:

```text
anchoredPosition = MidPoint
width            = Length
rotation Z       = Angle
```

Floor 01 có:

```text
6 Room icon
5 Connection line
```

##### Regenerate cleanup

Mỗi lần Build Minimap:

```text
ClearMinimap()
├── Destroy RoomIcon children cũ
├── Destroy Connection children cũ
├── roomIcons.Clear()
└── currentRoom = null
```

Sau đó mới build layout mới.

Đã test regenerate nhiều seed trong cùng Play Mode không được giữ icon/connection/reference cũ.

---

#### 19J. Bug Minimap quan trọng khi thêm Connection

Triệu chứng:

```text
Connection line đúng layout
Room icon lại chồng hết tại tâm
```

Dữ liệu `LayoutPosition` và `CalculateMinimapPosition()` thực tế vẫn đúng vì connection dùng cùng phép tính và đã hiển thị đúng.

Nguyên nhân nằm trong `CreateRoomIcon()`:

```csharp
Vector2 minimapPosition = CalculateMinimapPosition(...);
```

đã tính vị trí nhưng thiếu bước **áp vị trí lên RectTransform**:

```csharp
createdRoomIconRectTransform.anchoredPosition = minimapPosition;
```

Sau khi thêm lại dòng trên, icon trở về đúng layout.

Bài học debug:

```text
Data đúng
+ công thức đúng
+ một View đúng
+ một View sai
→ kiểm tra bước gán dữ liệu vào View sai trước
```

Đây là regression do refactor khi tách `CalculateMinimapPosition()` cho cả icon và connection.

---

#### 19K. Trạng thái cuối Tuần 4

Đã Play Test và xác nhận:

```text
Edgar Setup & Prototype       DONE
Room Templates                DONE
Graph & Floor Data            DONE
Runtime Integration           DONE
Minimap từ Edgar Data         DONE
```

Progress sau Tuần 4:

```text
20 / 50 task DONE
```

Task kế tiếp:

```text
Tuần 5 - Thứ 2
Room Lifecycle + Edgar
```

Mục tiêu kế tiếp không phải generate thêm data, mà dùng chính các dữ liệu đã chuẩn bị:

```text
RoomContext
RoomController
RoomDoor[]
EnemySpawnPoints[]
        ↓
Enter → Lock → Fight → Clear → Unlock
```


#### 19L. Room Lifecycle + Edgar — Tuần 5 Thứ 2 — DONE

Mục tiêu của bước này là **tái sử dụng lifecycle CombatRoom đã làm ở Tuần 3 cho mọi Combat/Boss Room được Edgar sinh runtime**, thay vì tạo một combat system mới.

Ranh giới trách nhiệm:

```text
Edgar
→ sinh layout / Room Template / Door / Corridor
→ không quyết định damage, enemy death, reward hoặc RoomCleared

RoomController của project
→ quyết định Room có phải Combat/Boss hay không
→ chuẩn bị Enemy runtime
→ khóa/mở đúng Door của Room
→ theo dõi Enemy chết
→ phát OnRoomCleared đúng một lần
```

##### Flow đầy đủ của Combat Room runtime

```text
Edgar generate Room
        ↓
EdgarDungeonPostProcessing
        ↓
RoomContext
├── RoomId
├── RoomType
├── EnemySpawnPoints[]
└── RoomDoor[] được gán vào RoomController
        ↓
RoomController.StartCombat()
        ↓
IsCombatRoom() ?
├── Start / Reward / Shop → return
└── Combat / Boss → tiếp tục
        ↓
PrepareRuntimeEnemies()
        ↓
EnemySpawnPoints[]
        ↓
Instantiate Enemy tại từng SpawnPoint
        ↓
parent vào child Enemies
        ↓
roomEnemies[] được cập nhật
aliveEnemyCount = số Enemy vừa tạo
        ↓
Subscribe EnemyHealth.OnDeath
        ↓
LockDoors()
        ↓
ActivateEnemies()
        ↓
FIGHT
        ↓
EnemyHealth chết
        ↓
EnemyHealth.OnDeath
        ↓
RoomController.HandleEnemyDeath()
        ↓
aliveEnemyCount--
        ↓
CheckRoomCleared()
        ↓
aliveEnemyCount == 0 ?
        ↓ Có
isRoomCleared = true
isRoomActive = false
        ↓
UnlockDoors()
        ↓
OnRoomCleared?.Invoke()
        ↓
Room đã Clear thì StartCombat() lần sau return
```

##### Vì sao spawn Enemy nằm trong `RoomController`, không nằm trong Edgar post-processing?

`EdgarDungeonPostProcessing` chỉ có trách nhiệm **chuyển dữ liệu plugin thành dữ liệu project**. Nó thu thập `EnemySpawnPoint` và đưa reference vào `RoomContext`, nhưng không quyết định khi nào combat bắt đầu.

```text
EdgarDungeonPostProcessing
→ "Room này có 3 EnemySpawnPoint ở đây"

RoomController
→ "Khi Combat bắt đầu, tôi sẽ tạo/activate Enemy ở các điểm đó"
```

Nhờ vậy nếu sau này thay Edgar bằng generator khác, combat lifecycle vẫn chủ yếu làm việc qua `RoomContext` / `RoomController`.

##### Tương thích với CombatRoom cũ

CombatRoom cố định từ Tuần 3 có thể đã chứa Enemy sẵn trong child `Enemies`. Vì vậy `RoomController` vẫn giữ `FindRoomEnemies()` trong `Awake()`.

```text
Room cũ có Enemy sẵn
→ roomEnemies.Length > 0
→ không spawn thêm Enemy runtime

Room Edgar có Enemies rỗng
→ roomEnemies.Length == 0
→ khi StartCombat mới spawn từ EnemySpawnPoints[]
```

Cách này tránh lỗi tạo Enemy trùng khi regression test scene cũ.

##### Cách test ở thời điểm chưa có Player Spawn / Room Transition

Tuần 5 Thứ 3 mới nối Player qua cửa + Cinemachine + cập nhật Current Room. Vì vậy ở Thứ 2 không ép Player phải đi xuyên dungeon để test lifecycle.

Dùng debug ContextMenu trên `RoomController` trong Unity Editor:

```text
Test Start Combat
→ gọi trực tiếp StartCombat()

Test Defeat All Room Enemies
→ gọi EnemyHealth.TakeDamage(...) trên Enemy thật
→ vẫn đi qua EnemyHealth.OnDeath thật
→ vẫn kiểm tra đúng event/lifecycle
```

Bài test lifecycle cần xác nhận:

```text
Combat/Boss Room
→ Spawn đúng số Enemy từ SpawnPoint
→ Lock đúng Door của Room đó
→ Enemy active
→ Enemy chết: aliveEnemyCount giảm
→ Enemy cuối chết: RoomCleared
→ Unlock đúng Door của Room đó
→ OnRoomCleared chỉ phát một lần
→ gọi StartCombat lại sau Clear không spawn/lock lại
```

`RoomReward` hiện chỉ là script test cơ bản trong scene Gameplay cũ; **không phải dependency bắt buộc của Room Template Edgar trong task này**. Reward/loot runtime sẽ được xử lý ở các task Loot sau theo roadmap.

##### Runtime state/field quan trọng của `RoomController`

Ngoài state cũ:

```text
isRoomActive
isRoomCleared
aliveEnemyCount
roomEnemies[]
roomDoors[]
```

RoomController hiện dùng thêm ý nghĩa runtime:

```text
roomContext
→ biết RoomType + EnemySpawnPoints

enemyPrefab
→ prefab Enemy chung dùng cho SpawnPoint ở giai đoạn hiện tại

hasSpawnedRuntimeEnemies
→ ngăn một Room Instantiate Enemy lần thứ hai
```

Không tạo Enemy spawning Manager riêng ở giai đoạn này.

##### Thứ tự lifecycle bắt buộc

```text
1. Validate loại Room / state
2. PrepareRuntimeEnemies
3. isRoomActive = true
4. LockDoors
5. ActivateEnemies
6. Fight
7. EnemyHealth.OnDeath
8. aliveEnemyCount--
9. Clear khi count = 0
10. UnlockDoors
11. OnRoomCleared một lần
```

Không được đưa `CheckRoomCleared()` lên trước `PrepareRuntimeEnemies()` cho Room Edgar, vì `Enemies` container ban đầu rỗng.

##### Kết quả Play Test cuối task

Đã xác nhận:

```text
Combat Room 3 SpawnPoint → 3 Enemy runtime
Boss Room 1 SpawnPoint   → 1 Enemy runtime

Enemy parent đúng Enemies container
aliveEnemyCount đúng
Lock đúng Door của Room
Activate Enemy đúng lúc
EnemyHealth.OnDeath giảm count
Enemy cuối chết → Clear
Clear → Unlock đúng Door
OnRoomCleared chỉ 1 lần
StartCombat sau Clear không spawn/lock lại
Room A không điều khiển Door Room B
Console sạch
```

Trạng thái kiến trúc sau task:

```text
Edgar Layout/Data
      ↓
RoomContext
      ↓
RoomController
      ↓
Combat Lifecycle runtime
```

**Tuần 5 - Thứ 2 DONE.** Task tiếp theo là `Room Transition + Cinemachine`; Player/Camera/Minimap room-entry thực tế chưa được coi là hoàn thành.


#### 19M. Room Transition + Cinemachine — Tuần 5 Thứ 3 — DONE

Mục tiêu kiến trúc là biến room-entry thật thành nguồn duy nhất để cập nhật Current Room, Camera và Minimap mà không nhét dependency Cinemachine/UI vào `RoomController`.

##### Flow runtime hiện tại

```text
Edgar generate dungeon
        ↓
EdgarDungeonPostProcessing
        ├── RoomContext / Door / SpawnPoint / connection
        ├── inject RoomTransitionController cho RoomTrigger
        ├── inject Player Transform + CameraController cho RoomController
        └── tìm Start Room sau BuildMinimap
                ↓
RoomTransitionController.SetInitialRoom(Start)
        ├── CurrentRoom = Start
        ├── Player → Start.PlayerSpawnPoint
        └── CameraController.FocusRoom(Start)

Player vào Room B
        ↓
RoomTrigger
        ├── RoomTransitionController.EnterRoom(Room B)
        └── RoomController.StartCombat()
                ↓
RoomTransitionController
        ├── CurrentRoom = Room B
        ├── CameraController.FocusRoom(Room B)
        └── MinimapGenerator.SetCurrentRoom(Room B)
```

##### `RoomTransitionController`

Path:

```text
Assets/Scripts/Gameplay/Room/RoomTransitionController.cs
```

Trách nhiệm:
- giữ `CurrentRoom`;
- khởi tạo Start Room;
- đặt Player tại `PlayerSpawnPoint` lúc bắt đầu;
- yêu cầu `CameraController` focus Room;
- yêu cầu `MinimapGenerator` cập nhật Current Room.

Không sở hữu combat, enemy spawn/death, Edgar API hoặc trực tiếp thao tác `Main Camera.transform`.

Contract chính:

```text
SetInitialRoom(RoomContext)
EnterRoom(RoomContext)
CurrentRoom
```

`EnterRoom()` bỏ qua transition nếu `newRoom == currentRoom`.

##### Camera framing bằng `CameraAnchor`

Mỗi gameplay Room Template có child `CameraAnchor`.

`RoomContext.CameraAnchor` trỏ tới marker của chính Room runtime.

Camera flow:

```text
RoomContext.CameraAnchor.position
        ↓
CameraController.FocusRoom()
        ↓
CameraController.SetCameraPosition()
        ↓
RoomCameraTarget.position
        ↓
Cinemachine Camera Tracking Target
        ↓
Position Composer / Damping
        ↓
Main Camera
```

Normal Room hiện được frame cố định theo Room. Player chạy sát mép không làm Camera follow tự do.

Không dùng `RoomContext.LayoutPosition` làm vị trí Camera vì layout coordinate và world coordinate là hai hệ khác nhau.

Không dùng `DOMove()` cho Camera target trong flow hiện tại vì Cinemachine Position Composer đã xử lý chuyển động/damping; tránh DOTween và Cinemachine cùng điều khiển chuyển động.

##### `PlayerSpawnPoint`

Path:

```text
Assets/Scripts/Gameplay/Room/PlayerSpawnPoint.cs
```

Marker này hiện chỉ cần ở Start Room.

```text
Room_Start_01
├── CameraAnchor
└── PlayerSpawnPoint
```

`CameraAnchor` và `PlayerSpawnPoint` tách riêng trách nhiệm:

```text
CameraAnchor
→ tâm frame Camera

PlayerSpawnPoint
→ vị trí Player bắt đầu
```

`RoomContext` giữ `PlayerSpawnPoint`; `EdgarDungeonPostProcessing` thu reference sau generation.

##### `RoomTrigger` runtime contract

`RoomTrigger` vẫn là detector room-entry tối giản:

```text
Player enter
        ↓
RoomTransitionController.EnterRoom(roomContext)
        ↓
RoomController.StartCombat()
```

Không chứa logic Camera/Minimap cụ thể.

`RoomTransitionController` là Scene object nên Room prefab không giữ reference cứng. Post Processing inject reference vào RoomTrigger runtime sau khi Edgar instantiate Room.

##### Runtime Scene references cho Enemy

Enemy prefab có hai dependency thuộc Scene:

```text
EnemyAI.playerTransform
EnemyDeathCameraShake.cameraController
```

Không kéo Scene object vào prefab asset.

Contracts:

```text
EnemyAI.SetPlayerTransform(Transform)
EnemyDeathCameraShake.SetCameraController(CameraController)
```

`EdgarDungeonPostProcessing` cấp Player Transform + CameraController cho `RoomController`.

Khi chuẩn bị Enemy runtime (sau Tuần 5 - Thứ 4):

```text
RoomController.PrepareRuntimeEnemies()
        ↓
EnemyPool.GetEnemy(...)
        ↓
EnemyHealth / EnemyAI reset state
        ↓
ConfigureSpawnedEnemy()
        ├── EnemyAI.SetPlayerTransform(playerTransform)
        └── EnemyDeathCameraShake.SetCameraController(cameraController)
        ↓
inactive chờ combat
        ↓
combat mới Activate
```

Prefab vẫn độc lập Scene; reference Player/Camera được cấp lại runtime và Enemy được tái sử dụng thay vì Instantiate cho từng Room.

##### Player prefab

Player nên được prefab hóa khi cấu hình đã ổn định để tránh copy GameObject giữa các Scene làm lệch component/reference. Tuy nhiên tài liệu hiện **không coi việc prefab hóa Player là đã hoàn thành** nếu chưa có xác nhận riêng.

##### Regression đã Play Test

```text
Start Room spawn/current/camera/minimap đúng
Start → Combat đúng
Combat → Clear → Start đúng
Reward/Boss/nhiều Room đúng
Camera không follow Player tự do trong Normal Room
Camera không drift/giật
Minimap luôn đồng bộ CurrentRoom
Enemy runtime Chase Player đúng sau injection
EnemyDeathCameraShake đúng sau injection
```

**Tuần 5 - Thứ 3 DONE.**


#### 19N. Object Pooling — Tuần 5 Thứ 4 — DONE

Mục tiêu là mở rộng pooling từ Projectile sang Enemy, Drop và DamageText mà không đưa gameplay vào pool.

Ranh giới đã chốt:

```text
Pool
→ tạo/tái sử dụng/thu hồi object
→ reset lifecycle state

Room / Enemy / Loot / UI gameplay
→ quyết định khi nào object được dùng và ý nghĩa gameplay của nó
```

Không tạo `PoolManager` chung ở giai đoạn này.

##### Enemy runtime sau pooling

`EdgarDungeonPostProcessing` có `EnemyPool` Scene reference và inject vào từng `RoomController` bằng `SetEnemyPool()`.

```text
EnemyPool (Scene)
        ↓
EdgarDungeonPostProcessing
        ↓
RoomController.SetEnemyPool()
        ↓
PrepareRuntimeEnemies()
        ↓
EnemyPool.GetEnemy()
```

Death event được đổi từ `Action` sang `Action<EnemyHealth>` để listener nhận đúng Enemy vừa chết.

RoomController không release Enemy ngay giữa chuỗi `OnDeath`; nó hoàn tất count/clear trước rồi release vào frame kế tiếp để các listener death khác xử lý xong.

##### Drop / DamageText

`DropPool`/`PooledDrop` chỉ chuẩn bị object reuse cho Loot task sau; chưa roll loot và chưa có Coin/Heart/Item pickup thật.

`DamageTextPool`/`DamageText` dùng TextMeshPro World Space. Animation test dùng DOTween Sequence:

```text
DOMove lên trên
+
DOFade alpha về 0
→ OnComplete ReturnToPool
```

##### Kết quả test

```text
Enemy: Get → combat → death → Release → Room khác Get lại cùng object
Drop: Get → Release → Get lại cùng instance, state reset
DamageText: Show → move/fade → auto Release → Get lại cùng instance
Profiler: không còn spike Instantiate/Destroy lớn do Enemy combat sau khi pool đã có object
```

**Tuần 5 - Thứ 4 DONE.** Task tiếp theo là `Loot Data`.


#### 19O. Loot Data — Tuần 5 Thứ 5 — DONE

Loot được tách thành **pickup nhỏ** và **Item lớn** để tránh trộn hai loại reward có vai trò khác nhau.

##### Pickup nhỏ

```text
RoomController.OnRoomCleared
        ↓
RoomLootRoller
        ↓
RoomClearLootTable
        ↓
weighted roll
        ↓
None / Coin / Heart / Key / Bomb
```

Data:

```text
LootRewardType
→ None / Coin / Heart / Key / Bomb

LootEntry
├── RewardType
├── Weight
└── Amount

LootTable
└── LootEntry[] + RollLoot()
```

`Weight` là trọng số tương đối, không cần tổng bằng 100. `Weight <= 0` không tham gia roll.

`RoomLootRoller` chỉ yêu cầu roll và phát `OnLootRolled(LootEntry)`; chưa spawn physical pickup trong task này.

##### Item lớn

```text
Room Template
    ↓
RoomContext.ItemPoolData
    ↓
ItemRewardRoller
    ↓
ItemPoolData.RollItem()
    ↓
ItemData
```

Data:

```text
ItemData
→ identity/visual cơ bản của một Item

ItemPoolEntry
├── ItemData
└── Weight

ItemPoolData
└── ItemPoolEntry[] + RollItem()
```

Pool được phân theo nguồn/phòng:

```text
Reward Room → RewardItemPool
Boss Room   → BossItemPool
Angel Room  → AngelItemPool (về sau)
Devil Room  → DevilItemPool (về sau)
```

Một `ItemData` có thể thuộc nhiều pool và có Weight khác nhau ở từng pool. Vì vậy Weight nằm trong `ItemPoolEntry`, không nằm trong `ItemData`.

`RoomContext.ItemPoolData` là cấu hình prefab cố định, không cần Edgar post-processing gán runtime. `ItemRewardRoller` chỉ roll một lần mỗi Room và phát `OnItemRolled(ItemData)`; world item/pedestal và Item Effects chưa làm.

##### Ranh giới trách nhiệm

```text
LootTable / ItemPoolData
→ quyết định "ra reward nào"

RoomLootRoller / ItemRewardRoller
→ quyết định "khi được yêu cầu thì roll và phát kết quả"

DropPool / world pickup task sau
→ quyết định "object ngoài world được lấy/reuse thế nào"

Pickup gameplay task sau
→ quyết định "Player nhặt thì resource/health thay đổi thế nào"
```

**Tuần 5 - Thứ 5 DONE.** Task tiếp theo là `Loot & Floor Flow`.



#### 19P. Loot & Floor Flow — Tuần 5 Thứ 6 — DONE theo scope đã chốt

Đây là thay đổi kiến trúc lớn nối Loot Data với physical world object, đồng thời chuẩn hóa vị trí spawn bằng Tilemap marker.

##### Scope hoàn thành

```text
Small Pickup world spawn
Pickup collect/effect
Item pedestal world reward
FloorExit cơ bản
```

Deferred có chủ đích:

```text
DOTween pop / hút pickup
→ visual polish, chưa làm

NextFloor hoàn chỉnh / floor index / RunProgress
→ chuyển sang flow RunProgress/Boss Flow ở Tuần 7
```

##### Small Pickup runtime

```text
RoomController.OnRoomCleared
        ↓
RoomLootRoller
        ↓
LootTable.RollLoot()
        ↓
OnLootRolled(LootEntry)
        ↓
RoomLootDropSpawner
        ↓
RoomMarkerTilemap.SmallPickupMarkerTile
        ↓
DropPool.GetDrop()
        ↓
PooledDrop.Configure()
```

`PooledDrop` hiện giữ:
- `RewardType`;
- `RewardAmount`;
- visual sprite tương ứng Coin/Heart/Key/Bomb;
- owner pool + reset lifecycle.

`PooledDropPickup` xử lý trigger:

```text
Coin → PlayerResources.AddCoins()
Key  → PlayerResources.AddKeys()
Bomb → PlayerResources.AddBombs()
Heart → PlayerHealth.TryHeal()
```

`PlayerHealth.TryHeal()` trả `bool` để Heart chỉ bị tiêu thụ khi thực sự hồi được HP. Full HP → `false` → Heart vẫn nằm world.

Giai đoạn hiện tại dùng **một Collider2D chung** cho các small pickup. Collider riêng theo sprite chỉ là cải tiến sau nếu play test chứng minh cần.

##### Runtime dependency injection cho DropPool

Room prefab không giữ Scene reference `DropPool`.

```text
DropPool (Scene)
        ↓
EdgarDungeonPostProcessing
        ↓
RoomLootDropSpawner.SetDropPool()
        ↓
Room runtime
```

Giữ cùng pattern với `EnemyPool`; `RoomController` không bị kéo loot responsibility vào.

##### Item lớn / pedestal

```text
Reward Room
→ RewardItemPool
→ ItemRewardRoller
→ RoomItemRewardSpawner
→ ItemRewardMarkerTile
→ ItemPedestal
```

```text
Boss Room
→ Boss Clear
→ BossItemPool
→ ItemRewardRoller
→ RoomItemRewardSpawner
→ ItemRewardMarkerTile
→ ItemPedestal
```

`ItemPedestal` hiện chỉ giữ/hiển thị `ItemData`; Player collect + Item Effects chưa làm.

##### FloorExit cơ bản

```text
Boss chưa clear
→ chưa spawn Exit

Boss clear
→ FloorExitSpawner
→ FloorExitMarkerTile
→ Instantiate FloorExit prefab

Player chạm FloorExit
→ OnPlayerEnteredExit
```

`FloorExit.prefab` vẫn là gameplay object thật với Collider/script. Marker Tile chỉ xác định cell spawn.

##### Gameplay Marker Tilemap — contract mới

Room Template có Tilemap riêng:

```text
GameplayMarkers
├── Tilemap
├── TilemapRenderer
└── RoomMarkerTilemap
```

Marker tile hiện tại:

```text
SmallPickupMarkerTile
ItemRewardMarkerTile
FloorExitMarkerTile
```

Marker là `TileBase` asset được paint vào cell, **không phải child GameObject**.

`RoomMarkerTilemap` dùng helper chung:

```text
TryGetMarkerWorldPosition(TileBase markerTile, out Vector3 worldPosition)
        ↓
markerTilemap.cellBounds
        ↓
markerTilemap.GetTile(cell)
        ↓
markerTilemap.GetCellCenterWorld(cell)
```

Ranh giới:

```text
Marker Tile
→ WHERE: spawn ở đâu

PooledDrop / ItemPedestal / FloorExit prefab
→ WHAT/HOW: object gì và gameplay làm gì
```

`GameplayMarkers` renderer có thể tắt lúc Play mà Tilemap data vẫn đọc được.

##### Migration khỏi Empty Transform marker

Đã cleanup khỏi **current** architecture:

```text
RewardSpawnPoint
FloorExitSpawnPoint
```

Không còn:
- field/getter/setter tương ứng trong `RoomContext`;
- collect tương ứng trong `EdgarDungeonPostProcessing`;
- Empty marker object trong Room Template;
- script marker tương ứng trong current folder structure.

Vẫn giữ:

```text
EnemySpawnPoint
PlayerSpawnPoint
```

Enemy Spawn Marker Tile là hướng được người dùng đồng ý cân nhắc về sau, nhưng **chưa làm** để tránh refactor hệ enemy đang ổn ngoài roadmap.

##### Boss reward orchestration

Hiện không có `BossRewardManager`:

```text
RoomController.OnRoomCleared
├── RoomItemRewardSpawner
└── FloorExitSpawner
```

Chỉ cân nhắc Coordinator/Manager khi Boss Clear có sequence nhiều bước phụ thuộc thứ tự. Manager khi đó chỉ điều phối; không tự ôm roll item, camera, audio, pickup và progression vào một God Object.

##### Regression đã Play Test

```text
✓ Small pickup đúng marker + pooling.
✓ None không spawn.
✓ Coin/Key/Bomb resource đúng.
✓ Heart full HP không bị tiêu thụ; thiếu HP heal đúng.
✓ Reward/Boss pedestal đúng Item Pool + marker.
✓ Boss trước clear chưa có Item/Exit.
✓ Boss clear spawn Item + FloorExit đúng hai marker riêng.
✓ FloorExit trigger đúng một lần.
✓ Edgar regenerate vẫn khớp Grid marker.
✓ Cleanup marker Transform cũ không gây Missing Script/NullReference.
✓ Console sạch.
```

**Tuần 5 - Thứ 6 DONE theo scope đã chốt. Task tiếp theo: Tuần 6 - Thứ 2 — Enemy Architecture.**

## Tùy chọn — Tuần 6

### A* Pathfinding Project
Chưa cài.

Chỉ cân nhắc nếu Enemy direct chase trong dungeon Edgar thực tế thường xuyên kẹt tường hoặc cần tìm đường thật.

## Không cần hiện tại
- Pooling plugin ngoài → dùng `UnityEngine.Pool.ObjectPool<T>`.
- Save plugin → dự kiến tự làm JSON.
- Behavior Tree framework → tự viết State Machine vừa đủ.
- Game Feel framework khác → DOTween đã đủ.
- Odin Inspector → chỉ cân nhắc sau nếu Inspector data quá rối.

---

# 20. Kiến trúc module hiện tại của project

> Kiến trúc module phải bám theo folder structure thực tế trong Unity. Lịch sử cũ có thể nhắc `RewardSpawnPoint`, nhưng **current architecture sau Tuần 5 ưu tiên Marker Tile**.

```text
Core
→ Bootstrap / GameManager / SceneLoader

Data
├── PlayerData / FloorData
├── LootRewardType / RoomType
├── LootTable + LootEntry
└── ItemData + ItemPoolData + ItemPoolEntry

Gameplay/Player
→ input / movement / dash / shooter / health / run resources tối thiểu

Gameplay/Enemy
→ health / AI / hit feedback / pool

Gameplay/Projectile
→ projectile / collision / pool

Gameplay/Room
├── RoomContext / RoomController / RoomDoor / RoomTrigger
├── RoomTransitionController
├── RoomLootRoller
├── RoomMarkerTilemap
├── EnemySpawnPoint
└── PlayerSpawnPoint

Gameplay/Pickup
├── DropPool
├── PooledDrop
├── PooledDropPickup
└── RoomLootDropSpawner

Gameplay/Item
├── ItemRewardRoller
├── RoomItemRewardSpawner
└── ItemPedestal

Gameplay/Dungeon
├── EdgarDungeonGenerator
├── EdgarDungeonPostProcessing
├── FloorEdgarConfig
├── FloorExit
└── FloorExitSpawner

Gameplay/Camera
→ CameraController / EnemyDeathCameraShake

UI
→ Loading / HUD / DamageText / Minimap
```

### Ranh giới trách nhiệm current

```text
LootTable / ItemPoolData
→ quyết định reward/item nào

RoomLootRoller / ItemRewardRoller
→ yêu cầu roll và phát kết quả

RoomMarkerTilemap
→ đọc cell marker và trả world position

DropPool
→ reuse PooledDrop

PooledDropPickup
→ áp small pickup lên Player

RoomItemRewardSpawner
→ spawn pedestal đúng marker

FloorExitSpawner
→ spawn Exit sau Boss Clear

FloorExit
→ interaction Player với Exit

RoomController
→ combat room lifecycle; không roll loot và không spawn reward

EdgarDungeonPostProcessing
→ adapter Edgar + inject Scene runtime refs; không sở hữu gameplay reward rules
```

### Manager policy hiện tại

Không tạo `BossRewardManager` chỉ để gom hai listener. Manager/Coordinator chỉ được cân nhắc khi có **orchestration thật** giữa nhiều bước và thứ tự thực thi trở thành requirement.

---

# 21. Những thứ cố ý CHƯA làm

Không được tự giả định các phần sau đã tồn tại hoặc đã hoàn thành:
- Enemy Architecture nâng cao / State Machine `Idle / Chase / Attack` — task kế tiếp.
- Tách Movement/Attack thành component/interface tái sử dụng.
- Enemy Ranged.
- DamageInfo / DamageCalculator.
- DOTween pop/hút Small Pickup ngoài world.
- Player nhặt Item lớn và Item Effects/stat modifier.
- NextFloor hoàn chỉnh, floor index và RunProgress/ProgressionManager.
- Enemy Spawn Marker Tile; hiện `EnemySpawnPoint` GameObject vẫn là contract thật.
- A* Pathfinding.
- Boss system hoàn chỉnh / Boss sequence orchestration.
- Save.
- Audio.
- Main Menu / Pause / GameOver.
- Large Room camera bounds/follow riêng.

Đã có nền tảng thật và không được coi là “chưa làm” nữa:
- Physical Coin/Heart/Key/Bomb pickup ngoài world.
- Pickup collect cập nhật `PlayerResources` / `PlayerHealth` và ReturnToPool.
- Heart full HP không bị tiêu thụ.
- Loot weighted data: `LootTable`, `LootEntry`, `RoomLootRoller`.
- Item pool weighted data: `ItemData`, `ItemPoolEntry`, `ItemPoolData`, `ItemRewardRoller`.
- Reward/Boss Item pedestal world spawn.
- FloorExit cơ bản sau Boss Clear + trigger event.
- `GameplayMarkers` Tilemap + `RoomMarkerTilemap` cho Small Pickup / Item Reward / FloorExit.
- `RewardSpawnPoint` / `FloorExitSpawnPoint` đã được thay bằng Tile marker trong current flow.
- Enemy pooling + Drop pooling + DamageText pooling.
- Room lifecycle / Room transition / Cinemachine / Minimap.
- Edgar Free runtime generation + adapter boundary.

---

# 23. Quy trình debug bắt buộc

Nếu game có bug:
1. Xem thông báo lỗi trong Unity Console.
2. Giải thích dòng lỗi nói gì.
3. Xác định script + line gây lỗi.
4. Giải thích nguyên nhân.
5. Sau đó mới sửa code.
6. Test lại.
7. Chỉ đánh dấu DONE sau khi người dùng xác nhận.

Không đưa code sửa ngay khi chưa đọc lỗi nếu Console đã có error cụ thể.

---

# 24. Quy tắc giải thích code

- Giải thích theo từng bước nhỏ.
- Không dùng thuật ngữ phức tạp nếu chưa giải thích.
- Dùng ví dụ thực tế/ẩn dụ khi cần.
- Code C# viết tường minh.
- Không viết tắt khó đọc.
- Chia hàm rõ trách nhiệm.
- Comment tiếng Việt ở các dòng quan trọng.

Khi dùng DOTween bắt buộc giải thích:
1. Hàm DOTween đó là gì.
2. Các tham số trong `()` có nghĩa gì.
3. Tại sao dùng DOTween thay vì tự viết logic thông thường.
4. Tween đang tác động object nào.
5. Có nguy cơ xung đột physics/timeScale không.

Quy định khi dùng plugin Edgar Generation:  
1. Hãy giải thích các hàm và các từ ngữ chuyên dụng có sử dụng trong tài liệu.
2. Trước khi làm, giải thích quy trình thực hiện.
---

# 25. Quy tắc kiến trúc quan trọng

## Không over-engineer
Không tự thêm (chỉ thêm khi cần thiết):
- Dependency Injection framework.
- Service Locator.
- ECS.
- EventManager toàn cục.
- Manager cho mọi hệ thống.
- Interface chỉ để “trông chuyên nghiệp”.
- Abstraction chưa có nhu cầu thật.

## Single responsibility ở mức vừa đủ

```text
PlayerController      → Input
PlayerMovement        → movement physics
PlayerDash            → dash timing/state
PlayerShooter         → shooting
Projectile            → projectile movement/lifetime
ProjectileCollision   → collision filtering + chuyển damage sang EnemyHealth
ProjectilePool        → object reuse
EnemyHealth           → enemy HP + damage/death events
EnemyHitFeedback      → enemy hit visual
EnemyAI               → detection/chase/contact attack cơ bản ở Tuần 3; refactor architecture ở Tuần 6
PlayerHealth          → player HP + invincibility + health/death events
DamageSource          → contact damage request
```

## Physics vs Visual

Root:
```text
Rigidbody2D
Collider2D
gameplay
```

Child Visual:
```text
SpriteRenderer
Tween
Animation
Flash
Scale
Punch
```

## ScriptableObject
Dùng cho configuration/static data.

Không mutate asset gốc trực tiếp trong runtime cho stat thay đổi của một run.

---
