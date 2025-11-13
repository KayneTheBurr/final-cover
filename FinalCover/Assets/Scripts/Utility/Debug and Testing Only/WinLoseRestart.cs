using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoseRestart : MonoBehaviour
{
    public GameObject winPanel, losePanel;
    private EnemyCharacterManager enemy;
    private PlayerManager player;
    private bool _done;

    
    private void Start()
    {
        player = FindFirstObjectByType<PlayerManager>();
        enemy = FindFirstObjectByType<EnemyCharacterManager>();
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        _done = false;
    }
    private void Update()
    {
        if(_done) return;
        if (!player || !enemy) return;

        if (player.isDead)
        {
            losePanel.SetActive(true);
            _done = true;
        }
        else if (enemy.isDead)
        {
            winPanel.SetActive(true);
            _done = true;
        }
    }
    public void HardRestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
