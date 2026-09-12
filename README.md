# RUIF — RimWorld UI Framework（独立 UI 框架 Mod）

> 状态：**纯组件库**（只提供可复用 UI 组件与固定设计令牌；**不含主题 / 换肤机制**），含 `Assemblies/` · `Languages/` · `DevDocs/`，发布区 `Mods\RUIF`
> 更新：2026-09-12（**移除主题 / 换肤层及其全部公共 API**，框架回归纯组件供应）

## 1. 定位

**通用的、面向全体 RimWorld Mod 开发者的 UI 框架**：以独立框架 Mod 形态分发（与 Harmony / HugsLib 同模式，全游戏只加载一份 RUIF.dll），任何 Mod 引用即可获得统一组件与声明式布局——Mod 作者只声明「要什么组件、什么数据」，不碰 `Rect` 布局数学与 `GUI.color` 配对。

**范式定调**：即时模式 + 轻量布局助手（Layout 算一次 Rect + struct 游标复用），**不走**保留模式 UI Tree 路线。

**纯组件供应**：框架只提供能力，不内置任何界面，也**不提供换肤**——`Tokens` 是一套固定、只读的设计令牌；
没有风格预设、没有装饰层、没有运行期换色、没有 XML 主题配置。需要不同视觉风格的消费方请自行包装。

## 2. 当前状态（2026-09-12）

| 维度 | 实测值 |
|---|---|
| 版本 | `Api.VersionString` ↔ `About/About.xml` modVersion（当前 **v1.0.1**，2026-09-12；变更全部记入 `CHANGELOG.md`） |
| 编译 | `dotnet build Source/RUIF.csproj -c Release` → **0 错 0 警** |
| 单测 | `dotnet test Tests/RUIF.Tests.csproj -c Release` → **171/171** |
| 组件 | 原子 42 · 输入 28 · 数据 40 · 导航 12 · 反馈 17 · 图表 10 · 工业 4 |
| 设计令牌 | `RUIF.Tokens`：16 色 + 间距 + 字号 + 尺寸，**编译期常量 / 只读静态值，运行期不可变** |
| 主题 / 换肤 | **无**（已移除 `Theme` 引擎、`ThemePresets`、`ThemeDecor`、`DrawDecor`、`ThemeMotion`、`VanillaUiSkin`、`RUIFSettings`） |
| 本地化 | **框架源码零硬编码界面文案**：组件 60 键，English + 简体中文随包 |
| 绘制路径翻译 | `Core/Tr.cs` → 有界缓存 `Core/TrCache.cs`：命中即零字典查找 / 零分配，语言切换自动失效（容量 512） |
| 程序集引用 | 仅 `mscorlib` / `System.Core` / `Assembly-CSharp` / `UnityEngine.{Core,IMGUI,TextRendering}Module` —— **不含 0Harmony** |
| 运行时副作用 | **无 Def 界面入口** · 无存档字段 · 组件无持久状态 |
| 公共 API | 见 `DevDocs/API参考.md`（**生成物，需在下次发布前重新跑生成器**） |

## 3. 分层与目录

```text
RUIF/
├── About/About.xml                  Mod 元数据（packageId = PersonalChronicle.RUIFramework）
├── Assemblies/                      RUIF.dll / RUIF.pdb / RUIF.xml（IDE 悬停文档）
├── Languages/                       English · ChineseSimplified（Keyed）
├── Source/
│   ├── Core/                        L0 Draw 作用域 / Api（版本门面）/ Tokens（设计令牌）/ Tr + TrCache
│   ├── Layout/                      L4 VStack · HStack · Grid · Scroll + Atoms
│   └── Components/                  L2/L3 原子与复合组件
├── Tests/                           NUnit 单测（net48）
├── DevDocs/                         【生成】对外开发者包（随包分发）
├── _archive/                        【归档·不编译】v1.x 组件演示源码（不随包发布）
├── _scripts/                        见 §5
└── docs/                            见 §4
```

## 4. 文档索引

| 文档 | 内容 |
|---|---|
| `docs/零基础上手教程.html` | **第三方开发者向零基础教程**（30 分钟：接依赖 → 第一个窗口 → 组件配方表 → 六个坑 → 报错对照表；示例对发布区 `RUIF.dll` 真编译） |
| `docs/开发者入口.html` | **对外唯一入口**（随包分发为 `DevDocs/index.html`）：30 秒定位 → 依赖两模式 → 5 分钟上手 → 文档地图 → 常见坑 |
| `docs/API参考.md` | **自动生成**的对外 API 全量清单；生成器 `_scripts/gen_api_reference.ps1`（**主题相关条目需在移除后重新生成**） |
| `docs/终极宝典v1.0-fixed.html` | 全量实例与布局配方（随包分发） |
| `docs/接入指南与引擎可行性审计.html` | 接入流程 + **依赖语义与降级矩阵** + 引擎三级可行性审计（✅原生 / ⚠️有代价 / ❌不承诺） |
| `docs/健康评估与发布记录.md` | GATE 判定、例外登记（EXC）、NOT RUN 清单、发布记录 |
| `docs/体检报告_*.md` | 历次体检报告（**历史记录**，其中 v2.2.x 篇描述的是已移除的主题 / 装饰层） |
| `docs/CHANGELOG.md` | 逐版本变更与 Gate 摘要（SemVer 口径） |
| `docs/组件库全量目录.md` | 组件分类索引（按用途分类，含 📋 待落地；**视觉参考见 `终极宝典v1.0-fixed.html`**） |
| `docs/底层UI框架方案.html` · `原版UI代码标准（Spy取证）.md` · `API签名核实清单.md` | 设计底稿与反编译实证 |

## 5. 脚本（`_scripts/`）

| 脚本 | 用途 |
|---|---|
| `sync_devdocs.ps1` | 生成对外开发者包 `DevDocs/`（白名单文档 + `BUILD-INFO.txt`；内部设计文档不进包） |
| `sync_ruif.ps1` | 开发区 → 发布区同步：进程守卫 + 白名单目录 + 镜像清理 + 框架 dll MD5 |
| `verify_release.ps1` | **发布门禁**：版本一致性 / 引用白名单 / 两区哈希 / 中英键集合对称 / 翻译键覆盖+死键审计 / 发布区卫生 / 开发者包完整性 / XML 良构 / 占位符一致性 |
| `gen_api_reference.ps1` | 反射生成 `docs/API参考.md`（发布前必跑，与实现同步） |
| `gen_preview.py` | 程序化生成封面 `About/Preview.png`（**待更新：旧版取色自已删除的主题预设**） |
| `health_check.ps1` | 静态体检（注释覆盖 / 死 API / 文档↔实现一致性 / 中文硬编码 / 红线 / 分配热点 / 测试覆盖；**装饰绘制上限组已随装饰层一并移除**） |
| `_verify/consumer_docs/verify_samples.ps1` | **文档示例真编译**：把随包文档里的示例对发布区 `RUIF.dll` 编译一次（依据接入审计 S-1；改文档示例后必跑） |

发布前顺序：**编译 → 单测 → `gen_api_reference.ps1` → `sync_devdocs.ps1` → `sync_ruif.ps1` → `verify_release.ps1`**。

## 6. 设计令牌（`RUIF.Tokens`）

`Tokens` 是组件库唯一、固定的视觉基线（唯一真源）：所有组件绘制一律取此处的令牌，运行期不可变。

| 类别 | 成员 |
|---|---|
| 颜色 | `Window` `Panel` `PanelRaised` `Card` `CardHover` `Border` `BorderSoft` `BorderHair` `Text` `Muted` `Dim` `Accent` `Info` `Alive` `Dead` `Warn` + `AccentSoft`（派生）+ `Series(int)`（序列轮转） |
| 间距 | `SpaceXxs` `SpaceXs` `SpaceSm` `SpaceMd` `SpaceLg` |
| 字号 | `FontLabel` `FontBody` `FontValue` `FontBodyLineHeight` |
| 尺寸 | `HeaderHeight` `SidebarWidth` `Gap` `PanelPadding` `CardGap` `SectionTitleHeight` `RuleHeight` `ScrollbarThickness` `CardPadX` `CardPadY` `GridGap` `BlockGap` `ProgressbarH` |

```csharp
// 组件内一律经令牌取色 / 取度量
Draw.Box(rect, Tokens.PanelRaised);
Draw.Label(rect, text, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
```

## 7. 红线

- **框架不带 UI、不带换肤**：不注册底栏按钮、不内置演示界面、不画窗口；只提供组件、令牌与布局。
- 框架只消费 Read Model 快照，组件内**禁读游戏状态**、禁 `new Color`、禁裸 `GUI.color`（颜色一律走 `Tokens`）。
- **绘制路径禁重复本地化解析**：文案一律经 `Tr`（带缓存）或 `.Translate()`。
- RW 1.6 硬约束内建于 L0（RadioButton 无 Group / ButtonText 无 disabled / DrawBoxSolid 重置 GUI.color 等）。
- 类内若定义了 `Draw` 方法，调用框架绘制必须写全限定 `RUIF.Draw.*`（否则 CS0119，已踩两次）。
- 公共 API 遵守 SemVer —— 删改公共 API 须先 `[Obsolete]` 保留一个主版本（**本次主题层移除为例外**：该层无外部消费者）。
- 用户可见文本一律走 Keyed / DefInjected；**开发者向诊断串**例外，见 EXC-003。

## 8. 已知未完成 / NOT RUN

- ⚠️ 简化件：`CommandPalette` 全局 Ctrl+K 焦点仲裁 · `MultiLineEditor` 真多行编辑 · `ScrollFadeHint`/`OverflowFade` 真 alpha 遮罩 · `HotkeyBind` 键位捕获（均留后续版本）。
- ⚠️ 带参译文里 **float/string 参数**未走缓存（int 参数已走）：缓存键需拼接参数，与省下的格式化分配净收益相当，故不做 —— 见 `TrCache` 注释与体检报告性能结论。
- ⚠️ `_scripts/gen_preview.py` 与 `health_check.ps1` 仍含主题 / 装饰相关逻辑，待同步清理。
- ⚠️ `docs/` 与 `DevDocs/` 中部分**历史报告与宝典历史章节**（体检报告 v2.2.x、宝典的 12 套风格色块章节等）仍描述已移除的主题 / 装饰层，作为历史记录保留。
- 未发布到 Workshop / GitHub；无公开版本标签。本地发布包：`…\RimWorld\Mods\RUIF`（框架，19 文件），由 `sync_ruif.ps1` 同步并经 `verify_release.ps1` 十道门禁校验。
