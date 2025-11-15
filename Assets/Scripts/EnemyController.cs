using UnityEngine;

namespace Lizeria02
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.5f;

        private Transform target;
        private GameController gameController;

        public void Initialize(Transform targetTransform, GameController controller, float speed)
        {
            target = targetTransform;
            gameController = controller;
            moveSpeed = speed;
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * (moveSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                gameController?.OnPlayerHit();
            }
        }

        private void OnDestroy()
        {
            if (gameController != null)
            {
                gameController.NotifyEnemyDestroyed(this);
            }
        }
    }
}

