using System;
using System.Collections.Generic;

namespace RUIF
{
    /// <summary>
    /// 翻译结果的<b>有界缓存</b> —— 落地 PERF-001（绘制循环不得重复本地化解析）
    /// 与 PERF-002（缓存必须有来源、失效条件和容量上限）。
    ///
    /// <para><b>为什么必须有</b>：RUIF 组件是即时模式绘制，每个 <c>Tr.Get</c> 调用点每帧执行一次
    /// （v2.1.1 体检实测：41 处无参 + 21 处带参，全部位于 <c>Draw.Label</c>/<c>ButtonText</c> 的实参位置）。
    /// 引擎的 <c>"key".Translate()</c> 每次都要在 <c>LanguageDatabase</c> 的语言字典里查找；
    /// 带参重载还会走 <c>Formatted</c> 做占位符替换并<b>分配新字符串</b>。缓存命中后两者都消失。</para>
    ///
    /// <para><b>三要素（审计要求）</b>：
    /// ① <b>来源</b>：构造时注入的 factory 委托（唯一权威仍是 <c>LanguageDatabase</c>，缓存只加速，不产生新事实）；
    /// ② <b>失效</b>：构造时注入的语言戳委托，其返回值引用变化即整体清空（语言切换）；
    /// ③ <b>上限</b>：条目数达构造参数 capacity 即清空重建，绝不无界增长。</para>
    ///
    /// <para>⚠️ <b>只缓存 int 参数的复合键</b>：int 键可用无分配结构体承载；字符串参数的复合键需要拼接
    /// （拼接本身即一次分配），与省下的 <c>Formatted</c> 分配净收益相当，故不缓存 —— 该取舍记录在
    /// <c>docs/API参考.md</c> 与体检报告的性能结论里。</para>
    ///
    /// <para>⚠️ <b>可离线单测</b>：本类不引用 UnityEngine/Verse（语言戳与取值都由委托注入），
    /// 因此单测可用假语言戳与假 factory 验证命中 / 失效 / 上限三种语义。</para>
    /// </summary>
    internal sealed class TrCache
    {
        private readonly int capacity;
        private readonly Func<object> langProvider;
        private readonly Dictionary<string, string> plain;
        private readonly Dictionary<IntKey, string> withInt;

        private object langStamp;
        private bool stampBound;

        internal TrCache(int capacity, Func<object> langProvider)
        {
            if (capacity < 1) throw new ArgumentOutOfRangeException("capacity", "capacity must be >= 1");
            this.capacity = capacity;
            this.langProvider = langProvider ?? (() => null);
            plain = new Dictionary<string, string>(Math.Min(capacity, 64));
            withInt = new Dictionary<IntKey, string>(Math.Min(capacity, 64));
        }

        /// <summary>当前条目数（恒 ≤ capacity）。</summary>
        internal int Count { get { return plain.Count + withInt.Count; } }

        /// <summary>容量上限。</summary>
        internal int Capacity { get { return capacity; } }

        /// <summary>因语言戳变化而整体失效的次数（语言切换可见化，供诊断与测试断言）。</summary>
        internal int LanguageInvalidations { get; private set; }

        /// <summary>因达到容量上限而清空重建的次数（持续增长说明容量偏小或键空间过大）。</summary>
        internal int CapacityFlushes { get; private set; }

        /// <summary>无参取值：命中直接返回同一 string 实例（零查找、零分配）。</summary>
        internal string Get(string key, Func<string, string> factory)
        {
            Sync();
            string cached;
            if (plain.TryGetValue(key, out cached)) return cached;
            string value = factory(key);
            if (value == null) return null;   // 不缓存 null：避免把"翻译环境暂缺"固化成永久结果
            Store();
            plain[key] = value;
            return value;
        }

        /// <summary>int 单参取值：无分配复合键（绘制路径主力）。</summary>
        internal string Get(string key, int value, Func<string, int, string> factory)
        {
            Sync();
            IntKey composite = new IntKey(key, value);
            string cached;
            if (withInt.TryGetValue(composite, out cached)) return cached;
            string translated = factory(key, value);
            if (translated == null) return null;
            Store();
            withInt[composite] = translated;
            return translated;
        }

        private void Store()
        {
            if (plain.Count + withInt.Count < capacity) return;
            plain.Clear();
            withInt.Clear();
            CapacityFlushes++;
        }

        private void Sync()
        {
            object current = langProvider();
            if (stampBound && ReferenceEquals(current, langStamp)) return;
            if (stampBound) LanguageInvalidations++;
            stampBound = true;
            langStamp = current;
            plain.Clear();
            withInt.Clear();
        }

        /// <summary>无分配的 int 复合键（避免"键 + 参数"拼接产生新字符串）。</summary>
        private struct IntKey : IEquatable<IntKey>
        {
            private readonly string key;
            private readonly int value;

            internal IntKey(string key, int value)
            {
                this.key = key;
                this.value = value;
            }

            public bool Equals(IntKey other)
            {
                return value == other.value && string.Equals(key, other.key, StringComparison.Ordinal);
            }

            public override bool Equals(object obj)
            {
                return obj is IntKey && Equals((IntKey)obj);
            }

            public override int GetHashCode()
            {
                return (key == null ? 0 : key.GetHashCode()) * 397 ^ value;
            }
        }
    }
}
