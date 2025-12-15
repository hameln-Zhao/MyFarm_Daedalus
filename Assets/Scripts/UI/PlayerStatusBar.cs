using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 订阅 EventHandler 的 HP/MP 变化事件，更新 UI 条（Image Filled）。
/// </summary>
public class PlayerStatsUI : MonoBehaviour
{
    [Header("HP/MP Fill Images (Type = Filled, Horizontal)")]
    [SerializeField] private Image _hpFillImage;
    [SerializeField] private Image _mpFillImage;

    private void OnEnable()
    {
        EventHandler.PlayerHealthChanged += OnPlayerHealthChanged;
        EventHandler.PlayerManaChanged += OnPlayerManaChanged;
    }

    private void OnDisable()
    {
        EventHandler.PlayerHealthChanged -= OnPlayerHealthChanged;
        EventHandler.PlayerManaChanged -= OnPlayerManaChanged;
    }

    private void OnPlayerHealthChanged(int current, int max)
    {
        if (_hpFillImage == null) return;
        _hpFillImage.fillAmount = (max <= 0) ? 0f : (float)current / max;
    }

    private void OnPlayerManaChanged(int current, int max)
    {
        if (_mpFillImage == null) return;
        _mpFillImage.fillAmount = (max <= 0) ? 0f : (float)current / max;
    }
}