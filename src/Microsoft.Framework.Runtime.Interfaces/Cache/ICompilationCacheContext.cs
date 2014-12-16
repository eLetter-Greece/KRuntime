using System;

namespace Microsoft.Framework.Runtime
{
    [AssemblyNeutral]
    public interface ICacheContext
    {
        object Key { get; }

        Action<ICacheDependency> Monitor { get; }
    }
}