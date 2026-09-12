using UnityEngine;
using Verse;

namespace RUIF.DevTools
{
    /// <summary>
    /// RUIF 组件画廊窗口（F11 / 底栏按钮打开）。
    ///
    /// 已是完整画廊：左侧分类索引 + 右侧可滚演示区（令牌/原子/输入/数据/导航/版式/预设/图表）。
    /// 与 <see cref="MainTabWindow_Gallery"/> 共用 <see cref="GalleryContent.Draw"/> 同一份演示内容。
    /// 绘制纪律：字体等绘制态改了必恢复（见各 Section 实现）。
    /// </summary>
    public class Dialog_Gallery : Window
    {
        public override Vector2 InitialSize => new Vector2(1180f, 1040f);

        public Dialog_Gallery()
        {
            doCloseX = true;
            absorbInputAroundWindow = true;
            forcePause = false;
        }

        /// <summary>打开占位 Gallery（示例消费 Mod / 调试入口调用）。</summary>
        public static void Open()
        {
            Find.WindowStack.Add(new Dialog_Gallery());
        }

        public override void DoWindowContents(Rect inRect)
        {
            GalleryContent.Draw(inRect);   // 与底栏页签共用同一份演示内容
        }
    }
}
