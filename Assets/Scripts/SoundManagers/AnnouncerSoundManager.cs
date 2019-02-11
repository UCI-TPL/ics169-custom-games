using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerSoundManager : MonoBehaviour {
	//AUDIO SOURCES
	public AudioSource regularKill;
	public AudioSource multiKills;
	public AudioSource critical_hits;
	public AudioSource misses; 

	//REGULAR KILLS (6)
	public AudioClip murder;
	public AudioClip homicide;
	public AudioClip bloodshed;
	public AudioClip destruction;
	public AudioClip manslaughter;
	public AudioClip massacre;

	// MULTI-KILLS (6)
	public AudioClip double_kill;
	public AudioClip triple_kill;
	public AudioClip quadra_kill;
	public AudioClip penta_kill;
	public AudioClip hexa_kill;
	public AudioClip hepta_kill;

	//CRITICAL HITS (4)
	public AudioClip crit;
	public AudioClip critical;
	public AudioClip exceptional_hit;
	public AudioClip critical_hit;

	//MISSES (2)
	public AudioClip try_aiming_next_time;
	public AudioClip you_suck;

	private AudioClip[] regular;
	private AudioClip[] multi;
	private AudioClip[] criticals;
	private AudioClip[] miss;
	// Use this for initialization
	void Start () 
	{
		regular =  new AudioClip[]{murder, homicide, bloodshed, destruction,
								   manslaughter, massacre};

		multi = new AudioClip[]{double_kill, triple_kill, quadra_kill, 
								penta_kill, hexa_kill, hepta_kill};

		criticals = new AudioClip[]{crit, critical, exceptional_hit, 
									critical_hit};

		miss = new AudioClip[]{try_aiming_next_time,you_suck};
	}

	public void randKillLine()
	{
		regularKill.clip = (regular[Random.Range(0,regular.Length)]);
		regularKill.Play();
	}
	// //assumes at least 2 kills
	// //otheriwse it should be a regular kill
	public void multiKill(int numKills)
	{
		if(numKills >= 2)
		{
			multiKills.clip = (multi[numKills-2]);
			multiKills.Play();
		}
		else
		{
			randKillLine();
		}
		
	}
	public void randCritLine()
	{
		critical_hits.clip = (criticals[Random.Range(0,criticals.Length)]);
		critical_hits.Play();
	}

	public void randMissLine()
	{
		misses.clip = miss[Random.Range(0, miss.Length)];
		misses.Play();
	}
	// Update is called once per frame
	void Update () 
	{
		
	}
}
