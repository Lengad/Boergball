﻿using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float CameraMoveSpeed = 120.0f;
    [SerializeField]
    private float clampAngle = 80.0f;
    [SerializeField]
    private float inputSensitivity = 150.0f;
    [SerializeField]
    private float mouseX;
    [SerializeField]
    private float mouseY;
    [SerializeField]
    private float finalInputX;
    [SerializeField]
    private float finalInputZ;
    private float rotY = 0.0f;
    private float rotX = 0.0f;

    public Transform target;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        finalInputX =  mouseX;
        finalInputZ =  mouseY;

        rotY += finalInputX * inputSensitivity * Time.deltaTime;
        rotX += finalInputZ * inputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(-rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;
        CameraUpdater();
    }

    private void CameraUpdater()
    {
        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position + Vector3.up * 1.2f, step);
    }
}
