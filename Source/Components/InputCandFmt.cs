using UnityEngine;

namespace RUIF
{
    /// <summary>④ 输入候选组件的纯计算（可离线单测）。</summary>
    public static class InputCandFmt
    {
        /// <summary>由鼠标 X 在滑杆轨道上反解取值（钳制 0~1 → [min,max]）。</summary>
        public static float SliderValueFromPos(float mouseX, Rect track, float min, float max)
            => min + Mathf.Clamp01((mouseX - track.x) / Mathf.Max(1f, track.width)) * (max - min);

        /// <summary>区域选中态底色。</summary>
        public static Color AreaRowTone(bool selected) => selected ? Tokens.AccentSoft : Tokens.Card;

        /// <summary>键位槽底色（监听中=强调，常规=面板凸起）。</summary>
        public static Color KeybindSlotTone(bool listening) => listening ? Tokens.Accent : Tokens.PanelRaised;
    }
}
