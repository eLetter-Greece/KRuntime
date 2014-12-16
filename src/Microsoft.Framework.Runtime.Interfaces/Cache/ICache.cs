using System;

namespace Microsoft.Framework.Runtime
{
    [AssemblyNeutral]
    public interface ICache
    {
        object Get(object key, Func<ICacheContext, object> factory);

        object Get(object key, Func<ICacheContext, object, object> factory);
    }
}