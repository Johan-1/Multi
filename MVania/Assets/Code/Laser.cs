using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{

    [SerializeField] public Vector3 localDirection;
    [SerializeField] Transform _raycastPoint;

    [SerializeField] public bool constant;
    [SerializeField] public bool isActive;

    // have array of toogletimes to be able to create moore diverse patterns
    [SerializeField] public float[] toggleActiveTimes;

    [SerializeField] ParticleSystem[] _hitParticleSystems;
    [SerializeField] ParticleSystem[] _otherParticleSystems;
    
    
    LineRenderer _lineRenderer;
    
    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;

        // if laser is not constant start coroutine that will toogle on/off
        if (!constant)
            StartCoroutine(ToggleRay());
    }
   
    void Update()
    {     
        // if laser is active do raycast to get the endposition of line
        if (isActive)
           DoRayCast();                    
    }

    void DoRayCast()
    {
        //normalize direction 
        localDirection.Normalize();
        
        // convert raydirection from localspace to worldspace
        RaycastHit2D hit = Physics2D.Raycast(_raycastPoint.position, transform.TransformDirection(localDirection), 100.0f);
        if (hit)
        {
            // set the start/end position of linerenderer
            _lineRenderer.SetPosition(0, _raycastPoint.position);
            _lineRenderer.SetPosition(1, _raycastPoint.position + transform.TransformDirection(localDirection) * hit.distance);

            // loop over all hitParticles and set position to hitPoint
            for (int i = 0; i < _hitParticleSystems.Length; i++)
                _hitParticleSystems[i].transform.position = _lineRenderer.GetPosition(1);

            // deal damage if object have health
            if (hit.collider.gameObject.GetComponent<IDamageable>() != null)
                hit.collider.gameObject.GetComponent<IDamageable>().ModifyHealth(-1);                       
        }  
        
    }   

    IEnumerator ToggleRay()
    {
        float timer = 0.0f;
        int index = 0; // keep track of witch time we will use in array

        while (true)
        {
            timer += Time.deltaTime;

            if (timer >= toggleActiveTimes[index])
            {
                // toggle linerenderer and raycast
                isActive = !isActive;
                _lineRenderer.enabled = isActive;

                // if we just got active do a raycast right away to avoid line being rendered on old positions
                if (isActive)
                    DoRayCast();
                
                //loop over and toogle all particles
                for (int i = 0; i < _hitParticleSystems.Length; i++)                
                    ToggleParticle(_hitParticleSystems[i], isActive);

                for (int i = 0; i < _otherParticleSystems.Length; i++)
                    ToggleParticle(_otherParticleSystems[i], isActive);               

                // reset timer and move to next time in array
                timer = 0.0f;
                index++;

                // reset to 0 if outofbounds
                if (index == toggleActiveTimes.Length)
                    index = 0;
            }            
            yield return null;
        }

    }

    void ToggleParticle(ParticleSystem system, bool active)
    {
        if (!active)
        {            
            system.Stop(true, ParticleSystemStopBehavior.StopEmitting);           
        }
        else
        {            
            system.Play(true);            
        }

    }
}
