using System;
using UnityEngine;

namespace NumbersBlast.Data
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "NumbersBlast/Audio Config")]
    public class AudioConfig : ScriptableObject
    {
        [Header("Music")]
        public AudioClip MenuMusic;
        public AudioClip GameplayMusic;
        [Range(0f, 1f)] public float MusicVolume = 0.5f;

        [Header("UI")]
        public AudioClip ButtonClick;
        public AudioClip PopupOpen;
        public AudioClip PopupClose;

        [Header("Gameplay - Piece")]
        public AudioClip PiecePickup;
        public AudioClip PiecePlace;
        public AudioClip PieceReturn;

        [Header("Gameplay - Merge")]
        public AudioClip Merge;

        [Header("Gameplay - Line Clear")]
        public AudioClip LineClear;

        [Header("Gameplay - Score")]
        public AudioClip ScoreUp;

        [Header("Game State")]
        public AudioClip GameOver;
        public AudioClip GameStart;
        public AudioClip NewPiecesSpawn;

        [Header("Tutorial")]
        public AudioClip TutorialStep;
        public AudioClip TutorialComplete;

        [Header("SFX Volume")]
        [Range(0f, 1f)] public float SFXVolume = 1f;
    }
}
