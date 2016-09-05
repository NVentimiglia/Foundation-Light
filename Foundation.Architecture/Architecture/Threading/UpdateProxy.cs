using System;
using UnityEngine;

namespace SignalMQ.Infrastructure.Tools
{
    /// <summary>
    /// Unity Specific
    /// </summary>
    [AddComponentMenu("SignalMq/UpdateProxy")]
    public class UpdateProxy : MonoBehaviour, IDisposable
    {
        private static GameObject root;

        private bool isDisposed;

        static UpdateProxy()
        {
            root = new GameObject("_UpdateProxy");
            DontDestroyOnLoad(root);
        }

        /// <summary>
        /// Creates a new UpdateHelper
        /// </summary>
        /// <param name="onUpdate"></param>
        /// <returns></returns>
        public static UpdateProxy Get(Action onUpdate)
        {
            var helper = root.AddComponent<UpdateProxy>();
            helper.OnUpdate = onUpdate;
            helper.enabled = onUpdate != null;
            return helper;
        }
        

        /// <summary>
        /// Destroys the instance
        /// </summary>
        public void Dispose()
        {
            isDisposed = true;
            ThreadHelper.OnMainThread(DisposeInternal);
        }

        void DisposeInternal()
        {
            enabled = false;
            Destroy(this);
        }


        /// <summary>
        /// The update delegate
        /// </summary>
        public Action OnUpdate;

        /// <summary>
        /// Mono
        /// </summary>
        void Update()
        {
            if (!isDisposed)
            {
                OnUpdate();
            }
        }
    }
}
