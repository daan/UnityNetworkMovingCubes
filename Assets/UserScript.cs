using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


struct UserState
{
    public Vector3 pos;
}


public class UserScript : NetworkBehaviour
{

    // for making new cubes
    public GameObject myCubePrefab;

    Plane mGroundPlane;
    // this is the cube that we are dragging
    GameObject mDragCube = null;
    Vector3 mGroundDragStart;
    Vector3 mCubeDragStart;


    [SyncVar]
    UserState serverState;

    void Awake()
    {
        InitState();
    }

    void Start()
    {
        SyncState();
    }

    [Server]
    void InitState()
    {
        serverState = new UserState
        {
            pos = new Vector3()
        };
    }

    [Command]
    void CmdMove(Vector3 delta)
    {

        serverState = Move(serverState, delta);
    }



    // ALL CUBE COMMANDS

    [Command]
    void CmdNewCube()
    {
        print("make cube");
        GameObject c = (GameObject)Instantiate(myCubePrefab);
        NetworkServer.Spawn(c);
        //NetworkServer.SpawnWithClientAuthority(c, connectionToClient);
    }

    [Command]
    void CmdChangeCubePos(GameObject obj, Vector3 pos)
    {
        obj.GetComponent<CubeScript>().SetMyPos(pos);
    }

    [Command]
    void CmdChangeCubeColor(GameObject obj, int col)
    {
        obj.GetComponent<CubeScript>().SetMyColor(col);
    }



    /*
    print("grab cube ownership");
    if (obj.GetComponent<NetworkIdentity>().hasAuthority)
    {
        print("object has authority");
        obj.GetComponent<NetworkIdentity>().RemoveClientAuthority(connectionToClient);
    }
    obj.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    */



    UserState Move(UserState prev, Vector3 delta)
    {
        return new UserState
        {
            pos = prev.pos + delta
        };
    }


    void SyncState()
    {
        transform.position = serverState.pos;
    }

    bool RayGroundIntersection(Ray aRay, out Vector3 pos)
    {
        // move this to better location....
        mGroundPlane = new Plane(new Vector3(0, 1, 0), 0.0f);
        float t;
        pos = new Vector3();
        if (!mGroundPlane.Raycast(aRay, out t)) return false;
        pos = aRay.GetPoint(t);
        return true;
    }

    void HandleInput()
    {
        if (Input.GetKeyDown("space"))
        {
            CmdNewCube();
        }

        // motion
        Vector3 delta = new Vector3();

        if (Input.GetKeyDown("w"))
        {
            delta.x = 1;
            CmdMove(delta);
        }
        else if (Input.GetKeyDown("s"))
        {
            delta.x = -1;
            CmdMove(delta);
        }
        else if (Input.GetKeyDown("a"))
        {
            delta.z = 1;
            CmdMove(delta);
        }
        else if (Input.GetKeyDown("d"))
        {
            delta.z = -1;
            CmdMove(delta);
        }

        if (Input.GetMouseButtonDown(0))
        {
            // on mouse down check whether we hit a cube.

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // this is kind of silly, but it seems to work
                if (hit.transform.name == "CubePrefab(Clone)")
                {
                    GameObject cube = hit.collider.gameObject;
                    CmdChangeCubeColor(cube, 3);
                    mDragCube = cube;

                    if (RayGroundIntersection(ray, out mGroundDragStart))
                    {
                    }
                    mCubeDragStart = cube.transform.position;
                }

                // print("you selected " + hit.transform.name);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            print("stop dragging");
            mDragCube = null;
        }

        if (Input.GetMouseButton(0))
        {
            if (mDragCube == null) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 groundDrag;
            if (RayGroundIntersection(ray, out groundDrag))
            {
                CmdChangeCubePos(mDragCube, mCubeDragStart + groundDrag - mGroundDragStart);
            }
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            HandleInput();

        }
        SyncState();
    }

}
