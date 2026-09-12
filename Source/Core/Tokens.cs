using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// 组件库设计令牌：颜色 / 间距 / 字号 / 尺寸的唯一来源。
    /// 本组件库为纯组件供应，不含任何换肤能力——所有令牌为编译期常量或只读静态值，
    /// 运行期不可切换、不可注册、不可注入。
    /// </summary>
    public static class Tokens
    {
        // ---- 间距 ----

        /// <summary>最小间距（4f）。用于元素内部紧贴排布。</summary>
        public const float SpaceXxs = 4f;
        /// <summary>次小间距（8f）。用于相关控件之间。</summary>
        public const float SpaceXs = 8f;
        /// <summary>小间距（12f）。组件默认间距，也是最常用的取值。</summary>
        public const float SpaceSm = 12f;
        /// <summary>中间距（16f）。用于分组之间。</summary>
        public const float SpaceMd = 16f;
        /// <summary>大间距（24f）。用于大区块之间。</summary>
        public const float SpaceLg = 24f;

        // ---- 字号 / 行高 ----

        /// <summary>正文行高（22f）。单行文本占位与多行排版的行距基准。</summary>
        public const float FontBodyLineHeight = 22f;

        // ---- 尺寸 ----

        /// <summary>页头高度（56f）。主标签页 / 全屏外壳顶部条高度。</summary>
        public const float HeaderHeight = 56f;
        /// <summary>侧栏宽度（208f）。左侧导航 / 筛选栏默认宽度。</summary>
        public const float SidebarWidth = 208f;
        /// <summary>通用间隙（12f）。与 <see cref="SpaceSm"/> 同值，语义偏"结构间隙"。</summary>
        public const float Gap = 12f;
        /// <summary>面板内边距（12f）。面板 / 卡片内容到边框的距离。</summary>
        public const float PanelPadding = 12f;
        /// <summary>卡片间距（8f）。同组卡片之间的间隙。</summary>
        public const float CardGap = 8f;
        /// <summary>区块标题高度（30f）。带标题区块的标题条高度。</summary>
        public const float SectionTitleHeight = 30f;
        /// <summary>分隔线粗细（1f）。水平 / 垂直规则线的默认线宽。</summary>
        public const float RuleHeight = 1f;
        /// <summary>滚动条厚度（18f）。垂直滚动条占位宽度。</summary>
        public const float ScrollbarThickness = 18f;
        /// <summary>卡片水平内边距（12f）。卡片左右内容内缩。</summary>
        public const float CardPadX = 12f;
        /// <summary>卡片垂直内边距（8f）。卡片上下内容内缩。</summary>
        public const float CardPadY = 8f;
        /// <summary>网格间隙（8f）。网格单元之间的横纵间距。</summary>
        public const float GridGap = 8f;
        /// <summary>块间距（12f）。堆叠区块之间的垂直间隔。</summary>
        public const float BlockGap = 12f;
        /// <summary>进度条高度（12f）。条形进度类组件的默认高度。</summary>
        public const float ProgressbarH = 12f;

        // ---- 字体 ----

        /// <summary>标签字体档（Tiny）。用于小字标签 / 次要信息。</summary>
        public static GameFont FontLabel => GameFont.Tiny;
        /// <summary>正文字体档（Small）。组件默认字体。</summary>
        public static GameFont FontBody => GameFont.Small;
        /// <summary>数值字体档（Medium）。用于 KPI 数字 / 强调值。</summary>
        public static GameFont FontValue => GameFont.Medium;

        // ---- 颜色基线（唯一、固定、不可变）----

        /// <summary>窗口底色（最外层背景）。</summary>
        public static readonly Color Window      = new Color(0.086f, 0.094f, 0.114f);
        /// <summary>面板底色。窗口内一级容器背景。</summary>
        public static readonly Color Panel       = new Color(0.114f, 0.125f, 0.149f);
        /// <summary>凸起面板底色。浮于面板之上的次级容器。</summary>
        public static readonly Color PanelRaised = new Color(0.141f, 0.157f, 0.188f);
        /// <summary>卡片底色。列表项 / 卡片默认背景。</summary>
        public static readonly Color Card        = new Color(0.133f, 0.149f, 0.180f);
        /// <summary>卡片悬停底色。鼠标悬停态卡片背景。</summary>
        public static readonly Color CardHover   = new Color(0.173f, 0.192f, 0.231f);
        /// <summary>边框色。强调边框 / 分隔。</summary>
        public static readonly Color Border      = new Color(0.271f, 0.294f, 0.337f);
        /// <summary>柔和边框色。常规描边。</summary>
        public static readonly Color BorderSoft  = new Color(0.192f, 0.212f, 0.247f);
        /// <summary>发丝边框色。最轻的描边（大区块外框）。</summary>
        public static readonly Color BorderHair  = new Color(0.165f, 0.180f, 0.212f);
        /// <summary>正文色。主要文本。</summary>
        public static readonly Color Text        = new Color(0.851f, 0.867f, 0.890f);
        /// <summary>次要文字色。副标题 / 单位 / 辅助说明。</summary>
        public static readonly Color Muted       = new Color(0.561f, 0.588f, 0.627f);
        /// <summary>最弱文字色。占位符 / 禁用文本 / 空态。</summary>
        public static readonly Color Dim         = new Color(0.431f, 0.463f, 0.510f);
        /// <summary>强调色。主操作 / 选中 / 数据高亮。</summary>
        public static readonly Color Accent      = new Color(0.306f, 0.604f, 0.941f);
        /// <summary>信息色。提示 / 中性语义。</summary>
        public static readonly Color Info        = new Color(0.478f, 0.722f, 0.910f);
        /// <summary>正常 / 健康 / 成功语义色。</summary>
        public static readonly Color Alive       = new Color(0.345f, 0.702f, 0.408f);
        /// <summary>错误 / 死亡 / 危险语义色。</summary>
        public static readonly Color Dead        = new Color(0.851f, 0.325f, 0.310f);
        /// <summary>警告 / 注意语义色。</summary>
        public static readonly Color Warn        = new Color(0.878f, 0.627f, 0.235f);

        /// <summary>强调色的低透明度变体（用于选中底、光晕等）。</summary>
        public static Color AccentSoft => Draw.WithAlpha(Accent, 0.16f);

        /// <summary>图表 / 序列色：在固定基线上循环取色。</summary>
        public static Color Series(int index)
        {
            switch (((index % 6) + 6) % 6)
            {
                case 0: return Accent;
                case 1: return Info;
                case 2: return Alive;
                case 3: return Warn;
                case 4: return Dead;
                default: return Muted;
            }
        }
    }
}
