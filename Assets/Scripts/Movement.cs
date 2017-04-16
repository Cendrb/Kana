using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public float acceleration = 1f;
    public float deceleration = 0.5f;
    public float maxSteerSpeed = 2f;
    public float brakeSpeed = 4f;
    public float steerSpeedBonus = 10f;
    private float curSteerSpeed = 0f;
    private Vector3 angle = Vector3.zero;
    private Vector2 carNormal;
    private float speed = 0f;
    private Quaternion rotate = Quaternion.identity;

    void Start()
    {
        this.angle.z = this.transform.rotation.eulerAngles.z;
    }

    void Update()
    {
        this.carNormal = new Vector2(Mathf.Sin((-this.angle.z) * Mathf.Deg2Rad), Mathf.Cos((-this.angle.z) * Mathf.Deg2Rad));
        this.rotate.eulerAngles = this.angle;
        this.transform.rotation = this.rotate;
        //Debug.Log ("["+ (-angle.z)+"] "+"("+carNormal.x + ", " + carNormal.y + ")");

        //Forward and Backward Motion
        if (Input.GetAxis("Vertical") > 0)
        {
            if (this.curSteerSpeed < this.maxSteerSpeed)
                this.curSteerSpeed += 0.035f;

            if (this.speed < 0) //This will make changing direction better and easier
                this.speed += this.acceleration / 15;
            else
                this.speed += this.acceleration / 30;

            GetComponent<Rigidbody2D>().velocity = this.carNormal * this.speed; ;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            if (this.speed > 0)
            {
                this.speed -= this.brakeSpeed / 30;
                if (this.curSteerSpeed > 0)
                    this.curSteerSpeed -= 0.03f;
            }
            else
            {
                this.speed -= this.deceleration / 30;
                if (this.curSteerSpeed < this.maxSteerSpeed)
                    this.curSteerSpeed += 0.01f;
            }
            GetComponent<Rigidbody2D>().velocity = this.carNormal * this.speed;
        }
        else if (Input.GetAxis("Vertical") == 0)
        {
            if (this.curSteerSpeed > 0)
                this.curSteerSpeed -= 0.02f;
            if (this.speed > 0)
            {
                this.speed -= this.deceleration / 10;
                if (this.speed < 0.01)
                    this.speed = 0;
            }
            else if (this.speed < 0)
            {
                this.speed += this.deceleration / 10;
                if (this.speed > -0.01)
                    this.speed = 0;
            }
            GetComponent<Rigidbody2D>().velocity = this.carNormal * this.speed;
        }
        if (this.speed < 0.1 && this.speed > -0.1)
            this.curSteerSpeed = 0;
        //Steering Motion
        if (this.speed != 0)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (this.speed > 0)
                    this.angle += new Vector3(0, 0, -this.curSteerSpeed) * this.steerSpeedBonus / 100;
                else if (this.speed < 0)
                    this.angle += new Vector3(0, 0, this.curSteerSpeed) * this.steerSpeedBonus / 100;
                this.rotate.eulerAngles = this.angle;
                this.transform.rotation = this.rotate;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                if (this.speed > 0)
                    this.angle += new Vector3(0, 0, +this.curSteerSpeed) * this.steerSpeedBonus / 100;
                if (this.speed < 0)
                    this.angle += new Vector3(0, 0, -this.curSteerSpeed) * this.steerSpeedBonus / 100;
                this.rotate.eulerAngles = this.angle;
                this.transform.rotation = this.rotate;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag != null)
        {
            this.speed = -this.speed * 0.3f;
            GetComponent<Rigidbody2D>().angularVelocity = 0;
        }
    }

}