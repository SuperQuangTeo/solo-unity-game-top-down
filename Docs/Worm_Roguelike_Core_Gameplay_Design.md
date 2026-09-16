# GAMEPLAY CORE DESIGN DOCUMENT

## Quy ước thuật ngữ

Trong tài liệu này, một số thuật ngữ game development được giữ bằng tiếng Anh để thuận tiện khi thiết kế và lập trình:

- **Biome**: khu vực sinh thái có môi trường, kẻ địch và đặc trưng gameplay riêng.
- **Mutated**: trạng thái đã bị biến đổi về hình dạng, sức mạnh hoặc hành vi.
- **Mutation**: sự biến đổi của Player hoặc Enemy.
- **Hazard**: mối nguy đến từ môi trường.
- **Build**: bộ sức mạnh hình thành từ các Item mà Player thu thập trong một run.
- **Item Pool**: tập hợp Item mà hệ thống có thể chọn để spawn.
- **Attack Pattern**: mẫu hoặc chuỗi hành vi tấn công của Enemy/Boss.
- **Lore**: cốt truyện nền và thông tin về thế giới game.
- **Route**: nhánh đường mà Player lựa chọn trong quá trình chơi.
- **Run**: một lượt chơi hoàn chỉnh từ lúc bắt đầu cho đến khi chết hoặc hoàn thành hành trình.

---

## 1. Tổng quan ý tưởng

### 1.1. Thể loại
- **Roguelike (thể loại chơi theo từng lượt run, có tính ngẫu nhiên và khả năng chơi lại cao) / Dungeon Crawler (thể loại tập trung vào khám phá và chiến đấu trong hầm ngục)**
- Góc nhìn: **Top-down (góc nhìn từ trên xuống) 2D**
- Cấu trúc gameplay theo **Room (phòng)** và **Floor (tầng chơi)**
- Hướng phát triển gameplay lấy cảm hứng từ *The Binding of Isaac*, nhưng có hệ thống thế giới, tiến trình và nhánh khám phá riêng.

### 1.2. Nhân vật chính
Người chơi điều khiển một **con giun đất** sinh sống dưới lòng đất.

Hành trình bắt đầu ở những lớp đất tương đối bình thường. Khi người chơi tiến sâu hơn, môi trường dần trở nên:
- Tối hơn.
- Nguy hiểm hơn.
- Dị dạng hơn.
- Xa lạ hơn.

Các sinh vật dưới lòng đất cũng dần bị biến đổi về hình dạng, hành vi và sức mạnh.

---

# 2. Core Gameplay Loop (vòng lặp gameplay cốt lõi)

Gameplay chính của một run (một lượt chơi hoàn chỉnh) được xây dựng theo vòng lặp:

1. Người chơi bắt đầu tại một **Floor**.
2. Khám phá các **Room** trong Floor.
3. Chiến đấu với kẻ địch trong từng Room.
4. Thu thập Pickup (vật phẩm nhặt nhanh trong màn chơi), Coin và các tài nguyên khác.
5. Tìm và hoàn thành **Reward Room (phòng nhận vật phẩm thưởng)** để nhận Item (vật phẩm nâng cấp) nâng cấp.
6. Khám phá các phòng đặc biệt nếu có.
7. Tiến đến **Boss Room (phòng Boss)**.
8. Đánh bại Boss (trùm).
9. Boss thả một **Boss Reward Item (vật phẩm thưởng sau khi hạ Boss)**.
10. Người chơi đi xuống Floor tiếp theo.
11. Độ khó tăng dần theo độ sâu.
12. Đến một mốc nhất định, người chơi phải lựa chọn **Branching Path (cơ chế lựa chọn nhánh đường)** để quyết định khu vực tiếp theo của run.

---

# 3. Hệ thống Floor

## 3.1. Khái niệm Floor

Mỗi **Floor** đại diện cho một tầng/lớp sinh thái khác nhau trong thế giới game.

Một Floor gồm nhiều Room được kết nối với nhau theo layout được sinh ngẫu nhiên hoặc bán ngẫu nhiên.

Ví dụ cấu trúc:

```text
Start Room
    |
Combat Room
    |
+---+---------+
|             |
Reward Room   Combat Room
              |
          Boss Room
```

Mỗi Floor sẽ có:
- Start Room.
- Nhiều Combat Room (phòng chiến đấu).
- Ít nhất một Reward Room.
- Boss Room.
- Có thể xuất hiện một số Room đặc biệt.

---

## 3.2. Tiến trình theo độ sâu

Độ sâu là yếu tố chính thể hiện tiến trình của game.

Càng đi xuống sâu:

- Ánh sáng môi trường giảm.
- Màu sắc chuyển dần sang tối và lạnh hơn.
- Layout Room có thể phức tạp hơn.
- Số lượng Enemy (kẻ địch) tăng.
- Enemy có nhiều Attack Pattern (mẫu hoặc kiểu tấn công) hơn.
- Enemy có thể xuất hiện Mutation (sự biến đổi).
- Elite Enemy (kẻ địch tinh anh, mạnh hơn kẻ địch thường) xuất hiện thường xuyên hơn.
- Boss có nhiều Phase hoặc Pattern phức tạp hơn.
- Hazard (mối nguy từ môi trường) trong Room xuất hiện nhiều hơn.

Ví dụ progression:

```text
Floor 1
Soft Soil

Floor 2
Wet Soil

Floor 3
Root Network

Floor 4
Ancient Underground

Floor 5
Branching Point

        /                \
Surface Route        Deep Earth Route
```

---

# 4. Hệ thống Room

## 4.1. Combat Room

Combat Room là loại Room xuất hiện thường xuyên nhất.

Khi Player (nhân vật do người chơi điều khiển) đi vào Room:

1. Cửa Room đóng lại.
2. Enemy được kích hoạt hoặc spawn.
3. Player phải tiêu diệt toàn bộ Enemy.
4. Khi Enemy cuối cùng bị tiêu diệt:
   - Room được đánh dấu Clear.
   - Cửa mở.
   - Có khả năng spawn Pickup.

Reward sau Combat Room có thể bao gồm:
- Heart.
- Coin.
- Bomb.
- Key.

---

## 4.2. Reward Room

Mỗi Floor có ít nhất **1 Reward Room**.

Reward Room chứa một **Item tăng sức mạnh lâu dài trong run hiện tại**.

Ví dụ:

```text
Reward Room
      |
Item Pedestal
      |
Random Passive / Active Item
```

Item có thể thay đổi:
- Damage.
- Attack Speed (tốc độ tấn công).
- Movement Speed (tốc độ di chuyển).
- Max HP.
- Projectile.
- Bomb.
- Khả năng đặc biệt.
- Hiệu ứng liên quan đến cơ thể con giun.

Người chơi chỉ cần khám phá được Reward Room để nhận Item.

Reward Room đóng vai trò là một nguồn nâng cấp ổn định cho mỗi Floor.

---

## 4.3. Boss Room

Boss Room thường nằm ở khu vực cuối của Floor.

Khi Player bước vào:

1. Cửa Boss Room đóng.
2. Boss xuất hiện.
3. Boss Fight bắt đầu.
4. Player đánh bại Boss.
5. Boss spawn một **Boss Reward Item**.
6. Lối đi xuống Floor tiếp theo được mở.

Boss Reward Item có thể mạnh hơn Item bình thường hoặc thiên về nâng cấp chỉ số chính.

Ví dụ:

```text
Boss Defeated
      ↓
Boss Reward Item
      ↓
Next Floor Entrance
```

---

# 5. Pickup System

Game sử dụng 4 loại Pickup chính.

## 5.1. Heart

Heart dùng để hồi máu.

Có thể spawn:
- Sau khi clear Room.
- Từ Enemy.
- Từ Chest.
- Từ Object có thể phá.

---

## 5.2. Coin

Coin là loại tiền chính trong một run.

Coin có thể dùng cho:
- Shop.
- Machine.
- Event Room.
- Một số Item hoặc Mechanic đặc biệt.

---

## 5.3. Bomb

Bomb là tài nguyên chiến đấu và khám phá.

Bomb có thể dùng để:
- Gây sát thương diện rộng.
- Phá Object.
- Phá một số loại Wall đặc biệt.
- Kích hoạt một số cơ chế trong Room.

---

## 5.4. Key

Key dùng để mở:
- Chest.
- Locked Room (phòng bị khóa).
- Treasure Room đặc biệt.
- Một số Door hoặc Event.

---

# 6. Item System

Item là yếu tố chính tạo Build (cách xây dựng bộ sức mạnh của nhân vật) cho Player trong mỗi run.

Item được chia thành hai nguồn chính:

## Reward Room Item

Item nhận từ Reward Room.

Mỗi Floor đảm bảo Player có cơ hội nhận ít nhất một nâng cấp.

---

## Boss Reward Item

Item spawn sau khi đánh bại Boss.

Boss Reward có thể thiên về:
- Damage.
- HP.
- Attack Speed.
- Movement Speed.
- Special Stat (chỉ số đặc biệt).

Boss Reward giúp Player đủ sức đối đầu với Floor tiếp theo.

---

# 7. Difficulty Scaling

Độ khó tăng dựa trên độ sâu của Floor.

Các yếu tố có thể tăng:

### Enemy
- HP.
- Damage.
- Movement Speed.
- Attack Speed.
- Số lượng Attack Pattern.

### Room
- Nhiều Enemy hơn.
- Nhiều Hazard hơn.
- Arena nhỏ hơn hoặc phức tạp hơn.

### Elite Enemy

Ở Floor sâu hơn có thể xuất hiện phiên bản Elite.

Ví dụ:

```text
Normal Enemy
    ↓
Mutated Enemy
    ↓
Elite Mutated Enemy
```

Elite Enemy có thể:
- Có Aura.
- Có Shield.
- Chia tách sau khi chết.
- Explode khi chết.
- Teleport.
- Tạo Poison Area.

---

# 8. Enemy Mutation

Mutation là một cơ chế quan trọng để thể hiện việc sinh vật bị biến đổi khi Player tiến sâu xuống lòng đất.

Ví dụ cùng một loại Enemy:

```text
Floor 1
Normal Ant

Floor 3
Armored Ant

Floor 5
Mutated Ant

Deep Earth
Fossil Ant
```

Mutation có thể thay đổi:
- Sprite.
- Size.
- HP.
- Damage.
- Movement.
- Attack Pattern.
- Special Ability (khả năng đặc biệt).

Nhờ đó Player vẫn có thể nhận ra loại Enemy cũ nhưng phải đối phó với phiên bản nguy hiểm hơn.

---

# 9. Branching Path System

Đây là một trong những cơ chế chính giúp game khác biệt.

Sau khi Player đạt tới một Floor nhất định, ví dụ:

```text
Floor 5
```

Player sẽ phải lựa chọn một trong hai hướng đi.

```text
                 Branching Point
                       |
          +------------+------------+
          |                         |
     Surface Path               Deep Earth Path
```

Lựa chọn này sẽ thay đổi:
- Environment.
- Enemy.
- Boss.
- Hazard.
- Item Pool (nhóm vật phẩm có thể được chọn để xuất hiện).
- Visual.
- Lore (cốt truyện nền và thông tin về thế giới game).
- Ending (kết thúc của game hoặc một nhánh cốt truyện) hoặc tiến trình sau đó.

---

# 10. Surface Path

Nếu Player lựa chọn đi lên phía trên mặt đất, môi trường game chuyển từ không gian ngầm sang hệ sinh thái trên mặt đất.

## Environment

Ví dụ:
- Grass.
- Mud.
- Tree Root Surface.
- Garden.
- Forest Floor.
- Human Area.

Môi trường sáng hơn nhưng Player sẽ phải đối mặt với những mối đe dọa hoàn toàn khác.

---

## Enemy

Các Enemy trên mặt đất có thể bao gồm:

- Kiến.
- Bọ cánh cứng.
- Nhện.
- Ong.
- Chim.
- Ếch.
- Các loài côn trùng săn mồi.

Enemy trên mặt đất có thể có đặc trưng:
- Di chuyển nhanh.
- Tấn công từ trên không.
- Jump Attack.
- Dive Attack.
- Long Range Attack.

---

## Human Entity

Một trong những thực thể nguy hiểm nhất trên Surface Route (nhánh đi lên mặt đất) có thể là **Con Người**.

Đối với Player là một con giun, con người giống như một sinh vật khổng lồ.

Player không nhất thiết chiến đấu trực tiếp với toàn bộ cơ thể con người.

Gameplay có thể sử dụng các đòn tấn công từ môi trường như:

- Bàn chân giẫm xuống.
- Xẻng đào đất.
- Thuốc trừ sâu.
- Máy cắt cỏ.
- Dòng nước tưới cây.

Con người có thể đóng vai trò:
- Boss.
- Environmental Boss (Boss hoạt động như một mối nguy của môi trường).
- Một thực thể tạo Hazard cho toàn bộ Floor.

---

# 11. Deep Earth Path

Nếu Player tiếp tục đào sâu xuống lòng đất, môi trường sẽ ngày càng phi tự nhiên.

Ví dụ:

```text
Deep Soil
↓
Ancient Rock
↓
Fossil Layer
↓
Magma Cavern
↓
Core Region
```

---

## Environment

Các khu vực có thể bao gồm:

- Hang đá cổ.
- Mỏ khoáng vật.
- Hang tinh thể.
- Fossil Cavern.
- Magma Cavern.
- Khu vực gần lõi Trái Đất.

---

## Enemy

Enemy ở Deep Earth có thể là:

- Sinh vật sống trong bóng tối.
- Sinh vật bị Mutation nặng.
- Sinh vật ký sinh.
- Sinh vật sống trong dung nham.
- Sinh vật cổ đại.
- Sinh vật hóa thạch sống lại.

Ví dụ:

```text
Fossil Worm
Fossil Beetle
Ancient Centipede
Magma Larva
Lava Parasite
```

---

## Fossil Enemy (kẻ địch hóa thạch)

Một số hóa thạch trong Room có thể sống lại khi Player tiếp cận.

Ví dụ:

```text
Player enters Room

Fossil appears inactive

Player approaches

Fossil cracks

Enemy awakens
```

Điều này giúp tạo Surprise Encounter (cuộc chạm trán bất ngờ) và tăng cảm giác nguy hiểm ở khu vực sâu.

---

# 12. Environment Progression

Visual của game nên thay đổi rõ rệt theo Floor.

Ví dụ:

```text
Soft Soil
Brown / Warm

Wet Soil
Dark Brown / Blue

Root Network
Brown / Green

Ancient Underground
Gray / Purple

Deep Earth
Dark Purple / Black

Magma
Red / Orange

Surface
Green / Yellow / Blue
```

Việc thay đổi:
- Tilemap (hệ thống bản đồ ô trong Unity).
- Lighting (hệ thống ánh sáng).
- Particle (hiệu ứng hạt).
- Background.
- Enemy design.
- Ambient Sound (âm thanh môi trường).

sẽ giúp Player cảm nhận rõ mình đang khám phá một hệ sinh thái mới.

---

# 13. Run Structure

Một run có thể được tổ chức như sau:

```text
Floor 1
Soft Soil
    ↓
Floor 2
Wet Soil
    ↓
Floor 3
Root Network
    ↓
Floor 4
Ancient Underground
    ↓
Floor 5
Branching Point

    ┌───────────────────┐
    │                   │
Surface Route      Deep Earth Route
    │                   │
Surface Floors      Fossil Floors
    │                   │
Human Zone          Magma Zone
    │                   │
Surface Boss        Core Boss
```

Hai Route (nhánh đường) có thể dẫn tới:
- Boss khác nhau.
- Ending khác nhau.
- Item khác nhau.
- Lore khác nhau.

---

# 14. Player Power Progression

Trong mỗi run, Player tăng sức mạnh chủ yếu thông qua Item.

Ví dụ tiến trình:

```text
Start
↓
Basic Worm
↓
Reward Item
↓
Boss Item
↓
Reward Item
↓
Boss Item
↓
Build Synergy
↓
Late Game Build
```

Mục tiêu là tạo cảm giác Player từ một con giun yếu trở thành một sinh vật ngày càng mạnh và dị thường.

Sự thay đổi của Player có thể phản ánh trực tiếp bằng Visual Mutation (biến đổi ngoại hình).

Ví dụ:

- Giun mọc gai.
- Giun có lớp giáp.
- Giun phát sáng.
- Giun mọc nhiều mắt.
- Giun có Acid.
- Giun có Fire Body.

---

# 15. Bổ sung đề xuất

Các ý trong phần này là đề xuất mở rộng, không thay đổi cơ chế cốt lõi phía trên.

## 15.1. Worm Mutation System

Item không chỉ tăng chỉ số mà còn có thể làm thay đổi cơ thể Player.

Ví dụ:

```text
Poison Item
→ Body becomes green
→ Attack inflicts poison
```

```text
Fire Item
→ Body glows red
→ Attack can burn enemies
```

```text
Armor Item
→ Worm gains shell segments
→ Increase defense
```

Điều này giúp Player nhìn vào nhân vật và nhận ra Build hiện tại.

---

## 15.2. Environmental Hazard

Mỗi biome (khu vực sinh thái có môi trường và kẻ địch đặc trưng) có Hazard riêng.

### Underground
- Rock Fall.
- Mud.
- Poison Gas.

### Surface
- Rain Drop.
- Bird Shadow.
- Human Footstep.

### Deep Earth
- Lava.
- Steam.
- Falling Rock.
- Magma Explosion.

Hazard giúp các biome khác nhau không chỉ về hình ảnh mà còn khác về gameplay.

---

## 15.3. Secret Room (phòng bí mật)

Một số Wall có thể bị phá bằng Bomb để mở Secret Room.

Secret Room có thể chứa:
- Coin.
- Rare Item.
- Chest.
- NPC.
- Lore.

Điều này tăng giá trị cho Bomb ngoài combat.

---

## 15.4. Route-exclusive Item (vật phẩm chỉ xuất hiện ở một nhánh đường)

Surface Route và Deep Earth Route (nhánh tiếp tục đi sâu xuống lòng đất) có thể có Item Pool riêng.

Ví dụ:

### Surface Item
- Feather.
- Leaf Armor.
- Bee Sting.
- Sunlight Seed.

### Deep Earth Item
- Fossil Bone.
- Magma Core.
- Crystal Heart.
- Ancient Parasite.

Nhờ đó mỗi Route sẽ tạo ra Build khác nhau.

---

## 15.5. Route-exclusive Boss (Boss chỉ xuất hiện ở một nhánh đường)

Hai Route nên có Boss riêng.

Ví dụ:

### Surface

```text
Bird
↓
Frog
↓
Human
```

### Deep Earth

```text
Ancient Worm
↓
Fossil Beast
↓
Magma Titan
```

Điều này tăng Replay Value (giá trị khiến người chơi muốn chơi lại) vì Player cần chơi nhiều run để khám phá toàn bộ nội dung.

---

# 16. Design Pillars (các trụ cột thiết kế)

Game nên xoay quanh 4 trụ cột chính:

### Exploration

Khám phá các lớp sinh thái khác nhau của Trái Đất.

### Build Variety (độ đa dạng trong cách xây dựng sức mạnh)

Mỗi run tạo ra một Build khác nhau thông qua Item.

### Biological Mutation

Player và Enemy đều trở nên ngày càng dị dạng.

### Branching Journey

Player quyết định hướng tiến hóa của hành trình:

```text
Go Up
Explore the Surface

or

Go Down
Discover the Deep Earth
```

Đây có thể trở thành cơ chế nhận diện chính của game.

---

# 17. Tóm tắt Core Identity (bản sắc cốt lõi của game)

Core identity của game có thể được mô tả ngắn gọn như sau:

> Một roguelike dungeon crawler top-down nơi người chơi điều khiển một con giun khám phá các tầng sinh thái của Trái Đất. Càng đi sâu, môi trường và sinh vật càng trở nên dị dạng và nguy hiểm. Trong mỗi run, người chơi thu thập Item để biến đổi cơ thể và xây dựng sức mạnh, đánh bại Boss của từng Floor và cuối cùng lựa chọn giữa việc tiến lên mặt đất hoặc tiếp tục đào sâu về phía lõi Trái Đất.