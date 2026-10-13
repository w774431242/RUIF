using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>交互原子的纯计算（可离线单测）。</summary>
    public static class InteractFmt
    {
        /// <summary>眼睛图标的正方形区域（宿主 rect 内居中）。</summary>
        public static Rect EyeRect(Rect host, float size)
            => new Rect(host.center.x - size / 2f, host.center.y - size / 2f, size, size);

        /// <summary>搜索命中分段数量（普通段 + 命中段交替）。</summary>
        public static int HighlightSegmentCount(string text, string query)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(query)) return 1;
            int count = 0;
            int idx = 0;
            while (idx < text.Length)
            {
                int hit = text.IndexOf(query, idx, StringComparison.OrdinalIgnoreCase);
                if (hit < 0) { count++; break; }
                if (hit > idx) count++;
                count++;
                idx = hit + query.Length;
            }
            return Mathf.Max(1, count);
        }

        /// <summary>把手点阵（rows × cols）中第 i 个点的尺寸（用于零分配循环绘制）。</summary>
        public static void GripDotSize(Rect area, int rows, int cols, float gap, out float w, out float h)
        {
            int r = Mathf.Max(1, rows), c = Mathf.Max(1, cols);
            w = Mathf.Max(1f, (area.width - gap * (c - 1)) / c);
            h = Mathf.Max(1f, (area.height - gap * (r - 1)) / r);
        }
    }

    /// <summary>
    /// 交互原子（批4-2b，Mod Manager / 工坊实证）：拖拽把手、可见性眼、搜索命中高亮、工具图标钮。
    /// 无状态；命中返回由调用方处理。图标一律用<b>短文本或几何绘制</b>（规避 emoji 字形缺失）。
    /// </summary>
    public static class InteractAtoms
    {
        /// <summary>拖拽把手（点阵）；返回是否按下（拖拽状态由调用方持有）。</summary>
        public static bool GripHandle(Rect rect, int rows = 3, int cols = 2, Color? color = null)
        {
            Color c = color ?? Tokens.Muted;
            InteractFmt.GripDotSize(Layout.Inset(rect, 2f, 3f), rows, cols, 2f, out float w, out float h);
            var inner = Layout.Inset(rect, 2f, 3f);
            for (int r = 0; r < rows; r++)
                for (int col = 0; col < cols; col++)
                    Draw.Box(new Rect(inner.x + col * (w + 2f), inner.y + r * (h + 2f), w, h), c);
            return Widgets.ButtonInvisible(rect);
        }

        /// <summary>可见性切换（眼睛）：外框 + 瞳点；不可见时画斜杠；返回是否点击。</summary>
        public static bool EyeToggle(Rect rect, bool visible, Color? color = null)
        {
            Color c = color ?? (visible ? Tokens.Text : Tokens.Dim);
            Rect eye = InteractFmt.EyeRect(rect, Mathf.Min(rect.width, rect.height) * 0.8f);
            Draw.Border(eye, c);
            if (visible)
            {
                float s = Mathf.Max(2f, eye.width * 0.3f);
                Draw.Box(new Rect(eye.center.x - s / 2f, eye.center.y - s / 2f, s, s), c);
            }
            else
            {
                // 斜杠：分段小方块近似（避免依赖贴图或矩阵）
                int n = 5;
                for (int i = 0; i < n; i++)
                {
                    float t = i / (float)(n - 1);
                    float x = eye.x + t * eye.width;
                    float y = eye.yMax - t * eye.height;
                    Draw.Box(new Rect(x - 1f, y - 1f, 2f, 2f), Tokens.Dead);
                }
            }
            return Widgets.ButtonInvisible(rect);
        }

        /// <summary>搜索命中高亮文本：命中段画 accent 底并加粗色，普通段走正文色。</summary>
        public static void SearchHighlight(Rect rect, string text, string query,
            GameFont font = GameFont.Small, Color? hitColor = null)
        {
            string t = text ?? string.Empty;
            string q = query ?? string.Empty;
            if (q.Length == 0)
            {
                Draw.Label(rect, t, font, Tokens.Text, TextAnchor.MiddleLeft);
                return;
            }
            Color hit = hitColor ?? Tokens.Accent;
            float x = rect.x;
            int idx = 0;
            using (Draw.Font(font))
            {
                while (idx < t.Length)
                {
                    int hitIdx = t.IndexOf(q, idx, StringComparison.OrdinalIgnoreCase);
                    string seg = hitIdx < 0 ? t.Substring(idx)
                        : (hitIdx == idx ? q : t.Substring(idx, hitIdx - idx));
                    bool isHit = hitIdx == idx;
                    float w = Text.CalcSize(seg).x;
                    var r = new Rect(x, rect.y, w, rect.height);
                    if (isHit) Draw.Box(r, Draw.WithAlpha(hit, 0.28f));
                    Draw.Label(r, seg, font, isHit ? hit : Tokens.Text, TextAnchor.MiddleLeft);
                    x += w;
                    idx += seg.Length;
                }
            }
        }

        /// <summary>工具栏图标钮（方形）：激活态 accent 底 + accent 描边；返回是否点击。</summary>
        public static bool ToolbarIcon(Rect rect, string glyph, bool active = false)
        {
            if (active)
            {
                Draw.Box(rect, Tokens.AccentSoft);
                Draw.Border(rect, Tokens.Accent);
            }
            else
            {
                Draw.Box(rect, Tokens.PanelRaised);
                Draw.Border(rect, Mouse.IsOver(rect) ? Tokens.Accent : Tokens.BorderSoft);
            }
            Draw.Label(rect, glyph ?? string.Empty, Tokens.FontLabel,
                active ? Tokens.Accent : Tokens.Muted, TextAnchor.MiddleCenter);
            return Widgets.ButtonInvisible(rect);
        }
    }
}
