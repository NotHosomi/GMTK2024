using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreNumber : MonoBehaviour
{

    const float decay_start_time = 0.3f;
    const float fade_speed = 1.5f;
    const float rise_speed = 1f;

    public void init(int amount)
    {
        TextMeshPro tm = gameObject.GetComponent<TextMeshPro>();
        tm.text = "<b>+" + amount.ToString() + "</b>";
    }

    void Update()
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
