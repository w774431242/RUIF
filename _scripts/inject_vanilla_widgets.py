#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""将 Mono.Cecil 静态枚举的 Verse.Widgets 组件按族归类，注入标准规范组件预览.html。
每个原版组件带「原版 Widgets」徽章 + 推荐尺寸 + 静态示意 demo + 原版真实用法，
与 RUIF 标准卡同规格成品化。幂等：每次运行先剥离旧 .comp.vanilla 再重新注入。"""
import re

DUMP = r"D:/toolbox/asmverify/widgets.txt"
HTML = r"E:/SteamLibrary/steamapps/common/RimWorld/11/RUIF/docs/02-组件图鉴/标准规范组件预览.html"

DOMAIN_TAG = {
    "text":"Text","visual":"Visual","panel":"Panel","progress":"Progress",
    "input":"Input","official":"Official","layout":"Layout","nav":"Nav",
}

DOMAIN = {
    "LabelCacheHeight":"text","Label":"text","LabelEllipses":"text","LongLabel":"text",
    "LabelScrollable":"text","LabelWithIcon":"text","DefLabelWithIcon":"text","LabelFit":"text",
    "HyperlinkWithIcon":"text","DrawNumberOnMap":"text","DrawStringOnMap":"text",
    "NoneLabel":"text","MouseAttachedLabel":"text","WorldAttachedLabel":"text",
    "CanDrawIconFor":"visual","DefIcon":"visual","ThingIcon":"visual","GetIconFor":"visual",
    "CroppedTerrainTextureRect":"visual","DrawAltRect":"visual","DrawLine":"visual",
    "DrawLineHorizontal":"visual","DrawLineVertical":"visual","DrawBoxSolid":"visual",
    "DrawBoxSolidWithOutline":"visual","DrawBox":"visual","DrawButtonGraphic":"visual",
    "DrawRectFast":"visual","DrawTextureFitted":"visual","DrawTextureRotated":"visual",
    "ToUVRect":"visual","DrawTexturePart":"visual","DrawHighlightSelected":"visual",
    "DrawHighlightIfMouseover":"visual","DrawHighlight":"visual","DrawLightHighlight":"visual",
    "DrawStrongHighlight":"visual","DrawTextHighlight":"visual","ColorSelectorIcon":"visual",
    "ColorBox":"visual","ColorSelector":"visual","HSVColorWheel":"visual",
    "ColorTemperatureBar":"visual","ColorTextfields":"visual","DrawWindowBackground":"visual",
    "DrawMenuSection":"visual","DrawWindowBackgroundTutor":"visual","DrawOptionUnselected":"visual",
    "DrawOptionSelected":"visual","DrawOptionBackground":"visual","DrawShadowAround":"visual",
    "DrawAtlas":"visual","DrawTitleBG":"visual",
    "ListSeparator":"panel",
    "FillableBar":"progress","FillableBarLabeled":"progress","FillableBarChangeArrows":"progress",
    "DraggableBar":"progress",
    "Checkbox":"input","CheckboxLabeled":"input","ToggleInvisibleDraggable":"input",
    "CheckboxLabeledSelectable":"input","GetCheckboxTexture":"input","CheckboxDraw":"input",
    "CheckboxMulti":"input","RadioButton":"input","RadioButtonLabeled":"input","ButtonText":"input",
    "ButtonTextDraggable":"input","CustomButtonText":"input","ButtonTextSubtle":"input",
    "ButtonImage":"input","ButtonImageDraggable":"input","ButtonImageFitted":"input",
    "ButtonImageWithBG":"input","ButtonInvisible":"input","ButtonInvisibleDraggable":"input",
    "TextField":"input","TextArea":"input","TextEntryLabeled":"input","DelayedTextField":"input",
    "TextFieldVector":"input","TextFieldNumeric":"input","TextFieldNumericLabeled":"input",
    "TextFieldPercent":"input","HorizontalSlider":"input","FrequencyHorizontalSlider":"input",
    "IntEntry":"input","FloatRange":"input","IntRange":"input","QualityRange":"input",
    "FloatRangeWithTypeIn":"input","Dropdown":"input",
    "InfoCardButton":"official","InfoCardButtonCentered":"official",
    "BeginGroup":"layout","EndGroup":"layout","ScrollHorizontal":"layout","BeginScrollView":"layout",
    "AdjustRectsForScrollView":"layout","EndScrollView":"layout",
    "CloseButtonFor":"nav","BackButtonFor":"nav",
}

CN = {
    "BeginGroup":"分组开始","EndGroup":"分组结束","CanDrawIconFor":"图标可绘制判定","DefIcon":"Def 图标",
    "ThingIcon":"物体图标","GetIconFor":"取图标","CroppedTerrainTextureRect":"地形贴图裁剪矩形",
    "DrawAltRect":"交替背景矩形","ListSeparator":"列表分隔条","DrawLine":"画线","DrawLineHorizontal":"画水平线",
    "DrawLineVertical":"画垂直线","DrawBoxSolid":"实心方块","DrawBoxSolidWithOutline":"带描边实心方块",
    "DrawBox":"画框","LabelCacheHeight":"标签高度缓存","Label":"标签","LabelEllipses":"省略号标签",
    "LongLabel":"长标签","LabelScrollable":"可滚动标签","LabelWithIcon":"带图标标签",
    "DefLabelWithIcon":"Def 带图标标签","LabelFit":"自适应标签","HyperlinkWithIcon":"带图标超链接",
    "DrawNumberOnMap":"地图数字","DrawStringOnMap":"地图文字","Checkbox":"勾选框","CheckboxLabeled":"带标签勾选框",
    "ToggleInvisibleDraggable":"隐形可拖开关","CheckboxLabeledSelectable":"带标签可选勾选框",
    "GetCheckboxTexture":"取勾选贴图","CheckboxDraw":"绘制勾选框","CheckboxMulti":"三态勾选框",
    "RadioButton":"单选按钮","RadioButtonLabeled":"带标签单选","ButtonText":"文本按钮",
    "ButtonTextDraggable":"可拖文本按钮","DrawButtonGraphic":"绘制按钮图形","DrawRectFast":"快速填充矩形",
    "CustomButtonText":"自定义文本按钮","ButtonTextSubtle":"弱化文本按钮","ButtonImage":"图片按钮",
    "ButtonImageDraggable":"可拖图片按钮","ButtonImageFitted":"自适应图片按钮","ButtonImageWithBG":"带底图图片按钮",
    "CloseButtonFor":"关闭按钮","BackButtonFor":"返回按钮","ButtonInvisible":"隐形按钮",
    "ButtonInvisibleDraggable":"隐形可拖按钮","TextField":"文本框","TextArea":"文本域","TextEntryLabeled":"带标签文本输入",
    "DelayedTextField":"延迟文本框","TextFieldVector":"向量文本框","TextFieldNumeric":"数值文本框",
    "TextFieldNumericLabeled":"带标签数值框","TextFieldPercent":"百分比文本框","HorizontalSlider":"水平滑块",
    "FrequencyHorizontalSlider":"频率水平滑块","IntEntry":"整数输入","FloatRange":"浮点范围","IntRange":"整数范围",
    "QualityRange":"品质范围","FloatRangeWithTypeIn":"浮点范围(可输入)","FillableBar":"填充条",
    "FillableBarLabeled":"带标签填充条","FillableBarChangeArrows":"填充条变化箭头","DraggableBar":"可拖拽条",
    "DrawWindowBackground":"窗口背景","DrawMenuSection":"菜单分区背景","DrawWindowBackgroundTutor":"教程窗口背景",
    "DrawOptionUnselected":"未选选项背景","DrawOptionSelected":"已选选项背景","DrawOptionBackground":"选项背景",
    "DrawShadowAround":"阴影","DrawAtlas":"图集绘制","ToUVRect":"转 UV 矩形","DrawTexturePart":"贴图局部绘制",
    "ScrollHorizontal":"水平滚动","BeginScrollView":"开始滚动视图","AdjustRectsForScrollView":"滚动视图矩形调整",
    "EndScrollView":"结束滚动视图","ColorSelectorIcon":"颜色选择器图标","ColorBox":"颜色框","ColorSelector":"颜色选择器",
    "HSVColorWheel":"HSV 色轮","ColorTemperatureBar":"色温条","ColorTextfields":"颜色文本框",
    "DrawHighlightSelected":"选中高亮","DrawHighlightIfMouseover":"悬停高亮","DrawHighlight":"高亮",
    "DrawLightHighlight":"轻高亮","DrawStrongHighlight":"强高亮","DrawTextHighlight":"文本高亮","DrawTitleBG":"标题背景",
    "InfoCardButton":"信息卡按钮","InfoCardButtonCentered":"居中信息卡按钮","DrawTextureFitted":"适配贴图绘制",
    "DrawTextureRotated":"旋转贴图绘制","NoneLabel":"空标签","MouseAttachedLabel":"跟随鼠标标签",
    "WorldAttachedLabel":"世界锚定标签","Dropdown":"下拉菜单",
}

USAGE = {
    "Label":'Verse.Widgets.Label(rect, "殖民者: " + pawn.Name.ToStringShort);',
    "ButtonText":'if (Verse.Widgets.ButtonText(rect, "确定"))\n{\n    // 点击处理\n}',
    "CheckboxLabeled":'Verse.Widgets.CheckboxLabeled(rect, "启用自动狩猎", ref autoHunt);',
    "RadioButtonLabeled":'autoMode = Verse.Widgets.RadioButtonLabeled(rect, "平衡模式", autoMode == Mode.Balanced);',
    "Checkbox":'Verse.Widgets.Checkbox(topLeft, ref checkOn, 24f, false, false, null, null);',
    "CheckboxMulti":'MultiCheckboxState st = Verse.Widgets.CheckboxMulti(rect, state, false);',
    "RadioButton":'chosen = Verse.Widgets.RadioButton(topLeft, chosen, false);',
    "TextField":'string name = Verse.Widgets.TextField(rect, nameBuffer);',
    "TextFieldNumeric":'Verse.Widgets.TextFieldNumeric(rect, ref count, ref countBuf, 0f, 999f);',
    "TextFieldNumericLabeled":'Verse.Widgets.TextFieldNumericLabeled(rect, "数量", ref count, ref buf, 0f, 999f);',
    "TextFieldPercent":'Verse.Widgets.TextFieldPercent(rect, ref pct, ref buf, 0f, 1f);',
    "HorizontalSlider":'Verse.Widgets.HorizontalSlider(rect, ref volume, 0f, 1f, "音量", 0.05f);',
    "FrequencyHorizontalSlider":'Verse.Widgets.FrequencyHorizontalSlider(rect, freq, 0.5f, 2f, true);',
    "IntEntry":'Verse.Widgets.IntEntry(rect, ref value, ref buf, 1);',
    "FloatRange":'Verse.Widgets.FloatRange(rect, id, ref range, 0f, 1f, "范围", ToStringStyle.FloatTwo, 12f, null, 0.01f);',
    "IntRange":'Verse.Widgets.IntRange(rect, id, ref range, 1, 100, "数量");',
    "QualityRange":'Verse.Widgets.QualityRange(rect, id, ref qRange);',
    "FloatRangeWithTypeIn":'Verse.Widgets.FloatRangeWithTypeIn(rect, id, ref fRange, 0f, 100f, ToStringStyle.FloatOne, "范围");',
    "FillableBar":'Verse.Widgets.FillableBar(new Rect(x, y, 100f, 20f), pawn.health.summaryHealth.Percent);',
    "FillableBarLabeled":'Verse.Widgets.FillableBarLabeled(rect, pawn.needs.food.CurLevelPercentage, 80, "饱食度");',
    "FillableBarChangeArrows":'Verse.Widgets.FillableBarChangeArrows(barRect, changeRate);',
    "DraggableBar":'Verse.Widgets.DraggableBar(barRect, null, null, null, null, ref dragging, barVal, ref target, bands, 1, 0f, 1f);',
    "ThingIcon":'Verse.Widgets.ThingIcon(iconRect, thing, 1f);',
    "DefIcon":'Verse.Widgets.DefIcon(iconRect, def, null, 1f);',
    "GetIconFor":'Texture2D tex = Verse.Widgets.GetIconFor(thingDef, null, null, null);',
    "CanDrawIconFor":'if (Verse.Widgets.CanDrawIconFor(def))\n{\n    Verse.Widgets.DefIcon(rect, def, null, 1f);\n}',
    "InfoCardButton":'if (Verse.Widgets.InfoCardButton(rect, selectedDef))\n{\n    Find.WindowStack.Add(new Dialog_InfoCard(selectedDef));\n}',
    "InfoCardButtonCentered":'if (Verse.Widgets.InfoCardButtonCentered(rect, thing))\n{\n    Find.WindowStack.Add(new Dialog_InfoCard(thing));\n}',
    "Dropdown":'Verse.Widgets.Dropdown(rect, selTarget, t => t, t => GenMenu.Of(t), "选择", null, null, null, null, false);',
    "BeginScrollView":'Verse.Widgets.BeginScrollView(outRect, ref scrollPos, viewRect, true);\n// ... 滚动内容 ...\nVerse.Widgets.EndScrollView();',
    "EndScrollView":'Verse.Widgets.EndScrollView();',
    "ScrollHorizontal":'Verse.Widgets.ScrollHorizontal(outRect, ref scrollPos, viewRect, 20f);',
    "AdjustRectsForScrollView":'Verse.Widgets.AdjustRectsForScrollView(parentRect, ref outRect, ref viewRect);',
    "CloseButtonFor":'if (Verse.Widgets.CloseButtonFor(rect))\n{\n    Close();\n}',
    "BackButtonFor":'if (Verse.Widgets.BackButtonFor(rect))\n{\n    GoBack();\n}',
    "NoneLabel":'Verse.Widgets.NoneLabel(curY, width, "无可用项");',
    "ButtonTextSubtle":'Verse.Widgets.ButtonTextSubtle(rect, "展开", 0.5f, 4f, SoundDefOf.Tick_Tiny, default, null, false);',
    "CustomButtonText":'Verse.Widgets.CustomButtonText(ref rect, "自定义", bg, fg, border, unfilled, true, 1, true, true, 1f);',
    "ButtonImage":'if (Verse.Widgets.ButtonImage(rect, TexButton.Play, Color.white, Color.blue, true, "播放"))\n{\n}',
    "ButtonImageWithBG":'if (Verse.Widgets.ButtonImageWithBG(rect, image, new Vector2(32f, 32f)))\n{\n}',
    "ButtonInvisible":'if (Verse.Widgets.ButtonInvisible(rect, true))\n{\n}',
    "ButtonInvisibleDraggable":'Verse.Widgets.ButtonInvisibleDraggable(rect, true);',
    "ButtonTextDraggable":'DraggableResult r = Verse.Widgets.ButtonTextDraggable(rect, "拖动", true, true, true, null);',
    "ButtonImageDraggable":'Verse.Widgets.ButtonImageDraggable(rect, tex);',
    "ButtonImageFitted":'if (Verse.Widgets.ButtonImageFitted(rect, iconTex))\n{\n}',
    "GetCheckboxTexture":'Texture2D t = Verse.Widgets.GetCheckboxTexture(state);',
    "CheckboxDraw":'Verse.Widgets.CheckboxDraw(x, y, active, disabled, 24f, null, null);',
    "ToggleInvisibleDraggable":'Verse.Widgets.ToggleInvisibleDraggable(rect, ref checkOn, true, false);',
    "CheckboxLabeledSelectable":'selected = Verse.Widgets.CheckboxLabeledSelectable(rect, "选项", ref selected, ref checkOn, icon, 1f);',
    "TextEntryLabeled":'string v = Verse.Widgets.TextEntryLabeled(rect, "名称", buffer);',
    "TextArea":'string note = Verse.Widgets.TextArea(rect, noteBuffer, false);',
    "DelayedTextField":'string s = Verse.Widgets.DelayedTextField(rect, s, ref buf, prevFocus, "FieldName");',
    "TextFieldVector":'Verse.Widgets.TextFieldVector(rect, ref vec, ref buf, -100f, 100f);',
    "LabelWithIcon":'Verse.Widgets.LabelWithIcon(rect, "木材", TexResource.Wood, 1f);',
    "DefLabelWithIcon":'Verse.Widgets.DefLabelWithIcon(rect, def, 4f, 4f);',
    "LabelFit":'bool fit = Verse.Widgets.LabelFit(rect, "简短文本");',
    "LabelEllipses":'Verse.Widgets.LabelEllipses(rect, "过长的文本会被截断显示");',
    "LongLabel":'Verse.Widgets.LongLabel(x, width, "多行说明文本", ref curY, true);',
    "LabelScrollable":'Verse.Widgets.LabelScrollable(rect, longText, ref scrollPos, false, false, false);',
    "LabelCacheHeight":'Verse.Widgets.LabelCacheHeight(ref rect, "缓存高度标签", true, false);',
    "HyperlinkWithIcon":'Verse.Widgets.HyperlinkWithIcon(rect, hyperlink, "查看详情", 4f, 4f, null, false, null);',
    "DrawNumberOnMap":'Verse.Widgets.DrawNumberOnMap(screenPos, 5, Color.white);',
    "DrawStringOnMap":'Verse.Widgets.DrawStringOnMap(screenPos, "X", Color.yellow);',
    "MouseAttachedLabel":'Verse.Widgets.MouseAttachedLabel("拖放到此处", 0f, 0f, null);',
    "WorldAttachedLabel":'Verse.Widgets.WorldAttachedLabel(worldPos, "标记", 0f, 20f, null);',
    "DrawHighlight":'Verse.Widgets.DrawHighlight(cellRect);',
    "DrawHighlightSelected":'Verse.Widgets.DrawHighlightSelected(rect);',
    "DrawHighlightIfMouseover":'Verse.Widgets.DrawHighlightIfMouseover(rect);',
    "DrawLightHighlight":'Verse.Widgets.DrawLightHighlight(rect);',
    "DrawStrongHighlight":'Verse.Widgets.DrawStrongHighlight(rect, null);',
    "DrawTextHighlight":'Verse.Widgets.DrawTextHighlight(rect, 2f, null);',
    "DrawBox":'Verse.Widgets.DrawBox(rect, 1, TexUI.GrayTex);',
    "DrawBoxSolid":'Verse.Widgets.DrawBoxSolid(rect, Color.gray);',
    "DrawBoxSolidWithOutline":'Verse.Widgets.DrawBoxSolidWithOutline(rect, Color.black, Color.white, 1);',
    "DrawLine":'Verse.Widgets.DrawLine(start, end, Color.white, 1f);',
    "DrawLineHorizontal":'Verse.Widgets.DrawLineHorizontal(x, y, length, Color.white);',
    "DrawLineVertical":'Verse.Widgets.DrawLineVertical(x, y, length);',
    "DrawAltRect":'Verse.Widgets.DrawAltRect(rect);',
    "DrawButtonGraphic":'Verse.Widgets.DrawButtonGraphic(rect);',
    "DrawRectFast":'Verse.Widgets.DrawRectFast(rect, Color.red, null);',
    "DrawTextureFitted":'Verse.Widgets.DrawTextureFitted(rect, thingDef.uiIcon, 1f);',
    "DrawTextureRotated":'Verse.Widgets.DrawTextureRotated(center, tex, angle, 1f, null);',
    "DrawTexturePart":'Verse.Widgets.DrawTexturePart(drawRect, uvRect, tex);',
    "ToUVRect":'Rect uv = Verse.Widgets.ToUVRect(rect, texSize);',
    "DrawAtlas":'Verse.Widgets.DrawAtlas(rect, Widgets.ButtonBGAtlas);',
    "DrawWindowBackground":'Verse.Widgets.DrawWindowBackground(windowRect);',
    "DrawMenuSection":'Verse.Widgets.DrawMenuSection(rect);',
    "DrawWindowBackgroundTutor":'Verse.Widgets.DrawWindowBackgroundTutor(rect);',
    "DrawOptionUnselected":'Verse.Widgets.DrawOptionUnselected(rect);',
    "DrawOptionSelected":'Verse.Widgets.DrawOptionSelected(rect);',
    "DrawOptionBackground":'Verse.Widgets.DrawOptionBackground(rect, selected);',
    "DrawShadowAround":'Verse.Widgets.DrawShadowAround(rect);',
    "DrawTitleBG":'Verse.Widgets.DrawTitleBG(rect);',
    "CroppedTerrainTextureRect":'Rect r = Verse.Widgets.CroppedTerrainTextureRect(tex);',
    "ColorBox":'if (Verse.Widgets.ColorBox(rect, ref color, Color.white, 24, 2, null))\n{\n}',
    "ColorSelector":'if (Verse.Widgets.ColorSelector(rect, ref color, baseColors, ref height, icon, 24, 2, null))\n{\n}',
    "ColorSelectorIcon":'Verse.Widgets.ColorSelectorIcon(rect, icon, color, true);',
    "HSVColorWheel":'Verse.Widgets.HSVColorWheel(wheelRect, ref color, ref dragging, null, "ColorPicker");',
    "ColorTemperatureBar":'Verse.Widgets.ColorTemperatureBar(rect, ref color, ref dragging, null);',
    "ColorTextfields":'Verse.Widgets.ColorTextfields(ref aggregator, ref color, ref buffers, ref colorBuf, prevFocus, "Color", ColorComponents.All, ColorComponents.All);',
    "ListSeparator":'Verse.Widgets.ListSeparator(ref curY, width, "分区标题");',
    "BeginGroup":'Verse.Widgets.BeginGroup(rect);\n// ... 组内内容（坐标相对 group 原点） ...\nVerse.Widgets.EndGroup();',
    "EndGroup":'Verse.Widgets.EndGroup();',
}

# 推荐尺寸（按方法名，缺省回退到域默认）
SIZE = {
    "ButtonText":"new Rect(x, y, 120f, 30f)","ButtonTextDraggable":"new Rect(x, y, 120f, 30f)",
    "CustomButtonText":"new Rect(x, y, 120f, 30f)","ButtonTextSubtle":"new Rect(x, y, 160f, 30f)",
    "ButtonImage":"new Rect(x, y, 32f, 32f)","ButtonImageDraggable":"new Rect(x, y, 32f, 32f)",
    "ButtonImageFitted":"new Rect(x, y, 32f, 32f)","ButtonImageWithBG":"new Rect(x, y, 32f, 32f)",
    "ButtonInvisible":"new Rect(x, y, 32f, 32f)","ButtonInvisibleDraggable":"new Rect(x, y, 32f, 32f)",
    "CloseButtonFor":"new Rect(x, y, 24f, 24f)","BackButtonFor":"new Rect(x, y, 80f, 24f)",
    "Checkbox":"new Rect(x, y, 24f, 24f)","CheckboxDraw":"new Rect(x, y, 24f, 24f)",
    "CheckboxLabeled":"new Rect(x, y, 200f, 24f)","CheckboxLabeledSelectable":"new Rect(x, y, 200f, 24f)",
    "ToggleInvisibleDraggable":"new Rect(x, y, 24f, 24f)","CheckboxMulti":"new Rect(x, y, 24f, 24f)",
    "GetCheckboxTexture":"new Rect(x, y, 24f, 24f)",
    "RadioButton":"new Rect(x, y, 20f, 20f)","RadioButtonLabeled":"new Rect(x, y, 200f, 24f)",
    "TextField":"new Rect(x, y, 200f, 24f)","TextArea":"new Rect(x, y, 200f, 80f)",
    "TextEntryLabeled":"new Rect(x, y, 200f, 24f)","DelayedTextField":"new Rect(x, y, 200f, 24f)",
    "TextFieldVector":"new Rect(x, y, 200f, 24f)","TextFieldNumeric":"new Rect(x, y, 120f, 24f)",
    "TextFieldNumericLabeled":"new Rect(x, y, 200f, 24f)","TextFieldPercent":"new Rect(x, y, 120f, 24f)",
    "IntEntry":"new Rect(x, y, 120f, 24f)",
    "HorizontalSlider":"new Rect(x, y, 200f, 22f)","FrequencyHorizontalSlider":"new Rect(x, y, 200f, 22f)",
    "FloatRange":"new Rect(x, y, 200f, 28f)","IntRange":"new Rect(x, y, 200f, 28f)",
    "QualityRange":"new Rect(x, y, 200f, 28f)","FloatRangeWithTypeIn":"new Rect(x, y, 200f, 28f)",
    "Dropdown":"new Rect(x, y, 160f, 24f)",
    "Label":"new Rect(x, y, 200f, 22f)","LabelEllipses":"new Rect(x, y, 200f, 22f)",
    "LongLabel":"new Rect(x, y, width, 44f)","LabelScrollable":"new Rect(x, y, 200f, 60f)",
    "LabelWithIcon":"new Rect(x, y, 200f, 24f)","DefLabelWithIcon":"new Rect(x, y, 200f, 24f)",
    "LabelFit":"new Rect(x, y, 200f, 22f)","LabelCacheHeight":"new Rect(x, y, 200f, 22f)",
    "HyperlinkWithIcon":"new Rect(x, y, 200f, 22f)","NoneLabel":"new Rect(x, y, 200f, 22f)",
    "MouseAttachedLabel":"跟随鼠标定位","WorldAttachedLabel":"世界坐标锚定",
    "DrawNumberOnMap":"地图屏幕坐标","DrawStringOnMap":"地图屏幕坐标",
    "DefIcon":"new Rect(x, y, 48f, 48f)","ThingIcon":"new Rect(x, y, 48f, 48f)",
    "GetIconFor":"返回 Texture2D","CanDrawIconFor":"判定用，无尺寸",
    "CroppedTerrainTextureRect":"返回 Rect",
    "DrawLine":"起止点 Vector2","DrawLineHorizontal":"x,y,length","DrawLineVertical":"x,y,length",
    "DrawBox":"new Rect(x, y, w, h)","DrawBoxSolid":"new Rect(x, y, w, h)",
    "DrawBoxSolidWithOutline":"new Rect(x, y, w, h)","DrawAltRect":"new Rect(x, y, w, h)",
    "DrawButtonGraphic":"new Rect(x, y, w, h)","DrawRectFast":"new Rect(x, y, w, h)",
    "DrawTextureFitted":"new Rect(x, y, w, h)","DrawTextureRotated":"中心 + 角度",
    "DrawTexturePart":"绘制矩形 + UV","ToUVRect":"返回 Rect",
    "DrawHighlight":"new Rect(x, y, w, h)","DrawHighlightSelected":"new Rect(x, y, w, h)",
    "DrawHighlightIfMouseover":"new Rect(x, y, w, h)","DrawLightHighlight":"new Rect(x, y, w, h)",
    "DrawStrongHighlight":"new Rect(x, y, w, h)","DrawTextHighlight":"new Rect(x, y, w, h)",
    "ColorBox":"new Rect(x, y, 24f, 24f)","ColorSelectorIcon":"new Rect(x, y, 24f, 24f)",
    "ColorSelector":"new Rect(x, y, 200f, 200f)","HSVColorWheel":"new Rect(x, y, 120f, 120f)",
    "ColorTemperatureBar":"new Rect(x, y, 200f, 16f)","ColorTextfields":"聚合器驱动",
    "DrawWindowBackground":"new Rect(x, y, w, h)","DrawMenuSection":"new Rect(x, y, w, h)",
    "DrawWindowBackgroundTutor":"new Rect(x, y, w, h)","DrawOptionUnselected":"new Rect(x, y, w, h)",
    "DrawOptionSelected":"new Rect(x, y, w, h)","DrawOptionBackground":"new Rect(x, y, w, h)",
    "DrawShadowAround":"new Rect(x, y, w, h)","DrawAtlas":"new Rect(x, y, w, h)",
    "DrawTitleBG":"new Rect(x, y, w, h)",
    "ListSeparator":"new Rect(x, y, 200f, 20f)",
    "FillableBar":"new Rect(x, y, 120f, 18f)","FillableBarLabeled":"new Rect(x, y, 200f, 22f)",
    "FillableBarChangeArrows":"new Rect(x, y, 120f, 18f)","DraggableBar":"new Rect(x, y, 160f, 16f)",
    "InfoCardButton":"new Rect(x, y, 24f, 24f)","InfoCardButtonCentered":"new Rect(x, y, 24f, 24f)",
    "BeginGroup":"new Rect(x, y, w, h)","EndGroup":"配合 BeginGroup",
    "ScrollHorizontal":"outRect + viewRect","BeginScrollView":"outRect + viewRect",
    "AdjustRectsForScrollView":"父矩形 + 子矩形","EndScrollView":"配合 BeginScrollView",
}
SIZE_DOMAIN_DEFAULT = {
    "text":"new Rect(x, y, 200f, 22f)","visual":"new Rect(x, y, 64f, 64f)",
    "panel":"new Rect(x, y, 200f, 20f)","progress":"new Rect(x, y, 140f, 18f)",
    "input":"new Rect(x, y, 160f, 24f)","official":"new Rect(x, y, 24f, 24f)",
    "layout":"new Rect(x, y, 200f, 120f)","nav":"new Rect(x, y, 80f, 24f)",
}

# 静态示意 demo 类型映射（方法名 -> demo 类型；缺省按域）
DEMO = {
    "Label":"label","LabelEllipses":"label","LongLabel":"label","LabelScrollable":"label",
    "LabelWithIcon":"label","DefLabelWithIcon":"label","LabelFit":"label","LabelCacheHeight":"label",
    "HyperlinkWithIcon":"label","NoneLabel":"label","DrawNumberOnMap":"map","DrawStringOnMap":"map",
    "MouseAttachedLabel":"attached","WorldAttachedLabel":"attached",
    "ButtonText":"btn","ButtonTextDraggable":"btn","CustomButtonText":"btn","ButtonTextSubtle":"btn_sub",
    "ButtonInvisible":"btn","ButtonInvisibleDraggable":"btn","CloseButtonFor":"btn_close","BackButtonFor":"btn_back",
    "ButtonImage":"icon","ButtonImageDraggable":"icon","ButtonImageFitted":"icon","ButtonImageWithBG":"icon",
    "DrawButtonGraphic":"btn",
    "Checkbox":"check","CheckboxLabeled":"check","CheckboxLabeledSelectable":"check","CheckboxMulti":"check",
    "CheckboxDraw":"check","GetCheckboxTexture":"check","ToggleInvisibleDraggable":"check",
    "RadioButton":"radio","RadioButtonLabeled":"radio",
    "TextField":"text","TextArea":"text","TextEntryLabeled":"text","DelayedTextField":"text",
    "TextFieldVector":"text","TextFieldNumeric":"text","TextFieldNumericLabeled":"text","TextFieldPercent":"text",
    "IntEntry":"text",
    "HorizontalSlider":"slider","FrequencyHorizontalSlider":"slider","DraggableBar":"slider",
    "FloatRange":"range","IntRange":"range","QualityRange":"range","FloatRangeWithTypeIn":"range",
    "Dropdown":"dropdown",
    "DefIcon":"icon","ThingIcon":"icon","GetIconFor":"icon","CanDrawIconFor":"icon","CroppedTerrainTextureRect":"icon",
    "DrawTextureFitted":"icon","DrawTextureRotated":"icon","DrawTexturePart":"icon",
    "DrawAltRect":"box","DrawBoxSolid":"box","DrawBoxSolidWithOutline":"box","DrawBox":"box","DrawRectFast":"box","ToUVRect":"box",
    "DrawLine":"line","DrawLineHorizontal":"line","DrawLineVertical":"line",
    "DrawHighlight":"highlight","DrawHighlightSelected":"highlight","DrawHighlightIfMouseover":"highlight",
    "DrawLightHighlight":"highlight","DrawStrongHighlight":"highlight","DrawTextHighlight":"highlight",
    "ColorBox":"color","ColorSelectorIcon":"color","ColorSelector":"color","ColorTextfields":"color",
    "HSVColorWheel":"hsv","ColorTemperatureBar":"hsv",
    "DrawWindowBackground":"window","DrawMenuSection":"window","DrawWindowBackgroundTutor":"window","DrawShadowAround":"window",
    "DrawOptionUnselected":"option","DrawOptionSelected":"option","DrawOptionBackground":"option",
    "DrawAtlas":"atlas","DrawTitleBG":"titlebg",
    "ListSeparator":"separator",
    "FillableBar":"bar","FillableBarLabeled":"bar","FillableBarChangeArrows":"bar",
    "InfoCardButton":"infocard","InfoCardButtonCentered":"infocard",
    "BeginGroup":"box","EndGroup":"box","ScrollHorizontal":"scroll","BeginScrollView":"scroll",
    "AdjustRectsForScrollView":"scroll","EndScrollView":"scroll",
}
DEMO_DOMAIN_DEFAULT = {
    "text":"label","visual":"box","panel":"separator","progress":"bar",
    "input":"btn","official":"infocard","layout":"box","nav":"btn",
}
# 部分 demo 需要带标签/颜色参数
DEMO_ARG = {
    "ButtonText":"确定","ButtonTextDraggable":"拖动","CustomButtonText":"自定义","ButtonTextSubtle":"展开",
    "ButtonInvisible":"隐形按钮","ButtonInvisibleDraggable":"隐形可拖","CloseButtonFor":"✕","BackButtonFor":"← 返回",
    "CheckboxLabeled":"启用自动狩猎","CheckboxLabeledSelectable":"选项",
    "RadioButtonLabeled":"平衡模式","FillableBarLabeled":"饱食度","LabelWithIcon":"木材","DefLabelWithIcon":"物品",
    "TextFieldPercent":"进度","TextFieldNumeric":"数量","TextFieldNumericLabeled":"数量",
    "HorizontalSlider":"音量","FrequencyHorizontalSlider":"频率","FloatRange":"范围","IntRange":"数量","QualityRange":"品质",
    "FloatRangeWithTypeIn":"范围","Dropdown":"选择…","ListSeparator":"分区标题","NoneLabel":"无可用项",
    "MouseAttachedLabel":"跟随鼠标","WorldAttachedLabel":"世界标记","DrawNumberOnMap":"地图数字","DrawStringOnMap":"地图文字",
    "HyperlinkWithIcon":"查看详情","LabelEllipses":"过长文本会被截断","LongLabel":"多行说明文本","LabelCacheHeight":"缓存高度标签",
    "LabelFit":"简短文本","Label":"殖民者：Minh","TextArea":"多行备注…","TextEntryLabeled":"名称","DelayedTextField":"延迟输入",
    "TextFieldVector":"向量","TextField":"输入文本…","IntEntry":"整数","DefLabelWithIcon":"物品","DefIcon":"Def","ThingIcon":"物体",
    "GetIconFor":"Icon","CanDrawIconFor":"可绘制","CroppedTerrainTextureRect":"地形","DrawTextureFitted":"贴图","DrawTextureRotated":"旋转",
    "DrawTexturePart":"局部","ColorBox":"颜色","ColorSelector":"颜色选择","ColorSelectorIcon":"颜色","ColorTextfields":"颜色输入",
    "HSVColorWheel":"色轮","ColorTemperatureBar":"色温","DrawWindowBackground":"窗口","DrawMenuSection":"菜单区",
    "DrawWindowBackgroundTutor":"教程窗","DrawOptionUnselected":"未选","DrawOptionSelected":"已选","DrawOptionBackground":"选项",
    "DrawAtlas":"图集","DrawTitleBG":"标题","DrawAltRect":"交替","DrawBoxSolid":"实心","DrawBoxSolidWithOutline":"描边",
    "DrawBox":"画框","DrawRectFast":"填充","ToUVRect":"UV","DrawLine":"线","DrawLineHorizontal":"水平线",
    "DrawLineVertical":"垂直线","DrawHighlight":"高亮","DrawHighlightSelected":"选中高亮","DrawHighlightIfMouseover":"悬停高亮",
    "DrawLightHighlight":"轻高亮","DrawStrongHighlight":"强高亮","DrawTextHighlight":"文本高亮","DrawShadowAround":"阴影",
    "FillableBar":"填充","FillableBarChangeArrows":"变化箭头","DraggableBar":"拖拽条","InfoCardButton":"信息",
    "InfoCardButtonCentered":"信息","Checkbox":"勾选","CheckboxDraw":"勾选","GetCheckboxTexture":"勾选贴图",
    "ToggleInvisibleDraggable":"开关","CheckboxMulti":"三态","RadioButton":"单选","BeginGroup":"分组","EndGroup":"分组结束",
    "ScrollHorizontal":"滚动","BeginScrollView":"滚动视图","AdjustRectsForScrollView":"矩形调整","EndScrollView":"滚动结束",
}

def demo_html(name):
    kind = DEMO.get(name) or DEMO_DOMAIN_DEFAULT.get(DOMAIN.get(name, "visual"), "box")
    arg = DEMO_ARG.get(name, "")
    if kind == "label":
        return '<div class="vdemo"><span class="vlab">%s</span></div>' % arg
    if kind == "map":
        return '<div class="vdemo"><span class="vlab">%s（地图屏幕坐标叠加）</span></div>' % arg
    if kind == "attached":
        return '<div class="vdemo"><span class="vlab" style="background:#fff3a0;padding:1px 6px">%s</span></div>' % arg
    if kind == "btn":
        return '<div class="vdemo"><span class="vbtn">%s</span> <span class="vbtn vdis">禁用</span></div>' % arg
    if kind == "btn_sub":
        return '<div class="vdemo"><span class="vbtn sub">%s</span></div>' % arg
    if kind == "btn_close":
        return '<div class="vdemo"><span class="vbtn">%s</span></div>' % arg
    if kind == "btn_back":
        return '<div class="vdemo"><span class="vbtn">%s</span></div>' % arg
    if kind == "icon":
        return '<div class="vdemo"><span class="vicon">%s</span></div>' % arg
    if kind == "check":
        return '<div class="vdemo"><span class="vchk on"><i></i>%s</span> <span class="vchk off"><i></i>未选</span></div>' % arg
    if kind == "radio":
        return '<div class="vdemo"><span class="vchk vrad on"><i></i>%s</span> <span class="vchk vrad"><i></i>其它</span></div>' % arg
    if kind == "text":
        return '<div class="vdemo"><span class="vtxt">%s</span> <span class="vtxt empty">空</span></div>' % arg
    if kind == "slider":
        return '<div class="vdemo"><span class="vslider"><i></i></span> <span class="vslider empty"><i></i></span></div>'
    if kind == "range":
        return '<div class="vdemo"><span class="vbar" style="height:10px"><i style="width:40%;left:20%"></i></span></div>'
    if kind == "dropdown":
        return '<div class="vdemo"><div class="vdd"><span>%s</span><span>▾</span></div> <div class="vdd"><span class="sel">已选</span><span>▾</span></div></div>' % arg
    if kind == "box":
        return '<div class="vdemo"><span class="vswbox" style="background:#dcdcdc"></span></div>'
    if kind == "line":
        return '<div class="vdemo"><span style="display:block;width:160px;height:1px;background:#555"></span></div>'
    if kind == "highlight":
        return '<div class="vdemo"><span class="vhl"></span></div>'
    if kind == "color":
        return '<div class="vdemo"><span class="vswbox" style="background:#c33"></span><span class="vswbox" style="background:#3a7a4a"></span><span class="vswbox" style="background:#36c"></span></div>'
    if kind == "hsv":
        return '<div class="vdemo"><span class="vsw"></span></div>'
    if kind == "window":
        return '<div class="vdemo"><span class="vwin"></span></div>'
    if kind == "option":
        return '<div class="vdemo"><span class="vopt">%s</span></div>' % arg
    if kind == "atlas":
        return '<div class="vdemo"><span class="vwin" style="background:#3a3a3a"></span></div>'
    if kind == "titlebg":
        return '<div class="vdemo"><span class="vwin" style="background:#1d1d1d"></span></div>'
    if kind == "separator":
        return '<div class="vsep"><span>%s</span></div>' % arg
    if kind == "bar":
        return '<div class="vdemo"><span class="vbar"><i></i></span> <span class="vbar empty"><i></i></span></div>'
    if kind == "infocard":
        return '<div class="vdemo"><span class="vinfo">i</span></div>'
    if kind == "scroll":
        return '<div class="vscroll">滚动内容区域<br>多行文本示例行 1<br>多行文本示例行 2<br>可滚动…</div>'
    return '<div class="vdemo"><span class="vswbox" style="background:#dcdcdc"></span></div>'

SKIP = {"Painting","ClearLabelCache","ChangeType","ResetSliderDraggingIDs",
        "EnsureMousePositionStackEmpty","WidgetsOnGUI"}

def esc(s):
    return s.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")

def parse_dump(path):
    methods = []
    in_methods = False
    with open(path, encoding="utf-8") as f:
        for line in f:
            s = line.rstrip("\n")
            if s.strip() == "-- methods --":
                in_methods = True
                continue
            if not in_methods:
                continue
            m = re.match(r"\s*\[public static\]\s+(\S.*?)\s+(\w+)\((.*)\)\s*$", s)
            if not m:
                continue
            ret, name, params = m.group(1), m.group(2), m.group(3)
            if name.startswith("get_") or name in SKIP:
                continue
            methods.append((ret, name, params))
    return methods

def split_params(params):
    out, depth, cur = [], 0, ""
    for ch in params:
        if ch == "<":
            depth += 1
        elif ch == ">":
            depth -= 1
        if ch == "," and depth == 0:
            out.append(cur.strip()); cur = ""
        else:
            cur += ch
    if cur.strip():
        out.append(cur.strip())
    return out

def auto_usage(name, params):
    parts = []
    for p in split_params(params):
        if not p:
            continue
        toks = p.split()
        pname = toks[-1].replace("&", "")
        ref = "ref " if "&" in p else ""
        parts.append(ref + pname)
    return "Verse.Widgets.%s(%s);" % (name, ", ".join(parts))

def merged_card(name, overloads):
    """合并同名重载为单卡：列出全部重载签名 + 用法（不丢方法）。
    overloads: list of (ret, params)，name 为公共方法名。"""
    d = DOMAIN.get(name, "visual")
    cn = CN.get(name, name)
    tag = DOMAIN_TAG.get(d, d)
    sig_lines = []
    usage_blocks = []
    for idx, (ret, params) in enumerate(overloads):
        sig_lines.append('        <div class="sig">%s</div>' % esc("%s %s(%s)" % (ret, name, params)))
        # 典型用法：首重载优先取手写 USAGE；其余重载用 auto_usage 生成，保证每个重载都有代码示例
        if idx == 0 and name in USAGE:
            usage_blocks.append(USAGE[name])
        else:
            usage_blocks.append(auto_usage(name, params))
    sigs_html = "\n".join(sig_lines)
    size = SIZE.get(name) or SIZE_DOMAIN_DEFAULT.get(d, "new Rect(x, y, 120f, 24f)")
    demo = demo_html(name)
    usage_esc = esc("\n\n".join(usage_blocks))
    n = len(overloads)
    ubtn_label = ("原版用法（%d 个重载） ▼" % n) if n > 1 else "原版用法 ▼"
    desc = ("原版 <b>Verse.Widgets.%s</b> —— 非 RUIF 标准组件，仅作 API 对照参考，"
            "调用需引入 RimWorld 程序集（Assembly-CSharp）。本卡合并 %d 个重载签名（不丢方法），"
            "下图为静态示意，非运行时真实渲染。") % (name, n)
    html = (
        '      <div class="comp vanilla">\n'
        '        <div class="ch"><b><span class="cn">%s</span> · Verse.Widgets.%s</b>'
        '<span class="tag">%s</span><span class="badge b-vanilla">原版 Widgets</span></div>\n'
        '%s\n'
        '        <div class="size">推荐尺寸：<code>%s</code></div>\n'
        '        <div class="demo">%s</div>\n'
        '        <button class="ubtn" type="button" aria-expanded="false">%s</button>\n'
        '        <div class="real hide"><button class="cbtn">复制</button>'
        '<pre class="copy">%s</pre></div>\n'
        '        <p class="desc">%s</p>\n'
        '      </div>\n' % (cn, name, tag, sigs_html, size, demo, ubtn_label, usage_esc, desc)
    )
    return d, html

def strip_vanilla(src):
    """移除所有已注入的 .comp.vanilla 卡片（按平衡 div 匹配）。"""
    out = []
    i = 0
    n = len(src)
    open_tag = '<div class="comp vanilla">'
    while i < n:
        idx = src.find(open_tag, i)
        if idx == -1:
            out.append(src[i:]); break
        out.append(src[i:idx])
        pos = idx + len(open_tag)
        depth = 1
        j = pos
        while j < n:
            oi = src.find('<div', j)
            ci = src.find('</div>', j)
            if ci == -1:
                break
            if oi != -1 and oi < ci:
                depth += 1; j = oi + 4
            else:
                depth -= 1; j = ci + 6
                if depth == 0:
                    break
        while j < n and src[j] in '\r\n':
            j += 1
        i = j
    return "".join(out)

def insert_into_grid(src, domain_id, cards_html):
    marker = 'class="domain" id="%s"' % domain_id
    start = src.find(marker)
    if start == -1:
        return src, False
    grid_open = src.find('<div class="grid">', start)
    if grid_open == -1:
        return src, False
    pos = grid_open + len('<div class="grid">')
    depth = 1
    i = pos
    n = len(src)
    while i < n:
        oi = src.find("<div", i)
        ci = src.find("</div>", i)
        if ci == -1:
            break
        if oi != -1 and oi < ci:
            depth += 1
            i = oi + 4
        else:
            depth -= 1
            i = ci + 6
            if depth == 0:
                return src[:ci] + cards_html + src[ci:], True
    return src, False

def main():
    methods = parse_dump(DUMP)
    print("解析到原版组件方法: %d" % len(methods))
    # 按方法名聚合同名重载（不丢方法，仅合并展示）
    groups = {}
    order = []
    for (ret, name, params) in methods:
        if name not in groups:
            groups[name] = []
            order.append(name)
        groups[name].append((ret, params))
    merged = len(order)
    overloaded = sum(1 for k in order if len(groups[k]) > 1)
    print("合并重载后独立方法: %d（含重载的方法 %d 个，原 %d 个签名）"
          % (merged, overloaded, len(methods)))
    by_domain = {}
    for name in order:
        d, h = merged_card(name, groups[name])
        by_domain.setdefault(d, []).append(h)

    with open(HTML, encoding="utf-8") as f:
        src = f.read()

    before_vanilla = src.count('<div class="comp vanilla">')
    src = strip_vanilla(src)
    after_strip = src.count('<div class="comp vanilla">')
    print("剥离旧原版卡: %d -> %d" % (before_vanilla, after_strip))

    total = 0
    for d in sorted(by_domain):
        cards = "".join(by_domain[d])
        before = src.count('<div class="comp')
        src, ok = insert_into_grid(src, d, cards)
        after = src.count('<div class="comp')
        print("  域 %-9s 注入 %2d 张 -> %s (comp 计数 %d->%d)" %
              (d, len(by_domain[d]), "OK" if ok else "FAIL", before, after))
        total += len(by_domain[d])
        if not ok:
            print("  !! 域 %s 注入失败" % d)

    with open(HTML, "w", encoding="utf-8") as f:
        f.write(src)
    print("完成：共注入 %d 张原版组件卡（含 .size + .demo 成品化）" % total)

if __name__ == "__main__":
    main()
