using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class LineCollision : MonoBehaviour
{
    /// <summary>
    /// Drawer object
    /// </summary>
    public GameObject Drawer;

    /// <summary>
    /// Transparent Drawer object
    /// </summary>
    public GameObject DrawerGhost;

    /// <summary>
    /// Renderer of 3Dof line
    /// </summary>
    public LineRenderer LineRender;

    //Line points
    public GameObject PointToLine;
    public GameObject StartPoint;
    public GameObject EndPoint;

    [HideInInspector]
    public Vector3 PointPosition;

    /// <summary>
    /// Current object chirality
    /// </summary>
    public Chirality Chirality;

    /// <summary>
    /// Current object Node Type
    /// </summary>
    public NodeType Node;

    [HideInInspector]
    public bool IsCollided = false;

    [HideInInspector]
    public bool Enabled;

    private Vector3 startPos;

    [HideInInspector]
    public bool Pressed;

    private bool drawing;

    private DataSmoother smoother = new DataSmoother(new float[] { 1, 1, 1, 1, 1, 0.5f, 0.3f });

    private const float delta = .05f;

    void Start ()
    {
        startPos = Drawer.transform.localPosition;
    }

    void Update()
    {
        if (FinchCalibration.IsCalibrating)
            return;

        Pressed = FinchController.GetPress(Chirality, RingElement.Touch) || FinchController.GetPress(Chirality, RingElement.HomeButton);
        Enabled = IsCollided && Pressed;

        if (FinchNodeManager.GetUpperArmCount() == 0)
        {
            RaycastHit hit;
            LineRender.enabled = Pressed;
            DrawerGhost.SetActive(FinchNodeManager.IsConnected(Node));

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                Drawer.SetActive(FinchNodeManager.IsConnected(Node) && Pressed);
                IsCollided = true;
                PointPosition = hit.point;
                LineRender.SetPosition(0, StartPoint.transform.position);
                LineRender.SetPosition(1, PointToLine.transform.position);

                Vector3 position = new Vector3(startPos.x, startPos.y, startPos.z + hit.distance);

                if (drawing)
                {
                    smoother.UpdateData(position);
                }
                else
                {
                    smoother.ResetData(position);
                }

                Drawer.transform.position = hit.point;//smoother.SmoothData;
                drawing = true;
            }
            else
            {
                PointPosition = new Vector3();
                Drawer.SetActive(false);
                IsCollided = false;
                Drawer.transform.localPosition = startPos;
                LineRender.SetPosition(0, StartPoint.transform.position);
                LineRender.SetPosition(1, EndPoint.transform.position);
                drawing = false;
            }
        }
        else
        {
            PointPosition = Drawer.transform.position;
            Drawer.transform.localPosition = startPos;
            Drawer.SetActive(FinchNodeManager.IsConnected(Node) && Pressed);
            DrawerGhost.SetActive(!Pressed && FinchNodeManager.IsConnected(Node));
            IsCollided = true;
            LineRender.enabled = false;
        }
    }
}
