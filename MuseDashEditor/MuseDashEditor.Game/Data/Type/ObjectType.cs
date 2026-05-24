namespace MuseDashEditor.Game.Data.Type;

public enum ObjectType
{
    //@formatter:off
    Empty = 0,                        // Used only for parsing and saving

    // MapObjects
    Music = 36,                       // 10

    // HitObjects
    Small = 1,                        // 01
    SmallUp = 2,                      // 02
    SmallDown = 3,                    // 03
    Medium1 = 4,                      // 04
    Medium1Up = 5,                    // 05
    Medium1Down = 6,                  // 06
    Medium2 = 7,                      // 07
    Medium2Up = 8,                    // 08
    Medium2Down = 9,                  // 09
    Large1 = 10,                      // 0A
    Large2 = 11,                      // 0B
    Raider = 12,                      // 0C
    Hammer = 13,                      // 0D
    Gemini = 14,                      // 0E
    Hold = 15,                        // 0F
    Masher = 16,                      // 0G
    Gear = 17,                        // 0H
    RaiderUpsideDown = 18,            // 0I
    HammerUpsideDown = 19,            // 0J
    PItem = 72,                       // 20 // IDK what this is
    Ghost = 73,                       // 21
    MediumBullet = 108,               // 30
    MediumBulletUp = 109,             // 31
    MediumBulletDown = 110,           // 32
    MediumBulletLaneshift = 111,      // 33
    SmallBullet = 112,                // 34
    SmallBulletUp = 113,              // 35
    SmallBulletDown = 114,            // 36
    SmallBulletLaneshift = 115,       // 37
    LargeBullet = 116,                // 38
    LargeBulletUp = 117,              // 39
    LargeBulletDown = 118,            // 3A
    LargeBulletLaneshift = 119,       // 3B

    // Collectibles
    Heart = 74,                       // 22
    Note = 75,                        // 23

    // BossHitObjects
    BossMelee1 = 37,                  // 11
    BossMelee2 = 38,                  // 12
    BossProjectile1 = 39,             // 13
    BossProjectile2 = 40,             // 14
    BossProjectile3 = 41,             // 15
    BossMasher1 = 42,                 // 16
    BossMasher2 = 43,                 // 17
    BossGear = 44,                    // 18
    BossBullet1 = 120,                // 3C
    BossBullet1Laneshift = 121,       // 3D
    BossBullet2 = 122,                // 3E
    BossBullet2Laneshift = 123,       // 3F

    // BossSpecialObjects
    BossEntrance = 46,                // 1A
    BossExit = 47,                    // 1B
    BossReadyPhase1 = 48,             // 1C
    BossEndPhase1 = 49,               // 1D
    BossReadyPhase2 = 50,             // 1E
    BossEndPhase2 = 51,               // 1F
    BossSwapPhase1To2 = 52,           // 1G
    BossSwapPhase2To1 = 53,           // 1H
    HideBoss = 57,                    // 1L
    UnhideBoss = 58,                  // 1M

    // SpecialObjects

    // SpeedChangeObjects
    Speed1Both = 24,                  // 0O
    Speed2Both = 25,                  // 0P
    Speed3Both = 26,                  // 0Q
    Speed1Low = 27,                   // 0R
    Speed2Low = 28,                   // 0S
    Speed3Low = 29,                   // 0T
    Speed1High = 30,                  // 0U
    Speed2High = 31,                  // 0V
    Speed3High = 32,                  // 0W

    // SceneSwitchObjects
    SceneSwitchSpaceStation = 60,     // 1O
    SceneSwitchRetrocity = 61,        // 1P
    SceneSwitchCastle = 62,           // 1Q
    SceneSwitchRainyNight = 63,       // 1R
    SceneSwitchCandyland = 64,        // 1S
    SceneSwitchOriental = 65,         // 1T
    SceneSwitchGrooveCoaster = 66,    // 1U
    SceneSwitchTouhou = 67,           // 1V
    SceneSwitchDjmax = 68,            // 1W
    SceneSwitchMiku = 69,             // 1X

    // DesignObjects
    HideNotes = 55,                   // 1J
    UnhideNotes = 56,                 // 1K
    HideBackground = 77,              // 25
    UnhideBackground = 78,            // 26
    ScreenScrollUp = 79,              // 27
    ScreenScrollDown = 80,            // 28
    ScreenScrollOff = 81,             // 29
    ScanlineRipplesOn = 82,           // 2A
    ScanlineRipplesOff = 83,          // 2B
    ChromaticAberrationOn = 84,       // 2C
    ChromaticAberrationOff = 85,      // 2D
    VignetteOn = 86,                  // 2E
    VignetteOff = 87,                 // 2F
    TvStaticOn = 88,                  // 2G
    TvStaticOff = 89,                 // 2H
    FlashbangStart = 90,              // 2I
    FlashbangMid = 91,                // 2J
    FlashbangEnd = 92,                // 2K
    BgStopOn = 95,                    // 2N
    BgStopOff = 96,                   // 2O
    MosaicOn = 97,                    // 2P
    MosaicOff = 98,                   // 2Q
    SepiaOn = 99,                     // 2R
    SepiaOff = 100,                   // 2S
    FocusLinesBlack = 101,            // 2T
    FocusLinesWhite = 102,            // 2U
    FocusLinesOff = 103,              // 2V
    FilmGrainOn = 104,                // 2W
    FilmGrainOff = 105,               // 2X
    FlashbangColorWhite = 454,        // CM
    FlashbangColorBlack = 455,        // CN
    FlashbangColorRed = 456,          // CO
    FlashbangColorGreen = 457,        // CP
    FlashbangColorBlue = 458,         // CQ
    FlashbangColorCyan = 459,         // CR
    FlashbangColorMagenta = 460,      // CS
    FlashbangColorYellow = 461,       // CT

    // Other
    AutoplayOn = 106,                 // 2Y
    AutoplayOff = 107,                // 2Z
    //@formatter:on
}
