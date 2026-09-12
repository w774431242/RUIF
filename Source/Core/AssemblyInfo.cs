using System.Runtime.CompilerServices;

// v2.1.1 重新引入（v1.2.1 曾因"无 internal 需暴露"而删除）：
// 现在框架内有需要离线单测的 internal 类型 —— 最有代表性的是 Core/TrCache.cs
// （翻译有界缓存：命中 / 语言失效 / 容量上限三种语义都必须在无引擎环境下可断言）。
// 只向测试程序集开放，不进入公共 API 面（公共 API 由 dll 导出清单与 docs/API参考.md 界定）。
[assembly: InternalsVisibleTo("RUIF.Tests")]
