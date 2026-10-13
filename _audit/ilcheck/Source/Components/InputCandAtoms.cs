using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>④ 输入候选组件（原 📋，官方实证支持，现落地）：批量滑杆配置窗 / 区域·允许区编辑器 /
    /// 键位绑定面板。无状态；值与选中态由调用方持有（ref / 数组原地回写）。官方依据：
    /// <c>Listing_Standard.Slider</c>/<c>Widgets.HorizontalSlider</c> · <c>Area</c>/<c>AllowedArea</c>/
    /// <c>RadioButtonLabeled</c> · <c>KeyBindingDef</c>/<c>Dialog_KeyBindings</c>/<c>KeyBindingSlot</c>
    /// （见《原版UI代码标准（Spy取证）》）。</summary>
    public static class InputCandAtoms
    {
        // ---- 批量滑杆配置窗 ----

        /// <summary>滑杆配置。</summary>
        public struct SliderCfg
        {
            /// <summary>标签。</summary>
            public string label;
            /// <summary>下限。</summary>
            public float min;
            /// <summary>上限。</summary>
            public float max;
            /// <summary>单位后缀（如 % / h）。</summary>
            public string unit;
        }

        /// <summary>批量滑杆配置窗：多滑杆同列，拖拽轨道原地回写 values[i]。</summary>
        public static void BatchSliders(Rect rect, SliderCfg[] cfg, float[] values)
        {
            int n = Mathf.Min(cfg == null ? 0 : cfg.Length, values == null ? 0 : values.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 6f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                Rect row = col.Take(rowH - 6f);
                var r = Layout.Row(row, 8f);
                float labW = Mathf.Max(80f, row.width * 0.34f);
                Draw.Label(r.Take(labW), cfg[i].label ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Rect track = r.Take(r.Rest.width - 56f);
                Draw.Box(track, Tokens.PanelRaised);
                Draw.Border(track, Tokens.BorderSoft);
                float t = cfg[i].max > cfg[i].min ? (values[i] - cfg[i].min) / (cfg[i].max - cfg[i].min) : 0f;
                Draw.Box(new Rect(track.x, track.y, track.width * Mathf.Clamp01(t), track.height), Tokens.Accent);
                // 拖拽：Mouse.IsOver 含窗口/滚动区门禁（红线）；用 IMGUI 原生 MouseDown/MouseDrag 判定按住态
                if (Verse.Mouse.IsOver(track) && Event.current.isMouse && Event.current.button == 0
                    && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
                    values[i] = InputCandFmt.SliderValueFromPos(Event.current.mousePosition.x, track, cfg[i].min, cfg[i].max);
                string v = values[i].ToString("F1") + (cfg[i].unit ?? string.Empty);
                Draw.Label(r.Rest, v, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleRight);
            }
        }

        // ---- 区域·允许区编辑器 ----

        /// <summary>区域·允许区编辑器：区域名 + 单选 dot，点击行选中（AllowedArea 选择器形态）。</summary>
        public static void AreaEditor(Rect rect, string[] areas, ref int selected)
        {
            if (areas == null) return;
            var col = Layout.Column(rect, 3f);
            float rowH = Mathf.Min(26f, rect.height / Mathf.Max(1, areas.Length));
            for (int i = 0; i < areas.Length; i++)
            {
                Rect row = col.Take(rowH);
                Draw.Box(row, InputCandFmt.AreaRowTone(i == selected));
                Rect dot = new Rect(row.x + 4f, row.y + (row.height - 14f) / 2f, 14f, 14f);
                Draw.Box(dot, i == selected ? Tokens.Accent : Tokens.PanelRaised);
                Draw.Border(dot, Tokens.BorderSoft);
                if (i == selected) Draw.Label(dot, "●", Tokens.FontLabel, Tokens.Window, TextAnchor.MiddleCenter);
                Draw.Label(new Rect(row.x + 24f, row.y, row.width - 28f, row.height),
                    areas[i] ?? string.Empty, Tokens.FontLabel, i == selected ? Tokens.Text : Tokens.Muted, TextAnchor.MiddleLeft);
                if (Widgets.ButtonInvisible(row)) selected = i;
            }
        }

        // ---- 键位绑定面板 ----

        /// <summary>键位绑定面板：动作 + 可点击键位槽；点击切换监听态（具体按键捕获由调用方完成）。</summary>
        public static void KeyBindPanel(Rect rect, string[] actions, string[] keys, bool[] listening)
        {
            int n = Mathf.Min(actions == null ? 0 : actions.Length, keys == null ? 0 : keys.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 3f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                Rect row = col.Take(rowH - 3f);
                var r = Layout.Row(Layout.Inset(row, 4f, 0f), 8f);
                Draw.Label(r.Take(r.Rest.width - 96f), actions[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Rect slot = r.Rest;
                bool listen = listening != null && i < listening.Length && listening[i];
                Draw.Box(slot, InputCandFmt.KeybindSlotTone(listen));
                Draw.Border(slot, Tokens.BorderSoft);
                Draw.Label(slot, listen ? "按下按键…" : (keys[i] ?? "—"), Tokens.FontLabel, listen ? Tokens.Accent : Tokens.Text, TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(slot) && listening != null && i < listening.Length) listening[i] = !listening[i];
            }
        }
    }
}
