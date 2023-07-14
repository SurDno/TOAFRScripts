using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public string _name;
    public enum role
    {
        friend, enemy, neutral
    }
    public role Role; 
    private string color;
    public Text dialog;
    public GameObject dialogPanel;
    public string[] message;
    private byte curMessage = 0, l;
    // Start is called before the first frame update
    void Start()
    {
        l = (byte)(message.Length - 1);
        switch(Role)
        {
            case role.friend:  color = "#7FFF00"; break;
            case role.enemy:   color = "red"; break;
            case role.neutral: color = "yellow"; break;
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    void OnTriggerEnter(Collider col)
    {
        if(col.tag=="Player")
        {
            dialogPanel.SetActive(true);
            StartCoroutine(timer());
        }
    }

    IEnumerator timer()
    {
        float t = 3f;
        dialog.text = "<color="+color+">"+_name + ":</color> " + message[curMessage];
		while(t>0f||curMessage!=l)
		{
            
			t -= Time.deltaTime;
            if(t<0&&curMessage!=l)
            {
                t = 3f;
                curMessage++;
                dialog.text = "<color="+color+">"+_name + ":</color> " + message[curMessage];
            }
			yield return null;
		}
        dialog.text = "";
        dialogPanel.SetActive(false);
    }

}
