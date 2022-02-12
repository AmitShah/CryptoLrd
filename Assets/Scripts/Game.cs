using UnityEngine;
using System.Runtime.InteropServices;
using DG.Tweening;

public class Game : MonoBehaviour
{
    private float effectDuration = 0.1f;
    private float shakeStrength = 1f;
    private int shakeVibrato = 0;
    private float shakeRandomness = 0.5f;

    Tweener shake;
    public void Awake()
    {
        this.shake  = this.transform.DOShakePosition(effectDuration, new Vector3(shakeStrength,shakeStrength,0), shakeVibrato, shakeRandomness);
        this.shake.SetAutoKill(false); 
    }

    public void DoShake() {	    
        this.transform.DOShakePosition(effectDuration, new Vector3(shakeStrength,shakeStrength,0), shakeVibrato, shakeRandomness);
        //this.shake.Play();
    }

    public int Counter = 0;
     // Update is called once per frame
     protected float Timer;
      public int GoldValue = 15;
     public int Power = 5;
     public int DelayAmount = 1; // Second count


    void Update()
    {
    	if (Input.GetMouseButtonDown(0))
		{
			   Counter++;
               OnHitCallback();
               DoShake();
                
		}

        Timer += Time.deltaTime;
 
         if (Timer >= DelayAmount)
         {
             Timer = 0f;
             Counter+=Power;
         }
         //update text
        this.GetComponentInChildren<TMPro.TextMeshPro>().text = Counter.ToString().Replace("0","O");
    }


    [DllImport("__Internal")]
    private static extern void OnHit(string position);

    public void OnHitCallback()
    {
        var position = JsonUtility.ToJson(Input.mousePosition);

#if UNITY_WEBGL == true && UNITY_EDITOR == false
    OnHit(position);
#endif
      
    }
}