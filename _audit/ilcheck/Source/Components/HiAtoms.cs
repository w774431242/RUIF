using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// A0 · 新功能脉冲高亮原子族 —— 桥接 <c>Verse.UIHighlighter</c>（第二轮取证新增发现）。
    ///
    /// <para><b>跨帧协议（照抄，否则永不显示）</b>：高亮是<b>两帧协作</b>的 ——</para>
    /// <list type="number">
    /// <item>本帧（通常是"打开某面板"的那一帧）调 <see cref="Tag"/> 登记一个 tag；</item>
    /// <item><b>下一帧</b>绘制该 tag 对应控件时调 <see cref="Opportunity"/>，
    /// 引擎才会画脉冲光晕（判定条件是 <c>Time.frameCount == 登记帧 + 1</c>）。</item>
    /// </list>
    /// <para>单独调 <see cref="Opportunity"/> 不会有任何效果 —— 这是"高亮不出现"的唯一根因。</para>
    ///
    /// <para><b>视觉参数（原版实测）</b>：脉冲频率 1.2、振幅 0.7（<c>Pulser.PulseBrightness</c>），
    /// 底色为白色透明叠加（<c>Widgets.DrawTextHighlight</c>，默认外扩 4px）。</para>
    ///
    /// <para><b>定位</b>：用于"新手引导 / 版本新功能提示"，属可选装饰，缺失不影响功能。</para>
    /// </summary>
    public static class HiAtoms
    {
        /// <summary>登记一个待高亮项（本帧调用；tag 用稳定字符串，如 <c>"RUIF.Panel.Research"</c>）。</summary>
        public static void Tag(string tag)
        {
            if (!tag.NullOrEmpty()) UIHighlighter.HighlightTag(tag);
        }

        /// <summary>在目标控件处绘制脉冲高亮（须在 <see cref="Tag"/> 的<b>下一帧</b>调用）。</summary>
        public static void Opportunity(Rect rect, string tag)
        {
            if (tag.NullOrEmpty()) return;
            UIHighlighter.HighlightOpportunity(rect, tag);
        }
    }
}
