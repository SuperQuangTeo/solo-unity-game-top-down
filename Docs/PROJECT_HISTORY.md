# PROJECT HISTORY — Roguelike 2D Unity

> Nhật ký kỹ thuật của các task đã hoàn thành, bug đã gặp, cách xử lý và các bài regression đã được người dùng Play Test xác nhận.
> File này dùng để tra cứu lịch sử; **không phải file bắt đầu phiên chat mới**.

---

## Trạng thái lịch sử hiện tại

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
Tuần 4 - Thứ 2      DONE
Tuần 4 - Thứ 3      DONE
Tuần 4 - Thứ 4      DONE
Tuần 4 - Thứ 5      DONE
Tuần 4 - Thứ 6      DONE
Tuần 5 - Thứ 2      DONE
Tuần 5 - Thứ 3      DONE
Tuần 5 - Thứ 4      DONE
Tuần 5 - Thứ 5      DONE
Tuần 5 - Thứ 6      DONE
Tuần 6 - Thứ 2      DONE  — Enemy Architecture
Tuần 6 - Thứ 3      DONE  — Enemy Ranged

Tổng: 27 / 50 task DONE
```

Chi tiết từng task, bug và checklist test được giữ bên dưới từ handoff cũ.

---

# 2. Tiến độ hiện tại

## Tuần 1 — Core & Bootstrap
**DONE toàn bộ.**

Đã hoàn thành:
- Khởi tạo project Unity 2D.
- Git repository.
- Import và setup DOTween.
- Cấu trúc folder cơ bản.
- `GameManager`.
- `SceneLoader`.
- Loading Screen.
- Fade In / Fade Out bằng DOTween.
- Scene `Bootstrap`.
- `Bootstrap.cs`.
- `GameManager` + `SceneLoader` được đặt trong Bootstrap.
- `DontDestroyOnLoad` hoạt động.
- Bootstrap tự load scene tiếp theo thành công.
- Đã test không tạo manager trùng khi chuyển scene.

### Luồng Core hiện tại

```text
Game Start
   ↓
Bootstrap Scene
   ↓
GameManager + SceneLoader
   ↓
Bootstrap.cs
   ↓
SceneLoader.LoadScene(...)
   ↓
LoadingScreen.Show()
   ↓
LoadSceneAsync()
   ↓
LoadingScreen.Hide()
   ↓
Gameplay Scene
```

## Tuần 2 — Thứ 2: Input System
**DONE.**

Input Actions hiện tại:

```text
Player
├── Move
│   └── WASD
├── Shoot
│   └── Arrow Keys
└── Dash
    └── Space
```

Đã test:
- W / A / S / D đúng Vector2.
- Di chuyển chéo hoạt động.
- Arrow Keys trả về đúng hướng bắn.
- Move và Shoot hoạt động độc lập.
- Ví dụ: giữ D và bắn ↑ vẫn cho Move = phải, Shoot = lên.

## Tuần 2 — Thứ 3: Player Movement
**DONE.**

Đã hoàn thành và test:
- `PlayerController.cs`
- `PlayerMovement.cs`
- `PlayerDash.cs`
- `PlayerData.cs`
- `DefaultPlayerData.asset`
- Dash bằng Space.
- Dash cooldown.
- Dash theo hướng Move hiện tại.
- Không Dash nếu đang đứng yên.
- `DOPunchScale()` cho Visual khi Dash.
- Root Player không bị scale khi Visual tween.
- ScriptableObject thay đổi MoveSpeed/Dash data và gameplay cập nhật đúng.

### Hierarchy Player hiện tại

```text
Player
├── Rigidbody2D
├── Collider2D
├── PlayerController
├── PlayerMovement
├── PlayerDash
├── PlayerShooter
├── PlayerHealth
├── FirePoint
└── Visual
    └── SpriteRenderer
```

### Quy tắc Player Visual
- Root `Player` giữ Rigidbody2D, Collider2D và gameplay scripts.
- Child `Visual` giữ SpriteRenderer và hiệu ứng scale/punch/visual bằng DOTween.
- Không tween scale root Player vì có thể ảnh hưởng physics/collider.

## Tuần 2 — Thứ 4: Player Combat
**DONE.**

Đã test Projectile, ProjectilePool, collision filtering và object reuse thành công.

## Tuần 2 — Thứ 5: Combat Foundation
**DONE.**

Đã hoàn thành và test:
- Enemy Dummy + Layer Enemy.
- EnemyData / EnemyHealth.
- Projectile damage.
- OnDamaged / OnDeath.
- Enemy hit flash bằng DOTween.
- Death chỉ phát một lần.

## Tuần 2 — Thứ 6: Player Health
**DONE.**

Đã hoàn thành và test:
- PlayerData Health config.
- PlayerHealth.
- Contact DamageSource.
- Invincibility window.
- OnHealthChanged / OnDeath.
- Event độc lập HUD.
- Death chỉ phát một lần.


## Tuần 3 — Thứ 2: HUD UI
**DONE — người dùng đã chạy thử và xác nhận.**

Đã hoàn thành và test:
- `HUDCanvas` + `HUDRoot`.
- Kiến trúc UI tối giản theo hướng MVP: `HUDPresenter` đứng giữa nguồn gameplay và `HUDView`.
- `HUDPresenter` lắng nghe `PlayerHealth.OnHealthChanged`; không poll HP trong `Update()`.
- HP hiển thị dạng Heart kiểu Isaac: Full / Half / Empty.
- Quy ước hiện tại: `1 HP = nửa trái tim`, `2 HP = 1 trái tim đầy`.
- Heart không còn gắn cứng bằng `Image[]` trong Inspector.
- `HUDView` nhận `maxHealth` từ Presenter, tự tính số Heart cần thiết và `Instantiate` `HeartUI.prefab` vào `HeartContainer`.
- `HUDView` không đọc trực tiếp `PlayerData`; nguồn runtime đi theo `PlayerHealth → HUDPresenter → HUDView`.
- Heart thay đổi trạng thái được `DOPunchScale()` nhẹ; có `DOKill()` và reset scale để tránh tween chồng.
- Ở thời điểm task HUD, Coin mới dùng `TestCurrencySource` tạm để kiểm thử event. Đến Vertical Slice, nguồn test này đã được thay bằng `PlayerResources.OnCoinsChanged` trong gameplay thật.
- `CoinText` cập nhật qua `OnCurrencyChanged`, không poll trong `Update()`.
- `CoinText` dùng `DOPunchScale()` khi giá trị thay đổi.
- Đã test Heart tự sinh với các trường hợp HP/MaxHealth cần thiết, Full/Half/Empty đúng, animation đúng.
- Đã test Coin test tăng đúng qua event và animation hoạt động.
- Console không có lỗi đỏ trong test cuối task.

### Quyết định thay đổi so với dòng roadmap gốc
Roadmap ban đầu ghi `Slider.DOValue()` cho HP. Người dùng đã chọn HUD máu dạng Isaac nên phần HP được điều chỉnh có chủ đích thành:

```text
Slider HP
→ không dùng

Heart HUD
├── Full Heart
├── Half Heart
├── Empty Heart
└── DOPunchScale khi trạng thái thay đổi
```

Không quay lại thêm Slider cho HP trừ khi người dùng đổi thiết kế.



## Tuần 3 — Thứ 3: Enemy AI cơ bản
**DONE — người dùng đã chạy thử và xác nhận.**

Đã hoàn thành và test:
- Bổ sung `MoveSpeed`, `AttackDamage`, `AttackCooldown` vào `EnemyData`.
- Chuẩn bị `Rigidbody2D` cho Enemy Dummy.
- Tạo `EnemyAI.cs`.
- Detection Player bằng khoảng cách + `detectionRange`.
- Chase Player bằng `Rigidbody2D.MovePosition()` và `EnemyData.MoveSpeed`.
- Contact attack dùng `EnemyData.AttackDamage`.
- Attack cooldown dùng `EnemyData.AttackCooldown`.
- Khi contact, Enemy dừng Chase để tránh chạy xuyên qua Player liên tục.
- Telegraph attack bằng `DOPunchScale()` trên child `Visual`, không tween root physics.
- Player có thể né attack nếu rời contact trước khi telegraph kết thúc.
- `DamageSource` cũ được gỡ khỏi Enemy Dummy đang dùng `EnemyAI` để tránh hai nguồn cấu hình damage song song.
- `EnemyAI` lắng nghe `EnemyHealth.OnDeath`; khi Enemy chết thì dừng Detection / Chase / Attack.
- Nếu Enemy chết trong lúc telegraph, attack coroutine bị dừng, tween telegraph bị kill/reset và Player không nhận damage sau đó.
- `ChasePlayer()` có guard `isDead` riêng ngoài guard ở `FixedUpdate()` để tự bảo vệ hàm nếu sau này bị gọi từ nơi khác.
- Đã test Enemy chết khi Chase, chết giữa telegraph và đứng chạm xác Enemy; AI không còn di chuyển/gây damage.
- Console không có lỗi đỏ trong bài test cuối task.

### Bug gameplay đã phát hiện và sửa trong task này

Triệu chứng:
```text
EnemyHealth.currentHealth = 0
→ OnDeath đã phát
→ nhưng EnemyAI vẫn Chase / Attack
```

Nguyên nhân:
- `EnemyHealth` và `EnemyAI` là hai module độc lập.
- `EnemyAI` trước đó chưa đăng ký `EnemyHealth.OnDeath`, nên AI không biết Enemy đã chết.

Cách xử lý đã test:
```text
EnemyHealth.OnDeath
        ↓
EnemyAI.HandleDeath()
        ↓
isDead = true
        ↓
dừng Chase
dừng Attack Coroutine
DOKill telegraph
reset Visual scale
không xử lý contact damage nữa
```

Quy tắc:
- Không poll `EnemyHealth.CurrentHealth` mỗi frame chỉ để biết death.
- Dùng event `OnDeath` vì death là sự kiện xảy ra một lần.
- `OnEnable()` đăng ký event và `OnDisable()` hủy đăng ký để phù hợp với pooling về sau.

### Quyết định kiến trúc Enemy đã chốt cho tương lai

Không refactor `EnemyAI` thành nhiều abstraction ngay ở Tuần 3.

Mốc refactor chính giữ tại **Tuần 6 – Thứ 2: Enemy Architecture**:
- Refactor thành State Machine vừa đủ: `Idle / Chase / Attack`.
- Tách state logic khỏi visual; `EnemyData` tiếp tục giữ stat.
- Khi đã thực sự xuất hiện từ **hai kiểu Movement hoặc Attack khác nhau**, đánh giá tách Movement/Attack thành component tái sử dụng.
- Chỉ thêm `IEnemyMovement` / `IEnemyAttack` khi có nhiều implementation cần chung một contract.
- Không tạo interface/abstraction trước khi behavior thứ hai thực sự xuất hiện.
- `Enemy Ranged` ở Tuần 6 – Thứ 3 sẽ là bài kiểm chứng trực tiếp cho architecture vừa refactor.
- A* Pathfinding vẫn chỉ là decision gate: chỉ cân nhắc nếu direct chase trong dungeon Edgar thực tế thường xuyên kẹt tường/vật cản.

Hướng dài hạn ưu tiên **Composition** hơn cây kế thừa Enemy lớn, nhưng chỉ áp dụng khi nhu cầu thực tế xuất hiện.


---


## Tuần 3 — Thứ 4: Room Combat
**DONE — người dùng đã chạy thử và xác nhận.**

Đã hoàn thành và test:
- Tạo `CombatRoom` cố định với `RoomController`, `RoomTrigger`, `RoomDoor` và `Enemies` container.
- `RoomTrigger` dùng `BoxCollider2D` với `Is Trigger = true` để phát hiện Player bước vào.
- `RoomController` giữ `isRoomActive`, `isRoomCleared`, `aliveEnemyCount` và danh sách Enemy thuộc riêng room.
- Enemy được tìm bằng `enemyContainer.GetComponentsInChildren<EnemyHealth>(true)` để vẫn nhận được Enemy đang inactive.
- Enemy con được đặt inactive trước combat; khi Player vào, `StartCombat()` khóa cửa rồi activate Enemy.
- `RoomController` đăng ký `EnemyHealth.OnDeath`; mỗi Enemy chết làm giảm `aliveEnemyCount` đúng một lần.
- Khi `aliveEnemyCount == 0` trong lúc room đang active: đánh dấu clear, kết thúc combat, mở cửa và phát `OnRoomCleared`.
- `isRoomCleared` chặn việc combat/clear chạy lại khi Player đi ra rồi quay vào room.
- `RoomDoor` tự quản lý collider của cửa: Lock → bật collider; Unlock → tắt collider.
- `RoomController` quản lý `RoomDoor[]` để hỗ trợ một hoặc nhiều cửa mà không cần Manager riêng.
- Đã test room có nhiều Enemy: Enter → Lock → Activate → Fight → Clear → Unlock.
- Đã test room không có Enemy: Enter → clear ngay → Unlock, không nhốt Player vĩnh viễn.
- Đã test `OnRoomCleared` bằng listener tạm và xác nhận chỉ phát đúng một lần.
- Console không có lỗi đỏ trong bài test cuối task.

### Hierarchy CombatRoom hiện tại

```text
CombatRoom
├── RoomController
├── RoomTrigger
│   └── BoxCollider2D (Is Trigger = true)
├── Doors
│   └── RoomDoor
│       └── BoxCollider2D (Is Trigger = false)
└── Enemies
    └── EnemyDummy... (inactive trước combat)
```

### Luồng Room Combat đã chốt

```text
Player Enter
    ↓
RoomTrigger
    ↓
RoomController.StartCombat()
    ↓
isRoomActive = true
    ↓
LockDoors()
    ↓
ActivateEnemies()
    ↓
Fight
    ↓
EnemyHealth.OnDeath
    ↓
aliveEnemyCount--
    ↓
aliveEnemyCount == 0
    ↓
isRoomCleared = true
isRoomActive = false
    ↓
UnlockDoors()
    ↓
OnRoomCleared (một lần)
```

Quy tắc:
- `RoomController` không poll HP Enemy trong `Update()`.
- Room dùng `EnemyHealth.OnDeath` làm contract để biết Enemy chết.
- Edgar/procedural dungeon chưa tham gia; đây vẫn là CombatRoom cố định của vertical slice.


## Tuần 3 — Thứ 5: Camera - Cinemachine
**DONE — người dùng đã chạy thử và xác nhận.**

Package hiện tại:
```text
Cinemachine 3.1.7
```

Đã hoàn thành và test:
- Cài Cinemachine 3.1.7.
- `Main Camera` có `Cinemachine Brain`.
- Tạo `PlayerCamera` bằng `Cinemachine Camera` và dùng root `Player` làm `Tracking Target`.
- Camera giữ góc nhìn 2D Orthographic; không follow child `Visual` đang chạy DOTween.
- `Cinemachine Position Composer` dùng cho follow/framing.
- Giá trị framing đã test hiện tại: Damping X/Y khoảng `0.3`; Dead Zone Width/Height khoảng `0.15`; Lookahead chưa dùng.
- Camera follow Player ổn định khi đi thường, đi chéo và Dash.
- Thêm `Cinemachine Impulse Listener` và `Cinemachine Impulse Source` để camera shake.
- Tạo `CameraController.cs` làm lớp giao tiếp nhỏ; gameplay gọi `ShakeCamera(float shakeForce)` thay vì gọi Cinemachine trực tiếp ở nhiều nơi.
- `CameraController` dùng `GenerateImpulseWithForce(shakeForce)` để phát impulse.
- Không dùng `DOShakePosition()` trên Main Camera để tránh DOTween và Cinemachine cùng tranh quyền điều khiển transform camera.
- Tạo `EnemyDeathCameraShake.cs` làm adapter gameplay: nghe `EnemyHealth.OnDeath` rồi gọi `CameraController.ShakeCamera()`.
- Enemy chết vẫn đồng thời dừng AI, giảm Room enemy count, clear room đúng; camera shake không làm Player dịch chuyển.
- Shake kết thúc thì camera trở lại follow bình thường.
- Console không có lỗi đỏ trong bài test cuối task.

### Luồng Camera hiện tại

```text
Player root
   ↓ Tracking Target
PlayerCamera
├── Cinemachine Camera
├── Position Composer
├── Impulse Listener
├── Impulse Source
└── CameraController
        ↓
Cinemachine Brain
        ↓
Main Camera
```

Gameplay shake:
```text
EnemyHealth.OnDeath
        ↓
EnemyDeathCameraShake
        ↓
CameraController.ShakeCamera()
        ↓
Cinemachine Impulse
        ↓
PlayerCamera / Main Camera shake
```

Quy tắc:
- Gameplay không tự sửa `Main Camera.transform`.
- Không rải API Cinemachine vào `EnemyHealth`, `RoomController`, Projectile hoặc UI.
- `CameraController` là điểm giao tiếp nhỏ cho các yêu cầu camera của gameplay.
- Camera framing theo từng Room sẽ tiếp tục ở đúng task Tuần 5 - Thứ 3, không kéo lên sớm.

### Thiết kế Camera / Room đã chốt cho giai đoạn Dungeon

> Đây là **thiết kế đích cho Tuần 5**, chưa được coi là đã triển khai ở Tuần 3. Camera Player-follow hiện tại chỉ là nền tảng/prototype của Vertical Slice.

```text
Normal Room
→ 1 Room ≈ 1 màn hình
→ Camera framing theo Room, không follow Player tự do
→ Player đứng sát mép vẫn không nhìn thấy Room kế bên

Qua Door
→ xác nhận Player đã vào Room mới
→ cập nhật Current Room
→ chuyển camera sang framing của Room mới

Large Room
→ Camera có thể follow Player
→ nhưng phải bị giới hạn trong bounds của chính Room
→ không được lộ Room kế bên
```

Combat Room về sau dùng ngữ nghĩa **Room Entry**:
```text
Player thực sự Enter Room
        ↓
Room có Enemy và chưa Clear?
        ↓ Có
Lock Doors ngay
        ↓
Activate / Spawn Enemy
        ↓
Fight → Clear → Unlock
```

Quyết định quan trọng:
- Không dùng việc Player đi tới giữa phòng mới bắt đầu combat làm thiết kế cuối cùng.
- `RoomTrigger` lớn của fixed CombatRoom hiện tại chỉ là detector tối thiểu phục vụ Vertical Slice.
- Khi tích hợp Edgar/Room Transition ở Tuần 5, room-entry sẽ được chuẩn hóa theo Door/Room context.
- Door không tự sở hữu combat logic; `RoomController` vẫn là nơi quyết định Lock/Activate/Clear/Unlock.
- Camera framing/transition đi qua `CameraController`; gameplay không tự điều khiển `Main Camera.transform`.


## Tuần 3 — Thứ 6: Vertical Slice
**DONE — người dùng đã chạy regression toàn vòng và xác nhận.**

Đã hoàn thành và test:
- Tạo `PlayerResources.cs` trên root Player để giữ resource runtime tối thiểu: `Coin`, `Key`, `Bomb`.
- Giữ code tường minh với `AddCoins()`, `AddKeys()`, `AddBombs()` và các event riêng `OnCoinsChanged`, `OnKeysChanged`, `OnBombsChanged`; chưa generic hóa khi chưa cần.
- Vertical Slice hiện chỉ nối **Coin** vào gameplay thật; Key/Bomb mới có nền tảng data/runtime, chưa làm physical pickup hoặc gameplay sử dụng.
- `HUDPresenter` đã bỏ nguồn `TestCurrencySource` và lắng nghe `PlayerResources.OnCoinsChanged`.
- `HUDView` tiếp tục cập nhật CoinText và `DOPunchScale()` khi Coin thay đổi.
- Tạo `RoomReward.cs`: lắng nghe `RoomController.OnRoomCleared`, sau đó gọi `PlayerResources.AddCoins(coinRewardAmount)`.
- Reward chỉ xảy ra một lần vì `OnRoomCleared` chỉ phát một lần.
- Đã dọn listener/source test tạm sau khi nguồn gameplay thật hoạt động.
- Đã regression toàn vòng: Movement / Dash / Shooting / PlayerHealth / HUD / Enemy AI / Room Combat / Camera / Reward Coin.
- Đi ra rồi vào lại Room đã clear không khóa cửa, không activate lại Enemy và không cộng Reward lần hai.
- Stop/Play lại reset resource runtime về 0 đúng với trạng thái hiện tại chưa có RunProgress/Save.
- Console không có lỗi đỏ trong bài test cuối Vertical Slice.

### Luồng Vertical Slice hiện tại

```text
Player Enter CombatRoom
        ↓
Lock Door + Activate Enemy
        ↓
Fight
├── Player nhận damage → PlayerHealth → HUD Heart
├── Projectile damage → EnemyHealth / Hit Feedback
└── Enemy death → EnemyAI stop + Camera Shake + Room enemy count--
        ↓
Enemy cuối chết
        ↓
RoomCleared
├── Unlock Doors
└── RoomReward
      ↓
PlayerResources.AddCoins()
      ↓
OnCoinsChanged
      ↓
HUDPresenter → HUDView → CoinText
```

### PlayerResources — quyết định kiến trúc hiện tại

```text
PlayerResources
├── Coins
├── Keys
└── Bombs
```

- Đây là component runtime trên Player, **không phải CurrencyManager/RunManager**.
- Chưa làm hệ `RunProgress` đầy đủ; mốc đó vẫn giữ ở Tuần 7.
- Physical `Coin/Key/Bomb Pickup` chưa làm ở đây; pickup thật vẫn đi đúng roadmap Tuần 5.
- Khi RunProgress xuất hiện, sẽ đánh giá nơi sở hữu resource xuyên scene/run; không refactor sớm ở Tuần 3.


## Tuần 4 — Thứ 2: Edgar - Setup & Prototype
**DONE — người dùng đã generate/test và xác nhận Console không có lỗi đỏ.**

Đã hoàn thành và test:
- Project đang dùng Unity `6000.5.2f1`.
- Thử cài Edgar bằng Git URL `#upm` trước; bản `upm` cũ gây lỗi compile `CS0619` tại `AssetDatabase.GetAssetPath(int)` trên Unity 6000.5.
- Không sửa trực tiếp package trong `Library/PackageCache`; đã gỡ package lỗi và xác nhận Console sạch.
- Chuyển sang Edgar Free `2.1.0` bằng `.unitypackage`; import thành công và Console không có lỗi đỏ.
- Tạo scene prototype riêng `EdgarPrototype.unity`, không sửa Gameplay Scene chính trong bước prototype.
- Tạo Tile Palette riêng cho prototype; tileset cơ bản `16x16` dùng `Pixels Per Unit = 16` để 1 tile = 1 Unity Unit.
- Tạo 3 Room Template đơn giản bằng Tilemap với cấu trúc mặc định của Edgar: `Floor`, `Walls`, `Collideable`, `Other 1`, `Other 2`, `Other 3`.
- Room thường dùng `Simple Door Mode`; `Door Length = 1`, sau đó tăng `Door Margin = 2` để tránh cửa quá sát góc khi dùng corridor.
- Tạo `PrototypeLevelGraph` gồm 3 node nối tuyến tính `Room 1 → Room 2 → Room 3`.
- Gán 3 room vào `Default Room Templates`.
- Ban đầu generate với `Use Corridors = OFF`: layout đúng nhưng wall giữa các room nằm sát/chồng layer ở điểm nối.
- Tạo `CorridorHorizontal` và `CorridorVertical`; cả hai dùng `Manual Door Mode`, đúng 2 door ở hai cạnh đối diện.
- Gán corridor vào `Corridor Room Templates`, bật lại `Use Corridors`.
- Generate dungeon mẫu thành công: cấu trúc đúng graph, room không chồng nhau, corridor tạo khoảng nối hợp lệ, Console không có lỗi đỏ.

### Quyết định kỹ thuật từ prototype Edgar

```text
Edgar
→ sinh layout / chọn Room Template / kết nối room

Code project
→ RoomController / RoomDoor / Enemy / Reward / Camera / lifecycle gameplay
```

- Không đưa gameplay thật vào Room Template ở task prototype.
- Không tự viết procedural layout thay Edgar.
- Không sửa package trong `Library/PackageCache` khi plugin không tương thích Unity; ưu tiên version plugin tương thích chính thức.
- Bước tiếp theo là Tuần 4 - Thứ 3: chuẩn hóa Room Template gameplay Start/Combat/Reward/Boss.



## Tuần 4 — Thứ 3: Edgar - Room Templates
**DONE — người dùng đã generate/test bộ 4 loại room và xác nhận Console không có lỗi đỏ.**

Đã hoàn thành và test:
- Chuẩn hóa hierarchy nền cho Room Template gameplay: `Tilemaps`, `RoomTrigger`, `Doors`, `Enemies`, `EnemySpawnPoints`, `RewardSpawnPoint`.
- Root Room Template dùng `RoomController` hiện có; `Enemy Container` trỏ về child `Enemies`.
- `RoomTrigger` dùng `BoxCollider2D (Is Trigger = true)` + `RoomTrigger.cs`; reference `roomController` trỏ về `RoomController` của chính room.
- Tạo marker component `EnemySpawnPoint.cs` và `RewardSpawnPoint.cs`; hiện chỉ đánh dấu Transform, chưa chứa logic spawn.
- Tạo `RoomDoor.prefab` độc lập với `BoxCollider2D` + `RoomDoor.cs`; trạng thái mặc định Unlock, `Lock()` bật collider, `Unlock()` tắt collider.
- Không đặt cứng RoomDoor vào Simple Door Mode của Edgar vì connection thực tế chỉ được biết sau generate; child `Doors` của template hiện để rỗng, runtime integration sẽ xử lý ở Tuần 4 - Thứ 5.
- Tạo bộ template tối thiểu: `Room_Start_01`, `Room_Combat_01`, `Room_Reward_01`, `Room_Boss_01`.
- `Room_Start_01`: không EnemySpawnPoint, RewardSpawnPoint inactive.
- `Room_Combat_01`: 3 EnemySpawnPoint, RewardSpawnPoint active.
- `Room_Reward_01`: không EnemySpawnPoint, RewardSpawnPoint active.
- `Room_Boss_01`: 1 EnemySpawnPoint, RewardSpawnPoint active.
- Test một Room gameplay riêng sau Edgar generate: các GameObject/component và reference nội bộ vẫn giữ đúng.
- Tạo graph test 4 loại room theo tuyến `Start → Combat → Reward → Boss`, gán Individual Room Template cho từng node.
- Generate thành công đủ 4 loại room; hierarchy/marker giữ đúng, Console không có lỗi đỏ.

### Quyết định kiến trúc từ Room Templates

```text
Edgar Simple Door positions
→ chỉ là vị trí kết nối có thể dùng

RoomDoor gameplay
→ prefab độc lập
→ chỉ gắn/khởi tạo ở đúng connection thực tế sau generation
```

- `RoomController` tiếp tục sở hữu lifecycle gameplay; Edgar không sở hữu combat/damage/reward logic.
- SpawnPoint hiện chỉ là marker, chưa thêm Spawner/WaveManager.
- `RoomReward` Vertical Slice vẫn là logic cộng Coin trực tiếp; chưa đổi sang physical reward trong task này.
- Bước tiếp theo: Tuần 4 - Thứ 4 — `FloorData` + graph/floor configuration.



## Tuần 4 — Thứ 4: Edgar - Graph & Floor Data
**DONE — người dùng đã tạo data/graph, test validation, generate nhiều lần và xác nhận hoạt động.**

Mục tiêu của task:

```text
Room Template đã chuẩn hóa
        ↓
FloorData mô tả một Floor bằng dữ liệu project
        ↓
LevelGraph mô tả quan hệ các Room cho Edgar Free
        ↓
FloorEdgarConfig ánh xạ 2 phần ở một điểm tập trung
```

### Bước 1 — tạo `RoomType`

Path:

```text
Assets/Scripts/Data/Enum/RoomType.cs
```

Giá trị:

```text
Start
Combat
Reward
Shop
Boss
```

Quyết định:
- `RoomType` là ngôn ngữ của project, không phải type của Edgar.
- `Shop` được chuẩn bị trong enum nhưng chưa có Room Template Shop thật.

### Bước 2 — tạo `FloorData`

Path:

```text
Assets/Scripts/Data/ScriptableObjects/Dungeon/FloorData.cs
```

Data:

```text
FloorData
├── RoomCount
├── DifficultyLevel
├── RoomTypes[]
├── UseRandomSeed
└── FixedSeed
```

Asset đã tạo:

```text
Assets/Data/Dungeon/Floors/Floor_01_Data.asset
```

Giá trị test:

```text
RoomCount       = 6
DifficultyLevel = 1
RoomTypes       = Start / Combat / Reward / Boss
UseRandomSeed   = true
FixedSeed       = 12345
```

### Bước 3 — hiểu giới hạn Edgar Free

Edgar Free dùng Fixed Level Graph, vì vậy project **không tự viết procedural graph generator** để thay phần PRO.

Quy ước:

```text
FloorData.RoomCount
→ metadata / validation

LevelGraph asset
→ graph thật để Edgar generate
```

### Bước 4 — tạo Floor 01 Level Graph

Asset:

```text
Assets/Data/Dungeon/Edgar/Floor_01_LevelGraph.asset
```

Graph:

```text
              Reward
                 |
Start — Combat_01 — Combat_02 — Combat_03 — Boss
```

Kết quả:

```text
6 gameplay Room
5 graph connections
```

Template mapping:
- Start → `Room_Start_01`
- Combat nodes → `Room_Combat_01`
- Reward → `Room_Reward_01`
- Boss → `Room_Boss_01`

### Bước 5 — tạo `FloorEdgarConfig`

Path:

```text
Assets/Scripts/Gameplay/Dungeon/FloorEdgarConfig.cs
```

Asset:

```text
Assets/Data/Dungeon/Edgar/Floor_01_EdgarConfig.asset
```

Flow:

```text
FloorData
   +
LevelGraph
   ↓
FloorEdgarConfig
```

`OnValidate()` kiểm tra:

```text
RoomCount có khớp graph node count không?
Có RoomType.Start không?
Có RoomType.Boss không?
```

Đã test bằng cách cố ý đổi RoomCount / thiếu Boss để đảm bảo warning xuất hiện.

### Bước 6 — regression graph

Đã generate nhiều lần và xác nhận:
- đủ 6 gameplay Room;
- Start có đường hợp lệ tới Boss;
- Reward là nhánh phụ;
- corridor hoạt động;
- room không overlap;
- Console sạch.

Quyết định kiến trúc:

```text
FloorData
→ không gọi Edgar

FloorEdgarConfig
→ điểm ánh xạ asset project ↔ Edgar

RoomController
→ không biết LevelGraph/Edgar
```

---

## Tuần 4 — Thứ 5: Edgar - Runtime Integration
**DONE — người dùng đã test runtime generation, regenerate nhiều seed, RoomContext, Door và SpawnPoint.**

Mục tiêu:

```text
Floor config
    ↓
Generate dungeon runtime
    ↓
Lấy RoomInstance Edgar
    ↓
Chuyển thành dữ liệu gameplay project
```

### Bước 1 — `EdgarDungeonGenerator`

Path:

```text
Assets/Scripts/Gameplay/Dungeon/EdgarDungeonGenerator.cs
```

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

Adapter gán:

```text
FloorEdgarConfig.LevelGraph
→ DungeonGeneratorGrid2D.FixedLevelGraphConfig.LevelGraph

FloorData.UseRandomSeed
→ DungeonGeneratorGrid2D.UseRandomSeed

FloorData.FixedSeed
→ DungeonGeneratorGrid2D.RandomGeneratorSeed
```

Edgar component đặt:

```text
Generate On = Manually
```

để không generate hai lần.

### Bước 2 — `EdgarDungeonPostProcessing`

Path:

```text
Assets/Scripts/Gameplay/Dungeon/EdgarDungeonPostProcessing.cs
```

Kế thừa:

```text
DungeonGeneratorPostProcessingComponentGrid2D
```

Edgar gọi `Run(level)` sau khi dungeon sinh xong.

Đầu tiên đã test:
- `level.RoomInstances`;
- phân biệt gameplay Room và Corridor bằng `IsCorridor`;
- lấy `RoomTemplateInstance`;
- tìm `RoomController` trên root template runtime.

### Bước 3 — đọc Door data của Edgar

Từ mỗi gameplay Room:

```text
roomInstance.Doors
    ↓
DoorInstanceGrid2D
├── DoorLine
├── FacingDirection
└── ConnectedRoomInstance
```

Đã debug vị trí Door bằng cách đổi local grid cell sang world:

```text
DoorLine.From / To
        ↓
Grid.GetCellCenterWorld()
        ↓
trung điểm Door
        ↓
Debug.DrawLine dấu + đỏ
```

### Bước 4 — tạo `RoomDoor` gameplay runtime

Thiết kế:
- không tạo Door cứng sẵn ở mọi candidate door;
- chỉ instantiate `RoomDoor.prefab` tại Door Edgar thực sự sử dụng;
- chỉ tạo ở gameplay Room, không tạo thêm ở Corridor.

Flow:

```text
RoomInstance.Doors
      ↓
GetDoorWorldPosition()
      ↓
Instantiate RoomDoor.prefab
      ↓
parent = Room/Doors
      ↓
RoomController.SetRoomDoors(RoomDoor[])
```

Đã test thành công:
- đúng 10 RoomDoor runtime cho 5 graph connections;
- vị trí đúng doorway;
- parent đúng `Doors`;
- collider mặc định Unlock;
- Console sạch.

### Bước 5 — tạo `RoomContext`

Path:

```text
Assets/Scripts/Gameplay/Room/RoomContext.cs
```

Ban đầu dùng để tách identity/runtime data khỏi Edgar API.

Các Room Template được gán `RoomType` cố định:

```text
Start  → Start
Combat → Combat
Reward → Reward
Boss   → Boss
```

Post Processing gán `RoomId` runtime tuần tự.

Quy tắc:
- ID chỉ để định danh runtime;
- không giả định Start có ID 0;
- muốn tìm Start dùng `RoomType.Start`.

### Bước 6 — thu thập SpawnPoint

Post Processing chỉ thu reference, **chưa spawn Enemy/Reward**:

```text
Room runtime
   ↓
GetComponentsInChildren<EnemySpawnPoint>(true)
   ↓
RoomContext.EnemySpawnPoints[]
```

`true` được dùng vì Enemy marker là dữ liệu vị trí, kể cả object inactive vẫn cần tìm được.

Reward:

```text
GetComponentInChildren<RewardSpawnPoint>()
```

nên Start có Reward marker inactive được coi là không có reward runtime.

Kết quả test:

```text
Start  → 0 Enemy, Reward None
Combat → 3 Enemy, Reward Có
Reward → 0 Enemy, Reward Có
Boss   → 1 Enemy, Reward Có
```

### Bước 7 — test regenerate / multi-seed

Đã dùng ContextMenu debug để gọi `GenerateDungeon()` nhiều lần trong cùng Play Mode.

Đã kiểm tra:
- không giữ dungeon cũ;
- vẫn đúng 6 gameplay Room;
- RoomId reset theo dungeon mới;
- Door không duplicate;
- SpawnPoint reference thuộc dungeon mới;
- random/fixed seed hoạt động;
- Console sạch.

Kết luận kiến trúc sau task:

```text
Edgar API
    ↓
EdgarDungeonGenerator + EdgarDungeonPostProcessing
    ↓
RoomContext / RoomController
    ↓
Gameplay khác
```

---

## Tuần 4 — Thứ 6: Minimap từ Edgar Data
**DONE — người dùng đã test room layout, state, logical connection/corridor và sửa regression icon placement.**

Mục tiêu:

```text
Room data từ Edgar
       ↓
chuyển sang RoomContext
       ↓
MinimapGenerator
       ↓
Room Icon + Connection + Current/Visited state
```

Không dùng Minimap PRO của Edgar.

### Bước 1 — thêm runtime state/layout vào `RoomContext`

Bổ sung:

```text
LayoutPosition
IsVisited
ConnectedRooms[]
```

`LayoutPosition` lấy từ:

```text
roomInstance.Position (Vector3Int)
      ↓
X/Y
      ↓
Vector2Int
      ↓
RoomContext.LayoutPosition
```

`IsVisited` là runtime state, không đặt trong FloorData ScriptableObject.

### Bước 2 — dựng UI minimap

Hierarchy:

```text
HUDCanvas
└── MinimapRoot
    ├── ConnectionContainer
    └── RoomIconContainer
```

Prefab:

```text
MinimapRoomIcon.prefab
MinimapConnection.prefab
```

Không dùng Grid/Horizontal/Vertical Layout Group vì dungeon layout là tự do.

### Bước 3 — `MinimapGenerator`

Path:

```text
Assets/Scripts/UI/Minimap/MinimapGenerator.cs
```

Flow build:

```text
BuildMinimap(RoomContext[])
     ↓
ValidateReferences()
     ↓
ClearMinimap()
     ↓
FindStartRoom()
     ↓
Start.LayoutPosition = minimap origin
     ↓
CreateRoomIcon() cho từng Room
     ↓
CreateRoomConnections()
     ↓
SetCurrentRoom(Start)
```

Tọa độ UI:

```text
relativeLayoutPosition
= room.LayoutPosition - start.LayoutPosition

minimapPosition
= relativeLayoutPosition × minimapScale

RectTransform.anchoredPosition
= minimapPosition
```

### Bước 4 — `MinimapRoomIcon`

Mỗi icon giữ reference tới đúng `RoomContext`.

Logic hiển thị:

```text
Current?
├── Có  → Current Color
└── Không
      ↓
   IsVisited?
   ├── Có  → Visited Color
   └── Không → Unvisited Color
```

`MinimapGenerator` chỉ giữ một `currentRoom` reference, không tạo `isCurrentRoom` trên mọi RoomContext.

### Bước 5 — logical connection qua Corridor Edgar

Post Processing làm pass thứ hai sau khi mọi RoomContext đã sẵn sàng.

Flow:

```text
Room A Door
    ↓
ConnectedRoomInstance
    ↓
Corridor?
├── Không → gameplay Room B
└── Có
     ↓
   duyệt Door Corridor
     ↓
   bỏ lối quay về Room A
     ↓
   Room B
```

Sau đó lưu:

```text
RoomContext A.ConnectedRooms += B
RoomContext B.ConnectedRooms += A
```

Corridor Edgar không được đưa vào `RoomContext`.

Đã test:
- Start / Boss có 1 connection;
- Combat nhánh có 3;
- các Combat còn lại đúng theo graph;
- tổng reference hai chiều = 10;
- graph connection thật = 5.

### Bước 6 — vẽ Connection trên UI

Connection Minimap là đường biểu diễn quan hệ Room A ↔ Room B, **không phải hình corridor tile chính xác ngoài world**.

Để tránh line duplicate vì dữ liệu hai chiều:

```text
chỉ tạo nếu
roomA.RoomId < roomB.RoomId
```

Công thức:

```text
Direction = B - A
Length    = Direction.magnitude
MidPoint  = (A + B) / 2
Angle     = Atan2(Direction.y, Direction.x)
```

UI line:

```text
anchoredPosition = MidPoint
width            = Length
rotation Z       = Angle
```

Floor 01 mong đợi:

```text
6 Room Icons
5 Connection Lines
```

### Bug regression quan trọng khi thêm connection

Triệu chứng:

```text
Connection đúng layout
nhưng tất cả Room Icon chồng vào tâm
```

Điều này giúp khoanh vùng:
- `RoomContext.LayoutPosition` đúng;
- `CalculateMinimapPosition()` đúng;
- lỗi nằm ở nhánh áp dữ liệu vào Room Icon.

Trong `CreateRoomIcon()` đã có:

```csharp
Vector2 minimapPosition = CalculateMinimapPosition(...);
```

nhưng bị thiếu:

```csharp
createdRoomIconRectTransform.anchoredPosition = minimapPosition;
```

Sau khi thêm lại dòng trên, layout icon hoạt động đúng.

Bài học:

```text
Tính đúng giá trị
≠
Object đã nhận giá trị
```

Khi refactor code dùng chung, phải kiểm tra cả bước tính và bước apply.

### Regression cuối Tuần 4

Đã xác nhận các phần cần thiết để chuyển task:

```text
6 gameplay Room
5 logical connection
Room icon khớp layout
Start làm Current ban đầu
Visited / Unvisited logic có API hoạt động
Regenerate dọn icon/connection/reference cũ
Door / SpawnPoint không regression
Edgar API vẫn bị cô lập sau adapter
```

Task kế tiếp:

```text
Tuần 5 - Thứ 2
Room Lifecycle + Edgar
```



## Tuần 5 — Thứ 2: Room Lifecycle + Edgar
**DONE — người dùng đã Play Test đầy đủ và xác nhận.**

### Mục tiêu

Tái sử dụng lifecycle `CombatRoom` đã làm ở Tuần 3 cho các Room runtime được Edgar sinh, không tạo một combat system thứ hai.

Flow đích:

```text
Room runtime Edgar
        ↓
Enter / StartCombat
        ↓
Lock Door
        ↓
Spawn / Activate Enemy
        ↓
Fight
        ↓
Enemy chết hết
        ↓
Room Clear
        ↓
Unlock Door
```

### Bước 1 — phân biệt Room có combat

`RoomController` lấy `RoomContext` trên cùng root Room.

Logic:

```text
RoomType.Start   → không combat
RoomType.Reward  → không combat
RoomType.Shop    → không combat
RoomType.Combat  → combat
RoomType.Boss    → combat
```

`RoomTrigger` vẫn chỉ có trách nhiệm báo Player đi vào và gọi `RoomController.StartCombat()`; không nhét luật `RoomType` vào Trigger.

Lý do:

```text
RoomTrigger
→ "Player đã vào Room"

RoomController
→ "Room này có cần chạy Combat lifecycle không?"
```

Giữ trách nhiệm rõ ràng và tránh logic bị chia đôi.

### Bước 2 — spawn Enemy runtime từ `EnemySpawnPoint`

Room Template Edgar có child `Enemies` rỗng và marker `EnemySpawnPoint`, nên `Awake()` không thể tìm thấy Enemy thật ngay từ đầu.

Vấn đề trước tích hợp:

```text
Room Edgar vừa instantiate
→ Enemies rỗng
→ roomEnemies.Length = 0
→ aliveEnemyCount = 0
→ StartCombat
→ CheckRoomCleared
→ Room clear ngay ❌
```

Giải pháp được chốt trong `RoomController`:

```text
StartCombat()
        ↓
PrepareRuntimeEnemies()
        ↓
RoomContext.EnemySpawnPoints[]
        ↓
Instantiate Enemy prefab tại từng SpawnPoint
        ↓
parent vào child Enemies
        ↓
SetActive(false)
        ↓
roomEnemies[] = Enemy runtime
aliveEnemyCount = roomEnemies.Length
        ↓
Subscribe EnemyHealth.OnDeath
```

Giai đoạn hiện tại dùng **một Enemy prefab chung** cho các SpawnPoint Combat/Boss. Chưa xây hệ spawn nhiều loại Enemy vì roadmap chưa cần.

### Bước 3 — tương thích CombatRoom cũ

`RoomController.Awake()` vẫn gọi `FindRoomEnemies()` để scene Vertical Slice cũ tiếp tục chạy.

Quy tắc:

```text
Room cũ có Enemy đặt sẵn
→ roomEnemies.Length > 0
→ hasSpawnedRuntimeEnemies = true
→ không Instantiate thêm

Room Edgar có Enemies rỗng
→ roomEnemies.Length == 0
→ hasSpawnedRuntimeEnemies = false
→ StartCombat mới spawn từ SpawnPoint
```

Điều này ngăn regression tạo Enemy gấp đôi.

### Bước 4 — thứ tự StartCombat đã chốt

```text
StartCombat()
        ↓
IsCombatRoom() ?
        ↓
isRoomCleared ?
        ↓
isRoomActive ?
        ↓
PrepareRuntimeEnemies()
        ↓
isRoomActive = true
        ↓
LockDoors()
        ↓
ActivateEnemies()
        ↓
CheckRoomCleared()
```

`PrepareRuntimeEnemies()` phải chạy **trước** `CheckRoomCleared()`, nếu không Room Edgar sẽ bị coi là rỗng và clear ngay.

### Bước 5 — death event và Room Clear

Mỗi Enemy runtime đăng ký:

```text
EnemyHealth.OnDeath
        ↓
RoomController.HandleEnemyDeath()
        ↓
aliveEnemyCount--
        ↓
CheckRoomCleared()
```

Enemy cuối chết:

```text
aliveEnemyCount = 0
        ↓
isRoomCleared = true
isRoomActive = false
        ↓
UnlockDoors()
        ↓
OnRoomCleared?.Invoke()
```

Không poll HP Enemy trong `Update()`.

### Bước 6 — cách test khi chưa có Room Transition/Cinemachine

Ở thời điểm này Player chưa được đưa qua từng Room Edgar bằng flow cuối cùng, nên không ép test bằng cách chạy Player qua dungeon.

Dùng ContextMenu Editor tạm trên `RoomController`:

```text
Test Start Combat
→ gọi StartCombat()

Test Defeat All Room Enemies
→ gọi EnemyHealth.TakeDamage(int.MaxValue)
→ đi qua EnemyHealth.OnDeath thật
```

Cách test này kiểm tra đúng lifecycle mà không kéo task Tuần 5 - Thứ 3 lên sớm.

### Kết quả Play Test đã xác nhận

```text
✓ Combat Room 3 SpawnPoint tạo đúng 3 Enemy runtime.
✓ Boss Room 1 SpawnPoint tạo đúng 1 Enemy runtime.
✓ Enemy được parent vào child Enemies của đúng Room.
✓ aliveEnemyCount nhận đúng số Enemy.
✓ Door của Room được Lock khi StartCombat.
✓ Enemy được Activate khi combat bắt đầu.
✓ EnemyHealth.OnDeath làm aliveEnemyCount giảm đúng.
✓ Enemy cuối chết → isRoomCleared = true.
✓ Sau Clear → isRoomActive = false.
✓ Door của Room được Unlock sau Clear.
✓ OnRoomCleared chỉ phát đúng một lần.
✓ StartCombat sau khi Room đã Clear không spawn Enemy mới và không Lock Door lại.
✓ Combat Room A không Lock/Unlock nhầm Door của Room B.
✓ Regression trên Combat/Boss Room runtime Edgar hoạt động.
✓ Console không có lỗi trong bài test cuối.
```

### Quyết định về `RoomReward`

`RoomReward` hiện là **script test của Vertical Slice trong scene Gameplay cũ** để chứng minh `OnRoomCleared → PlayerResources.AddCoins()`.

Nó **không phải dependency của Room Template Edgar** và không được kéo vào task Room Lifecycle + Edgar.

Reward/Loot runtime thật sẽ tiếp tục ở đúng các task Loot phía sau roadmap.

### Kết luận task

```text
Edgar
→ layout / Room Template / Door / Corridor

RoomContext
→ dữ liệu Room runtime / SpawnPoint / connection

RoomController
→ Spawn Enemy / Lock / Fight / Clear / Unlock

EnemyHealth
→ nguồn event death
```

`Room Lifecycle + Edgar` đạt tiêu chí DONE.

Task tiếp theo:

```text
Tuần 5 - Thứ 3
Room Transition + Cinemachine
```


## Tuần 5 — Thứ 3: Room Transition + Cinemachine
**DONE — người dùng đã Play Test toàn flow và xác nhận.**

### Mục tiêu

Kết nối Room runtime Edgar với Current Room, Camera/Cinemachine và Minimap theo room-entry thật:

```text
Player vào Room mới
        ↓
RoomTrigger
        ↓
RoomTransitionController.EnterRoom(newRoom)
        ↓
Current Room đổi
        ├── CameraController.FocusRoom(newRoom)
        ├── MinimapGenerator.SetCurrentRoom(newRoom)
        └── RoomController.StartCombat()
```

Không viết lại Room lifecycle đã hoàn thành ở Thứ 2.

### Bước 1 — Camera từ Player-follow sang Room framing

Tạo `RoomCameraTarget` trong Scene và đổi `Cinemachine Camera.Tracking Target` từ Player sang `RoomCameraTarget`.

Normal Room hiện dùng:

```text
CameraAnchor của Room
        ↓
CameraController.SetCameraPosition()
        ↓
RoomCameraTarget
        ↓
Cinemachine Position Composer
        ↓
Main Camera
```

Kết quả:
- Player chạy trong Room nhưng Camera không follow Player tự do.
- Khi `RoomCameraTarget` đổi vị trí, Cinemachine tự chuyển frame.
- Cinemachine Impulse / camera shake vẫn hoạt động.
- Không dùng `DOMove()` cho Camera/RoomCameraTarget; Damping của Cinemachine chịu trách nhiệm chuyển động Camera để tránh hai hệ cùng điều khiển.

### Bước 2 — `CameraAnchor` cho Room

Mỗi gameplay Room Template được thêm child marker `CameraAnchor`.

`RoomContext` giữ `CameraAnchor`.

`CameraController` có contract:

```text
FocusRoom(RoomContext)
        ↓
roomContext.CameraAnchor.position
        ↓
SetCameraPosition(Vector2)
```

Không dùng `RoomContext.LayoutPosition` làm world camera position vì đó là layout coordinate phục vụ dungeon/minimap, không phải world-space camera center.

### Bước 3 — `RoomTransitionController`

Tạo component nhỏ để sở hữu `CurrentRoom` và điều phối Camera + Minimap:

```text
RoomTransitionController
├── CurrentRoom
├── CameraController
└── MinimapGenerator
```

Contract chính:

```text
SetInitialRoom(RoomContext)
EnterRoom(RoomContext)
```

`EnterRoom()` bỏ qua nếu `newRoom == currentRoom`, tránh transition lặp lại cùng một Room.

Không tạo `RoomManager`/`DungeonManager`.

### Bước 4 — RoomTrigger giữ trách nhiệm nhỏ

`RoomTrigger` tiếp tục:
- phát hiện Player entry;
- gọi `RoomController.StartCombat()` như lifecycle cũ;
- báo `RoomTransitionController.EnterRoom(roomContext)`.

Không để `RoomTrigger` tự chứa logic Cinemachine/Minimap.

`RoomTransitionController` được inject cho từng RoomTrigger runtime sau Edgar generation, không kéo Scene object vào Room prefab.

### Bước 5 — Edgar post-processing nối runtime references

`EdgarDungeonPostProcessing`:
- nhận `RoomTransitionController` của Scene;
- sau khi tạo từng gameplay Room, tìm `RoomTrigger` và gọi `SetRoomTransitionController(...)`;
- sau `BuildMinimap()` tìm `RoomType.Start`;
- gọi `RoomTransitionController.SetInitialRoom(startRoom)`.

Minimap vẫn tự `SetCurrentRoom(Start)` khi build; RoomTransitionController chịu Current Room gameplay + Camera.

### Bước 6 — Player spawn vào Start Room

Thêm marker `PlayerSpawnPoint` chỉ cho Start Room.

`RoomContext` giữ `PlayerSpawnPoint`; Post Processing thu reference tương tự Enemy/Reward spawn marker.

`RoomTransitionController.SetInitialRoom()`:
- gán `currentRoom = Start`;
- đặt `Player Rigidbody2D.position` vào `Start.PlayerSpawnPoint`;
- gọi `CameraController.FocusRoom(Start)`.

Không dùng `CameraAnchor` làm Player spawn để tránh camera center và player spawn phụ thuộc nhau.

### Bước 7 — runtime dependency injection cho Enemy prefab

Enemy prefab có hai reference thuộc Scene không thể kéo cứng vào prefab:

```text
EnemyAI.playerTransform
EnemyDeathCameraShake.cameraController
```

Đã thêm contract runtime:

```text
EnemyAI.SetPlayerTransform(...)
EnemyDeathCameraShake.SetCameraController(...)
```

`EdgarDungeonPostProcessing` cấp Player Transform + CameraController cho `RoomController`.

Khi `RoomController` instantiate Enemy:

```text
Instantiate Enemy
        ↓
ConfigureSpawnedEnemy()
        ├── EnemyAI ← Player runtime Transform
        └── EnemyDeathCameraShake ← CameraController runtime
        ↓
SetActive(false)
        ↓
Activate khi combat bắt đầu
```

Không dùng `FindObjectOfType()`/`GameObject.Find()` và không tạo Manager chỉ để tìm Player/Camera.

### Play Test đã xác nhận

```text
✓ Dungeon generate bình thường.
✓ Player tự xuất hiện tại PlayerSpawnPoint của Start Room.
✓ CurrentRoom ban đầu = Start.
✓ Camera frame CameraAnchor của Start.
✓ Minimap Start = Current.

✓ Player Start → Combat:
  CurrentRoom đổi đúng.
  RoomCameraTarget tới CameraAnchor mới.
  Camera frame đúng Room mới.
  Minimap Start = Visited, Combat = Current.
  Combat lifecycle vẫn Spawn/Lock/Activate đúng.

✓ Combat → Clear → Start:
  Door Unlock đúng.
  CurrentRoom quay về Start.
  Camera quay về Start.
  Minimap cập nhật hai chiều đúng.
  Room đã Clear không spawn/lock lại.

✓ Đi qua Reward/Boss/nhiều Room:
  camera center đúng;
  Camera không follow Player khi chạy sát mép;
  không drift/giật;
  CurrentRoom và Minimap Current luôn đồng bộ.

✓ Enemy runtime nhận Player Transform bằng code và Chase Player đúng.
✓ Enemy runtime nhận CameraController bằng code và EnemyDeathCameraShake vẫn chạy.
✓ Console không có lỗi gameplay trong test cuối.
```

### Ghi chú Editor Edgar

Trong lúc compile từng xuất hiện:

```text
SerializedObjectNotCreatableException: Object at index 0 is null
Edgar.Unity.Editor.DungeonGeneratorInspector.OnEnable()
```

Stack trace nằm trong Custom Inspector của Edgar (`Assets/Edgar/Editor/...`), không đi qua gameplay code.

Sau khi lock/unlock Inspector và compile lại, lỗi không còn. Runtime vẫn bình thường nên **không sửa source Edgar**.

### Player prefab

Nên chuyển Player ổn định thành prefab để nhiều Scene dùng cùng cấu hình. Tuy nhiên việc prefab hóa Player chưa được xác nhận riêng và **không phải tiêu chí DONE của Room Transition + Cinemachine**.

### Kết luận

```text
Edgar runtime Room
        ↓
RoomContext + RoomTrigger
        ↓
RoomTransitionController
        ├── Current Room
        ├── CameraController → RoomCameraTarget → Cinemachine
        └── MinimapGenerator
        ↓
RoomController
        └── Combat lifecycle
```

**Tuần 5 - Thứ 3 DONE.**


## Tuần 5 — Thứ 4: Object Pooling
**DONE — người dùng đã Play Test + Profiler và xác nhận.**

### Mục tiêu

Mở rộng pooling từ Projectile sang Enemy, Drop và DamageText nhưng giữ pool chỉ làm nhiệm vụ cấp/thu hồi object, không chứa gameplay.

```text
ProjectilePool (đã có)
        ↓
EnemyPool
DropPool
DamageTextPool
```

Không tạo `PoolManager`/`PoolRegistry` chung vì dependency hiện tại chưa cần.

### Bước 1 — chuẩn hóa reset contract của Enemy

`EnemyHealth` trước đây chỉ khởi tạo HP trong `Awake()`, nên object reuse sẽ giữ `currentHealth = 0` và `isDead = true` nếu không reset.

Đã thêm `EnemyHealth.ResetForReuse()`:

```text
currentHealth = EnemyData.MaxHealth
isDead = false
```

`EnemyAI` có nhiều runtime state cần làm sạch:

```text
isDead
isPlayerDetected
isTouchingPlayer
isAttacking
nextAttackTime
attackCoroutine
DOPunchScale tween
Rigidbody2D velocity/angular velocity
```

Đã thêm `EnemyAI.ResetForReuse()` và các helper cleanup để dừng coroutine, Kill tween, trả Visual về scale gốc và reset Rigidbody2D.

`EnemyHitFeedback` không cần refactor lớn vì `OnDisable()` đã unsubscribe `OnDamaged`, `DOKill()` và trả màu gốc.

### Bước 2 — death event truyền đúng Enemy vừa chết

Đổi contract:

```text
EnemyHealth.OnDeath
Action
→ Action<EnemyHealth>
```

Mục đích:

```text
Enemy A chết
→ RoomController nhận chính Enemy A
→ Release đúng Enemy A về pool
```

Các listener `EnemyAI`, `EnemyDeathCameraShake`, `RoomController` cập nhật handler signature tương ứng.

### Bước 3 — `EnemyPool`

Tạo `EnemyPool.cs` dùng:

```text
ObjectPool<EnemyHealth>
```

Flow:

```text
GetEnemy(position, roomEnemyContainer)
        ↓
OnGetEnemy
├── EnemyHealth.ResetForReuse()
└── EnemyAI.ResetForReuse()
        ↓
parent vào Room/Enemies
đặt SpawnPoint
        ↓
vẫn inactive
        ↓
RoomController.ActivateEnemies() khi combat bắt đầu
```

Release:

```text
SetActive(false)
→ các component OnDisable cleanup
→ parent trở lại EnemyPool
```

### Bước 4 — nối `RoomController` + Edgar runtime

`RoomController` bỏ trách nhiệm `Instantiate(enemyPrefab)` cho Room Edgar và nhận `EnemyPool` runtime.

```text
EdgarDungeonPostProcessing
        ↓
RoomController.SetEnemyPool(enemyPool)
        ↓
RoomController.PrepareRuntimeEnemies()
        ↓
EnemyPool.GetEnemy(...)
```

`ConfigureSpawnedEnemy()` vẫn cấp lại:

```text
EnemyAI.SetPlayerTransform(playerTransform)
EnemyDeathCameraShake.SetCameraController(cameraController)
```

CombatRoom cũ có Enemy đặt sẵn vẫn tương thích; chỉ Enemy runtime mượn từ pool mới được release về `EnemyPool`.

### Bước 5 — release sau death event

Không release Enemy ngay giữa chuỗi `OnDeath`.

Flow đã chốt:

```text
Enemy chết
→ OnDeath bắt đầu
→ RoomController unsubscribe Enemy này
→ aliveEnemyCount--
→ CheckRoomCleared()
→ EnemyAI / CameraShake và listener khác hoàn tất death handling
→ sang frame kế tiếp
→ EnemyPool.ReleaseEnemy(deadEnemy)
```

Cách này tránh disable GameObject trong lúc các listener khác của cùng death event còn đang xử lý.

### Bước 6 — Drop Pool

Tạo:

```text
Gameplay/Pickup/PooledDrop.cs
Gameplay/Pickup/DropPool.cs
```

Flow:

```text
GetDrop(position)
→ ResetForReuse
→ đặt position
→ Active
→ ReleaseDrop
→ inactive + parent về DropPool
```

Reset đã test:
- Kill tween;
- scale gốc;
- rotation sạch.

Task này **chưa nối Drop vào Coin/Heart/Item hoặc LootTable** để không kéo Loot task lên sớm.

### Bước 7 — DamageText Pool

Tạo:

```text
UI/DamageText/DamageText.cs
UI/DamageText/DamageTextPool.cs
```

DamageText dùng `TextMeshPro` World Space, không dùng `TextMeshProUGUI` Screen Space Canvas.

Animation:

```text
DOTween.Sequence
├── DOMove lên trên
└── DOFade alpha → 0
        ↓
OnComplete
        ↓
ReturnToPool
```

Reset:
- Kill Sequence/tween cũ;
- scale gốc;
- màu/alpha gốc.

Vì là World Space nên sau này muốn hiển thị trên đầu Enemy chỉ cần dùng world position của Enemy + offset; không cần World-to-Screen conversion.

### Play Test / Profiler đã xác nhận

```text
✓ Enemy Room A chết → Release → Room B Get lại cùng object.
✓ Khi Pool còn Enemy, Room tiếp theo không Create thêm object không cần thiết.
✓ Enemy reuse có đầy MaxHealth.
✓ EnemyAI Chase lại bình thường.
✓ Contact Attack / cooldown hoạt động sau reuse.
✓ Hit feedback reset đúng.
✓ Camera shake vẫn chạy sau reuse.
✓ aliveEnemyCount giảm đúng và Door chỉ Unlock khi Enemy cuối chết.
✓ Không duplicate OnDeath subscription.
✓ Drop Get/Release/Get reuse cùng instance.
✓ Drop reset scale/state đúng.
✓ DamageText bay lên + fade + auto Release.
✓ DamageText Get lại cùng instance và alpha/color reset đúng.
✓ Console không lỗi đỏ trong regression cuối.
✓ Profiler không xuất hiện spike Instantiate/Destroy lớn do Enemy combat sau khi pool đã có object.
```

### Kết luận

```text
Pool
→ lifecycle/object reuse

RoomController
→ combat lifecycle

Enemy
→ health/AI/gameplay

Loot Data (task sau)
→ quyết định drop gì
```

**Tuần 5 - Thứ 4 DONE.**

Task tiếp theo:

```text
Tuần 5 - Thứ 5
Loot Data
```


## Tuần 5 — Thứ 5: Loot Data
**DONE — người dùng đã Play Test weighted loot + item pool và xác nhận.**

### Mục tiêu

Tách quyết định reward khỏi Enemy/Room gameplay và đưa tỉ lệ drop vào asset.

Thiết kế cuối được chốt thành hai nhánh độc lập:

```text
Pickup nhỏ
→ RoomClearLootTable
→ None / Coin / Heart / Key / Bomb
→ Weight

Item lớn
→ ItemPoolData theo loại phòng
→ Reward / Boss / Angel / Devil... pool
→ Weight giữa các ItemData trong đúng pool
```

### Pickup nhỏ — LootTable

Tạo:
- `LootRewardType`: `None`, `Coin`, `Heart`, `Key`, `Bomb`.
- `LootEntry`: `RewardType`, `Weight`, `Amount`.
- `LootTable`: ScriptableObject với `RollLoot()` weighted random.
- `RoomClearLootTable.asset`.
- `RoomLootRoller`: nghe `RoomController.OnRoomCleared`, gọi `LootTable.RollLoot()` và phát `OnLootRolled`.

Weighted roll đã test với cấu hình chia đều và cấu hình Coin Weight cao hơn; chỉ sửa asset đã làm phân bố thay đổi rõ rệt.

`None` là một kết quả roll hợp lệ và về sau có nghĩa không spawn pickup.

### Item lớn — ItemData + Item Pool

Tạo:
- `ItemData`: identity/visual cơ bản của một Item.
- `ItemPoolEntry`: `ItemData + Weight`.
- `ItemPoolData`: ScriptableObject với `RollItem()` weighted random.
- `RewardItemPool.asset`, `BossItemPool.asset` và các ItemData test.
- `RoomContext.ItemPoolData` để Room Template giữ pool phù hợp.
- `ItemRewardRoller`: gọi đúng `RoomContext.ItemPoolData.RollItem()` và phát `OnItemRolled`.

Quy tắc đã chốt:
- Item lớn không random chung với Coin/Heart/Key/Bomb.
- Reward Room dùng Reward Item Pool.
- Boss Room dùng Boss Item Pool.
- Angel/Devil/Special Room về sau dùng pool riêng theo cùng contract.
- Một `ItemData` có thể xuất hiện ở nhiều Item Pool với Weight khác nhau.
- Weight nằm ở `ItemPoolEntry`, không nằm trong `ItemData`.
- Một Room chỉ roll Item reward một lần.

### Regression đã xác nhận

```text
✓ Room Clear roll pickup nhỏ đúng một lần.
✓ Đi lại Room đã clear không roll lại.
✓ Đổi Weight RoomClearLootTable không cần sửa code.
✓ Reward Room chỉ roll item thuộc RewardItemPool.
✓ Boss Room chỉ roll item thuộc BossItemPool.
✓ Đổi Weight ItemPoolData làm phân bố item thay đổi.
✓ Cùng ItemData có thể có Weight khác nhau giữa các pool.
✓ ItemRewardRoller không roll lần hai trong cùng Room.
✓ Start/Combat Room không giữ ItemPoolData lớn.
✓ Console sạch trong test cuối.
```

### Ranh giới task

Chưa làm:
- spawn Coin/Heart/Key/Bomb ngoài world;
- Player nhặt pickup và cập nhật resource/health;
- DOTween pop/hút pickup;
- Item pedestal/world pickup;
- Item stat effects;
- FloorExit/NextFloor.

Các phần này thuộc task kế tiếp hoặc roadmap sau.

**Tuần 5 - Thứ 5 DONE.**

Task tiếp theo:

```text
Tuần 5 - Thứ 6
Loot & Floor Flow
```

---


## Tuần 5 — Thứ 6: Loot & Floor Flow
**DONE theo phạm vi người dùng đã chốt — đã Play Test toàn bộ phần được ưu tiên.**

### Scope đã chốt trước khi code

Roadmap gốc gộp pickup, DOTween visual và NextFloor trong cùng ngày. Người dùng chủ động chia nhỏ và chọn làm:

```text
1. Small Pickup spawn ngoài world
2. Player nhặt Coin / Heart / Key / Bomb
4. Item lớn spawn pedestal
5. FloorExit cơ bản
```

Chủ động hoãn:

```text
3. DOTween pop / hút pickup
→ visual polish, chưa ưu tiên

6. NextFloor hoàn chỉnh / floor index / progression
→ để nối với RunProgress / Boss Flow ở Tuần 7
```

Vì vậy task được đánh dấu DONE theo **scope đã thống nhất**, không được hiểu là DOTween pickup hoặc NextFloor đã hoàn thành.

### 1. Small Pickup spawn ngoài world

`RoomLootRoller` giữ đúng trách nhiệm roll dữ liệu; không trực tiếp spawn object.

Flow:

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
DropPool.GetDrop()
        ↓
PooledDrop.Configure(lootEntry)
```

`PooledDrop` được mở rộng để giữ runtime data:

```text
RewardType
RewardAmount
Sprite theo Coin / Heart / Key / Bomb
```

`None` là kết quả hợp lệ:

```text
RewardType.None
→ không gọi DropPool
→ không spawn object
```

`EdgarDungeonPostProcessing` inject `DropPool` của Scene cho `RoomLootDropSpawner` runtime bằng `SetDropPool()`, theo đúng pattern đã dùng cho `EnemyPool`.

### 2. Player nhặt Pickup

Tạo `PooledDropPickup` để xử lý trigger, không nhét resource logic vào `DropPool`.

Flow:

```text
Player chạm PooledDrop
        ↓
PooledDropPickup
        ↓
RewardType?
├── Coin → PlayerResources.AddCoins(amount)
├── Key  → PlayerResources.AddKeys(amount)
├── Bomb → PlayerResources.AddBombs(amount)
└── Heart → PlayerHealth.TryHeal(amount)
        ↓
reward áp dụng thành công
        ↓
PooledDrop.ReturnToPool()
```

Bổ sung `PlayerHealth.TryHeal(int healAmount)` với contract:

```text
Player chết → false
healAmount <= 0 → false
Full HP → false
Thiếu HP → cộng HP, clamp MaxHealth, phát OnHealthChanged, true
```

Quyết định gameplay quan trọng:
- Heart khi full HP **không bị mất**.
- Heart chỉ ReturnToPool nếu `TryHeal()` trả `true`.
- HUD Heart tự cập nhật qua `PlayerHealth.OnHealthChanged`, pickup không reference HUD.
- `Amount > 1` đã test truyền đúng từ `LootEntry → PooledDrop → PlayerResources/PlayerHealth`.
- Giai đoạn hiện tại dùng **một Collider2D chung** cho mọi small pickup; collider riêng theo sprite chỉ cân nhắc nếu play test sau này thấy cần.

### 3. Item lớn spawn pedestal

Tạo:
- `ItemPedestal` — giữ `ItemData` và hiển thị Icon.
- `RoomItemRewardSpawner` — điều phối thời điểm roll/spawn pedestal.

Reward Room:

```text
Room runtime setup xong
→ InitializeRoomItemReward()
→ ItemRewardRoller.RollItemReward()
→ RewardItemPool
→ ItemData
→ ItemPedestal
```

Boss Room:

```text
Boss Room setup
→ chưa spawn Item

Boss clear
→ RoomController.OnRoomCleared
→ RoomItemRewardSpawner
→ ItemRewardRoller
→ BossItemPool
→ ItemPedestal
```

Item Effects/stat modifier vẫn **chưa làm**; đúng roadmap Tuần 6 - Thứ 6.

### 4. FloorExit cơ bản

Tạo:
- `FloorExit` — trigger phát `OnPlayerEnteredExit`.
- `FloorExitSpawner` — nghe Boss Room clear và spawn Exit một lần.

Flow:

```text
Boss chưa clear
→ chưa có FloorExit

Boss clear
→ FloorExitSpawner
→ spawn FloorExit

Player chạm FloorExit
→ OnPlayerEnteredExit
→ hiện mới phát tín hiệu/log
→ chưa NextFloor
```

`FloorExit.prefab` vẫn giữ `Collider2D + FloorExit.cs`; marker vị trí không thay thế gameplay prefab.

### 5. Thay đổi kiến trúc lớn — Gameplay Marker Tilemap

Ban đầu Item/Pickup/FloorExit dùng Empty Transform marker:

```text
RewardSpawnPoint
FloorExitSpawnPoint
```

Sau khi trao đổi và Play Test, flow đã chuyển sang **Tile marker** để vị trí luôn khớp Grid/Tilemap/Edgar.

Hierarchy logic:

```text
GameplayMarkers GameObject
├── Tilemap
├── TilemapRenderer
└── RoomMarkerTilemap
```

Marker là `TileBase` asset được paint vào `GameplayMarkers`, **không phải component và không phải child GameObject**.

Marker hiện có:

```text
SmallPickupMarkerTile
ItemRewardMarkerTile
FloorExitMarkerTile
```

`RoomMarkerTilemap` dùng helper chung:

```text
TryGetMarkerWorldPosition(markerTile, out worldPosition)
        ↓
find TileBase trong cellBounds
        ↓
Tilemap.GetCellCenterWorld(cell)
        ↓
World Position chính giữa cell
```

Các flow sau migration:

```text
Small Pickup
→ SmallPickupMarkerTile
→ RoomLootDropSpawner
→ PooledDrop

Item lớn
→ ItemRewardMarkerTile
→ RoomItemRewardSpawner
→ ItemPedestal

FloorExit
→ FloorExitMarkerTile
→ FloorExitSpawner
→ FloorExit prefab
```

Lợi ích đã xác nhận:
- Vị trí spawn khớp đúng cell, không lệch do Empty Transform đặt tay.
- Edgar di chuyển Room runtime thì marker đi cùng Tilemap của Room.
- Regenerate dungeon vẫn lấy đúng marker của Room mới.
- Tile marker chỉ giữ **vị trí**; gameplay object vẫn giữ Collider/script/interaction.

Sau migration và regression thành công:
- `RewardSpawnPoint` đã được loại khỏi current flow và cleanup khỏi `RoomContext` / Edgar collect logic / Room Template.
- `FloorExitSpawnPoint` đã được loại khỏi current flow tương tự.
- `RewardSpawnPoint.cs` và `FloorExitSpawnPoint.cs` không còn thuộc current architecture.
- `EnemySpawnPoint` vẫn dùng GameObject marker; người dùng muốn cân nhắc chuyển Enemy spawn sang Marker Tile **về sau**, chưa làm trong task này.

### 6. Vì sao chưa có `BossRewardManager`

Hiện Boss Clear mới có hai listener độc lập:

```text
RoomController.OnRoomCleared
├── RoomItemRewardSpawner
└── FloorExitSpawner
```

Chưa tạo `BossRewardManager` vì chưa có chuỗi nhiều bước phụ thuộc thứ tự. Chỉ cân nhắc Coordinator/Manager khi Boss Clear cần orchestration như delay, camera focus, input lock, chest, victory VFX/SFX, spawn reward rồi mở Exit theo sequence.

### Regression cuối đã xác nhận

```text
✓ Combat Room Clear spawn small pickup đúng SmallPickupMarkerTile.
✓ None không spawn object.
✓ Coin/Key/Bomb cập nhật PlayerResources và ReturnToPool.
✓ Heart thiếu HP hồi đúng + HUD cập nhật + ReturnToPool.
✓ Heart full HP không bị tiêu thụ.
✓ Amount > 1 hoạt động đúng.
✓ Drop reuse qua DropPool không giữ reward state cũ.
✓ Reward Room spawn đúng một Item Pedestal từ RewardItemPool.
✓ Boss trước clear chưa có Item/Exit.
✓ Boss clear spawn đúng Item Pedestal từ BossItemPool.
✓ Boss clear spawn đúng FloorExit ở FloorExitMarkerTile.
✓ Item Pedestal và FloorExit dùng marker khác nhau, không chồng vị trí.
✓ Player chạm FloorExit phát signal/log đúng một lần.
✓ Regenerate Edgar vẫn đọc đúng cell marker của Room runtime mới.
✓ Cleanup RewardSpawnPoint/FloorExitSpawnPoint không gây Missing Script/NullReference.
✓ Console sạch trong regression cuối.
```

### Deferred có chủ đích

```text
DOTween pop / hút pickup
→ chưa làm

Player nhặt Item lớn / Item Effects
→ Tuần 6 - Thứ 6

NextFloor hoàn chỉnh / floor index / RunProgress
→ Tuần 7, nối với RunProgress / Boss Flow

Enemy Spawn Marker Tile
→ hướng cải tiến về sau, chưa refactor
```

**Tuần 5 - Thứ 6 DONE theo scope đã chốt.**

Task tiếp theo:

```text
Tuần 6 - Thứ 2
Enemy Architecture
```

---

# 17. Debug quan trọng đã gặp ở SceneLoader / DOTween

Đã từng gặp:
- Fade Out không nhìn thấy.
- `SetUpdate(true)` từng gây hành vi tween không như mong đợi trong một tình huống.
- Fade In từng chỉ lên alpha khoảng 0.04 rồi dừng.
- Đã debug bằng Console và xác nhận chỉ gọi Show một lần.
- Cuối cùng đã sửa và test SceneLoader/LoadingScreen chạy thành công.

Quy tắc:
- Không tự sửa lại SceneLoader nếu chưa có bug mới.
- Nếu tween pause/timeScale gặp vấn đề ở Tuần 9, đọc lại logic update mode trước.
- Khi debug phải xem Console/log và xác định callback nào chạy trước khi sửa.

---

# 26. Checklist đã test thành công đến hiện tại

## Core
- [x] Bootstrap chạy đầu game.
- [x] GameManager tồn tại qua scene.
- [x] SceneLoader tồn tại qua scene.
- [x] Không duplicate manager.
- [x] Async load hoạt động.
- [x] Loading Screen hoạt động.
- [x] Fade In/Out hoạt động.

## Input
- [x] WASD.
- [x] Shoot Arrow Keys.
- [x] Move/Shoot độc lập.
- [x] Dash Space.

## Player
- [x] Di chuyển 4 hướng.
- [x] Di chuyển chéo.
- [x] MoveSpeed lấy từ PlayerData.
- [x] Dash 4 hướng.
- [x] Dash chéo.
- [x] Dash cooldown.
- [x] Không Dash khi đứng yên.
- [x] DOPunchScale Visual.
- [x] Root Player không bị scale.

## Shooting
- [x] Shoot direction đúng.
- [x] FireInterval hoạt động.
- [x] Spawn offset đúng hướng.
- [x] Projectile bay bằng Rigidbody2D.
- [x] Lifetime hoạt động.

## Pooling
- [x] Projectile Get từ Pool.
- [x] Projectile Release về Pool.
- [x] Projectile được tái sử dụng.
- [x] Số clone không tăng vô hạn khi bắn lâu.
- [x] ownerPool trả Projectile đúng Pool.

## Collision
- [x] Environment collision → ReturnToPool.
- [x] Layer ngoài CollisionLayers → ignore.
- [x] Projectile không bị Destroy khi hết vòng đời bình thường.
- [x] Enemy collision → damage đúng 1 lần rồi ReturnToPool.

## Enemy Combat Foundation
- [x] Enemy Dummy + Layer Enemy.
- [x] EnemyData ScriptableObject.
- [x] EnemyHealth lấy MaxHealth từ EnemyData.
- [x] TakeDamage hoạt động.
- [x] HP không xuống dưới 0.
- [x] OnDamaged phát đúng.
- [x] OnDeath chỉ phát 1 lần.
- [x] Hit flash bằng SpriteRenderer.DOColor().
- [x] DOKill/reset màu tránh tween chồng.
- [x] Visual feedback không đổi logic gameplay.

## Player Health
- [x] PlayerData có MaxHealth.
- [x] PlayerData có InvincibilityDuration.
- [x] PlayerHealth khởi tạo currentHealth đúng.
- [x] Contact DamageSource gây đúng 1 damage mỗi hit hợp lệ.
- [x] Invincibility window 0.5 giây hoạt động.
- [x] Đứng chạm Enemy liên tục không tụt HP mỗi physics frame.
- [x] OnHealthChanged phát đúng current/max health.
- [x] OnDeath chỉ phát 1 lần.
- [x] Health event không phụ thuộc HUD.


## HUD UI
- [x] HUD Canvas + HUDRoot.
- [x] `HUDPresenter` / `HUDView` tối giản.
- [x] HP nhận dữ liệu qua `PlayerHealth.OnHealthChanged`.
- [x] Không poll HP trong `Update()`.
- [x] HP hiển thị Full / Half / Empty Heart đúng.
- [x] Quy ước 1 HP = nửa Heart.
- [x] Heart tự sinh từ `HeartUI.prefab` theo `maxHealth`.
- [x] Không cần gắn cứng `Image[]` Heart trong Inspector.
- [x] `HUDView` không đọc trực tiếp `PlayerData`.
- [x] Heart thay đổi dùng `DOPunchScale()` và không ảnh hưởng gameplay.
- [x] Coin test cập nhật qua `TestCurrencySource.OnCurrencyChanged`.
- [x] CoinText dùng `DOPunchScale()` khi giá trị thay đổi.
- [x] Run Currency thật chưa được tạo sớm; giữ đúng roadmap.
- [x] Đã test các trường hợp Heart/MaxHealth cần thiết.
- [x] Console không có lỗi đỏ sau test HUD.


## Enemy AI cơ bản
- [x] `EnemyData` có MoveSpeed / AttackDamage / AttackCooldown.
- [x] Player ngoài detection → Enemy đứng yên.
- [x] Player vào detection → Enemy Chase.
- [x] MoveSpeed trong EnemyData thay đổi tốc độ Chase.
- [x] Contact Player → Enemy dừng Chase và bắt đầu telegraph.
- [x] `DOPunchScale()` chỉ tác động child Visual.
- [x] Hết telegraph và còn contact → Player nhận đúng AttackDamage.
- [x] Player rời contact trong telegraph → không nhận damage.
- [x] AttackCooldown hoạt động, không damage mỗi physics frame.
- [x] Rời contact nhưng còn detection → Enemy Chase lại.
- [x] Enemy chết khi Chase → dừng Chase.
- [x] Enemy chết giữa telegraph → coroutine/tween bị hủy và không gây damage.
- [x] Đứng chạm Enemy đã chết → không nhận damage.
- [x] `ChasePlayer()` có guard `isDead` riêng.
- [x] Console không có lỗi đỏ sau test Enemy AI.

## Room Combat
- [x] `RoomController` tìm được cả Enemy inactive trong `Enemies` container.
- [x] Player vào `RoomTrigger` → `StartCombat()` chỉ bắt đầu một lần.
- [x] Enter → Lock Door.
- [x] Enemy inactive được Activate khi combat bắt đầu.
- [x] `EnemyHealth.OnDeath` giảm `AliveEnemyCount` đúng.
- [x] Enemy chưa chết hết → Door vẫn Lock.
- [x] Enemy cuối chết → Room Clear → Door Unlock.
- [x] `OnRoomCleared` chỉ phát một lần.
- [x] Đi ra/vào lại room đã clear không khóa cửa lại.
- [x] Room không có Enemy clear ngay và không nhốt Player.
- [x] Console không có lỗi đỏ sau test Room Combat.

## Camera - Cinemachine
- [x] Cinemachine 3.1.7 đã cài.
- [x] Main Camera có Cinemachine Brain.
- [x] PlayerCamera Tracking Target = root Player.
- [x] Camera Orthographic giữ góc nhìn 2D.
- [x] Position Composer follow Player ổn định.
- [x] Damping X/Y khoảng 0.3 đã test.
- [x] Dead Zone Width/Height khoảng 0.15 đã test.
- [x] Dash không làm camera rung/nhảy bất thường do Player Visual tween.
- [x] Cinemachine Impulse Source/Listener hoạt động.
- [x] `CameraController.ShakeCamera()` là lớp giao tiếp với Cinemachine.
- [x] Enemy death gọi shake qua `EnemyDeathCameraShake`, không gọi Cinemachine trong `EnemyHealth`.
- [x] Shake xong camera trở lại follow bình thường.
- [x] Camera shake không làm Player dịch chuyển.
- [x] Console không có lỗi đỏ sau test Camera.

## Vertical Slice
- [x] `PlayerResources` giữ Coin/Key/Bomb runtime tối thiểu.
- [x] HUD Coin nhận `PlayerResources.OnCoinsChanged`.
- [x] `RoomReward` cộng Coin khi `RoomCleared`.
- [x] Reward không phát lại khi vào lại room đã clear.
- [x] Play lại reset resource runtime về 0.
- [x] Full loop Enter → Lock → Fight → Clear → Unlock → Reward hoạt động.
- [x] HUD Health/Coin, Camera, Enemy AI và Room event không regression.
- [x] Console không có lỗi đỏ sau regression Vertical Slice.


## Edgar - Graph & Floor Data
- [x] `RoomType` đã có Start/Combat/Reward/Shop/Boss.
- [x] `FloorData` giữ RoomCount, Difficulty, RoomTypes, seed.
- [x] `Floor_01_Data` cấu hình 6 Room.
- [x] `Floor_01_LevelGraph` có Start → Boss hợp lệ + Reward branch.
- [x] `FloorEdgarConfig` là điểm ánh xạ tập trung FloorData ↔ LevelGraph.
- [x] Validation RoomCount/Start/Boss đã test.
- [x] Generate graph nhiều lần không overlap, Console sạch.

## Edgar - Runtime Integration
- [x] `EdgarDungeonGenerator` áp Floor config và gọi Generate runtime.
- [x] Edgar generator dùng Manual generation để tránh double generate.
- [x] Post Processing phân biệt gameplay Room và Corridor.
- [x] Mỗi gameplay Room nhận RoomId + RoomType + LayoutPosition.
- [x] RoomDoor runtime được tạo đúng Door Edgar sử dụng.
- [x] `RoomController.SetRoomDoors()` nhận đúng Door của chính Room.
- [x] SpawnPoint được thu thập đúng Start/Combat/Reward/Boss.
- [x] Regenerate nhiều seed liên tiếp không duplicate runtime Door/context.

## Minimap từ Edgar Data
- [x] Minimap dùng `RoomContext`, không gọi Edgar API trực tiếp.
- [x] Room icon lấy vị trí từ `LayoutPosition` và `minimapScale`.
- [x] Start Room dùng làm minimap origin.
- [x] Current / Visited / Unvisited state đã có logic.
- [x] `ConnectedRooms[]` chỉ chứa gameplay Room, không chứa Corridor.
- [x] Connection UI chống duplicate bằng RoomId ordering.
- [x] Floor 01 có 6 icon và 5 connection line.
- [x] `ClearMinimap()` dọn icon/connection/reference cũ khi regenerate.
- [x] Bug icon dồn tâm do thiếu gán `anchoredPosition = minimapPosition` đã xác định và sửa.



## Room Lifecycle + Edgar
- [x] `RoomController` phân biệt Combat/Boss với Start/Reward/Shop qua `RoomContext.RoomType`.
- [x] Combat/Boss Room spawn Enemy runtime từ `EnemySpawnPoints[]`.
- [x] CombatRoom cũ có Enemy sẵn không bị spawn trùng.
- [x] Enemy runtime được parent vào đúng `Enemies` container.
- [x] `aliveEnemyCount` khởi tạo đúng sau spawn.
- [x] Enemy inactive trước combat và được Activate khi `StartCombat()` chạy.
- [x] `EnemyHealth.OnDeath` giảm `aliveEnemyCount` đúng.
- [x] Enemy cuối chết → Room Clear.
- [x] Clear → `isRoomActive = false`, `isRoomCleared = true`.
- [x] Clear → chỉ Unlock Door của chính Room.
- [x] `OnRoomCleared` chỉ phát một lần.
- [x] `StartCombat()` sau Clear không spawn/lock lại.
- [x] Room A không Lock/Unlock nhầm Door Room B.
- [x] `RoomReward` không phải dependency của Room Edgar; vẫn chỉ là helper test Vertical Slice cũ.
- [x] Console sạch trong test cuối.

## Room Transition + Cinemachine
- [x] `RoomTransitionController` giữ Current Room runtime.
- [x] `RoomTrigger` báo đúng Room mới khi Player entry.
- [x] Start Room được xác định bằng `RoomType.Start`, không dựa RoomId.
- [x] Player spawn đúng `PlayerSpawnPoint` của Start Room.
- [x] Normal Room Camera frame theo `CameraAnchor`, không follow Player tự do.
- [x] `RoomCameraTarget` là Tracking Target của Cinemachine.
- [x] Camera đổi đúng khi Player chạm trigger của Room mới.
- [x] Minimap Current/Visited đổi đồng bộ với Current Room.
- [x] Start → Combat → Clear → Start hoạt động hai chiều.
- [x] Reward/Boss/nhiều Room không làm Camera lệch hoặc drift.
- [x] Không dùng DOTween để tranh quyền transform với Cinemachine.
- [x] Enemy runtime nhận `Player Transform` và `CameraController` sau spawn.
- [x] Enemy Chase và EnemyDeathCameraShake vẫn hoạt động sau runtime injection.
- [x] Console không có lỗi gameplay trong regression cuối.

## Object Pooling mở rộng
- [x] `EnemyPool` dùng `ObjectPool<EnemyHealth>`.
- [x] `EnemyHealth.ResetForReuse()` hồi MaxHealth + reset death state.
- [x] `EnemyAI.ResetForReuse()` reset AI/cooldown/coroutine/Rigidbody/tween.
- [x] `EnemyHealth.OnDeath` truyền đúng Enemy vừa chết.
- [x] `RoomController` lấy Enemy runtime từ pool thay vì Instantiate từng Room.
- [x] Enemy runtime vẫn nhận Player Transform + CameraController sau Get.
- [x] Enemy được Release về pool sau death lifecycle.
- [x] Enemy reuse không duplicate event và không làm sai aliveEnemyCount.
- [x] `DropPool` / `PooledDrop` Get-Release-reuse đúng.
- [x] Drop reset scale/rotation/tween đúng.
- [x] `DamageTextPool` / `DamageText` Get-Release-reuse đúng.
- [x] DamageText World Space dùng DOTween move + fade và tự Release.
- [x] DamageText reset alpha/color/scale/tween khi reuse.
- [x] Profiler không còn spike Instantiate/Destroy lớn do Enemy combat sau warm-up/reuse.
- [x] Console sạch trong regression cuối Object Pooling.

## Loot & Floor Flow
- [x] `RoomLootDropSpawner` nhận `OnLootRolled` và spawn từ `DropPool`.
- [x] `PooledDrop` giữ RewardType / RewardAmount và đổi sprite theo reward.
- [x] `PooledDropPickup` xử lý Coin / Heart / Key / Bomb.
- [x] Coin/Key/Bomb cập nhật `PlayerResources` đúng.
- [x] `PlayerHealth.TryHeal()` không tiêu thụ Heart khi full HP.
- [x] Pickup nhận thành công → ReturnToPool.
- [x] Item lớn Reward/Boss dùng đúng Item Pool và spawn một Pedestal.
- [x] Boss clear mới spawn Item + FloorExit.
- [x] `GameplayMarkers` Tilemap dùng `SmallPickupMarkerTile` / `ItemRewardMarkerTile` / `FloorExitMarkerTile`.
- [x] `RoomMarkerTilemap` đổi Cell → World bằng `GetCellCenterWorld()`.
- [x] RewardSpawnPoint/FloorExitSpawnPoint cũ đã cleanup khỏi current flow.
- [x] Regenerate Edgar không làm lệch marker/spawn position.
- [x] FloorExit trigger phát tín hiệu đúng một lần.
- [x] DOTween pickup visual và NextFloor hoàn chỉnh được ghi rõ là deferred, không giả định đã làm.
- [x] Console sạch trong regression cuối Tuần 5 - Thứ 6.


## Tuần 6 — Thứ 2: Enemy Architecture — DONE

**Người dùng đã Play Test và xác nhận toàn bộ regression sau refactor.**

### Flow architecture sau refactor

```text
EnemyAI
├── Detection
├── State Machine
│   ├── Idle
│   ├── Chase
│   └── Attack
└── gọi Attack Behaviour
            ↓
EnemyAttackBehaviour (abstract MonoBehaviour)
├── MeleeEnemyAttack
└── RangedEnemyAttack
```

### Thay đổi chính
- `EnemyAI` giữ detection, State Machine, Chase, death/reset pooling và runtime Player reference.
- Logic Attack cụ thể được tách khỏi `EnemyAI`.
- Tạo `EnemyAttackBehaviour` dạng `abstract MonoBehaviour` với contract:
  - `CanAttack(Transform playerTransform)`
  - `UpdateAttack(Transform playerTransform)`
  - `ResetAttack()`
- Tạo `MeleeEnemyAttack` và chuyển contact detection, cooldown, telegraph, damage, coroutine và visual reset sang component này.
- `EnemyAI` hỏi `attackBehaviour.CanAttack()` để quyết định Attack State và gọi `attackBehaviour.UpdateAttack()` khi đang Attack.
- `EnemyAI` gọi `ResetAttack()` khi Enemy chết/disable/reuse.
- `EnemyData` tiếp tục giữ stat; không đưa state/visual vào data.
- Chưa tạo `IEnemyMovement` / `IEnemyAttack`; abstract component đủ cho nhu cầu Unity Inspector hiện tại.
- Chưa cài A* vì chưa có bằng chứng direct chase thường xuyên kẹt trong dungeon.

### Regression đã test
```text
✓ Idle / Chase / Attack chuyển đúng.
✓ Melee contact attack vẫn hoạt động.
✓ Telegraph vẫn chỉ tác động child Visual.
✓ Player rời contact trong telegraph không nhận damage.
✓ Cooldown hoạt động.
✓ Enemy chết giữa telegraph không gây damage sau death.
✓ Enemy Pool reuse không giữ Attack state/cooldown/coroutine/tween cũ.
✓ EnemyHealth / RoomController / Door / Clear không regression.
✓ Console sạch.
```

---

## Tuần 6 — Thứ 3: Enemy Ranged — DONE

**Người dùng đã Play Test và xác nhận toàn bộ flow Ranged + pooling regression.**

### Flow

```text
EnemyAI
    ↓
RangedEnemyAttack.CanAttack()
    ↓
Player đã Detect + nằm trong Attack Range
    ↓
Attack State
    ↓
Telegraph trên Visual
    ↓
FirePoint + hướng Enemy → Player
    ↓
EnemyProjectilePool.GetProjectile()
    ↓
Projectile.Initialize()
    ↓
Player / Environment collision
    ↓
ReturnToPool
```

### Thay đổi chính
- Tạo `RangedEnemyAttack` kế thừa `EnemyAttackBehaviour`.
- Ranged dùng `attackRange` riêng với `EnemyAI.detectionRange`.
- Telegraph dùng DOTween trên child `Visual`; không tween Rigidbody2D root.
- Có `FirePoint` và `projectileSpawnOffset` để đặt projectile ngoài thân Enemy.
- Projectile lấy từ `ProjectilePool` và dùng lại object.
- `ProjectilePool` là Scene runtime reference, được cấp cho Ranged Enemy bằng setter/runtime injection thay vì kéo cứng Scene object vào prefab.
- Ranged projectile dùng `EnemyData.AttackDamage` làm nguồn damage và đã test thay đổi damage theo asset.
- `ResetAttack()` dọn coroutine/cooldown/tween để tương thích Enemy Pool.

### Regression đã test
```text
✓ Player ngoài Detection → Idle.
✓ Player trong Detection nhưng ngoài Attack Range → Chase.
✓ Player trong Attack Range → Attack.
✓ Telegraph xuất hiện trước khi bắn.
✓ Projectile bay đúng hướng tới Player.
✓ Projectile trúng Player → damage + ReturnToPool.
✓ Projectile trúng Environment → ReturnToPool.
✓ Cooldown hoạt động.
✓ Enemy chết giữa telegraph → không bắn projectile sau death.
✓ Enemy Pool reuse → Enemy Ranged Attack lại bình thường.
✓ Projectile Pool reuse → không giữ state cũ.
✓ EnemyData.AttackDamage thay đổi → Ranged damage thay đổi tương ứng.
✓ Console sạch.
```

### Ghi chú kiến trúc
`Enemy Ranged` là implementation thứ hai chứng minh việc tách Attack thành component là có nhu cầu thực tế. Tuy nhiên project **chưa thêm `IEnemyAttack` interface riêng**; `abstract EnemyAttackBehaviour : MonoBehaviour` đang vừa làm contract vừa giữ khả năng kéo component trong Unity Inspector.

---

## Trạng thái sau Tuần 6 — Thứ 3

```text
27 / 50 task DONE

Task hiện tại:
Tuần 6 - Thứ 4 — Damage System

DamageInfo / DamageCalculator
→ chưa triển khai
→ bước đầu tiên là review damage flow hiện tại trước khi sửa code.
```

## Enemy Architecture + Ranged
- [x] Enemy State Machine tối thiểu Idle / Chase / Attack.
- [x] `EnemyAttackBehaviour` abstract component.
- [x] `MeleeEnemyAttack` tách khỏi `EnemyAI`.
- [x] `RangedEnemyAttack` dùng chung Attack Behaviour.
- [x] Ranged telegraph trên Visual.
- [x] Ranged projectile dùng Projectile Pool.
- [x] Runtime injection cho Scene Projectile Pool.
- [x] Enemy/Projectile pooling regression.
- [x] Ranged damage lấy theo `EnemyData.AttackDamage`.
- [x] Console sạch sau test cuối.

## Debug / Regression
- [x] Stop Play Mode không còn MissingReferenceException tại ProjectilePool.OnDestroyProjectile.
- [x] Không có lỗi đỏ Console sau test Combat Foundation + Player Health.
- [x] Không có lỗi đỏ Console sau test HUD Heart/Coin.
- [x] Enemy chết không còn Chase/Attack; attack đang telegraph được cancel an toàn.

---
## 2026-09-25 — Edgar Multiple LevelGraph Variants — DONE

Người dùng đã Play Test và xác nhận flow random LevelGraph hoạt động đúng.

### Thay đổi
- `FloorEdgarConfig` đổi từ một `LevelGraph` sang `List<LevelGraph>`.
- `FloorEdgarConfig` bỏ validation `FloorData.RoomCount` với Edgar Graph.
- Mỗi `LevelGraph` có thể có số room khác nhau; số room layout lấy từ `LevelGraph.Rooms.Count`.
- `EdgarDungeonGenerator` random chọn một graph trước `DungeonGeneratorGrid2D.Generate()`.
- Room Template tiếp tục được cấu hình thủ công trên từng graph node trong Edgar Graph Editor.
- Không sửa source Edgar và không thêm Manager/framework mới.

### Runtime flow
```text
FloorEdgarConfig.List<LevelGraph>
        ↓
Random chọn 1 LevelGraph
        ↓
FixedLevelGraphConfig.LevelGraph
        ↓
DungeonGeneratorGrid2D.Generate()
```

### Test
```text
✓ Nhiều LevelGraph cấu hình được trong FloorEdgarConfig.
✓ Random graph trước Generate hoạt động.
✓ Các graph không cần cùng số room.
✓ Console sạch trong test cuối.
```

### Ghi chú
Đây là điều chỉnh kiến trúc bổ sung cho Edgar Free, không phải task roadmap mới. Task hiện tại của roadmap vẫn là **Tuần 6 - Thứ 4 — Damage System**.

