using UnityEngine;

namespace ithappy.Animals_FREE
{
    /// <summary>
    /// Input controller for creatures - handles player input for movement
    /// This script was recreated to replace a missing script from the asset pack.
    /// </summary>
    [RequireComponent(typeof(CreatureMover))]
    [DisallowMultipleComponent]
    public class CreatureInputController : MonoBehaviour
    {
        [Header("Input Axis Names")]
        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";
        [SerializeField]
        private string m_VerticalAxis = "Vertical";
        [SerializeField]
        private string m_JumpButton = "Jump";
        [SerializeField]
        private KeyCode m_RunKey = KeyCode.LeftShift;

        [Header("Camera Settings")]
        [SerializeField]
        private Camera m_Camera;
        [SerializeField]
        private string m_MouseX = "Mouse X";
        [SerializeField]
        private string m_MouseY = "Mouse Y";
        [SerializeField]
        private string m_MouseScroll = "Mouse ScrollWheel";

        private CreatureMover m_Mover;
        private Transform m_CameraTransform;

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();
            
            // Use the assigned camera or find the main camera
            if (m_Camera == null)
            {
                m_Camera = Camera.main;
            }
            
            if (m_Camera != null)
            {
                m_CameraTransform = m_Camera.transform;
            }
        }

        private void Update()
        {
            if (m_Mover == null) return;

            // Get input axis
            float horizontal = Input.GetAxis(m_HorizontalAxis);
            float vertical = Input.GetAxis(m_VerticalAxis);
            Vector2 axis = new Vector2(horizontal, vertical);

            // Check for run input
            bool isRunning = Input.GetKey(m_RunKey);

            // Check for jump input
            bool isJumping = Input.GetButtonDown(m_JumpButton);

            // Calculate target position for looking (camera forward or creature forward)
            Vector3 target = transform.position + transform.forward;
            if (m_CameraTransform != null)
            {
                target = transform.position + m_CameraTransform.forward * 10f;
            }

            // Set input on the mover
            m_Mover.SetInput(in axis, in target, in isRunning, in isJumping);
        }
    }
}
