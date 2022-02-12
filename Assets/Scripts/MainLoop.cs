using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using DG.Tweening;

public class MainLoop : MonoBehaviour
{
    public Dictionary<string, System.Numerics.BigInteger> clickState;
    public string playerKey;
    public System.Numerics.BigInteger stateCounter;
    public System.Numerics.BigInteger bufferCounter;
    public GameObject counter;
    System.Numerics.BigInteger hitpoints;
    System.Numerics.BigInteger attack;
    public GameObject devil;
    public Material devilMat;
    public GameObject hitObj;
    

    public void Awake()
    {
        hitpoints = 1_000_000;
        updateHitPoints();
        attack = 1;

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.GetRayIntersection(ray, 100f);
            if (hit)
            {
                var pixel = sampleTextureColorAt(hit);
                //we only count a hit if you did so on a color pixel
                if (pixel.a > 0)
                {
                    bufferCounter += attack;
                    OnHitCallback();
                    updateHitPoints();
                    //StartCoroutine(shaker.GetComponent<Shaker>().Play());
                    var shaker = devil.GetComponent<Shaker>();
                    if (!shaker.running)
                    {
                        StartCoroutine(shaker.SlashRoutine(1f));
                    }
                    
                    hitObj.GetComponent<Animator>().Play("OnHitAnimation",0,0f);
                    hitObj.transform.position = new Vector3(hit.point.x, hit.point.y, hitObj.transform.position.z);
                    //hitObj.GetComponent<Animator>().SetTrigger("OnHitAnimation");

                }
                //Debug.Log("Hit Counter:"+count.ToString()+ pixel.ToString("F4"));
            }
        }

    }

    public void UpdateClickState(string json) {
        Debug.Log($"received: {json}");
        #if UNITY_WEBGL == true && UNITY_EDITOR == false

        var state = JsonUtility.FromJson<Dictionary<string, System.Numerics.BigInteger>>(json);
        var sum = new System.Numerics.BigInteger(0);
        if (state.TryGetValue(playerKey, out sum)) {
            state.Remove(playerKey);
        }
        sum = System.Numerics.BigInteger.Max(sum, attack);
        foreach(var e in state) {
            sum += e.Value;
        }
        stateCounter = sum;
        #endif
    }

    public void setPlayerKey(string starkkey) {

    }

    void updateHitPoints()
    {
       var output = hitpoints - (stateCounter + bufferCounter);
       counter.GetComponent<TMPro.TextMeshPro>().text = output.ToString().Replace("0", "O");
    }

    Color sampleTextureColorAt(RaycastHit2D hit)
    {
        //we dont need Input.mousePosition, we can get it from the hit position
        var sprite = hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite;
        Vector3 worldPos = hit.point;// Camera.main.ScreenToWorldPoint(mouseCoord);
        Vector2 coords = TextureSpaceCoord(sprite, worldPos);
        //Vector2 uvs = TextureSpaceUV(sprite, worldPos);

        Color pixel = sprite.texture.GetPixel((int)coords.x, (int)coords.y);
        return pixel;
    }

    // private void Start() {
    //     sprite = GetComponent<SpriteRenderer>().sprite;
    // }

    public Vector2 TextureSpaceCoord(Sprite sprite, Vector3 worldPos)
    {
        float ppu = sprite.pixelsPerUnit;

        // Local position on the sprite in pixels.
        Vector2 localPos = transform.InverseTransformPoint(worldPos) * ppu;

        // When the sprite is part of an atlas, the rect defines its offset on the texture.
        // When the sprite is not part of an atlas, the rect is the same as the texture (x = 0, y = 0, width = tex.width, ...)
        var texSpacePivot = new Vector2(sprite.rect.x, sprite.rect.y) + sprite.pivot;
        Vector2 texSpaceCoord = texSpacePivot + localPos;

        return texSpaceCoord;
    }

    public Vector2 TextureSpaceUV(Sprite sprite, Vector3 worldPos)
    {
        Texture2D tex = sprite.texture;
        Vector2 texSpaceCoord = TextureSpaceCoord(sprite, worldPos);

        // Pixels to UV(0-1) conversion.
        Vector2 uvs = texSpaceCoord;
        uvs.x /= tex.width;
        uvs.y /= tex.height;


        return uvs;
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
