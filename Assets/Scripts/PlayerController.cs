using UnityEngine;

namespace Lizeria02
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float smoothing = 12f;

        private Rigidbody2D body;
        private Vector2 desiredVelocity;
        private GameController gameController;

        private void Awake()
        {
            gameObject.tag = "Player";

            if (!TryGetComponent(out body))
            {
                body = gameObject.AddComponent<Rigidbody2D>();
            }

            body.gravityScale = 0f;
            body.drag = 8f;
            body.angularDrag = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (!TryGetComponent(out Collider2D collider))
            {
                collider = gameObject.AddComponent<CircleCollider2D>();
            }
            collider.isTrigger = false;

            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            spriteRenderer.sprite = Resources.GetBuiltinResource<Sprite>("Sprites/Square.psd");
            spriteRenderer.color = new Color(0.2f, 0.6f, 1f);
            spriteRenderer.sortingOrder = 2;
        }

        private void Start()
        {
            gameController = FindObjectOfType<GameController>();
        }

        private void Update()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            input = Vector2.ClampMagnitude(input, 1f);
            desiredVelocity = input * moveSpeed;
        }

        private void FixedUpdate()
        {
            var newVelocity = Vector2.Lerp(body.velocity, desiredVelocity, smoothing * Time.fixedDeltaTime);
            body.velocity = newVelocity;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<EnemyController>() != null)
            {
                gameController?.OnPlayerHit();
            }
        }

        public void ResetPlayer(Vector3 position)
        {
            transform.position = position;
            body.velocity = Vector2.zero;
        }
    }
}

