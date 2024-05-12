using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 基础面板
/// </summary>
public sealed class BasicPanel : UIPanelBase
{
    /// <summary>
    /// 输入层
    /// </summary>
    private static readonly Dictionary<KeyCode, UnityEngine.Events.UnityAction> _inputActionDic = new()
    {
        { KeyCode.UpArrow, () => Move(InputType.Up) },
        { KeyCode.DownArrow, () => Move(InputType.Down) },
        { KeyCode.LeftArrow, () => Move(InputType.Left) },
        { KeyCode.RightArrow, () => Move(InputType.Right) }
    };

    /// <summary>
    /// 移动输入集合
    /// </summary>
    private static readonly List<KeyCode> _moveInputList = new() { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    /// <summary>
    /// 当前按键
    /// </summary>
    private static KeyCode? _keyCode;

    /// <summary>
    /// 摇杆轴
    /// </summary>
    private static float _axisX, _axisY;

    /// <summary>
    /// 任务介绍
    /// </summary>
    private static UnityEngine.UI.Text _missionT;

    /// <summary>
    /// 屏幕宽高
    /// </summary>
    private static float _screenX, _screenY;

    protected override void Awake()
    {
        base.Awake();

        /*for (int i = 0; i != _moveInputList.Count; i++)
        {
            int index = i;

            InputType inputType = i.I2E<InputType>();

            CGB(inputType.ToString()).Init(() => Move(inputType), false);
        }*/

        CGB("Escape").Init(Escape);
        CGC(ref _missionT, "MissionName");

        GameManager_.MissionTUpdate = MissionTUpdate;
    }

    protected override void Escape() => GameManager_.Trigger(ESCAPE_PANEL_EVENT);

    protected override void Enter() => GameManager_.Leader.Detect();

    protected override void Update()
    {
        base.Update();

        if (IsActive)
        {
            _keyCode = null;

            if (Input.GetMouseButtonUp(0)) GameManager_.Leader.ScreenRaycast();

            if (Input.GetMouseButton(0))
            {
                if (_screenX <= Input.mousePosition.x && _screenY <= Input.mousePosition.y)
                {
                    _keyCode = KeyCode.UpArrow;
                }
                else if (Input.mousePosition.x < _screenX && Input.mousePosition.y < _screenY)
                {
                    _keyCode = KeyCode.DownArrow;
                }
                else if (Input.mousePosition.x < _screenX && _screenY <= Input.mousePosition.y)
                {
                    _keyCode = KeyCode.LeftArrow;
                }
                else
                {
                    _keyCode = KeyCode.RightArrow;
                }
            }

            for (int i = 0; i != _moveInputList.Count; i++)
            {
                if (Input.GetKeyDown(_moveInputList[i]))
                {
                    (_moveInputList[i], _moveInputList[0]) = (_moveInputList[0], _moveInputList[i]);

                    break;
                }
            }

            for (int i = 0; i != _moveInputList.Count; i++)
            {
                if (Input.GetKey(_moveInputList[i]))
                {
                    _keyCode = _moveInputList[i];

                    break;
                }
            }

            _axisX = Input.GetAxis("LeftHorizontal");
            _axisY = Input.GetAxis("LeftVertical");

            if (0 < _axisY)
            {
                _keyCode = KeyCode.UpArrow;
            }
            else if (_axisY < 0)
            {
                _keyCode = KeyCode.DownArrow;
            }
            else if (_axisX < 0)
            {
                _keyCode = KeyCode.LeftArrow;
            }
            else if (0 < _axisX)
            {
                _keyCode = KeyCode.RightArrow;
            }

            if (null != _keyCode) _inputActionDic[(KeyCode)_keyCode]();
        }
    }

    private static void Move(InputType inputType) => GameManager_.Leader.InputHandle(inputType);

    private static void MissionTUpdate(string value) => _missionT.text = value;

    public override void Active(string[] argumentArray = null)
    {
        base.Active();

        _screenX = Screen.width * 0.5f;
        _screenY = Screen.height * 0.5f;

        GameManager_.Leader.MovementSwitch(true);
    }

    public override void Inactive(bool hide)
    {
        base.Inactive(hide);

        if (GameManager_.InGame) GameManager_.Leader.MovementSwitch(false);
    }
}