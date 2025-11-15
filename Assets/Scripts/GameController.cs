using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lizeria02
{
    public class GameController : MonoBehaviour
    {
        [Header("Gameplay Settings")]
        [SerializeField] private float playAreaExtent = 8f;
        [SerializeField] private float timeLimitSeconds = 120f;
        [SerializeField] private int startingPickupCount = 4;
        [SerializeField] private float pickupRespawnDelay = 1.5f;
        [SerializeField] private float enemySpawnInterval = 6f;
        [SerializeField] private float enemySpeed = 2.6f;
        [SerializeField] private int targetScore = 10;

        [Header("Visual Settings")]
        [SerializeField] private Color pickupColor = new Color(1f, 0.92f, 0.2f);
        [SerializeField] private Color enemyColor = new Color(0.95f, 0.25f, 0.3f);

        private readonly List<Pickup> activePickups = new List<Pickup>();
        private readonly List<EnemyController> activeEnemies = new List<EnemyController>();

        private PlayerController player;
        private Text scoreText;
        private Text timerText;
        private Text messageText;

        private float timeRemaining;
        private float nextPickupTime;
        private float nextEnemySpawn;
        private int score;
        private bool isGameActive;

        private void Awake()
        {
            Physics2D.gravity = Vector2.zero;
        }

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError("PlayerController not found in scene.");
            }

            scoreText = FindText("ScoreText");
            timerText = FindText("TimerText");
            messageText = FindText("MessageText");

            StartNewGame();
        }

        private void Update()
        {
            if (!isGameActive)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    StartNewGame();
                }
                return;
            }

            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                EndGame(false, "Time's up!\nPress R to try again.");
            }

            if (isGameActive && Time.time >= nextPickupTime && activePickups.Count < startingPickupCount)
            {
                SpawnPickup();
                nextPickupTime = Time.time + pickupRespawnDelay;
            }

            if (isGameActive && Time.time >= nextEnemySpawn)
            {
                SpawnEnemy();
                nextEnemySpawn = Time.time + enemySpawnInterval;
            }

            UpdateUI();
        }

        private Text FindText(string name)
        {
            var go = GameObject.Find(name);
            return go != null ? go.GetComponent<Text>() : null;
        }

        private void StartNewGame()
        {
            ClearDynamicObjects();

            score = 0;
            timeRemaining = timeLimitSeconds;
            isGameActive = true;
            nextPickupTime = Time.time;
            nextEnemySpawn = Time.time + enemySpawnInterval;

            if (player != null)
            {
                player.ResetPlayer(Vector3.zero);
            }

            for (int i = 0; i < startingPickupCount; i++)
            {
                SpawnPickup();
            }

            if (messageText != null)
            {
                messageText.text = "Collect 10 crystals before time runs out!\nAvoid the red drones.\nPress R after finishing to restart.";
            }

            UpdateUI();
        }

        private void ClearDynamicObjects()
        {
            foreach (var pickup in activePickups)
            {
                if (pickup != null)
                {
                    Destroy(pickup.gameObject);
                }
            }
            activePickups.Clear();

            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            activeEnemies.Clear();
        }

        private void UpdateUI()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }

            if (timerText != null)
            {
                timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";
            }
        }

        public void OnPickupCollected(Pickup pickup)
        {
            if (!isGameActive)
            {
                return;
            }

            score++;
            activePickups.Remove(pickup);
            UpdateUI();

            if (score >= targetScore)
            {
                EndGame(true, "You collected all the crystals!\nPress R to play again.");
            }
        }

        public void NotifyPickupDestroyed(Pickup pickup)
        {
            activePickups.Remove(pickup);
        }

        public void NotifyEnemyDestroyed(EnemyController enemy)
        {
            activeEnemies.Remove(enemy);
        }

        public void OnPlayerHit()
        {
            if (!isGameActive)
            {
                return;
            }

            EndGame(false, "You were caught!\nPress R to try again.");
        }

        private void EndGame(bool playerWon, string message)
        {
            isGameActive = false;
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        private void SpawnPickup()
        {
            Vector2 spawnPos = Random.insideUnitCircle * (playAreaExtent - 1f);
            var pickupGo = new GameObject("Pickup");
            pickupGo.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0f);

            var spriteRenderer = pickupGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.GetBuiltinResource<Sprite>("Sprites/Square.psd");
            spriteRenderer.color = pickupColor;
            spriteRenderer.sortingOrder = 1;
            pickupGo.transform.localScale = Vector3.one * 0.7f;

            var collider = pickupGo.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            var pickup = pickupGo.AddComponent<Pickup>();
            pickup.Initialize(this);
            activePickups.Add(pickup);
        }

        private void SpawnEnemy()
        {
            if (player == null)
            {
                return;
            }

            Vector2 spawnDirection = Random.insideUnitCircle.normalized;
            Vector2 spawnPos = spawnDirection * playAreaExtent;
            var enemyGo = new GameObject("Enemy");
            enemyGo.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0f);

            var spriteRenderer = enemyGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.GetBuiltinResource<Sprite>("Sprites/Square.psd");
            spriteRenderer.color = enemyColor;
            spriteRenderer.sortingOrder = 0;
            enemyGo.transform.localScale = Vector3.one * 0.9f;

            var collider = enemyGo.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            var controller = enemyGo.AddComponent<EnemyController>();
            controller.Initialize(player.transform, this, enemySpeed);
            activeEnemies.Add(controller);
        }
    }
}

