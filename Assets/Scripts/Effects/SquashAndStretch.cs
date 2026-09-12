using System;
using System.Collections;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
   [Header("Notes")]
   [SerializeField, Multiline(2)] private string notes;

   [Header("Squash and Stretch Core")]
   [SerializeField, Tooltip("The child sprite to animate.")] private Transform transformToAffect;
   [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.X;
   [SerializeField, Range(0.01f, 1f)] private float animationDuration = 0.25f;
   [SerializeField] private bool canBeOverwritten;
   [SerializeField] private bool playOnStart;

    [Flags]
    public enum SquashStretchAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }

    [Header("Animation Settings")]
    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float maximumScale = 1.3f;
    [SerializeField] private bool resetToInitialScaleAfterAnimation = true;

    [SerializeField] private AnimationCurve squashStretchCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 1f),
        new Keyframe(1f, 0f)
    );

    [Header("Looping Settings")]
    [SerializeField] private bool looping;
    [SerializeField, Min(0f)] private float loopingDelay = 0.5f;

    private Coroutine _squashAndStretchCoroutine;
    private WaitForSeconds _loopingDelayWaitForSeconds;
    private Vector3 _initialScaleVector;

    private bool affectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool affectY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool affectZ => (axisToAffect & SquashStretchAxis.Z) != 0;

    private void Awake()
    {
        if (transformToAffect == null)
        {
            SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
            if (sprite != null)
                transformToAffect = sprite.transform;
        }

        if (transformToAffect == null || transformToAffect == transform)
        {
            Debug.LogError("Assign the child Sprite as Transform To Affect.", this);
            enabled = false;
            return;
        }

        _initialScaleVector = transformToAffect.localScale;
        _loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);
    }

    private void Start()
    {
        if (playOnStart)
            CheckForAndStartCoroutine();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only react to impacts against a surface below the cheese.
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f &&
                collision.relativeVelocity.sqrMagnitude > 0.25f)
            {
                PlaySquashAndStretch();
                return;
            }
        }
    }

    private void OnDisable()
    {
        if (_squashAndStretchCoroutine != null)
            StopCoroutine(_squashAndStretchCoroutine);
        _squashAndStretchCoroutine = null;

        if (transformToAffect != null && transformToAffect != transform)
            transformToAffect.localScale = _initialScaleVector;
    }

    [ContextMenu("Play Squash and Stretch")]

    public void PlaySquashAndStretch()
    {
        if (!Application.isPlaying || !isActiveAndEnabled)
            return;

        CheckForAndStartCoroutine();
    }
    private void CheckForAndStartCoroutine()
    {
        if (axisToAffect == SquashStretchAxis.None)
        {
            Debug.Log("Axis to affect is set to None.", gameObject);
            return;
        }

        if (_squashAndStretchCoroutine != null)
        {
            if (!canBeOverwritten)
                return;

            StopCoroutine(_squashAndStretchCoroutine);
            if (resetToInitialScaleAfterAnimation)
                transformToAffect.localScale = _initialScaleVector;
        }

        _squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
    }

    private IEnumerator SquashAndStretchEffect()
    {
        do
        {
            float elapsedTime = 0;
            Vector3 originalScale = _initialScaleVector;
            Vector3 modifiedScale = originalScale;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;

                float curvePosition = Mathf.Clamp01(elapsedTime / Mathf.Max(animationDuration, 0.01f));
                float curveValue = squashStretchCurve.Evaluate(curvePosition);
                float remappedValue = initialScale + (curveValue * (maximumScale - initialScale));

                float minimumThreshold = 0.0001f;
                if (Mathf.Abs(remappedValue) < minimumThreshold)
                    remappedValue = minimumThreshold;

                if (affectX)
                    modifiedScale.x = originalScale.x * remappedValue;
                else
                    modifiedScale.x = originalScale.x / remappedValue;

                if (affectY)
                    modifiedScale.y = originalScale.y * remappedValue;
                else
                    modifiedScale.y = originalScale.y / remappedValue;

                if (affectZ)
                    modifiedScale.z = originalScale.z * remappedValue;
                else
                    modifiedScale.z = originalScale.z;

                transformToAffect.localScale = modifiedScale;

                yield return null;
            }

            if (resetToInitialScaleAfterAnimation)
                transformToAffect.localScale = originalScale;

            if (looping)
                yield return _loopingDelayWaitForSeconds;
        } while (looping);

        _squashAndStretchCoroutine = null;
    }

    public void SetLooping(bool shouldLoop)
    {
        looping = shouldLoop;
    }
}
