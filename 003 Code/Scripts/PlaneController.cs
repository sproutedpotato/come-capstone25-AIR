using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [SerializeField] private GameObject target;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);
    }
}
