using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(MeshRenderer))]
public class ColorChanger : NetworkBehaviour
{
    #region Variables
    [SerializeField] private InputAction ColorChangeInput;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    private Material playerMaterial = null;
    #endregion

    #region Netcode Variables
    private NetworkVariable<Color> currentColor = new NetworkVariable<Color>(Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    #endregion

    private void Awake()
    {
        playerMaterial = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        if (IsOwner)
        {
            ChangeColorServerRpc(color1);
        }
    }

    private void OnEnable()
    {
        currentColor.OnValueChanged += UpdateColor;
        ColorChangeInput.Enable();
    }

    private void OnDisable()
    {
        currentColor.OnValueChanged -= UpdateColor;
        ColorChangeInput.Disable();
    }

    private void Update()
    {
        if(IsOwner)
        {
            if(ColorChangeInput.WasPressedThisFrame())
            {
                Color newColor = color1;

                if(currentColor.Value == color1)
                {
                    newColor = color2;
                }

                ChangeColorServerRpc(newColor);
            }
        }
    }

    private void UpdateColor(Color previous, Color current)
    {
        if (!IsClient) return;

        playerMaterial.color = current;
    }
    
    [ServerRpc]
    private void ChangeColorServerRpc(Color newColor)
    {
        currentColor.Value = newColor;
    }
}
