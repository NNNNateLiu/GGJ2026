using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveTest : MonoBehaviour
{
    private CharacterController controller;
    public float Speed = 10f;
    public float Gravity = 9.81f;
    private Vector3 Velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        controller = transform.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;
        var move = direction * (Speed * Time.deltaTime);
        controller.Move(move);
        
        Velocity.y -= Gravity * Time.deltaTime;
        controller.Move(Velocity * Time.deltaTime);
    }
}
