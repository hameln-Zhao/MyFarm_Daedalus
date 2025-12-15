using UnityEngine;

/// <summary>
/// 管理玩家 HP / MP 及其事件分发。
/// 挂在 Player 同一个 GameObject 上。
/// 其他脚本通过 EventHandler 订阅变化。
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("SO文件")]
    [SerializeField] private PlayerStats_SO playerStats_SO;
    
    [Header("调试显示")]
    [SerializeField] private int _currentHP;
    [SerializeField] private int _currentMP;
    [SerializeField] private bool _isDead;
    
    public int MaxHP { get; private set; }
    public int MaxMP { get; private set; }
    public int CurrentHP => _currentHP;
    public int CurrentMP => _currentMP;
    public bool IsDead => _isDead;

    private void Awake()
    {
        InitFromConfigOrFallback();
        ClampAll();
    }

    private void Start()
    {
        // 启动时主动同步一次 UI（避免 UI 比 stats 先启用导致条不正确）
        BroadcastHPChanged();
        BroadcastMPChanged();
    }

    #region 初始化 / 重置

    private void InitFromConfigOrFallback()
    {
        if (playerStats_SO != null)
        {
            MaxHP = Mathf.Max(1, playerStats_SO.maxHP);
            MaxMP = Mathf.Max(0, playerStats_SO.maxMP);

            _currentHP = Mathf.Clamp(playerStats_SO.startHP, 0, MaxHP);
            _currentMP = Mathf.Clamp(playerStats_SO.startMP, 0, MaxMP);
        }
        else
        {
            // 没配 SO 也能跑（兼容你旧的 Inspector 配置思维）
            if (MaxHP <= 0) MaxHP = 100;
            if (MaxMP < 0) MaxMP = 50;

            // 如果 Inspector 没填 current，就给满
            if (_currentHP <= 0) _currentHP = MaxHP;
            if (_currentMP <= 0) _currentMP = MaxMP;
        }

        _isDead = (_currentHP <= 0);
    }

    /// <summary>将状态重置为模板初始值（复活、读档、重开局常用）。</summary>
    public void ResetToConfig()
    {
        if (playerStats_SO == null)
        {
            Debug.LogWarning("PlayerStats: ResetToConfig called but playerStats_SO is null.");
            return;
        }

        MaxHP = Mathf.Max(1, playerStats_SO.maxHP);
        MaxMP = Mathf.Max(0, playerStats_SO.maxMP);
        _currentHP = Mathf.Clamp(playerStats_SO.startHP, 0, MaxHP);
        _currentMP = Mathf.Clamp(playerStats_SO.startMP, 0, MaxMP);

        _isDead = (_currentHP <= 0);

        BroadcastHPChanged();
        BroadcastMPChanged();

        // 如果你希望“复活”也广播一个事件，可以在这里加
        // EventHandler.CallPlayerRevived();
    }

    private void ClampAll()
    {
        MaxHP = Mathf.Max(1, MaxHP);
        MaxMP = Mathf.Max(0, MaxMP);
        _currentHP = Mathf.Clamp(_currentHP, 0, MaxHP);
        _currentMP = Mathf.Clamp(_currentMP, 0, MaxMP);
    }

    #endregion

    #region HP 接口

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || _isDead) return;

        int before = _currentHP;
        _currentHP = Mathf.Clamp(_currentHP - dmg, 0, MaxHP);
        int real = before - _currentHP; // 实际扣掉多少

        if (real > 0)
        {
            EventHandler.CallPlayerDamaged(real, _currentHP, MaxHP);
            BroadcastHPChanged();
        }

        if (_currentHP <= 0 && !_isDead)
        {
            _isDead = true;
            EventHandler.CallPlayerDied();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || _isDead) return;

        int before = _currentHP;
        _currentHP = Mathf.Clamp(_currentHP + amount, 0, MaxHP);
        int real = _currentHP - before;

        if (real > 0)
        {
            EventHandler.CallPlayerHealed(real, _currentHP, MaxHP);
            BroadcastHPChanged();
        }
    }

    /// <summary>
    /// 设置最大 HP。
    /// keepRatio=true：保持当前血量比例；false：仅修改上限并 clamp 当前值。
    /// </summary>
    public void SetMaxHP(int newMax, bool keepRatio = true)
    {
        if (newMax <= 0) return;

        float ratio = (MaxHP > 0) ? (float)_currentHP / MaxHP : 1f;

        MaxHP = newMax;

        if (keepRatio)
            _currentHP = Mathf.RoundToInt(MaxHP * ratio);

        _currentHP = Mathf.Clamp(_currentHP, 0, MaxHP);

        // 如果最大值变更导致“从死变活/从活变死”，你也可以自行决定是否处理
        if (_currentHP <= 0 && !_isDead)
        {
            _isDead = true;
            EventHandler.CallPlayerDied();
        }

        BroadcastHPChanged();
    }

    #endregion

    #region MP 接口

    public bool ConsumeMP(int cost)
    {
        if (cost <= 0) return true;
        if (_currentMP < cost) return false;

        _currentMP -= cost;

        EventHandler.CallPlayerManaConsumed(cost, _currentMP, MaxMP);
        BroadcastMPChanged();
        return true;
    }

    public void RecoverMP(int amount)
    {
        if (amount <= 0) return;

        int before = _currentMP;
        _currentMP = Mathf.Clamp(_currentMP + amount, 0, MaxMP);
        int real = _currentMP - before;

        if (real > 0)
        {
            EventHandler.CallPlayerManaRecovered(real, _currentMP, MaxMP);
            BroadcastMPChanged();
        }
    }

    public void SetMaxMP(int newMax, bool keepRatio = true)
    {
        if (newMax < 0) return;

        float ratio = (MaxMP > 0) ? (float)_currentMP / MaxMP : 1f;

        MaxMP = newMax;

        if (keepRatio)
            _currentMP = Mathf.RoundToInt(MaxMP * ratio);

        _currentMP = Mathf.Clamp(_currentMP, 0, MaxMP);

        BroadcastMPChanged();
    }

    #endregion

    #region 事件封装（避免漏发/重复写）

    private void BroadcastHPChanged()
    {
        EventHandler.CallPlayerHealthChanged(_currentHP, MaxHP);
    }

    private void BroadcastMPChanged()
    {
        EventHandler.CallPlayerManaChanged(_currentMP, MaxMP);
    }

    #endregion
    



}

