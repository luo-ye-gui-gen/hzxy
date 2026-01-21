using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;
    [SerializeField] private float sfxMinimunDistance;

    public bool playBGM;
    private int bgmIndex;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Invoke("AllowSFX", 1f);
    }

    void Update()
    {
        if (!playBGM)
        {
            StopAllBGM();
        }
        else
        {
            if (!bgm[bgmIndex].isPlaying)
            {
                PlayBGM(bgmIndex);
            }
        }
    }

    public void PlaySFX(int _sfxIndex,Transform _source)
    {
        // if (sfx[_sfxIndex].isPlaying) return;

        // if (_source != null && Vector2.Distance(GameManager.instance.playerHealth.transform.position, _source.position) > sfxMinimunDistance)
        //     return;

        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].Play();
        }
    }

    public void StopSFX(int index) => sfx[index].Stop();

    public void StopSFXWithTime(int _index) => StartCoroutine(DecreaseVolume(sfx[_index]));

    IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > .1f)
        {
            _audio.volume = _audio.volume * .2f;
            yield return new WaitForSeconds(.6f);

            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }

    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;
        StopAllBGM();

        bgm[bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
}
