using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    Transform thisTransform;
    [SerializeField] float rotationSpeed = 10;
    // Start is called before the first frame update
    void Start()
    {
        thisTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        thisTransform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f);
    }
}
