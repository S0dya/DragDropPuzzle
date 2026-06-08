using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PT.Logic.ProjectContext
{
    public abstract class InputManagerBase  : MonoBehaviour
    {
        private protected Inputs _input;

        private protected List<InputActionMap> _actionMapsList = new();
        
        protected readonly Dictionary<InputAction, InputActionsCallbacks> _callbacksMap = new();

        protected class InputActionsCallbacks
        {
            public Action<InputAction.CallbackContext> Performed;
            public Action<InputAction.CallbackContext> Canceled;

            public InputActionsCallbacks(Action<InputAction.CallbackContext> performed = null, Action<InputAction.CallbackContext> canceled = null)
            {
                Performed = performed;
                Canceled = canceled;
            }
        }

        protected void Start()
        {
            DisableMaps();
            
            OnStart();
        }

        protected virtual void OnEnable()
        {
            _input = new Inputs();

            OnSub();

            _input.Enable();
        }

        protected virtual void OnDisable()
        {
            OnUnSub();

            foreach (var kvp in _callbacksMap)
            {
                if (kvp.Value.Performed != null) kvp.Key.performed -= kvp.Value.Performed;
                if (kvp.Value.Canceled != null) kvp.Key.canceled -= kvp.Value.Canceled;
            }
            _callbacksMap.Clear();

            _input.Disable();
        }

        protected abstract void OnStart();
        protected abstract void OnSub();
        protected abstract void OnUnSub();
        
        protected void Subscribe(InputAction action, 
            Action<InputAction.CallbackContext> performed = null,
            Action<InputAction.CallbackContext> canceled = null)
        {
            var inputActionsCallbacks = new InputActionsCallbacks(performed, canceled);
            
            if (performed != null) action.performed += inputActionsCallbacks.Performed;
            if (canceled != null) action.canceled += inputActionsCallbacks.Canceled;
            
            _callbacksMap[action] = inputActionsCallbacks;
        }
        
        protected void EnableActionMap(InputActionMap mapToEnable)
        {
            DisableMaps();

            mapToEnable.Enable();
        }

        protected void DisableMaps()
        {
            foreach (InputActionMap map in _actionMapsList) map.Disable();
        }
    }
}