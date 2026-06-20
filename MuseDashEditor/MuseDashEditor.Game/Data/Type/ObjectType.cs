// Copyright 2026 Axel "Azn9" Joly <contact@azn9.dev>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

using MuseDashEditor.Game.Data.Object.DesignObject;
using MuseDashEditor.Game.Data.Object.GameObject;

namespace MuseDashEditor.Game.Data.Type;

public enum ObjectType
{
    //@formatter:off
    Empty = 0,                        // Used only for parsing and saving

    // MapObjects
    Music = 36,                       // 10

    // === HitObjects ===
    [GameObjectData(HitSoundType.Small, TextureType.Small)]  Small = 1,                                      // 01
    [GameObjectData(HitSoundType.Small, TextureType.Small, MovementType.Up)]  SmallUp = 2,                   // 02
    [GameObjectData(HitSoundType.Small, TextureType.Small, MovementType.Down)]  SmallDown = 3,               // 03
    [GameObjectData(HitSoundType.Small, TextureType.Medium1)]  Medium1 = 4,                                  // 04
    [GameObjectData(HitSoundType.Small, TextureType.Medium1, MovementType.Up)]  Medium1Up = 5,               // 05
    [GameObjectData(HitSoundType.Small, TextureType.Medium1, MovementType.Down)]  Medium1Down = 6,           // 06
    [GameObjectData(HitSoundType.Small, TextureType.Medium2)]  Medium2 = 7,                                  // 07
    [GameObjectData(HitSoundType.Small, TextureType.Medium2, MovementType.Up)]  Medium2Up = 8,               // 08
    [GameObjectData(HitSoundType.Small, TextureType.Medium2, MovementType.Down)]  Medium2Down = 9,           // 09
    [GameObjectData(HitSoundType.Large1, TextureType.Large1)] Large1 = 10,                                   // 0A
    [GameObjectData(HitSoundType.Large2, TextureType.Large2)] Large2 = 11,                                   // 0B
    [GameObjectData(HitSoundType.Raider, TextureType.Raider)] Raider = 12,                                   // 0C
    [GameObjectData(HitSoundType.Hammer, TextureType.Hammer)] Hammer = 13,                                   // 0D
    [GameObjectData(HitSoundType.Small, TextureType.Gemini)]  Gemini = 14,                                   // 0E
    [GameObjectData(HitSoundType.Hold, TextureType.Hold)]   Hold = 15,                                       // 0F
    [GameObjectData(HitSoundType.Masher, TextureType.Masher)] Masher = 16,                                   // 0G
    [GameObjectData(HitSoundType.Gear, TextureType.Gear)]   Gear = 17,                                       // 0H
    [GameObjectData(HitSoundType.Raider, TextureType.Raider, MovementType.Laneshift)] RaiderUpsideDown = 18, // 0I
    [GameObjectData(HitSoundType.Hammer, TextureType.Hammer, MovementType.Laneshift)] HammerUpsideDown = 19, // 0J
    [GameObjectData(HitSoundType.Ghost, TextureType.Ghost)]  Ghost = 73,                                     // 21

    // Collectibles
    [GameObjectData(HitSoundType.Heart, TextureType.Heart)] Heart = 74,                 // 22
    [GameObjectData(HitSoundType.Note, TextureType.Note)]  Note = 75,                   // 23

    // Touhou special hit objects
    [TouhouSpecialGameObjectData(HitSoundType.Note, TextureType.PItem)]  PItem = 72,                                                 // 20
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.BossBullet1)]  BossBullet1 = 120,                                    // 3C
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.BossBullet1, MovementType.Laneshift)]  BossBullet1Laneshift = 121,   // 3D
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.BossBullet2)]  BossBullet2 = 122,                                    // 3E
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.BossBullet2, MovementType.Laneshift)]  BossBullet2Laneshift = 123,   // 3F
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.MediumBullet)]  MediumBullet = 108,                                  // 30
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.MediumBullet, MovementType.Up)]  MediumBulletUp = 109,               // 31
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.MediumBullet, MovementType.Down)]  MediumBulletDown = 110,           // 32
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.MediumBullet, MovementType.Laneshift)]  MediumBulletLaneshift = 111, // 33
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.SmallBullet)]  SmallBullet = 112,                                    // 34
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.SmallBullet, MovementType.Up)]  SmallBulletUp = 113,                 // 35
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.SmallBullet, MovementType.Down)]  SmallBulletDown = 114,             // 36
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.SmallBullet, MovementType.Laneshift)]  SmallBulletLaneshift = 115,   // 37
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.LargeBullet)]  LargeBullet = 116,                                    // 38
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.LargeBullet, MovementType.Up)]  LargeBulletUp = 117,                 // 39
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.LargeBullet, MovementType.Down)]  LargeBulletDown = 118,             // 3A
    [TouhouSpecialGameObjectData(HitSoundType.Gear, TextureType.LargeBullet, MovementType.Laneshift)]  LargeBulletLaneshift = 119,   // 3B

    // BossHitObjects
    [GameObjectData(HitSoundType.Small, TextureType.Boss)]  BossMelee1 = 37,                  // 11
    [GameObjectData(HitSoundType.Small, TextureType.Boss)]  BossMelee2 = 38,                  // 12
    [GameObjectData(HitSoundType.Small, TextureType.BossProjectile1)]  BossProjectile1 = 39,  // 13
    [GameObjectData(HitSoundType.Small, TextureType.BossProjectile2)]  BossProjectile2 = 40,  // 14
    [GameObjectData(HitSoundType.Small, TextureType.BossProjectile3)]  BossProjectile3 = 41,  // 15
    [GameObjectData(HitSoundType.Masher, TextureType.Boss)] BossMasher1 = 42,                 // 16
    [GameObjectData(HitSoundType.Masher, TextureType.Boss)] BossMasher2 = 43,                 // 17
    [GameObjectData(HitSoundType.Gear, TextureType.Gear)]   BossGear = 44,                    // 18

    // === SpecialObjects ===

    // Boss
    [DesignObjectData] BossEntrance = 46,                // 1A
    [DesignObjectData] BossExit = 47,                    // 1B
    [DesignObjectData] BossReadyPhase1 = 48,             // 1C
    [DesignObjectData] BossEndPhase1 = 49,               // 1D
    [DesignObjectData] BossReadyPhase2 = 50,             // 1E
    [DesignObjectData] BossEndPhase2 = 51,               // 1F
    [DesignObjectData] BossSwapPhase1To2 = 52,           // 1G
    [DesignObjectData] BossSwapPhase2To1 = 53,           // 1H
    [DesignObjectData] HideBoss = 57,                    // 1L
    [DesignObjectData] UnhideBoss = 58,                  // 1M

    // SpeedChangeObjects
    [DesignObjectData] Speed1Both = 24,                  // 0O
    [DesignObjectData] Speed2Both = 25,                  // 0P
    [DesignObjectData] Speed3Both = 26,                  // 0Q
    [DesignObjectData] Speed1Low = 27,                   // 0R
    [DesignObjectData] Speed2Low = 28,                   // 0S
    [DesignObjectData] Speed3Low = 29,                   // 0T
    [DesignObjectData] Speed1High = 30,                  // 0U
    [DesignObjectData] Speed2High = 31,                  // 0V
    [DesignObjectData] Speed3High = 32,                  // 0W

    // SceneSwitchObjects
    [DesignObjectData] SceneSwitchSpaceStation = 60,     // 1O
    [DesignObjectData] SceneSwitchRetrocity = 61,        // 1P
    [DesignObjectData] SceneSwitchCastle = 62,           // 1Q
    [DesignObjectData] SceneSwitchRainyNight = 63,       // 1R
    [DesignObjectData] SceneSwitchCandyland = 64,        // 1S
    [DesignObjectData] SceneSwitchOriental = 65,         // 1T
    [DesignObjectData] SceneSwitchGrooveCoaster = 66,    // 1U
    [DesignObjectData] SceneSwitchTouhou = 67,           // 1V
    [DesignObjectData] SceneSwitchDjmax = 68,            // 1W
    [DesignObjectData] SceneSwitchMiku = 69,             // 1X

    // DesignObjects
    [DesignObjectData] HideNotes = 55,                   // 1J
    [DesignObjectData] UnhideNotes = 56,                 // 1K
    [DesignObjectData] HideBackground = 77,              // 25
    [DesignObjectData] UnhideBackground = 78,            // 26
    [DesignObjectData] ScreenScrollUp = 79,              // 27
    [DesignObjectData] ScreenScrollDown = 80,            // 28
    [DesignObjectData] ScreenScrollOff = 81,             // 29
    [DesignObjectData] ScanlineRipplesOn = 82,           // 2A
    [DesignObjectData] ScanlineRipplesOff = 83,          // 2B
    [DesignObjectData] ChromaticAberrationOn = 84,       // 2C
    [DesignObjectData] ChromaticAberrationOff = 85,      // 2D
    [DesignObjectData] VignetteOn = 86,                  // 2E
    [DesignObjectData] VignetteOff = 87,                 // 2F
    [DesignObjectData] TvStaticOn = 88,                  // 2G
    [DesignObjectData] TvStaticOff = 89,                 // 2H
    [DesignObjectData] FlashbangStart = 90,              // 2I
    [DesignObjectData] FlashbangMid = 91,                // 2J
    [DesignObjectData] FlashbangEnd = 92,                // 2K
    [DesignObjectData] BgStopOn = 95,                    // 2N
    [DesignObjectData] BgStopOff = 96,                   // 2O
    [DesignObjectData] MosaicOn = 97,                    // 2P
    [DesignObjectData] MosaicOff = 98,                   // 2Q
    [DesignObjectData] SepiaOn = 99,                     // 2R
    [DesignObjectData] SepiaOff = 100,                   // 2S
    [DesignObjectData] FocusLinesBlack = 101,            // 2T
    [DesignObjectData] FocusLinesWhite = 102,            // 2U
    [DesignObjectData] FocusLinesOff = 103,              // 2V
    [DesignObjectData] FilmGrainOn = 104,                // 2W
    [DesignObjectData] FilmGrainOff = 105,               // 2X
    [DesignObjectData] FlashbangColorWhite = 454,        // CM
    [DesignObjectData] FlashbangColorBlack = 455,        // CN
    [DesignObjectData] FlashbangColorRed = 456,          // CO
    [DesignObjectData] FlashbangColorGreen = 457,        // CP
    [DesignObjectData] FlashbangColorBlue = 458,         // CQ
    [DesignObjectData] FlashbangColorCyan = 459,         // CR
    [DesignObjectData] FlashbangColorMagenta = 460,      // CS
    [DesignObjectData] FlashbangColorYellow = 461,       // CT

    // Other
    AutoplayOn = 106,                 // 2Y
    AutoplayOff = 107,                // 2Z

    // Cheats
    [GameObjectData(HitSoundType.Hold, TextureType.HoldBody)] HoldBody = 999
    //@formatter:on
}
