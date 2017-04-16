using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;


        private void Awake()
        {
            this.m_Character = GetComponent<PlatformerCharacter2D>();
        }


        private void Update()
        {
            if (!this.m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                this.m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            this.m_Character.Move(h, crouch, this.m_Jump);
            this.m_Jump = false;
        }
    }
}
