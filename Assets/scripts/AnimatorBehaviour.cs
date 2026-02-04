using UnityEngine;

public class AnimationBehaviour : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    void Start()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }
    public void Move(Vector3 direction, bool isSprinting)
    {
        float speedValue = direction.magnitude;
        if (isSprinting)
        {
            speedValue *= 2f;
        }
        _animator.SetFloat("speed", speedValue, 0.1f, Time.deltaTime);
    }
    
    public void Dance()
    {
        if (_animator.GetBool("Dance"))
        {
            _animator.SetBool("Dance", false);
            
        }
        else {
            _animator.SetBool("Dance", true);
            
        }
        
    }

    public void Aim()
    {
        
        if (_animator.GetBool("aiming"))
        {
            _animator.SetBool("aiming", false);
            
            _animator.SetLayerWeight(1, 0f);

        }
        else
        {
            _animator.SetBool("aiming", true);
            _animator.SetLayerWeight(1, 1f);
        }
    }

    public void Jump(bool change)
    {
        _animator.SetBool("isJumping", change);


    }
}
