using RimWorld;
using UnityEngine;
using Verse;

namespace RUIF.DevTools
{
    /// <summary>
    /// RUIF 组件画廊 —— 底栏主按钮页签（MainTabWindow 接入点）。
    ///
    /// 用途：让框架在游戏里"一键可见"：底栏点 RUIF（或 F11）即打开本窗口。
    /// 内容与 <see cref="Dialog_Gallery"/> 共用 <see cref="GalleryContent.Draw"/>（S1 起已画真实组件，非占位）。
    ///
    /// 约束：①不持有持久状态（GATE-UI）②字体等绘制态改了必恢复。
    /// </summary>
    public class MainTabWindow_Gallery : MainTabWindow
    {
        public override Vector2 RequestedTabSize => new Vector2(1180f, 1040f);

        public override void DoWindowContents(Rect inRect)
        {
            // S1：改画真实内容（令牌 + 布局 + 绘制层），不再是纯文本占位
            GalleryContent.Draw(inRect);
        }
    }
}
