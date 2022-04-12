using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class Shooter : NetworkBehaviour
{
    #region Variables
    [SerializeField] private InputAction ShootInput;
    private float _reach = 5f;
    private LineRenderer _line;
    private Material _playerMaterial = null;
    #endregion

    #region NetcodeVariables
    private NetworkVariable<Color> currentColor = new NetworkVariable<Color>(Color.white, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    #endregion

    private void Awake()
    {
        _playerMaterial = GetComponent<MeshRenderer>().material;
        _line = GetComponentInChildren<LineRenderer>();
    }

    void Start()
    {
        
    }

    private void OnEnable()
    {
        currentColor.OnValueChanged += UpdateColor;
        ShootInput.Enable();
    }

    private void OnDisable()
    {
        currentColor.OnValueChanged -= UpdateColor;
        ShootInput.Disable();
    }

    void Update()
    {
        if (IsOwner)
        {
            // Draw ray in shooting direction
            Color rayColor = Color.green;
            if (ShootInput.IsPressed())
            {
                rayColor = Color.red;
            }
            if (ShootInput.WasReleasedThisFrame())
            {
                rayColor = Color.green;
            }
            _line.material.color = rayColor;
        }

    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            // Cast a ray in shooting direction
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Ray ray = new Ray(transform.position, forward);
            if (ShootInput.WasPressedThisFrame())
            {
                ShootServerRpc(ray, _reach);
            }
        }
    }

    private void UpdateColor(Color previous, Color current)
    {
        if (!IsClient) return;

        _playerMaterial.color = current;
    }

    [ServerRpc]
    private void ShootServerRpc(Ray ray, float reach)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _reach))
        {
            ulong playerId = hit.collider.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        }
    }
}
