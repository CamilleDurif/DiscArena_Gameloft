using System.Collections;
using UnityEngine;

public class Puck : MonoBehaviour
{
    [SerializeField] private ParticleSystem groundParticles;
    [SerializeField] private ParticleSystem collisionParticles; 
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidbody;

    [HideInInspector] public bool isGhost;

    public float damage = 2;

    private bool checkLastPuck = false;

    private void Start()
    {
        checkLastPuck = false;
    }

    private void Update()
    {
        if (isGhost)
        {
            return;
        }
        
        if (rigidbody.velocity.magnitude <= 1)
        {
            groundParticles.Stop();
        }
        else
        {
            if (!groundParticles.isPlaying)
            {
                groundParticles.Play();
            }
        }

        if (rigidbody.velocity.magnitude <= 0.1f && checkLastPuck)
        {
            GameManager.Instance.EndScreen(false);
        }
    }

    public void AddImpulse(Vector3 force, bool isLastPuck = false)
    {
        rigidbody.AddForce(force, ForceMode.Impulse);
        if (isLastPuck)
        {
            StartCoroutine(LastPuckCoroutine());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGhost || collision.gameObject.CompareTag("Ground"))
        {
            return;
        }
        
        collisionParticles.Play();
        
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.OnhittingObstacle(collision.gameObject.GetComponentInParent<Obstacle>());
        }
    }

    public void TriggerBumpAnimation()
    {
        animator.SetTrigger("Bump");
    }

    private IEnumerator LastPuckCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        checkLastPuck = true;

    }
}
