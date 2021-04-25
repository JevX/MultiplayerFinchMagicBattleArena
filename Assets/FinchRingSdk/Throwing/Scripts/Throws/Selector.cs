using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class Selector : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Pickupable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    public Chirality Chirality;

    public Transform PointOne;
    public Transform PointTwo;
    public LineRenderer Linear;
    RaycastHit hit;

    public Transform Selected { get; private set; }

    private void Update()
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        Ray ray = new Ray(PointOne.position, PointTwo.position);
        Debug.DrawLine(PointOne.position, PointTwo.position, Color.green);

        if (Physics.Raycast(ray, out hit))
        {
            Linear.SetPosition(1, hit.point);
            Linear.SetPosition(0, ray.origin);
            var selection = hit.transform;
            if (selection.CompareTag(selectableTag))
            {
                if (selection != Selected)
                {
                    Select(selection);
                }
            }
            else
            {
                Deselect();
            }
        }
        else
        {
            Linear.SetPosition(1, PointTwo.position);
            Linear.SetPosition(0, ray.origin);
            Deselect();
        }
    }

    void Select(Transform selection)
    {
        var selectionRenderer = selection.GetComponent<Renderer>();
        if (selectionRenderer != null)
        {
            selectionRenderer.material = highlightMaterial;
        }

        Selected = selection;
    }

    void Deselect()
    {
        if (Selected != null)
        {
            var selectionRenderer = Selected.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            Selected = null;
        }
    }
}
