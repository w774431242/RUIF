using UnityEngine;

namespace RUIF
{
    /// <summary>⑦ 导航候选组件的纯计算（可离线单测）。</summary>
    public static class NavCandFmt
    {
        /// <summary>主标签页底栏选中态文字色。</summary>
        public static Color MainTabTone(bool active) => active ? Tokens.Accent : Tokens.Text;

        /// <summary>策略列表行底色（选中=强调软底）。</summary>
        public static Color PolicyRowTone(bool selected) => selected ? Tokens.AccentSoft : Tokens.Card;

        /// <summary>向导步骤节点色（i&lt;step 已完成 / i==step 当前 / 否则待办）。</summary>
        public static Color WizardStepTone(int i, int step)
            => i < step ? Tokens.Accent : i == step ? Tokens.Accent : Tokens.Muted;

        /// <summary>向导步骤连接线色（已完成段=强调，待办段=边框软色）。</summary>
        public static Color WizardConnectorTone(int i, int step) => i < step ? Tokens.Accent : Tokens.BorderSoft;

        /// <summary>向导步骤序号色。</summary>
        public static Color WizardNumberTone(int i, int step) => i <= step ? Tokens.Window : Tokens.Muted;
    }
}
