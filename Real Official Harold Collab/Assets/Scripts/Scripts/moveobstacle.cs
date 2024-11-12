using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveobstacle : MonoBehaviour
{
    public float radius = 1.0f;        // Radius of the circular path
    public float speed = 1.0f;        // Speed of rotation
    private float angle = 0.0f;       // Current angle

    private void Update()
    {
        // Calculate the new position based on the angle
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        // Set the object's position
        transform.position = new Vector3(x, y, transform.position.z);

        // Update the angle based on speed and time
        angle += speed * Time.deltaTime;

        // Ensure the angle stays between 0 and 2*pi for a continuous circle
        if (angle > Mathf.PI * 2)
        {
            angle -= Mathf.PI * 2;
        }
    }
}
