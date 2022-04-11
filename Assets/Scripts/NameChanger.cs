using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.InputSystem;
using System;

public enum PlayerNames
{
    Eric = 0,
    Dani = 1,
    Mario = 2,
    Iker = 3,
    Sergio = 4,
    David = 5,
    Julio = 6
}

public class NameChanger : NetworkBehaviour
{
    #region Variables
    [SerializeField] private InputAction ChangeNameInput;
    private Canvas playerCanvas = null;
    private Transform canvasTransform = null;
    private Text playerText = null;
    #endregion

    #region Netcode Variables
    private NetworkVariable<FixedString64Bytes> currentName = new NetworkVariable<FixedString64Bytes>("None", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    #endregion

    void Awake()
    {
        playerCanvas = GetComponentInChildren<Canvas>();
        canvasTransform = playerCanvas.transform;
        playerText = GetComponentInChildren<Text>();

        Camera mainCamera = FindObjectOfType<Camera>();
        playerCanvas.worldCamera = mainCamera;
    }

    private void Start()
    {
        if(IsOwner)
        {
            ChangeNameServerRpc("None");
        }
    }

    private void OnEnable()
    {
        ChangeNameInput.Enable();
        currentName.OnValueChanged += UpdateName;
    }

    private void OnDisable()
    {
        ChangeNameInput.Disable();
        currentName.OnValueChanged -= UpdateName;
    }

    private void UpdateName(FixedString64Bytes previous, FixedString64Bytes current)
    {
        if (!IsClient) return;

        playerText.text = current.ToString();
    }

    void Update()
    {
        canvasTransform.LookAt(playerCanvas.worldCamera.transform);
        if(IsOwner)
        {
            if (ChangeNameInput.WasPressedThisFrame())
            {
                string newRandomName = GetRandomName();
                ChangeNameServerRpc(newRandomName);
            }
        }
    }

    private string GetRandomName()
    {
        int randomIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(PlayerNames)).Length);
        return Enum.GetName(typeof(PlayerNames), randomIndex);
    }

    [ServerRpc]
    private void ChangeNameServerRpc(string newName)
    {
        currentName.Value = newName;
    }
}
