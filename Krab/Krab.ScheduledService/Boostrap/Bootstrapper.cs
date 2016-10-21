﻿using Krab.ScheduledService.Jobs;
using log4net;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;

namespace Krab.ScheduledService.Boostrap
{
    public static class Bootstrapper
    {
        public static void Configure()
        {
            var container = new UnityContainer();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            RegisterInstances(container);
        }

        private static void RegisterInstances(IUnityContainer container)
        {
            container.RegisterInstance(typeof(ILog), LogManager.GetLogger("ServiceLogger"));

            Configuration.Register(container);
            DataAccess.Configuration.Register(container);
            Caching.Configuration.Register(container);
            Api.Configuration.Register(container);
            Global.Configuration.Register(container);

            TryGetInstances();
        }

        private static void TryGetInstances()
        {
            var locator = ServiceLocator.Current;
            
            if (locator == null)
            {
                throw new Exception("Unable to find Service Locator!");
            }

            var logger = locator.GetInstance<ILog>();

            var types = new List<Type>
            {
                typeof(IDeleteLogs),
                typeof(IProcessKeywordResponseSets)
            };

            foreach (var type in types)
            {
                try
                {
                    locator.GetInstance(type);
                }
                catch (Exception ex)
                {
                    logger.Warn($"Unable to find instane of {type.Name}!");
                    logger.Error($"Unable to find instane of {type.Name}!", ex);

#if DEBUG
                    throw;
#endif
                }
            }
        }
    }
}
