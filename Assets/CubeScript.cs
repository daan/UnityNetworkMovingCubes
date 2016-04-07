using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

struct CubeState
{
    public Vector3 pos;
    public int col;
}


public class CubeScript : NetworkBehaviour
{

    [SyncVar]
    CubeState serverState;

    Color[] mColors = { Color.yellow, Color.red, Color.blue, Color.green };



    void Awake()
    {
    }

    void Start()
    {
        SyncState();
    }

    [Server]
    void InitState()
    {
        serverState = new CubeState
        {
            pos = new Vector3(),
            col = 0
        };
    }

    public void SetMyColor(int col)
    {
        int c = (serverState.col + 1) % 4;


        serverState = SetColor(serverState, c);
    }
    CubeState SetColor(CubeState prev, int newCol)
    {
        return new CubeState
        {
            pos = prev.pos,
            col = newCol
        };
    }

    public void SetMyPos(Vector3 pos)
    {
        serverState = SetPos(serverState, pos);
    }

    CubeState SetPos(CubeState prev, Vector3 newPos)
    {
        return new CubeState
        {
            pos = newPos,
            col = prev.col
        };
    }


    void SyncState()
    {
        transform.position = serverState.pos;
        gameObject.GetComponent<Renderer>().material.color = mColors[serverState.col];
    }


    /*
    [Command]
    void CmdServerAssignClient()
    {
        print("server assign to client");
        this.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }

    [Command]
    void CmdServerRemoveClient()
    {
        this.GetComponent<NetworkIdentity>().RemoveClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
    }
    */


    void Update()
    {
        SyncState();
    }



}
