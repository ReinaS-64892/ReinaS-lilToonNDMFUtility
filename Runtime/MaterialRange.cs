#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace lilToonNDMFUtility
{
    [Serializable]
    public class MaterialRange
    {
        public Mode RangeMode;
        public enum Mode
        {
            All,
            ParentObject,
            Manual,
        }
        public List<AvatarObjectReference?> ParentObjects = new();
        public List<Material?> ManualSelections = new();
    }

    internal interface ITargetMaterialRange
    {
        MaterialRange TargetMaterialRange { get; }
    }
}
