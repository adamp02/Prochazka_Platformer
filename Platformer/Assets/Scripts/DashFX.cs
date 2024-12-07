using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashFX : MonoBehaviour
{

    SpriteRenderer sprite;
    public float opacityRate = 1f;
    public float alpha = 1;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1f, 1f, 1f, 0.5f);
        StartCoroutine(DestroySelf());
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        sprite.color = new Color(1f, 1f, 1f, alpha);

        alpha -= opacityRate;
    }
}
