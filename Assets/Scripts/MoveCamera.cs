using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject move;


    float sensitivity = 180.0f;
    float speed = 12.0f;
    float maxVertical = 75.0f;

    // keep track of the starting rotation
    float verticalRot = 0.0f;
    float horizontalRot = 0.0f;
    void Start()
    {   
        // init
        Vector3 currentRotation = transform.localRotation.eulerAngles;
        verticalRot = currentRotation.y;
        horizontalRot = currentRotation.x;
    }

    // Update is called once per frame
    void Update()
    {
        // this is a very simple camera control
        // like, it doesn't lock the mouse to the middle of the screen.
        // cuz my main points aint about here so, hey, as long as I can look around.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        horizontalRot += mouseX * sensitivity * Time.deltaTime;
        verticalRot += mouseY * sensitivity * Time.deltaTime;
        verticalRot = Mathf.Clamp(verticalRot, -maxVertical, maxVertical);

        Quaternion localRotation = Quaternion.Euler(verticalRot, horizontalRot, 0.0f);
        transform.rotation = localRotation;


        // and walking around
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement = transform.rotation * movement;
        movement.y = 0;
        move.transform.position += move.transform.TransformDirection(movement) * speed * Time.deltaTime;
    }
}
