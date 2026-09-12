# RUI Framework 1.0 评估与 UIKit 对齐

> 评估对象：`D:\AAA下载\RUI_Framework_1.0_开发者UI框架设计.html`（RimWorld 1.6 开发者 UI 框架设计样板）
> 评估日期：2026-09-08
> 关联文档：`RUIF/docs/底层UI框架方案.html`（RUIF 独立 Mod 草案）
> 结论性质：**设计表达优秀，但存在范式错配风险；方向可吸收，路径须走渐进式并补齐 RW 1.6 真实约束。**

---

## 0 · 一句话结论

RUI 的**分层与三档 API 表达质量很高**，可直接吸收；但它用「保留模式(ретained-mode) 的 UI Tree 心智模型」去套 RimWorld 的「即时模式(IMGUI) 每帧重绘」现实，若不先坐实范式，后续设计全是空中楼阁。本项目若要落地，应**吸收其优点、走 UIKit 的渐进路径、把 RW 1.6 真实坑写进 Core 层**。

---

## 1 · 设计亮点（应吸收项）

| 项 | 内容 | 对我方价值 |
|---|---|---|
| 分层架构 | Core → Layout → Components → Data → Theme（+ DevTools） | 与 UIKit L0–L5 高度一致，可对齐命名 |
| 三档 API | Minimal `RUI.Button("开始")` / Advanced `.Style().Tooltip()` / Expert `.Virtualized().MultiSelect()` | DX 设计比 UIKit 更成体系，直接抄 |
| Token 与组件分离 | `RTheme.Color.Accent` 不写死 `Color.red` | 与现有 `UITheme` 单一真源原则一致 |
| 兼容性策略 | Verse/Unity API 收到 Adapter 层，不泄漏公共 API | 未来 1.7 改 API 只动 Adapter |
| 性能清单 | 虚拟化、缓存、Dirty Flag、Texture 缓存、Profiler | 与我方性能纪律吻合 |
| 交互式文档 | tab 切换 + 搜索过滤的 HTML | 对外展示组件库的标准形态 |

---

## 2 · 关键风险（按严重度）

### ⚠️ 2.1 保留模式 ≠ IMGUI（最致命）
文档生命周期：`Build UI Tree → Measure → Layout → Draw → Input → State Update` + `Rebuild/Reuse`。这是 WPF/Flutter 的保留模式心智。但 RimWorld 窗口是：
```csharp
public override void DoWindowContents(Rect inRect) { /* 每帧重画 */ }
```
IMGUI 没有"保留的组件树"，每帧重新执行绘制代码。在 IMGUI 上硬造「树 + Measure + Layout 每帧」：
- 每帧建树（闭包/`List`/`x=>x.Name`）→ 直接撞文档 §11 自己写的"避免每帧 new List/LINQ/闭包"铁律，**自相矛盾**；
- 池化 + Dirty Flag 复用树 → 复杂度飙升，等于自研迷你 UI 引擎。

**必须先二选一定调**：① 即时模式 + 轻量布局助手（只算一次 Rect，如 UIKit L4）；② 真·保留模式虚拟树（类 React，需自研 diff/reconcile，工作量 ×5 且 IMGUI 下收益存疑）。

### ⚠️ 2.2 完全没碰 RimWorld 1.6 真实坑
文档是框架无关理想设计，但真能用的 RW UI 框架必须把这些写进底层，否则每行都在踩雷（"隐藏 Rect/Widgets 样板"正是其卖点，却漏了）：
- `Listing_Standard.RadioButton` 返回 bool、**无 RadioButtonGroup**（MEMORY 已记）；
- `Widgets.ButtonText` **无 disabled 参数**，禁用须 `active:false`；
- `GUI.color` 改了**必须配对恢复**（try/finally），否则染色泄漏全游戏 —— `RUI.Column` 的 `using` 作用域正好是该强制配对的接缝；
- `TipSignal` 第二参是**延迟秒数**，非开关；
- **CJK 行高经验值 22f**，文档排版假设了 Web 字体度量，不适配。

### ⚠️ 2.3 State 生命周期未定义
`RUI.State.Create(() => new CareerState())` —— **状态存在哪？** IMGUI 窗口关掉即弃，状态必须由 Mod 侧（GameComponent / 静态）持有，不能活在窗口里。未说清 state 所有权与跨窗口重开存活，是会丢数据的真雷（Scribe 铁律刚踩过）。

### 2.4 范围过大，v1.0 难收敛
Charts / Timeline / Animation / Visual Layout Designer / Theme Editor 全进 Roadmap。作为"设计"可野心大，但作为**本项目可落地方案**会永远停在 PPT。UIKit 用 I1(L0+L4)→I4 渐进，首迭代只做"布局助手 + 绘制态配对"，立刻减样板、零语义风险。

### 2.5 与现有资产关系未说明
本项目已有可运行 `UITheme`(L1) + `UIComponents`(L2/L3) 两层。RUI 是纯 greenfield，对存量零复用。
- 若它是**社区发布框架**（独立、兼容多 Mod）→ 目标不同，可并存；
- 若它是**本项目内部基建** → 直接 extend 现有两层比另起炉灶省力，且已 0 错 0 警跑着。

---

## 3 · 与本项目 UIKit 方案对比

| 维度 | RUI_Framework(此文档) | 本项目 UIKit 方案 |
|---|---|---|
| 范式 | 保留模式 Tree（隐含） | 即时模式 + 声明式布局助手 |
| 起步范围 | 一次铺全（Core→Charts→DevTools） | I1 只做 L0+L4，立刻见效 |
| RW 真实坑 | 未覆盖 | 已写入 L0 适配器契约 |
| 存量复用 | greenfield | 复用 UITheme/UIComponents |
| DX 三档 API | ✅ 更成熟 | 可借鉴补强 |
| DevTools | ✅ 更完整 | 未规划（可后加） |

---

## 4 · 落地前必须回答的 5 个问题（决定死活）

1. **范式**：即时模式助手，还是真保留模式虚拟树？（影响 80% 工作量）
2. **State 归属**：状态由谁持有、如何跨窗口/读档存活？
3. **每帧零分配**：闭包式 API（`x=>x.Name`）如何不违反自身性能铁律？要不要用对象池/`NativeArray`？
4. **是否复用现有 UITheme/UIComponents**，还是另立 `RUI.Theme` 重复造？
5. **目标定位**：社区发布框架（需独立、兼容多 Mod）还是本项目内部基建（优先复用、快速收敛）？

---

## 5 · 吸收与修正建议（合并进 UIKit）

- **吸收**：三档 API 设计、DevTools 规划、Token/组件分离、兼容性 Adapter 层 —— 并入 `底层UI框架方案.html`。
- **修正**：把 RUI 的 retained-tree 生命周期改为「即时模式 + Layout 算一次 Rect + Dirty 复用」，并把 RW 1.6 真实坑写进 L0 `Draw/Input` 契约。
- **收敛**：首版砍掉 Charts/Animation/Designer，先做 `Core + Layout + Button + Label + Panel + Progress + Theme`（对应 RUI 0.1–0.2）。

---

## 6 · 决策建议

1. 若目标是**社区发布框架**：以 RUI 为蓝本，但必须先回答 §4 的范式与 State 问题，并把 §2.2 的 RW 坑列为 Core 层硬约束。
2. 若目标是**本项目内部基建**：不要另起 RUI，直接扩展现有 `UITheme/UIComponents` 为 UIKit，首迭代 I1（L0 运行时适配 + L4 布局编排），低风险、立竿见影。
3. 无论哪种，**三档 API 与 DevTools 规划都值得直接搬**。

---

## 7 · 待办

- [ ] 把 RUI 三档 API、DevTools、Adapter 层吸收进 `底层UI框架方案.html`
- [ ] 在 UIKit L0 明确列出 RW 1.6 绘制态/禁用/RadioButton/TipSignal/CJK 行高契约
- [ ] 明确本项目 UIKit 与"可能独立的 RUI"是复用还是并存
