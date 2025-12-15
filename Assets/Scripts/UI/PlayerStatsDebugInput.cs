using UnityEngine;

public class PlayerStatsDebugInput : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;

    private void Awake()
    {
        if (_playerStats == null)
            _playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (_playerStats == null) return;

        if (Input.GetKeyDown(KeyCode.H)) _playerStats.TakeDamage(10);
        if (Input.GetKeyDown(KeyCode.J)) _playerStats.Heal(10);

        if (Input.GetKeyDown(KeyCode.M)) _playerStats.ConsumeMP(5);
        if (Input.GetKeyDown(KeyCode.N)) _playerStats.RecoverMP(5);

        if (Input.GetKeyDown(KeyCode.R)) _playerStats.ResetToConfig();
    }
}