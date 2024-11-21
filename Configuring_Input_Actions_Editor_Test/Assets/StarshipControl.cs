using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class StarshipControl : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 10f;

    [SerializeField] private float _rotationSpeed = 360f;

    [SerializeField] private float _accelerationFactor = 5f;

    [SerializeField] private float _decelerationFactor = 10f;

    private float currentSpeed;

    private InputSystem_Actions inputActions;

    private CharacterController characterController;

    private Vector3 input;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        GatherInput();

        Look();

        CalculareSpeed();

        Move();
    }

    private void CalculareSpeed()
    {
        if (input == Vector3.zero && currentSpeed > 0) 
        {
            currentSpeed -= _decelerationFactor * Time.deltaTime;
        }
        else if(input == Vector3.zero && currentSpeed < maxSpeed)
        {
            currentSpeed += _accelerationFactor * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
    }

    private void Look()
    {
        if (input == Vector3.zero) return;

        Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

        Vector3 multipliedMatrix = isometricMatrix.MultiplyPoint3x4(input);

        Quaternion rotation = Quaternion.LookRotation(multipliedMatrix, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        Vector3 moveDirection = transform.forward * maxSpeed * input.magnitude * Time.deltaTime;
    }

    private void GatherInput()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        input = new Vector3(inputVector.x, 0, inputVector.y);

        Debug.Log(input);
    }

}
