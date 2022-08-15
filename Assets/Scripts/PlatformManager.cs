
using UnityEngine;

public class PlatformManager : ScriptableObject
{
    public enum InputMode
    {
        None = 0,
        Mouse = 1,
        Keyboard = 2,
        MouseAndKeyboard = 3,
        Touch = 4,
        Joystick = 5
    }

    public enum Platform //todo implement
    {
        None = 0,
        Desktop = 1,
        Mobile = 2
    }  
    
    //INPUT MODE
    private InputMode currInputMode;

    public InputMode CurrInputMode
    {
        get => currInputMode;
        set => currInputMode = value;
    }

    //PLATFORM
    private Platform currPlatform;

    public Platform CurrPlatform
    {
        get => currPlatform;
        set => currPlatform = value;
    }
    public void Init()
    {
        CurrPlatform = Platform.Desktop;
        CurrInputMode = InputMode.MouseAndKeyboard;
    }
}
