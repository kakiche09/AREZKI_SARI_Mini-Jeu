using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    PlayerInput inputs;

    void Awake()
    {
        inputs = GetComponent<PlayerInput>();
    }

    void Start()
    {
        // Buttons (Main)
        inputs.actions.FindAction("BS").performed     += BS.Event;
        inputs.actions.FindAction("BW").performed     += BW.Event;
        inputs.actions.FindAction("BN").performed     += BN.Event;
        inputs.actions.FindAction("BE").performed     += BE.Event;

        // Buttons (Menu)
        inputs.actions.FindAction("Back").performed   += Back.Event;
        inputs.actions.FindAction("Menu").performed   += Menu.Event;

        // Bumpers
        inputs.actions.FindAction("LB").performed     += LB.Event;
        inputs.actions.FindAction("RB").performed     += RB.Event;        

        // Triggers
        inputs.actions.FindAction("LT").performed     += LT.Event;
        inputs.actions.FindAction("RT").performed     += RT.Event;

        // Sticks (Movement)
        inputs.actions.FindAction("LS_m").performed   += LS_m.Event;
        inputs.actions.FindAction("RS_m").performed   += RS_m.Event;

        // Sticks (Buttons)
        inputs.actions.FindAction("LS_b").performed   += LS_b.Event;
        inputs.actions.FindAction("RS_b").performed   += RS_b.Event;

        // D-Pad
        inputs.actions.FindAction("DPad").performed   += DPAD_m.Event;
    }

    // Boutons (Main)
    public InputEvent BS = new InputEvent();
    public InputEvent BW = new InputEvent();
    public InputEvent BN = new InputEvent();
    public InputEvent BE = new InputEvent();

    // Buttons (Menu)
    public InputEvent Back = new InputEvent();
    public InputEvent Menu = new InputEvent();

    // Bumpers
    public InputEvent LB = new InputEvent();
    public InputEvent RB = new InputEvent();

    // Triggers
    public InputEvent_axis LT = new InputEvent_axis();
    public InputEvent_axis RT = new InputEvent_axis();

    // Sticks (Movement)
    public InputEvent_vector2 LS_m = new InputEvent_vector2();
    public InputEvent_vector2 RS_m = new InputEvent_vector2();

    // Sticks (Buttons)
    public InputEvent LS_b = new InputEvent();
    public InputEvent RS_b = new InputEvent();

    // D-Pad
    public InputEvent_vector2 DPAD_m = new InputEvent_vector2();
}

public class InputEvent
{
    public delegate void ActionPerformed();
    public event ActionPerformed callback;

    public void Event(InputAction.CallbackContext context)
    {
        if (callback != null)
            callback();
    }
}

public class InputEvent_vector2
{
    public delegate void ActionPerformed(Vector2 value);
    public event ActionPerformed callback;

    public void Event(InputAction.CallbackContext context)
    {
        if (callback != null)
            callback(context.ReadValue<Vector2>());
    }
}

public class InputEvent_axis
{
    public delegate void ActionPerformed(float value);
    public event ActionPerformed callback;

    public void Event(InputAction.CallbackContext context)
    {
        if (callback != null)
            callback(context.ReadValue<float>());
    }
}


