using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

    public GameObject player;       //Public variable to store a reference to the player game object
    public int orthographicSizeMin = 1;
    public int orthographicSizeMax = 8;

    private Vector3 offset;         //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = player.transform.position + offset + new Vector3(0,0);

        if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
            Camera.main.orthographicSize++;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // back
            Camera.main.orthographicSize--;

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax);

    }
}
