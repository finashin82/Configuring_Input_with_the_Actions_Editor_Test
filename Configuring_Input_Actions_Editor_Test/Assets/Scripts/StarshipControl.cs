using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class StarshipControl : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    private CharacterController characterController;

    [SerializeField] private float _maxSpeed = 10f;
   
    [SerializeField] private float _accelerationFactor = 3f;

    [SerializeField] private float _decelerationFactor = 1f;

    [SerializeField] AudioSource _soundStarship;

    private bool isEngine;

    private bool isEngineOn = false;

    private bool start = false;

    private bool isFly = false;

    private bool isPlaySound = true;

    private float currentSpeed;

    private float amplitude;

    private float flightAltitude = 2f;

    private float speedUp = 3f;

    private float time = 0f;

    private float cornerRolling = 3f;       

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
        Rolling(cornerRolling);

        if (isEngine && !start)
        {
            isEngineOn = !isEngineOn;

            start = true;

            time = 0;

            isEngine = false;

            Debug.Log(isEngineOn);
        }

        if (isEngineOn && start)
        {
            if (isPlaySound)
            {
                isPlaySound = false;
                _soundStarship.Play();
            }            

            UpDown(Vector3.up);

            isFly = true;            
        }

        if (!isEngineOn && start) 
        {
            UpDown(Vector3.down);

            isFly = false;
            if (!isPlaySound)
            {
                isPlaySound = true;

                _soundStarship.Stop();
            }            
        }

        GatherInput();

        CalculareSpeed();

        if (isFly)
        {
            Move();
        }
    }

    /// <summary>
    /// Взлет и посадка
    /// </summary>
    /// <param name="direction"></param>
    private void UpDown(Vector3 direction)
    {
        time += Time.deltaTime;

        if (time < flightAltitude)
        {
            characterController.Move(direction * speedUp * Time.deltaTime);
        }
        else
        {
            start = false;
        }
    }

   /// <summary>
   /// Качание объекта
   /// </summary>
   /// <param name="corner"></param>
    private void Rolling(float corner)
    {
        float x = Mathf.Sin(Time.time) * amplitude;
        float y = Mathf.Sin(Time.time) * amplitude;
        float z = Mathf.Sin(Time.time) * amplitude;

        transform.rotation = Quaternion.Euler(x, y, z);

        if (Mathf.Round(amplitude) == 0)
        {
            amplitude = UnityEngine.Random.Range(-corner, corner);
        }
    }

    /// <summary>
    /// Вычисление скорости
    /// </summary>
    private void CalculareSpeed()
    {
        if (input == Vector3.zero && currentSpeed > 0)
        {
            currentSpeed -= _decelerationFactor * Time.deltaTime;
        }
        else if (input != Vector3.zero && currentSpeed < _maxSpeed)
        {
            currentSpeed += _accelerationFactor * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, _maxSpeed);
    }

    /// <summary>
    /// Движение игрока
    /// </summary>
    private void Move()
    {
        characterController.Move(input * currentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Получаем входные данные
    /// </summary>
    private void GatherInput()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        input = new Vector3(inputVector.x, 0, inputVector.y);

        isEngine = inputActions.Player.Sprint.IsPressed();
    }
}
