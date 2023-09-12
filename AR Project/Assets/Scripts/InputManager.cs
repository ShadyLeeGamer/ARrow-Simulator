using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour
{
    GameManager gameManager;

    public Action<float> OnHorizontalRotateInput;
    public Action<float> OnVerticalRotateInput;

    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable(); // Enable mouse input to simulate touch
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += OnFingerDown;
        EnhancedTouch.Touch.onFingerMove += OnFingerMove;
        EnhancedTouch.Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
        EnhancedTouch.Touch.onFingerMove -= OnFingerMove;
        EnhancedTouch.Touch.onFingerUp -= OnFingerUp;
    }

    void OnFingerDown(EnhancedTouch.Finger finger)
    {
        // Ignore multi-touch
        if (finger.index != 0 ||
            IsPointerOverUI(finger.currentTouch.screenPosition))
            return;
    }

    void OnFingerMove(EnhancedTouch.Finger finger)
    {
        // Ignore multi-touch
        if (finger.index != 0 ||
            IsPointerOverUI(finger.currentTouch.screenPosition))
            return;

        if (gameManager.TurnType == GameManager.TurnTypes.Battle)
        {
            OnHorizontalRotateInput?.Invoke(Input.GetTouch(0).deltaPosition.x);
            OnVerticalRotateInput?.Invoke(Input.GetTouch(0).deltaPosition.y);
        }
    }

    void OnFingerUp(EnhancedTouch.Finger finger)
    {
    }

    bool IsPointerOverUI(Vector2 touchPosition)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}