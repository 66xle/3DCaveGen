using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    public GameObject map;
    public GameObject target;
    private CaveGen script;
    private LayerMask layerMask = (1 << 0);


    // Start is called before the first frame update
    void Start()
    {
        script = map.GetComponent("CaveGen") as CaveGen;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out hit, distance, layerMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);

            Vector3 point = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            point += (new Vector3(hit.normal.x, hit.normal.y, hit.normal.z)) * -0.5f;

            script.data[Mathf.RoundToInt(point.x - 0.5f), Mathf.RoundToInt(point.y + 0.5f), Mathf.RoundToInt(point.z - 0.5f)] = 0;

            //script.CreateMesh();

        }
        else
        {
            Debug.DrawLine(transform.position, target.transform.position, Color.blue);
        }
    }
}
