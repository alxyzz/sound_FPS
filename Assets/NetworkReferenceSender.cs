using UnityEngine;
using Mirror;

public class NetworkReferenceSender : NetworkBehaviour
{
    //// Reference to the GameObject to send
    //[SerializeField] private GameObject referenceToSend;

    //// NetworkConnection object representing the connection to the server
    //private NetworkConnection serverConnection;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    // Get the connection to the server
    //    serverConnection = NetworkClient.connection;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    // Check if the reference has been set and the client is connected to the server
    //    if (referenceToSend != null && serverConnection != null && serverConnection.isAuthenticated)
    //    {
    //        // Send the reference to the server
    //        serverConnection.Send(new ReferenceToServerMessage { reference = referenceToSend });

    //        // Reset the reference to prevent sending it multiple times
    //        referenceToSend = null;
    //    }
    //}
}