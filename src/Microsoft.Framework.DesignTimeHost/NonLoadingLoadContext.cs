﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.Framework.Runtime;

namespace Microsoft.Framework.DesignTimeHost
{
    public class NonLoadingLoadContext : IAssemblyLoadContext
    {
        public Assembly LoadFile(string path)
        {
            AssemblyPath = path;
            return null;
        }

        public Assembly LoadStream(Stream assemblyStream, Stream pdbStream)
        {
            var ms = new MemoryStream((int)assemblyStream.Length);
            assemblyStream.CopyTo(ms);
            AssemblyBytes = ms.ToArray();

            if (pdbStream != null)
            {
                ms = new MemoryStream((int)assemblyStream.Length);
                pdbStream.CopyTo(ms);
                PdbBytes = ms.ToArray();
            }

            return null;
        }

        public Assembly Load(string name)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {

        }

        public byte[] AssemblyBytes { get; set; }
        public byte[] PdbBytes { get; set; }

        public string AssemblyPath { get; set; }
    }
}