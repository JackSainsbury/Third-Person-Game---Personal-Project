using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Library : MonoBehaviour {

    /// <summary>
    /// 
    /// Fade a particle system which consists of trails, over seconds.
    /// 
    /// </summary>
    /// <param name="m_targetTrails"></param>
    /// <param name="time"></param>
    /// <param name="instance"></param>
	public static void P_TrailFadeAfterEXT(ParticleSystem.TrailModule m_targetTrails, float time, MonoBehaviour instance){
        instance.StartCoroutine(P_TrailFadeAfter(m_targetTrails, time));
	}

	static IEnumerator P_TrailFadeAfter(ParticleSystem.TrailModule m_targetTrails, float time){
        float p_trailTimer = 0;

        while (p_trailTimer < time) {
            // Increase timer
            p_trailTimer += Time.deltaTime;

            // Base subtractive value to slide the alpha
            float alphaBase = p_trailTimer / time;

            // Set colour for trail
            m_targetTrails.colorOverTrail = new ParticleSystem.MinMaxGradient (
                new Color (1, 1, 1, 1 - alphaBase), 
                new Color (1, 1, 1, 1 - (alphaBase * 2))
            );

            yield return null;
        }
        // Charge has finished

        yield return null;
	}

    /// <summary>
    /// 
    /// Fade a sprite opacity in and out over time
    /// 
    /// </summary>
    /// <param name="m_targetSprite"></param>
    /// <param name="time"></param>
    /// <param name="instance"></param>
    public static void P_SpriteFadeInOutEXT(SpriteRenderer m_targetSprite, float value, float time, MonoBehaviour instance)
    {
        instance.StartCoroutine(P_SpriteFadeInOut(m_targetSprite, value, time));
    }

    static IEnumerator P_SpriteFadeInOut(SpriteRenderer m_targetSprite, float value, float time)
    {
        float m_fadeTimer = 0;

        while (m_fadeTimer < time)
        {
            // Determine the opacity value to set
            float opacity = m_fadeTimer / time;
            opacity = 1 - Mathf.Abs((opacity * 2) - 1);

            // Only mod the opacity, not the base colour
            Color baseCol = m_targetSprite.color;
            m_targetSprite.color = new Color(baseCol.r, baseCol.b, baseCol.g, opacity * value);

            m_fadeTimer += Time.deltaTime;
            yield return null;
        }

        // Kill opacity
        m_targetSprite.color = new Color(m_targetSprite.color.r, m_targetSprite.color.b, m_targetSprite.color.g, 0);

        // Disable the sprite renderer
        m_targetSprite.enabled = false;

        yield return null;
    }

    /// <summary>
    /// 
    /// Fade a sprite opacity in and out over time
    /// 
    /// </summary>
    /// <param name="m_targetSprite"></param>
    /// <param name="time"></param>
    /// <param name="instance"></param>
    public static void P_ScaleObjectInOutEXT(GameObject m_targetObject, float scaleFrom, float scaleTo, float time, bool inOut, MonoBehaviour instance)
    {
        instance.StartCoroutine(P_ScaleObjectInOut(m_targetObject, scaleFrom, scaleTo, time, inOut));
    }

    static IEnumerator P_ScaleObjectInOut(GameObject m_targetObject, float scaleFrom, float scaleTo, float time, bool inOut)
    {
        float m_fadeTimer = 0;

        float diference = scaleTo - scaleFrom;

        while (m_fadeTimer < time)
        {
            // Determine the opacity value to set
            float opacity = m_fadeTimer / time;

            // Do InOut or single lerp
            if (inOut)
            {
                opacity = 1 - Mathf.Abs((opacity * 2) - 1);
            }

            float value = scaleFrom + (opacity * diference);

            // Scale the object
            m_targetObject.transform.localScale = new Vector3(value, value, value);

            m_fadeTimer += Time.deltaTime;
            yield return null;
        }

        m_targetObject.transform.localScale = new Vector3(0, 0, 0);

        yield return null;
    }
}
