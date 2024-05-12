using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// UI面板基类
/// </summary>
[DisallowMultipleComponent]
public abstract class UIPanelBase : UIBase_
{
    /// <summary>
    /// 面板返回事件
    /// </summary>
    public static readonly GameEventData PANEL_RETURN_EVENT = new(GameEventType.UIPanelReturn);

    /// <summary>
    /// 空面板事件
    /// </summary>
    public static readonly GameEventData NULL_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.NullPanel.ToString());

    /// <summary>
    /// 主面板事件
    /// </summary>
    protected static readonly GameEventData MAIN_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.MainPanel.ToString());

    /// <summary>
    /// 基础面板事件
    /// </summary>
    public static readonly GameEventData BASIC_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.BasicPanel.ToString());

    /// <summary>
    /// Escape面板事件
    /// </summary>
    protected static readonly GameEventData ESCAPE_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.EscapePanel.ToString());

    /// <summary>
    /// 物品面板事件
    /// </summary>
    protected static readonly GameEventData ITEM_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.ItemPanel.ToString());

    /// <summary>
    /// 物品面板事件
    /// </summary>
    protected static readonly GameEventData BATTLE_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.BattlePanel.ToString());

    /// <summary>
    /// 存档面板事件
    /// </summary>
    protected static readonly GameEventData SL_PANEL_EVENT = new(GameEventType.UIPanel, UIPanel.SLPanel.ToString(), "False");

    /// <summary>
    /// BG停止事件
    /// </summary>
    protected static readonly GameEventData BG_STOP = new(GameEventType.BGPlay, "-1");

    /// <summary>
    /// 激活
    /// </summary>
    protected bool IsActive { get; private set; }

    /// <summary>
    /// 输入开关
    /// </summary>
    public bool InputSwitch { get; protected set; } = true;

    /// <summary>
    /// 按下输入层
    /// </summary>
    public readonly Dictionary<KeyCode, UnityAction> DownInputDic = new();

    /// <summary>
    /// 长按输入层
    /// </summary>
    public readonly Dictionary<KeyCode, UnityAction> HoldInputDic = new();

    /// <summary>
    /// 总开关
    /// </summary>
    //private bool _switch;  //切换面板后临时关闭输入直到抬起防止重复触发

    protected override void Awake()
    {
        base.Awake();

        DownInputDic.Add(KeyCode.Escape, Escape);
        DownInputDic.Add(KeyCode.JoystickButton1, Escape);
        DownInputDic.Add(KeyCode.JoystickButton0, Enter);
        DownInputDic.Add(KeyCode.Space, Enter);
        DownInputDic.Add(KeyCode.Return, Enter);
        DownInputDic.Add(KeyCode.KeypadEnter, Enter);
        DownInputDic.Add(KeyCode.UpArrow, Up);
        DownInputDic.Add(KeyCode.DownArrow, Down);
        DownInputDic.Add(KeyCode.LeftArrow, Left);
        DownInputDic.Add(KeyCode.RightArrow, Right);

        Hide();
    }

    /// <summary>
    /// 激活
    /// </summary>
    /// <param name="argumentArray">可选参数列表</param>
    public virtual void Active(string[] argumentArray = null)
    {
        if (!IsActive)
        {
            IsActive = true;

            Display();
            RectT.SetAsLastSibling();
        }
    }

    /// <summary>
    /// 失活
    /// </summary>
    /// <param name="hide">隐藏/冻结</param>
    public virtual void Inactive(bool hide)
    {
        if (IsActive)
        {
            IsActive = false;

            if (hide) Hide();
        }
    }

    public void ForceHide() => Hide();

    /// <summary>
    /// 返回
    /// </summary>
    protected virtual void Escape() { }

    /// <summary>
    /// 进入
    /// </summary>
    protected virtual void Enter() { }

    /// <summary>
    /// 上
    /// </summary>
    protected virtual void Up() { }

    /// <summary>
    /// 下
    /// </summary>
    protected virtual void Down() { }

    /// <summary>
    /// 左
    /// </summary>
    protected virtual void Left() { }

    /// <summary>
    /// 右
    /// </summary>
    protected virtual void Right() { }
}


/// <summary>
/// 输入类型
/// </summary>
public enum InputType
{
    Up,
    Down,
    Left,
    Right,
    Esc,
    Enter,
    Space,
}