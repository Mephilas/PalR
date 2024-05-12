using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI管理器
/// </summary>
public sealed class UIManager_ : SingletonBase<UIManager_>
{
    /// <summary>
    /// UI面板集合
    /// </summary>
    private static readonly Dictionary<UIPanel, UIPanelBase> _uiPanelDic = new();

    /// <summary>
    /// 旧面板
    /// </summary>
    private static UIPanel _lastPanel;

    /// <summary>
    /// 当前UI面板
    /// </summary>
    private static UIPanel _currentUIPanel;

    /// <summary>
    /// 当前UI面板
    /// </summary>
    private static UIPanelBase _currentPanel;

    /*/// <summary>
    /// 黑图
    /// </summary>
    public static Sprite EmptyBlack { get; private set; }

    /// <summary>
    /// 白图
    /// </summary>
    public static Sprite EmptyWhite { get; private set; }*/

    /// <summary>
    /// 摇杆轴
    /// </summary>
    private static float _axisX, _axisY;

    /// <summary>
    /// 摇杆归零
    /// </summary>
    private static bool _axisZero = true;

    /// <summary>
    /// 上级面板隐藏
    /// </summary>
    private static bool _lastHide;

    protected override void Awake()
    {
        base.Awake();

        GameManager_.Register(GameEventType.UIPanel, PanelSwitch);
        GameManager_.Register(GameEventType.UIPanelReturn, PanelReturn);

        /*EmptyBlack = Resources.Load<Sprite>("EmptyBlack");
        EmptyWhite = Resources.Load<Sprite>("EmptyWhite");*/

        foreach (UIPanel uiPanel in Enum.GetValues(typeof(UIPanel)))
        {
            //uiPanelDic.Add(uiPanel, Instantiate(Resources.Load<GameObject>("Prefabs/UI/" + uiPanel), canvasT).GetComponent<UIPanelBase>());
            _uiPanelDic.Add(uiPanel, Root_.Instance.CGC<UIPanelBase>("Canvas/" + uiPanel.ToString()));
        }
    }

    protected override void Start()
    {
        base.Start();

        (_currentPanel = _uiPanelDic[_currentUIPanel = UIPanel.InitPanel]).Active();
    }

    protected override void Update()
    {
        base.Update();

        InputCheck();
    }

    /// <summary>
    /// 输入检查
    /// </summary>
    private static void InputCheck()
    {
        if (!_currentPanel.InputSwitch) return;

        foreach (KeyCode keyCode in _currentPanel.DownInputDic.Keys)
        {
            if (Input.GetKeyDown(keyCode) && null != _currentPanel.DownInputDic[keyCode])
            {
                _currentPanel.DownInputDic[keyCode]();

                break;
            }
        }

        foreach (KeyCode keyCode in _currentPanel.HoldInputDic.Keys)
        {
            if (Input.GetKey(keyCode) && null != _currentPanel.HoldInputDic[keyCode])
            {
                _currentPanel.HoldInputDic[keyCode]();

                break;
            }
        }

        _axisX = Input.GetAxis("LeftHorizontal");
        _axisY = Input.GetAxis("LeftVertical");

        if (_axisZero)
        {
            _axisZero = false;

            if (0 < _axisY)
            {
                _currentPanel.DownInputDic[KeyCode.UpArrow]();
            }
            else if (_axisY < 0)
            {
                _currentPanel.DownInputDic[KeyCode.DownArrow]();
            }
            else if (_axisX < 0)
            {
                _currentPanel.DownInputDic[KeyCode.LeftArrow]();
            }
            else if (0 < _axisX)
            {
                _currentPanel.DownInputDic[KeyCode.RightArrow]();
            }
        }

        _axisZero = 0 == _axisX && 0 == _axisY;
    }

    /// <summary>
    /// UI面板切换
    /// </summary>
    /// <param name="switchArgumentArray">切换参数集合</param>
    private static void PanelSwitch(string[] switchArgumentArray)
    {
        _lastPanel = _currentUIPanel;

        _lastHide = true;
        if (1 != switchArgumentArray.Length) _lastHide = bool.Parse(switchArgumentArray[1]);

        ToolsE.Log("Last panel : " + _currentPanel.gameObject.name + "   New panel : " + switchArgumentArray[0]);
        _currentPanel.Inactive(_lastHide);
        (_currentPanel = _uiPanelDic[_currentUIPanel = switchArgumentArray[0].S2E<UIPanel>()]).Active(switchArgumentArray);
    }

    /// <summary>
    /// UI面板返回
    /// </summary>
    private static void PanelReturn(string[] nil = null) => PanelSwitch(new string[] { _lastPanel.ToString() });

    /// <summary>
    /// 清除所有面板
    /// </summary>
    public static void PanelClear()
    {
        for (int i = 0; i != _uiPanelDic.Count; i++)
            _uiPanelDic[(UIPanel)i].ForceHide();
    }

    /// <summary>
    /// 面板比较
    /// </summary>
    /// <param name="uiPanel">面板枚举</param>
    /// <returns>相同</returns>
    public static bool PanelCompare(in UIPanel uiPanel) => uiPanel == _currentUIPanel;
}


/// <summary>
/// UI面板
/// </summary>
public enum UIPanel
{
    /// <summary>
    /// CG面板
    /// </summary>
    CGPanel,

    /// <summary>
    /// 主界面
    /// </summary>
    MainPanel,

    /// <summary>
    /// 台词面板
    /// </summary>
    DialoguePanel,

    /// <summary>
    /// 提示面板
    /// </summary>
    TipPanel,

    /// <summary>
    /// 交易面板
    /// </summary>
    TradePanel,

    /// <summary>
    /// 选择面板
    /// </summary>
    ChoosePanel,

    /// <summary>
    /// 基础面板
    /// </summary>
    BasicPanel,

    /// <summary>
    /// 战斗面板
    /// </summary>
    BattlePanel,

    /// <summary>
    /// 战斗扩展面板
    /// </summary>
    BattleExtensionPanel,

    /// <summary>
    /// 战斗物品面板
    /// </summary>
    BattleItemPanel,

    /// <summary>
    /// 战斗仙术面板
    /// </summary>
    BattleSkillPanel,

    /// <summary>
    /// 结算面板
    /// </summary>
    SettlementPanel,

    /// <summary>
    /// 团灭面板
    /// </summary>
    GGPanel,

    /// <summary>
    /// Escape面板
    /// </summary>
    EscapePanel,

    /// <summary>
    /// 状态面板
    /// </summary>
    StatePanel,

    /// <summary>
    /// 仙术面板
    /// </summary>
    SkillPanel,

    /// <summary>
    /// 物品面板
    /// </summary>
    ItemPanel,

    /// <summary>
    /// 使用面板
    /// </summary>
    UsePanel,

    /// <summary>
    /// 装备面板
    /// </summary>
    EquipPanel,

    /// <summary>
    /// 系统面板
    /// </summary>
    SystemPanel,

    /// <summary>
    /// 存档面板
    /// </summary>
    SLPanel,

    /// <summary>
    /// 初始化面板
    /// </summary>
    InitPanel,

    /// <summary>
    /// 空面板
    /// </summary>
    NullPanel
}