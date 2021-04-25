using UnityEngine;
using MoreMountains.Tools;
using System.Collections.Generic;

public class SpawnPropsOnTarget : MonoBehaviour, MMEventListener<MMGameEvent>
{
    [SerializeField] private MMMultipleObjectPooler propsPooler;
    public List<GameObject> listCoord;

    public float checkDisabledObjectsFrequency = 5;
    private float timer;

    List<ObjectLinkedToStartingPosition> propsList = new List<ObjectLinkedToStartingPosition>();

    bool gameStarted = false;

    struct ObjectLinkedToStartingPosition
    {
        public GameObject props;
        public Vector3 position;

        public ObjectLinkedToStartingPosition(GameObject props, Vector3 position) : this()
        {
            this.props = props;
            this.position = position;
        }
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        if (gameEvent.EventName == "GameStarted")
        {
            //listCoord = PlacementController.Instance.GetListCoord();
            //float koefDots = PlacementController.Instance.GetKoefScale();
            for (int i = 0; i < listCoord.Count; i++)
            {
                GameObject props = propsPooler.GetPooledGameObject();

                if (props)
                {
                    float randomX = UnityEngine.Random.Range(1, 9) / 10;
                    float randomY = UnityEngine.Random.Range(1, 9) / 10;

                    props.transform.localPosition = listCoord[i].transform.position;// + new Vector3(randomX,0f,randomY);
                    props.transform.localScale = PlacementController.Instance.transform.localScale;
                    props.SetActive(true);

                    propsList.Add(new ObjectLinkedToStartingPosition(props, listCoord[i].transform.position));

                    Debug.Log("Stend props = "+ props);
                }                  
            }

            gameStarted = true;
        }
    }

    private void FixedUpdate()
    {
        if (gameStarted)
        {
            timer += Time.fixedDeltaTime;

            if (timer > checkDisabledObjectsFrequency)
            {
                for (int i = 0; i < propsList.Count; i++)
                {
                    if (!propsList[i].props.activeInHierarchy)
                    {
                        propsList[i].props.transform.position = propsList[i].position;
                        propsList[i].props.transform.rotation = Quaternion.Euler(0, 0, 0);
                        propsList[i].props.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        propsList[i].props.SetActive(true);
                        break;
                    }
                }
                timer = 0;
            }
        }
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
    }
}
