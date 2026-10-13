using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// L0 · 文本测量与行高（"参数空间"的量化入口）。
    ///
    /// <para><b>为什么必须有这一层</b>：布局里最常见的两类事故 —— 文本被硬切、组件与标题错位 ——
    /// 根因都是<b>行高靠手写魔数</b>。中文在不同字号下的实际行高并不等于"数字 + 2"，
    /// 而原版字号只有三档（<c>GameFont{Tiny,Small,Medium}</c>），所以行高<b>只能向引擎取值</b>：
    /// <see cref="RowHeight(GameFont)"/> 直接转发 <c>Text.LineHeightOf</c> —— 那是原版在静态构造期
    /// 逐个字号实测出来的真值表，不是任何"约定俗成的数字"。
    /// 历史上这里写死的 18f / 22f / 30f 三个数字全部撤除（详见 <see cref="RowHeight(GameFont)"/>）。</para>
    ///
    /// <para><b>⚠ 必须保持为方法，不得缓存</b>：<c>Text.TinyFontSupported</c> 运行期可变
    /// （玩家可在选项里随时勾选「禁用小字体」）⇒ 任何"首次访问即定值"的 <c>static readonly</c>
    /// 缓存都会冻错值。<see cref="RowHeight(GameFont)"/> 因此是<b>每次调用都重新求值</b>的纯转发。</para>
    ///
    /// <para><b>⛔ 不要在这里封装 <c>Text.CalcHeight</c></b>：原版<b>没有</b>它的缓存版，
    /// 提供包装等于邀请别人每帧调用 —— 这是性能事故（取证 §5 第 7 条：文本测量必须走缓存）。
    /// 需要精确高度时自行调用并**自己缓存**。</para>
    ///
    /// <para><b>与 <see cref="Px"/> 的分工</b>：<see cref="Px"/> 管"坐标落在哪个物理像素"，
    /// 本类管"一段文本需要多少逻辑高度/宽度"。</para>
    /// </summary>
    public static class Measure
    {
        /// <summary>
        /// 行高（逻辑像素）—— 取<b>引擎真值</b>，并叠加原版的<b>静默降级镜像</b>。
        ///
        /// <para><b>原实现（已删除）</b>：手写魔数 <c>Tiny=18f / Medium=30f / default=22f</c>。
        /// 三档全是估值：Tiny 真值不等于 18、Medium 真值不等于 30，而 <c>default</c> 分支
        /// 还会把任何未来新增字号静默当成 Small ⇒ 布局与文字实际占位不一致，
        /// 表现即"文字压到图形上 / 被矩形硬切"。</para>
        ///
        /// <para><b>为什么必须叠一层 <see cref="DegradeTiny"/></b>：<c>Verse.Text</c> 的
        /// <c>Font</c> setter 会<b>静默</b>把不受支持的 Tiny 降级成 Small（<c>Verse.Text.cs:31-35</c>），
        /// 因此"请求 Tiny"与"实际画出的字号"在禁用小字体时并不相同；
        /// 行高必须跟随<b>实际画出的</b>那一档，否则仍然错位。</para>
        /// </summary>
        public static float RowHeight(GameFont font)
            => Text.LineHeightOf(DegradeTiny(font, Text.TinyFontSupported));

        /// <summary>
        /// 镜像原版 <c>Verse.Text.Font</c> setter 的静默降级规则（<c>Verse.Text.cs:31-35</c>）：
        /// Tiny 不受支持时，原版把它当 Small 画。
        ///
        /// <para><b>纯函数</b>（不读写任何引擎状态）⇒ 可在<b>无 Unity 宿主的离线单测</b>里断言。
        /// 这是本类唯一的可离线验证入口 —— 单测<b>不得</b>调用 <see cref="RowHeight(GameFont)"/>：
        /// 那会触碰 <c>Verse.Text</c> 静态构造 → <c>Resources.Load</c>（ECall），
        /// 在离线 net48 宿主抛异常并拖垮整个测试程序集。</para>
        /// </summary>
        internal static GameFont DegradeTiny(GameFont font, bool tinySupported)
            => (font == GameFont.Tiny && !tinySupported) ? GameFont.Small : font;

        /// <summary>带上下呼吸空间的"可点行高"（按钮 / 列表行）—— 比纯文本行高多一圈内边距。</summary>
        public static float ControlHeight(GameFont font) => RowHeight(font) + 8f;

        /// <summary>
        /// 文本宽度（真实测量，<b>不缓存</b>；调用方无需自行设置 <c>Text.Font</c>）。
        ///
        /// <para><b>⚠ 为什么不用 <c>GenUI.GetWidthCached</c>（原实现，2026-09-16 撤除）</b>：
        /// 该缓存字典的键<b>只有 string、不含字号</b>
        /// （<c>Verse.GenUI.cs:146-160</c> + 缓存字段 <c>:964</c>），且该字典<b>全局共享</b>
        /// （原版与其它 Mod 共用同一实例）⇒ <b>跨字号会取到错值</b>：
        /// 先以 Tiny 量过的串，再以 Medium 查询会拿回 Tiny 的宽度。
        /// 这是<b>正确性缺陷</b>而非性能取舍，<see cref="Fits"/> 会随之误判。</para>
        ///
        /// <para><b>为什么不自建带字号的缓存</b>：全仓 <c>Width</c>/<c>Fits</c> 当前<b>无活跃调用点</b>
        /// （<c>Clamp</c> 也仅 <c>Industrial.Gauge</c> 1 处）⇒ 零压力。
        /// 而自建缓存还需额外处理「UI 缩放变化后失效」与「数值串把字典挤爆」两个问题，
        /// 相对可忽略的收益是<b>净负收益</b>。写法与组件层既有实践一致
        /// （<c>DataAtoms.cs</c> / <c>TextAtoms.cs</c> 等均直接 <c>Text.CalcSize</c>）。</para>
        /// </summary>
        public static float Width(string text, GameFont font)
        {
            if (text.NullOrEmpty()) return 0f;
            using (Draw.Font(font)) return Text.CalcSize(text).x;
        }

        /// <summary>在给定宽度内是否放得下（用于决定要不要走省略号分支）。</summary>
        public static bool Fits(string text, float width, GameFont font)
            => text.NullOrEmpty() || Width(text, font) <= width;

        /// <summary>
        /// 按宽度截断并补省略号（走 <c>GenText.Truncate</c>）。
        ///
        /// <para><b>⚠ 缓存与分配事实（2026-09-16 更正）</b>：<c>GenText.Truncate</c> 的第三参
        /// <c>Dictionary&lt;string,string&gt; cache</c> 是<b>可选</b>的，不传即<b>零缓存</b>
        /// （<c>Verse.GenText.cs:923-928</c>）—— 原注释里"原版自带缓存参数"的说法会让人误以为
        /// "不传也走缓存"。且该方法<b>返回新字符串</b>（每帧调用必分配）
        /// ⇒ <b>不得放进逐帧热循环</b>；确需在热路径截断时，应由调用方持缓存字典传入。</para>
        ///
        /// <para>用途：宁可显示"殖民地遭遇持续三天的有毒尘…"，也不要让文本被矩形硬切一半。</para>
        /// </summary>
        public static string Clamp(string text, float width, GameFont font)
        {
            if (text.NullOrEmpty() || width <= 1f) return text;
            using (Draw.Font(font)) return text.Truncate(width);
        }

        /// <summary>
        /// 一行"标签 + 值"的高度（两侧取较大字号），用于把行高从魔数改为可推导值。
        /// </summary>
        public static float RowHeight(GameFont left, GameFont right)
            => Mathf.Max(RowHeight(left), RowHeight(right));
    }
}
