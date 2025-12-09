using System;
using UnityEngine;

namespace SojaExiles
{
    public class FirstEventTrigger2 : MonoBehaviour
    {
        public event Action OnEventTriggered;

        public void TriggerEvent()
        {
            OnEventTriggered?.Invoke();
        }
    }
}
