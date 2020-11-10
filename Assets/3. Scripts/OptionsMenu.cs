using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider VolumeSlider;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        VolumeSlider = gameObject.GetComponentInChildren<Slider>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = VolumeSlider.value;
    }
}
