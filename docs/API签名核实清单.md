# RUIF · API 签名核实清单（I0 成果）

> 日期：2026-09-09 · 工具：`ilspycmd 8.2.0.7535` 反编译 **Assembly-CSharp.dll**（RimWorld 1.6 实机程序集）
> 目的：把设计文档里所有「编码期反编译核实」的官方 API 全部落定，**后续编码只认本表，不靠记忆**
> 方法：`ilspycmd <Assembly-CSharp.dll> -t <Namespace.Type> -o <dir>`，产物文件 **0 字节 = 该名称不存在**
> 证据：反编译产物为一次性取证，提取签名后已删除（不随 Mod 分发）

---

## 1. 命名空间纠正（重要，设计文档按此修订）

| 设计文档原记 | 实际（反编译实证） | 影响组件 |
|---|---|---|
| `GenDate` | ⚠️ **是 `RimWorld.GenDate`**（`Verse.GenDate` 反编译为 0 字节，不存在） | `TimeStamp` |
| `Caravan` | ⚠️ **是 `RimWorld.Planet.Caravan`** | `CaravanPanel` |
| `Messages` | ⚠️ **是 `Verse.Messages`**（非 `RimWorld.Messages`） | `MessageFeed` |
| `PawnRecords` | ⚠️ 类型名为 **`RimWorld.Pawn_RecordsTracker`** | `RecordsTable` |
| `Ability / AbilityDef` | ⚠️ **是 `RimWorld.Ability`**（`Verse.Ability` / `Verse.AbilityDef` 均不存在） | `PsycastTree` |

---

## 2. 核实结果总表

| # | 类型 / 成员 | 命名空间 | 状态 | 关键签名 / 说明 | 对应组件 |
|---|---|---|---|---|---|
| 1 | `GenDate` | **RimWorld** | ✅ | `DateFullStringAt(...)` 等（13.9KB 类） | TimeStamp |
| 2 | `GenText` | Verse | ✅ | 文本工具族 | FormattedNumber |
| 3 | `GenColor` | Verse | ✅ | 颜色工具族 | Swatch |
| 4 | `KeyPrefs` | Verse | ✅ | 键位偏好与冲突检测 | HotkeyBind / KeyConflictHint |
| 5 | `TickManager` | Verse | ✅ | 时间流速管理 | TimeSpeedControl |
| 6 | `LetterStack` | Verse | ✅ | 信件栈 | LetterCard / LetterMergeRule |
| 7 | `WealthWatcher` | RimWorld | ✅ | 财富统计 | WealthBreakdown |
| 8 | `BillStack` | RimWorld | ✅ | 工作台账单栈 | BillList |
| 9 | `Tale` | RimWorld | ✅ | 传说系统 | TaleCard |
| 10 | `Pawn_RecordsTracker` | RimWorld | ✅ | pawn 记录 | RecordsTable |
| 11 | `Caravan` | **RimWorld.Planet** | ✅ | 商队（世界对象） | CaravanPanel |
| 12 | `Messages` | **Verse** | ✅ | 瞬息消息管理 | MessageFeed |
| 13 | `Ability` | RimWorld | ✅ | 1.6 技能/灵能体系统一为 Ability | PsycastTree |
| 14 | `Psycast` | RimWorld | ✅ | 类仍存在（30KB 级） | PsycastTree |
| 15 | `HistoryEvent` / `HistoryEventDef` | RimWorld | ✅ | 历史事件系统 | HistoryFeed |
| 16 | `Widgets.DrawTextureRotated` | Verse | ✅ | `DrawTextureRotated(Vector2 center, Texture tex, float angle, float scale = 1f, Material material = null)` — **官方已有旋转贴图**，LoadingSpinner 由 ⚠️ 升 ✅ | LoadingSpinner |
| 17 | `Widgets.TextFieldNumeric` | Verse | ✅ | `TextFieldNumeric(rect, ref val, ref buffer, min, max)`（泛型 + buffer） | NumericField / QuickAmounts |
| 18 | `Widgets.HorizontalSlider` | Verse | ✅ | **`HorizontalSlider(Rect rect, ref float value, FloatRange range, string label = null, float roundTo = -1f)`** — 是 **ref 语义**，与部分旧文档的"返回 float"用法不同 | SliderWithValue / DoubleSlider |
| 19 | `Widgets.ButtonText` | Verse | ✅ | `ButtonText(Rect, string, bool drawBackground = true, bool doMouseoverSound = true, bool active = true, TextAnchor? overrideTextAnchor = null)` — **确认无 disabled 参数，禁用走 `active:false`** ✅ 规则成立 | 全部按钮 |
| 20 | `Widgets.RadioButton` | Verse | ✅ | `RadioButton(Vector2 topLeft, bool chosen, bool disabled = false)` + `RadioButtonLabeled` — 有 disabled、返回 bool | Segmented / 单选 |
| 21 | `Widgets.BeginScrollView` | Verse | ✅ | `BeginScrollView(Rect outRect, ref Vector2 scrollPosition, Rect viewRect, bool showScrollbars = true)` | ScrollView / ThemedScrollBar |
| 22 | `Widgets.DrawAltRect` | Verse | ✅ | `DrawAltRect(Rect rect)` | ZebraRow |
| 23 | `Widgets.DrawBoxSolid` | Verse | ✅ | `DrawBoxSolid(Rect rect, Color color)`（内部重置 GUI.color — 规则不变） | 全部底绘制 |
| 24 | `FloatMenu` / `FloatMenuOption` | Verse | ✅ | 知识库 §38 核心 UI 族（本轮未重验，KB 已收录） | InPlaceDropdown |

---

## 3. 结论

- **10 个原「待核实」全部落定**，其中 **5 个命名空间需纠正**（见 §1）；
- `DrawTextureRotated` 官方已有 → **LoadingSpinner 从 ⚠️ 升级为 ✅**；
- `HorizontalSlider` 是 **ref 语义**（不是返回值），SliderWithValue/DoubleSlider 设计按此调整；
- `ButtonText` 无 disabled、`RadioButton` 有 disabled —— 与既有 RW 1.6 铁律一致，规则**得到实证确认**；
- 本表是 I1–I3 编码的**唯一 API 依据**；如后续发现与实机不符，以本表增补修订，不以记忆为准。
