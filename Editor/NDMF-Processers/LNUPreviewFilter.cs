using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using nadena.dev.ndmf.preview;
using UnityEngine;

namespace lilToonNDMFUtility
{
    internal abstract class LNUPreviewFilter<TLUNRangedComponent> : IRenderFilter
    where TLUNRangedComponent : Behaviour, ITargetMaterialRange
    {
        public ImmutableList<RenderGroup> GetTargetGroups(ComputeContext context)
        {
            var renderGroups = new List<RenderGroup>();
            foreach (var ar in context.GetAvatarRoots())
            {
                var ctx = new LNUPreviewGroupingContext(context, ar);
                var unificatorList = context.GetComponentsInChildren<TLUNRangedComponent>(ar, true).Where(b => context.Observe(b, ob => ob.enabled));
                var u2g = new Dictionary<TLUNRangedComponent, HashSet<Renderer>>();

                foreach (var unificator in unificatorList)
                    u2g[unificator] = ctx.GetCtxRenderers(ctx.GetCtxMaterials(unificator).ToHashSet()).ToHashSet();

                var groups = GetRendererGrouping(u2g);
                var orderDictionary = GetOrderDictionary(unificatorList);

                foreach (var group in groups)
                    renderGroups.Add(RenderGroup.For(group.Key).WithData<FilterPassingData>(new(group.Value, orderDictionary)));
            }
            return renderGroups.ToImmutableList();
        }

        private static Dictionary<IEnumerable<Renderer>, HashSet<TLUNRangedComponent>>
            GetRendererGrouping(
                Dictionary<TLUNRangedComponent, HashSet<Renderer>> targetRendererGroup
                )
        {
            var renderer2Behavior = new Dictionary<Renderer, HashSet<TLUNRangedComponent>>();

            foreach (var targetKV in targetRendererGroup)
            {
                var thisTTGroup = new HashSet<TLUNRangedComponent>() { { targetKV.Key } };
                var thisGroupTarget = new HashSet<Renderer>();
                foreach (var target in targetKV.Value)
                {
                    if (renderer2Behavior.ContainsKey(target))
                    {
                        var ttbGroup = renderer2Behavior[target];
                        thisGroupTarget.UnionWith(renderer2Behavior.Where(i => i.Value == ttbGroup).Select(i => i.Key));//同じ TTB が紐づくレンダラーを集める
                        thisTTGroup.UnionWith(ttbGroup);
                    }
                    else { thisGroupTarget.Add(target); }
                }

                foreach (var t in thisGroupTarget) { renderer2Behavior[t] = thisTTGroup; }
            }

            var grouping = new Dictionary<IEnumerable<Renderer>, HashSet<TLUNRangedComponent>>();
            foreach (var group in renderer2Behavior.Values.Distinct()) { grouping.Add(renderer2Behavior.Where(i => i.Value == group).Select(i => i.Key), group); }
            return grouping;
        }
        private Dictionary<TLUNRangedComponent, int> GetOrderDictionary(IEnumerable<TLUNRangedComponent> components)
        {
            var componentsIndex = new Dictionary<TLUNRangedComponent, int>();

            var index = 0;
            foreach (var b in components)
            {
                componentsIndex[b] = index;
                index += 1;
            }

            return componentsIndex;
        }

        internal class FilterPassingData
        {
            public HashSet<TLUNRangedComponent> targetComponents;
            public Dictionary<TLUNRangedComponent, int> componentExecutingOrder;

            public FilterPassingData(HashSet<TLUNRangedComponent> targetComponents, Dictionary<TLUNRangedComponent, int> componentExecutingOrder)
            {
                this.targetComponents = targetComponents;
                this.componentExecutingOrder = componentExecutingOrder;
            }
        }
        internal class PreviewContextRecallerNode : IRenderFilterNode
        {
            private LNUPreviewContext _lnuPreviewContext;

            public PreviewContextRecallerNode(LNUPreviewContext lnuPreviewContext) { _lnuPreviewContext = lnuPreviewContext; }
            public RenderAspects WhatChanged => RenderAspects.Material;
            public void OnFrame(Renderer original, Renderer proxy) { _lnuPreviewContext.ReplaceRecall(proxy); }

            void IDisposable.Dispose()
            {
                _lnuPreviewContext.Dispose();
            }
        }
        public abstract Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context);
    }

}
