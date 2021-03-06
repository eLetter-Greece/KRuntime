// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Framework.DependencyInjection.ServiceLookup;

namespace Microsoft.Framework.Runtime.Common.DependencyInjection
{
    internal class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, ServiceEntry> _entries = new Dictionary<Type, ServiceEntry>();
        private readonly IServiceProvider _fallbackServiceProvider;

        public ServiceProvider()
        {
            Add(typeof(IServiceProvider), this, includeInManifest: false);
            Add(typeof(IServiceManifest), new ServiceManifest(this), includeInManifest: false);
        }

        public ServiceProvider(IServiceProvider fallbackServiceProvider)
            : this()
        {
            _fallbackServiceProvider = fallbackServiceProvider;
        }

        public void Add(Type type, object instance)
        {
            Add(type, instance, includeInManifest: true);
        }

        public void Add(Type type, object instance, bool includeInManifest)
        {
            _entries[type] = new ServiceEntry
            {
                Instance = instance,
                IncludeInManifest = includeInManifest
            };
        }

        public object GetService(Type serviceType)
        {
            ServiceEntry entry;
            if (_entries.TryGetValue(serviceType, out entry))
            {
                return entry.Instance;
            }

            Array serviceArray = GetServiceArrayOrNull(serviceType);

            if (serviceArray != null && serviceArray.Length != 0)
            {
                return serviceArray;
            }

            if (_fallbackServiceProvider != null)
            {
                return _fallbackServiceProvider.GetService(serviceType);
            }

            return serviceArray;
        }

        private Array GetServiceArrayOrNull(Type serviceType)
        {
            var typeInfo = serviceType.GetTypeInfo();

            if (typeInfo.IsGenericType &&
                serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var itemType = typeInfo.GenericTypeArguments[0];

                ServiceEntry entry;
                if (_entries.TryGetValue(itemType, out entry))
                {
                    var serviceArray = Array.CreateInstance(itemType, 1);
                    serviceArray.SetValue(entry.Instance, 0);
                    return serviceArray;
                }
                else
                {
                    return Array.CreateInstance(itemType, 0);
                }
            }

            return null;
        }

        private IEnumerable<Type> GetManifestServices()
        {
            var services = _entries.Where(p => p.Value.IncludeInManifest)
                                   .Select(p => p.Key);

            var fallbackManifest = _fallbackServiceProvider?.GetService(typeof(IServiceManifest)) as IServiceManifest;

            if (fallbackManifest != null)
            {
                return fallbackManifest.Services.Concat(services);
            }

            return services;
        }

        private class ServiceEntry
        {
            public object Instance { get; set; }
            public bool IncludeInManifest { get; set; }
        }

        private class ServiceManifest : IServiceManifest
        {
            private readonly ServiceProvider _serviceProvider;

            public ServiceManifest(ServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public IEnumerable<Type> Services
            {
                get
                {
                    return _serviceProvider.GetManifestServices().Distinct();
                }
            }
        }
    }
}
