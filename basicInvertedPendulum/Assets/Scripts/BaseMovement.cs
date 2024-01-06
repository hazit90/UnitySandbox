using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    public float speed = 5.0f;  // Speed of the base movement

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // 'A' and 'D' keys
        float moveVertical = Input.GetAxis("Vertical");     // 'W' and 'S' keys

        Vector3 movement = new Vector3(0f, 0.0f, moveVertical);

        transform.Translate(movement * speed * Time.deltaTime);
    }
}
