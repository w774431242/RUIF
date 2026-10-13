# RUIF 组件总目（自动生成 · 勿手改）

> 来源：`Assemblies/RUIF.dll` 反编译解析（`M:` 条目**只有参数没有返回类型** ⇒ 返回类型取自反编译实现）。
> 原始数据 `RUIF组件总目.tsv`；**示例 Mod 的标准编号表** `_demo/RUIF_Demo/Source/StdIds.gen.cs`（与本文件同口径）。

## 标准编号体系

`<族代号>-<两位序号>`。**序号按族代号连续（不按类）**，族内排序键 = **类优先级 → 类名 → 方法名**，其中类优先级为 **主体类(0) → `*Cand` 候选(1) → `*Fmt` 工具(2)**。**编号稳定**：新增件序号往后排，不重排已有件。

> ⚠ 修正记录（2026-09-13）：初版漏掉类优先级 ⇒ 主体类 `Charts` 被 `ChartCandAtoms`/`ChartCandFmt` 挤到 `CHT-19` 起。加优先级后 `Charts` = `CHT-01…10`。

| 族代号 | 类 | 件数 |
|---|---|---:|
| `BRG` | HiAtoms · PawnTableAtoms · ReorderAtoms · ReorderFmt | 10 |
| `CHT` | ChartCandAtoms · ChartCandFmt · Charts · ChartsFmt | 36 |
| `DAT` | DataAtoms · DataCandAtoms · DataCandFmt · DataExAtoms · DataExFmt · DataFmt | 49 |
| `DOM` | DomainCandAtoms · DomainCandFmt | 7 |
| `FBK` | FeedbackAtoms · FeedbackAtomsB · FeedbackCandAtoms · FeedbackCandFmt · FeedbackFmt | 25 |
| `IND` | Industrial · IndustrialFmt | 15 |
| `INP` | InputAtoms · InputCandAtoms · InputCandFmt · InputExAtoms · InputExFmt · InputFmt · InputPlusAtoms · InputPlusFmt | 66 |
| `INT` | InteractAtoms · InteractFmt | 7 |
| `LAY` | LayoutCandAtoms | 1 |
| `MSC` | MiscAtoms · MiscFmt | 8 |
| `NAV` | CommandPalette · NavAtoms · NavAtomsB · NavCandAtoms · NavCandFmt · NavFmt | 33 |
| `OFF` | OfficialDataAtoms · OfficialDataAtomsB · OfficialDataAtomsC · OfficialFmt | 23 |
| `PNL` | PanelAtoms · PanelFmt | 6 |
| `PRG` | ProgressAtoms · ProgressFmt | 10 |
| `STS` | StatusAtoms · StatusFmt | 11 |
| `TBL` | TableAtoms · TableCandAtoms · TableFmt | 13 |
| `TXT` | TextAtoms · TextFmt | 7 |
| `VIS` | VisualAtoms · VisualsFmt | 7 |
| `WAS` | Api · Atoms · Draw · Layout · Measure · Px · Shapes · TexAtoms · TexCache · TexFmt · Tokens | 67 |

## 计数总览

| 页签 | 组件 | 候选 | 工具 | 小计 |
|---|---:|---:|---:|---:|
| **原子** | 36 | 1 | 32 | 69 |
| **输入** | 29 | 3 | 34 | 66 |
| **数据** | 21 | 6 | 22 | 49 |
| **图表** | 10 | 13 | 13 | 36 |
| **反馈** | 15 | 2 | 8 | 25 |
| **导航** | 13 | 5 | 15 | 33 |
| **工业与官方** | 22 | 4 | 19 | 45 |
| **桥接** | 7 | 1 | 3 | 11 |
| **基座（非组件）** | 55 | 0 | 12 | 67 |
| **合计** | **208** | **35** | **162** | **405** |

## 页签「原子」

| 编号 | 组件 | 返回 | 所在类 | 类型 |
|---|---|---|---|---|
| `INT-01` | EyeToggle | `bool` | InteractAtoms | 组件 |
| `INT-02` | GripHandle | `bool` | InteractAtoms | 组件 |
| `INT-03` | SearchHighlight | `void` | InteractAtoms | 组件 |
| `INT-04` | ToolbarIcon | `bool` | InteractAtoms | 组件 |
| `INT-05` | EyeRect | `Rect` | InteractFmt | 工具 |
| `INT-06` | GripDotSize | `void` | InteractFmt | 工具 |
| `INT-07` | HighlightSegmentCount | `int` | InteractFmt | 工具 |
| `MSC-01` | BubbleTag | `void` | MiscAtoms | 组件 |
| `MSC-02` | PointCostBadge | `void` | MiscAtoms | 组件 |
| `MSC-03` | ThinScrollBar | `void` | MiscAtoms | 组件 |
| `MSC-04` | BubbleTail | `Rect` | MiscFmt | 工具 |
| `MSC-05` | PointsOverdrawn | `bool` | MiscFmt | 工具 |
| `MSC-06` | PointsRemaining | `int` | MiscFmt | 工具 |
| `MSC-07` | ScrollThumbHeight | `float` | MiscFmt | 工具 |
| `MSC-08` | ScrollThumbY | `float` | MiscFmt | 工具 |
| `PNL-01` | FlashOverlay | `void` | PanelAtoms | 组件 |
| `PNL-02` | MiniProfileChip | `void` | PanelAtoms | 组件 |
| `PNL-03` | SectionBar | `void` | PanelAtoms | 组件 |
| `PNL-04` | StatRow | `void` | PanelAtoms | 组件 |
| `PNL-05` | FlashAlpha | `float` | PanelFmt | 工具 |
| `PNL-06` | RowSplit | `void` | PanelFmt | 工具 |
| `PRG-01` | CapacityBar | `void` | ProgressAtoms | 组件 |
| `PRG-02` | FillWipe | `void` | ProgressAtoms | 组件 |
| `PRG-03` | ProgressRing | `void` | ProgressAtoms | 组件 |
| `PRG-04` | ZoomIndicator | `bool` | ProgressAtoms | 组件 |
| `PRG-05` | CapacityRatio | `float` | ProgressFmt | 工具 |
| `PRG-06` | OverflowRatio | `float` | ProgressFmt | 工具 |
| `PRG-07` | RingFilledSegments | `int` | ProgressFmt | 工具 |
| `PRG-08` | RingSegments | `int` | ProgressFmt | 工具 |
| `PRG-09` | WipeWidth | `float` | ProgressFmt | 工具 |
| `PRG-10` | ZoomLabel | `string` | ProgressFmt | 工具 |
| `STS-01` | Caret | `void` | StatusAtoms | 组件 |
| `STS-02` | Caret | `void` | StatusAtoms | 组件 |
| `STS-03` | Glyph | `void` | StatusAtoms | 组件 |
| `STS-04` | PulseDot | `void` | StatusAtoms | 组件 |
| `STS-05` | ScrollFadeHint | `void` | StatusAtoms | 组件 |
| `STS-06` | Spinner | `void` | StatusAtoms | 组件 |
| `STS-07` | Spinner | `void` | StatusAtoms | 组件 |
| `STS-08` | DotRect | `Rect` | StatusFmt | 工具 |
| `STS-09` | GlyphOf | `void` | StatusFmt | 工具 |
| `STS-10` | PulseAlpha | `float` | StatusFmt | 工具 |
| `STS-11` | TrailAlpha | `float` | StatusFmt | 工具 |
| `TBL-01` | DensitySwitch | `bool` | TableAtoms | 组件 |
| `TBL-02` | FrozenSplit | `void` | TableAtoms | 组件 |
| `TBL-03` | HeaderCell | `bool` | TableAtoms | 组件 |
| `TBL-04` | ResizeHandle | `bool` | TableAtoms | 组件 |
| `TBL-05` | ValueCell | `void` | TableAtoms | 组件 |
| `TBL-06` | PawnTableFrame | `void` | TableCandAtoms | 候选 |
| `TBL-07` | BandColor | `Color` | TableFmt | 工具 |
| `TBL-08` | ColumnSplit | `void` | TableFmt | 工具 |
| `TBL-09` | FrozenContentOffsetX | `float` | TableFmt | 工具 |
| `TBL-10` | NextSort | `SortState` | TableFmt | 工具 |
| `TBL-11` | ResizeWidth | `float` | TableFmt | 工具 |
| `TBL-12` | RowHeight | `float` | TableFmt | 工具 |
| `TBL-13` | ValueRatio | `float` | TableFmt | 工具 |
| `TXT-01` | DateFull | `string` | TextAtoms | 组件 |
| `TXT-02` | RichTextLine | `float` | TextAtoms | 组件 |
| `TXT-03` | SubtitleLine | `void` | TextAtoms | 组件 |
| `TXT-04` | Typewriter | `void` | TextAtoms | 组件 |
| `TXT-05` | UnitLabel | `void` | TextAtoms | 组件 |
| `TXT-06` | FormatNumber | `string` | TextFmt | 工具 |
| `TXT-07` | TypewriterChars | `int` | TextFmt | 工具 |
| `VIS-01` | GradientBar | `void` | VisualAtoms | 组件 |
| `VIS-02` | OverflowFade | `void` | VisualAtoms | 组件 |
| `VIS-03` | PinBadge | `void` | VisualAtoms | 组件 |
| `VIS-04` | Swatch | `void` | VisualAtoms | 组件 |
| `VIS-05` | FadeAlpha | `float` | VisualsFmt | 工具 |
| `VIS-06` | LerpT | `float` | VisualsFmt | 工具 |
| `VIS-07` | PinRect | `Rect` | VisualsFmt | 工具 |

## 页签「输入」

| 编号 | 组件 | 返回 | 所在类 | 类型 |
|---|---|---|---|---|
| `INP-01` | HotkeyBind | `bool` | InputAtoms | 组件 |
| `INP-02` | SearchField | `bool` | InputAtoms | 组件 |
| `INP-03` | Segmented | `bool` | InputAtoms | 组件 |
| `INP-04` | SliderWithValue | `bool` | InputAtoms | 组件 |
| `INP-05` | StateSample | `void` | InputAtoms | 组件 |
| `INP-06` | Stepper | `bool` | InputAtoms | 组件 |
| `INP-07` | TagPicker | `bool` | InputAtoms | 组件 |
| `INP-08` | ToggleSwitch | `bool` | InputAtoms | 组件 |
| `INP-09` | ColorPicker | `bool` | InputExAtoms | 组件 |
| `INP-10` | CycleButton | `bool` | InputExAtoms | 组件 |
| `INP-11` | DoubleSlider | `bool` | InputExAtoms | 组件 |
| `INP-12` | DragListRow | `void` | InputExAtoms | 组件 |
| `INP-13` | EditableLabel | `bool` | InputExAtoms | 组件 |
| `INP-14` | GripHit | `bool` | InputExAtoms | 组件 |
| `INP-15` | NumberScrubber | `void` | InputExAtoms | 组件 |
| `INP-16` | PriorityGrid | `bool` | InputExAtoms | 组件 |
| `INP-17` | TimetableMatrix | `bool` | InputExAtoms | 组件 |
| `INP-18` | FilterRow | `bool` | InputPlusAtoms | 组件 |
| `INP-19` | InPlaceDropdown | `bool` | InputPlusAtoms | 组件 |
| `INP-20` | KeyConflictHint | `bool` | InputPlusAtoms | 组件 |
| `INP-21` | MultiLineEditor | `bool` | InputPlusAtoms | 组件 |
| `INP-22` | MultiSelectList | `bool` | InputPlusAtoms | 组件 |
| `INP-23` | NumericField | `bool` | InputPlusAtoms | 组件 |
| `INP-24` | PaintStrokeGrid | `bool` | InputPlusAtoms | 组件 |
| `INP-25` | PathField | `bool` | InputPlusAtoms | 组件 |
| `INP-26` | QueueEditor | `bool` | InputPlusAtoms | 组件 |
| `INP-27` | QuickAmounts | `bool` | InputPlusAtoms | 组件 |
| `INP-28` | SliderTicks | `bool` | InputPlusAtoms | 组件 |
| `INP-29` | WheelAdjust | `bool` | InputPlusAtoms | 组件 |
| `INP-30` | AreaEditor | `void` | InputCandAtoms | 候选 |
| `INP-31` | BatchSliders | `void` | InputCandAtoms | 候选 |
| `INP-32` | KeyBindPanel | `void` | InputCandAtoms | 候选 |
| `INP-33` | AreaRowTone | `Color` | InputCandFmt | 工具 |
| `INP-34` | KeybindSlotTone | `Color` | InputCandFmt | 工具 |
| `INP-35` | SliderValueFromPos | `float` | InputCandFmt | 工具 |
| `INP-36` | DropIndex | `int` | InputExFmt | 工具 |
| `INP-37` | Hex | `string` | InputExFmt | 工具 |
| `INP-38` | HsvColor | `Color` | InputExFmt | 工具 |
| `INP-39` | NextIndex | `int` | InputExFmt | 工具 |
| `INP-40` | NextPriority | `int` | InputExFmt | 工具 |
| `INP-41` | NextTimetableState | `int` | InputExFmt | 工具 |
| `INP-42` | Reorder | `void` | InputExFmt | 工具 |
| `INP-43` | RgbToHsv | `void` | InputExFmt | 工具 |
| `INP-44` | ScrubValue | `float` | InputExFmt | 工具 |
| `INP-45` | DebounceReady | `bool` | InputFmt | 工具 |
| `INP-46` | Resolve | `InputState` | InputFmt | 工具 |
| `INP-47` | SegmentIndex | `int` | InputFmt | 工具 |
| `INP-48` | SegmentRect | `Rect` | InputFmt | 工具 |
| `INP-49` | Slider01 | `float` | InputFmt | 工具 |
| `INP-50` | SliderValue | `float` | InputFmt | 工具 |
| `INP-51` | StepValue | `int` | InputFmt | 工具 |
| `INP-52` | ConflictCount | `int` | InputPlusFmt | 工具 |
| `INP-53` | EnqueueInt | `int` | InputPlusFmt | 工具 |
| `INP-54` | HasConflict | `bool` | InputPlusFmt | 工具 |
| `INP-55` | IsMax | `bool` | InputPlusFmt | 工具 |
| `INP-56` | IsValidPath | `bool` | InputPlusFmt | 工具 |
| `INP-57` | MoveToFrontInt | `void` | InputPlusFmt | 工具 |
| `INP-58` | NormalizePath | `string` | InputPlusFmt | 工具 |
| `INP-59` | QuickAmount | `float` | InputPlusFmt | 工具 |
| `INP-60` | RemoveAtInt | `bool` | InputPlusFmt | 工具 |
| `INP-61` | SelectRange | `void` | InputPlusFmt | 工具 |
| `INP-62` | SnapToTicks | `float` | InputPlusFmt | 工具 |
| `INP-63` | SplitLines | `string[]` | InputPlusFmt | 工具 |
| `INP-64` | ToggleAt | `bool` | InputPlusFmt | 工具 |
| `INP-65` | TryParseClamp | `bool` | InputPlusFmt | 工具 |
| `INP-66` | WheelStep | `float` | InputPlusFmt | 工具 |

## 页签「数据」

| 编号 | 组件 | 返回 | 所在类 | 类型 |
|---|---|---|---|---|
| `DAT-01` | HealthRows | `void` | DataAtoms | 组件 |
| `DAT-02` | HeatCalendar | `void` | DataAtoms | 组件 |
| `DAT-03` | NeedsPanel | `void` | DataAtoms | 组件 |
| `DAT-04` | PawnCard | `void` | DataAtoms | 组件 |
| `DAT-05` | RankList | `void` | DataAtoms | 组件 |
| `DAT-06` | SkillBars | `void` | DataAtoms | 组件 |
| `DAT-07` | StatTable | `void` | DataAtoms | 组件 |
| `DAT-08` | TimelinePro | `void` | DataAtoms | 组件 |
| `DAT-09` | TraitChips | `void` | DataAtoms | 组件 |
| `DAT-10` | BuffRow | `void` | DataExAtoms | 组件 |
| `DAT-11` | CompareBars | `void` | DataExAtoms | 组件 |
| `DAT-12` | CreatureCard | `void` | DataExAtoms | 组件 |
| `DAT-13` | DockHost | `int` | DataExAtoms | 组件 |
| `DAT-14` | FactionCard | `void` | DataExAtoms | 组件 |
| `DAT-15` | NodeGraph | `bool` | DataExAtoms | 组件 |
| `DAT-16` | PowerGrid | `void` | DataExAtoms | 组件 |
| `DAT-17` | ProfilePreset | `bool` | DataExAtoms | 组件 |
| `DAT-18` | QuestCard | `void` | DataExAtoms | 组件 |
| `DAT-19` | RoomStats | `void` | DataExAtoms | 组件 |
| `DAT-20` | StorageBreakdown | `void` | DataExAtoms | 组件 |
| `DAT-21` | WeatherStrip | `void` | DataExAtoms | 组件 |
| `DAT-22` | BillList | `void` | DataCandAtoms | 候选 |
| `DAT-23` | InfoCard | `void` | DataCandAtoms | 候选 |
| `DAT-24` | ResourceReadout | `void` | DataCandAtoms | 候选 |
| `DAT-25` | StorageContents | `void` | DataCandAtoms | 候选 |
| `DAT-26` | ThingFilterTree | `void` | DataCandAtoms | 候选 |
| `DAT-27` | TradePanel | `void` | DataCandAtoms | 候选 |
| `DAT-28` | BillRatio | `float` | DataCandFmt | 工具 |
| `DAT-29` | CountTone | `Color` | DataCandFmt | 工具 |
| `DAT-30` | TradeTone | `Color` | DataCandFmt | 工具 |
| `DAT-31` | TreeRow | `Rect` | DataCandFmt | 工具 |
| `DAT-32` | CompareWinner | `int` | DataExFmt | 工具 |
| `DAT-33` | DockZone | `int` | DataExFmt | 工具 |
| `DAT-34` | LinkMid | `Vector2` | DataExFmt | 工具 |
| `DAT-35` | NetBalance | `float` | DataExFmt | 工具 |
| `DAT-36` | NodeRect | `Rect` | DataExFmt | 工具 |
| `DAT-37` | PresetIndex | `int` | DataExFmt | 工具 |
| `DAT-38` | Ratio | `float` | DataExFmt | 工具 |
| `DAT-39` | RelationTone | `Color` | DataExFmt | 工具 |
| `DAT-40` | Share | `float` | DataExFmt | 工具 |
| `DAT-41` | ShareRatios | `void` | DataExFmt | 工具 |
| `DAT-42` | Sum | `float` | DataExFmt | 工具 |
| `DAT-43` | TrainingTone | `Color` | DataExFmt | 工具 |
| `DAT-44` | WeatherTone | `Color` | DataExFmt | 工具 |
| `DAT-45` | Bar01 | `float` | DataFmt | 工具 |
| `DAT-46` | HeatLevel | `int` | DataFmt | 工具 |
| `DAT-47` | RankMedal | `string` | DataFmt | 工具 |
| `DAT-48` | RowRect | `Rect` | DataFmt | 工具 |
| `DAT-49` | SkillColor | `Color` | DataFmt | 工具 |

## 页签「图表」

| 编号 | 组件 | 返回 | 所在类 | 类型 |
|---|---|---|---|---|
| `CHT-01` | AreaChart | `void` | Charts | 组件 |
| `CHT-02` | BarChartH | `void` | Charts | 组件 |
| `CHT-03` | BarChartV | `void` | Charts | 组件 |
| `CHT-04` | DonutChart | `void` | Charts | 组件 |
| `CHT-05` | HeatmapGrid | `void` | Charts | 组件 |
| `CHT-06` | LineChart | `void` | Charts | 组件 |
| `CHT-07` | PieChart | `void` | Charts | 组件 |
| `CHT-08` | ScatterChart | `void` | Charts | 组件 |
| `CHT-09` | Sparkline | `void` | Charts | 组件 |
| `CHT-10` | StackedBarChartV | `void` | Charts | 组件 |
| `CHT-11` | BoxPlot | `void` | ChartCandAtoms | 候选 |
| `CHT-12` | Bubble | `void` | ChartCandAtoms | 候选 |
| `CHT-13` | Funnel | `void` | ChartCandAtoms | 候选 |
| `CHT-14` | Gantt | `void` | ChartCandAtoms | 候选 |
| `CHT-15` | Histogram | `void` | ChartCandAtoms | 候选 |
| `CHT-16` | Ohlc | `void` | ChartCandAtoms | 候选 |
| `CHT-17` | Pareto | `void` | ChartCandAtoms | 候选 |
| `CHT-18` | Radar | `void` | ChartCandAtoms | 候选 |
| `CHT-19` | RangeArea | `void` | ChartCandAtoms | 候选 |
| `CHT-20` | Sankey | `void` | ChartCandAtoms | 候选 |
| `CHT-21` | Spc | `void` | ChartCandAtoms | 候选 |
| `CHT-22` | StepLine | `void` | ChartCandAtoms | 候选 |
| `CHT-23` | Waterfall | `void` | ChartCandAtoms | 候选 |
| `CHT-24` | BandColor | `Color` | ChartCandFmt | 工具 |
| `CHT-25` | Cumulative | `void` | ChartCandFmt | 工具 |
| `CHT-26` | Max | `float` | ChartCandFmt | 工具 |
| `CHT-27` | Min | `float` | ChartCandFmt | 工具 |
| `CHT-28` | SeriesColor | `Color` | ChartCandFmt | 工具 |
| `CHT-29` | BarGeom | `void` | ChartsFmt | 工具 |
| `CHT-30` | HeatColor | `Color` | ChartsFmt | 工具 |
| `CHT-31` | HistogramBin | `int` | ChartsFmt | 工具 |
| `CHT-32` | NiceTicks | `int` | ChartsFmt | 工具 |
| `CHT-33` | Scale | `float` | ChartsFmt | 工具 |
| `CHT-34` | SeriesMax | `float` | ChartsFmt | 工具 |
| `CHT-35` | SeriesRange | `void` | ChartsFmt | 工具 |
| `CHT-36` | SweepDeg | `float` | ChartsFmt | 工具 |

## 页签「反馈」

| 编号 | 组件 | 返回 | 所在类 | 类型 |
|---|---|---|---|---|
| `FBK-01` | AlertBanner | `bool` | FeedbackAtoms | 组件 |
| `FBK-02` | ConfirmDialog | `bool` | FeedbackAtoms | 组件 |
| `FBK-03` | EmptyState | `bool` | FeedbackAtoms | 组件 |
| `FBK-04` | ProgressOverlay | `void` | FeedbackAtoms | 组件 |
| `FBK-05` | RichTooltip | `void` | FeedbackAtoms | 组件 |
| `FBK-06` | Toast | `void` | FeedbackAtoms | 组件 |
| `FBK-07` | ActionFeedback | `void` | FeedbackAtomsB | 组件 |
| `FBK-08` | AlertBlinker | `void` | FeedbackAtomsB | 组件 |
| `FBK-09` | BatchConfirm | `bool` | FeedbackAtomsB | 组件 |
| `FBK-10` | CriticalOverlay | `bool` | FeedbackAtomsB | 组件 |
| `FBK-11` | InlineError | `bool` | FeedbackAtomsB | 组件 |
| `FBK-12` | NotificationEntry | `bool` | FeedbackAtomsB | 组件 |
| `FBK-13` | StepProgressList | `void` | FeedbackAtomsB | 组件 |
| `FBK-14` | StickyNote | `void` | FeedbackAtomsB | 组件 |
| `FBK-15` | UndoToast | `bool` | FeedbackAtomsB | 组件 |
| `FBK-16` | AlertTray | `void` | FeedbackCandAtoms | 候选 |
| `FBK-17` | FloatMenu | `int` | FeedbackCandAtoms | 候选 |
| `FBK-18` | AlertSeverityTone | `Color` | FeedbackCandFmt | 工具 |
| `FBK-19` | FloatMenuRowTone | `Color` | FeedbackCandFmt | 工具 |
| `FBK-20` | BatchSummary | `string` | FeedbackFmt | 工具 |
| `FBK-21` | BlinkAlpha | `float` | FeedbackFmt | 工具 |
| `FBK-22` | MergeHidden | `int` | FeedbackFmt | 工具 |
| `FBK-23` | MergeVisible | `int` | FeedbackFmt | 工具 |
| `FBK-24` | StepMark | `string` | FeedbackFmt | 工具 |
| `FBK-25` | ToastAlpha | `float` | FeedbackFmt | 工具 |

## 页签「导航」

| 编号 | 组件 | 返回 | 所在类 | 类型 |
|---|---|---|---|---|
| `NAV-01` | Draw | `Command` | CommandPalette | 组件 |
| `NAV-02` | Filter | `int` | CommandPalette | 组件 |
| `NAV-03` | AnchorDots | `void` | NavAtoms | 组件 |
| `NAV-04` | Breadcrumb | `void` | NavAtoms | 组件 |
| `NAV-05` | CollapsibleTree | `TreeNode` | NavAtoms | 组件 |
| `NAV-06` | Pager | `void` | NavAtoms | 组件 |
| `NAV-07` | TabBar | `void` | NavAtoms | 组件 |
| `NAV-08` | FocusSearch | `bool` | NavAtomsB | 组件 |
| `NAV-09` | JumpList | `int` | NavAtomsB | 组件 |
| `NAV-10` | LayerSwitch | `void` | NavAtomsB | 组件 |
| `NAV-11` | NavBackBar | `NavAction` | NavAtomsB | 组件 |
| `NAV-12` | SidebarCollapse | `void` | NavAtomsB | 组件 |
| `NAV-13` | TimeSpeedControl | `void` | NavAtomsB | 组件 |
| `NAV-14` | InspectTabs | `void` | NavCandAtoms | 候选 |
| `NAV-15` | MainTabStrip | `void` | NavCandAtoms | 候选 |
| `NAV-16` | PolicyList | `void` | NavCandAtoms | 候选 |
| `NAV-17` | SearchBox | `void` | NavCandAtoms | 候选 |
| `NAV-18` | WizardSteps | `void` | NavCandAtoms | 候选 |
| `NAV-19` | MainTabTone | `Color` | NavCandFmt | 工具 |
| `NAV-20` | PolicyRowTone | `Color` | NavCandFmt | 工具 |
| `NAV-21` | WizardConnectorTone | `Color` | NavCandFmt | 工具 |
| `NAV-22` | WizardNumberTone | `Color` | NavCandFmt | 工具 |
| `NAV-23` | WizardStepTone | `Color` | NavCandFmt | 工具 |
| `NAV-24` | Approach | `float` | NavFmt | 工具 |
| `NAV-25` | ClampPage | `int` | NavFmt | 工具 |
| `NAV-26` | CountNodes | `int` | NavFmt | 工具 |
| `NAV-27` | FuzzyScore | `int` | NavFmt | 工具 |
| `NAV-28` | LabelUnits | `int` | NavFmt | 工具 |
| `NAV-29` | PageList | `void` | NavFmt | 工具 |
| `NAV-30` | RelativeTime | `string` | NavFmt | 工具 |
| `NAV-31` | TabWidth | `float` | NavFmt | 工具 |
| `NAV-32` | TimeBucketOf | `TimeBucket` | NavFmt | 工具 |
| `NAV-33` | TotalPages | `int` | NavFmt | 工具 |

## 页签「工业与官方」

| 编号 | 组件 | 返回 | 所在类 | 类型 |
|---|---|---|---|---|
| `DOM-01` | CircuitGraph | `void` | DomainCandAtoms | 候选 |
| `DOM-02` | DataBridgePanel | `void` | DomainCandAtoms | 候选 |
| `DOM-03` | GridMap | `void` | DomainCandAtoms | 候选 |
| `DOM-04` | ProductionFlow | `void` | DomainCandAtoms | 候选 |
| `DOM-05` | CellTone | `Color` | DomainCandFmt | 工具 |
| `DOM-06` | NodeKindColor | `Color` | DomainCandFmt | 工具 |
| `DOM-07` | ProgressTone | `Color` | DomainCandFmt | 工具 |
| `IND-01` | BottleneckRibbon | `void` | Industrial | 组件 |
| `IND-02` | Gauge | `void` | Industrial | 组件 |
| `IND-03` | GaugeNeedleDeg | `float` | Industrial | 组件 |
| `IND-04` | LedColor | `Color` | Industrial | 组件 |
| `IND-05` | LedMatrix | `void` | Industrial | 组件 |
| `IND-06` | MachineStateTimeline | `void` | Industrial | 组件 |
| `IND-07` | StateColor | `Color` | Industrial | 组件 |
| `IND-08` | ZoneColor | `Color` | Industrial | 组件 |
| `IND-09` | BottleneckIndex | `int` | IndustrialFmt | 工具 |
| `IND-10` | GaugeNorm | `float` | IndustrialFmt | 工具 |
| `IND-11` | GaugeZone | `int` | IndustrialFmt | 工具 |
| `IND-12` | LedCell | `Rect` | IndustrialFmt | 工具 |
| `IND-13` | MergeRunLengths | `int` | IndustrialFmt | 工具 |
| `IND-14` | SegmentsTotal | `float` | IndustrialFmt | 工具 |
| `IND-15` | Shares | `int` | IndustrialFmt | 工具 |
| `OFF-01` | HistoryFeed | `void` | OfficialDataAtoms | 组件 |
| `OFF-02` | RecordsTable | `void` | OfficialDataAtoms | 组件 |
| `OFF-03` | TaleCard | `void` | OfficialDataAtoms | 组件 |
| `OFF-04` | WealthBreakdown | `void` | OfficialDataAtoms | 组件 |
| `OFF-05` | BillList | `void` | OfficialDataAtomsB | 组件 |
| `OFF-06` | LetterCard | `void` | OfficialDataAtomsB | 组件 |
| `OFF-07` | PsycastTree | `void` | OfficialDataAtomsB | 组件 |
| `OFF-08` | QuestBoard | `void` | OfficialDataAtomsB | 组件 |
| `OFF-09` | TitleCard | `void` | OfficialDataAtomsB | 组件 |
| `OFF-10` | CaravanPanel | `void` | OfficialDataAtomsC | 组件 |
| `OFF-11` | GeneChips | `void` | OfficialDataAtomsC | 组件 |
| `OFF-12` | IdeoCard | `void` | OfficialDataAtomsC | 组件 |
| `OFF-13` | MessageFeed | `void` | OfficialDataAtomsC | 组件 |
| `OFF-14` | PolicyCard | `void` | OfficialDataAtomsC | 组件 |
| `OFF-15` | BillRatio | `float` | OfficialFmt | 工具 |
| `OFF-16` | CheckColor | `Color` | OfficialFmt | 工具 |
| `OFF-17` | ComplexityLabel | `string` | OfficialFmt | 工具 |
| `OFF-18` | EtaLabel | `string` | OfficialFmt | 工具 |
| `OFF-19` | HexWithHash | `string` | OfficialFmt | 工具 |
| `OFF-20` | RelationBandOf | `RelationBand` | OfficialFmt | 工具 |
| `OFF-21` | RelationColor | `Color` | OfficialFmt | 工具 |
| `OFF-22` | RelationLabel | `string` | OfficialFmt | 工具 |
| `OFF-23` | WealthTotal | `float` | OfficialFmt | 工具 |

## 页签「桥接」

| 编号 | 组件 | 返回 | 所在类 | 类型 |
|---|---|---|---|---|
| `BRG-01` | Opportunity | `void` | HiAtoms | 组件 |
| `BRG-02` | Tag | `void` | HiAtoms | 组件 |
| `BRG-03` | CurrentMapColonists | `IEnumerable<Pawn>` | PawnTableAtoms | 组件 |
| `BRG-04` | CycleSort | `string` | PawnTableAtoms | 组件 |
| `BRG-05` | PawnTableFrame | `bool` | PawnTableAtoms | 组件 |
| `BRG-06` | Grip | `void` | ReorderAtoms | 组件 |
| `BRG-07` | List | `bool` | ReorderAtoms | 组件 |
| `BRG-08` | LandingIndex | `int` | ReorderFmt | 工具 |
| `BRG-09` | Moved | `int[]` | ReorderFmt | 工具 |
| `BRG-10` | WillChange | `bool` | ReorderFmt | 工具 |
| `LAY-01` | CockpitFrame | `void` | LayoutCandAtoms | 候选 |

## 页签「基座（非组件）」

| 编号 | 组件 | 返回 | 所在类 | 类型 |
|---|---|---|---|---|
| `WAS-01` | IsAvailable | `bool` | Api | 组件 |
| `WAS-02` | Anchor | `Rect` | Atoms | 组件 |
| `WAS-03` | AspectBox | `Rect` | Atoms | 组件 |
| `WAS-04` | SectionFrame | `Rect` | Atoms | 组件 |
| `WAS-05` | Anchor | `AnchorScope` | Draw | 组件 |
| `WAS-06` | Atlas | `void` | Draw | 组件 |
| `WAS-07` | Atlas | `void` | Draw | 组件 |
| `WAS-08` | Border | `void` | Draw | 组件 |
| `WAS-09` | Box | `void` | Draw | 组件 |
| `WAS-10` | Color | `ColorScope` | Draw | 组件 |
| `WAS-11` | EllipsisLabel | `void` | Draw | 组件 |
| `WAS-12` | Enabled | `EnabledScope` | Draw | 组件 |
| `WAS-13` | Font | `FontScope` | Draw | 组件 |
| `WAS-14` | Label | `void` | Draw | 组件 |
| `WAS-15` | LineH | `void` | Draw | 组件 |
| `WAS-16` | LineV | `void` | Draw | 组件 |
| `WAS-17` | Rotate | `RotateScope` | Draw | 组件 |
| `WAS-18` | WithAlpha | `Color` | Draw | 组件 |
| `WAS-19` | BeginScroll | `Rect` | Layout | 组件 |
| `WAS-20` | Column | `VStack` | Layout | 组件 |
| `WAS-21` | EndScroll | `void` | Layout | 组件 |
| `WAS-22` | Grid | `Grid` | Layout | 组件 |
| `WAS-23` | Inset | `Rect` | Layout | 组件 |
| `WAS-24` | Row | `HStack` | Layout | 组件 |
| `WAS-25` | Split | `void` | Layout | 组件 |
| `WAS-26` | Clamp | `string` | Measure | 组件 |
| `WAS-27` | ControlHeight | `float` | Measure | 组件 |
| `WAS-28` | Fits | `bool` | Measure | 组件 |
| `WAS-29` | RowHeight | `float` | Measure | 组件 |
| `WAS-30` | RowHeight | `float` | Measure | 组件 |
| `WAS-31` | Width | `float` | Measure | 组件 |
| `WAS-32` | Ceil | `float` | Px | 组件 |
| `WAS-33` | Floor | `float` | Px | 组件 |
| `WAS-34` | Floor | `Vector2` | Px | 组件 |
| `WAS-35` | HLine | `Rect` | Px | 组件 |
| `WAS-36` | Snap | `Rect` | Px | 组件 |
| `WAS-37` | Span | `float` | Px | 组件 |
| `WAS-38` | VLine | `Rect` | Px | 组件 |
| `WAS-39` | CornerFrame | `void` | Shapes | 组件 |
| `WAS-40` | GradientRule | `void` | Shapes | 组件 |
| `WAS-41` | InsetPanel | `void` | Shapes | 组件 |
| `WAS-42` | RuleWithLabel | `void` | Shapes | 组件 |
| `WAS-43` | Watermark | `void` | Shapes | 组件 |
| `WAS-44` | Canvas | `bool` | TexAtoms | 组件 |
| `WAS-45` | FadeLeft | `void` | TexAtoms | 组件 |
| `WAS-46` | FadeRight | `void` | TexAtoms | 组件 |
| `WAS-47` | GradientRect | `void` | TexAtoms | 组件 |
| `WAS-48` | Heat | `void` | TexAtoms | 组件 |
| `WAS-49` | Sector | `void` | TexAtoms | 组件 |
| `WAS-50` | Clear | `void` | TexCache | 组件 |
| `WAS-51` | Key | `long` | TexCache | 组件 |
| `WAS-52` | KeyVersioned | `long` | TexCache | 组件 |
| `WAS-53` | Publish | `Texture2D` | TexCache | 组件 |
| `WAS-54` | TryGet | `bool` | TexCache | 组件 |
| `WAS-55` | Series | `Color` | Tokens | 组件 |
| `WAS-56` | AlphaFade | `Color32[]` | TexFmt | 工具 |
| `WAS-57` | Blend | `Color32` | TexFmt | 工具 |
| `WAS-58` | Coverage | `float` | TexFmt | 工具 |
| `WAS-59` | FillDisc | `void` | TexFmt | 工具 |
| `WAS-60` | FillRect | `void` | TexFmt | 工具 |
| `WAS-61` | FillSector | `void` | TexFmt | 工具 |
| `WAS-62` | Heat | `Color` | TexFmt | 工具 |
| `WAS-63` | HGradient | `Color32[]` | TexFmt | 工具 |
| `WAS-64` | Index | `int` | TexFmt | 工具 |
| `WAS-65` | Ramp | `Color` | TexFmt | 工具 |
| `WAS-66` | Solid | `Color32[]` | TexFmt | 工具 |
| `WAS-67` | VGradient | `Color32[]` | TexFmt | 工具 |

