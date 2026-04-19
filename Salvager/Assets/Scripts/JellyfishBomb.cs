using System.Collections;
using UnityEngine;

public class JellyfishBomb : Jellyfish
{
    public GameObject hitEffect;
    public float hitDelay = 0.05f;

    public GameObject[] disableOnHit;

    protected override void OnFirstHit()
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        foreach (GameObject obj in disableOnHit)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            Magnet magnet = playerObj.GetComponent<Magnet>();

            if (magnet != null)
            {
                bool hasUpgrade2 = PlayerPrefs.GetInt("PlayerUpgrade2", 0) == 1;

                int totalHits = hasUpgrade2 ? 5 : 3;

                StartCoroutine(DoHits(magnet, totalHits));
            }
        }
    }

    IEnumerator DoHits(Magnet magnet, int totalHits)
    {
        for (int i = 0; i < totalHits; i++)
        {
            if (magnet.isDead) break;

            yield return new WaitForSecondsRealtime(hitDelay);

            magnet.Hit();
        }
    }
}