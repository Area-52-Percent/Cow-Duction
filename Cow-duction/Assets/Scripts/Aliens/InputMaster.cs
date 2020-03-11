// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Aliens/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a86bc6fe-aa51-4a88-93da-5441d064bafc"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""a3f1ddf6-16c1-4e21-ac6f-ec50461b7491"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""591ac3b8-256b-4395-841d-a81cfd2869a0"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""13ed1fcd-a78f-487d-af42-aeef3e0fcf83"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Turn"",
                    ""type"": ""Value"",
                    ""id"": ""f5d5d181-a79b-4656-aceb-7b2dbef68631"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ascend"",
                    ""type"": ""Value"",
                    ""id"": ""ceb9a68f-7c56-428b-9303-1c55a69c6f39"",
                    ""expectedControlType"": ""Integer"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Descend"",
                    ""type"": ""Value"",
                    ""id"": ""b0c455ac-7e89-4559-aace-f137817e63aa"",
                    ""expectedControlType"": ""Integer"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TiltRight"",
                    ""type"": ""Button"",
                    ""id"": ""7846ee6d-dcef-403c-aa0a-e7a4a23f2134"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TiltLeft"",
                    ""type"": ""Button"",
                    ""id"": ""74909315-815a-45e7-8815-42e3a6398671"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cloak"",
                    ""type"": ""Button"",
                    ""id"": ""0492bc0b-85e3-44bf-a343-760ade2e0b59"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Release"",
                    ""type"": ""Button"",
                    ""id"": ""31b119fb-2a5a-4714-9020-2e98c9316d03"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PushPull"",
                    ""type"": ""Value"",
                    ""id"": ""e8532783-472a-4c70-9a4d-2e9e4139f500"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShootCow"",
                    ""type"": ""Button"",
                    ""id"": ""871b9732-1cab-4695-8207-0a5d3cf76ea5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dba96b0a-dd84-4fea-8672-2b23f2cc44f2"",
                    ""path"": ""<XInputController>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09f97627-6053-4ede-bfd2-6227cf350049"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c6be565-a419-42bb-8613-38928114c9d1"",
                    ""path"": ""<XInputController>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f4699ecf-b880-4203-87fb-17c36354c761"",
                    ""path"": ""<XInputController>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Turn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee0a4ad9-00a4-4f3b-bd2d-b23983f03a9d"",
                    ""path"": ""<XInputController>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Ascend"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6cd6bd75-1726-4f07-a895-dc311724d411"",
                    ""path"": ""<XInputController>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Descend"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""43cd067c-0345-458a-806e-7a8f296191ce"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""TiltRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1490a0f3-5bcd-4886-8920-d3da7e9f28d0"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""TiltLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb0521f3-8a06-4200-bfbd-d4526f19ae82"",
                    ""path"": ""<XInputController>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Cloak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19b1085a-f332-42c0-b5be-6bed5f4e0af3"",
                    ""path"": ""<XInputController>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Release"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cd918548-ea29-4b42-84d3-b0ea6cd01c4a"",
                    ""path"": ""<XInputController>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""PushPull"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""64ea9ef2-67b2-43ca-b89f-31c2bdbf201b"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""ShootCow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": []
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Shoot = m_Player.FindAction("Shoot", throwIfNotFound: true);
        m_Player_Aim = m_Player.FindAction("Aim", throwIfNotFound: true);
        m_Player_Turn = m_Player.FindAction("Turn", throwIfNotFound: true);
        m_Player_Ascend = m_Player.FindAction("Ascend", throwIfNotFound: true);
        m_Player_Descend = m_Player.FindAction("Descend", throwIfNotFound: true);
        m_Player_TiltRight = m_Player.FindAction("TiltRight", throwIfNotFound: true);
        m_Player_TiltLeft = m_Player.FindAction("TiltLeft", throwIfNotFound: true);
        m_Player_Cloak = m_Player.FindAction("Cloak", throwIfNotFound: true);
        m_Player_Release = m_Player.FindAction("Release", throwIfNotFound: true);
        m_Player_PushPull = m_Player.FindAction("PushPull", throwIfNotFound: true);
        m_Player_ShootCow = m_Player.FindAction("ShootCow", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Shoot;
    private readonly InputAction m_Player_Aim;
    private readonly InputAction m_Player_Turn;
    private readonly InputAction m_Player_Ascend;
    private readonly InputAction m_Player_Descend;
    private readonly InputAction m_Player_TiltRight;
    private readonly InputAction m_Player_TiltLeft;
    private readonly InputAction m_Player_Cloak;
    private readonly InputAction m_Player_Release;
    private readonly InputAction m_Player_PushPull;
    private readonly InputAction m_Player_ShootCow;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Shoot => m_Wrapper.m_Player_Shoot;
        public InputAction @Aim => m_Wrapper.m_Player_Aim;
        public InputAction @Turn => m_Wrapper.m_Player_Turn;
        public InputAction @Ascend => m_Wrapper.m_Player_Ascend;
        public InputAction @Descend => m_Wrapper.m_Player_Descend;
        public InputAction @TiltRight => m_Wrapper.m_Player_TiltRight;
        public InputAction @TiltLeft => m_Wrapper.m_Player_TiltLeft;
        public InputAction @Cloak => m_Wrapper.m_Player_Cloak;
        public InputAction @Release => m_Wrapper.m_Player_Release;
        public InputAction @PushPull => m_Wrapper.m_Player_PushPull;
        public InputAction @ShootCow => m_Wrapper.m_Player_ShootCow;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Shoot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Aim.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                @Turn.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurn;
                @Turn.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurn;
                @Turn.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurn;
                @Ascend.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAscend;
                @Ascend.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAscend;
                @Ascend.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAscend;
                @Descend.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDescend;
                @Descend.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDescend;
                @Descend.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDescend;
                @TiltRight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTiltRight;
                @TiltRight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTiltRight;
                @TiltRight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTiltRight;
                @TiltLeft.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTiltLeft;
                @TiltLeft.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTiltLeft;
                @TiltLeft.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTiltLeft;
                @Cloak.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCloak;
                @Cloak.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCloak;
                @Cloak.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCloak;
                @Release.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRelease;
                @Release.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRelease;
                @Release.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRelease;
                @PushPull.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPushPull;
                @PushPull.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPushPull;
                @PushPull.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPushPull;
                @ShootCow.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShootCow;
                @ShootCow.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShootCow;
                @ShootCow.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShootCow;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Turn.started += instance.OnTurn;
                @Turn.performed += instance.OnTurn;
                @Turn.canceled += instance.OnTurn;
                @Ascend.started += instance.OnAscend;
                @Ascend.performed += instance.OnAscend;
                @Ascend.canceled += instance.OnAscend;
                @Descend.started += instance.OnDescend;
                @Descend.performed += instance.OnDescend;
                @Descend.canceled += instance.OnDescend;
                @TiltRight.started += instance.OnTiltRight;
                @TiltRight.performed += instance.OnTiltRight;
                @TiltRight.canceled += instance.OnTiltRight;
                @TiltLeft.started += instance.OnTiltLeft;
                @TiltLeft.performed += instance.OnTiltLeft;
                @TiltLeft.canceled += instance.OnTiltLeft;
                @Cloak.started += instance.OnCloak;
                @Cloak.performed += instance.OnCloak;
                @Cloak.canceled += instance.OnCloak;
                @Release.started += instance.OnRelease;
                @Release.performed += instance.OnRelease;
                @Release.canceled += instance.OnRelease;
                @PushPull.started += instance.OnPushPull;
                @PushPull.performed += instance.OnPushPull;
                @PushPull.canceled += instance.OnPushPull;
                @ShootCow.started += instance.OnShootCow;
                @ShootCow.performed += instance.OnShootCow;
                @ShootCow.canceled += instance.OnShootCow;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnTurn(InputAction.CallbackContext context);
        void OnAscend(InputAction.CallbackContext context);
        void OnDescend(InputAction.CallbackContext context);
        void OnTiltRight(InputAction.CallbackContext context);
        void OnTiltLeft(InputAction.CallbackContext context);
        void OnCloak(InputAction.CallbackContext context);
        void OnRelease(InputAction.CallbackContext context);
        void OnPushPull(InputAction.CallbackContext context);
        void OnShootCow(InputAction.CallbackContext context);
    }
}
