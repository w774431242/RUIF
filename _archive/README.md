# RUIF `_archive/`（不参与编译、不进发布包）

## DevTools_Gallery/

组件演示区（Gallery）的完整源码，**v2.0.0 起移出编译集**：框架定位收敛为"只提供能力"，
不内置演示界面（用户决策：「不需要演示区，这只是框架」）。移出后的影响与替代：

| 原来的作用 | 现在的替代 |
|---|---|
| 在游戏里逐个看组件长相 | `DevDocs/终极宝典v1.0-fixed.html`（离线可开，全量组件卡；原 `组件设计图鉴.html` 已于 2026-09-12 退场，内容已并入宝典） |
| 有一份"用了很多组件"的代码参考 | `Sample/Source/Dialog_SampleRuifWindow.cs`（官方绘制范本） |
| 开发者随手打开看效果（F11 / 底栏按钮） | 无 —— 消费 Mod 自己画自己的窗口 |

移出同时关闭了 **EXC-001**（Gallery 演示区约 320 处中文硬编码），因为不再有随包分发的硬编码界面文案。

**保留原因**：源码仍在（未删除），若将来需要恢复，做法是：

1. 把 `DevTools_Gallery/` 移回 `RUIF/Source/DevTools/`；
2. 在 `Source/RUIF.csproj` 的 ItemGroup 里加回 `<Compile Include="$(SourceRoot)DevTools\**\*.cs" />`；
3. 在设置页恢复入口（或重建 `Defs/MainButtons/RUIF_Gallery.xml`）；
4. 重新跑 `_scripts/gen_api_reference.ps1`（公共 API 会变化）与 `verify_release.ps1`。

> ⚠️ 注意：这里的源码引用的是 v1.x 时期的 `RUIF.*` API。恢复前先与当前 `docs/API参考.md` 对齐
> （v2.0.0 新增了 `Theme.TryGet`、`RUIFSettings*`，未做破坏性改名，因此应可直接编译）。
