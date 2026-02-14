namespace EasyDoorSystem
{
    using UnityEngine;
    using UnityEngine.Events;

    public class Puerta : MonoBehaviour
    {
        public enum MovementType { Rotation, Position, Both }

        [Header("Door Settings")]
        [SerializeField] private MovementType movementType = MovementType.Rotation;
        [SerializeField] private float movementSpeed = 2f;
        [SerializeField] private float rotationSpeed = 2f;
        [Tooltip("Automatically close after specified time (0 = no auto-close)")]
        [SerializeField] private float autoCloseDelay = 0f;
        [SerializeField] private float detectionRange = 3f;

        [Header("Transform Targets")]
        [SerializeField] private Vector3 closedRotation = new Vector3(0, -179.573f, 000.9f);
        [SerializeField] private Vector3 openedRotation;
        [SerializeField] private Vector3 closedPosition = new Vector3(0.405f, -0.0179081f, 0.009f);
        [SerializeField] private Vector3 openedPosition;

        [Header("Events")]
        public UnityEvent OnDoorOpening;
        public UnityEvent OnDoorClosed;


        public bool IsOpen { get; private set; }
        public bool IsMoving { get; private set; }

        private Transform playerTransform;
        private Coroutine movementCoroutine;
        [SerializeField] private bool _playerHasKey = false;

        public Player Player;
        private void Awake()
        {
            
            if (!Player) Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        }

        private void Update()
        {
            _playerHasKey = Player.hasKey;
            if ( !playerTransform) return;

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= detectionRange && !IsOpen && _playerHasKey)
            {
                OpenDoor();
            }
            else if (distance > detectionRange && IsOpen)
            {
                CloseDoor();
            }
        }

        public void OpenDoor()
        {
            if (IsOpen || IsMoving || !_playerHasKey) return;

            MoveDoor(openedPosition, openedRotation, true);
            OnDoorOpening.Invoke();
        }

        public void CloseDoor()
        {
            if (!IsOpen || IsMoving) return;

            MoveDoor(closedPosition, closedRotation, false);
            OnDoorClosed.Invoke();
        }

        private void MoveDoor(Vector3 targetPosition, Vector3 targetRotation, bool opening)
        {
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);

            movementCoroutine = StartCoroutine(AnimateDoor(
                movementType != MovementType.Rotation ? targetPosition : transform.localPosition,
                movementType != MovementType.Position ? targetRotation : transform.localEulerAngles,
                opening
            ));
        }

        private System.Collections.IEnumerator AnimateDoor(Vector3 targetPos, Vector3 targetRot, bool opening)
        {
            IsMoving = true;
            Quaternion startRot = transform.localRotation;
            Vector3 startPos = transform.localPosition;
            Quaternion targetQuaternion = Quaternion.Euler(targetRot);

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * Mathf.Max(movementSpeed, rotationSpeed);

                if (movementType != MovementType.Position)
                {
                    transform.localRotation = Quaternion.Slerp(startRot, targetQuaternion, progress * rotationSpeed);
                }

                if (movementType != MovementType.Rotation)
                {
                    transform.localPosition = Vector3.Lerp(startPos, targetPos, progress * movementSpeed);
                }

                yield return null;
            }

            // Ensure final positions are exact
            if (movementType != MovementType.Position)
                transform.localRotation = targetQuaternion;

            if (movementType != MovementType.Rotation)
                transform.localPosition = targetPos;

            IsOpen = opening;
            IsMoving = false;

            if (autoCloseDelay > 0 && IsOpen)
                Invoke(nameof(CloseDoor), autoCloseDelay);
        }

        


    }
}