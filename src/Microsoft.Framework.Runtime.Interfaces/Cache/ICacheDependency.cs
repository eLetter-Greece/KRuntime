using System;

namespace Microsoft.Framework.Runtime
{
    [AssemblyNeutral]
    public interface ICacheDependency
    {
        bool HasChanged { get; }
    }
}