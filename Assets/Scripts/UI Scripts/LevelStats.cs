using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LevelStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI enemiesAlive;
    [SerializeField] private TextMeshProUGUI lives;
    [SerializeField] private TextMeshProUGUI waveCountdown;

    private void Update()
    {
        coinsText.text = "$" + PlayerManager.Instance.Coins.ToString();
        enemiesAlive.text = "Enemies Left: " + PlayerManager.Instance.EnemiesAlive.ToString();
        lives.text = "Lives: " + PlayerManager.Instance.Lives.ToString();

        var waveCountdownFloat = WaveManager.Instance.currentWaveCountdown;
        if (waveCountdownFloat <= 0f)
        {
            waveCountdown.color = Color.red;
        }
        else waveCountdown.color = Color.white;

        waveCountdownFloat = (waveCountdownFloat < 0) ? 0 : waveCountdownFloat;
        waveCountdown.text = string.Format("{0:00}:{1:00}", Mathf.Floor(waveCountdownFloat / 60), waveCountdownFloat % 60);
    }
}
