using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{

    public float speed;
    public float acceleration;
    public float turning;
    public float breaks;
    private Vector2 moveDir = Vector2.zero;
    private float actualSpeed = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveDir = transform.TransformDirection(moveDir);
        moveDir *= speed;
        controller.Move(moveDir * Time.deltaTime);
    }
}
