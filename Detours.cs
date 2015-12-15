﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EightyOne.Areas;
using EightyOne.Redirection;
using EightyOne.ResourceManagers;
using EightyOne.Terrain;
using EightyOne.Zones;

namespace EightyOne
{
    public static class Detours
    {
        private static Dictionary<MethodInfo, RedirectCallsState> redirectsOnLoaded;
        private static Dictionary<MethodInfo, RedirectCallsState> redirectsOnCreated;
        public static bool IsEnabled;

        public static void SetUp()
        {
            Redirect(true);
        }

        public static void Deploy()
        {
            if (IsEnabled)
            {
                return;
            }
            IsEnabled = true;
            FakeWaterManager.Init();
            FakeDistrictManager.Init();
            FakeDistrictTool.Init();
            FakeImmaterialResourceManager.Init();
            FakeTerrainManager.Init();
            FakeZoneManager.Init();
            FakeZoneTool.Init();
            FakeElectricityManager.Init();
            Redirect(false);
            FakeGameAreaManager.Init();
        }

        public static void Redirect(bool onCreated)
        {
            if (onCreated)
            {
                redirectsOnCreated = new Dictionary<MethodInfo, RedirectCallsState>();
            }
            else
            {
                redirectsOnLoaded = new Dictionary<MethodInfo, RedirectCallsState>();
            }
            var redirects = onCreated ? redirectsOnCreated : redirectsOnLoaded;
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var customAttributes = type.GetCustomAttributes(typeof(TargetType), false);
                if (customAttributes.Length != 1)
                {
                    continue;
                }
                var targetType = ((TargetType)customAttributes[0]).Type;
                foreach (
                    var method in
                        type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                        BindingFlags.NonPublic)
                            .Where(method =>
                            {
                                var attributes = method.GetCustomAttributes(typeof(RedirectMethodAttribute), false);
                                if (attributes.Length != 1)
                                {
                                    return false;
                                }
                                return ((RedirectMethodAttribute)attributes[0]).OnCreated == onCreated;
                            }))
                {
                    var tuple = RedirectionUtil.RedirectMethod(targetType, method);
                    redirects.Add(tuple.First, tuple.Second);
                }
            }
        }

        private static void RevertRedirect(bool onCreated)
        {
            var redirects = onCreated ? redirectsOnCreated : redirectsOnLoaded;
            foreach (var kvp in redirects)
            {
                RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);
            }
            redirects.Clear();
        }

        public static void Revert()
        {
            if (!IsEnabled)
            {
                return;
            }
            IsEnabled = false;
            RevertRedirect(false);
            FakeImmaterialResourceManager.OnDestroy();
            FakeDistrictManager.OnDestroy();
            FakeWaterManager.OnDestroy();
            FakeElectricityManager.OnDestroy();
        }

        public static void TearDown()
        {
            RevertRedirect(true);
        }
    }
}