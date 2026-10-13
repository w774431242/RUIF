using RimWorld;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// 底栏「组件验证台」按钮的工作类。
    ///
    /// <para><b>本类自身就是一条验收证据</b>：它只实现一个成员（<see cref="Activate"/>）就拿到了
    /// 原版底栏按钮的全部行为（标签缩写 / 悬停音效 / 悬停位移 / Tooltip / 禁用态 / 贴底定位），
    /// 因为余下都由 <c>RimWorld.MainButtonWorker.DoButton</c> 提供 ——
    /// 也就是第二轮取证里把「页签族」从 ⚠️ 升级为 ✅ 的那条依据（无需 Harmony）。</para>
    /// </summary>
    public class DemoButtonWorker : MainButtonWorker
    {
        /// <inheritdoc />
        public override void Activate()
        {
            // 用 WindowStack 自身查询，不持有静态引用（与设置页入口走同一条路径，避免两套状态）
            DemoWindow open = Find.WindowStack.WindowOfType<DemoWindow>();
            if (open != null)
            {
                Find.WindowStack.TryRemove(open, doCloseSound: false);
                return;
            }
            Find.WindowStack.Add(new DemoWindow());
        }

        /// <inheritdoc />
        public override float ButtonBarPercent => 0f;
    }
}
