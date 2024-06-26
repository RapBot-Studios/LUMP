// Copyright (C) 2022 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Threading;
using UnityEngine;

using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Api
{
    internal class Utils
    {
        /// <summary>
        /// Creates and return a client factory instance of the identified platform.
        /// </summary>
        internal static IUmpClientFactory GetClientFactory()
        {
            String typeName = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                typeName = "GoogleMobileAds.Ump.iOS.UmpClientFactory,GoogleMobileAds.Ump.iOS";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                typeName =
                        "GoogleMobileAds.Ump.Android.UmpClientFactory,GoogleMobileAds.Ump.Android";
            }
            else
            {
                typeName = "GoogleMobileAds.Ump.Unity.UmpClientFactory,GoogleMobileAds.Ump.Unity";
            }
            Type type = Type.GetType(typeName);
            return (IUmpClientFactory)Activator.CreateInstance(type);
        }
        private static SynchronizationContext _synchronizationContext;
        private static int _unityMainThreadId;
        public static void SetUnityMainThreadSynchronizationContext()
        {
            // Cache the Threading variables for RaiseAdEventsOnUnityMainThread.
            // SynchronizationContext.Current is null if initialized from a background thread.
            _synchronizationContext = SynchronizationContext.Current;
            _unityMainThreadId = Thread.CurrentThread.ManagedThreadId;
        }
        public static void RaiseAction(Action action)
        {
            SetUnityMainThreadSynchronizationContext();
            if (action == null)
            {
                return;
            }
            if (_synchronizationContext == null ||
                Thread.CurrentThread.ManagedThreadId == _unityMainThreadId)
            {
                action();
                return;
            }

            _synchronizationContext.Post((state) =>
            {
                action();
            }, action);
        }
    }
}