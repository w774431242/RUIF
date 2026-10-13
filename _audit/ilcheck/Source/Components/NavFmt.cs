using UnityEngine;

namespace RUIF
{
    /// <summary>相对时间档位（**语义**，与文案解耦；供消费者自定义渲染，也让分档可离线单测）。</summary>
    public enum TimeBucket { JustNow, Minutes, Hours, Days }

    /// <summary>导航原子的纯计算（可离线单测）。全部无副作用，不触碰 IMGUI 全局状态。</summary>
    public static class NavFmt
    {
        /// <summary>面包屑分隔符。</summary>
        public const string Separator = "›";

        /// <summary>导航返回栈容量上限。</summary>
        public const int StackLimit = 32;

        /// <summary>标签宽度估算单位数：CJK / 全角记 2，其余记 1（离线可测，避免每帧 CalcSize 分配）。</summary>
        public static int LabelUnits(string label)
        {
            if (string.IsNullOrEmpty(label)) return 0;
            int units = 0;
            for (int i = 0; i < label.Length; i++)
                units += label[i] > 0x2E7F ? 2 : 1;
            return units;
        }

        /// <summary>页签估算宽度：单位数 × unitWidth + 两侧留白。</summary>
        public static float TabWidth(string label, float unitWidth, float padPerTab)
            => LabelUnits(label) * unitWidth + padPerTab;

        /// <summary>总页数（total/pageSize 任一非正时视为 1 页）。</summary>
        public static int TotalPages(int total, int pageSize)
            => total <= 0 || pageSize <= 0 ? 1 : (total + pageSize - 1) / pageSize;

        /// <summary>页码钳制。</summary>
        public static int ClampPage(int page, int totalPages)
            => Mathf.Clamp(page, 0, Mathf.Max(0, totalPages - 1));

        /// <summary>
        /// 分页按钮序列（-1 = 省略号）：恒含首页/末页与当前 ±1，间隔折叠为省略号。
        /// 写入调用方缓冲零分配；总页数不超过缓冲时全量展开，超出缓冲时截断。
        /// </summary>
        public static void PageList(int totalPages, int page, int[] buf, out int count)
        {
            count = 0;
            if (buf == null || buf.Length == 0) return;
            totalPages = Mathf.Max(1, totalPages);
            page = ClampPage(page, totalPages);

            if (totalPages <= buf.Length)
            {
                for (int i = 0; i < totalPages; i++) buf[count++] = i;
                return;
            }

            Add(buf, ref count, 0);
            int lo = Mathf.Max(1, page - 1);
            int hi = Mathf.Min(totalPages - 2, page + 1);
            if (lo > 1) Add(buf, ref count, -1);
            for (int i = lo; i <= hi; i++) Add(buf, ref count, i);
            if (hi < totalPages - 2) Add(buf, ref count, -1);
            Add(buf, ref count, totalPages - 1);
        }

        private static void Add(int[] buf, ref int count, int value)
        {
            if (count < buf.Length) buf[count++] = value;
        }

        /// <summary>
        /// 模糊匹配分：子序列命中（忽略大小写），连续命中递增加分，首字符前缀额外加成；不命中返回 -1。
        /// 空查询视为全量命中（0 分）。
        /// </summary>
        public static int FuzzyScore(string query, string target)
        {
            if (string.IsNullOrEmpty(query)) return 0;
            if (string.IsNullOrEmpty(target)) return -1;
            string q = query.ToLowerInvariant();
            string t = target.ToLowerInvariant();
            int score = 0, from = 0, run = 0;
            for (int qi = 0; qi < q.Length; qi++)
            {
                char c = q[qi];
                if (c == ' ') continue;
                int hit = t.IndexOf(c, from);
                if (hit < 0) return -1;
                run = hit == from ? run + 2 : 1;
                if (hit == 0) score += 3;
                score += run;
                from = hit + 1;
            }
            return score;
        }

        /// <summary>相对时间档位（**语义**，与文案解耦；负数按 0 处理）。</summary>
        public static TimeBucket TimeBucketOf(float ageSec)
        {
            if (ageSec < 0f) ageSec = 0f;
            if (ageSec < 60f) return TimeBucket.JustNow;
            if (ageSec < 3600f) return TimeBucket.Minutes;
            if (ageSec < 86400f) return TimeBucket.Hours;
            return TimeBucket.Days;
        }

        /// <summary>相对时间文案（JumpList 用）：刚刚 / Xm / Xh / Xd —— 已本地化（<c>RUIF.Comp.Relative.*</c>）。</summary>
        public static string RelativeTime(float ageSec)
        {
            float a = ageSec < 0f ? 0f : ageSec;
            switch (TimeBucketOf(a))
            {
                case TimeBucket.JustNow: return Tr.Get("RUIF.Comp.Relative.JustNow");
                case TimeBucket.Minutes: return Tr.Get("RUIF.Comp.Relative.Minutes", Mathf.FloorToInt(a / 60f));
                case TimeBucket.Hours: return Tr.Get("RUIF.Comp.Relative.Hours", Mathf.FloorToInt(a / 3600f));
                default: return Tr.Get("RUIF.Comp.Relative.Days", Mathf.FloorToInt(a / 86400f));
            }
        }

        /// <summary>帧间趋近（侧栏宽度/指示条插值）：smooth ∈ [0,1]，1 立即到位。</summary>
        public static float Approach(float current, float target, float smooth)
            => Mathf.Lerp(current, target, Mathf.Clamp01(smooth));

        /// <summary>树节点计数（含自身与全部后代）。</summary>
        public static int CountNodes(TreeNode node)
        {
            if (node == null) return 0;
            int n = 1;
            var children = node.Children;
            if (children != null)
                for (int i = 0; i < children.Count; i++)
                    n += CountNodes(children[i]);
            return n;
        }
    }
}
