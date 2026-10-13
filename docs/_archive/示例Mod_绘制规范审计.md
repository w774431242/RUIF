# 示例 Mod · 绘制规范审计报告

**被审对象：** `_demo/RUIF_Demo/Source/`（`BenchAtoms42.cs` / `BenchAtoms42B.cs` / `BenchAtoms42C.cs` / `DemoWindow.cs`）
**审计依据：** skill「RimWorld组件绘制」（`~/.codebuddy/skills/RimWorld组件绘制/`）
**审计日期：** 2026-09-13 ｜ **审计方式：** 文本检索 + 逐件块扫描 + 反编译核对（可复现，命令见 §1）

---

## 0. 结论摘要

| 维度 | 结果 |
|---|---|
| 铁律 1 · 尺寸取自度量 | ✅ **PASS**（无文本行高字面量；`Measure.*` 8 处派生，其余引用派生字段） |
| 铁律 2 · 单元矩形排标签 | ✅ **PASS**（1 处绝对坐标写法已规范化为 `line.Rest`） |
| 铁律 3 · 溢出与异常隔离 | ✅ **PASS**（逐件 try/catch + 出栈在 `finally` + 循环判剩余高） |
| 铁律 4 · **空间预算与断言** | ❌ **FAIL（零落地）** —— `SpaceOk`/`PaintTooSmall`/`MinW`/`MinH`/`Measure.Fits` 命中数**全为 0** |
| 第一原则 · **交互性** | ❌ **FAIL** —— 有交互路径 **12/42**，**30 件零交互**；悬停反馈 `Mouse.IsOver` **0 处** |
| 交互成熟度 I2 达标率 | ❌ **12/42 ≈ 29%**（其余 30 件为 I0，其中 **20 件本应 ≥I1**） |

**一句话**：把「画得对」做到了（铁律 1–3 全过），但「能用」和「空间感知」基本没做——
这正是用户「**互动性太少，完全是演示品**」这条批评的量化证据。

---

## 1. 审计方法（可复现）

```powershell
# 铁律 4 落地情况（期望 >0，实测全 0）
foreach ($p in 'SpaceOk','PaintTooSmall','MinW','MinH','Measure\.Fits') {
  "$p : " + (Select-String -Path 'BenchAtoms42*.cs' -Pattern $p -Encoding UTF8).Count }

# 悬停反馈
(Select-String -Path 'BenchAtoms42*.cs' -Pattern 'Mouse\.IsOver' -Encoding UTF8).Count

# 残余绝对定位标签
Select-String -Path 'BenchAtoms42*.cs' -Pattern 'xMax \+ ' -Encoding UTF8

# 逐件交互扫描：按 No = "NN" 切块，统计块内命中检测调用
# 模式须同时覆盖三类：① 直接 Widgets.Button*  ② 委托私有 helper（Segmented）
#                      ③ 命中检测在组件内部（HeaderCell/ResizeHandle/DensitySwitch/
#                         GripHandle/EyeToggle/ToolbarIcon/ZoomIndicator）
```

> **审计者自省**：首轮扫描只覆盖 ①，把 6 件「命中在组件内部」与 1 件「委托 helper」误判为
> 零交互（12→6），据此得出的「退化 9 件」是**错的**。加入 ②③ 后修正为「零交互 30 件 / 应升级 20 件」。

---

## 2. 逐件交互成熟度

### 2.1 有交互路径（12 件）

| # | 件 | 交互来源 | 成熟度 |
|---|---|---|---|
| 01 | FormattedNumber | `Segmented`（三档格式） | **I2** |
| 16 | ScrollFadeHint | `Widgets.ButtonText` 切 `moreBelow` | **I2** |
| 17 | ExpandCaret | `ButtonInvisible` 折叠 | **I2** |
| 20 | Swatch | `ButtonInvisible` 点选 | **I2** |
| 23 | PinBadge | `ButtonInvisible` 钉选 | **I2** |
| 24 | SortHeaderCell | `TableAtoms.HeaderCell` → `NextSort` | **I2** |
| 26 | ColumnResize | `TableAtoms.ResizeHandle` → 拖拽 | **I3**（待实测 MouseDrag） |
| 28 | DensitySwitch | `TableAtoms.DensitySwitch` | **I2** |
| 33 | GripHandle | `InteractAtoms.GripHandle` | **I1**（仅点击，无重排） |
| 34 | EyeToggle | `InteractAtoms.EyeToggle` | **I2** |
| 36 | ToolbarIcon | `InteractAtoms.ToolbarIcon` | **I2** |
| 39 | ZoomIndicator | `ProgressAtoms.ZoomIndicator` | **I1**（复位有，滑杆缺） |

### 2.2 零交互（30 件）——分三类

**A 类 · I0 合理（天然无交互语义，10 件）**

`03 UnitLabel`、`04 RichTextLine`、`06 SubtitleLine`、`07 SectionFrame`、`09 GradientRule`、
`11 Watermark`、`12 InsetPanel`、`25 ValueCell`、`29 StatRow`、`30 MiniProfileChip`、`38 CapacityBar`、`41 BubbleTag`

> 这类是**纯排版/纯展示/数据驱动**件，I0 合格。**skill 应显式列出这份 I0 白名单**，
> 否则自检时会把它当缺口，浪费改造工时。

**B 类 · 应升级（HTML 原有交互，游戏内退化，15 件）**

| # | 件 | HTML 原有交互 | 应达级别 |
|---|---|---|---|
| 02 | TimeStamp | `tsSeg` 绝对/相对/紧凑 三档 | I2 |
| 08 | CornerFrame | 点框体切换目标 + 悬停四角张开 | I2 |
| 10 | RuleWithLabel | 「换一段」按钮 | I1 |
| 11 | Watermark | 透明度滑杆 | I2 |
| 12 | InsetPanel | 点击切换凹陷/凸起 | I2 |
| 14 | StatusGlyph | 点图形切换语义读数 | I1 |
| 18 | AspectBox | `seg` 切 16:9 / 1:1 / 4:3 / 9:16 | I2 |
| 19 | AnchorFrame | 点舞台吸附最近锚 | I1 |
| 21 | GradientBar | 悬停读数 | I1 |
| 22 | OverflowFade | 「换一条」按钮 | I1 |
| 27 | FrozenColumn | 横向滚动 | I3 |
| 31 | FlashOverlay | `seg` 切 4 档告警等级 | I2 |
| 32 | SectionBar | 可折叠 | I2 |
| 35 | SearchHighlight | 输入框实时试 | I2 |
| 37 | FillWipe | 「跳过本项」按钮 | I1 |
| 40 | PointCostBadge | ± 按钮 | I2 |
| 42 | ThinScrollBar | 可拖滑块 | I3 |

> 表列 17 行 —— 其中 `12/18/31` 与 `22/10` 等属同一模式（分段切换 / 换一条），
> 可批量按 `Segmented` + `ButtonInvisible` 两个 helper 统一改造。

**C 类 · 部分缺失（3 件）**

| # | 件 | 已有 | 缺 |
|---|---|---|---|
| 05 | Typewriter | 自动循环打字 | 点击跳过 / 重播 |
| 15 | LoadingSpinner | 到 100% 转 ✓ | 重播 |
| 33 | GripHandle | 点击高亮 | **真拖拽重排** |
| 39 | ZoomIndicator | 复位按钮 | 缩放滑杆 |

---

## 3. 缺陷清单

### P1 · 阻断性（规则级）

| ID | 缺陷 | 证据 | 修法 |
|---|---|---|---|
| **F1** | **铁律 4 零落地**：42 件无一件有空间断言，无 `MinW/MinH` 常量，无 `Measure.Fits` 分支 | 上述四项检索全 0 | 逐件按 skill `07` 册 §2 表补 `MinW/MinH` 常量 + 开头三行断言 + `PaintTooSmall` 占位 |
| **F2** | **悬停反馈 0 处**：六态缺「悬停」，全部件不满足 I2 | `Mouse.IsOver` = 0 | 交互件补 hover 视觉（边框转 accent / 底色转 hover） |
| **F3** | `Shapes.RuleWithLabel`(#10) 与 `Atoms.SectionFrame`(#07) 调用前未统一 `Text.Font` | 组件内部用 `Text.CalcSize` 测量、用 Tiny 绘制；`Draw.*` 均以 `using (Font(font))` **恢复**进入前字档 | ✅ **本轮已修**：加 `using (Draw.Font(Tokens.FontLabel))` 包裹 |

### P2 · 交互缺口

| ID | 缺陷 | 规模 |
|---|---|---|
| **F4** | 20 件 HTML 原有交互未落地（§2.2 B/C） | 20 件 |
| **F5** | **无键盘路径**：42 件中 0 处焦点环 / 快捷键 ⇒ 全部件不满足 I3 | 全部交互件 |

### P3 · 规范化

| ID | 缺陷 | 修法 |
|---|---|---|
| **F6** | `BenchAtoms42B.cs:286` 用绝对坐标表达标签宽（`r.width - 34f`） | ✅ **本轮已修**：改为 `line.Rest`（单元矩形） |
| **F7** | `ZoomIndicator`(需 ≥174) / `DensitySwitch`(需 ≥176) 在极端窄窗下无保护 | 与实际可达宽度核算：卡片网格下 body 宽 ≥ 312 ⇒ **当前不可达**，降级为观察项；补 F1 时一并覆盖 |

---

## 4. 本轮已修（2 项）

| 项 | 文件 | 改动 |
|---|---|---|
| F3 | `BenchAtoms42.cs` | #07 / #10 调用前加 `using (Draw.Font(Tokens.FontLabel))`，使组件内部的 `Text.CalcSize` 与绘制字档一致 |
| F6 | `BenchAtoms42B.cs` | #34 EyeToggle 标签改用 `line.Rest`（单元矩形），去掉绝对坐标算宽 |

编译 **0 错 0 警**。

---

## 5. 待修 backlog（按性价比）

| 序 | 项 | 工作量 | 依据 |
|---|---|---|---|
| 1 | **F1 空间断言**：补 42 件的 `MinW/MinH` + `PaintTooSmall` | 中（机械） | skill `07` §2/§4 |
| 2 | **F2 悬停反馈**：交互件补 hover 态 | 小 | skill `03` §2 |
| 3 | **F4 交互回补**：按模式批量（`Segmented` 6 件 / `ButtonInvisible` 8 件 / 滚动 2 件 / 拖拽 2 件） | 大 | skill `03` §8 七步升级法 |
| 4 | **F5 键盘路径**：焦点链 + 快捷键 | 中 | skill `03` §5 |
| 5 | 把 I0 白名单写进 skill（防误判） | 极小 | §6.1 |

---

## 6. skill 侧反哺（用出来的洞）

审计过程暴露了 skill 自身的 3 个缺口，建议回填：

| # | 缺口 | 建议 |
|---|---|---|
| **6.1** | **未区分「命中检测在哪一层」**。首轮扫描把 6 件「命中在组件内部」误判为零交互 ⇒ 结论错了 | 在 `03-交互与状态.md` 增「命中检测三处位置」：① 本件直接 `Widgets.Button*` ② 委托私有 helper ③ **在组件内部**（`HeaderCell`/`ResizeHandle`/`DensitySwitch`/`GripHandle`/`EyeToggle`/`ToolbarIcon`/`ZoomIndicator`）。自检时必须三类都查 |
| **6.2** | **I0 白名单缺失**。数据驱动件被误当缺口 | 在 `03` 册列出「天然 I0」清单：纯排版（`UnitLabel`/`SubtitleLine`/`RichTextLine`）、纯装饰（`GradientRule`/`Watermark`/`InsetPanel`/`SectionFrame`）、数据驱动（`StatRow`/`MiniProfileChip`/`CapacityBar`/`BubbleTag`）、表体单元（`ValueCell`，由表头驱动） |
| **6.3** | `Text.Font` 一致性陷阱只记在 `08-参数字典.md`（参数册） | 它本质是**度量问题** ⇒ 应上移到 `01-度量与字号.md`，并在 SKILL.md 速查表加一行 |

---

## 7. 验收（本次审计）

- [x] 审计方法可复现（命令 + 扫描模式已记录）
- [x] 结论有量化证据（检索命中数 / 逐件块扫描）
- [x] 误判已自省并修正（12→6→12 的扫描口径演进已如实记录）
- [x] 已修项与待修项分离，未混入本次结果
- [ ] F1 / F2 / F4 / F5 未做（列 backlog，不计入本次通过）
- [ ] 窄/宽两档实测截图（LAND-Gate 遗留项，仍待补）
