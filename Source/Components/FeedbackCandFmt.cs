using UnityEngine;

namespace RUIF
{
    /// <summary>⑥ 反馈候选组件的纯计算（可离线单测）。</summary>
    public static class FeedbackCandFmt
    {
        /// <summary>警报严重度配色（0 提示 / 1 警告 / 2 危险）。</summary>
        public static Color AlertSeverityTone(int level)
        {
            switch (level)
            {
                case 2: return Tokens.Dead;
                case 1: return Tokens.Warn;
                default: return Tokens.Info;
            }
        }

        /// <summary>浮动菜单行配色（悬停=强调软底，常规=卡片底）。</summary>
        public static Color FloatMenuRowTone(bool hover) => hover ? Tokens.AccentSoft : Tokens.Card;
    }
}
