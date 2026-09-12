# 创意工坊 UI Mod 参考研究

> 目的：把创意工坊上已验证成功的 UI Mod 作为 RUIF 设计的**活证据与模式库**。
> 日期：2026-09-08 · 信息来源：GitHub（RimHUD/WorkTab/VEF 仓库）、RimWorld Base、环世界中文社区
> **版权红线：只学习交互模式与设计语言，不照搬任何代码 / 美术资产 / 布局数据。RUIF 全部自绘实现。**

---

## 1. 为什么值得参考

工坊 Mod 是「RimWorld 引擎 UI 能做到什么」的**实机活证据**——比任何设计推测都硬。本节逐个拆解代表 Mod 的 UI 模式，映射到 RUIF 组件图鉴，并把被实证的新能力吸收进 ✅/⚠️ 分级。

---

## 2. 代表性 Mod 逐个拆解

### 2.1 RimHUD（Jaxe-Dev）— 检视面板重设计
- **模式**：①检视面板集成模式（可调宽度）②独立浮动窗口（可停靠屏幕任意位置）③视觉警告系统（致命病症/待处理伤口/临近精神崩溃时高亮）④**布局预设** ⑤**向其他 Mod 开发者暴露自定义组件 API** ⑥翻译以语言子 Mod 分离发布
- **实现方式**：大量 Harmony 补丁（`InspectPaneUtility` 标签页/OnGUI/尺寸、`GeneUIUtility` Transpiler、`ActiveTip` 等），部分 Prefix 完全绕过原方法
- **对 RUIF 的启示**：
  - ✅ **实证新能力**：浮动可停靠窗口、检视面板宽度扩展、警告驱动的条件高亮——全部引擎可实现 → 候选组件 **DockHost（停靠宿主）**、**SectionResize**
  - ✅ **验证真实需求**：RimHUD 暴露第三方组件 API 证明「组件注册扩展点」（方案 §10）是工坊级真实需求，不是我们的臆想
  - ⚠️ **反面教材**：它对 `InspectPaneUtility` 的多处补丁 = 每次游戏更新都可能断 + 与其他改检视面板的 Mod 高冲突——正是 RUIF「核心不打补丁」纪律要避开的路线。RUIF 做自己的窗口，不抢原版面板
  - ✅ 翻译子 Mod 分离 → 采纳为 RUIF 本地化发布形态（本体英文 + 语言子包）

### 2.2 Work Tab（fluffy-mods）— 工作优先级重设计
- **模式**：①接管原版工作页签 ②按分栏排序 ③**拖拽改优先级** + 批量开关/调整 ④**预设（Profile）保存/加载** ⑤**时间表调度**（逐小时×逐工作）
- **对 RUIF 的启示**：
  - ✅ **实证新能力**：拖拽交互网格、时间表矩阵、批量套用——纯 IMGUI 全部可行（它就是这么做的）→ 候选组件 **PriorityGrid（拖拽优先级网格）**、**TimetableMatrix（时间表矩阵）**、**ProfilePreset（配置档案）**
  - ✅ 预设/档案模式 → 布局预设、过滤器预设都可复用此交互

### 2.3 Fluffy Mod Manager — Mod 列表管理
- **模式**：搜索过滤 + 拖拽排序 + 多档案（Profiles）切换 + 分组 + 冲突提示
- **对 RUIF 的启示**：✅ 长列表的搜索/拖拽/档案切换组合已被大规模验证；ProfilePreset 再添实证；「分组折叠列表」→ CollapsibleTree 的应用实证

### 2.4 Numbers / Animal Tab / Medical Tab — 列式数据页签
- **模式**：把 Pawn 数据做成**可配置列的表格**（用户自选列、排序、过滤、存列配置）
- **对 RUIF 的启示**：✅ StatTable（可排序统计表）的真实用户期望 = **列可配置 + 列配置可存档** → StatTable Expert 档补 `.SaveColumns()`；列式页签是大玩家群体的刚需形态 → 候选 **ColumnTab 页面预设**（M1 主从的表格特化）

### 2.5 ResearchPal / Research Tree（fluffy）— 研究树
- **模式**：研究项目节点图（依赖连线自动布局 + 缩放平移 + 完成态着色 + 前置高亮）
- **对 RUIF 的启示**：✅ **实证新能力**：节点图渲染（框+折线连线+平移缩放）在引擎内完全可行 → 候选组件 **NodeGraph（节点图，通用化后可用于科技树/工序链/谱系）**；注意与图鉴 ⚠️ 分级一致——自动布局算法在数据层，绘制层仍是框+折线（ResearchPal 的连线就是直线/折线，不是贝塞尔）

### 2.6 Vanilla UI Expanded（VE 团队，2026 新作）— 系统性 UI 改良
- **模式**：对原版 UI 做一系列小而美的改良（信息密度、布局整理）
- **对 RUIF 的启示**：⚠️ 这是**最直接的同类**——「改良原版 UI 而非提供组件库」。RUIF 的差异化必须守住：VEF/VE 改良**原版**界面（必然走向补丁路线），RUIF 提供**新窗口的组件库**（零补丁）。两者可共存，但文档要说清边界，避免用户误装冲突

### 2.7 Vanilla Expanded Framework（VEF）— 既有"框架"先例
- **模式**：VE 系列共享代码库（Def/行为/部分 UI 辅助），开源、多 Mod 依赖同一框架
- **对 RUIF 的启示**：✅ **共享框架 Mod 模式在工坊完全成立**（数百个 Mod 依赖 VEF）——RUIF 的分发模式（方案 §10）有重量级先例；VEF 开源+PR 协作模式值得借鉴（RUIF 走 GitHub 开源）

### 2.8 其他值得留意的
| Mod | 可借鉴模式 |
|---|---|
| Interaction Bubbles | 世界空间气泡（对话/需求浮字）→ 候选 SpeechBubble（ⓘ 属地图层非窗口层，RUIF v1 暂不纳入，记 backlog） |
| Dubs Mint Menus | 菜单树重构（账单/配方的分组树）→ CollapsibleTree + TagPicker 组合实证 |
| Colour Picker | 取色器 → 印证图鉴 ColorPicker 候选（HSV 条 ✅ 可行） |
| Combat Readiness / 各战术面板 | HUD 化指挥界面 → M6 指挥台 HUD 模式实证 |

---

## 3. 交叉验证结论（吸收进图鉴）

| 实证结论 | 来源 | 对图鉴的动作 |
|---|---|---|
| 拖拽优先级网格 100% 可行 | Work Tab | 新增候选 **PriorityGrid**（✅，Expert 档） |
| 节点依赖图（折线+缩放）100% 可行 | ResearchPal/ResearchTree | 新增候选 **NodeGraph**（✅ 绘制 / 自动布局算法在数据层） |
| 浮动可停靠窗口 100% 可行 | RimHUD | 新增候选 **DockHost**（✅，Harmony-free 实现：自管 Window + 停靠吸附数学） |
| 配置档案（Profile）模式刚需 | Work Tab / Mod Manager | 新增候选 **ProfilePreset**（✅） |
| 时间表矩阵 100% 可行 | Work Tab | 新增候选 **TimetableMatrix**（✅） |
| 第三方组件注册是真实需求 | RimHUD custom components | 强化方案 §10 扩展点优先级（提前到 I3） |
| 列配置可存档是表格刚需 | Numbers | StatTable 补 `.SaveColumns()` |
| 共享框架模式工坊成立 | VEF | RUIF 分发模式定心丸；开源协作借鉴 |

## 4. 差异化定位（对工坊同类要说清的话）

```text
RimHUD / Work Tab / Numbers …  = 改良某个原版界面（深入但专精，多数依赖 Harmony 补丁）
VEF                             = VE 系列私有共享库（行为框架，非 UI 组件框架）
Vanilla UI Expanded             = 原版 UI 系统性改良（补丁路线）
RUIF                            = 面向 Mod 开发者的【通用 UI 组件框架】：新窗口零补丁搭建，
                                  组件库 + 布局 + 主题 + DevTools，公共 API 稳定
```

RUIF 不替代上述任何 Mod，而是让**未来的 Mod 作者不必再为每个 Mod 手写一遍** RimHUD 式的界面。

## 5. 行动项

- [ ] PriorityGrid / NodeGraph / DockHost / ProfilePreset / TimetableMatrix 加入图鉴 backlog（标注"工坊实证"）
- [ ] StatTable Expert 档补列配置存档
- [ ] 方案 §10 扩展点提前级：组件注册从 I3 提前评估
- [ ] RUIF 发布形态确认：本体英文 + 语言子包（借鉴 RimHUD）
- [ ] 图鉴 H 节补一行「工坊实证来源」注记
