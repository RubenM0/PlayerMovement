using System;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private PlayerMovement player;
    
    [SerializeField] private AudioClip dashFX;
    [SerializeField] private AudioClip footstepFX;
    
    private float footstepTimer;
    private float footstepTimerMax = 0.4f;
    
    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        player.OnPlayerDashed += PlayerMovement_OnPlayerDashed;
    }

    private void Update()
    {
        PlayerFootstepSounds();
    }

    private void PlayerFootstepSounds()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f)
        {
            footstepTimer = footstepTimerMax;
            
            if (player.IsWalking && !player.isCrouching)
                SoundManager.instance.PlaySFX(footstepFX, transform.position, 0.4f, 0.1f);
        }
    }
    
    private void PlayerMovement_OnPlayerDashed(object sender, EventArgs e)
    {
        SoundManager.instance.PlaySFX(dashFX, transform.position, 0.8f, 0.1f);
    }
}