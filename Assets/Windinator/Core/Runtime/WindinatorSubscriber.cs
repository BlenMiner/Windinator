using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator
{
    public class WindinatorSubscriber : MonoBehaviour
    {
        static WindinatorSubscriber singleton;

        public static WindinatorSubscriber Instance
        {
            get
            {
                if (singleton == null)
                {
                    GameObject go = new GameObject("[Bootstrap] Windinator");
                    singleton = go.AddComponent<WindinatorSubscriber>();
                }

                return singleton;
            }
        }

        public Action onUpdate;

        private void Update()
        {
            onUpdate?.Invoke();
        }
    }
}