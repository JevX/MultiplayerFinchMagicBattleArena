using Photon.Pun;
using UnityEngine;

namespace Main.Scripts.PhotonAR
{
    /// <summary>
    /// стандартная синхронизация от photona с добавлением точки относительности - основного игрового объекта
    /// </summary>
    public class PhotonTransformViewAR : MonoBehaviourPun, IPunObservable
    {   
        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = false;
        
        [SerializeField] private string nameObjectAnchor = "MAIN_OBJECT";
        
        private float m_Distance;
        private float m_Angle;
        private Vector3 m_Direction;
        private Vector3 m_NetworkPosition;
        private Vector3 m_StoredPosition;
        private Quaternion m_NetworkRotation;
        private bool m_firstTake = false;
        
        private Transform objectAnchor = null;
        protected Transform cacheTr;
        
        public void Awake()
        {
            cacheTr = transform;
            m_StoredPosition = cacheTr.position;
            m_NetworkPosition = Vector3.zero;
            m_NetworkRotation = Quaternion.identity;
            objectAnchor = GameObject.Find(nameObjectAnchor).transform;
            AwakeMethod();
        }

        protected virtual void AwakeMethod(){}

        void OnEnable()
        {
            m_firstTake = true;
        }

        public void Update()
        {
            var tr = transform;

            if (!this.photonView.IsMine)
            {
                tr.position = Vector3.MoveTowards(tr.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                tr.rotation = Quaternion.RotateTowards(tr.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
        }

        protected virtual Quaternion GetCurrentRotation()
        {
            return transform.rotation;
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Write
            if (stream.IsWriting)
            {
                if (this.m_SynchronizePosition)
                {
                    this.m_Direction = cacheTr.position - this.m_StoredPosition;
                    this.m_StoredPosition =  cacheTr.position;
                    stream.SendNext( cacheTr.position-objectAnchor.position);
                    stream.SendNext(this.m_Direction);
                }

                if (this.m_SynchronizeRotation)
                {
                    stream.SendNext(GetCurrentRotation());
                }

                if (this.m_SynchronizeScale)
                {
                    stream.SendNext( cacheTr.localScale);
                }
            }
            // Read
            else
            {
                if (this.m_SynchronizePosition)
                {
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext() + objectAnchor.position;
                    this.m_Direction = (Vector3)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        cacheTr.position = this.m_NetworkPosition;

                        this.m_Distance = 0f;
                    }
                    else
                    {
                        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                        this.m_NetworkPosition += this.m_Direction * lag;
                        this.m_Distance = Vector3.Distance( cacheTr.position, this.m_NetworkPosition);
                    }

                }

                if (this.m_SynchronizeRotation)
                {
                    this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        this.m_Angle = 0f;
                        
                        cacheTr.rotation = this.m_NetworkRotation;
                    }
                    else
                    {
                        this.m_Angle = Quaternion.Angle( cacheTr.rotation, this.m_NetworkRotation);
                    }
                }

                if (this.m_SynchronizeScale)
                {
                    cacheTr.localScale = (Vector3)stream.ReceiveNext();
                }

                if (m_firstTake)
                {
                    m_firstTake = false;
                }
            }
        }
    }
}