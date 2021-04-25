using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialParticlesDescription
{
    public Color _TintR;
    public Color _TintG;
    public Color _TintB;
    public Color _ColorAddR;
    public Color _ColorAddG;
    public Color _ColorAddB;

    public void Lerp(MaterialParticlesDescription target, float alpha)
    {
        _TintR = Color.Lerp(_TintR, target._TintR, alpha);
        _TintG = Color.Lerp(_TintG, target._TintG, alpha);
        _TintB = Color.Lerp(_TintB, target._TintB, alpha);
        _ColorAddR = Color.Lerp(_ColorAddR, target._ColorAddR, alpha);
        _ColorAddG = Color.Lerp(_ColorAddG, target._ColorAddG, alpha);
        _ColorAddB = Color.Lerp(_ColorAddB, target._ColorAddB, alpha);
    }
}

public class ThrowingObject : MonoBehaviour
{
    public MaterialParticlesDescription MatDescNormal = new MaterialParticlesDescription();
    public MaterialParticlesDescription MatDescSelected = new MaterialParticlesDescription();
    public MaterialParticlesDescription MatDescLowVelocity = new MaterialParticlesDescription();
    MaterialParticlesDescription desc_cur = new MaterialParticlesDescription();
    public Material mat;
    private Material instaited_material;


    public GameObject trail;
    public GameObject bounce;
    GameObject trail_figure;
    GameObject bounce_figure;

    public Material particlesOpt;
    public MeshRenderer[] particles;
    public Mesh finalCollider;
    public GameObject OptimaizedPiramydObject;
    [HideInInspector]
    public bool readyToOptimize = false;
    private MeshCollider meshcollider;
    private float cur = 0;
    private float vel = 0;
    private float interpolation_speed = 1;
    private bool materialOptimised = false;

    GameObject centerFigureAnimation;
    GameObject controller;
    Rigidbody rigid;
    ObjectPickup ObjPick;
    private bool hasGrab = false;
    public float floorY = 0.5f;
    private bool firstCollisionBeen = false;
    private float timerToOptimized = 0;
    public bool useFloor;
    private ManagerPiramydRigidbodies managerPyramid;
    Vector3 OldColPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

    const float epsilent_metres = 0.05f; // metres
    const float min_collision_delay = 0.35f; // seconds
    float timerLastFloorCollision = float.MinValue;
    private float timer_InternalCollision = 0;

    void Start()
    {
        managerPyramid = GameObject.Find("ManagerPiramydRigidbodies").GetComponent<ManagerPiramydRigidbodies>();
        Finch.FinchCalibration.OnCalibrationEnd += OnCalibrationEnd;
        instaited_material = new Material(mat);

        foreach (var i in particles)
        {
            i.sharedMaterial = instaited_material;
        }
        meshcollider = gameObject.GetComponent<MeshCollider>();
        centerFigureAnimation = GameObject.Find("Object");

        controller = GameObject.Find("ControllerRight");
        if (controller == null)
            controller = GameObject.Find("ControllerLeft");

        // print(controller);
        ObjPick = controller.GetComponent<ObjectPickup>();
        // print(ObjPick);
        rigid = gameObject.GetComponent<Rigidbody>();

        managerPyramid.throwingObjects.Add(this);
        // controller = gameObject.GetComponent<ObjectPickup>();
    }

    public void DestroyObject()
    {
        managerPyramid.throwingObjects.Remove(this);
        Destroy(gameObject);
    }

    void OnCalibrationEnd()
    {
        if (Finch.FinchNodeManager.IsConnected(Finch.NodeType.RightHand))
        {
            controller = GameObject.Find("ControllerRight");
            ObjPick = controller.GetComponent<ObjectPickup>();
           // print("right");
        }
        else if (Finch.FinchNodeManager.IsConnected(Finch.NodeType.LeftHand))
        {
            controller = GameObject.Find("ControllerLeft");
            ObjPick = controller.GetComponent<ObjectPickup>();
           // print("left");
        }
    }
    // Update is called once per frame
    void Update()
    {
        {   //work in shader
            cur = MathUtility.Lerp(cur, ObjPick.obj_selected ? 1 : 0, Time.deltaTime, interpolation_speed);
            vel = Mathf.Clamp01(rigid.velocity.magnitude);
            float alpha_selected_current01 = cur;
            float alpha_low_velocity01 = vel;

            desc_cur.Lerp(MatDescNormal, 1);
            desc_cur.Lerp(MatDescSelected, alpha_selected_current01);
            desc_cur.Lerp(MatDescLowVelocity, alpha_selected_current01);

            instaited_material.SetColor("_TintR", desc_cur._TintR);
            instaited_material.SetColor("_TintG", desc_cur._TintG);
            instaited_material.SetColor("_TintB", desc_cur._TintB);
            instaited_material.SetColor("_ColorAddR", desc_cur._ColorAddR);
            instaited_material.SetColor("_ColorAddG", desc_cur._ColorAddG);
            instaited_material.SetColor("_ColorAddB", desc_cur._ColorAddB);
        }

        //optimize for perfomance
        if (readyToOptimize && !materialOptimised)
        {
            timerToOptimized += Time.deltaTime;
            if (timerToOptimized > 5)
                Optimaized();
        }

        UpdateBounceRespawnExit();

    }

    public void CreateAndConnectTrail()
    {
        trail_figure = Instantiate(trail, gameObject.transform.position, Quaternion.identity);
        trail_figure.transform.parent = gameObject.transform;
        Destroy(trail_figure, 5);
    }

    public void SwitchColliderToNormal()
    {
        meshcollider.sharedMesh = finalCollider;
    }

    public void Optimaized()
    {
        foreach (var i in particles)
        {
            i.GetComponent<MeshRenderer>().enabled = false;
        }
        OptimaizedPiramydObject.SetActive(true);
        materialOptimised = true;
    }

    //float Lerp(float Current, float Target, float DeltaTime, float InterpSpeed)
    //{
    //    if (InterpSpeed <= 0.0f) return Target;
    //    float Alpha = Mathf.Clamp(DeltaTime * InterpSpeed, 0.0f, 1.0f);
    //    return Target * Alpha + Current * (1.0f - Alpha);
    //}

    void UpdateBounceRespawnExit()
    {
        if (timerLastFloorCollision == float.MinValue)
            return;

        timerLastFloorCollision += Time.deltaTime;
        if (timerLastFloorCollision > min_collision_delay
            && (OldColPos - transform.position).magnitude > epsilent_metres)
        {
            timerLastFloorCollision = float.MinValue;

            OldColPos = new Vector3(float.MinValue, 0, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (trail_figure != null)
            trail_figure.transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission = false;

        //активация текущего заспавленного обьекта когда мы кинули в него взятым обьектом
        if (gameObject == centerFigureAnimation.GetComponent<CenterFigureAnimator>().currentSpawnedObject)
        {
            managerPyramid.MakeCurentObjectPhysical();
        }

        if (collision.gameObject.tag == "Floor" && (OldColPos - transform.position).magnitude > epsilent_metres)
        {
            timerLastFloorCollision = 0;

            if (!firstCollisionBeen)
                managerPyramid.CollideWithFloor();
            timer_InternalCollision = Time.time;
            SpawnBounce(collision);
            firstCollisionBeen = true;

            OldColPos = transform.position;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        timer_InternalCollision = 0;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" && (OldColPos - transform.position).magnitude > epsilent_metres)
        {
            if (timer_InternalCollision > 0)
            {
                float deltaTime = Time.time - timer_InternalCollision;
                if (deltaTime > 0.35)
                {
                    SpawnBounce(collision);
                    OldColPos = transform.position;
                    timer_InternalCollision = Time.time;
                }
            }

        }

    }
    void SpawnBounce(Collision col)
    {

        Vector3 HitNormal = Vector3.zero;
        bounce_figure = Instantiate(bounce, col.contacts[0].point, Quaternion.identity);
        RaycastHit hit;
        int layerMask = ~(1 << 10);
        
        Ray ray = new Ray(gameObject.transform.position, (col.contacts[0].point - gameObject.transform.position).normalized);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            HitNormal = hit.normal;
        }

        /////////////////////////////////////////////////////////////////////////in MATHF
        Vector3 tangent;
        Vector3 c1 = Vector3.Cross(HitNormal, Vector3.right);
        Vector3 c2 = Vector3.Cross(HitNormal, Vector3.up);
        if (c1.magnitude > c2.magnitude)
        {
            tangent = c1;
        }
        else
        {
            tangent = c2;
        }
        Vector3 bitangent = Vector3.Cross(tangent.normalized, HitNormal);
        /////////////////////////////////////////////////////////////////////////////
        ///

        //if (Physics.Raycast(gameObject.transform.position, (col.contacts[0].point - gameObject.transform.position).normalized, out hit, Mathf.Infinity, layerMask))
        //{
        //    Debug.DrawRay(gameObject.transform.position, (col.contacts[0].point - gameObject.transform.position).normalized, Color.yellow, 9999f);
        //    Debug.Log("Did Hit");
        //}

        bounce_figure.transform.rotation = Quaternion.LookRotation(bitangent, HitNormal);

        AudioMaster.Play(AudioMaster.Instance.bounce);
        if (useFloor)
            bounce_figure.transform.position = new Vector3(transform.position.x, floorY, transform.position.z);
        else
            bounce_figure.transform.position = new Vector3(transform.position.x, col.contacts[0].point.y, transform.position.z);



        Destroy(bounce_figure, 5);
    }
}
