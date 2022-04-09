using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    [SerializeField] private InputAction Move;

    private Transform _playerTransform;
    private Vector2 _movementInput;
    private float _rotSpeed = 5f;
    private float _speed = .2f;

    private void Awake()
    {
        _playerTransform = transform;
    }

    private void OnEnable()
    {
        Move.Enable();
    }

    private void OnDisable()
    {
        Move.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            MoveServerRpc(Move.ReadValue<Vector2>()); // client-server call
        }
    }

    private void FixedUpdate()
    {
        _playerTransform.Rotate(Vector3.up * _movementInput.x * _rotSpeed);
        _playerTransform.Translate(Vector3.forward * _movementInput.y * _speed);
    }

    [ServerRpc] // this method will only be executed in the server
    private void MoveServerRpc(Vector2 moveInput)
    {
        _movementInput = moveInput;
    }
}
