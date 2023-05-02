using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    // Start is called before the first frame update
    private NatureEnvController envController;
    public float cameraSpeed = 2f;
    void Start()
    {
        envController = GetComponentInParent<NatureEnvController>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(envController.transform.position);
        this.transform.Translate(cameraSpeed * Vector3.right * Time.deltaTime);
    }
}
