using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// L0 · 文本测量与行高（"参数空间"的量化入口）。
    ///
    /// <para><b>为什么必须有这一层</b>：布局里最常见的两类事故 —— 文本被硬切、组件与标题错位 ——
    /// 根因都是<b>行高靠手写魔数</b>。中文在不同字号下的实际行高并不等于"数字 + 2"，
    /// 而原版字号只有三档（<c>GameFont{Tiny,Small,Medium}</c>，<c>Text.SmallFontHeight = 22f</c>），
    /// 所以完全可以给出<b>确定的</b>行高表，而不是每个调用点自己估。</para>
    ///
    /// <para><b>测量一律走缓存</b>：宽度用 <c>GenUI.GetWidthCached</c>（原版自带 string→尺寸缓存）。
    /// <b>⛔ 不要在这里封装 <c>Text.CalcHeight</c></b>：原版<b>没有</b>它的缓存版，
    /// 提供包装等于邀请别人每帧调用 —— 这是性能事故（取证 §5 第 7 条：文本测量必须走缓存）。
    /// 需要精确高度时自行调用并**自己缓存**。</para>
    ///
    /// <para><b>与 <see cref="Px"/> 的分工</b>：<see cref="Px"/> 管"坐标落在哪个物理像素"，
    /// 本类管"一段文本需要多少逻辑高度/宽度"。</para>
    /// </summary>
    public static class Measure
    {
        /// <summary>行高（逻辑像素）。取整数值以便与 <see cref="Px"/> 对齐后不产生半像素行。</summary>
        public static float RowHeight(GameFont font)
        {
            switch (font)
            {
                case GameFont.Tiny: return 18f;
                case GameFont.Medium: return 30f;
                default: return 22f;   // = Text.SmallFontHeight
            }
        }

        /// <summary>带上下呼吸空间的"可点行高"（按钮 / 列表行）—— 比纯文本行高多一圈内边距。</summary>
        public static float ControlHeight(GameFont font) => RowHeight(font) + 8f;

        /// <summary>文本宽度（走原版缓存；调用方无需自行设置 <c>Text.Font</c>）。</summary>
        public static float Width(string text, GameFont font)
        {
            if (text.NullOrEmpty()) return 0f;
            using (Draw.Font(font)) return text.GetWidthCached();
        }

        /// <summary>在给定宽度内是否放得下（用于决定要不要走省略号分支）。</summary>
        public static bool Fits(string text, float width, GameFont font)
            => text.NullOrEmpty() || Width(text, font) <= width;

        /// <summary>
        /// 按宽度截断并补省略号（走 <c>GenText.Truncate</c>，<b>原版自带缓存参数</b>）。
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
