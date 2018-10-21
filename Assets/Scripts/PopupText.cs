using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour {
    public Animator animator;
    private Text damagetext;

	// Use this for initialization
	void OnEnable() {
        AnimatorClipInfo[] clipinfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipinfo[0].clip.length);
        damagetext = animator.GetComponent<Text>();
	}
	
    public void SetText(string text)
    {
        damagetext.text = text;
    }
}
