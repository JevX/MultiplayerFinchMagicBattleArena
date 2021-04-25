using UnityEngine;

public class MoveWhenTouchWand : MonoBehaviour
{
    public string nameObject;
    public bool toRight = false;
    public bool toLeft = false;

    private void FixedUpdate()
    {
        if (toLeft)
        {
            //Debug.Log("Move to the left");
            Transform bookPos = MagicBookStartAnimation.Instance.dotPositionForBook.transform;
            Vector3 relativePos = (Camera.main.transform.position - new Vector3(0f, 0f, 0f)) - bookPos.position;

            Quaternion rotation = Quaternion.LookRotation(relativePos);

            Quaternion current = bookPos.localRotation;

            bookPos.localRotation = Quaternion.Slerp(current, rotation, Time.fixedDeltaTime);
            bookPos.Translate(0.3f * Time.fixedDeltaTime, 0, 0);
        }

        if (toRight)
        {
            //Debug.Log("Move to the right");
            Transform bookPos = MagicBookStartAnimation.Instance.dotPositionForBook.transform;
            Vector3 relativePos = (Camera.main.transform.position + new Vector3(0f, 0f, 0f)) - bookPos.position;

            Quaternion rotation = Quaternion.LookRotation(relativePos);

            Quaternion current = bookPos.localRotation;

            bookPos.localRotation = Quaternion.Slerp(current, rotation, Time.fixedDeltaTime);
            bookPos.Translate(-0.3f * Time.fixedDeltaTime, 0, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Enter: collider = "+other.name);
        if (this.nameObject == "back" && other.transform.gameObject.name == "Wand")
        {
            //Debug.Log("Wand Trigger Enter - Back");
            toLeft = true;
            toRight = false;
        }

        if (this.nameObject == "front" && other.transform.gameObject.name == "Wand")
        {
            //Debug.Log("Wand Trigger Enter - Front");
            toLeft = false;
            toRight = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (nameObject == "back" && other.name == "Wand")
        {
            //Debug.Log("Wand Trigger Exit - Back");
            toLeft = false;
            toRight = false;
        }

        if (nameObject == "front" && other.name == "Wand")
        {
            //Debug.Log("Wand Trigger Exit - Front");
            toLeft = false;
            toRight = false;
        }
    }



}
