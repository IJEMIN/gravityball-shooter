using UnityEngine;

public class InputModeSwitch : MonoBehaviour
{
    public void ToggleMode(GravityBall.InputMode inputMode)
    {
        GravityBall.Instance.inputMode = inputMode;
    }

    public void ActiveSwipe(bool active)
    {
        GravityBall.Instance.inputMode =
            active ? GravityBall.InputMode.Swipe : GravityBall.InputMode.Push;
    }

    public void ActivePush(bool active)
    {
        GravityBall.Instance.inputMode =
            active ? GravityBall.InputMode.Push : GravityBall.InputMode.Swipe;
    }
}