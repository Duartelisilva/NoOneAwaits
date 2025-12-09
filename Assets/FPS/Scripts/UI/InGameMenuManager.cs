using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class InGameMenuManager : MonoBehaviour
    {
        [Tooltip("Root GameObject of the menu used to toggle its activation")]
        public GameObject MenuRoot;

        [Tooltip("Master volume when menu is open")] [Range(0.001f, 1f)]
        public float VolumeWhenMenuOpen = 0.5f;

        [Tooltip("Slider component for look sensitivity")]
        public Slider LookSensitivitySlider;
        

        PlayerInputHandler m_PlayerInputsHandler;
        
        private InputAction m_SubmitAction;
        private InputAction m_CancelAction;
        private InputAction m_NavigateAction;
        private InputAction m_MenuAction;
        public GameObject options;
        void Start()
        {
            m_PlayerInputsHandler = FindFirstObjectByType<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerInputHandler, InGameMenuManager>(m_PlayerInputsHandler,
                this);

            MenuRoot.SetActive(false);

            LookSensitivitySlider.value = m_PlayerInputsHandler.LookSensitivity;
            LookSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);

            m_SubmitAction = InputSystem.actions.FindAction("UI/Submit");
            m_CancelAction = InputSystem.actions.FindAction("UI/Cancel");
            m_NavigateAction = InputSystem.actions.FindAction("UI/Navigate");
            m_MenuAction = InputSystem.actions.FindAction("UI/Menu");
            
            m_SubmitAction.Enable();
            m_CancelAction.Enable();
            m_NavigateAction.Enable();
            m_MenuAction.Enable();
        }

        void Update()
        {
            // Lock cursor when clicking outside of menu
            if (!MenuRoot.activeSelf && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (m_MenuAction.WasPressedThisFrame()
                || (MenuRoot.activeSelf && m_CancelAction.WasPressedThisFrame()))
            {
                if(options.activeSelf) 
                {
                SetPauseMenuActivation(!MenuRoot.activeSelf);
                }

            }

            if (m_NavigateAction.ReadValue<Vector2>().y != 0)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    LookSensitivitySlider.Select();
                }
            }

            if(!options.activeSelf && MenuRoot.activeSelf)
                SetPauseMenuActivation(!MenuRoot.activeSelf);
        }

        public void ClosePauseMenu()
        {
            SetPauseMenuActivation(false);
        }

        void SetPauseMenuActivation(bool active)
        {
            MenuRoot.SetActive(active);

            if (MenuRoot.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                AudioUtility.SetMasterVolume(VolumeWhenMenuOpen);

                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                AudioUtility.SetMasterVolume(1);
            }

        }

        void OnMouseSensitivityChanged(float newValue)
        {
            m_PlayerInputsHandler.LookSensitivity = newValue;
        }
    }
}