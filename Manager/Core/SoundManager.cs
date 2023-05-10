using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];

    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        // @Sound 객체 찾기
        GameObject root = GameObject.Find("@Sound");

        // 객체가 없으면 생성
        if (root == null){
            root = new GameObject{name = "@Sound"};
            Object.DontDestroyOnLoad(root);

            // System.Enum.GetNames : Enum 안에 변수이름을 다 읽어오기
            string[] soundName = System.Enum.GetNames(typeof(Define.Sound));

            // (soundName.Length - 1) 이유 : Define.Sound의 마지막 값은 MaxCount이기 때문. (MaxCount : 전체 개수 확인용)
            for(int i = 0; i < soundName.Length-1; i++){
                // AudioSource 컴포넌트를 가진 객체 생성 후 @Sound 객체를 부모로 설정.
                GameObject go = new GameObject{name = soundName[i]};
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            // Bgm은 Loop = True 설정
            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    // Audio 재생 : Clip 경로로 받을 때
    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    // Audio 재생 : AudioClip으로 받을 때
    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm){
            AudioSource audioSource = _audioSources[(int)type];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else if (type == Define.Sound.Effect){
            AudioSource audioSource = _audioSources[(int)type];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    // 같은 Effect audio를 계속 Load해올 시 메모리에 부하가 올 수 있기 때문에
    // Dictionary를 사용하여 한번 사용한 audio는 Load하지 않고 Dictionary에 저장하여 사용한다.
    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        // path 경로에 Sounds/ 가 없을 경우
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.Sound.Bgm){
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else if (type == Define.Sound.Effect){
            if (_audioClips.TryGetValue(path, out audioClip) == false){
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }
        
        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }

    // 다른 Scene으로 넘어가면서 Dictionary를 초기화 해주지 않으면
    // 계속계속 쌓여 메모리 초과가 될 수 있기 때문에 Clear을 진행해 준다.
    public void Clear()
    {
        foreach(AudioSource audioSource in _audioSources){
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }
}
