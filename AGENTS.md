# AGENTS.md — Worm Roguelike 2D Unity

## Mục đích

Đây là project Unity 2D Roguelike. Hãy làm việc theo roadmap hiện tại, giữ architecture đang có và ưu tiên code dễ hiểu cho người mới học lập trình.

File này chứa **quy tắc làm việc ổn định cho Codex**.  
Trạng thái task hiện tại không ghi ở đây mà lấy từ `Docs/PROJECT_HANDOFF.md`.

---

## 1. Nguồn thông tin của project

Luôn ưu tiên theo thứ tự sau:

1. `Docs/PROJECT_HANDOFF.md`
   - Nguồn đầu tiên phải đọc khi bắt đầu task.
   - Cho biết task hiện tại, task tiếp theo, scope, phần đã làm và phần chưa làm.

2. `Docs/PROJECT_ARCHITECTURE.md`
   - Dùng khi cần hiểu module, contract, folder, data flow hoặc quy tắc kỹ thuật.
   - Không đọc toàn bộ file nếu chỉ cần một section liên quan.

3. `Docs/PROJECT_HISTORY.md`
   - Chỉ dùng để tra bug cũ, quyết định cũ, regression test hoặc cách một module đã được triển khai trước đây.
   - Không đọc toàn bộ History mặc định.

4. Roadmap `.xlsx` trong `Docs/` có tên chứa `V4.1`
   - Đây là source of truth về thứ tự task.
   - Chỉ đọc khi cần đối chiếu roadmap, kiểm tra scope hoặc cập nhật tiến độ sau khi Play Test thành công.

5. `Docs/Worm_Roguelike_Core_Gameplay_Design_v2_Aligned_V4.1.md`
   - Chỉ đọc khi task liên quan gameplay design, progression, route/biome hoặc khi cần kiểm tra một mechanic có phù hợp định hướng game hay không.

Không tạo bản sao riêng kiểu `*_CODEX.md` cho Handoff, Architecture, History hoặc Roadmap.

---

## 2. Cách đọc code để tiết kiệm context

Khi bắt đầu task:

- Đọc `Docs/PROJECT_HANDOFF.md` trước.
- Lấy danh sách script liên quan từ Handoff nếu đã có.
- Bắt đầu từ các script trực tiếp liên quan đến task.
- Chỉ mở thêm script khi có dependency trực tiếp, event, method, field hoặc contract cần hiểu.
- Ưu tiên search tên class / method / event trước khi mở thêm nhiều file.
- Không scan toàn bộ `Assets/Scripts` chỉ để “hiểu project”.
- Không đọc toàn bộ các file tài liệu dài nếu section nhỏ đã đủ trả lời.

Không đọc hoặc sửa các folder sinh tự động của Unity trừ khi người dùng yêu cầu rõ:
`.vs/`, `Library/`, `Logs/`, `obj/`, `Temp/`.

Không sửa source plugin/package hoặc `GeneratedAssets/` nếu task không yêu cầu trực tiếp.

---

## 3. Roadmap và phạm vi task

- Bám đúng thứ tự task trong roadmap V4.1.
- Không tự nhảy sang task tiếp theo.
- Không kéo feature của tuần/ngày sau lên làm sớm.
- Không tự thêm hệ thống, Manager, interface hoặc abstraction chỉ vì “đẹp code”.
- Nếu roadmap, gameplay design hoặc dependency có điểm chưa rõ và có thể làm thay đổi architecture/scope, hỏi người dùng trước khi sửa.
- Không tự coi một task là DONE.

Một task chỉ được đánh dấu DONE khi:
1. Code đã hoàn thành.
2. Người dùng đã Play Test trong Unity.
3. Người dùng xác nhận kết quả chạy đúng.

Trước khi người dùng xác nhận, trạng thái phải là:

`CODE COMPLETE — AWAITING UNITY PLAY TEST`

---

## 4. Chia task thành các đầu việc nhỏ

Mỗi task trong roadmap phải được chia thành các đầu việc nhỏ, dễ hiểu và có thể kiểm tra riêng.

Không được xử lý toàn bộ một task lớn trong một lần nếu task đó có thể tách thành nhiều bước hợp lý.

Trước khi bắt đầu một task, hãy:

1. Xác định mục tiêu cuối của task.
2. Chia task thành các đầu việc nhỏ theo đúng thứ tự dependency.
3. Trình bày ngắn gọn flow tổng của task.
4. Giải thích vì sao nên đi theo thứ tự đó.
5. Nêu rõ đầu việc nào đang thực hiện trước.
6. Chỉ sửa code thuộc đầu việc hiện tại, trừ khi dependency bắt buộc phải sửa cùng.

Ví dụ:

`Enemy Architecture`
→ phân tích EnemyAI hiện tại
→ xác định các state đang ẩn trong bool
→ tạo state tối thiểu
→ chuyển Idle
→ chuyển Chase
→ chuyển Attack
→ kiểm tra death/reset/pooling
→ Play Test regression

Không được nhảy từ bước đầu sang bước cuối chỉ để hoàn thành nhanh hơn.

Sau khi hoàn thành một đầu việc nhỏ:
- tóm tắt ngắn phần vừa thay đổi;
- nêu rõ phần nào chưa làm;
- nếu cần người dùng kiểm tra trong Unity ở bước đó thì dừng để người dùng test;
- chỉ chuyển sang đầu việc tiếp theo khi flow hiện tại đã rõ và không có vấn đề chưa giải quyết.

Không tự đánh dấu toàn bộ task DONE chỉ vì một vài đầu việc đã hoàn thành.

---

## 5. Khi có điểm chưa rõ phải hỏi lại

Nếu có bất kỳ điểm nào chưa rõ có thể ảnh hưởng đến:
- gameplay;
- architecture;
- thứ tự roadmap;
- dependency;
- data flow;
- public contract;
- prefab/scene setup;
- plugin behavior;
- hoặc scope của task;

thì phải hỏi người dùng trước khi tự quyết định.

Không được:
- tự đoán ý người dùng khi có nhiều cách triển khai hợp lý;
- tự chọn architecture mới khi requirement chưa rõ;
- tự bổ sung feature ngoài roadmap để “hoàn thiện hơn”;
- tự sửa một phần kế hoạch hoặc scope mà chưa trao đổi;
- tự coi hành vi hiện tại là bug nếu chưa có bằng chứng từ code, Console hoặc Play Test.

Khi cần hỏi lại:
- hỏi đúng vấn đề còn mơ hồ;
- nêu ngắn gọn vì sao điểm đó ảnh hưởng tới implementation;
- nếu có 2–3 phương án rõ ràng thì có thể liệt kê để người dùng chọn;
- không tiếp tục implement phần phụ thuộc vào quyết định đó cho tới khi người dùng trả lời.

Nếu phần chưa rõ không ảnh hưởng đến đầu việc hiện tại, có thể tiếp tục phần chắc chắn và ghi rõ phần đang tạm hoãn.

---

## 6. Quy trình khi bắt đầu một đầu việc

Trước khi sửa code cho đầu việc hiện tại, trình bày ngắn gọn:

- Task lớn đang thuộc roadmap nào.
- Đầu việc nhỏ hiện tại là gì.
- Flow hiện tại.
- Flow dự kiến sau thay đổi.
- Vì sao dùng cách triển khai này.
- File nào cần đọc.
- File nào dự kiến cần sửa.
- Dependency trực tiếp nào cần kiểm tra.
- Rủi ro regression quan trọng cần giữ.

Nếu thay đổi có thể ảnh hưởng architecture hoặc roadmap, dừng và hỏi người dùng trước khi implement.

Nếu yêu cầu đã rõ và người dùng đã yêu cầu implement, có thể tiếp tục code sau phần phân tích trên.

---

## 7. Quy tắc viết C#

- Code phải tường minh, dễ đọc cho người mới.
- Không viết tắt tên biến/hàm khó hiểu.
- Chia hàm nhỏ theo trách nhiệm rõ ràng.
- Comment bằng tiếng Việt ở các dòng/đoạn logic quan trọng.
- Không refactor file ngoài scope nếu không thật sự cần.
- Không thay đổi public contract/event/signature đang được module khác dùng nếu chưa kiểm tra dependency.
- Không mutate ScriptableObject asset gốc cho runtime state.
- Giữ physics gameplay ở Rigidbody2D; visual/tween ưu tiên ở child Visual.
- Không thêm framework lớn như Service Locator, DI framework, ECS, global EventManager hoặc Manager tổng nếu chưa có nhu cầu thật.

---

## 8. DOTween

Mỗi khi thêm hoặc thay đổi DOTween API, phần giải thích phải nói rõ:

- Hàm DOTween đó làm gì.
- Các tham số quan trọng trong `()` có nghĩa gì.
- Tween đang tác động object nào.
- Vì sao dùng DOTween thay vì code timing thủ công.
- Có nguy cơ xung đột với Rigidbody2D, Cinemachine, TimeScale hoặc gameplay logic hay không.

DOTween ưu tiên cho visual/game feel/UI.  
Không dùng DOTween làm physics movement chính khi collision phụ thuộc Rigidbody2D.

---

## 9. Edgar Generation

Giữ ranh giới hiện tại:

`Edgar API`
→ `EdgarDungeonGenerator / EdgarDungeonPostProcessing`
→ `RoomContext / RoomController`
→ gameplay khác.

Edgar chịu trách nhiệm layout/generation.  
Gameplay project chịu trách nhiệm combat, room lifecycle, reward, camera, minimap và progression.

Không để module gameplay mới gọi Edgar API trực tiếp nếu adapter hiện tại đã đủ.

Khi dùng thuật ngữ hoặc API Edgar, giải thích bằng ngôn ngữ đơn giản.

---

## 10. Object Pooling

Project hiện dùng `UnityEngine.Pool.ObjectPool<T>`.

Khi sửa Enemy / Projectile / Drop / DamageText phải kiểm tra:

- State có reset khi reuse không.
- Coroutine cũ có dừng không.
- DOTween cũ có Kill/reset không.
- Rigidbody2D velocity/state có sạch không.
- Event subscription có bị duplicate không.
- Scene reference runtime có cần inject lại không.

Không tạo `PoolManager` chung nếu các pool riêng hiện tại vẫn đủ đơn giản.

---

## 11. Debug

Khi người dùng báo bug:

1. Đọc Unity Console/error trước nếu có.
2. Giải thích thông báo lỗi đang nói gì.
3. Xác định script + line liên quan.
4. Giải thích nguyên nhân bằng ngôn ngữ đơn giản.
5. Sau đó mới sửa code.
6. Đưa checklist test lại.

Nếu chưa có Console/error cần thiết để xác định nguyên nhân, yêu cầu người dùng cung cấp log thay vì đoán và sửa bừa.

---

## 12. Ranh giới giữa Codex và Unity Editor

Codex chỉ tự động thực hiện phần **code và các thay đổi text/code cần thiết trong repository**.
Người dùng tự thao tác trực tiếp trong Unity Editor.

Codex **không được tự chỉnh các thiết lập Unity Editor** thay cho người dùng, bao gồm:
- không tự sửa scene `.unity`;
- không tự sửa prefab `.prefab`;
- không tự chỉnh Inspector/serialized reference trong asset;
- không tự tạo hoặc chỉnh Layer/Tag trong `ProjectSettings`;
- không tự tạo GameObject/Component trong scene hoặc prefab;
- không tự tạo pool object, spawn point, fire point hoặc hierarchy setup trong Unity Editor;
- không tự thay đổi giá trị tuning trong Inspector nếu người dùng chưa yêu cầu rõ;
- không sửa trực tiếp file YAML của scene/prefab/project settings để mô phỏng thao tác Editor.

Codex vẫn được phép sửa C# cần thiết để hỗ trợ setup đó, ví dụ:
- thêm `[SerializeField]`;
- thêm public method/setter cho runtime injection khi architecture yêu cầu;
- thêm component script mới;
- thêm enum/data field/code contract đúng scope task.

Nếu code phụ thuộc vào một quyết định Editor/gameplay chưa rõ, Codex phải hỏi người dùng trước khi implement phần phụ thuộc đó.

Sau khi hoàn thành phần code của một task hoặc đầu việc cần setup Unity Editor, Codex phải dừng và trả lời rõ mục **Unity Editor Setup** theo thứ tự thao tác. Tối thiểu phải nêu:
1. Scene hoặc prefab nào cần mở.
2. GameObject nào cần tạo/chọn.
3. Component nào cần Add Component.
4. Field Inspector nào cần gán reference.
5. Layer/Tag nào cần tạo hoặc gán.
6. Prefab/Pool/Spawn Point/Fire Point nào cần tạo hoặc nối nếu task có dùng.
7. Giá trị tuning ban đầu nào người dùng cần nhập nếu đã được chốt.
8. Thứ tự thao tác để người mới có thể làm theo mà không phải đoán.
9. Checklist Unity Play Test sau khi setup xong.

Không được coi việc code xong là tương đương với Editor setup đã xong.
Không được đánh dấu task DONE cho tới khi người dùng tự setup trong Unity Editor, Play Test và xác nhận PASS.

---

## 13. Verification và Unity Play Test

Codex có thể:
- kiểm tra diff;
- search dependency;
- chạy các kiểm tra local an toàn và phù hợp nếu cần.

Nhưng Codex không được tự tuyên bố gameplay Unity đã chạy đúng chỉ vì code compile hoặc không thấy lỗi tĩnh.

Sau khi code xong phải đưa checklist Play Test cụ thể cho người dùng.

Không update task thành DONE trước khi người dùng xác nhận Play Test.

---

## 14. Cập nhật tài liệu sau khi Play Test PASS

Chỉ sau khi người dùng xác nhận test thành công:

- `Docs/PROJECT_HANDOFF.md`
  - cập nhật Current Progress, Next Task, First Next Step và blocker nếu có.

- `Docs/PROJECT_HISTORY.md`
  - thêm task vừa hoàn thành, flow, regression đã test, bug/quyết định đáng nhớ.

- `Docs/PROJECT_ARCHITECTURE.md`
  - chỉ cập nhật nếu contract/module/data flow/architecture thật sự thay đổi.

- Roadmap V4.1 `.xlsx` trong `Docs/`
  - cập nhật trạng thái task và tổng tiến độ.
  - nếu scope có phần deferred, ghi rõ; không giả định đã làm.

Không rewrite toàn bộ tài liệu khi chỉ cần sửa/append đúng section liên quan.

---

## 15. Git

- Không commit, push, reset, checkout hoặc xóa file nếu người dùng chưa yêu cầu.
- Trước thay đổi lớn, kiểm tra working tree nếu cần để tránh ghi đè công việc chưa commit.
- Khi người dùng yêu cầu tạo commit, tóm tắt đúng các thay đổi kể từ commit trước; không đưa file/generated content không liên quan vào commit.

---

## 16. Phong cách giao tiếp

Người dùng mới học lập trình khoảng vài tháng.

- Giải thích đơn giản, theo từng bước nhỏ.
- Dùng ví dụ/ẩn dụ thực tế khi logic khó hình dung.
- Không dùng thuật ngữ hàn lâm nếu chưa giải thích.
- Không đưa quá nhiều thay đổi một lúc.
- Không tự kết luận sớm khi dữ liệu chưa đủ.
