using System;
using System.Collections.Generic;

namespace Rehawk.DOTweenSequencing.Editor
{
    [Serializable]
    public class TweenStepClipboardData
    {
        public string AssemblyQualifiedTypeName;
        public string Json;
        
        public List<ObjectRefEntry> ObjectRefs = new();
    }

    [Serializable]
    public struct ObjectRefEntry
    {
        public string RelativePath;
        public UnityEngine.Object Value;
    }
}