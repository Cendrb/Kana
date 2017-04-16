using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        private void Awake()
        {
            // Setting up references.
            this.m_GroundCheck = this.transform.Find("GroundCheck");
            this.m_CeilingCheck = this.transform.Find("CeilingCheck");
            this.m_Anim = GetComponent<Animator>();
            this.m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }


        private void FixedUpdate()
        {
            this.m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.m_GroundCheck.position, k_GroundedRadius, this.m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != this.gameObject)
                {
                    this.m_Grounded = true;
                }
            }
            this.m_Anim.SetBool("Ground", this.m_Grounded);

            // Set the vertical animation
            this.m_Anim.SetFloat("vSpeed", this.m_Rigidbody2D.velocity.y);
        }


        public void Move(float move, bool crouch, bool jump)
        {
            // If crouching, check to see if the character can stand up
            if (!crouch && this.m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(this.m_CeilingCheck.position, k_CeilingRadius, this.m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            // Set whether or not the character is crouching in the animator
            this.m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (this.m_Grounded || this.m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move* this.m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                this.m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                this.m_Rigidbody2D.velocity = new Vector2(move* this.m_MaxSpeed, this.m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !this.m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && this.m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (this.m_Grounded && jump && this.m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                this.m_Grounded = false;
                this.m_Anim.SetBool("Ground", false);
                this.m_Rigidbody2D.AddForce(new Vector2(0f, this.m_JumpForce));
            }
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            this.m_FacingRight = !this.m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = this.transform.localScale;
            theScale.x *= -1;
            this.transform.localScale = theScale;
        }
    }
}
