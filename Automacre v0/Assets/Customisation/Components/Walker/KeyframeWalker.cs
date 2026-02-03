using UnityEngine;

public class KeyframeWalker : MonoBehaviour
{
    Animator animator;
    public AnimationClip WalkForwardclip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
        Invoke("EnableAnimator", Random.Range(0,1f));
    }

    // Update is called once per frame
    void Update()
    {
    }

   void  EnableAnimator()
    {
        animator.enabled = true;
    }
}
