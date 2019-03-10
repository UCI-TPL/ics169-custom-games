using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherMan_Effects : MonoBehaviour {
    public GameObject _target_object;
    public ParticleSystem _system;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(_target_object != null)
        {
            this.gameObject.transform.LookAt(_target_object.transform);
        }
	}

    public void move_target_to(GameObject _target)
    {
        _target_object.transform.position = _target.transform.position;
        Debug.Log("-----------  Position Moved");
        Debug.Log(_target.transform.position);
    }

    public void play_effect()
    {
        _system.Play();
    }

    //private void LateUpdate()
    //{
    //    float speed = 25f;
    //    Vector3 _heading = this.gameObject.transform.position - _target_object.transform.position;
    //    float distance = _heading.magnitude;
    //    Vector3 _normal_heading = _heading / distance;

    //    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_system.particleCount];
    //    int count = _system.GetParticles(particles);

    //    for (int i = 0; i < count; i++)
    //    {
    //        //float yVel = (particles[i].remainingLifetime / particles[i].startLifetime) * distance;
    //        particles[i].velocity = new Vector3(_normal_heading.x * speed, _normal_heading.y * speed, _normal_heading.z * speed);
    //    }

    //    _system.SetParticles(particles, count);
    //}
}
