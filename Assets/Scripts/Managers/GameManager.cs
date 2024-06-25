using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthController playerHP;

    [Header("Lists")]
    [SerializeField] private List<Enemy> enemies;
    public List<FlammableObject> flammables;

    [Header("Audio")]
    [SerializeField] private AudioSource winGameSoundEffect;
    [SerializeField] private AudioSource loseGameSoundEffect;

    public int flammablesTotal;

    public event Action onNewDeadTree = delegate { };

    public enum GameEndedReason { WIN = 0, PLAYER_KILLED, FOREST_KILLED }

    public event Action<GameEndedReason> onLevelEnded;

    private void OnEnable()
    {
        playerHP.onDead += LoseGame;

        Enemy.onSpawn += SpawnedEnemy;
        Enemy.onTrapped += KillCounter;

        FlammableObject.onSpawn += SpawnedFlammable;
        FlammableObject.onDeath += DeadNatureCounter;
    }

    private void OnDisable()
    {
        playerHP.onDead -= LoseGame;

        Enemy.onSpawn -= SpawnedEnemy;
        Enemy.onTrapped -= KillCounter;

        FlammableObject.onSpawn -= SpawnedFlammable;
        FlammableObject.onDeath -= DeadNatureCounter;
    }

    private void KillCounter(Enemy obj)
    {
        enemies.Remove(obj);

        if (enemies.Count == 0)
        {
            onLevelEnded?.Invoke(GameEndedReason.WIN);
            winGameSoundEffect.Play();
        }
    }

    private void DeadNatureCounter(FlammableObject obj)
    {
        flammables.Remove(obj);
        onNewDeadTree?.Invoke();

        if (flammables.Count == 0)
        {
            loseGameSoundEffect.Play();
            onLevelEnded?.Invoke(GameEndedReason.FOREST_KILLED);
        }
    }

    private void SpawnedEnemy(Enemy obj)
    {
        if (!enemies.Contains(obj))
        {
            enemies.Add(obj);
        }
    }

    private void SpawnedFlammable(FlammableObject obj)
    {
        flammables.Add(obj);
        flammablesTotal++;
    }

    private void LoseGame()
    {
        loseGameSoundEffect.Play();
        onLevelEnded?.Invoke(GameEndedReason.PLAYER_KILLED);
    }
}
