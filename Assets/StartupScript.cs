using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class StartupScript : MonoBehaviour
{

    void Start()
    {
    }


    void Awake()
    {
        Network.sendRate = 100;


        // get the networkmanager. probably more efficient way to do so
        GameObject nm = GameObject.Find("NetworkManager");
        if (nm == null)
        {
            print("error no network manager");
            return;
        }

        // let's try to start a host.....

        NetworkClient host = nm.GetComponent<NetworkManager>().StartHost();

        if (host == null)
        {
            print("host is null");
        }
        else
        {
            print("host started");
            return;
        }

        /*
        // let's try to start a server.

        if (nm.GetComponent<NetworkManager>().StartServer())
        {
            print("great we started a server");
            return;
        } 
        */


        // ok let's try to start a client!


        NetworkClient client = nm.GetComponent<NetworkManager>().StartClient();

        if (client == null)
        {
            print("client is null");
        }
        else
        {
            print("client started");
        }

    }

    void Update()
    {

    }
}
