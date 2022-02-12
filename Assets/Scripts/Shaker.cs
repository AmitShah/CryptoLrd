using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Shaker : MonoBehaviour
{
    public bool running = false;
    public Vector3 pos;
    public static class ShakeAnimProperties
    {
        static public float Duration { get { return .1f; } }
        static public Vector3 Strength { get { return new Vector3(1f, 1f, 0); } }
        static public int Vibrato { get { return 100; } }
        static public float Randomness { get { return 10f; } }
        static public bool FadeOut { get { return false; } }
    }


    void Start()
    {
        var T = this.transform;
        Tweener shakey = DOTween.Shake(() => T.position, x => T.position = x, ShakeAnimProperties.Duration, ShakeAnimProperties.Strength, ShakeAnimProperties.Vibrato, ShakeAnimProperties.Randomness, ShakeAnimProperties.FadeOut);
        shakey.SetId<Tweener>("ShakeID");
        shakey.SetAutoKill(false);
        pos = T.transform.position;
        //shake = shakey;
    }

    public IEnumerator SlashRoutine(float time)

    {
        running = true;
        Tweener t = transform.DOShakePosition(ShakeAnimProperties.Duration, ShakeAnimProperties.Strength, ShakeAnimProperties.Vibrato, ShakeAnimProperties.Randomness, ShakeAnimProperties.FadeOut);
        t.onComplete=()=> {
            transform.position = pos;
            running = false;
        };
        yield return t.WaitForCompletion();
     
        //if (shake.IsPlaying()) {
        //    Debug.Log("animation already runnng");
        //    yield return 0;
        //}
        //var ogPosition = transform.position;
        //Debug.Log("playing shake aimation");
        //shake.onComplete = () =>
        //{
        //    Debug.Log("resetting position");
        //    transform.position = ogPosition;            
        //    shake.Rewind();
        //};
        //yield return shake.Play().WaitForCompletion();
    }
}
