using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>九宫锚点（布局原子 <see cref="Atoms.Anchor"/> 用）。</summary>
    public enum Anchor9
    {
        TopLeft, TopCenter, TopRight,
        MidLeft, Center, MidRight,
        BottomLeft, BottomCenter, BottomRight
    }

    /// <summary>
    /// 布局原子（S1）：锁比例容器、九宫锚定、带标题组框。
    /// 纯几何计算 + 令牌绘制，不持有任何状态。
    /// </summary>
    public static class Atoms
    {
        /// <summary>在 outer 内按宽高比（w/h）居中取最大矩形——根治头像/缩略图拉伸变形。</summary>
        public static Rect AspectBox(Rect outer, float ratio)
        {
            if (ratio <= 0f) return outer;
            float w = outer.width;
            float h = w / ratio;
            if (h > outer.height)
            {
                h = outer.height;
                w = h * ratio;
            }
            return new Rect(outer.x + (outer.width - w) / 2f, outer.y + (outer.height - h) / 2f, w, h);
        }

        /// <summary>九宫锚定：把给定尺寸的框贴到父容器的某个锚位（可带边距）。</summary>
        public static Rect Anchor(Rect parent, Anchor9 a, Vector2 size, float margin = 0f)
        {
            float x, y;
            switch (a)
            {
                case Anchor9.TopLeft: case Anchor9.MidLeft: case Anchor9.BottomLeft:
                    x = parent.x + margin; break;
                case Anchor9.TopRight: case Anchor9.MidRight: case Anchor9.BottomRight:
                    x = parent.xMax - size.x - margin; break;
                default:
                    x = parent.x + (parent.width - size.x) / 2f; break;
            }
            switch (a)
            {
                case Anchor9.TopLeft: case Anchor9.TopCenter: case Anchor9.TopRight:
                    y = parent.y + margin; break;
                case Anchor9.BottomLeft: case Anchor9.BottomCenter: case Anchor9.BottomRight:
                    y = parent.yMax - size.y - margin; break;
                default:
                    y = parent.y + (parent.height - size.y) / 2f; break;
            }
            return new Rect(x, y, size.x, size.y);
        }

        /// <summary>带标题的组框（fieldset）：标题嵌在上边线断口处，返回内容区。</summary>
        public static Rect SectionFrame(Rect outer, string title)
        {
            Draw.Box(outer, Tokens.Panel);
            Draw.Border(outer, Tokens.BorderSoft);

            if (!string.IsNullOrEmpty(title))
            {
                using (Draw.Font(Tokens.FontLabel))
                {
                    float tw = Text.CalcSize(title).x + 8f;
                    // 断口：用背景色盖掉标题区间的一段上边线
                    Draw.Box(new Rect(outer.x + 6f, outer.y - 1f, tw, 2f), Tokens.Window);
                    Draw.Label(new Rect(outer.x + 10f, outer.y - 8f, tw, 16f), title, Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleLeft);
                }
            }
            return Layout.Inset(outer, Tokens.SpaceXs, Tokens.SpaceXs + 4f);
        }
    }
}
