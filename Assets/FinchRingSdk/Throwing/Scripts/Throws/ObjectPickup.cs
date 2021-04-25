using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class ObjectPickup : MonoBehaviour
{

    public float ForceCoef;
    public float HoldLever = 0.3f;
    public float HoldMagnetSpeed = 0.8f;

    public string selectableTag = "Pickupable";

    public Transform PointOne;
    public Transform PointTwo;
    public Transform Selected { get; private set; }

    public bool obj_selected { get; private set; }
    public bool throwing;

    [HideInInspector] public GameObject obj;

    private GameObject trackedObj;
    private FixedJoint fJoint;
    private Rigidbody trackedRigidbody;
    private FinchControllerVisual controller;
    private DataSmoother smoother = new DataSmoother(new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
    private bool threeDofMode;
    private RaycastHit hitCast;
    private Vector3 previousPosition;
    private Vector3 velosity;
    private LineRenderer lineRender;
    private GameObject podium;
    private ManagerPiramydRigidbodies managerPyramid;
    private CenterFigureAnimator centerFigureAnimator;
    private GameObject hologram;

    void Start()
    {
        Finch.FinchCalibration.OnCalibrationStart += OnCalibrationStart;
        managerPyramid = GameObject.Find("ManagerPiramydRigidbodies").GetComponent<ManagerPiramydRigidbodies>();
        podium = GameObject.Find("podiumMeshRenderer");
        hologram = GameObject.Find("HologramRays");
        centerFigureAnimator = GameObject.Find("Object")?.GetComponent<CenterFigureAnimator>();
        trackedObj = gameObject;
        fJoint = GetComponent<FixedJoint>();
        controller = GetComponent<FinchControllerVisual>();
        lineRender = GetComponent<LineRenderer>();
    }

    void OnCalibrationStart()
    {
        DropObj();
    }

    void Update()
    {
        if (centerFigureAnimator == null)
            centerFigureAnimator = GameObject.Find("Object")?.GetComponent<CenterFigureAnimator>();

        if (podium == null)
            podium = GameObject.Find("podiumMeshRenderer");

        UpdateLine();
        threeDofMode = (FinchNodeManager.GetUpperArmCount() == 0);

        ///////////////////////////////////////////////////////////////// Line Render only.
        {
            if (threeDofMode && !FinchCalibration.IsCalibrating)
            {
                if ((FinchNodeManager.IsConnected(NodeType.RightHand) && gameObject.name == "ControllerRight") && FinchController.GetPress(controller.Chirality, RingElement.Touch))
                    lineRender.enabled = true;
                else
                    lineRender.enabled = false;

                if ((FinchNodeManager.IsConnected(NodeType.LeftHand) && gameObject.name == "ControllerLeft") && FinchController.GetPress(controller.Chirality, RingElement.Touch))
                    lineRender.enabled = true;
            }
            //all line disable in calibration
            if (FinchCalibration.IsCalibrating)
            {
                lineRender.enabled = false;
            }
        }
        /////////////////////////////////////////////////////////////////

        if (threeDofMode && Selected != null && FinchController.GetPress(controller.Chirality, RingElement.HomeButton))
        {
            obj = Selected.gameObject;
        }
        else if (threeDofMode)
        {
            obj = null;
        }

        velosity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
        smoother.UpdateData(velosity);

        if (controller == null)
        {
            return;
        }

        if (FinchController.GetPressDown(controller.Chirality, RingElement.HomeButton))
        {
            PickUpObj();
        }

        if (FinchController.GetPressUp(controller.Chirality, RingElement.HomeButton))
        {
            DropObj();
        }
    }

    void FixedUpdate()
    {
        NodeType nodeType;
        Finch.Internal.FinchCore.Finch_Node bone;
        if (controller.Chirality == Chirality.Right)
        {
            nodeType = NodeType.RightHand;
            bone = Finch.Internal.FinchCore.Finch_Node.RightHand;
        }
        else
        {
            nodeType = NodeType.LeftHand;
            bone = Finch.Internal.FinchCore.Finch_Node.LeftHand;
        }

        if (throwing)
        {
            Transform origin;
            if (trackedObj.transform != null)
            {
                origin = trackedObj.transform;
            }
            else
            {
                origin = trackedObj.transform.parent;
            }

            Vector3 impulse = getImpulse(
                Finch.Internal.FinchCore.Finch_GetNodeLinearVelocity(bone, Finch.Internal.FinchCore.Finch_ImuDataType.GlobalCs | Finch.Internal.FinchCore.Finch_ImuDataType.Calculated).ToUnity(),
                threeDofMode);

            if (float.IsNaN(impulse.x) || float.IsNaN(impulse.y) || float.IsNaN(impulse.z))
            {
                impulse = Vector3.zero;
            }
            trackedRigidbody.AddForce(impulse, ForceMode.VelocityChange);
            trackedRigidbody.angularVelocity = ((origin != null) ?
                origin.TransformVector(FinchInput.GetAngularVelocity(nodeType) * .5f) :
                FinchInput.GetAngularVelocity(nodeType) * .5f);

            trackedRigidbody.maxAngularVelocity = trackedRigidbody.angularVelocity.magnitude;

            throwing = false;
            trackedRigidbody = null;
        }
    }

    ////////////////////////////////////////////// For 6 dof
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(selectableTag))
        {
            bool objWasNull = (obj == null);
            obj = other.gameObject;

            if (objWasNull && FinchController.GetPressDown(controller.Chirality, RingElement.HomeButton))
            {
                PickUpObj();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(selectableTag))
        {
            obj = other.gameObject;
            obj = null;
        }
    }
    void Move3Dof(Vector3 rayOrigin, Vector3 rayDirection, float deltaTime)
    {
        Vector3 targetPosition = rayOrigin + rayDirection * HoldLever;
        fJoint.connectedBody = null;
        obj.transform.position = targetPosition;
        fJoint.connectedBody = obj.GetComponent<Rigidbody>();
    }

    Vector3 getImpulse(Vector3 velocity, bool isThreeDof)
    {
        float velCoef = 1.0f;
        Vector3 impulse = velocity * velCoef * ForceCoef;
        if (threeDofMode)
        {
            float verticalComponent = impulse.y;
            if (verticalComponent < 0)
            {
                verticalComponent *= 0.33f;
            }
            impulse = new Vector3(impulse.x, verticalComponent, impulse.z);
        }

        return impulse;
    }

    void PickUpObj()
    {
        if (obj != null)
        {
            obj.transform.parent = null;



            if (threeDofMode)
                Move3Dof(PointOne.position, (PointTwo.position - PointOne.position).normalized, Time.deltaTime);
            else
                obj.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.03f, gameObject.transform.position.z);

            managerPyramid.PickUp(obj);

            obj_selected = true;
            fJoint.connectedBody = obj.GetComponent<Rigidbody>();
            throwing = false;
            trackedRigidbody = null;

            if (hologram != null)
                centerFigureAnimator.HideHologram();
            else
            {
                hologram = GameObject.Find("HologramRays");
                centerFigureAnimator.HideHologram();
            }
        }
        else
        {
            fJoint.connectedBody = null;
        }
    }

    void DropObj()
    {
        if (fJoint.connectedBody != null)
        {

            managerPyramid.ThrowObject();
            obj = null;
            obj_selected = false;
            trackedRigidbody = fJoint.connectedBody;
            fJoint.connectedBody = null;
            throwing = true;
        }
    }

    public bool RaySphereIntersection(Vector3 ray_pos, Vector3 ray_dir, Vector3 spos, float r, out float tResult)
    {
        tResult = 0;
        Vector3 k = ray_pos - spos;
        float b = Vector3.Dot(k, ray_dir);
        float c = Vector3.Dot(k, k) - r * r;
        float d = b * b - c;

        if (d >= 0)
        {
            float sqrtfd = Mathf.Sqrt(d);
            float t1 = -b + sqrtfd;
            float t2 = -b - sqrtfd;
            float min_t = Mathf.Min(t1, t2);
            float max_t = Mathf.Max(t1, t2);

            float t = (min_t >= 0) ? min_t : max_t;
            tResult = t;
            return (t > 0);
        }
        return false;
    }

    void UpdateLine()
    {
        Ray ray = new Ray(PointOne.position, PointTwo.position - PointOne.position);

        //--------------------------------------------------------------
        int layerMask = 1 << 10;
        Transform selection = null;
        if (Physics.Raycast(ray, out hitCast, Mathf.Infinity, layerMask))
        {
            Vector3 hitCastPointLocal = (PointOne.parent == null) ?
                                        hitCast.point :
                                        PointOne.parent.InverseTransformPoint(hitCast.point);

            lineRender.SetPosition(1, hitCastPointLocal);
            lineRender.SetPosition(0, PointOne.localPosition);

            selection = hitCast.transform;
            if (!selection.CompareTag(selectableTag))
            {
                selection = null;
            }
        }
        else
        {
            lineRender.SetPosition(1, PointTwo.localPosition);
            lineRender.SetPosition(0, PointOne.localPosition);

            if (managerPyramid.IsPickUped()) {
                Vector3 towards = (PointTwo.position - PointOne.position);
                Vector3 rayView = towards.normalized;
                Vector3 oldPosition = PointTwo.position;
                PointTwo.position = PointOne.position + Mathf.Max(0, (PointOne.position - managerPyramid.PickapedObject.transform.position).magnitude - 0.07f) * rayView;


                lineRender.SetPosition(0, PointOne.localPosition);
                lineRender.SetPosition(1, PointTwo.localPosition);
                PointTwo.position = oldPosition;
            }
        }

        //--------------------------------------------------------------
        bool bReadyToSelect = selection != null;


        Transform pyramid_origin = centerFigureAnimator == null
                                    ? null
                                    : (centerFigureAnimator.currentSpawnedObject == null
                                        ? null
                                        : centerFigureAnimator.currentSpawnedObject.transform
                                    );
        //--------------------------------------------------------------
        if (pyramid_origin != null && !bReadyToSelect)
        {
            float SelectThreshold = 0.55f;
            Vector3 rayView = (PointTwo.position - PointOne.position).normalized;

            Vector3 rayTowardsObject = (pyramid_origin.transform.position - PointOne.position).normalized;
            bReadyToSelect = Vector3.Dot(rayView, rayTowardsObject) > SelectThreshold;

        }
        //--------------------------------------------------------------
        if (Finch.FinchCalibration.IsCalibrating)
        {
            bReadyToSelect = false;
        }
        if (bReadyToSelect)
        {
            if (selection == null)
            {
                selection = pyramid_origin;
            }
            if (selection != Selected && selection != null)
            {
                Select(selection);
            }
        }
        else
        {
            Deselect();
        }
    }

    void Select(Transform selection)
    {
        Selected = selection;
    }

    void Deselect()
    {
        if (Selected != null)
        {
            Selected = null;
        }
    }
}