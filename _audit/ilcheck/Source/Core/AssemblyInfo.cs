using System.Reflection;
using System.Runtime.CompilerServices;

// 程序集版本（2026-09-12 补：此前 GenerateAssemblyInfo=false 导致 dll 版本是 0.0.0.0，
// 被游戏内验证台的「诊断」页抓出来 —— Assembly.GetName().Version 全是 0，只有 Api.VersionString 有值）。
//
// ⚠️ 必须与 `Api.VersionString`（Core/Api.cs）保持一致；`_scripts/verify_release.ps1` 第 1 项
//    会比对两者，不一致直接 FAIL —— 靠门禁而非靠自觉。
[assembly: AssemblyVersion("1.1.2.0")]
[assembly: AssemblyFileVersion("1.1.2.0")]
[assembly: AssemblyInformationalVersion("1.1.2")]

// v2.1.1 重新引入（v1.2.1 曾因"无 internal 需暴露"而删除）：
// 现在框架内有需要离线单测的 internal 类型 —— 最有代表性的是 Core/TrCache.cs
// （翻译有界缓存：命中 / 语言失效 / 容量上限三种语义都必须在无引擎环境下可断言）。
// 只向测试程序集开放，不进入公共 API 面（公共 API 由 dll 导出清单与 docs/API参考.md 界定）。
[assembly: InternalsVisibleTo("RUIF.Tests")]
