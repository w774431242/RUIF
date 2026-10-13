using NUnit.Framework;
using UnityEngine;

namespace RUIF.Tests
{
    /// <summary>
    /// v1.3.0 <see cref="ButtonFmt"/>（方案 §3.4/§3.5 冻结件、§5.3 状态×tone 表、§5.6 降级阶梯）与
    /// <see cref="InputFmt.ResolveSelection"/>（§5.12 选择件落位档）的<b>纯函数</b>离线单测。
    ///
    /// <para><b>可离线</b>：取值一律经 <see cref="Tokens"/> 与 <c>Draw.WithAlpha</c>（纯色算术），
    /// 零 GUI、零 ECall、零引擎状态。<b>⛔ 不测</b> <c>ButtonFmt.FocusRingRect</c>（<c>Px.Snap</c> →
    /// <c>LudeonSky.UIScaling</c> ECall）、一切 <c>Measure.*</c>（<c>Verse.Text</c> 静态构造会被拖崩）、
    /// 字形纹理 —— 不写"假装能跑"的测试。</para>
    ///
    /// <para><b>本文件钉住的设计要点</b>：① tone 是 <see cref="Tokens"/> 之上的<b>语义别名层</b>，
    /// <b>不引入新颜色</b>；② <c>Disabled</c> 优先且<b>剥除 tone</b>（危险钮禁用后不得再显红边）；
    /// ③ <c>Clicked = enabled &amp;&amp; hit</c> 的两两真值表；④ <c>Plan</c> 的 D0/D1/D3/D6 阶梯单调无反转。</para>
    /// </summary>
    [TestFixture]
    public class ButtonFmtTests
    {
        private const float Eps = 1e-4f;

        private static readonly ButtonTone[] AllTones =
            { ButtonTone.Default, ButtonTone.Accent, ButtonTone.Danger, ButtonTone.Warn };

        private static void AssertColor(Color e, Color a, string msg)
        {
            Assert.AreEqual(e.r, a.r, Eps, msg + " .r");
            Assert.AreEqual(e.g, a.g, Eps, msg + " .g");
            Assert.AreEqual(e.b, a.b, Eps, msg + " .b");
            Assert.AreEqual(e.a, a.a, Eps, msg + " .a");
        }

        // ------------------------------------------------------------ 常量（§5.6 / §5.5 冻结值）

        [Test]
        public void Constants_MatchFrozenTable()
        {
            Assert.AreEqual(32f, ButtonFmt.FullWidthPad, Eps, "D0 加宽量 = PadX8×2 + icon18 + gap6");
            Assert.AreEqual(28f, ButtonFmt.MinUsableWidth, Eps, "D6 阈值");
            Assert.AreEqual(48f, ButtonFmt.EllipsisFloorWidth, Eps, "D1 起段");
            Assert.AreEqual(4f, ButtonFmt.NarrowPadX, Eps, "D3 收窄内边距下限");
            Assert.AreEqual(0.55f, ButtonFmt.FocusRingAlpha, Eps, "焦点环透明度（与 StateSample 逐值一致）");
        }

        // ------------------------------------------------------------ tone → 令牌（唯一语义通道）

        [Test]
        public void Tone_DefaultAndAccent_BothMapToTokensAccent()
        {
            AssertColor(Tokens.Accent, ButtonFmt.Tone(ButtonTone.Default), "Default → Tokens.Accent");
            AssertColor(Tokens.Accent, ButtonFmt.Tone(ButtonTone.Accent), "Accent → Tokens.Accent");
        }

        [Test]
        public void Tone_Danger_MapToTokensDead()
            => AssertColor(Tokens.Dead, ButtonFmt.Tone(ButtonTone.Danger), "Danger → Tokens.Dead");

        [Test]
        public void Tone_Warn_MapToTokensWarn()
            => AssertColor(Tokens.Warn, ButtonFmt.Tone(ButtonTone.Warn), "Warn → Tokens.Warn");

        [Test]
        public void Tone_IntroducesNoNewColor()
        {
            // 语义别名层：每个 tone 的取值必须逐值等于 Tokens 既有色之一（不破"禁 new Color"）
            foreach (ButtonTone t in AllTones)
            {
                Color c = ButtonFmt.Tone(t);
                bool known = SameChannel(c, Tokens.Accent) || SameChannel(c, Tokens.Dead) || SameChannel(c, Tokens.Warn);
                Assert.IsTrue(known, "tone=" + t + " 的取值必须是 Tokens 既有色，不得是新色");
                Assert.AreEqual(1f, c.a, Eps, "tone=" + t + " 基础色必须不透明");
            }
        }

        [Test]
        public void ToneSoft_IsToneAt16PercentAlpha()
        {
            foreach (ButtonTone t in AllTones)
            {
                Color tone = ButtonFmt.Tone(t);
                Color soft = ButtonFmt.ToneSoft(t);
                Assert.AreEqual(tone.r, soft.r, Eps, "ToneSoft 只改 alpha（r）");
                Assert.AreEqual(tone.g, soft.g, Eps, "ToneSoft 只改 alpha（g）");
                Assert.AreEqual(tone.b, soft.b, Eps, "ToneSoft 只改 alpha（b）");
                Assert.AreEqual(0.16f, soft.a, Eps, "ToneSoft alpha = 0.16f");
            }
        }

        [Test]
        public void FocusRingColor_IsToneAtFocusRingAlpha()
        {
            foreach (ButtonTone t in AllTones)
            {
                Color tone = ButtonFmt.Tone(t);
                Color ring = ButtonFmt.FocusRingColor(t);
                Assert.AreEqual(tone.r, ring.r, Eps, "环色 rgb 同 tone（r）");
                Assert.AreEqual(tone.g, ring.g, Eps, "环色 rgb 同 tone（g）");
                Assert.AreEqual(tone.b, ring.b, Eps, "环色 rgb 同 tone（b）");
                Assert.AreEqual(ButtonFmt.FocusRingAlpha, ring.a, Eps, "环 alpha = 0.55f");
            }
        }

        private static bool SameChannel(Color a, Color b)
            => Mathf.Abs(a.r - b.r) < Eps && Mathf.Abs(a.g - b.g) < Eps
               && Mathf.Abs(a.b - b.b) < Eps && Mathf.Abs(a.a - b.a) < Eps;

        // ------------------------------------------------------------ 状态解析（禁用优先）

        [Test]
        public void Resolve_DisabledWinsOverEverything()
        {
            Assert.AreEqual(InputState.Disabled, ButtonFmt.Resolve(true, true, true, false), "禁用优先于按下/选中/悬停");
            Assert.AreEqual(InputState.Disabled, ButtonFmt.Resolve(false, false, false, false), "禁用 + 无悬停");
        }

        [Test]
        public void Resolve_Priority_PressedOverSelectedOverHoverOverDefault()
        {
            Assert.AreEqual(InputState.Pressed, ButtonFmt.Resolve(true, true, true, true), "按下 > 选中");
            Assert.AreEqual(InputState.Selected, ButtonFmt.Resolve(true, false, true, true), "选中 > 悬停");
            Assert.AreEqual(InputState.Hover, ButtonFmt.Resolve(true, false, false, true), "悬停 > 默认");
            Assert.AreEqual(InputState.Default, ButtonFmt.Resolve(false, false, false, true), "默认");
        }

        [Test]
        public void Resolve_NeverProducesFocusedState()
        {
            // §3.4：InputFmt.Resolve 不加 focused 参数 ⇒ 焦点态不由本路径产出（16 组合穷举）
            for (int m = 0; m < 16; m++)
            {
                bool h = (m & 1) != 0, p = (m & 2) != 0, s = (m & 4) != 0, e = (m & 8) != 0;
                Assert.AreNotEqual(InputState.Focused, ButtonFmt.Resolve(h, p, s, e),
                    "组合 h=" + h + " p=" + p + " s=" + s + " e=" + e + " 不得产出 Focused");
            }
        }

        // ------------------------------------------------------------ 6 态 × tone → 四通道（§5.3）

        [Test]
        public void Channels_Default_MatchesBaselineChannels()
        {
            ButtonFmt.Channels(InputState.Default, ButtonTone.Default, out Color fill, out Color border,
                out Color text, out Color icon);
            AssertColor(Tokens.PanelRaised, fill, "Default.fill");
            AssertColor(Tokens.BorderSoft, border, "Default.border");
            AssertColor(Tokens.Text, text, "Default.text");
            AssertColor(Tokens.Muted, icon, "Default.icon");
        }

        [Test]
        public void Channels_Hover_OnlyBorderTakesTone_FillNotTinted()
        {
            ButtonFmt.Channels(InputState.Hover, ButtonTone.Danger, out Color fill, out Color border,
                out Color text, out Color icon);
            AssertColor(Tokens.CardHover, fill, "Hover.fill 不随 tone");
            AssertColor(Tokens.Dead, border, "Hover.border = tone");
            AssertColor(Tokens.Text, text, "Hover.text 不随 tone");
            AssertColor(Tokens.Muted, icon, "Hover.icon 不随 tone");
        }

        [Test]
        public void Channels_PressedAndSelected_UseToneSoftFillAndToneChannels()
        {
            foreach (ButtonTone t in AllTones)
            {
                foreach (InputState st in new[] { InputState.Pressed, InputState.Selected })
                {
                    ButtonFmt.Channels(st, t, out Color fill, out Color border, out Color text, out Color icon);
                    string tag = st + "/" + t;
                    AssertColor(ButtonFmt.ToneSoft(t), fill, tag + " fill = ToneSoft");
                    AssertColor(ButtonFmt.Tone(t), border, tag + " border = Tone");
                    AssertColor(ButtonFmt.Tone(t), text, tag + " text = Tone");
                    AssertColor(ButtonFmt.Tone(t), icon, tag + " icon = Tone");
                }
            }
        }

        [Test]
        public void Channels_Disabled_StripsTone_IdenticalAcrossAllTones()
        {
            // §5.3 要点①：危险钮禁用后不得再显红边 —— 四 tone 的禁用四通道必须逐值相同且剥 tone
            ButtonFmt.Channels(InputState.Disabled, ButtonTone.Default, out Color f0, out Color b0,
                out Color t0, out Color i0);
            AssertColor(Tokens.Panel, f0, "Disabled.fill");
            AssertColor(Tokens.BorderSoft, b0, "Disabled.border 剥除 tone");
            AssertColor(Tokens.Dim, t0, "Disabled.text");
            AssertColor(Tokens.Dim, i0, "Disabled.icon");

            foreach (ButtonTone t in AllTones)
            {
                ButtonFmt.Channels(InputState.Disabled, t, out Color f, out Color b, out Color tx, out Color ic);
                string tag = "Disabled/" + t;
                AssertColor(f0, f, tag + " fill 与 Default tone 相同");
                AssertColor(b0, b, tag + " border 与 Default tone 相同");
                AssertColor(t0, tx, tag + " text 与 Default tone 相同");
                AssertColor(i0, ic, tag + " icon 与 Default tone 相同");
            }
        }

        [Test]
        public void Channels_DangerAndWarn_NeverTintFill()
        {
            // §5.3 要点②：危险感来自边框与字，不来自红底
            foreach (ButtonTone t in new[] { ButtonTone.Danger, ButtonTone.Warn })
            {
                foreach (InputState st in new[] { InputState.Default, InputState.Focused, InputState.Hover })
                {
                    ButtonFmt.Channels(st, t, out Color fill, out _, out _, out _);
                    bool tinted = SameChannel(fill, ButtonFmt.Tone(t));
                    Assert.IsFalse(tinted, st + "/" + t + " 的 fill 不得被 tone 染色");
                }
            }
        }

        [Test]
        public void Channels_Focused_PanelRaisedFillWithToneBorder()
        {
            ButtonFmt.Channels(InputState.Focused, ButtonTone.Warn, out Color fill, out Color border,
                out Color text, out Color icon);
            AssertColor(Tokens.PanelRaised, fill, "Focused.fill");
            AssertColor(Tokens.Warn, border, "Focused.border = tone");
            AssertColor(Tokens.Text, text, "Focused.text");
            AssertColor(Tokens.Muted, icon, "Focused.icon");
        }

        // ------------------------------------------------------------ 命中 / 焦点（真值表）

        [Test]
        public void Clicked_IsEnabledAndHit()
        {
            Assert.IsFalse(ButtonFmt.Clicked(false, false), "禁用 + 未命中");
            Assert.IsFalse(ButtonFmt.Clicked(false, true), "禁用 + 命中 ⇒ 仍不触发（事后 AND 的门禁）");
            Assert.IsFalse(ButtonFmt.Clicked(true, false), "可用 + 未命中");
            Assert.IsTrue(ButtonFmt.Clicked(true, true), "可用 + 命中");
        }

        [Test]
        public void ShouldRing_IsOrOfExplicitAndNamedFocus()
        {
            Assert.IsFalse(ButtonFmt.ShouldRing(false, false), "两入口都无 ⇒ 不画");
            Assert.IsTrue(ButtonFmt.ShouldRing(true, false), "focusRing 显式 ⇒ 画");
            Assert.IsTrue(ButtonFmt.ShouldRing(false, true), "按名命中 ⇒ 自动画");
            Assert.IsTrue(ButtonFmt.ShouldRing(true, true), "可并用");
        }

        // ------------------------------------------------------------ 降级阶梯（§5.6）

        [Test]
        public void Plan_WideLabel_DescendsPlaceholder_EllipsisNarrow_Full()
        {
            const float label = 100f;
            ButtonPlan p27 = ButtonFmt.Plan(27f, label);
            Assert.AreEqual(ButtonFit.Placeholder, p27.Fit, "w<28 ⇒ D6 占位");
            Assert.AreEqual(ButtonFmt.NarrowPadX, p27.PadX, Eps, "占位仍留 4f 内边距（不得归零）");

            ButtonPlan p132 = ButtonFmt.Plan(132f, label);
            Assert.AreEqual(ButtonFit.Full, p132.Fit, "w >= label+32 ⇒ D0 完整标签");
            Assert.AreEqual(WindowFmt.ButtonPadX, p132.PadX, Eps, "D0 PadX = 8f");

            Assert.AreEqual(ButtonFit.Ellipsis, ButtonFmt.Plan(131f, label).Fit, "131 ⇒ 落 D1 省略号");
            Assert.AreEqual(WindowFmt.ButtonPadX, ButtonFmt.Plan(131f, label).PadX, Eps, "D1 PadX = 8f");
            Assert.AreEqual(ButtonFit.Ellipsis, ButtonFmt.Plan(48f, label).Fit, "48 仍走 D1");
            Assert.AreEqual(ButtonFit.Ellipsis, ButtonFmt.Plan(47f, label).Fit, "47 ⇒ D3 收窄后仍省略");
            Assert.AreEqual(ButtonFmt.NarrowPadX, ButtonFmt.Plan(47f, label).PadX, Eps, "D3 PadX = 4f");
        }

        [Test]
        public void Plan_NarrowLabel_HitsD3NarrowPadFullBranch()
        {
            // label 较窄时 D3「收窄内边距后重试完整标签」可达（label+8 落在 [28,48) 内）
            const float label = 20f;
            ButtonPlan p30 = ButtonFmt.Plan(30f, label);
            Assert.AreEqual(ButtonFit.Full, p30.Fit, "w >= label+8 ⇒ D3 完整标签");
            Assert.AreEqual(ButtonFmt.NarrowPadX, p30.PadX, Eps, "D3 PadX = 4f");
            Assert.AreEqual(ButtonFit.Placeholder, ButtonFmt.Plan(27f, label).Fit, "w<28 ⇒ 占位");
            Assert.AreEqual(ButtonFit.Ellipsis, ButtonFmt.Plan(51f, label).Fit, "51 ⇒ D1（<52）");
            Assert.AreEqual(ButtonFit.Full, ButtonFmt.Plan(52f, label).Fit, "52 = label+32 ⇒ D0");
        }

        [Test]
        public void Plan_PadXNeverZero_AndMonotoneInWidth()
        {
            const float label = 100f;
            int rank = -1;      // Placeholder=0 < Ellipsis=1 < Full=2（随宽度增大须单调非降）
            for (int w = 0; w <= 250; w++)
            {
                ButtonPlan p = ButtonFmt.Plan(w, label);
                Assert.GreaterOrEqual(p.PadX, ButtonFmt.NarrowPadX, "w=" + w + " PadX 不得归零");
                Assert.IsTrue(p.PadX == ButtonFmt.NarrowPadX || p.PadX == WindowFmt.ButtonPadX,
                    "w=" + w + " PadX 只允许 4f / 8f 两档");

                int r = p.Fit == ButtonFit.Placeholder ? 0 : (p.Fit == ButtonFit.Ellipsis ? 1 : 2);
                Assert.GreaterOrEqual(r, rank, "w=" + w + " 阶梯出现反转（区间空洞/重叠）");
                rank = r;
            }
            Assert.AreEqual(2, rank, "扫到 250f 必须已到达 Full 档");
        }

        // ------------------------------------------------------------ 选择件落位档（§5.12）

        [Test]
        public void ResolveSelection_RowHeightBelowFloor_Omits()
        {
            // 纯可观测的下限闸：rowHeight < 14f ⇒ Omit
            // （"上调到 Measure.RowHeight(FontLabel)" 那半在 InputAtoms.ResolveRowHeight 内，
            //   依赖 Verse.Text 静态构造 ECall ⇒ 离线不可测，本文件不伪造）
            Assert.AreEqual(SelectionFit.Omit,
                InputFmt.ResolveSelection(13.9f, 18f, 1000f, 10f, 4f, 10f), "13.9 < 14 ⇒ Omit（无视宽度）");
            Assert.AreNotEqual(SelectionFit.Omit,
                InputFmt.ResolveSelection(14f, 18f, 1000f, 10f, 4f, 10f), "14 = 下限 ⇒ 不再 Omit");
            Assert.AreEqual(14f, InputFmt.SelectionOmitRowHeight, Eps, "Omit 阈值冻结 = 14f");
            Assert.AreEqual(12f, InputFmt.SelectionMinBox, Eps, "缩盒下限冻结 = 12f");
        }

        [Test]
        public void ResolveSelection_LadderBoundaries_ExactAndNoGap()
        {
            // box=18, gap=4, labelWidth=100, ellipsisWidth=10 ⇒ 分界 = 18 / 32 / 122
            const float box = 18f, gap = 4f, label = 100f, ell = 10f, row = 24f;
            Assert.AreEqual(SelectionFit.Full, InputFmt.ResolveSelection(row, box, 122f, label, gap, ell), "122 ⇒ Full");
            Assert.AreEqual(SelectionFit.Ellipsis, InputFmt.ResolveSelection(row, box, 121f, label, gap, ell), "121 ⇒ Ellipsis");
            Assert.AreEqual(SelectionFit.Ellipsis, InputFmt.ResolveSelection(row, box, 32f, label, gap, ell), "32 ⇒ Ellipsis");
            Assert.AreEqual(SelectionFit.BoxOnly, InputFmt.ResolveSelection(row, box, 31f, label, gap, ell), "31 ⇒ BoxOnly");
            Assert.AreEqual(SelectionFit.BoxOnly, InputFmt.ResolveSelection(row, box, 18f, label, gap, ell), "18 ⇒ BoxOnly（盒必须保留）");
            Assert.AreEqual(SelectionFit.Placeholder, InputFmt.ResolveSelection(row, box, 17f, label, gap, ell), "17 ⇒ Placeholder");
        }

        [Test]
        public void ResolveSelection_IsMonotoneInWidth_NoHolesOrOverlaps()
        {
            const float box = 18f, gap = 4f, label = 100f, ell = 10f, row = 24f;
            int rank = 0;   // Placeholder=0 < BoxOnly=1 < Ellipsis=2 < Full=3
            int transitions = 0;
            for (int w = 0; w <= 300; w++)
            {
                SelectionFit f = InputFmt.ResolveSelection(row, box, w, label, gap, ell);
                int r = f == SelectionFit.Placeholder ? 0 : (f == SelectionFit.BoxOnly ? 1 : (f == SelectionFit.Ellipsis ? 2 : 3));
                Assert.GreaterOrEqual(r, rank, "w=" + w + " 落位档出现反转（区间重叠）");
                if (r != rank) transitions++;
                rank = r;
            }
            Assert.AreEqual(3, transitions, "0..300 之间应恰好 3 次换档（18 / 32 / 122），无空洞无重复");
        }
    }
}
