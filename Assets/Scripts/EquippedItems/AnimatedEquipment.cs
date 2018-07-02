using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedEquipment : MonoBehaviour
{
    public Animation UseClip;

    private Animator _animator;

    // Use this for initialization
    void Start ()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void AnimateOnce()
    {
        _animator.SetTrigger("Use");
    }

    public void FaceDirection(Direction direction)
    {
        if (direction == Direction.Right)
        {
            this.transform.localEulerAngles = new Vector3(0.0f, 0.0f);
        }
        else if (direction == Direction.Left)
        {
            this.transform.localEulerAngles = new Vector3(0.0f, 180.0f);
        }
    }
}
