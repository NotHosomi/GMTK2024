using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreNumber : MonoBehaviour
{

    const float decay_start_time = 0.3f;
    const float fade_speed = 1.5f;
    const float rise_speed = 1f;
    float age = 0;

    private void Start()
    {
        //init(132, AttackEvents.BLOCK);
        //rise_dest = gameObject.transform.position + new Vector3(0, rise_offset, 0);
    }

    public void init(int amount)
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);

        TextMeshPro tm = gameObject.GetComponent<TextMeshPro>();
        tm.text = "<b>+" + amount.ToString() + "</b>";
        //Destroy(gameObject, 4);
    }

    // Update is called once per frame
    void Update()
    {
        if (age > decay_start_time)
        {

            Color32 col_main = fade(gameObject.GetComponent<TextMeshPro>().faceColor);
            Color32 col_outline = fade(gameObject.GetComponent<TextMeshPro>().outlineColor);
            if (col_main.a == 0)
            {
                // Fade has finished
                Destroy(gameObject);
                return;
            }
            gameObject.GetComponent<TextMeshPro>().faceColor = col_main;
            gameObject.GetComponent<TextMeshPro>().outlineColor = col_outline;

            // gameObject.GetComponent<TextMeshPro>().alpha += 255 * fade_out_speed * Time.deltaTime;
            gameObject.transform.position += new Vector3(0, rise_speed, 0) * Time.deltaTime;
        }
        age += Time.deltaTime;
    }

    private Color32 fade(Color32 col)
    {
        float alpha = col.a;
        alpha -= 255 * fade_speed * Time.deltaTime;
        if (alpha > 255)
            alpha = 255;
        else if (alpha < 0)
            alpha = 0;
        col.a = (byte)alpha;
        return col;
    }
}
