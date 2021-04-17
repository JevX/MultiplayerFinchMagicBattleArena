using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SynchronizationPlayers : MonoBehaviourPun, IPunObservable
{
    private GameObject MainObject = null;
    
    public bool isTeleportEnabled = true;
    public float teleportIfDistanceGreaterThan = 1.0f;
    
    private Transform tr;
    private PhotonView photonView;
    
    private Vector3 networkedPosition;
    private Quaternion networkedRotation;
    
    private float distance;
    private float angle;
    
    private void Awake()
    {
        tr = transform;
        photonView = GetComponent<PhotonView>();

        MainObject = GameObject.Find("MAIN_OBJECT");
        print(MainObject?.name ?? "проблема с поиском объекта MainObject");
        networkedPosition = new Vector3();
        networkedRotation = new Quaternion();
    }
    
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            tr.position = Vector3.MoveTowards(tr.position, networkedPosition, distance*(1.0f/ PhotonNetwork.SerializationRate));
            //tr.rotation = Quaternion.RotateTowards(tr.rotation, networkedRotation, angle*(1.0f/ PhotonNetwork.SerializationRate));
        }

    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position-MainObject.transform.position);
            stream.SendNext(tr.rotation);
        }
        else
        {
            networkedPosition = (Vector3)stream.ReceiveNext()+MainObject.transform.position;
            networkedRotation = (Quaternion)stream.ReceiveNext();

            if (isTeleportEnabled)
            {
                if (Vector3.Distance(tr.position, networkedPosition) > teleportIfDistanceGreaterThan)
                {
                    tr.position = networkedPosition;
                }
            }
        }
    }
}
