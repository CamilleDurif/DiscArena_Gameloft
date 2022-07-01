using UnityEngine;

public class Obstacle : WorldElement
{
    public ParticleSystem particles;
    
    public virtual void HittingObstacle()
    {
        particles.Play();
    }
}
