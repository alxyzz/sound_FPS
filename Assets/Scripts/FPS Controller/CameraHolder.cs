using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraHolder : MonoBehaviour
{   
    public Transform cameraPosition; //Gotta find this under LocalPlayerObject > Camera Position
    public FPSCameraController cameraController;



    void Start()
    {
        //transform.parent.parent
       // cameraController.fpsUI.transform.position = transform.position;
       // cameraController.fpsUI.transform.SetParent(transform);
    }

    void Update()
    {
        if (cameraController.enabled == false) {
            cameraController.enabled = true;

            // GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            // foreach (GameObject player in players) {
            //     if (player.name == "LocalGamePlayer") {
            //         cameraPosition = player.transform.Find("Camera Position");
            //         cameraController.orientation = player.transform.Find("Orientation");
            //         break;
            //     }
            // }
        }


        transform.position = cameraPosition.position;
        
    }
}
