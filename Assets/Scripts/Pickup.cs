using UnityEngine;

namespace Lizeria02
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 90f;

        private GameController gameController;

        public void Initialize(GameController controller)
        {
            gameController = controller;
            SetupVisuals();
        }

        private void Awake()
        {
            SetupVisuals();
            if (gameController == null)
            {
                gameController = FindObjectOfType<GameController>();
            }
        }

        private void Update()
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                gameController?.OnPickupCollected(this);
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            gameController?.NotifyPickupDestroyed(this);
        }

        private void SetupVisuals()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = Resources.GetBuiltinResource<Sprite>("Sprites/Square.psd");
            }

            spriteRenderer.sortingOrder = 1;
        }
    }
}

