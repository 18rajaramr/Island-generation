using UnityEngine;

public class Cameramovement : MonoBehaviour
{
    private Vector3 CameraPosition;
    [Header("Camera Settings")]
    [Range(0,20)] public float CameraSpeed;
    
    
    void Start()
    {
        CameraPosition = this.transform.position;

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W)){
            CameraPosition.z += CameraSpeed / 10000;
        }

        if (Input.GetKey(KeyCode.S)){
            CameraPosition.z -= CameraSpeed / 10000;
        }

        if (Input.GetKey(KeyCode.A)){
            CameraPosition.x -= CameraSpeed / 10000;
        }

        if (Input.GetKey(KeyCode.D)){
            CameraPosition.x += CameraSpeed / 10000;
        }

        this.transform.position = CameraPosition;
    }
}
