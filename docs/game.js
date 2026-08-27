(function () {
  "use strict";

  const ROWS = 4;
  const COLS = 5;
  const MAX_FREE_SPINS = 20;
  const RIFT_MAX = 100;
  const RIFT_SPIN_LEVEL_BOOST = 1;
  const RIFT_SPECIAL_PROBABILITY_MULTIPLIER = 2;
  const VOID_BONUS_TRIGGER_CHANCE = 0.3;
  const VOID_BONUS_ROUNDS = 3;
  const VOID_BONUS_PAYOUT_MULTIPLIER = 2;
  const VOID_SPECIAL_PROBABILITY_MULTIPLIER = 2;
  const JACKPOT_INITIAL_VALUE = 1000;
  const JACKPOT_SPIN_CONTRIBUTION_RATE = 0.05;
  const JACKPOT_TRIGGER_CHANCE = 0.1;
  const STAGE_QUOTA_BONUS_SPINS = 5;
  const STAGE_FAIL_AD_SPINS = 10;
  const STAGE_COIN_RESCUE_MULTIPLIER = 2;
  const STARDUST_EXCHANGE_COST = 10;
  const NATIVE_HIGH_TIER_WEIGHT_MULTIPLIER = 0.7;
  const PITY_TIER3_LIMIT = 10;
  const PITY_TIER4_LIMIT = 20;
  const STAGE_PITY_TRIGGER_LIMIT = 2;
  const TALENT_HIGH_TIER_CHANCE = 0.05;
  const TALENT_ENERGY_MULTIPLIER = 1.1;
  const TALENT_SPECIAL_MULTIPLIER = 1.2;
  const TALENT_OVERLOAD_CHANCE = 0.1;
  const TALENT_COMBO_CAP_MULTIPLIER = 2.6;
  const TALENT_FREE_SPIN_CAP_BONUS = 5;
  const TALENT_L5_RETAIN_CHANCE = 0.2;
  const MODE_ADVENTURE = "adventure";
  const MODE_ENDLESS = "endless";
  const MODE_TIMED = "timed";
  const MODE_ZEN = "zen";
  const TIMED_MODE_DURATION_MS = 120000;
  const STORAGE_KEY = "gem-merge-slot-demo-v1";
  const TUTORIAL_VERSION = 2;
  const FX_CONFIG = {
    enabled: true,
    quality: "medium",
    particleScale: 1,
    merge: true,
    lineWin: true,
    particles: true,
    shake: true,
    floatText: true,
    screenFlash: true,
    coinRain: true,
    comboBounce: true
  };
  const HAPTIC_STRENGTHS = {
    low: { label: "Low", scale: 0.62 },
    medium: { label: "Medium", scale: 0.82 },
    high: { label: "High", scale: 1 }
  };
  const HAPTIC_PRIORITY = {
    tap: 1,
    basic: 2,
    medium: 3,
    milestone: 4,
    heavy: 5,
    failure: 6,
    epic: 7
  };
  const HAPTIC_FALLBACK_PATTERNS = {
    tap: [12],
    basic: [18],
    medium: [26, 18, 18],
    milestone: [28, 24, 24],
    heavy: [36, 28, 32],
    failure: [42, 46, 28],
    epic: [28, 42, 72]
  };
  const QUALITY_PRESETS = {
    low: { label: "Low", particleScale: 0, particles: false, screenFlash: false, coinRain: false },
    medium: { label: "Medium", particleScale: 0.34, particles: true, screenFlash: true, coinRain: false },
    high: { label: "High", particleScale: 0.62, particles: true, screenFlash: true, coinRain: true }
  };
  const PERF_PROFILES = {
    small: { particleScale: 0.36, maxParticles: 34, maxBurst: 14, maxCoinRain: 3, textFitInterval: 260 },
    standard: { particleScale: 0.58, maxParticles: 74, maxBurst: 26, maxCoinRain: 7, textFitInterval: 180 },
    tablet: { particleScale: 0.48, maxParticles: 58, maxBurst: 22, maxCoinRain: 6, textFitInterval: 210 }
  };
  const PARTICLE_BURST_MULTIPLIERS = {
    "genesis-spectrum": 0.72,
    "crystal-spectrum": 0.46,
    "crystal-gold": 0.36,
    default: 0.26
  };
  const FLOAT_NODE_LIMIT = 18;
  const EFFECT_VIEWPORT_MARGIN = 80;
  const TUTORIAL_STEPS = [
    {
      title: "Spin the Board",
      text: "Tap the illuminated SPIN button now to create this round's crystals.",
      selector: "#spinButton",
      advanceOn: "click"
    },
    {
      title: "Merge Crystals",
      text: "Drag adjacent crystals of the same tier. A group of 3 or more evolves into the next tier.",
      selector: "#board",
      advanceOn: "next"
    },
    {
      title: "Open Your Items",
      text: "Tap the illuminated Items button to see each item name, effect, and quantity.",
      selector: "#topItemButton",
      advanceOn: "click",
      closeAfter: "itemModal"
    },
    {
      title: "Follow the Objective",
      text: "This panel shows the current goal, remaining Stage Spins, and completion progress.",
      selector: ".task-panel",
      advanceOn: "next"
    }
  ];
  const CONTROL_HINTS = {
    topItemButton: {
      key: "items",
      text: "Items: open your tool bag. Use Swap Cube, Purge Blast, Tier Leap, or Prism Call to solve tough boards."
    },
    itemButton: {
      key: "lobby",
      text: "Lobby: return to the mode-select screen. Your current progress is saved first."
    },
    undoButton: {
      key: "undo",
      text: "Undo: revert your last merge once during the current spin round."
    },
    topTalentButton: {
      key: "talent",
      text: "Talents: spend Stardust on permanent upgrades that make future stages easier."
    },
    activeSkillButton: {
      key: "ability",
      text: "Ability: use your equipped special skill after a spin, during the merge phase."
    },
    autoMergeButton: {
      key: "boardReset",
      text: "Board Reset: refreshes the current board layout while keeping stage progress."
    },
    resetButton: {
      key: "reset",
      text: "Reset: restart the current local save. Use carefully."
    }
  };
  const LEGAL_CONTENT = {
    terms: {
      title: "Terms of Service",
      sections: [
        {
          heading: "Free Entertainment Game",
          body: "crystal merge slot is provided as a free-to-play casual entertainment game. All virtual resources, including Coins, Free Spins, Stardust, and Items, are in-game entertainment items only and have no real-money value."
        },
        {
          heading: "Virtual Resources",
          body: "Virtual resources cannot be exchanged, redeemed, sold, transferred, or converted into cash, prizes, goods, services, or any real-world value. They exist only inside this local game experience."
        },
        {
          heading: "Local Save Data",
          body: "Your progress is stored locally in the current browser on your device. You may reset all local game data at any time by using the Reset Data button in Settings."
        },
        {
          heading: "Optional Rewarded Ads",
          body: "If rewarded ads are available in a future build, watching them is voluntary and only used to grant extra in-game rewards. The game does not require forced advertising to play."
        },
        {
          heading: "No Gambling",
          body: "This game is for casual entertainment only. It does not provide gambling, betting, cash-out, wagering, real-money prize, or any other gambling-related functionality."
        },
        {
          heading: "User Responsibility",
          body: "By playing, you agree to use the game only for personal entertainment and to follow these Terms of Service."
        }
      ]
    },
    privacy: {
      title: "Privacy Policy",
      sections: [
        {
          heading: "No Personal Data Collection",
          body: "This game does not collect, upload, sell, share, or transmit your personal information, device information, photos, location, contacts, or any similar private data."
        },
        {
          heading: "Local Progress Only",
          body: "All game progress is stored locally in the current browser on your device. It is not synced to an external server and is not available to the developer or any third party."
        },
        {
          heading: "Data Deletion",
          body: "You can delete all local save data at any time from the Settings page by using the Reset Data button. This clears locally stored progress from the current browser."
        },
        {
          heading: "No Third-Party Tracking",
          body: "This game does not use third-party data collection, third-party privacy tracking, or third-party advertising tracking to identify or profile users."
        },
        {
          heading: "Browser Storage",
          body: "The game uses browser local storage only to remember local game progress, settings, and whether you have accepted the Terms of Service and Privacy Policy."
        },
        {
          heading: "Policy Updates",
          body: "If this policy changes in a future version, the updated text will be shown inside the game before the updated terms apply."
        }
      ]
    }
  };

  const LEVELS = {
    1: { name: "Tier1 Shard", short: "T1", multiplier: 1 },
    2: { name: "Tier2 Focus Crystal", short: "T2", multiplier: 3 },
    3: { name: "Tier3 Starcore Crystal", short: "T3", multiplier: 10 },
    4: { name: "Tier4 Void Prism", short: "T4", multiplier: 50 },
    5: { name: "Tier5 Genesis Crystal", short: "T5", multiplier: 200 }
  };
  const GEM_EFFECTS = {
    1: {
      short: "Basic filler crystal with no special effect.",
      path: "Created by spins or gravity refills, and used as Tier1 merge material.",
      detail: "Base line multiplier x1. Tier1 Shards fill the board and do not trigger extra merge effects."
    },
    2: {
      short: "Clears low-tier crystals within 2 tiles on merge.",
      path: "Merge 3 adjacent Tier1 Shards.",
      detail: "Base line multiplier x3. When created, Tier2 Focus Crystal clears nearby low-tier crystals within 2 tiles and refills blanks."
    },
    3: {
      short: "Clears its full column on merge.",
      path: "Merge 3 adjacent Tier2 Focus Crystals.",
      detail: "Base line multiplier x10. When created, Tier3 Starcore Crystal clears its entire column and refills new symbols."
    },
    4: {
      short: "Triggers a half-board chain clear; new line wins x1.5.",
      path: "Merge 3 adjacent Tier3 Starcore Crystals.",
      detail: "Base line multiplier x50. When created, Tier4 Void Prism triggers a half-board chain clear and boosts new line wins to x1.5."
    },
    5: {
      short: "Triggers a full-board clear, board-wide x2, and grants 3 Free Spins.",
      path: "Merge 3 adjacent Tier4 Void Prisms.",
      detail: "Base line multiplier x200. When created, Tier5 Genesis Crystal clears the board, applies board-wide x2 rewards, and grants 3 Free Spins."
    }
  };
  const SPECIAL_EFFECTS = {
    bomb: {
      name: "Rift Bomb",
      short: "Auto-detonates and clears low-tier crystals in a 3x3 area.",
      detail: "Rarely appears on spins and does not join tier merges. It detonates on the board, clearing all Tier1/Tier2 crystals in its 3x3 area, then triggers gravity and cascade checks."
    },
    wild: {
      name: "Holo Proxy",
      short: "Wild crystal that can stand in for any tier once.",
      detail: "Rarely appears on spins and can count as any crystal tier for one merge. It disappears after the merge and only works through orthogonal adjacency."
    },
    coin: {
      name: "Gold Core",
      short: "High-value symbol; 3+ in a row pays 10x base Coins.",
      detail: "Rarely appears on spins and does not join tier merges. A horizontal line of 3 or more pays 10x base Coins, then clears."
    }
  };

  const SPECIAL_SYMBOLS = {
    bomb: { name: "Rift Bomb", short: "B", rewardMultiplier: 2 },
    wild: { name: "Holo Proxy", short: "W", rewardMultiplier: 0 },
    coin: { name: "Gold Core", short: "G", rewardMultiplier: 10 }
  };
  const SPECIAL_SPAWN_CONFIG = {
    bomb: { probability: 0.005 },
    wild: { probability: 0.003 },
    coin: { probability: 0.008 }
  };
  const ACTIVE_SKILLS = {
    columnRedraw: { name: "Column Reroll", icon: "▥", cooldown: 5 },
    fieldRankUp: { name: "Field Tier Up", icon: "⇧", cooldown: 8 },
    lowTierPurge: { name: "Low-Tier Purge", icon: "✺", cooldown: 10 }
  };
  const DEFAULT_ACTIVE_SKILL = "columnRedraw";
  const DEFAULT_UNLOCKED_SKILLS = Object.keys(ACTIVE_SKILLS);
  const GAME_MODES = {
    [MODE_ADVENTURE]: { title: "Campaign Mode", short: "Stage", desc: "Keep Stage objectives, Items, saves, and reward progression." },
    [MODE_ENDLESS]: { title: "Endless Mode", short: "Endless", desc: "No Stage limit. Spin endlessly for high scores and best combos." },
    [MODE_TIMED]: { title: "Time Challenge", short: "Timed", desc: "Two minutes to merge high-tier crystals and build big combos." },
    [MODE_ZEN]: { title: "Zen Mode", short: "Zen", desc: "No cost, no goals, no failure. Relaxed merging with no rewards." }
  };
  const TALENT_LINES = [
    {
      key: "base",
      title: "Core Path",
      nodes: [
        { key: "nativeHighTier", name: "Star Refraction", desc: "Native high-tier crystal chance +5%.", cost: 60 },
        { key: "energyCharge", name: "Rift Conduit", desc: "Rift Energy charge speed +10%.", cost: 140 },
        { key: "freeSpinCap", name: "Spin Capacity", desc: "Free Spins cap +5.", cost: 260 }
      ]
    },
    {
      key: "advanced",
      title: "Advanced Path",
      nodes: [
        { key: "overloadBoost", name: "Overload Resonance", desc: "3-to-1 merges can trigger overload tier jumps.", cost: 100 },
        { key: "comboCap", name: "Combo Overclock", desc: "Raises the Combo Multiplier cap.", cost: 220 },
        { key: "specialRate", name: "Symbol Resonance", desc: "Special symbol spawn chance increased.", cost: 380 }
      ]
    },
    {
      key: "ultimate",
      title: "Ultimate Path",
      nodes: [
        { key: "l5Retain", name: "Genesis Echo", desc: "Tier5 merge has a 20% chance to remain for 1 round.", cost: 650 }
      ]
    }
  ];
  const TALENT_NODE_MAP = TALENT_LINES.reduce((map, line) => {
    line.nodes.forEach((node, index) => {
      map[node.key] = { ...node, line: line.key, index };
    });
    return map;
  }, {});
  const DAILY_TASK_POOL = [
    { key: "dailyMergeL3", title: "Merge 2 Tier3 Starcore Crystals", type: "mergeLevel", level: 3, target: 2, reward: { coins: 120, randomItem: 1 } },
    { key: "dailyCombo5", title: "Hit a 5 Combo once", type: "comboThreshold", threshold: 5, target: 1, reward: { coins: 100, randomItem: 1 } },
    { key: "dailyUseTool", title: "Use 1 Item", type: "toolUse", target: 1, reward: { coins: 90, randomItem: 1 } },
    { key: "dailyClear", title: "Clear 15 crystals", type: "clear", target: 15, reward: { coins: 110, randomItem: 1 } },
    { key: "dailyLineWin", title: "Score 3 line wins", type: "lineWin", target: 3, reward: { coins: 130, randomItem: 1 } },
    { key: "dailySpin", title: "Complete 3 spins", type: "spin", target: 3, reward: { coins: 100, randomItem: 1 } }
  ];
  const WEEKLY_TASK_POOL = [
    { key: "weeklyLevel5", title: "Clear 5 Stages", type: "levelComplete", target: 5, reward: { freeSpins: 8, item: "summon" } },
    { key: "weeklyClear150", title: "Clear 150 crystals total", type: "clear", target: 150, reward: { freeSpins: 7, item: "leap" } },
    { key: "weeklyCombo10", title: "Hit 10 Combo 3 times", type: "comboThreshold", threshold: 10, target: 3, reward: { freeSpins: 8, item: "summon" } },
    { key: "weeklyMergeL4", title: "Merge 8 Tier4 Void Prisms", type: "mergeLevel", level: 4, target: 8, reward: { freeSpins: 7, item: "summon" } }
  ];
  const ACHIEVEMENTS = {
    firstSpin: { name: "First Spin", desc: "Complete 1 spin.", type: "spin", target: 1, stardust: 10, text: "Achievement: First Spin" },
    firstMerge: { name: "Merge Starter", desc: "Complete 1 merge.", type: "mergeAny", target: 1, stardust: 15, text: "Achievement: Merge Starter" },
    firstL3: { name: "Starcore Spark", desc: "Merge Tier3 for the first time.", type: "mergeLevel", level: 3, target: 1, stardust: 25, text: "Achievement: Starcore Spark" },
    firstL4: { name: "Void Prism", desc: "Merge Tier4 for the first time.", type: "mergeLevel", level: 4, target: 1, stardust: 50, text: "Achievement: Void Prism" },
    firstL5: { name: "Genesis Crystal", desc: "Merge Tier5 for the first time.", type: "mergeLevel", level: 5, target: 1, stardust: 120, text: "Achievement: First Tier5" },
    combo5: { name: "Combo Ignition", desc: "Hit a 5 Combo once.", type: "comboThreshold", threshold: 5, target: 1, stardust: 25, text: "Achievement: 5 Combo" },
    combo10: { name: "Ten-Combo Overdrive", desc: "Hit a 10 Combo once.", type: "comboThreshold", threshold: 10, target: 1, stardust: 60, text: "Achievement: 10 Combo" },
    clear100: { name: "Board Sweeper", desc: "Clear 100 crystals total.", type: "clear", target: 100, stardust: 35, text: "Achievement: Board Sweeper" },
    clear1000: { name: "Thousand Crystal Clear", desc: "Clear 1,000 crystals total.", type: "clear", target: 1000, stardust: 180, text: "Achievement: Thousand Clear" },
    line10: { name: "Line Win Rookie", desc: "Score 10 line wins total.", type: "lineWin", target: 10, stardust: 30, text: "Achievement: Line Win Rookie" },
    line100: { name: "Hundred-Line Shine", desc: "Score 100 line wins total.", type: "lineWin", target: 100, stardust: 140, text: "Achievement: Hundred-Line Shine" },
    goldCore3: { name: "Gold Core Resonance", desc: "Score 3 Gold Core line wins.", type: "goldCoreWin", target: 3, stardust: 50, text: "Achievement: Gold Core Resonance" },
    bomb10: { name: "Rift Blaster", desc: "Trigger 10 Rift Bombs.", type: "bomb", target: 10, stardust: 70, text: "Achievement: Rift Blaster" },
    tool10: { name: "Item Expert", desc: "Use 10 Items total.", type: "toolUse", target: 10, stardust: 80, text: "Achievement: Item Expert" },
    level5: { name: "Stage Pioneer", desc: "Clear 5 Stages total.", type: "levelComplete", target: 5, stardust: 70, text: "Achievement: Stage Pioneer" },
    level20: { name: "Deep Push", desc: "Clear 20 Stages total.", type: "levelComplete", target: 20, stardust: 220, text: "Achievement: Deep Push" },
    chapter1: { name: "Chapter Chest", desc: "Open 1 Chapter Chest.", type: "chapterComplete", target: 1, stardust: 100, text: "Achievement: Chapter Chest" },
    jackpotBreak: { name: "Jackpot Breaker", desc: "Break the Grand Jackpot once.", type: "jackpotBreak", target: 1, stardust: 160, text: "Achievement: Grand Jackpot Break" },
    voidRealm: { name: "Void Return", desc: "Complete 1 Void Realm.", type: "voidBonusComplete", target: 1, stardust: 120, text: "Achievement: Void Return" },
    talent1: { name: "Stardust Initiate", desc: "Unlock 1 permanent Talent.", type: "talentUnlock", target: 1, stardust: 60, text: "Achievement: Stardust Initiate" }
  };

  const ITEM_LABELS = {
    swap: "Swap Cube",
    blast: "Purge Blast",
    leap: "Tier Leap",
    summon: "Prism Call"
  };
  const ITEM_DETAILS = {
    swap: {
      icon: "◇",
      short: "Swap any two crystals, ignoring adjacency.",
      desc: "After use, click two crystals to swap their positions directly. Adjacency rules are ignored, making it easier to set up merges.",
      source: "Available from Stage rewards, Chapter Chests, and Daily Tasks."
    },
    blast: {
      icon: "✺",
      short: "Clear one selected low-tier crystal and refill.",
      desc: "After use, click any Tier1 or Tier2 crystal to clear it. Gravity fills the empty slot with a new symbol.",
      source: "Available from Stage rewards and Daily Tasks."
    },
    leap: {
      icon: "⇧",
      short: "Auto-merge 3 same-tier crystals upward.",
      desc: "After use, the board identifies a group of 3 same-tier crystals and merges them into the next tier without manual dragging.",
      source: "Available from Chapter Chests and Weekly Tasks."
    },
    summon: {
      icon: "★",
      short: "Guarantees at least 1 Tier4 Void Prism on the next spin.",
      desc: "Takes effect immediately. Your next spin is guaranteed to create at least 1 Tier4 Void Prism, greatly improving high-tier merge potential.",
      source: "Available from Chapter finale rewards and Achievement rewards."
    }
  };

  const CHALLENGE_LABELS = {
    merge: "Complete {n} merges",
    combo: "Reach a {n} Combo in one chain",
    clear: "Clear {n} crystals"
  };

  const ELEMENTS = {
    void: { name: "Void", short: "Void", color: "#a78bfa", beats: "quantum" },
    quantum: { name: "Quantum", short: "Blue", color: "#4edcff", beats: "neon" },
    neon: { name: "Neon", short: "Pink", color: "#ff6f9e", beats: "void" }
  };
  const ELEMENT_KEYS = Object.keys(ELEMENTS);

  const COLORS = ["red", "blue", "green"];
  const SPIN_DROPS = createSpinDropTable(1);
  const FILL_DROPS = [
    { kind: "gem", level: 1, weight: 73 },
    { kind: "gem", level: 2, weight: 24 },
    { kind: "gem", level: 3, weight: 2 }
  ];
  const PURGE_DROPS = [
    { kind: "gem", level: 2, weight: 50 },
    { kind: "gem", level: 3, weight: 50 }
  ];
  const RIFT_SPIN_DROPS = createSpinDropTable(RIFT_SPECIAL_PROBABILITY_MULTIPLIER);
  const VOID_SPIN_DROPS = createVoidSpinDropTable();

  const dom = {
    startScreen: document.getElementById("startScreen"),
    board: document.getElementById("board"),
    boardWrap: document.querySelector(".board-wrap"),
    playfield: document.querySelector(".playfield"),
    sideHud: document.querySelector(".side-hud"),
    coinValue: document.getElementById("coinValue"),
    freeSpinValue: document.getElementById("freeSpinValue"),
    stageValue: document.getElementById("stageValue"),
    stardustValue: document.getElementById("stardustValue"),
    dailyElementValue: document.getElementById("dailyElementValue"),
    comboValue: document.getElementById("comboValue"),
    multiplierValue: document.getElementById("multiplierValue"),
    bestComboValue: document.getElementById("bestComboValue"),
    riftEnergyBar: document.getElementById("riftEnergyBar") || document.getElementById("luckyEnergyBar"),
    riftEnergyText: document.getElementById("riftEnergyText") || document.getElementById("luckyEnergyText"),
    sideComboValue: document.getElementById("sideComboValue"),
    sideMultiplierValue: document.getElementById("sideMultiplierValue"),
    sideRiftEnergyBar: document.getElementById("sideRiftEnergyBar"),
    sideRiftEnergyText: document.getElementById("sideRiftEnergyText"),
    jackpotValue: document.getElementById("jackpotValue"),
    voidBonusBadge: document.getElementById("voidBonusBadge"),
    voidBonusText: document.getElementById("voidBonusText"),
    modeKicker: document.getElementById("modeKicker"),
    modeTitle: document.getElementById("modeTitle"),
    modeDescription: document.getElementById("modeDescription"),
    modeStats: document.getElementById("modeStats"),
    modeLeaderboard: document.getElementById("modeLeaderboard"),
    levelTypeLabel: document.getElementById("levelTypeLabel"),
    taskTitle: document.getElementById("taskTitle"),
    rewardPreview: document.getElementById("rewardPreview"),
    stageSpinQuotaText: document.getElementById("stageSpinQuotaText"),
    taskBookButton: document.getElementById("taskBookButton"),
    taskProgressBar: document.getElementById("taskProgressBar"),
    taskProgressText: document.getElementById("taskProgressText"),
    challengeTitle: document.getElementById("challengeTitle"),
    challengeProgressBar: document.getElementById("challengeProgressBar"),
    challengeProgressText: document.getElementById("challengeProgressText"),
    challengeBadge: document.getElementById("challengeBadge"),
    challengeToggleButton: document.getElementById("challengeToggleButton"),
    spinCostText: document.getElementById("spinCostText"),
    spinButton: document.getElementById("spinButton"),
    spinButtonLabel: document.getElementById("spinButtonLabel"),
    spinButtonCost: document.getElementById("spinButtonCost"),
    spinQuotaBadge: document.getElementById("spinQuotaBadge"),
    autoMergeButton: document.getElementById("autoMergeButton"),
    itemButton: document.getElementById("itemButton"),
    topItemButton: document.getElementById("topItemButton"),
    settingsButton: document.getElementById("settingsButton"),
    helpButton: document.getElementById("helpButton"),
    utilityMenuButton: document.getElementById("utilityMenuButton"),
    utilityMenu: document.getElementById("utilityMenu"),
    topTalentButton: document.getElementById("topTalentButton"),
    resetButton: document.getElementById("resetButton"),
    tipText: document.getElementById("tipText"),
    floatLayer: document.getElementById("floatLayer"),
    screenFlash: document.getElementById("screenFlash"),
    skillDock: document.getElementById("skillDock"),
    activeSkillButton: document.getElementById("activeSkillButton"),
    activeSkillIcon: document.getElementById("activeSkillIcon"),
    activeSkillName: document.getElementById("activeSkillName"),
    activeSkillCooldown: document.getElementById("activeSkillCooldown"),
    skillSwitchButton: document.getElementById("skillSwitchButton"),
    skillPanel: document.getElementById("skillPanel"),
    undoButton: document.getElementById("undoButton"),
    itemModal: document.getElementById("itemModal"),
    toolDetailModal: document.getElementById("toolDetailModal"),
    toolDetailIcon: document.getElementById("toolDetailIcon"),
    toolDetailTitle: document.getElementById("toolDetailTitle"),
    toolDetailName: document.getElementById("toolDetailName"),
    toolDetailDesc: document.getElementById("toolDetailDesc"),
    toolDetailSource: document.getElementById("toolDetailSource"),
    toolDetailCount: document.getElementById("toolDetailCount"),
    toolDetailUseButton: document.getElementById("toolDetailUseButton"),
    helpModal: document.getElementById("helpModal"),
    helpRulesPanel: document.getElementById("helpRulesPanel"),
    helpCodexPanel: document.getElementById("helpCodexPanel"),
    gemCodexList: document.getElementById("gemCodexList"),
    settingsModal: document.getElementById("settingsModal"),
    shakeToggle: document.getElementById("shakeToggle"),
    hapticStrengthValue: document.getElementById("hapticStrengthValue"),
    soundToggle: document.getElementById("soundToggle"),
    volumeSlider: document.getElementById("volumeSlider"),
    volumeValue: document.getElementById("volumeValue"),
    autoMergeToggle: document.getElementById("autoMergeToggle"),
    exitGameButton: document.getElementById("exitGameButton"),
    resetDataButton: document.getElementById("resetDataButton"),
    complianceModal: document.getElementById("complianceModal"),
    complianceConsentCheckbox: document.getElementById("complianceConsentCheckbox"),
    agreeComplianceButton: document.getElementById("agreeComplianceButton"),
    legalModal: document.getElementById("legalModal"),
    legalModalTitle: document.getElementById("legalModalTitle"),
    legalModalBody: document.getElementById("legalModalBody"),
    tutorialOverlay: document.getElementById("tutorialOverlay"),
    tutorialStepLabel: document.getElementById("tutorialStepLabel"),
    tutorialTitle: document.getElementById("tutorialTitle"),
    tutorialText: document.getElementById("tutorialText"),
    tutorialSkipButton: document.getElementById("tutorialSkipButton"),
    tutorialNextButton: document.getElementById("tutorialNextButton"),
    tutorialCard: document.querySelector("#tutorialOverlay .tutorial-card"),
    tutorialShades: Array.from(document.querySelectorAll("#tutorialOverlay .tutorial-shade")),
    tutorialTargetRing: document.querySelector("#tutorialOverlay .tutorial-target-ring"),
    taskBookModal: document.getElementById("taskBookModal"),
    taskRefreshText: document.getElementById("taskRefreshText"),
    dailyTaskList: document.getElementById("dailyTaskList"),
    weeklyTaskList: document.getElementById("weeklyTaskList"),
    achievementCodex: document.getElementById("achievementCodex"),
    talentModal: document.getElementById("talentModal"),
    talentTree: document.getElementById("talentTree"),
    talentStardustValue: document.getElementById("talentStardustValue"),
    rewardModal: document.getElementById("rewardModal"),
    rewardModalTitle: document.getElementById("rewardModalTitle"),
    rewardModalText: document.getElementById("rewardModalText"),
    rewardConfirmButton: document.getElementById("rewardConfirmButton"),
    stageFailModal: document.getElementById("stageFailModal"),
    watchAdSpinsButton: document.getElementById("watchAdSpinsButton"),
    restartStageButton: document.getElementById("restartStageButton"),
    coinRescueModal: document.getElementById("coinRescueModal"),
    coinRescueText: document.getElementById("coinRescueText"),
    watchAdCoinsButton: document.getElementById("watchAdCoinsButton"),
    exchangeStardustButton: document.getElementById("exchangeStardustButton"),
    jackpotModal: document.getElementById("jackpotModal"),
    jackpotModalText: document.getElementById("jackpotModalText"),
    jackpotConfirmButton: document.getElementById("jackpotConfirmButton"),
    swapCount: document.getElementById("swapCount"),
    blastCount: document.getElementById("blastCount"),
    leapCount: document.getElementById("leapCount"),
    summonCount: document.getElementById("summonCount")
  };

  let nextGemId = 1;
  let state = loadState();
  let runtime = createRuntime();
  let perf = createPerformanceRuntime();
  let audioRuntime = null;
  const BGM_TRACKS = {
    menu: "assets/audio/menu-ambience.wav",
    game: "assets/audio/gameplay-loop.wav",
    fail: "assets/audio/stage-failed.wav",
    combo: "assets/audio/combo-layer.wav"
  };
  const AUDIO_COOLDOWNS = {
    button: 35,
    tooltip: 120,
    drop: 80,
    coin: 75,
    combo: 55,
    merge: 45,
    warning: 500
  };
  const hapticRuntime = {
    lastAt: 0,
    lastPriority: 0
  };

  window.GEM_SLOT_FX_CONFIG = FX_CONFIG;

  init();

  function init() {
    updateViewportLayoutState();
    updatePerformanceProfile();
    ensureBoard();
    applyLevelConfig();
    refreshTasks(state);
    applyFxSettings();
    loadModeState(state.currentMode, true);
    startModeTimer();
    render();
    bindEvents();
    setupAudioSceneObserver();
    setTip("Spin to merge crystals. Gold Core and Rift Bomb trigger instant feedback.");
    updateComplianceGate();
    updateBgmMix(true);
  }

  function createRuntime() {
    return {
      animating: false,
      drag: null,
      selected: null,
      toolMode: null,
      toolSelection: null,
      skillMode: null,
      skillLevelBoostActive: false,
      freeSpinRound: false,
      riftSpinRound: false,
      voidBonusActive: false,
      voidBonusRound: 0,
      voidBonusTotal: 0,
      voidSavedBoard: null,
      voidSavedManualUnlocked: false,
      lineBoost: 1,
      fullBoost: 1,
      combo: 0,
      comboFadeTimer: null,
      comboResetTimer: null,
      rewardBuffer: null,
      mergePreview: null,
      selectedToolDetail: null,
      toolHoverTimer: null,
      toolHoverElement: null,
      modeTimer: null,
      tipTimer: null,
      textFitScheduled: false,
      textFitObserver: null,
      floatTextLane: 0,
      undoSnapshot: null,
      undoUsed: false,
      challengeOpen: false,
      codexRendered: false,
      resizePerfTimer: null,
      tutorialStep: 0,
      tutorialTarget: null,
      tutorialTargetHandler: null,
      abandonSaveArmed: false,
      abandonSaveTimer: null
    };
  }

  function createPerformanceRuntime() {
    return {
      profile: "standard",
      hidden: false,
      activeParticles: new Set(),
      activeFloatNodes: new Set(),
      textFitLast: 0,
      textFitTimer: null
    };
  }

  function bindEvents() {
    dom.spinButton.addEventListener("click", spin);
    dom.autoMergeButton.addEventListener("click", onAutoMergeShortcutClick);
    dom.itemButton.addEventListener("click", returnToModeSelect);
    dom.topItemButton.addEventListener("click", openItemModal);
    dom.topItemButton.addEventListener("mouseenter", () => showToolHover(dom.topItemButton, "tools"));
    dom.topItemButton.addEventListener("mouseleave", scheduleToolHoverHide);
    dom.itemButton.addEventListener("mouseenter", () => showToolHover(dom.itemButton, "lobby"));
    dom.itemButton.addEventListener("mouseleave", scheduleToolHoverHide);
    dom.settingsButton.addEventListener("click", openSettingsModal);
    dom.helpButton.addEventListener("click", openHelpModal);
    if (dom.utilityMenuButton && dom.utilityMenu) {
      dom.utilityMenuButton.addEventListener("click", toggleUtilityMenu);
      dom.utilityMenu.addEventListener("click", handleUtilityMenuAction);
      document.addEventListener("click", closeUtilityMenuFromOutside);
    }
    dom.taskBookButton.addEventListener("click", openTaskBookModal);
    dom.topTalentButton.addEventListener("click", openTalentModal);
    dom.challengeToggleButton.addEventListener("click", toggleChallengePanel);
    dom.resetButton.addEventListener("click", resetGame);
    dom.rewardConfirmButton.addEventListener("click", closeRewardModal);
    if (dom.watchAdSpinsButton) {
      dom.watchAdSpinsButton.addEventListener("click", grantAdStageSpins);
    }
    if (dom.restartStageButton) {
      dom.restartStageButton.addEventListener("click", restartCurrentStage);
    }
    if (dom.watchAdCoinsButton) {
      dom.watchAdCoinsButton.addEventListener("click", grantAdCoins);
    }
    if (dom.exchangeStardustButton) {
      dom.exchangeStardustButton.addEventListener("click", exchangeStardustForCoins);
    }
    dom.jackpotConfirmButton.addEventListener("click", closeJackpotModal);
    dom.toolDetailUseButton.addEventListener("click", useSelectedToolFromDetail);
    dom.activeSkillButton.addEventListener("click", onActiveSkillButtonClick);
    dom.skillDock.addEventListener("click", onSkillDockClick);
    dom.skillSwitchButton.addEventListener("click", toggleSkillPanel);
    dom.board.addEventListener("pointerdown", onPointerDown);
    dom.board.addEventListener("pointermove", onPointerMove);
    dom.board.addEventListener("pointerup", onPointerUp);
    dom.board.addEventListener("pointercancel", onPointerCancel);
    dom.board.addEventListener("mouseover", onGemHoverIn);
    dom.board.addEventListener("mouseout", onGemHoverOut);
    document.addEventListener("click", onDocumentClick);
    document.querySelectorAll("[data-start-mode]").forEach((button) => {
      button.addEventListener("click", () => selectStartMode(button.dataset.startMode));
    });
    document.querySelectorAll("[data-mode]").forEach((button) => {
      button.addEventListener("click", () => switchMode(button.dataset.mode));
    });
    document.querySelectorAll("[data-quality]").forEach((button) => {
      button.addEventListener("click", () => setQuality(button.dataset.quality));
    });
    document.querySelectorAll("[data-haptic-strength]").forEach((button) => {
      button.addEventListener("click", () => setHapticStrength(button.dataset.hapticStrength));
    });
    document.querySelectorAll("[data-help-tab]").forEach((button) => {
      button.addEventListener("click", () => switchHelpTab(button.dataset.helpTab));
    });
    dom.undoButton.addEventListener("click", undoLastMerge);
    if (dom.soundToggle) {
      dom.soundToggle.addEventListener("change", () => setSoundEnabled(dom.soundToggle.checked));
    }
    if (dom.shakeToggle) {
      dom.shakeToggle.addEventListener("change", () => setShakeEnabled(dom.shakeToggle.checked));
    }
    if (dom.volumeSlider) {
      dom.volumeSlider.addEventListener("input", () => setSoundVolume(Number(dom.volumeSlider.value) / 100));
      dom.volumeSlider.addEventListener("change", () => playSound("button"));
    }
    if (dom.autoMergeToggle) {
      dom.autoMergeToggle.addEventListener("change", () => setAutoMergeEnabled(dom.autoMergeToggle.checked));
    }
    bindSettingsToggleFallback();
    if (dom.exitGameButton) {
      dom.exitGameButton.addEventListener("click", exitGameToTitle);
    }
    if (dom.resetDataButton) {
      dom.resetDataButton.addEventListener("click", resetGameFromSettings);
    }
    if (dom.agreeComplianceButton) {
      dom.agreeComplianceButton.addEventListener("click", acceptCompliance);
    }
    if (dom.complianceConsentCheckbox) {
      dom.complianceConsentCheckbox.addEventListener("change", updateComplianceConsentState);
    }
    document.querySelectorAll("[data-legal-open]").forEach((button) => {
      button.addEventListener("click", () => openLegalModal(button.dataset.legalOpen));
    });
    dom.tutorialSkipButton.addEventListener("click", finishTutorial);
    dom.tutorialNextButton.addEventListener("click", advanceTutorial);
    window.addEventListener("resize", onViewportChanged);
    window.addEventListener("orientationchange", onViewportChanged);
    document.addEventListener("visibilitychange", onVisibilityChanged);
    window.addEventListener("pagehide", pauseVisualEffects);
    window.addEventListener("pageshow", resumeVisualEffects);
    document.addEventListener("mouseover", onToolInfoHoverIn);
    document.addEventListener("mouseout", onToolInfoHoverOut);
    window.addEventListener("beforeunload", saveState);
    setupTextFitObserver();
  }

  function onToolInfoHoverIn(event) {
    const trigger = event.target.closest("[data-tool-info]");
    if (!trigger || trigger.contains(event.relatedTarget)) {
      return;
    }
    showToolHover(trigger, trigger.dataset.toolInfo);
  }

  function onToolInfoHoverOut(event) {
    const trigger = event.target.closest("[data-tool-info]");
    if (!trigger || trigger.contains(event.relatedTarget)) {
      return;
    }
    scheduleToolHoverHide();
  }

  function onGemHoverIn(event) {
    const gem = event.target.closest(".gem");
    if (!gem || !dom.board.contains(gem) || gem.contains(event.relatedTarget)) {
      return;
    }
    showGemHover(gem);
  }

  function onGemHoverOut(event) {
    const gem = event.target.closest(".gem");
    if (!gem || gem.contains(event.relatedTarget)) {
      return;
    }
    scheduleToolHoverHide();
  }

  function updateViewportLayoutState() {
    const width = window.innerWidth || document.documentElement.clientWidth || 960;
    const height = window.innerHeight || document.documentElement.clientHeight || 640;
    const shortEdge = Math.min(width, height);
    const longEdge = Math.max(width, height);
    const touch = navigator.maxTouchPoints > 1;
    const forcePortraitShell = !touch && width > height && height >= 520;
    const orientation = forcePortraitShell ? "portrait" : width >= height ? "landscape" : "portrait";
    let deviceClass = "desktop";
    if (forcePortraitShell) {
      deviceClass = "phone";
    } else if (touch && longEdge >= 1024 && shortEdge >= 700) {
      deviceClass = "tablet";
    } else if (touch || width <= 760) {
      deviceClass = shortEdge <= 390 || longEdge <= 760 ? "small-phone" : "phone";
    }

    document.documentElement.style.setProperty("--app-viewport-height", `${height}px`);
    document.body.dataset.deviceClass = deviceClass;
    document.body.dataset.orientation = orientation;
    document.body.classList.toggle("is-small-phone", deviceClass === "small-phone");
    document.body.classList.toggle("is-phone", deviceClass === "phone" || deviceClass === "small-phone");
    document.body.classList.toggle("is-tablet", deviceClass === "tablet");
    document.body.classList.toggle("is-landscape", orientation === "landscape");
    document.body.classList.toggle("is-portrait", orientation === "portrait");
    document.body.classList.toggle("force-portrait-shell", forcePortraitShell);
    syncSideHudHost(deviceClass, orientation);
  }

  function syncSideHudHost(deviceClass, orientation) {
    if (!dom.sideHud || !dom.playfield) {
      return;
    }

    const isPhonePortrait = orientation === "portrait"
      && (deviceClass === "phone" || deviceClass === "small-phone");
    const target = isPhonePortrait ? document.body : dom.playfield;
    dom.sideHud.classList.toggle("mobile-hud-root", isPhonePortrait);
    if (dom.sideHud.parentElement !== target) {
      target.appendChild(dom.sideHud);
    }
  }

  function onViewportChanged() {
    if (runtime.resizePerfTimer) {
      window.clearTimeout(runtime.resizePerfTimer);
    }
    document.body.classList.add("layout-transitioning");
    runtime.resizePerfTimer = window.setTimeout(() => {
      runtime.resizePerfTimer = null;
      updateViewportLayoutState();
      updatePerformanceProfile();
      applyFxSettings();
      scheduleTextFit();
      refreshTutorialSpotlight();
      window.setTimeout(() => document.body.classList.remove("layout-transitioning"), 180);
    }, 160);
  }

  function onVisibilityChanged() {
    if (document.hidden) {
      pauseVisualEffects();
    } else {
      resumeVisualEffects();
    }
  }

  function pauseVisualEffects() {
    perf.hidden = true;
    document.body.classList.add("fx-paused");
    clearTransientVisualEffects();
    pauseAudioExperience();
  }

  function resumeVisualEffects() {
    perf.hidden = false;
    document.body.classList.remove("fx-paused");
    updateViewportLayoutState();
    updatePerformanceProfile();
    scheduleTextFit();
    resumeAudioExperience();
  }

  function getButtonSoundType(button) {
    if (!button) {
      return "button";
    }
    if (button.id === "spinButton") {
      return "spinPress";
    }
    if (button.id === "topItemButton") {
      return isPortraitUi() ? "lock" : "panelOpen";
    }
    if (button.id === "undoButton") {
      return "undo";
    }
    if (button.id === "topTalentButton") {
      return "talent";
    }
    if (button.id === "activeSkillButton") {
      return (state.items.blast || 0) > 0 ? "skillCharge" : "skillDenied";
    }
    if (button.id === "autoMergeButton" || button.id === "resetButton") {
      return "reset";
    }
    if (button.matches(".modal-close, [data-close-modal], [data-blast-cancel]")) {
      return "panelClose";
    }
    if (button.id === "settingsButton" || button.id === "helpButton" || button.id === "itemButton" || button.matches("[data-legal-open]")) {
      return "panelOpen";
    }
    return "button";
  }

  function onDocumentClick(event) {
    const clickedButton = event.target.closest("button");
    if (clickedButton) {
      unlockAudioExperience();
      playSound(getButtonSoundType(clickedButton));
      if (!clickedButton.disabled) {
        triggerHaptic("tap");
        showFirstControlHint(clickedButton);
      }
    }

    const closeId = event.target.dataset.closeModal;
    if (closeId && dom[closeId]) {
      dom[closeId].hidden = true;
      updateBgmMix();
      if (closeId === "toolDetailModal") {
        runtime.selectedToolDetail = null;
      }
    }

    const toolInfo = event.target.closest("[data-tool-info]");
    if (toolInfo) {
      openToolDetail(toolInfo.dataset.toolInfo);
      return;
    }

    const tool = event.target.closest("[data-tool]");
    if (tool) {
      activateTool(tool.dataset.tool);
    }

    const skill = event.target.closest("[data-skill-equip]");
    if (skill) {
      equipActiveSkill(skill.dataset.skillEquip);
      return;
    }

    const talent = event.target.closest("[data-talent-unlock]");
    if (talent) {
      unlockTalent(talent.dataset.talentUnlock);
      return;
    }

    if (dom.skillPanel && !event.target.closest(".skill-dock")) {
      dom.skillPanel.hidden = true;
    }
  }

  function showFirstControlHint(button) {
    if (!button || !CONTROL_HINTS[button.id]) {
      return;
    }

    const hint = CONTROL_HINTS[button.id];
    state.seenControlTips = state.seenControlTips && typeof state.seenControlTips === "object" ? state.seenControlTips : {};
    if (state.seenControlTips[hint.key]) {
      return;
    }

    state.seenControlTips[hint.key] = true;
    saveState();
    setTip(hint.text);
  }

  function createDefaultState() {
    return {
      coins: 1200,
      freeSpins: 0,
      level: 1,
      targetLevel: 2,
      targetRequired: 3,
      targetProgress: 0,
      targetAltLevel: 0,
      targetAltRequired: 0,
      targetAltProgress: 0,
      challengeType: "merge",
      challengeRequired: 3,
      challengeProgress: 0,
      stageSpinQuotaLevel: 1,
      stageSpinsLeft: getInitialStageSpinQuota(1),
      stageChallengeSpinAwarded: false,
      stagePity: createDefaultStagePity(1),
      stardust: 0,
      talents: [],
      achievements: {},
      achievementProgress: {},
      tasks: createDefaultTaskState(),
      complianceAccepted: false,
      settings: createDefaultSettings(),
      currentMode: MODE_ADVENTURE,
      modeData: createDefaultModeData(),
      riftEnergy: 0,
      riftReady: false,
      voidBonusReady: false,
      jackpot: JACKPOT_INITIAL_VALUE,
      dailyElement: randomElementKey(),
      dailyElementDate: getDailyDateKey(),
      skills: createDefaultSkillState(),
      bestCombo: 0,
      items: {
        swap: 1,
        blast: 1,
        leap: 1,
        summon: 1
      },
      seenItems: {},
      seenGemLevels: {},
      seenControlTips: {},
      summonNext: false,
      manualUnlocked: false,
      board: createBoard(FILL_DROPS)
    };
  }

  function loadState() {
    const fallback = createDefaultState();
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) {
        return fallback;
      }

      const parsed = JSON.parse(raw);
      const merged = {
        ...fallback,
        ...parsed,
        items: {
          ...fallback.items,
          ...(parsed.items || {})
        },
        seenItems: {
          ...fallback.seenItems,
          ...(parsed.seenItems || {})
        },
        seenGemLevels: {
          ...fallback.seenGemLevels,
          ...(parsed.seenGemLevels || {})
        },
        seenControlTips: {
          ...fallback.seenControlTips,
          ...(parsed.seenControlTips || {})
        },
        skills: normalizeSkillState(parsed.skills || fallback.skills)
      };

      merged.coins = Number.isFinite(merged.coins) ? merged.coins : fallback.coins;
      merged.talents = normalizeTalents(merged.talents);
      merged.achievements = normalizeAchievements(merged.achievements);
      merged.achievementProgress = normalizeAchievementProgress(merged.achievementProgress, merged.achievements);
      merged.tasks = normalizeTaskState(merged.tasks);
      merged.complianceAccepted = Boolean(parsed.complianceAccepted);
      merged.settings = normalizeSettings(merged.settings);
      applyFxSettings(merged.settings);
      refreshTasks(merged);
      merged.currentMode = GAME_MODES[merged.currentMode] ? merged.currentMode : MODE_ADVENTURE;
      merged.modeData = normalizeModeData(merged.modeData, merged);
      merged.stardust = Math.max(0, Math.floor(Number(merged.stardust) || 0));
      merged.freeSpins = clampNumber(merged.freeSpins, 0, getFreeSpinCap(merged.talents));
      merged.level = Math.max(1, Math.floor(merged.level || 1));
      merged.targetProgress = Math.max(0, Math.floor(merged.targetProgress || 0));
      merged.targetAltProgress = Math.max(0, Math.floor(merged.targetAltProgress || 0));
      merged.challengeProgress = Math.max(0, Math.floor(merged.challengeProgress || 0));
      normalizeStagePressureState(merged);
      merged.riftEnergy = clampNumber(parsed.riftEnergy ?? parsed.luckyEnergy ?? merged.riftEnergy ?? 0, 0, RIFT_MAX);
      merged.riftReady = Boolean(parsed.riftReady ?? parsed.luckyReady ?? merged.riftReady);
      merged.voidBonusReady = Boolean(parsed.voidBonusReady ?? merged.voidBonusReady);
      if (merged.voidBonusReady) {
        merged.riftReady = false;
      }
      const savedJackpot = Number(parsed.jackpot ?? merged.jackpot ?? JACKPOT_INITIAL_VALUE);
      merged.jackpot = Number.isFinite(savedJackpot) ? Math.max(JACKPOT_INITIAL_VALUE, Math.floor(savedJackpot)) : JACKPOT_INITIAL_VALUE;
      refreshDailyElement(merged);
      merged.bestCombo = Math.max(0, Math.floor(merged.bestCombo || 0));
      merged.board = normalizeBoard(merged.board);
      nextGemId = getMaxGemId(merged.board) + 1;
      return merged;
    } catch (error) {
      return fallback;
    }
  }

  function saveState() {
    if (state && runtime && !runtime.voidBonusActive && state.modeData && GAME_MODES[state.currentMode]) {
      captureCurrentModeState();
    }

    if (runtime && runtime.voidBonusActive && runtime.voidSavedBoard) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify({
        ...state,
        board: runtime.voidSavedBoard,
        manualUnlocked: runtime.voidSavedManualUnlocked,
        voidBonusReady: false,
        riftReady: false,
        riftEnergy: 0
      }));
      return;
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
  }

  function resetGame() {
    if (!window.confirm("Resetting will clear the current local save.")) {
      return;
    }

    localStorage.removeItem(STORAGE_KEY);
    nextGemId = 1;
    state = createDefaultState();
    runtime = createRuntime();
    applyLevelConfig();
    applyFxSettings();
    render();
    updateComplianceGate();
    startTutorialIfNeeded();
    setTip("Save data reset.");
  }

  function resetCurrentBoardLayout(event) {
    if (event) {
      event.stopPropagation();
    }
    if (runtime.animating) {
      return;
    }
    if (!isAdventureMode()) {
      setTip("Board Reset is only available in Campaign Mode.");
      return;
    }
    if (!window.confirm("Reset the current board layout? Stage progress and save data stay unchanged.")) {
      return;
    }

    runtime.toolMode = null;
    runtime.skillMode = null;
    runtime.selected = null;
    runtime.toolSelection = null;
    state.board = createBoard(FILL_DROPS);
    render();
    setTip("Current board layout reset. Stage progress was kept.");
  }

  function normalizeBoard(board) {
    const nextBoard = [];
    for (let r = 0; r < ROWS; r += 1) {
      const row = [];
      for (let c = 0; c < COLS; c += 1) {
        const symbol = Array.isArray(board) && Array.isArray(board[r]) ? board[r][c] : null;
        row.push(normalizeSymbol(symbol) || createDrop(FILL_DROPS));
      }
      nextBoard.push(row);
    }
    return nextBoard;
  }

  function cloneBoard(board) {
    return board.map((row) => row.map((symbol) => symbol ? { ...symbol } : null));
  }

  function normalizeSymbol(symbol) {
    if (!symbol || typeof symbol !== "object") {
      return null;
    }

    if (symbol.kind === "special" && SPECIAL_SYMBOLS[symbol.special]) {
      return {
        id: Number.isFinite(symbol.id) ? symbol.id : nextGemId++,
        kind: "special",
        special: symbol.special,
        level: 0,
        color: "special"
      };
    }

    if (Number.isFinite(symbol.level) && symbol.level >= 1 && symbol.level <= 5) {
      return {
        id: Number.isFinite(symbol.id) ? symbol.id : nextGemId++,
        kind: "gem",
        level: Math.floor(symbol.level),
        color: COLORS.includes(symbol.color) ? symbol.color : COLORS[Math.floor(Math.random() * COLORS.length)],
        element: normalizeElementKey(symbol.element)
      };
    }

    return null;
  }

  function ensureBoard() {
    state.board = normalizeBoard(state.board);
  }

  function applyLevelConfig() {
    const config = getLevelConfig(state.level);
    const challenge = getChallengeConfig(state.level);
    const previousChallengeType = state.challengeType;
    state.targetLevel = config.targetLevel;
    state.targetRequired = config.required;
    state.targetProgress = Math.min(state.targetProgress || 0, state.targetRequired);
    state.targetAltLevel = config.altLevel || 0;
    state.targetAltRequired = config.altRequired || 0;
    state.targetAltProgress = state.targetAltRequired > 0 ? Math.min(state.targetAltProgress || 0, state.targetAltRequired) : 0;
    state.challengeType = challenge.type;
    state.challengeRequired = challenge.required;
    state.challengeProgress = previousChallengeType === challenge.type ? Math.min(state.challengeProgress || 0, state.challengeRequired) : 0;
    normalizeStagePressureState(state);
  }

  function getLevelConfig(level) {
    const chapterLevel = ((level - 1) % 10) + 1;
    const targetLevel = chapterLevel === 10 ? 5 : Math.min(5, 2 + Math.floor((level - 1) / 3));
    const baseRequired = 2 + ((level - 1) % 3) + Math.floor((level - 1) / 10);
    const tutorialRequired = level <= 5 ? Math.max(1, Math.ceil(baseRequired * 0.7)) : baseRequired;
    const required = level >= 11 ? Math.ceil(tutorialRequired * 1.5) : tutorialRequired;
    const altLevel = level >= 16 ? Math.max(2, targetLevel - 1) : 0;
    const altRequired = altLevel ? Math.max(1, Math.ceil(required * 0.5)) : 0;
    return { targetLevel, required, altLevel, altRequired };
  }

  function getChallengeConfig(level) {
    const cycle = (level - 1) % 3;
    const step = Math.floor((level - 1) / 6);
    const difficultyBoost = level >= 11 ? 1.5 : 1;
    if (cycle === 0) {
      return { type: "merge", required: Math.ceil((3 + step) * difficultyBoost) };
    }
    if (cycle === 1) {
      return { type: "combo", required: Math.ceil((5 + step) * difficultyBoost) };
    }
    return { type: "clear", required: Math.ceil((8 + step * 2) * difficultyBoost) };
  }

  function getSpinCost() {
    const baseCost = 20 + Math.floor((state.level - 1) / 2) * 3;
    return isAdventureMode() && state.level <= 5 ? Math.max(8, Math.round(baseCost * 0.85)) : baseCost;
  }

  function getInitialStageSpinQuota(level) {
    if (level <= 5) {
      return 25;
    }
    if (level <= 15) {
      return 20;
    }
    return 15;
  }

  function getStageCoinContinueCost(level) {
    const targetLevel = level || state.level;
    if (targetLevel <= 5) {
      return 500;
    }
    if (targetLevel <= 15) {
      return 1200;
    }
    return 2500;
  }

  function createDefaultStagePity(level) {
    return {
      level: Math.max(1, Math.floor(level || 1)),
      noTier3: 0,
      noTier4: 0,
      triggers: 0
    };
  }

  function normalizeStagePressureState(targetState) {
    const level = Math.max(1, Math.floor(targetState.level || 1));
    if (targetState.stageSpinQuotaLevel !== level) {
      targetState.stageSpinQuotaLevel = level;
      targetState.stageSpinsLeft = getInitialStageSpinQuota(level);
      targetState.stageChallengeSpinAwarded = false;
      targetState.stagePity = createDefaultStagePity(level);
      return;
    }

    targetState.stageSpinsLeft = Math.max(0, Math.floor(Number(targetState.stageSpinsLeft) || 0));
    targetState.stageChallengeSpinAwarded = Boolean(targetState.stageChallengeSpinAwarded);
    const pity = targetState.stagePity && typeof targetState.stagePity === "object" ? targetState.stagePity : {};
    targetState.stagePity = {
      level,
      noTier3: Math.max(0, Math.floor(Number(pity.noTier3) || 0)),
      noTier4: Math.max(0, Math.floor(Number(pity.noTier4) || 0)),
      triggers: clampNumber(Math.floor(Number(pity.triggers) || 0), 0, STAGE_PITY_TRIGGER_LIMIT)
    };
  }

  function contributeToJackpot(spinCost) {
    const contribution = Math.max(1, Math.round(spinCost * JACKPOT_SPIN_CONTRIBUTION_RATE));
    state.jackpot += contribution;
  }

  function grantStardust(amount, reason, cell, skipTrack) {
    const value = Math.max(0, Math.floor(amount || 0));
    if (value <= 0 || !isAdventureMode()) {
      return;
    }

    state.stardust += value;
    if (dom.stardustValue) {
      dom.stardustValue.textContent = formatNumber(state.stardust);
    }
    if (dom.talentStardustValue) {
      dom.talentStardustValue.textContent = formatNumber(state.stardust);
    }
    showFloatText(`${reason} +${formatNumber(value)} Stardust`, cell || null, "stardust");
    if (!skipTrack) {
      trackEvent("stardustEarned", { amount: value }, cell || null);
    }
  }

  function grantAchievement(key, cell) {
    if (!isAdventureMode()) {
      return;
    }

    completeAchievement(key, cell);
  }

  function trackEvent(type, payload, cell) {
    if (!isAdventureMode()) {
      return;
    }

    const data = payload || {};
    let changed = false;

    changed = updateTaskProgress(type, data, cell) || changed;
    changed = updateAchievementProgress(type, data, cell) || changed;

    if (changed) {
      if (dom.taskBookModal && !dom.taskBookModal.hidden) {
        renderTaskBook();
      }
      renderHud();
      saveState();
    }
  }

  function updateTaskProgress(type, payload, cell) {
    refreshTasks(state);
    let changed = false;
    const lists = [state.tasks.daily, state.tasks.weekly ? [state.tasks.weekly] : []];

    lists.forEach((tasks) => {
      tasks.forEach((task) => {
        if (task.completed) {
          return;
        }

        const delta = getProgressDelta(task, type, payload);
        if (delta <= 0) {
          return;
        }

        task.progress = Math.min(task.target, task.progress + delta);
        changed = true;
        if (task.progress >= task.target) {
          completeTask(task, cell);
        }
      });
    });

    return changed;
  }

  function completeTask(task, cell) {
    if (task.completed) {
      return;
    }

    task.completed = true;
    task.rewarded = true;
    const reward = task.reward || {};
    if (reward.coins) {
      state.coins += reward.coins;
    }
    if (reward.freeSpins) {
      grantFreeSpins(reward.freeSpins);
    }
    if (reward.randomItem) {
      const item = randomItemKey();
      state.items[item] += reward.randomItem;
      revealItemIfNeeded(item);
    }
    if (reward.item && state.items[reward.item] !== undefined) {
      state.items[reward.item] += 1;
      revealItemIfNeeded(reward.item);
    }
    showFloatText(`Task Complete: ${task.title}`, cell || null, "stardust");
    setTip(`${task.title} completed. Rewards claimed.`);
  }

  function updateAchievementProgress(type, payload, cell) {
    let changed = false;
    Object.keys(ACHIEVEMENTS).forEach((key) => {
      if (state.achievements[key]) {
        return;
      }

      const achievement = ACHIEVEMENTS[key];
      const delta = getProgressDelta(achievement, type, payload);
      if (delta <= 0) {
        return;
      }

      state.achievementProgress[key] = Math.min(achievement.target, (state.achievementProgress[key] || 0) + delta);
      changed = true;
      if (state.achievementProgress[key] >= achievement.target) {
        completeAchievement(key, cell);
      }
    });
    return changed;
  }

  function completeAchievement(key, cell) {
    if (!isAdventureMode()) {
      return false;
    }

    const achievement = ACHIEVEMENTS[key];
    if (!achievement || state.achievements[key]) {
      return false;
    }

    state.achievements[key] = true;
    state.achievementProgress[key] = achievement.target;
    grantStardust(achievement.stardust, achievement.text, cell, true);
    showFloatText(`Achievement Unlocked: ${achievement.name}`, cell || null, "stardust");
    return true;
  }

  function getProgressDelta(definition, type, payload) {
    const amount = Math.max(1, Math.floor(payload.amount || 1));
    if (definition.type === "spin" && type === "spin") {
      return amount;
    }
    if (definition.type === "mergeAny" && type === "merge") {
      return amount;
    }
    if (definition.type === "mergeLevel" && type === "merge" && payload.level === definition.level) {
      return amount;
    }
    if (definition.type === "comboThreshold" && type === "comboThreshold" && payload.combo === definition.threshold) {
      return amount;
    }
    if (definition.type === "toolUse" && type === "toolUse") {
      return amount;
    }
    if (definition.type === "clear" && type === "clear") {
      return amount;
    }
    if (definition.type === "lineWin" && type === "lineWin") {
      return amount;
    }
    if (definition.type === "goldCoreWin" && type === "goldCoreWin") {
      return amount;
    }
    if (definition.type === "bomb" && type === "bomb") {
      return amount;
    }
    if (definition.type === "levelComplete" && type === "levelComplete") {
      return amount;
    }
    if (definition.type === "chapterComplete" && type === "chapterComplete") {
      return amount;
    }
    if (definition.type === "jackpotBreak" && type === "jackpotBreak") {
      return amount;
    }
    if (definition.type === "voidBonusComplete" && type === "voidBonusComplete") {
      return amount;
    }
    if (definition.type === "talentUnlock" && type === "talentUnlock") {
      return amount;
    }
    if (definition.type === "stardustEarned" && type === "stardustEarned") {
      return payload.amount || 0;
    }
    return 0;
  }

  function awardCoins(amount) {
    if (amount <= 0) {
      return;
    }

    if (runtime.rewardBuffer !== null) {
      runtime.rewardBuffer += amount;
      return;
    }

    applyCoinAward(amount);
  }

  function applyCoinAward(amount) {
    if (isScoreMode()) {
      addModeScore(amount);
      return;
    }
    if (isZenMode()) {
      return;
    }
    if (runtime.voidBonusActive) {
      runtime.voidBonusTotal += amount;
      renderRiftHud();
      return;
    }

    state.coins += amount;
    playSound("coin", { layers: Math.min(4, 1 + Math.floor(Math.log10(Math.max(1, amount)))) });
  }

  function beginRewardBuffer() {
    runtime.rewardBuffer = 0;
  }

  function flushRewardBuffer() {
    const buffered = runtime.rewardBuffer || 0;
    runtime.rewardBuffer = null;
    if (buffered > 0) {
      applyCoinAward(buffered);
      renderHud();
    }
  }

  function getChapter() {
    return Math.floor((state.level - 1) / 10) + 1;
  }

  function getChapterLevel() {
    return ((state.level - 1) % 10) + 1;
  }

  function getRewardPreview() {
    const coins = 120 + state.level * 20;
    const freeSpins = getChapterLevel() === 10 ? 3 : 1;
    return `+${formatNumber(coins)} Coins +${freeSpins} Free Spins`;
  }

  function render() {
    renderBoard();
    renderHud();
    updateButtons();
    updatePortraitShortcutLabels();
    saveState();
    scheduleTextFit();
  }

  function isPortraitUi() {
    return window.matchMedia && window.matchMedia("(orientation: portrait)").matches;
  }

  function updatePortraitShortcutLabels() {
    const portrait = isPortraitUi();
    dom.topItemButton.title = "Items";
    dom.topItemButton.setAttribute("aria-label", "Open Items");
    dom.autoMergeButton.title = portrait ? "Reset Board" : "Auto Merge";
    dom.autoMergeButton.setAttribute("aria-label", portrait ? "Reset Board" : "Auto Merge");
    dom.activeSkillButton.title = portrait ? `Global Crystal Blast, ${state.items.blast || 0} charges` : "Ability";
    dom.activeSkillButton.setAttribute("aria-label", portrait ? "Use Global Crystal Blast" : "Use Ability");
  }

  function renderHud() {
    refreshTasks(state);
    const chapter = getChapter();
    const chapterLevel = getChapterLevel();

    dom.coinValue.textContent = formatNumber(state.coins);
    dom.freeSpinValue.textContent = String(state.freeSpins);
    dom.stageValue.textContent = isAdventureMode() ? `${chapter}-${chapterLevel}` : GAME_MODES[state.currentMode].short;
    dom.stardustValue.textContent = formatNumber(state.stardust);
    dom.jackpotValue.textContent = formatNumber(state.jackpot);
    if (dom.dailyElementValue) {
      const dailyElement = ELEMENTS[state.dailyElement] || ELEMENTS.void;
      dom.dailyElementValue.textContent = dailyElement.short;
      dom.dailyElementValue.className = `element-label element-${state.dailyElement}`;
    }
    renderMissionHud(chapterLevel);
    if (dom.taskBookButton) {
      const summary = getTaskCompletionSummary();
      dom.taskBookButton.textContent = `${summary.done}/${summary.total} Tasks`;
    }

    dom.swapCount.textContent = state.items.swap;
    dom.blastCount.textContent = state.items.blast;
    dom.leapCount.textContent = state.items.leap;
    dom.summonCount.textContent = state.items.summon;

    renderComboHud();
    renderRiftHud();
    renderSkillHud();
    renderSettingsHud();
    renderChallengeToggle();
    renderTalentHud();
    renderTaskBookHud();
    renderModeHud();

    document.querySelectorAll("[data-tool]").forEach((button) => {
      const tool = button.dataset.tool;
      button.disabled = !isAdventureMode() || state.items[tool] <= 0 || runtime.animating;
    });
    document.querySelectorAll("[data-tool-info]").forEach((button) => {
      const tool = button.dataset.toolInfo;
      const detail = ITEM_DETAILS[tool];
      if (!detail) {
        return;
      }
      button.disabled = !isAdventureMode() || runtime.animating;
      button.title = `${ITEM_LABELS[tool]}: ${detail.short}`;
      button.setAttribute("aria-label", `${ITEM_LABELS[tool]}, ${detail.short}`);
    });
  }

  function renderMissionHud(chapterLevel) {
    if (isAdventureMode()) {
      const primaryProgress = Math.min(state.targetProgress, state.targetRequired);
      const hasAltTarget = state.targetAltLevel > 0 && state.targetAltRequired > 0;
      const altProgress = hasAltTarget ? Math.min(state.targetAltProgress, state.targetAltRequired) : 0;
      const progressPercent = hasAltTarget
        ? Math.min(100, ((primaryProgress + altProgress) / (state.targetRequired + state.targetAltRequired)) * 100)
        : Math.min(100, (primaryProgress / state.targetRequired) * 100);
      const challengePercent = Math.min(100, (state.challengeProgress / state.challengeRequired) * 100);
      const quotaText = `Stage Spins Left ${formatNumber(state.stageSpinsLeft)}`;
      const spendingFreeSpin = state.voidBonusReady || state.freeSpins > 0;
      const coinContinueCost = getStageCoinContinueCost();
      dom.levelTypeLabel.textContent = chapterLevel === 10 ? "Chapter Chest Stage" : "Standard Stage";
      dom.taskTitle.textContent = hasAltTarget ? `Objective: Merge ${LEVELS[state.targetLevel].name} + ${LEVELS[state.targetAltLevel].name}` : `Objective: Merge ${LEVELS[state.targetLevel].name}`;
      dom.rewardPreview.textContent = getRewardPreview();
      dom.taskProgressBar.style.width = `${progressPercent}%`;
      dom.taskProgressText.textContent = hasAltTarget ? `${primaryProgress}/${state.targetRequired} · ${altProgress}/${state.targetAltRequired}` : `${primaryProgress} / ${state.targetRequired}`;
      if (dom.stageSpinQuotaText) {
        dom.stageSpinQuotaText.textContent = quotaText;
        dom.stageSpinQuotaText.title = "Stage spins will deplete each normal spin, run out to fail the stage.";
      }
      dom.spinCostText.textContent = spendingFreeSpin ? `${quotaText} · Free Spins spend first` : state.stageSpinsLeft > 0 ? quotaText : `Stage Spins Left 0 · Coin Continue ${formatNumber(coinContinueCost)}`;
      dom.spinButtonCost.textContent = state.voidBonusReady ? "Void" : state.freeSpins > 0 ? "Free" : state.stageSpinsLeft > 0 ? `${formatNumber(state.stageSpinsLeft)} Left` : `${formatNumber(coinContinueCost)}`;
      updateSpinQuotaBadge(dom.spinButtonCost.textContent, state.stageSpinsLeft <= 5 && !spendingFreeSpin);
      updateSpinButtonLabel(spendingFreeSpin ? "Free" : "SPIN", spendingFreeSpin);
      dom.challengeTitle.textContent = getChallengeTitle();
      dom.challengeProgressBar.style.width = `${challengePercent}%`;
      dom.challengeProgressText.textContent = `${state.challengeProgress} / ${state.challengeRequired}`;
      dom.challengeBadge.textContent = state.challengeProgress >= state.challengeRequired ? `Completed +${STAGE_QUOTA_BONUS_SPINS} Stage Spins` : `Bonus +${STAGE_QUOTA_BONUS_SPINS} Stage Spins`;
      return;
    }

    const data = getModeData(state.currentMode);
    if (dom.stageSpinQuotaText) {
      dom.stageSpinQuotaText.textContent = "Stage Spins Left --";
      dom.stageSpinQuotaText.title = "";
    }
    dom.levelTypeLabel.textContent = GAME_MODES[state.currentMode].title;
    dom.rewardPreview.textContent = isZenMode() ? "No rewards" : "Mode data is separate";
    dom.spinCostText.textContent = isTimedMode() ? `Timer ${formatTime(getTimedRemainingMs())}` : "No Cost";
    dom.spinButtonCost.textContent = isTimedMode() && !data.active ? "Start" : "Free";
    updateSpinQuotaBadge(dom.spinButtonCost.textContent, false);
    updateSpinButtonLabel(isTimedMode() && !data.active ? "SPIN" : "Free", !(isTimedMode() && !data.active));

    if (isEndlessMode()) {
      dom.taskTitle.textContent = "Objective: Chase High Score";
      dom.taskProgressBar.style.width = "100%";
      dom.taskProgressText.textContent = `Run ${formatNumber(data.score || 0)}`;
      dom.challengeTitle.textContent = "Best Combo";
      dom.challengeProgressBar.style.width = `${Math.min(100, ((data.bestCombo || 0) / 10) * 100)}%`;
      dom.challengeProgressText.textContent = `${data.bestCombo || 0} Combo`;
      dom.challengeBadge.textContent = `Best ${formatNumber(data.bestScore || 0)}`;
      return;
    }

    if (isTimedMode()) {
      const remaining = getTimedRemainingMs();
      dom.taskTitle.textContent = data.active ? "Objective: Timed Score" : "Objective: Start 2-Min Challenge";
      dom.taskProgressBar.style.width = `${Math.max(0, Math.min(100, (remaining / TIMED_MODE_DURATION_MS) * 100))}%`;
      dom.taskProgressText.textContent = formatTime(remaining);
      dom.challengeTitle.textContent = "Run Score";
      dom.challengeProgressBar.style.width = `${Math.min(100, ((data.score || 0) / 5000) * 100)}%`;
      dom.challengeProgressText.textContent = formatNumber(data.score || 0);
      dom.challengeBadge.textContent = `Best ${formatNumber(data.bestScore || 0)}`;
      return;
    }

    dom.taskTitle.textContent = "Objective: Free Merge";
    dom.taskProgressBar.style.width = "100%";
    dom.taskProgressText.textContent = "Unlimited";
    dom.challengeTitle.textContent = "Relaxed Play";
    dom.challengeProgressBar.style.width = "100%";
    dom.challengeProgressText.textContent = `${data.spins || 0} Spins`;
    dom.challengeBadge.textContent = "No Objective";
  }

  function updateSpinButtonLabel(label, isFree) {
    if (dom.spinButtonLabel) {
      dom.spinButtonLabel.textContent = label;
    }
    dom.spinButton.classList.toggle("free-ready", Boolean(isFree));
  }

  function updateSpinQuotaBadge(text, isWarning) {
    if (!dom.spinQuotaBadge) {
      return;
    }
    dom.spinQuotaBadge.textContent = text;
    dom.spinQuotaBadge.classList.toggle("warning", Boolean(isWarning));
  }

  function toggleUtilityMenu(event) {
    event.stopPropagation();
    if (!dom.utilityMenu || !dom.utilityMenuButton) {
      return;
    }
    const willOpen = dom.utilityMenu.hidden;
    dom.utilityMenu.hidden = !willOpen;
    dom.utilityMenuButton.classList.toggle("menu-open", willOpen);
  }

  function handleUtilityMenuAction(event) {
    const button = event.target.closest("[data-utility-action]");
    if (!button) {
      return;
    }
    const action = button.dataset.utilityAction;
    dom.utilityMenu.hidden = true;
    dom.utilityMenuButton.classList.remove("menu-open");
    if (action === "settings") {
      openSettingsModal();
    }
    if (action === "help") {
      openHelpModal();
    }
  }

  function closeUtilityMenuFromOutside(event) {
    if (!dom.utilityMenu || dom.utilityMenu.hidden) {
      return;
    }
    if (dom.utilityMenu.contains(event.target) || dom.utilityMenuButton.contains(event.target)) {
      return;
    }
    dom.utilityMenu.hidden = true;
    dom.utilityMenuButton.classList.remove("menu-open");
  }

  function renderChallengeToggle() {
    const taskPanel = document.querySelector(".task-panel");
    if (!taskPanel || !dom.challengeToggleButton) {
      return;
    }

    taskPanel.classList.toggle("challenge-open", runtime.challengeOpen);
    dom.challengeToggleButton.textContent = runtime.challengeOpen ? "×" : "⚑";
    dom.challengeToggleButton.title = runtime.challengeOpen ? "Hide Side Challenge" : "View Side Challenge";
    dom.challengeToggleButton.setAttribute("aria-label", runtime.challengeOpen ? "Hide Side Challenge" : "View Side Challenge");
    dom.challengeToggleButton.classList.toggle("active", runtime.challengeOpen);
  }

  function toggleChallengePanel() {
    runtime.challengeOpen = !runtime.challengeOpen;
    renderChallengeToggle();
  }

  function renderModeHud() {
    const mode = GAME_MODES[state.currentMode] || GAME_MODES[MODE_ADVENTURE];
    dom.modeKicker.textContent = "Current Mode";
    dom.modeTitle.textContent = mode.title;
    dom.modeDescription.textContent = mode.desc;
    document.querySelectorAll("[data-mode]").forEach((button) => {
      button.classList.toggle("active", button.dataset.mode === state.currentMode);
      button.disabled = runtime.animating || (isTimedMode() && getModeData(MODE_TIMED).active && button.dataset.mode !== MODE_TIMED);
    });
    document.body.dataset.mode = state.currentMode;

    const data = getModeData(state.currentMode);
    if (isAdventureMode()) {
      dom.modeStats.innerHTML = `<span>Stage ${getChapter()}-${getChapterLevel()}</span><span>Stage Spins ${formatNumber(state.stageSpinsLeft)}</span><span>Coins ${formatNumber(state.coins)}</span>`;
      renderModeLeaderboard([]);
      return;
    }
    if (isEndlessMode()) {
      dom.modeStats.innerHTML = `<span>Run ${formatNumber(data.score || 0)}</span><span>Best ${formatNumber(data.bestScore || 0)}</span><span>Best Combo ${data.bestCombo || 0}</span>`;
      renderModeLeaderboard(data.leaderboard || []);
      return;
    }
    if (isTimedMode()) {
      const reward = data.rewards || { freeSpins: 0, items: {} };
      dom.modeStats.innerHTML = `<span>Timer ${formatTime(getTimedRemainingMs())}</span><span>Run ${formatNumber(data.score || 0)}</span><span>Mode Reward ${reward.freeSpins || 0}</span>`;
      renderModeLeaderboard(data.leaderboard || []);
      return;
    }
    dom.modeStats.innerHTML = `<span>No Cost</span><span>Spins ${data.spins || 0}</span><span>No Rewards</span>`;
    renderModeLeaderboard([]);
  }

  function renderModeLeaderboard(entries) {
    if (!entries || entries.length === 0) {
      dom.modeLeaderboard.hidden = true;
      dom.modeLeaderboard.innerHTML = "";
      return;
    }
    dom.modeLeaderboard.hidden = false;
    dom.modeLeaderboard.innerHTML = `<strong>Local Leaderboard</strong>${entries.map((entry, index) => `<span>#${index + 1} ${formatNumber(entry.score)} pts · ${entry.combo || 0} Combo</span>`).join("")}`;
  }

  function renderComboHud() {
    const bestCombo = isScoreMode() ? (getModeData(state.currentMode).bestCombo || 0) : state.bestCombo;
    dom.comboValue.textContent = String(runtime.combo);
    dom.multiplierValue.textContent = `×${getComboMultiplier().toFixed(2)}`;
    dom.bestComboValue.textContent = `BEST ${bestCombo}`;
    if (dom.sideComboValue) {
      dom.sideComboValue.textContent = String(runtime.combo);
    }
    if (dom.sideMultiplierValue) {
      dom.sideMultiplierValue.textContent = `×${getComboMultiplier().toFixed(2)}`;
    }
  }

  function renderRiftHud() {
    const percent = runtime.voidBonusActive || state.riftReady || state.voidBonusReady ? 100 : Math.min(100, state.riftEnergy);
    const energyText = `${Math.floor(percent)} / ${RIFT_MAX}`;
    dom.riftEnergyBar.style.width = `${percent}%`;
    dom.riftEnergyText.textContent = runtime.voidBonusActive ? `Void Realm ${runtime.voidBonusRound} / ${VOID_BONUS_ROUNDS}` : state.voidBonusReady ? `Void Realm Ready ${energyText}` : runtime.riftSpinRound ? `Neon Rift Active ${energyText}` : state.riftReady ? `Neon Rift Ready ${energyText}` : energyText;
    if (dom.sideRiftEnergyBar) {
      dom.sideRiftEnergyBar.style.height = `${percent}%`;
    }
    if (dom.sideRiftEnergyText) {
      dom.sideRiftEnergyText.textContent = runtime.voidBonusActive ? `${runtime.voidBonusRound}/${VOID_BONUS_ROUNDS}` : state.voidBonusReady ? "VOID" : energyText.replace(/\s/g, "");
    }
    if (dom.voidBonusBadge && dom.voidBonusText) {
      const visible = runtime.voidBonusActive || state.voidBonusReady;
      dom.voidBonusBadge.hidden = !visible;
      dom.voidBonusText.textContent = runtime.voidBonusActive ? `Round ${runtime.voidBonusRound}/${VOID_BONUS_ROUNDS} · Banked ${formatNumber(runtime.voidBonusTotal)}` : "READY · 3 FREE SPINS";
    }
    document.body.classList.toggle("rift-critical", percent >= 80 && !state.riftReady && !state.voidBonusReady && !runtime.voidBonusActive);
    document.body.classList.toggle("rift-ready", state.riftReady);
    document.body.classList.toggle("rift-active", runtime.riftSpinRound);
    document.body.classList.toggle("void-ready", state.voidBonusReady);
    document.body.classList.toggle("void-active", runtime.voidBonusActive);
  }

  function renderBoard() {
    if (!runtime.drag) {
      clearMergePreview();
    }
    const selectedKey = runtime.selected ? cellKey(runtime.selected) : "";
    const toolKey = runtime.toolSelection ? cellKey(runtime.toolSelection) : "";
    const nearMergeKeys = getNearMergeHintKeys();
    const html = [];

    for (let r = 0; r < ROWS; r += 1) {
      for (let c = 0; c < COLS; c += 1) {
        const symbol = state.board[r][c];
        const key = `${r},${c}`;
        const classes = ["cell"];
        if (runtime.skillMode === "columnRedraw") {
          classes.push("skill-target");
        }
        if (key === selectedKey || key === toolKey) {
          classes.push("selected");
        }
        if (nearMergeKeys.has(key)) {
          classes.push("near-merge");
        }

        html.push(`<button class="${classes.join(" ")}" type="button" role="gridcell" data-r="${r}" data-c="${c}" ${runtime.animating ? "disabled" : ""}>`);
        if (symbol) {
          html.push(renderSymbol(symbol));
        }
        html.push("</button>");
      }
    }

    dom.board.innerHTML = html.join("");
    if (!document.body.classList.contains("legacy-smooth")) {
      scheduleTextFit(dom.board);
    }
  }

  function renderSymbol(symbol) {
    if (symbol.kind === "special") {
      const config = SPECIAL_SYMBOLS[symbol.special];
      return `<span class="gem special-symbol special-${symbol.special}" data-id="${symbol.id}" data-special="${symbol.special}" aria-label="${config.name}"><span class="special-mark" aria-hidden="true"></span></span>`;
    }

    const displayLevel = getEffectiveGemLevel(symbol);
    const elementKey = normalizeElementKey(symbol.element);
    const colorClass = displayLevel <= 3 ? ` color-${symbol.color}` : "";
    const elementClass = ` element-${elementKey}`;
    const label = LEVELS[displayLevel].name;
    return `<span class="gem level-${displayLevel}${colorClass}${elementClass}" data-id="${symbol.id}" data-gem-level="${displayLevel}" aria-label="${label} ${ELEMENTS[elementKey].name}"><span class="element-rim" aria-hidden="true"></span></span>`;
  }

  function getNearMergeHintKeys() {
    const keys = new Set();
    if (!state.manualUnlocked || runtime.animating || runtime.drag || findNextMergeGroup()) {
      return keys;
    }

    findNearMergeGroups().slice(0, 4).forEach((group) => {
      group.forEach((cell) => keys.add(cellKey(cell)));
    });
    return keys;
  }

  function findNearMergeGroups() {
    const groups = [];
    const visited = new Set();

    for (let r = ROWS - 1; r >= 0; r -= 1) {
      for (let c = 0; c < COLS; c += 1) {
        const cell = { r, c };
        for (const level of getCandidateMergeLevels(cell)) {
          const key = `${level}:${cellKey(cell)}`;
          if (visited.has(key)) {
            continue;
          }

          const group = collectConnected(cell, level, visited);
          if (group.length === 2) {
            groups.push(group);
          }
        }
      }
    }

    groups.sort(sortCascadeMergeGroups);
    return groups;
  }

  function updateButtons() {
    const canSpin = !runtime.animating;
    const portrait = isPortraitUi();
    dom.spinButton.disabled = !canSpin;
    dom.autoMergeButton.disabled = portrait ? runtime.animating : runtime.animating || !state.manualUnlocked || !findNextMergeGroup();
    dom.undoButton.disabled = runtime.animating || !runtime.undoSnapshot || runtime.undoUsed;
    dom.itemButton.disabled = runtime.animating;
    dom.topItemButton.disabled = runtime.animating || !isAdventureMode();
    dom.settingsButton.disabled = false;
    dom.helpButton.disabled = false;
    if (dom.utilityMenuButton) {
      dom.utilityMenuButton.disabled = runtime.animating;
    }
    dom.topTalentButton.disabled = runtime.animating;
    dom.taskBookButton.disabled = runtime.animating;
    dom.resetButton.disabled = runtime.animating;
    updateSkillButtonState();
  }

  function onAutoMergeShortcutClick(event) {
    if (isPortraitUi()) {
      resetCurrentBoardLayout(event);
      return;
    }
    autoMergeBoard();
  }

  function renderSettingsHud() {
    const settings = normalizeSettings(state.settings);
    state.settings = settings;
    dom.settingsButton.innerHTML = `<span aria-hidden="true">⚙</span>`;
    dom.settingsButton.title = "Settings";
    dom.settingsButton.setAttribute("aria-label", "Settings");
    if (dom.shakeToggle) {
      dom.shakeToggle.checked = settings.shake;
    }
    if (dom.soundToggle) {
      dom.soundToggle.checked = settings.sound;
    }
    if (dom.volumeSlider) {
      dom.volumeSlider.value = Math.round(settings.volume * 100);
    }
    if (dom.volumeValue) {
      dom.volumeValue.textContent = `${Math.round(settings.volume * 100)}%`;
    }
    if (dom.autoMergeToggle) {
      dom.autoMergeToggle.checked = settings.autoMerge;
    }
    if (dom.hapticStrengthValue) {
      dom.hapticStrengthValue.textContent = HAPTIC_STRENGTHS[settings.hapticStrength].label;
    }
    document.querySelectorAll("[data-quality]").forEach((button) => {
      button.classList.toggle("active", button.dataset.quality === settings.quality);
    });
    document.querySelectorAll("[data-haptic-strength]").forEach((button) => {
      button.classList.toggle("active", button.dataset.hapticStrength === settings.hapticStrength);
      button.setAttribute("aria-pressed", button.dataset.hapticStrength === settings.hapticStrength ? "true" : "false");
    });
  }

  function openSettingsModal() {
    renderSettingsHud();
    dom.settingsModal.hidden = false;
    scheduleTextFit(dom.settingsModal);
  }

  function openHelpModal() {
    switchHelpTab("rules");
    dom.helpModal.hidden = false;
    scheduleTextFit(dom.helpModal);
  }

  function updateComplianceGate() {
    if (!dom.complianceModal) {
      return;
    }
    dom.complianceModal.hidden = Boolean(state.complianceAccepted);
    document.body.classList.toggle("compliance-required", !state.complianceAccepted);
    if (!state.complianceAccepted) {
      updateComplianceConsentState();
      scheduleTextFit(dom.complianceModal);
    }
  }

  function updateComplianceConsentState() {
    if (!dom.agreeComplianceButton) {
      return;
    }

    const checked = Boolean(dom.complianceConsentCheckbox && dom.complianceConsentCheckbox.checked);
    dom.agreeComplianceButton.disabled = !checked;
    dom.agreeComplianceButton.classList.toggle("consent-ready", checked);
  }

  function acceptCompliance() {
    if (!dom.complianceConsentCheckbox || !dom.complianceConsentCheckbox.checked) {
      updateComplianceConsentState();
      setTip("Please check the agreement box before playing.");
      return;
    }

    state.complianceAccepted = true;
    saveState();
    updateComplianceGate();
    setTip("Terms accepted. Choose a mode to start playing.");
  }

  function openLegalModal(type) {
    const content = LEGAL_CONTENT[type] || LEGAL_CONTENT.terms;
    if (!dom.legalModal || !dom.legalModalTitle || !dom.legalModalBody) {
      return;
    }

    dom.legalModalTitle.textContent = content.title;
    dom.legalModalBody.innerHTML = content.sections.map((section) => (
      `<section class="legal-section"><h3>${section.heading}</h3><p>${section.body}</p></section>`
    )).join("");
    dom.legalModal.hidden = false;
    scheduleTextFit(dom.legalModal);
  }

  function switchHelpTab(tab) {
    const target = tab === "codex" ? "codex" : "rules";
    if (target === "codex") {
      renderGemCodex();
    }
    document.querySelectorAll("[data-help-tab]").forEach((button) => {
      button.classList.toggle("active", button.dataset.helpTab === target);
    });
    if (dom.helpRulesPanel) {
      dom.helpRulesPanel.classList.toggle("active", target === "rules");
    }
    if (dom.helpCodexPanel) {
      dom.helpCodexPanel.classList.toggle("active", target === "codex");
    }
  }

  function renderGemCodex() {
    if (!dom.gemCodexList) {
      return;
    }
    if (runtime.codexRendered) {
      return;
    }

    const gemCards = Object.keys(LEVELS).map((key) => {
      const level = Number(key);
      const levelInfo = LEVELS[level];
      const effect = GEM_EFFECTS[level];
      return `<article class="gem-codex-card gem-codex-level-${level}"><div class="codex-gem-icon codex-live-symbol level-${level}" aria-hidden="true"><span class="gem level-${level}"><span class="element-rim"></span></span></div><div><strong>${levelInfo.name}</strong><span>Line Multiplier x${levelInfo.multiplier}</span><p>${effect.path}</p><p>${effect.detail}</p></div></article>`;
    }).join("");

    const specialCards = Object.keys(SPECIAL_EFFECTS).map((key) => {
      const effect = SPECIAL_EFFECTS[key];
      return `<article class="gem-codex-card special-codex-card"><div class="codex-gem-icon codex-live-symbol special-${key}" aria-hidden="true"><span class="gem special-symbol special-${key}"><span class="special-mark" aria-hidden="true"></span></span></div><div><strong>${effect.name}</strong><span>Special Symbol</span><p>${effect.detail}</p></div></article>`;
    }).join("");

    dom.gemCodexList.innerHTML = `<section><h3>Tier1-Tier5 Crystals</h3>${gemCards}</section><section><h3>Special Symbols</h3>${specialCards}</section>`;
    runtime.codexRendered = true;
  }

  function setQuality(quality) {
    if (!QUALITY_PRESETS[quality]) {
      return;
    }

    state.settings.quality = quality;
    applyFxSettings();
    render();
    setTip(`Graphics set to ${QUALITY_PRESETS[quality].label}.`);
  }

  function setShakeEnabled(enabled) {
    state.settings.shake = Boolean(enabled);
    applyFxSettings();
    render();
    setTip(state.settings.shake ? "Vibration enabled." : "Vibration disabled.");
  }

  function setHapticStrength(strength) {
    if (!HAPTIC_STRENGTHS[strength]) {
      return;
    }

    state.settings.hapticStrength = strength;
    applyFxSettings();
    renderSettingsHud();
    saveState();
    triggerHaptic(strength === "high" ? "medium" : "tap", { force: true });
    setTip(`Vibration strength set to ${HAPTIC_STRENGTHS[strength].label}.`);
  }

  function setSoundEnabled(enabled) {
    state.settings.sound = Boolean(enabled);
    applyFxSettings();
    if (state.settings.sound) {
      unlockAudioExperience();
      playSound("button");
    }
    renderSettingsHud();
    saveState();
    setTip(state.settings.sound ? "Sound enabled." : "Sound disabled.");
  }

  function setSoundVolume(volume) {
    state.settings.volume = clampNumber(volume, 0, 1);
    applyFxSettings();
    renderSettingsHud();
    saveState();
  }

  function setAutoMergeEnabled(enabled) {
    state.settings.autoMerge = Boolean(enabled);
    applyFxSettings();
    renderSettingsHud();
    saveState();
    setTip(state.settings.autoMerge ? "Auto Merge enabled. Valid groups will merge after spins." : "Auto Merge disabled.");
  }

  function bindSettingsToggleFallback() {
    if (!dom.settingsModal) {
      return;
    }

    const toggleMap = new Map([
      [dom.soundToggle, setSoundEnabled],
      [dom.shakeToggle, setShakeEnabled],
      [dom.autoMergeToggle, setAutoMergeEnabled]
    ]);
    let lastToggleAt = 0;

    const activate = (event) => {
      const row = event.target && event.target.closest ? event.target.closest(".settings-toggle") : null;
      if (!row || !dom.settingsModal.contains(row)) {
        return;
      }

      const toggle = row.querySelector('input[type="checkbox"]');
      const setter = toggleMap.get(toggle);
      if (!setter) {
        return;
      }

      const now = performance.now();
      if (now - lastToggleAt < 120) {
        event.preventDefault();
        event.stopPropagation();
        return;
      }
      lastToggleAt = now;

      event.preventDefault();
      event.stopPropagation();
      const nextChecked = !toggle.checked;
      toggle.checked = nextChecked;
      setter(nextChecked);
    };

    dom.settingsModal.addEventListener("click", activate, true);
    dom.settingsModal.addEventListener("touchend", activate, true);
  }

  function exitGameToTitle() {
    captureCurrentModeState();
    saveState();
    playSound("exit");
    if (requestNativeExit()) {
      return;
    }

    window.close();
    setTip("Exit is only available in the app build. Returning to title.");
    document.body.classList.remove("game-started");
    if (dom.startScreen) {
      dom.startScreen.hidden = false;
    }
    if (dom.settingsModal) {
      dom.settingsModal.hidden = true;
    }
    stopModeTimer();
    runtime.selected = null;
    runtime.drag = null;
    runtime.toolMode = null;
    runtime.toolSelection = null;
    runtime.skillMode = null;
    runtime.skillLevelBoostActive = false;
    clearUndoState();
  }

  function returnToModeSelect() {
    if (runtime.animating) {
      return;
    }

    captureCurrentModeState();
    saveState();
    playSound("exit");
    setTip("Returned to mode select.");
    document.body.classList.remove("game-started");
    if (dom.startScreen) {
      dom.startScreen.hidden = false;
    }
    document.querySelectorAll(".modal-backdrop").forEach((modal) => {
      modal.hidden = true;
    });
    stopModeTimer();
    runtime.selected = null;
    runtime.drag = null;
    runtime.toolMode = null;
    runtime.toolSelection = null;
    runtime.skillMode = null;
    runtime.skillLevelBoostActive = false;
    clearUndoState();
    updateBgmMix();
  }

  function requestNativeExit() {
    const bridge = window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.nativeExit;
    if (!bridge || typeof bridge.postMessage !== "function") {
      return false;
    }

    try {
      bridge.postMessage({ action: "exit" });
      return true;
    } catch (error) {
      return false;
    }
  }

  function resetGameFromSettings() {
    if (!runtime.abandonSaveArmed) {
      runtime.abandonSaveArmed = true;
      if (dom.resetDataButton) {
        dom.resetDataButton.textContent = "Tap Again to Confirm";
        dom.resetDataButton.classList.add("confirming");
      }
      setTip("Tap Abandon Save again to permanently clear this local save.");
      if (runtime.abandonSaveTimer) {
        window.clearTimeout(runtime.abandonSaveTimer);
      }
      runtime.abandonSaveTimer = window.setTimeout(() => {
        runtime.abandonSaveArmed = false;
        runtime.abandonSaveTimer = null;
        if (dom.resetDataButton) {
          dom.resetDataButton.textContent = "Abandon Save";
          dom.resetDataButton.classList.remove("confirming");
        }
      }, 6000);
      return;
    }

    if (runtime.abandonSaveTimer) {
      window.clearTimeout(runtime.abandonSaveTimer);
      runtime.abandonSaveTimer = null;
    }
    runtime.abandonSaveArmed = false;
    dom.settingsModal.hidden = true;
    localStorage.removeItem(STORAGE_KEY);
    nextGemId = 1;
    state = createDefaultState();
    runtime = createRuntime();
    applyLevelConfig();
    applyFxSettings();
    render();
    updateComplianceGate();
    startTutorialIfNeeded();
    setTip("Save abandoned. A new game has started.");
  }

  function startTutorialIfNeeded() {
    if (
      !state.complianceAccepted
      || state.settings.tutorialVersion >= TUTORIAL_VERSION
      || !document.body.classList.contains("game-started")
    ) {
      return;
    }

    runtime.tutorialStep = 0;
    showTutorialStep();
  }

  function showTutorialStep() {
    const step = TUTORIAL_STEPS[runtime.tutorialStep];
    if (!step) {
      finishTutorial();
      return;
    }

    clearTutorialFocus();
    dom.tutorialStepLabel.textContent = `${runtime.tutorialStep + 1} / ${TUTORIAL_STEPS.length}`;
    dom.tutorialTitle.textContent = step.title;
    dom.tutorialText.textContent = step.text;
    dom.tutorialNextButton.hidden = step.advanceOn === "click";
    dom.tutorialNextButton.textContent = runtime.tutorialStep === TUTORIAL_STEPS.length - 1 ? "Done" : "Next";
    dom.tutorialOverlay.hidden = false;

    const target = document.querySelector(step.selector);
    if (target) {
      target.classList.add("tutorial-focus");
      runtime.tutorialTarget = target;
      if (step.advanceOn === "click") {
        runtime.tutorialTargetHandler = () => {
          window.setTimeout(() => {
            if (step.closeAfter) {
              const modal = document.getElementById(step.closeAfter);
              if (modal) {
                modal.hidden = true;
              }
            }
            advanceTutorial();
          }, 160);
        };
        target.addEventListener("click", runtime.tutorialTargetHandler, { once: true });
      }
      window.requestAnimationFrame(() => updateTutorialSpotlight(target));
    }
  }

  function advanceTutorial() {
    runtime.tutorialStep += 1;
    if (runtime.tutorialStep >= TUTORIAL_STEPS.length) {
      finishTutorial();
      return;
    }

    showTutorialStep();
  }

  function finishTutorial() {
    clearTutorialFocus();
    dom.tutorialOverlay.hidden = true;
    state.settings.tutorialDone = true;
    state.settings.tutorialVersion = TUTORIAL_VERSION;
    saveState();
    setTip("Tutorial complete.");
  }

  function clearTutorialFocus() {
    if (runtime.tutorialTarget && runtime.tutorialTargetHandler) {
      runtime.tutorialTarget.removeEventListener("click", runtime.tutorialTargetHandler);
    }
    runtime.tutorialTarget = null;
    runtime.tutorialTargetHandler = null;
    document.querySelectorAll(".tutorial-focus").forEach((element) => {
      element.classList.remove("tutorial-focus");
    });
  }

  function refreshTutorialSpotlight() {
    if (!dom.tutorialOverlay || dom.tutorialOverlay.hidden) {
      return;
    }
    const step = TUTORIAL_STEPS[runtime.tutorialStep];
    const target = step ? document.querySelector(step.selector) : null;
    if (target) {
      window.requestAnimationFrame(() => updateTutorialSpotlight(target));
    }
  }

  function updateTutorialSpotlight(target) {
    if (
      !dom.tutorialOverlay
      || !target
      || dom.tutorialShades.length !== 4
      || !dom.tutorialTargetRing
    ) {
      return;
    }

    const rect = target.getBoundingClientRect();
    const viewportWidth = window.innerWidth || document.documentElement.clientWidth;
    const viewportHeight = window.innerHeight || document.documentElement.clientHeight;
    const padding = 9;
    const left = Math.max(0, rect.left - padding);
    const top = Math.max(0, rect.top - padding);
    const right = Math.min(viewportWidth, rect.right + padding);
    const bottom = Math.min(viewportHeight, rect.bottom + padding);
    const width = Math.max(0, right - left);
    const height = Math.max(0, bottom - top);
    const [topShade, leftShade, rightShade, bottomShade] = dom.tutorialShades;

    Object.assign(topShade.style, {
      left: "0px",
      top: "0px",
      width: `${viewportWidth}px`,
      height: `${top}px`
    });
    Object.assign(leftShade.style, {
      left: "0px",
      top: `${top}px`,
      width: `${left}px`,
      height: `${height}px`
    });
    Object.assign(rightShade.style, {
      left: `${right}px`,
      top: `${top}px`,
      width: `${Math.max(0, viewportWidth - right)}px`,
      height: `${height}px`
    });
    Object.assign(bottomShade.style, {
      left: "0px",
      top: `${bottom}px`,
      width: `${viewportWidth}px`,
      height: `${Math.max(0, viewportHeight - bottom)}px`
    });
    Object.assign(dom.tutorialTargetRing.style, {
      left: `${left}px`,
      top: `${top}px`,
      width: `${width}px`,
      height: `${height}px`
    });

    if (dom.tutorialCard) {
      const edge = 14;
      const cardHeight = dom.tutorialCard.offsetHeight || 190;
      const placeAbove = top > viewportHeight * 0.5;
      let cardTop = placeAbove
        ? top - cardHeight - 18
        : bottom + 18;
      cardTop = Math.max(edge, Math.min(viewportHeight - cardHeight - edge, cardTop));
      dom.tutorialCard.style.top = `${cardTop}px`;
    }
  }

  function renderTaskBookHud() {
    if (dom.taskBookModal && !dom.taskBookModal.hidden) {
      renderTaskBook();
    }
  }

  function openTaskBookModal() {
    if (runtime.animating) {
      return;
    }

    refreshTasks(state);
    renderTaskBook();
    dom.taskBookModal.hidden = false;
    scheduleTextFit(dom.taskBookModal);
  }

  function renderTaskBook() {
    if (!dom.dailyTaskList || !dom.weeklyTaskList || !dom.achievementCodex) {
      return;
    }

    const achievementKeys = Object.keys(ACHIEVEMENTS);
    const achievementDone = achievementKeys.filter((key) => state.achievements[key]).length;
    dom.taskRefreshText.textContent = `Daily ${state.tasks.dailyDate} · Weekly ${state.tasks.weeklyKey} · Achievements ${achievementDone}/${achievementKeys.length}`;
    dom.dailyTaskList.innerHTML = state.tasks.daily.map(renderTaskCard).join("");
    dom.weeklyTaskList.innerHTML = state.tasks.weekly ? renderTaskCard(state.tasks.weekly) : "";
    dom.achievementCodex.innerHTML = achievementKeys.map((key) => renderAchievementCard(key)).join("");
  }

  function renderTaskCard(task) {
    const percent = Math.min(100, (task.progress / task.target) * 100);
    const reward = formatTaskReward(task.reward);
    return `<article class="task-entry${task.completed ? " completed" : ""}"><div class="task-entry-head"><strong>${task.title}</strong><span>${task.completed ? "Completed" : reward}</span></div><div class="progress-track"><div class="progress-fill" style="width:${percent}%"></div></div><div class="task-entry-foot"><span>${Math.min(task.progress, task.target)} / ${task.target}</span><em>${task.completed ? "Reward Claimed" : "In Progress"}</em></div></article>`;
  }

  function renderAchievementCard(key) {
    const achievement = ACHIEVEMENTS[key];
    const completed = Boolean(state.achievements[key]);
    const progress = completed ? achievement.target : Math.min(state.achievementProgress[key] || 0, achievement.target);
    const percent = Math.min(100, (progress / achievement.target) * 100);
    return `<article class="achievement-card${completed ? " unlocked" : ""}"><div class="achievement-badge">${completed ? "✓" : "?"}</div><strong>${achievement.name}</strong><span>${achievement.desc}</span><div class="progress-track"><div class="progress-fill" style="width:${percent}%"></div></div><small>${formatNumber(progress)} / ${formatNumber(achievement.target)} · +${formatNumber(achievement.stardust)} Stardust</small></article>`;
  }

  function formatTaskReward(reward) {
    const parts = [];
    if (reward.coins) {
      parts.push(`+${formatNumber(reward.coins)} Coins`);
    }
    if (reward.freeSpins) {
      parts.push(`+${reward.freeSpins} Free Spins`);
    }
    if (reward.randomItem) {
      parts.push("Random Item");
    }
    if (reward.item) {
      parts.push(`${ITEM_LABELS[reward.item]}×1`);
    }
    return parts.join(" ");
  }

  function getTaskCompletionSummary() {
    const tasks = [...state.tasks.daily, state.tasks.weekly].filter(Boolean);
    return {
      done: tasks.filter((task) => task.completed).length,
      total: tasks.length
    };
  }

  function renderTalentHud() {
    if (dom.talentStardustValue) {
      dom.talentStardustValue.textContent = formatNumber(state.stardust);
    }
    if (dom.talentTree && !dom.talentModal.hidden) {
      renderTalentTree();
    }
  }

  function openTalentModal() {
    if (runtime.animating) {
      return;
    }

    renderTalentTree();
    dom.talentModal.hidden = false;
    scheduleTextFit(dom.talentModal);
  }

  function renderTalentTree() {
    dom.talentStardustValue.textContent = formatNumber(state.stardust);
    dom.talentTree.innerHTML = TALENT_LINES.map((line) => {
      const nodes = line.nodes.map((node) => {
        const unlocked = hasTalent(node.key);
        const available = isTalentAvailable(node.key);
        const affordable = state.stardust >= node.cost;
        const classes = ["talent-node"];
        if (unlocked) {
          classes.push("unlocked");
        } else if (available) {
          classes.push("available");
        } else {
          classes.push("locked");
        }
        if (available && !affordable && !unlocked) {
          classes.push("unaffordable");
        }
        const disabled = unlocked || !available;
        const costText = unlocked ? "Unlocked" : `${formatNumber(node.cost)} Stardust`;
        return `<button class="${classes.join(" ")}" type="button" data-talent-unlock="${node.key}" ${disabled ? "disabled" : ""}><span></span><strong>${node.name}</strong><em>${node.desc}</em><small>${costText}</small></button>`;
      }).join("");
      const unlockedCount = line.nodes.filter((node) => hasTalent(node.key)).length;
      return `<section class="talent-line talent-line-${line.key}"><div class="talent-line-head"><strong>${line.title}</strong><span>${unlockedCount} / ${line.nodes.length}</span></div><div class="talent-line-nodes">${nodes}</div></section>`;
    }).join("");
  }

  function unlockTalent(key) {
    const talent = TALENT_NODE_MAP[key];
    if (!talent || hasTalent(key) || !isTalentAvailable(key)) {
      return;
    }
    if (state.stardust < talent.cost) {
      setTip("Not enough Stardust to unlock this Talent.");
      return;
    }

    state.stardust -= talent.cost;
    state.talents.push(key);
    trackEvent("talentUnlock", { amount: 1 });
    render();
    renderTalentTree();
    showFloatText(`Talent Unlocked: ${talent.name}`, null, "stardust");
    setTip(`${talent.name} is now permanently active.`);
  }

  function hasTalent(key, talentList) {
    const owned = Array.isArray(talentList) ? talentList : state.talents;
    return owned.includes(key);
  }

  function isTalentAvailable(key) {
    const talent = TALENT_NODE_MAP[key];
    if (!talent) {
      return false;
    }
    if (talent.index === 0) {
      return true;
    }
    const line = TALENT_LINES.find((item) => item.key === talent.line);
    return line.nodes.slice(0, talent.index).every((node) => hasTalent(node.key));
  }

  function getFreeSpinCap(talentList) {
    return MAX_FREE_SPINS + (hasTalent("freeSpinCap", talentList) ? TALENT_FREE_SPIN_CAP_BONUS : 0);
  }

  function getEnergyChargeMultiplier() {
    return hasTalent("energyCharge") ? TALENT_ENERGY_MULTIPLIER : 1;
  }

  function getTalentSpecialProbabilityMultiplier() {
    return hasTalent("specialRate") ? TALENT_SPECIAL_MULTIPLIER : 1;
  }

  function getTalentHighTierChance() {
    return hasTalent("nativeHighTier") ? TALENT_HIGH_TIER_CHANCE : 0;
  }

  function getSpinDrops() {
    const drops = createSpinDropTable(getTalentSpecialProbabilityMultiplier(), getTalentHighTierChance());
    return isAdventureMode() && state.level <= 3 ? boostEarlySpinDrops(drops) : drops;
  }

  function getRiftSpinDrops() {
    return createSpinDropTable(RIFT_SPECIAL_PROBABILITY_MULTIPLIER * getTalentSpecialProbabilityMultiplier(), getTalentHighTierChance());
  }

  function getVoidSpinDrops() {
    return createVoidSpinDropTable(getTalentSpecialProbabilityMultiplier());
  }

  function boostEarlySpinDrops(drops) {
    const table = drops.map((item) => ({ ...item }));
    const level1 = table.find((item) => item.kind === "gem" && item.level === 1);
    const level2 = table.find((item) => item.kind === "gem" && item.level === 2);
    const level3 = table.find((item) => item.kind === "gem" && item.level === 3);
    if (!level1 || !level2 || !level3) {
      return table;
    }

    const shift = Math.min(level1.weight - 1, 900);
    if (shift <= 0) {
      return table;
    }

    level1.weight -= shift;
    level2.weight += Math.round(shift * 0.62);
    level3.weight += shift - Math.round(shift * 0.62);
    return table;
  }

  function selectStartMode(mode) {
    if (!GAME_MODES[mode] || runtime.animating) {
      return;
    }
    if (!state.complianceAccepted) {
      updateComplianceGate();
      return;
    }

    if (mode !== state.currentMode) {
      loadModeState(mode);
      clearUndoState();
    }

    document.body.classList.add("game-started");
    if (dom.startScreen) {
      dom.startScreen.hidden = true;
    }
    playSound("modeSelect");
    render();
    updateBgmMix();
    startModeTimer();
    startTutorialIfNeeded();
    setTip(`Entered ${GAME_MODES[mode].title}.`);
  }

  function switchMode(mode) {
    if (!GAME_MODES[mode] || runtime.animating || mode === state.currentMode) {
      return;
    }
    if (isTimedMode() && getModeData(MODE_TIMED).active) {
      setTip("Time Challenge is active. Switch modes after it ends.");
      return;
    }

    captureCurrentModeState();
    loadModeState(mode);
    clearUndoState();
    render();
    setTip(`Switched to ${GAME_MODES[mode].title}.`);
  }

  function captureCurrentModeState() {
    const data = getModeData(state.currentMode);
    data.board = cloneBoard(state.board);
    data.manualUnlocked = state.manualUnlocked;
  }

  function loadModeState(mode, skipCapture) {
    if (!skipCapture && state.currentMode) {
      captureCurrentModeState();
    }
    const data = getModeData(mode);
    state.currentMode = mode;
    state.board = normalizeBoard(data.board);
    state.manualUnlocked = Boolean(data.manualUnlocked);
    runtime.selected = null;
    runtime.drag = null;
    runtime.toolMode = null;
    runtime.toolSelection = null;
    runtime.skillMode = null;
    runtime.skillLevelBoostActive = false;
    startModeTimer();
  }

  function getModeData(mode) {
    if (!state.modeData) {
      state.modeData = createDefaultModeData();
    }
    if (!state.modeData[mode]) {
      state.modeData[mode] = createDefaultModeState(mode);
    }
    return state.modeData[mode];
  }

  function isAdventureMode() {
    return state.currentMode === MODE_ADVENTURE;
  }

  function isEndlessMode() {
    return state.currentMode === MODE_ENDLESS;
  }

  function isTimedMode() {
    return state.currentMode === MODE_TIMED;
  }

  function isZenMode() {
    return state.currentMode === MODE_ZEN;
  }

  function isScoreMode() {
    return isEndlessMode() || isTimedMode();
  }

  function addModeScore(amount, label) {
    if (!isScoreMode() || amount <= 0) {
      return;
    }
    const data = getModeData(state.currentMode);
    if (isTimedMode() && !data.active) {
      return;
    }
    const value = Math.max(0, Math.round(amount));
    data.score += value;
    data.bestScore = Math.max(data.bestScore || 0, data.score);
    if (label) {
      showFloatText(`${label} +${formatNumber(value)} pts`, null, "combo");
    }
    if (!isTimedMode()) {
      updateModeLeaderboard(state.currentMode);
    }
    renderModeHud();
  }

  function updateModeComboBest(combo) {
    if (!isScoreMode()) {
      return;
    }
    const data = getModeData(state.currentMode);
    data.bestCombo = Math.max(data.bestCombo || 0, combo);
  }

  function updateModeLeaderboard(mode) {
    const data = getModeData(mode);
    if (!Array.isArray(data.leaderboard)) {
      data.leaderboard = [];
    }
    const score = Math.floor(data.score || 0);
    if (score <= 0) {
      return;
    }
    const entry = {
      score,
      combo: data.bestCombo || 0,
      date: getDailyDateKey()
    };
    data.leaderboard.push(entry);
    data.leaderboard = data.leaderboard
      .sort((a, b) => b.score - a.score || b.combo - a.combo)
      .filter((item, index, list) => index === list.findIndex((other) => other.score === item.score && other.combo === item.combo && other.date === item.date))
      .slice(0, 5);
  }

  function startTimedRun() {
    const data = getModeData(MODE_TIMED);
    data.active = true;
    data.score = 0;
    data.bestCombo = 0;
    data.remaining = TIMED_MODE_DURATION_MS;
    data.endsAt = Date.now() + TIMED_MODE_DURATION_MS;
    data.board = createBoard(getSpinDrops());
    data.manualUnlocked = false;
    state.board = cloneBoard(data.board);
    state.manualUnlocked = false;
    startModeTimer();
    setTip("Time Challenge started: merge high-tier crystals and build combos for 2 minutes.");
  }

  function finishTimedRun() {
    const data = getModeData(MODE_TIMED);
    if (!data.active) {
      return;
    }
    captureCurrentModeState();
    data.active = false;
    data.remaining = 0;
    data.lastScore = data.score || 0;
    data.bestScore = Math.max(data.bestScore || 0, data.lastScore);
    data.runs = (data.runs || 0) + 1;
    const reward = getTimedReward(data.lastScore);
    data.rewards.freeSpins += reward.freeSpins;
    if (reward.item) {
      data.rewards.items[reward.item] = (data.rewards.items[reward.item] || 0) + 1;
    }
    updateModeLeaderboard(MODE_TIMED);
    stopModeTimer();
    state.manualUnlocked = false;
    showFloatText(`Time Result ${formatNumber(data.lastScore)} pts`, null, "jackpot");
    dom.rewardModalTitle.textContent = "Time Challenge Result";
    dom.rewardModalText.textContent = `Score ${formatNumber(data.lastScore)}. Mode reward: ${reward.freeSpins} Free Spins${reward.item ? `, ${ITEM_LABELS[reward.item]} x1` : ""}. Saved to Time Challenge data.`;
    dom.rewardModal.hidden = false;
    scheduleTextFit(dom.rewardModal);
    render();
  }

  function getTimedReward(score) {
    if (score >= 5000) {
      return { freeSpins: 8, item: "summon" };
    }
    if (score >= 2500) {
      return { freeSpins: 5, item: "leap" };
    }
    return { freeSpins: 2, item: "blast" };
  }

  function getTimedRemainingMs() {
    const data = getModeData(MODE_TIMED);
    if (!data.active) {
      return data.remaining || TIMED_MODE_DURATION_MS;
    }
    return Math.max(0, data.endsAt - Date.now());
  }

  function startModeTimer() {
    stopModeTimer();
    if (!isTimedMode() || !getModeData(MODE_TIMED).active) {
      return;
    }
    runtime.modeTimer = window.setInterval(() => {
      const data = getModeData(MODE_TIMED);
      data.remaining = getTimedRemainingMs();
      renderModeHud();
      if (data.remaining <= 0 && !runtime.animating) {
        finishTimedRun();
      }
    }, 500);
  }

  function stopModeTimer() {
    if (runtime.modeTimer) {
      window.clearInterval(runtime.modeTimer);
      runtime.modeTimer = null;
    }
  }

  function formatTime(ms) {
    const total = Math.max(0, Math.ceil(ms / 1000));
    const minutes = String(Math.floor(total / 60)).padStart(2, "0");
    const seconds = String(total % 60).padStart(2, "0");
    return `${minutes}:${seconds}`;
  }

  function createDefaultTaskState() {
    return refreshTaskState({
      dailyDate: "",
      daily: [],
      weeklyKey: "",
      weekly: null
    });
  }

  function normalizeTaskState(tasks) {
    const source = tasks && typeof tasks === "object" ? tasks : {};
    return refreshTaskState({
      dailyDate: source.dailyDate || "",
      daily: Array.isArray(source.daily) ? source.daily.map((task) => normalizeTaskInstance(task, DAILY_TASK_POOL)).filter(Boolean) : [],
      weeklyKey: source.weeklyKey || "",
      weekly: normalizeTaskInstance(source.weekly, WEEKLY_TASK_POOL)
    });
  }

  function createDefaultSettings() {
    return {
      quality: "medium",
      shake: true,
      hapticStrength: "high",
      sound: true,
      volume: 0.58,
      autoMerge: false,
      tutorialDone: false,
      tutorialVersion: 0
    };
  }

  function normalizeSettings(settings) {
    const fallback = createDefaultSettings();
    const source = settings && typeof settings === "object" ? settings : {};
    return {
      quality: QUALITY_PRESETS[source.quality] ? source.quality : fallback.quality,
      shake: source.shake === undefined ? fallback.shake : Boolean(source.shake),
      hapticStrength: HAPTIC_STRENGTHS[source.hapticStrength] ? source.hapticStrength : fallback.hapticStrength,
      sound: source.sound === undefined ? fallback.sound : Boolean(source.sound),
      volume: clampNumber(Number.isFinite(source.volume) ? source.volume : fallback.volume, 0, 1),
      autoMerge: source.autoMerge === undefined ? fallback.autoMerge : Boolean(source.autoMerge),
      tutorialDone: Boolean(source.tutorialDone),
      tutorialVersion: Math.max(0, Math.floor(Number(source.tutorialVersion) || 0))
    };
  }

  function applyFxSettings(settings) {
    const normalized = normalizeSettings(settings || state.settings);
    const preset = QUALITY_PRESETS[normalized.quality] || QUALITY_PRESETS.high;
    const profile = PERF_PROFILES[perf.profile] || PERF_PROFILES.standard;
    const legacySmooth = document.body.classList.contains("legacy-smooth");
    const legacyCoinRainMax = legacySmooth && normalized.quality !== "low"
      ? Math.max(2, Math.round((profile.maxCoinRain || 6) * (normalized.quality === "high" ? 0.86 : 0.58)))
      : 0;
    FX_CONFIG.quality = normalized.quality;
    FX_CONFIG.particleScale = preset.particleScale;
    FX_CONFIG.deviceParticleScale = profile.particleScale;
    FX_CONFIG.maxParticles = legacySmooth ? legacyCoinRainMax : Math.max(0, Math.round(profile.maxParticles * Math.max(0.2, preset.particleScale || 0)));
    FX_CONFIG.maxBurst = legacySmooth ? 0 : Math.max(0, Math.round(profile.maxBurst * Math.max(0.2, preset.particleScale || 0)));
    FX_CONFIG.maxCoinRain = legacySmooth ? legacyCoinRainMax : Math.max(0, Math.round(profile.maxCoinRain * Math.max(0.2, preset.particleScale || 0)));
    FX_CONFIG.textFitInterval = profile.textFitInterval;
    FX_CONFIG.particles = preset.particles && !legacySmooth;
    FX_CONFIG.screenFlash = preset.screenFlash;
    FX_CONFIG.coinRain = legacySmooth ? legacyCoinRainMax > 0 : preset.coinRain;
    FX_CONFIG.shake = normalized.shake;
    document.body.dataset.quality = normalized.quality;
    document.body.dataset.perfProfile = perf.profile;
    document.body.classList.toggle("shake-disabled", !normalized.shake);
    document.body.classList.toggle("perf-low-device", perf.profile === "small");
    document.body.classList.toggle("perf-tablet-device", perf.profile === "tablet");
    updateAudioSettings(normalized);
  }

  function updatePerformanceProfile() {
    const width = window.innerWidth || document.documentElement.clientWidth || 960;
    const height = window.innerHeight || document.documentElement.clientHeight || 640;
    const longEdge = Math.max(width, height);
    const shortEdge = Math.min(width, height);
    const memory = Number(navigator.deviceMemory) || 8;
    const cores = Number(navigator.hardwareConcurrency) || 8;
    const touch = navigator.maxTouchPoints > 1;
    let profile = "standard";
    if (touch && longEdge >= 1024 && shortEdge >= 700) {
      profile = "tablet";
    } else if (touch || longEdge <= 820 || shortEdge <= 500 || memory <= 4 || cores <= 4) {
      profile = "small";
    }
    perf.profile = profile;
    document.body.dataset.perfProfile = profile;
    document.body.classList.toggle("perf-low-device", profile === "small");
    document.body.classList.toggle("perf-tablet-device", profile === "tablet");
  }

  function createDefaultModeData() {
    return {
      [MODE_ADVENTURE]: createDefaultModeState(MODE_ADVENTURE),
      [MODE_ENDLESS]: createDefaultModeState(MODE_ENDLESS),
      [MODE_TIMED]: createDefaultModeState(MODE_TIMED),
      [MODE_ZEN]: createDefaultModeState(MODE_ZEN)
    };
  }

  function createDefaultModeState(mode) {
    if (mode === MODE_ENDLESS) {
      return {
        board: createBoard(FILL_DROPS),
        manualUnlocked: false,
        score: 0,
        bestScore: 0,
        bestCombo: 0,
        spins: 0,
        leaderboard: []
      };
    }
    if (mode === MODE_TIMED) {
      return {
        board: createBoard(FILL_DROPS),
        manualUnlocked: false,
        active: false,
        score: 0,
        bestScore: 0,
        lastScore: 0,
        bestCombo: 0,
        remaining: TIMED_MODE_DURATION_MS,
        endsAt: 0,
        runs: 0,
        leaderboard: [],
        rewards: {
          freeSpins: 0,
          items: {}
        }
      };
    }
    if (mode === MODE_ZEN) {
      return {
        board: createBoard(FILL_DROPS),
        manualUnlocked: false,
        spins: 0
      };
    }
    return {
      board: createBoard(FILL_DROPS),
      manualUnlocked: false
    };
  }

  function normalizeModeData(modeData, sourceState) {
    const defaults = createDefaultModeData();
    const source = modeData && typeof modeData === "object" ? modeData : {};
    const adventure = {
      ...defaults[MODE_ADVENTURE],
      ...(source[MODE_ADVENTURE] || {}),
      board: normalizeBoard((source[MODE_ADVENTURE] && source[MODE_ADVENTURE].board) || sourceState.board),
      manualUnlocked: Boolean(source[MODE_ADVENTURE] && source[MODE_ADVENTURE].manualUnlocked !== undefined ? source[MODE_ADVENTURE].manualUnlocked : sourceState.manualUnlocked)
    };
    const endless = normalizeScoreModeState(source[MODE_ENDLESS], defaults[MODE_ENDLESS]);
    const timed = normalizeScoreModeState(source[MODE_TIMED], defaults[MODE_TIMED]);
    timed.active = Boolean(timed.active);
    timed.remaining = clampNumber(timed.active ? timed.endsAt - Date.now() : timed.remaining, 0, TIMED_MODE_DURATION_MS);
    timed.rewards = {
      freeSpins: Math.max(0, Math.floor(Number(timed.rewards && timed.rewards.freeSpins) || 0)),
      items: {
        ...(timed.rewards && timed.rewards.items ? timed.rewards.items : {})
      }
    };
    const zen = {
      ...defaults[MODE_ZEN],
      ...(source[MODE_ZEN] || {}),
      board: normalizeBoard((source[MODE_ZEN] && source[MODE_ZEN].board) || defaults[MODE_ZEN].board),
      spins: Math.max(0, Math.floor(Number(source[MODE_ZEN] && source[MODE_ZEN].spins) || 0))
    };
    return {
      [MODE_ADVENTURE]: adventure,
      [MODE_ENDLESS]: endless,
      [MODE_TIMED]: timed,
      [MODE_ZEN]: zen
    };
  }

  function normalizeScoreModeState(sourceMode, fallback) {
    const source = sourceMode && typeof sourceMode === "object" ? sourceMode : {};
    return {
      ...fallback,
      ...source,
      board: normalizeBoard(source.board || fallback.board),
      manualUnlocked: Boolean(source.manualUnlocked),
      score: Math.max(0, Math.floor(Number(source.score) || 0)),
      bestScore: Math.max(0, Math.floor(Number(source.bestScore) || 0)),
      lastScore: Math.max(0, Math.floor(Number(source.lastScore) || 0)),
      bestCombo: Math.max(0, Math.floor(Number(source.bestCombo) || 0)),
      spins: Math.max(0, Math.floor(Number(source.spins) || 0)),
      runs: Math.max(0, Math.floor(Number(source.runs) || 0)),
      endsAt: Math.max(0, Math.floor(Number(source.endsAt) || 0)),
      remaining: clampNumber(source.remaining || TIMED_MODE_DURATION_MS, 0, TIMED_MODE_DURATION_MS),
      leaderboard: normalizeLeaderboard(source.leaderboard)
    };
  }

  function normalizeLeaderboard(leaderboard) {
    if (!Array.isArray(leaderboard)) {
      return [];
    }
    return leaderboard
      .map((entry) => ({
        score: Math.max(0, Math.floor(Number(entry.score) || 0)),
        combo: Math.max(0, Math.floor(Number(entry.combo) || 0)),
        date: entry.date || ""
      }))
      .filter((entry) => entry.score > 0)
      .sort((a, b) => b.score - a.score || b.combo - a.combo)
      .slice(0, 5);
  }

  function refreshTasks(targetState) {
    targetState.tasks = refreshTaskState(targetState.tasks || {});
  }

  function refreshTaskState(taskState) {
    const today = getDailyDateKey();
    const weekKey = getWeeklyKey();
    const next = taskState && typeof taskState === "object" ? taskState : {};

    if (next.dailyDate !== today || !Array.isArray(next.daily) || next.daily.length !== 3) {
      next.dailyDate = today;
      next.daily = pickTaskDefinitions(DAILY_TASK_POOL, 3, today).map(createTaskInstance);
    }

    if (next.weeklyKey !== weekKey || !next.weekly) {
      next.weeklyKey = weekKey;
      next.weekly = createTaskInstance(pickTaskDefinitions(WEEKLY_TASK_POOL, 1, weekKey)[0]);
    }

    return next;
  }

  function normalizeTaskInstance(task, pool) {
    if (!task || typeof task !== "object") {
      return null;
    }
    const definition = pool.find((item) => item.key === task.key);
    if (!definition) {
      return null;
    }
    const next = createTaskInstance(definition);
    next.progress = clampNumber(task.progress || 0, 0, next.target);
    next.completed = Boolean(task.completed) || next.progress >= next.target;
    next.rewarded = Boolean(task.rewarded) || next.completed;
    return next;
  }

  function createTaskInstance(definition) {
    return {
      ...definition,
      progress: 0,
      completed: false,
      rewarded: false,
      reward: { ...(definition.reward || {}) }
    };
  }

  function pickTaskDefinitions(pool, count, seed) {
    const candidates = [...pool];
    const picked = [];
    let value = hashString(seed);
    while (picked.length < count && candidates.length > 0) {
      const index = value % candidates.length;
      picked.push(candidates.splice(index, 1)[0]);
      value = (value * 1664525 + 1013904223) >>> 0;
    }
    return picked;
  }

  function getWeeklyKey(date) {
    const target = date ? new Date(date) : new Date();
    const dayOffset = (target.getDay() + 6) % 7;
    target.setDate(target.getDate() - dayOffset);
    return formatDateKey(target);
  }

  function formatDateKey(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
  }

  function hashString(text) {
    let hash = 2166136261;
    for (let i = 0; i < text.length; i += 1) {
      hash ^= text.charCodeAt(i);
      hash = Math.imul(hash, 16777619);
    }
    return hash >>> 0;
  }

  function renderSkillHud() {
    const skillKey = getEquippedSkillKey();
    const skill = ACTIVE_SKILLS[skillKey];
    const cooldown = getSkillCooldown(skillKey);

    if (isPortraitUi()) {
      dom.activeSkillIcon.textContent = "⚡";
      dom.activeSkillName.textContent = "BL";
      dom.activeSkillCooldown.textContent = state.items.blast > 0 ? String(state.items.blast) : "0";
    } else {
      dom.activeSkillIcon.textContent = "⚡";
      dom.activeSkillName.textContent = skill.name.slice(0, 2);
      dom.activeSkillCooldown.textContent = cooldown > 0 ? String(cooldown) : "";
    }
    dom.skillPanel.innerHTML = DEFAULT_UNLOCKED_SKILLS.map((key) => {
      const option = ACTIVE_SKILLS[key];
      const optionCooldown = getSkillCooldown(key);
      const selected = key === skillKey ? " selected" : "";
      const cooldownText = optionCooldown > 0 ? `${optionCooldown}` : "READY";
      return `<button class="skill-choice${selected}" type="button" data-skill-equip="${key}"><span>${option.icon}</span><strong>${option.name}</strong><em>${cooldownText}</em></button>`;
    }).join("");
    updateSkillButtonState();
    document.body.classList.toggle("skill-boost-active", runtime.skillLevelBoostActive);
  }

  function updateSkillButtonState() {
    const skillKey = getEquippedSkillKey();
    const cooldown = getSkillCooldown(skillKey);
    if (isPortraitUi()) {
      const hasCharges = (state.items.blast || 0) > 0;
      const clickable = isAdventureMode() && !runtime.animating && state.manualUnlocked;
      dom.activeSkillButton.disabled = !clickable;
      dom.skillSwitchButton.disabled = true;
      dom.activeSkillButton.classList.toggle("ready", clickable && hasCharges);
      dom.activeSkillButton.classList.toggle("cooling", runtime.animating);
      dom.activeSkillButton.classList.toggle("no-charges", !hasCharges);
      dom.activeSkillButton.classList.remove("targeting", "active");
      return;
    }

    const clickable = isAdventureMode() && !runtime.animating;
    const available = clickable && state.manualUnlocked;
    dom.activeSkillButton.disabled = !clickable;
    dom.skillSwitchButton.disabled = runtime.animating || !isAdventureMode();
    dom.activeSkillButton.classList.toggle("ready", cooldown === 0 && available);
    dom.activeSkillButton.classList.toggle("cooling", cooldown > 0);
    dom.activeSkillButton.classList.toggle("targeting", runtime.skillMode === "columnRedraw");
    dom.activeSkillButton.classList.toggle("active", skillKey === "fieldRankUp" && runtime.skillLevelBoostActive);
  }

  function onSkillDockClick(event) {
    if (event.target.closest("#activeSkillButton") || event.target.closest("#skillSwitchButton")) {
      return;
    }
    onActiveSkillButtonClick(event);
  }

  function toggleSkillPanel(event) {
    event.stopPropagation();
    if (runtime.animating || !isAdventureMode()) {
      return;
    }

    dom.skillPanel.hidden = !dom.skillPanel.hidden;
    renderSkillHud();
  }

  function openSkillPanel() {
    dom.skillPanel.hidden = false;
    renderSkillHud();
  }

  function equipActiveSkill(skillKey) {
    if (!state.skills.unlocked.includes(skillKey) || !ACTIVE_SKILLS[skillKey]) {
      return;
    }

    state.skills.equipped = skillKey;
    runtime.skillMode = null;
    dom.skillPanel.hidden = true;
    render();
    setTip(`Equipped Ability: ${ACTIVE_SKILLS[skillKey].name}.`);
  }

  function onActiveSkillButtonClick(event) {
    event.stopPropagation();
    if (isPortraitUi()) {
      openGlobalCrystalBlastPrompt();
      return;
    }

    if (runtime.animating || !isAdventureMode()) {
      return;
    }

    const skillKey = getEquippedSkillKey();
    const cooldown = getSkillCooldown(skillKey);
    if (cooldown > 0) {
      dom.activeSkillButton.classList.add("denied");
      window.setTimeout(() => dom.activeSkillButton.classList.remove("denied"), 280);
      openSkillPanel();
      setTip(`${ACTIVE_SKILLS[skillKey].name} is cooling down. ${cooldown} spins remaining.`);
      return;
    }

    if (!state.manualUnlocked) {
      openSkillPanel();
      setTip("Spin first, then use Ability during the merge phase.");
      return;
    }

    if (skillKey === "columnRedraw") {
      runtime.skillMode = runtime.skillMode === "columnRedraw" ? null : "columnRedraw";
      renderBoard();
      renderSkillHud();
      setTip(runtime.skillMode ? "Column Reroll: choose any column to refresh its crystals." : "Column Reroll targeting canceled.");
    } else if (skillKey === "fieldRankUp") {
      applyFieldRankUpSkill();
    } else if (skillKey === "lowTierPurge") {
      applyLowTierPurgeSkill();
    }
  }

  function openGlobalCrystalBlastPrompt() {
    if (runtime.animating || !isAdventureMode()) {
      return;
    }

    if (!state.manualUnlocked) {
      setTip("Spin first, then use Global Crystal Blast during the merge phase.");
      return;
    }

    if ((state.items.blast || 0) <= 0) {
      setTip("Global Crystal Blast needs 1 Purge Blast charge.");
      playSound("skillDenied");
      triggerHaptic("failure", { force: true });
      dom.activeSkillButton.classList.add("denied");
      window.setTimeout(() => dom.activeSkillButton.classList.remove("denied"), 280);
      return;
    }

    closeGlobalCrystalBlastPrompt();
    playSound("skillCharge");
    const modal = document.createElement("div");
    modal.className = "ability-blast-modal";
    modal.innerHTML = `
      <section class="ability-blast-card" role="dialog" aria-modal="true" aria-label="Global Crystal Blast">
        <strong>Global Crystal Blast</strong>
        <p>Spend 1 Purge Blast charge to shatter several low-tier crystals, then drop Tier2/Tier3 crystals for new merge chances.</p>
        <span>Charges: ${formatNumber(state.items.blast || 0)}</span>
        <div>
          <button type="button" data-blast-cancel>Cancel</button>
          <button type="button" data-blast-confirm>Use Skill</button>
        </div>
      </section>
    `;
    document.body.appendChild(modal);
    modal.addEventListener("click", (event) => {
      if (event.target === modal) {
        closeGlobalCrystalBlastPrompt();
      }
    });
    modal.querySelector("[data-blast-cancel]").addEventListener("click", closeGlobalCrystalBlastPrompt);
    modal.querySelector("[data-blast-confirm]").addEventListener("click", () => {
      closeGlobalCrystalBlastPrompt();
      useGlobalCrystalBlastSkill();
    });
  }

  function closeGlobalCrystalBlastPrompt() {
    const existing = document.querySelector(".ability-blast-modal");
    if (existing) {
      existing.remove();
    }
  }

  async function useGlobalCrystalBlastSkill() {
    if (runtime.animating || !isAdventureMode() || !state.manualUnlocked) {
      return;
    }

    const candidates = allCells().filter((cell) => {
      const symbol = getGem(cell);
      return symbol && symbol.kind === "gem" && symbol.level <= 2;
    });
    if (candidates.length === 0) {
      setTip("Global Crystal Blast found no low-tier crystals to shatter.");
      return;
    }
    if ((state.items.blast || 0) <= 0) {
      setTip("Global Crystal Blast needs 1 Purge Blast charge.");
      return;
    }

    consumeTool("blast");
    runtime.toolMode = null;
    runtime.skillMode = null;
    runtime.animating = true;
    beginComboChain();
    playSound("skillBlast");

    const selected = candidates
      .sort(() => Math.random() - 0.5)
      .slice(0, Math.min(candidates.length, 4 + Math.floor(Math.random() * 3)));
    selected.forEach((cell) => {
      state.board[cell.r][cell.c] = null;
      createParticlesAtCell(cell, 18, "#ffd45c", 96);
    });
    updateChallengeProgress("clear", selected.length);
    addClearEffectRiftEnergy(selected.length);
    playSkillReleaseFeedback("Global Crystal Blast", selected[0] || null);
    renderBoard();
    await delay(220);
    await settleBoard(PURGE_DROPS);

    const boostCells = allCells()
      .filter((cell) => {
        const symbol = getGem(cell);
        return symbol && symbol.kind === "gem" && symbol.level <= 2;
      })
      .sort(() => Math.random() - 0.5)
      .slice(0, 2);
    boostCells.forEach((cell) => {
      state.board[cell.r][cell.c] = createGem(3);
      createParticlesAtCell(cell, 12, "#4edcff", 78);
    });

    renderBoard();
    await delay(180);
    await resolveCascade(null, null);
    runtime.animating = false;
    captureCurrentModeState();
    render();
    scheduleComboFade();
    setTip("Global Crystal Blast used. Low-tier crystals shattered and high-tier drops entered the board.");
  }

  function tickSkillCooldowns() {
    DEFAULT_UNLOCKED_SKILLS.forEach((skillKey) => {
      state.skills.cooldowns[skillKey] = Math.max(0, getSkillCooldown(skillKey) - 1);
    });
  }

  function startSkillCooldown(skillKey) {
    state.skills.cooldowns[skillKey] = ACTIVE_SKILLS[skillKey].cooldown;
  }

  function getEquippedSkillKey() {
    return ACTIVE_SKILLS[state.skills.equipped] ? state.skills.equipped : DEFAULT_ACTIVE_SKILL;
  }

  function getSkillCooldown(skillKey) {
    return Math.max(0, Math.floor(state.skills.cooldowns[skillKey] || 0));
  }

  async function applyActiveSkillToCell(cell) {
    if (!isAdventureMode()) {
      return;
    }

    if (runtime.skillMode === "columnRedraw") {
      await applyColumnRedrawSkill(cell.c);
    }
  }

  async function applyColumnRedrawSkill(column) {
    const skillKey = "columnRedraw";
    if (runtime.animating || getSkillCooldown(skillKey) > 0 || !isAdventureMode()) {
      return;
    }

    runtime.skillMode = null;
    runtime.animating = true;
    beginComboChain();
    for (let r = 0; r < ROWS; r += 1) {
      state.board[r][column] = createDrop(FILL_DROPS);
      createParticlesAtCell({ r, c: column }, 14, "#4edcff", 86);
    }
    startSkillCooldown(skillKey);
    playSkillReleaseFeedback("Column Reroll", { r: Math.floor(ROWS / 2), c: column });
    renderBoard();
    await delay(240);
    await settleBoard();
    await resolveCascade(null, null);
    runtime.animating = false;
    captureCurrentModeState();
    render();
    scheduleComboFade();
    setTip("Column Reroll used. That column has been refreshed.");
  }

  function applyFieldRankUpSkill() {
    const skillKey = "fieldRankUp";
    if (runtime.animating || getSkillCooldown(skillKey) > 0 || !isAdventureMode()) {
      return;
    }

    runtime.skillMode = null;
    runtime.skillLevelBoostActive = true;
    startSkillCooldown(skillKey);
    playSkillReleaseFeedback("Field Tier Up", null);
    render();
    setTip("Field Tier Up used: all crystals gain +1 tier for this merge phase.");
  }

  async function applyLowTierPurgeSkill() {
    const skillKey = "lowTierPurge";
    if (runtime.animating || getSkillCooldown(skillKey) > 0 || !isAdventureMode()) {
      return;
    }

    const cells = allCells().filter((cell) => {
      const symbol = getGem(cell);
      return symbol && symbol.kind === "gem" && symbol.level <= 2;
    });
    if (cells.length === 0) {
      setTip("Low-Tier Purge found no Tier1/Tier2 crystals.");
      return;
    }

    runtime.skillMode = null;
    runtime.animating = true;
    beginComboChain();
    cells.forEach((cell) => {
      state.board[cell.r][cell.c] = null;
      createParticlesAtCell(cell, 16, "#57e5a2", 96);
    });
    startSkillCooldown(skillKey);
    updateChallengeProgress("clear", cells.length);
    addClearEffectRiftEnergy(cells.length);
    playSkillReleaseFeedback("Low-Tier Purge", null);
    renderBoard();
    await delay(260);
    await settleBoard(PURGE_DROPS);
    await resolveCascade(null, null);
    runtime.animating = false;
    captureCurrentModeState();
    render();
    scheduleComboFade();
    setTip("Low-Tier Purge used. Empty slots dropped Tier2/Tier3 crystals.");
  }

  function playSkillReleaseFeedback(label, cell) {
    showFloatText(label, cell, "skill");
    flashScreen("skill", 520);
    shake("soft");
    if (cell) {
      createParticlesAtCell(cell, 40, "#ffd45c", 132);
    } else {
      createParticlesAtBoard(54, "#4edcff", 150);
      createParticlesAtBoard(34, "#ff6f9e", 130);
    }
  }

  async function spin() {
    if (runtime.animating) {
      return;
    }

    if (state.voidBonusReady && isAdventureMode()) {
      await startVoidBonus();
      return;
    }

    const coinContinueCost = getStageCoinContinueCost();
    let paidSpin = false;
    let quotaSpin = false;
    const mode = state.currentMode;
    beginComboChain();
    runtime.freeSpinRound = false;
    runtime.riftSpinRound = false;
    runtime.lineBoost = 1;
    runtime.fullBoost = 1;
    runtime.selected = null;
    runtime.toolMode = null;
    runtime.toolSelection = null;
    runtime.skillMode = null;
    runtime.skillLevelBoostActive = false;

    if (isAdventureMode()) {
      if (state.freeSpins > 0) {
        state.freeSpins -= 1;
        runtime.freeSpinRound = true;
      } else if (state.stageSpinsLeft > 0) {
        state.stageSpinsLeft -= 1;
        quotaSpin = true;
      } else if (state.coins >= coinContinueCost) {
        state.coins -= coinContinueCost;
        contributeToJackpot(coinContinueCost);
        paidSpin = true;
      } else {
        handleStageSpinBlocked();
        renderHud();
        return;
      }
      trackEvent("spin", { amount: 1 });
    } else {
      const modeData = getModeData(mode);
      if (isTimedMode()) {
        if (!modeData.active) {
          startTimedRun();
        }
        if (getTimedRemainingMs() <= 0) {
          finishTimedRun();
          return;
        }
      }
      modeData.spins = (modeData.spins || 0) + 1;
    }

    if (isAdventureMode() && state.riftReady) {
      runtime.riftSpinRound = true;
      state.riftReady = false;
      state.riftEnergy = 0;
      playRiftActivationFeedback();
    }
    if (isAdventureMode() && (paidSpin || quotaSpin)) {
      addRiftEnergy(8);
    }
    if (isAdventureMode() && state.voidBonusReady) {
      tickSkillCooldowns();
      await startVoidBonus();
      return;
    }
    if (isAdventureMode()) {
      tickSkillCooldowns();
    }
    if (isAdventureMode() && quotaSpin && state.stageSpinsLeft <= 5 && state.stageSpinsLeft % 2 === 1) {
      playSound("warning");
    }
    resetUndoForNewRound();

    runtime.animating = true;
    state.manualUnlocked = false;
    dom.board.classList.add("spinning");
    playSound("spinStart");
    if (isAdventureMode()) {
      setTip(runtime.riftSpinRound ? "Neon Rift active: crystals are at least +1 tier this round, special symbol rates are doubled, and merge/line rewards are x1.5." : paidSpin ? `Coin Continue spent ${formatNumber(coinContinueCost)} Coins. Rift Energy +8.` : quotaSpin ? `Board spinning. Stage Spins Left ${formatNumber(state.stageSpinsLeft)}. Rift Energy +8.` : "Free Spin started.");
    } else if (isEndlessMode()) {
      setTip("Endless Mode spin: this round counts toward mode score and the local leaderboard.");
    } else if (isTimedMode()) {
      setTip(`Time Challenge spin: ${formatTime(getTimedRemainingMs())} remaining.`);
    } else {
      setTip("Zen Mode free spin: no cost, no objective, no rewards.");
    }

    state.board = createBoard(runtime.riftSpinRound ? getRiftSpinDrops() : getSpinDrops(), runtime.riftSpinRound ? RIFT_SPIN_LEVEL_BOOST : 0);
    if (isAdventureMode() && state.summonNext) {
      placeGuaranteedGem(4);
      state.summonNext = false;
    }
    if (isAdventureMode()) {
      applyStagePityGuarantee();
    }

    render();
    await delay(820);
    dom.board.classList.remove("spinning");
    playSound("spinStop");
    renderBoard();
    await settleBoard();
    await detonateAllBombSymbols();
    await awardLineWins("Initial Line");
    await resolveGoldCoreLineWins("Gold Core Line", false);

    if (isTimedMode() && getTimedRemainingMs() <= 0) {
      runtime.animating = false;
      state.manualUnlocked = false;
      captureCurrentModeState();
      render();
      finishTimedRun();
      return;
    }

    state.manualUnlocked = true;
    runtime.animating = false;
    captureCurrentModeState();
    render();
    scheduleComboFade();
    setTip(isZenMode() ? "Zen merge phase open. Drag freely to merge." : "Manual merge unlocked. Rift Bombs can detonate, and Holo Proxy can join merges and lines.");
    maybeRunAutoMergeAfterSpin();
  }

  function handleStageSpinBlocked() {
    if (!isAdventureMode()) {
      return;
    }

    if (hasUsableStageRescueItems() || state.stardust >= STARDUST_EXCHANGE_COST) {
      openCoinRescueModal();
      setTip("Stage Spins are depleted. Rescue Coins, use Items, or restart the Stage.");
      return;
    }

    openStageFailureModal();
  }

  function hasUsableStageRescueItems() {
    return Object.keys(state.items || {}).some((key) => state.items[key] > 0);
  }

  function openStageFailureModal() {
    if (!dom.stageFailModal) {
      return;
    }

    dom.stageFailModal.hidden = false;
    if (dom.coinRescueModal) {
      dom.coinRescueModal.hidden = true;
    }
    playSound("fail");
    triggerHaptic("failure", { force: true });
    scheduleTextFit(dom.stageFailModal);
  }

  function openCoinRescueModal() {
    if (!dom.coinRescueModal) {
      return;
    }

    const grantAmount = getCoinRescueAmount();
    if (dom.coinRescueText) {
      dom.coinRescueText.textContent = `Coin Continue costs ${formatNumber(getStageCoinContinueCost())} Coins per spin. Watch an ad for ${formatNumber(grantAmount)} Coins or exchange ${formatNumber(STARDUST_EXCHANGE_COST)} Stardust.`;
    }
    if (dom.watchAdCoinsButton) {
      dom.watchAdCoinsButton.textContent = `Watch Ad For ${formatNumber(grantAmount)} Coins`;
    }
    if (dom.exchangeStardustButton) {
      dom.exchangeStardustButton.textContent = `Exchange ${formatNumber(STARDUST_EXCHANGE_COST)} Stardust`;
      dom.exchangeStardustButton.disabled = state.stardust < STARDUST_EXCHANGE_COST;
    }
    dom.coinRescueModal.hidden = false;
    if (dom.stageFailModal) {
      dom.stageFailModal.hidden = true;
    }
    playSound("coin");
    triggerHaptic("failure", { force: true });
    scheduleTextFit(dom.coinRescueModal);
  }

  function getCoinRescueAmount() {
    return getStageCoinContinueCost() * STAGE_COIN_RESCUE_MULTIPLIER;
  }

  function grantAdStageSpins() {
    state.stageSpinsLeft += STAGE_FAIL_AD_SPINS;
    if (dom.stageFailModal) {
      dom.stageFailModal.hidden = true;
    }
    render();
    playSound("reward");
    setTip(`Rewarded ad complete. Stage Spins Left +${STAGE_FAIL_AD_SPINS}.`);
  }

  function restartCurrentStage() {
    if (!isAdventureMode() || runtime.animating) {
      return;
    }

    if (dom.stageFailModal) {
      dom.stageFailModal.hidden = true;
    }
    if (dom.coinRescueModal) {
      dom.coinRescueModal.hidden = true;
    }
    state.targetProgress = 0;
    state.targetAltProgress = 0;
    state.challengeProgress = 0;
    state.stageSpinQuotaLevel = state.level;
    state.stageSpinsLeft = getInitialStageSpinQuota(state.level);
    state.stageChallengeSpinAwarded = false;
    state.stagePity = createDefaultStagePity(state.level);
    state.manualUnlocked = false;
    state.board = createBoard(FILL_DROPS);
    runtime.selected = null;
    runtime.drag = null;
    runtime.toolMode = null;
    runtime.toolSelection = null;
    runtime.skillMode = null;
    runtime.skillLevelBoostActive = false;
    runtime.combo = 0;
    clearUndoState();
    applyLevelConfig();
    render();
    setTip("Stage restarted. Objective progress and Stage Spin quota have been reset.");
  }

  function grantAdCoins() {
    const amount = getCoinRescueAmount();
    state.coins += amount;
    if (dom.coinRescueModal) {
      dom.coinRescueModal.hidden = true;
    }
    render();
    playSound("reward");
    setTip(`Rewarded ad complete. +${formatNumber(amount)} Coins.`);
  }

  function exchangeStardustForCoins() {
    if (state.stardust < STARDUST_EXCHANGE_COST) {
      setTip("Not enough Stardust to exchange for Coins.");
      triggerHaptic("failure", { force: true });
      openCoinRescueModal();
      return;
    }

    const amount = getCoinRescueAmount();
    state.stardust -= STARDUST_EXCHANGE_COST;
    state.coins += amount;
    if (dom.coinRescueModal) {
      dom.coinRescueModal.hidden = true;
    }
    render();
    playSound("reward");
    setTip(`Exchanged ${formatNumber(STARDUST_EXCHANGE_COST)} Stardust for ${formatNumber(amount)} Coins.`);
  }

  async function startVoidBonus() {
    if (!isAdventureMode()) {
      return;
    }

    const savedBoard = cloneBoard(state.board);
    const savedManualUnlocked = state.manualUnlocked;

    runtime.animating = true;
    runtime.voidBonusActive = true;
    runtime.voidBonusRound = 0;
    runtime.voidBonusTotal = 0;
    runtime.voidSavedBoard = savedBoard;
    runtime.voidSavedManualUnlocked = savedManualUnlocked;
    runtime.freeSpinRound = false;
    runtime.riftSpinRound = false;
    runtime.lineBoost = 1;
    runtime.fullBoost = 1;
    runtime.selected = null;
    runtime.toolMode = null;
    runtime.toolSelection = null;
    runtime.skillMode = null;
    runtime.skillLevelBoostActive = false;
    state.voidBonusReady = false;
    state.riftReady = false;
    state.riftEnergy = 0;
    state.manualUnlocked = false;
    playVoidBonusStartFeedback();
    render();

    for (let round = 1; round <= VOID_BONUS_ROUNDS; round += 1) {
      runtime.voidBonusRound = round;
      beginComboChain();
      runtime.lineBoost = 1;
      runtime.fullBoost = 1;
      dom.board.classList.add("spinning");
      setTip(`Void Realm round ${round}/${VOID_BONUS_ROUNDS}: free spin. Realm winnings are banked and paid x${VOID_BONUS_PAYOUT_MULTIPLIER} at the end.`);
      state.board = createBoard(getVoidSpinDrops());
      render();
      await delay(820);
      dom.board.classList.remove("spinning");
      renderBoard();
      await settleBoard();
      await detonateAllBombSymbols();
      await awardLineWins("Realm Line");
      await resolveGoldCoreLineWins("Realm Gold Core", false);
      await resolveCascade(null, null);
      scheduleComboFade();
      render();
      await delay(360);
    }

    const baseTotal = runtime.voidBonusTotal;
    const payout = Math.round(baseTotal * VOID_BONUS_PAYOUT_MULTIPLIER);
    state.coins += payout;
    state.board = savedBoard;
    state.manualUnlocked = savedManualUnlocked;
    runtime.voidBonusActive = false;
    runtime.voidBonusRound = 0;
    runtime.voidBonusTotal = 0;
    runtime.freeSpinRound = false;
    runtime.riftSpinRound = false;
    runtime.lineBoost = 1;
    runtime.fullBoost = 1;
    runtime.voidSavedBoard = null;
    runtime.voidSavedManualUnlocked = false;
    runtime.animating = false;
    playVoidBonusEndFeedback(baseTotal, payout);
    trackEvent("voidBonusComplete", { amount: 1 });
    render();
    setTip(`Void Realm complete: banked ${formatNumber(baseTotal)}, paid ${formatNumber(payout)} Coins after multiplier.`);
  }

  function onPointerDown(event) {
    const cell = getCellFromEvent(event);
    if (!cell || runtime.animating) {
      return;
    }

    if (runtime.skillMode) {
      event.preventDefault();
      applyActiveSkillToCell(cell);
      return;
    }

    if (runtime.toolMode) {
      event.preventDefault();
      applyToolToCell(cell);
      return;
    }

    if (!state.manualUnlocked) {
      setTip("Spin first, then merge manually or activate special symbols.");
      return;
    }

    const symbol = getGem(cell);
    if (!symbol) {
      return;
    }

    const gemElement = event.target.closest(".gem");
    runtime.drag = {
      pointerId: event.pointerId,
      from: cell,
      startX: event.clientX,
      startY: event.clientY,
      gemElement,
      moved: false
    };
    if (gemElement) {
      gemElement.classList.add("dragging");
    }
    document.body.classList.add("dragging-gem");
    dom.board.setPointerCapture(event.pointerId);
    event.preventDefault();
  }

  function onPointerMove(event) {
    const drag = runtime.drag;
    if (!drag || drag.pointerId !== event.pointerId) {
      return;
    }

    const dx = event.clientX - drag.startX;
    const dy = event.clientY - drag.startY;
    if (Math.abs(dx) + Math.abs(dy) > 7) {
      drag.moved = true;
    }

    if (drag.gemElement) {
      drag.gemElement.style.transform = `translate(${dx}px, ${dy - 8}px) scale(1.1)`;
    }

    drag.previewX = event.clientX;
    drag.previewY = event.clientY;
    if (!drag.previewRaf) {
      drag.previewRaf = requestAnimationFrame(() => {
        if (!runtime.drag || runtime.drag !== drag) {
          return;
        }
        drag.previewRaf = 0;
        updateMergePreview(drag.previewX, drag.previewY);
      });
    }
  }

  async function onPointerUp(event) {
    const drag = runtime.drag;
    if (!drag || drag.pointerId !== event.pointerId) {
      return;
    }

    const targetElement = document.elementFromPoint(event.clientX, event.clientY);
    const targetCellElement = targetElement ? targetElement.closest(".cell") : null;
    const target = targetCellElement ? readCell(targetCellElement) : drag.from;
    const startSymbol = getGem(drag.from);
    clearDrag(event.pointerId);

    if (!target) {
      return;
    }

    if (!drag.moved && startSymbol && startSymbol.kind === "special" && startSymbol.special !== "wild") {
      setTip(`${SPECIAL_SYMBOLS[startSymbol.special].name} does not drag-merge. It triggers by its own rule.`);
      return;
    }

    if (drag.moved && !sameCell(drag.from, target)) {
      await tryManualSwap(drag.from, target);
      return;
    }

    if (!runtime.selected) {
      runtime.selected = drag.from;
      renderBoard();
      setTip("Select a second orthogonally adjacent crystal to merge.");
      return;
    }

    const from = runtime.selected;
    runtime.selected = null;
    renderBoard();
    if (!sameCell(from, target)) {
      await tryManualSwap(from, target);
    }
  }

  function onPointerCancel(event) {
    if (runtime.drag && runtime.drag.pointerId === event.pointerId) {
      clearDrag(event.pointerId);
    }
  }

  function clearDrag(pointerId) {
    const drag = runtime.drag;
    if (!drag) {
      return;
    }

    clearMergePreview();
    if (drag.previewRaf) {
      cancelAnimationFrame(drag.previewRaf);
    }
    if (drag.gemElement) {
      drag.gemElement.classList.remove("dragging");
      drag.gemElement.style.transform = "";
    }
    document.body.classList.remove("dragging-gem");

    if (dom.board.hasPointerCapture(pointerId)) {
      dom.board.releasePointerCapture(pointerId);
    }
    runtime.drag = null;
  }

  function updateMergePreview(clientX, clientY) {
    const drag = runtime.drag;
    if (!drag || !drag.moved) {
      clearMergePreview();
      return;
    }

    const targetElement = document.elementFromPoint(clientX, clientY);
    const targetCellElement = targetElement ? targetElement.closest(".cell") : null;
    const target = targetCellElement ? readCell(targetCellElement) : null;
    const preview = target ? getSwapMergePreview(drag.from, target) : null;
    showMergePreview(preview);
  }

  function getSwapMergePreview(from, to) {
    if (!from || !to || sameCell(from, to) || !areAdjacent(from, to) || !getGem(from) || !getGem(to)) {
      return null;
    }

    swapCells(from, to);
    const groups = findGroupsAfterSwap([from, to]);
    swapCells(from, to);
    if (groups.length === 0) {
      return null;
    }

    const bestGroup = groups[0];
    const mergeLevel = getGroupLevel(bestGroup);
    if (!mergeLevel) {
      return null;
    }
    const target = bestGroup.some((cell) => sameCell(cell, to)) ? to : chooseMergeTarget(bestGroup);

    const cells = [];
    const seen = new Set();
    groups.forEach((group) => {
      group.forEach((cell) => {
        const key = cellKey(cell);
        if (!seen.has(key)) {
          seen.add(key);
          cells.push(cell);
        }
      });
    });

    return {
      cells,
      target,
      level: getOverloadMergeInfo(mergeLevel, bestGroup.length).finalLevel
    };
  }

  function showMergePreview(preview) {
    if (!preview || preview.cells.length === 0) {
      clearMergePreview();
      return;
    }

    const previewKey = `${preview.level}:${cellKey(preview.target)}:${preview.cells.map(cellKey).sort().join("|")}`;
    if (runtime.mergePreview && runtime.mergePreview.key === previewKey) {
      return;
    }

    clearMergePreview();
    preview.cells.forEach((cell) => {
      const element = findCellElement(cell);
      if (element) {
        element.classList.add("merge-preview-cell");
      }
    });

    const targetElement = findCellElement(preview.target);
    let marker = null;
    if (targetElement) {
      const rect = targetElement.getBoundingClientRect();
      marker = document.createElement("span");
      marker.className = "merge-preview-badge";
      marker.textContent = LEVELS[preview.level].short;
      marker.style.left = `${rect.left + rect.width / 2}px`;
      marker.style.top = `${rect.top + rect.height / 2}px`;
      dom.floatLayer.appendChild(marker);
    }

    runtime.mergePreview = {
      key: previewKey,
      cells: preview.cells,
      marker
    };
  }

  function clearMergePreview() {
    if (!runtime.mergePreview) {
      return;
    }

    runtime.mergePreview.cells.forEach((cell) => {
      const element = findCellElement(cell);
      if (element) {
        element.classList.remove("merge-preview-cell");
      }
    });
    if (runtime.mergePreview.marker) {
      runtime.mergePreview.marker.remove();
    }
    runtime.mergePreview = null;
  }

  async function tryManualSwap(from, to) {
    if (runtime.animating) {
      return;
    }

    if (!areAdjacent(from, to)) {
      setTip("Only orthogonally adjacent crystals can be swapped.");
      return;
    }

    if (!getGem(from) || !getGem(to)) {
      return;
    }

    const undoSnapshot = createUndoSnapshot();
    runtime.animating = true;
    beginComboChain();
    swapCells(from, to);
    renderBoard();
    await delay(140);

    const group = findGroupAfterSwap([from, to]);
    if (!group) {
      swapCells(from, to);
      runtime.animating = false;
      captureCurrentModeState();
      render();
      setTip("No adjacent same-tier group of 3 formed. Swap reverted.");
      return;
    }

    commitUndoSnapshot(undoSnapshot);
    await resolveCascade(group, to);
    runtime.animating = false;
    captureCurrentModeState();
    render();
    scheduleComboFade();
    setTip("Merge complete. Keep playing or spin again.");
  }

  async function autoMergeBoard() {
    if (runtime.animating || !state.manualUnlocked) {
      return;
    }

    const group = findNextCascadeMergeGroup();
    if (!group) {
      setTip("No valid Auto Merge groups on the current board.");
      render();
      return;
    }

    const undoSnapshot = createUndoSnapshot();
    commitUndoSnapshot(undoSnapshot);
    runtime.animating = true;
    runtime.selected = null;
    runtime.toolMode = null;
    runtime.toolSelection = null;
    runtime.skillMode = null;
    beginComboChain();
    setTip("Auto Merge started: valid groups will resolve in order.");
    await resolveCascade(group, chooseMergeTarget(group));
    runtime.animating = false;
    captureCurrentModeState();
    render();
    scheduleComboFade();
    setTip("Auto Merge complete. Keep playing or spin again.");
  }

  function maybeRunAutoMergeAfterSpin() {
    if (!state.settings.autoMerge || runtime.animating || !state.manualUnlocked || !findNextMergeGroup()) {
      return;
    }

    window.setTimeout(() => {
      if (!runtime.animating && state.manualUnlocked && state.settings.autoMerge && findNextMergeGroup()) {
        autoMergeBoard();
      }
    }, 420);
  }

  function createUndoSnapshot() {
    captureCurrentModeState();
    return {
      state: JSON.parse(JSON.stringify(state)),
      combo: runtime.combo,
      freeSpinRound: runtime.freeSpinRound,
      riftSpinRound: runtime.riftSpinRound,
      lineBoost: runtime.lineBoost,
      fullBoost: runtime.fullBoost
    };
  }

  function commitUndoSnapshot(snapshot) {
    if (runtime.undoUsed || !snapshot) {
      return;
    }

    runtime.undoSnapshot = snapshot;
    updateButtons();
  }

  function resetUndoForNewRound() {
    runtime.undoSnapshot = null;
    runtime.undoUsed = false;
    updateButtons();
  }

  function clearUndoState() {
    runtime.undoSnapshot = null;
    runtime.undoUsed = false;
  }

  function undoLastMerge() {
    if (runtime.animating || runtime.undoUsed || !runtime.undoSnapshot) {
      return;
    }

    const snapshot = runtime.undoSnapshot;
    state = JSON.parse(JSON.stringify(snapshot.state));
    runtime.undoSnapshot = null;
    runtime.undoUsed = true;
    runtime.combo = snapshot.combo || 0;
    runtime.freeSpinRound = Boolean(snapshot.freeSpinRound);
    runtime.riftSpinRound = Boolean(snapshot.riftSpinRound);
    runtime.lineBoost = snapshot.lineBoost || 1;
    runtime.fullBoost = snapshot.fullBoost || 1;
    runtime.selected = null;
    runtime.drag = null;
    runtime.toolMode = null;
    runtime.toolSelection = null;
    runtime.skillMode = null;
    runtime.skillLevelBoostActive = false;
    applyFxSettings();
    loadModeState(state.currentMode, true);
    render();
    showFloatText("Merge Undone", null, "skill");
    setTip("Merge undone for this round. Undo refreshes next spin.");
  }

  function openItemModal() {
    if (runtime.animating || !isAdventureMode()) {
      return;
    }
    renderHud();
    dom.itemModal.hidden = false;
    scheduleTextFit(dom.itemModal);
  }

  function openToolDetail(tool) {
    if (!ITEM_DETAILS[tool] || runtime.animating || !isAdventureMode()) {
      return;
    }

    const detail = ITEM_DETAILS[tool];
    runtime.selectedToolDetail = tool;
    dom.toolDetailIcon.textContent = detail.icon;
    dom.toolDetailIcon.dataset.tool = tool;
    dom.toolDetailTitle.textContent = ITEM_LABELS[tool];
    dom.toolDetailName.textContent = ITEM_LABELS[tool];
    dom.toolDetailDesc.textContent = detail.desc;
    dom.toolDetailSource.textContent = detail.source;
    dom.toolDetailCount.textContent = state.items[tool] || 0;
    dom.toolDetailUseButton.disabled = (state.items[tool] || 0) <= 0;
    dom.toolDetailModal.hidden = false;
    scheduleTextFit(dom.toolDetailModal);
  }

  function useSelectedToolFromDetail() {
    const tool = runtime.selectedToolDetail;
    if (!tool || !ITEM_DETAILS[tool]) {
      return;
    }

    dom.toolDetailModal.hidden = true;
    activateTool(tool);
  }

  function showToolHover(anchor, tool) {
    if (runtime.toolHoverTimer) {
      window.clearTimeout(runtime.toolHoverTimer);
      runtime.toolHoverTimer = null;
    }
    hideToolHover();
    playSound("tooltip");

    const element = document.createElement("div");
    element.className = "tool-hover-tip";
    if (tool === "tools") {
      element.innerHTML = `<strong>Items</strong><span>View and use swap, blast, leap, and call Items.</span>`;
    } else if (tool === "lobby") {
      element.innerHTML = `<strong>Lobby</strong><span>Save progress and return to mode select.</span>`;
    } else {
      const detail = ITEM_DETAILS[tool];
      if (!detail) {
        return;
      }
      element.innerHTML = `<strong>${ITEM_LABELS[tool]}</strong><span>${detail.short}</span>`;
    }

    const rect = anchor.getBoundingClientRect();
    element.style.left = `${rect.right + 12}px`;
    element.style.top = `${rect.top + rect.height / 2}px`;
    dom.floatLayer.appendChild(element);
    runtime.toolHoverElement = element;
  }

  function showGemHover(gemElement) {
    if (runtime.toolHoverTimer) {
      window.clearTimeout(runtime.toolHoverTimer);
      runtime.toolHoverTimer = null;
    }
    hideToolHover();
    playSound("tooltip");

    const content = getGemHoverContent(gemElement);
    if (!content) {
      return;
    }

    const element = document.createElement("div");
    element.className = "tool-hover-tip gem-hover-tip";
    element.innerHTML = `<strong>${content.title}</strong><span>${content.meta}</span><span>${content.effect}</span>`;
    const rect = gemElement.getBoundingClientRect();
    dom.floatLayer.appendChild(element);
    const tipRect = element.getBoundingClientRect();
    const margin = 12;
    const aboveTop = rect.top - tipRect.height - margin;
    const centeredLeft = rect.left + rect.width / 2 - tipRect.width / 2;
    const canShowAbove = aboveTop >= margin + getSafeAreaTop();
    if (canShowAbove) {
      element.classList.add("above-cell");
      element.style.left = `${clampNumber(centeredLeft, margin, window.innerWidth - tipRect.width - margin)}px`;
      element.style.top = `${aboveTop}px`;
    } else {
      const rightLeft = rect.right + margin;
      const fallbackLeft = rightLeft + tipRect.width <= window.innerWidth - margin
        ? rightLeft
        : Math.max(margin, rect.left - tipRect.width - margin);
      element.classList.add("right-side");
      element.style.left = `${fallbackLeft}px`;
      element.style.top = `${clampNumber(rect.top + rect.height / 2 - tipRect.height / 2, margin + getSafeAreaTop(), window.innerHeight - tipRect.height - margin)}px`;
    }
    runtime.toolHoverElement = element;
  }

  function getSafeAreaTop() {
    const value = getComputedStyle(document.documentElement).getPropertyValue("--safe-top");
    const parsed = Number.parseFloat(value);
    return Number.isFinite(parsed) ? parsed : 0;
  }

  function getGemHoverContent(gemElement) {
    const level = Number(gemElement.dataset.gemLevel || 0);
    if (level && LEVELS[level] && GEM_EFFECTS[level]) {
      return {
        title: `${LEVELS[level].short} ${LEVELS[level].name}`,
        meta: `Base Line Multiplier x${LEVELS[level].multiplier}`,
        effect: GEM_EFFECTS[level].short
      };
    }

    const special = gemElement.dataset.special;
    if (special && SPECIAL_EFFECTS[special]) {
      return {
        title: SPECIAL_EFFECTS[special].name,
        meta: "Special Symbol",
        effect: SPECIAL_EFFECTS[special].short
      };
    }

    return null;
  }

  function scheduleToolHoverHide() {
    if (runtime.toolHoverTimer) {
      window.clearTimeout(runtime.toolHoverTimer);
    }
    runtime.toolHoverTimer = window.setTimeout(hideToolHover, 200);
  }

  function hideToolHover() {
    if (runtime.toolHoverElement) {
      const element = runtime.toolHoverElement;
      element.classList.add("leaving");
      window.setTimeout(() => element.remove(), 160);
      runtime.toolHoverElement = null;
    }
  }

  function revealItemIfNeeded(tool) {
    if (!ITEM_DETAILS[tool] || state.seenItems[tool]) {
      return;
    }

    state.seenItems[tool] = true;
    showItemUnlockToast(tool);
  }

  function showItemUnlockToast(tool) {
    const detail = ITEM_DETAILS[tool];
    const toast = document.createElement("div");
    toast.className = "item-unlock-toast";
    toast.innerHTML = `<span>${detail.icon}</span><strong>${ITEM_LABELS[tool]}</strong><em>${detail.short}</em>`;
    dom.floatLayer.appendChild(toast);
    window.setTimeout(() => toast.classList.add("leaving"), 1800);
    window.setTimeout(() => toast.remove(), 2200);
  }

  function activateTool(tool) {
    if (!ITEM_LABELS[tool] || state.items[tool] <= 0 || runtime.animating || !isAdventureMode()) {
      return;
    }

    dom.itemModal.hidden = true;
    runtime.selected = null;
    runtime.toolSelection = null;

    if (tool === "summon") {
      consumeTool(tool);
      state.summonNext = true;
      render();
      setTip("Prism Call active. The next spin guarantees at least 1 Tier4 Void Prism.");
      return;
    }

    runtime.toolMode = tool;
    renderBoard();
    if (tool === "swap") {
      setTip("Swap Cube: choose any two crystals to swap directly.");
    } else if (tool === "blast") {
      setTip("Purge Blast: choose one low-tier crystal to clear and refill.");
    } else if (tool === "leap") {
      setTip("Tier Leap: choose a tier with at least 3 crystals on the board.");
    }
  }

  async function applyToolToCell(cell) {
    if (runtime.animating || !runtime.toolMode || !isAdventureMode()) {
      return;
    }

    const tool = runtime.toolMode;
    const symbol = getGem(cell);
    if (!symbol) {
      return;
    }

    if (tool === "swap") {
      await applySwapTool(cell);
    } else if (tool === "blast") {
      await applyBlastTool(cell);
    } else if (tool === "leap") {
      await applyLeapTool(cell);
    }
  }

  async function applySwapTool(cell) {
    if (!runtime.toolSelection) {
      runtime.toolSelection = cell;
      renderBoard();
      setTip("Swap Cube: choose the second crystal.");
      return;
    }

    const from = runtime.toolSelection;
    runtime.toolSelection = null;
    if (sameCell(from, cell)) {
      renderBoard();
      setTip("Swap Cube: choose a different crystal.");
      return;
    }

    consumeTool("swap");
    runtime.toolMode = null;
    runtime.animating = true;
    beginComboChain();
    swapCells(from, cell);
    renderBoard();
    await delay(140);
    await resolveCascade(null, null);
    runtime.animating = false;
    captureCurrentModeState();
    render();
    scheduleComboFade();
    setTip("Swap Cube used.");
  }

  async function applyBlastTool(cell) {
    const symbol = getGem(cell);
    if (!symbol || symbol.kind !== "gem" || symbol.level >= 4) {
      setTip("Purge Blast only works on low-tier crystals.");
      return;
    }

    consumeTool("blast");
    runtime.toolMode = null;
    runtime.animating = true;
    beginComboChain();
    const splashCells = collectElementSplashCells([cell], [cell]);
    state.board[cell.r][cell.c] = null;
    bumpCombo("Purge", cell);
    const splashCleared = clearSplashCells(splashCells);
    updateChallengeProgress("clear", 1 + splashCleared);
    addClearEffectRiftEnergy(1 + splashCleared);
    createParticlesAtCell(cell, 22, "#74d7ff");
    renderBoard();
    await settleBoard();
    await resolveCascade(null, null);
    runtime.animating = false;
    captureCurrentModeState();
    render();
    scheduleComboFade();
    setTip("Purge Blast used.");
  }

  async function applyLeapTool(cell) {
    const symbol = getGem(cell);
    if (!symbol || symbol.kind !== "gem" || symbol.level >= 5) {
      setTip("Tier5 Genesis Crystals and special symbols cannot be Tier Leaped.");
      return;
    }

    const candidates = findCellsByLevel(symbol.level);
    if (candidates.length < 3) {
      setTip("Not enough crystals of this tier for Tier Leap.");
      return;
    }

    const group = pickLeapGroup(candidates, cell);
    consumeTool("leap");
    runtime.toolMode = null;
    runtime.animating = true;
    beginComboChain();
    await resolveCascade(group, cell);
    runtime.animating = false;
    captureCurrentModeState();
    render();
    scheduleComboFade();
    setTip("Tier Leap used.");
  }

  function consumeTool(tool) {
    state.items[tool] = Math.max(0, state.items[tool] - 1);
    trackEvent("toolUse", { amount: 1, tool });
    saveState();
  }

  async function detonateAllBombSymbols() {
    const bombCells = allCells().filter((cell) => {
      const symbol = getGem(cell);
      return symbol && symbol.kind === "special" && symbol.special === "bomb";
    });

    if (bombCells.length === 0) {
      return false;
    }

    for (const cell of bombCells) {
      await detonateBombSymbol(cell);
    }
    await settleBoard();
    await resolveCascade(null, null);
    return true;
  }

  async function detonateBombSymbol(cell) {
    const symbol = getGem(cell);
    if (!symbol || symbol.kind !== "special" || symbol.special !== "bomb") {
      return;
    }

    const blastCells = getAreaCells(cell, 1).filter((target) => {
      const targetSymbol = getGem(target);
      return sameCell(target, cell) || isLowTierGem(targetSymbol);
    });
    const clearCount = blastCells.filter((target) => isLowTierGem(getGem(target))).length;
    const splashCells = collectElementSplashCells(blastCells, blastCells);
    bumpCombo("Rift Bomb", cell);
    const reward = Math.round(getSpinCost() * SPECIAL_SYMBOLS.bomb.rewardMultiplier * Math.max(1, clearCount) * getIncomeBoost());
    awardCoins(reward);
    trackEvent("bomb", { amount: 1 }, cell);
    blastCells.forEach((target) => {
      state.board[target.r][target.c] = null;
      createParticlesAtCell(target, 16, "#ff8f70");
    });
    const splashCleared = clearSplashCells(splashCells);
    updateChallengeProgress("clear", clearCount + splashCleared);
    addClearEffectRiftEnergy(clearCount + splashCleared);
    showFloatText(`Blast +${formatNumber(reward)}`, cell, "jackpot");
    flashScreen("half");
    shake();
    renderBoard();
    await delay(240);
  }

  async function resolveGoldCoreLineWins(label, cascadeAfter) {
    const wins = findGoldCoreLineWins();
    if (wins.length === 0) {
      return 0;
    }

    let total = 0;
    const cells = [];
    wins.forEach((win) => {
      const middle = win.cells[Math.floor(win.cells.length / 2)];
      bumpCombo(label, middle);
      total += Math.round(getSpinCost() * SPECIAL_SYMBOLS.coin.rewardMultiplier * win.length * getIncomeBoost());
      cells.push(...win.cells);
    });

    awardCoins(total);
    trackEvent("goldCoreWin", { amount: wins.length });
    cells.forEach((cell) => {
      state.board[cell.r][cell.c] = null;
      createParticlesAtCell(cell, 24, "#ffd45c");
    });
    addClearEffectRiftEnergy(cells.length);
    await playLineWinFeedback(wins, total);
    renderBoard();
    await settleBoard();
    if (cascadeAfter) {
      await resolveCascade(null, null);
    }
    return total;
  }

  async function resolveCascade(initialGroup, preferredCell) {
    runtime.lineBoost = 1;
    runtime.fullBoost = 1;
    beginRewardBuffer();

    let group = initialGroup;
    let preferred = preferredCell;
    let guard = 0;

    while (guard < 18) {
      guard += 1;

      if (!group) {
        group = findNextCascadeMergeGroup();
        preferred = group ? chooseMergeTarget(group) : null;
      }

      if (!group) {
        break;
      }

      await mergeGroup(group, preferred || chooseMergeTarget(group), guard > 1);
      await settleBoard();
      await awardLineWins("Cascade Line");
      await resolveGoldCoreLineWins("Cascade Gold Core", false);
      await delay(150);
      group = null;
      preferred = null;
    }

    flushRewardBuffer();
    runtime.lineBoost = 1;
    runtime.fullBoost = 1;
    completeLevelIfNeeded();
  }

  async function mergeGroup(group, targetCell, isChainStep) {
    const mergeLevel = getGroupLevel(group);
    if (!mergeLevel || mergeLevel >= 5) {
      return;
    }

    const overload = getOverloadMergeInfo(mergeLevel, group.length);
    const newLevel = overload.finalLevel;
    const target = targetCell && group.some((cell) => sameCell(cell, targetCell)) ? targetCell : chooseMergeTarget(group);
    const elementInfo = getMergeElementInfo(group);

    if (canFx("merge")) {
      highlightMergeCells(group, target, overload.isOverload ? "overload-merging" : "merging", overload.isOverload ? 680 : 520);
    }
    await delay(canFx("merge") ? overload.isOverload ? 320 : 240 : 0);

    group.forEach((cell) => {
      state.board[cell.r][cell.c] = null;
    });
    state.board[target.r][target.c] = createGem(newLevel, elementInfo.resultElement);

    recordMerge(newLevel, target, overload, elementInfo);
    renderBoard();
    playMergeSpawn(target, newLevel, overload);
    if (isChainStep) {
      playChainMergeFeedback();
    }
    await playMergeFeedback(newLevel, target, overload);
    applySpecialEffect(newLevel, target);
    await delay(getMergeFeedbackDelay(newLevel, overload));
    renderBoard();
    await delay(newLevel >= 4 ? 260 : 150);
  }

  function recordMerge(newLevel, cell, overload, elementInfo) {
    bumpCombo(overload && overload.isOverload ? "Tier Jump Merge" : "Merge", cell);
    const overloadBoost = overload && overload.isOverload ? 1.5 : 1;
    const elementBoost = getElementMergeRewardBoost(elementInfo);
    const reward = Math.round(8 * LEVELS[newLevel].multiplier * getIncomeBoost() * overloadBoost * elementBoost);
    awardCoins(reward);
    trackEvent("merge", { amount: 1, level: newLevel }, cell);
    updateChallengeProgress("merge", 1);
    addRiftEnergy(3);
    showFloatText(`+${formatNumber(reward)}`, cell, overload && overload.isOverload ? "overload" : newLevel >= 4 ? "jackpot" : "coin");
    showElementMergeBonusText(elementInfo, cell);
    revealGemLevelIfNeeded(newLevel);
    if (newLevel === 5) {
      grantAchievement("firstL5", cell);
    }

    if (isAdventureMode() && !runtime.voidBonusActive && newLevel === state.targetLevel) {
      state.targetProgress = Math.min(state.targetRequired, state.targetProgress + 1);
    }
    if (isAdventureMode() && !runtime.voidBonusActive && state.targetAltLevel > 0 && newLevel === state.targetAltLevel) {
      state.targetAltProgress = Math.min(state.targetAltRequired, state.targetAltProgress + 1);
    }
  }

  function getMergeElementInfo(group) {
    const gemSymbols = group
      .map((cell) => getGem(cell))
      .filter((symbol) => symbol && symbol.kind === "gem");
    const elements = gemSymbols.map((symbol) => normalizeElementKey(symbol.element));
    const firstElement = elements[0] || randomElementKey();
    const sameElement = elements.length > 0 && elements.every((element) => element === firstElement);
    const hasDailyElement = elements.includes(state.dailyElement);
    return {
      sameElement,
      sameElementKey: sameElement ? firstElement : null,
      hasDailyElement,
      resultElement: sameElement ? firstElement : randomElementKey()
    };
  }

  function getElementMergeRewardBoost(elementInfo) {
    if (!elementInfo) {
      return 1;
    }

    const sameElementBoost = elementInfo.sameElement ? 1.2 : 1;
    const dailyElementBoost = elementInfo.hasDailyElement ? 1.3 : 1;
    return sameElementBoost * dailyElementBoost;
  }

  function showElementMergeBonusText(elementInfo, cell) {
    if (!elementInfo) {
      return;
    }

    if (elementInfo.sameElement) {
      showFloatText(`${ELEMENTS[elementInfo.sameElementKey].name} Resonance x1.2`, cell, "element");
    }
    if (elementInfo.hasDailyElement) {
      showFloatText(`Daily Element +30%`, cell, "element");
    }
  }

  function revealGemLevelIfNeeded(level) {
    if (level < 2 || level > 5 || state.seenGemLevels[level]) {
      return;
    }

    state.seenGemLevels[level] = true;
    showGemUnlockToast(level);
  }

  function showGemUnlockToast(level) {
    const levelInfo = LEVELS[level];
    const effect = GEM_EFFECTS[level];
    if (!levelInfo || !effect) {
      return;
    }

    const toast = document.createElement("div");
    toast.className = "gem-unlock-toast";
    toast.innerHTML = `<span>${levelInfo.short}</span><strong>Unlocked: ${levelInfo.name}</strong><em>${effect.short}</em>`;
    dom.floatLayer.appendChild(toast);
    window.setTimeout(() => toast.classList.add("leaving"), 1800);
    window.setTimeout(() => toast.remove(), 2200);
  }

  function getOverloadMergeInfo(baseLevel, groupSize) {
    const cappedSize = Math.min(groupSize, 5);
    const talentOverload = cappedSize === 3 && hasTalent("overloadBoost") && Math.random() < TALENT_OVERLOAD_CHANCE;
    const levelGain = talentOverload ? 2 : cappedSize >= 5 ? 3 : cappedSize >= 4 ? 2 : 1;
    return {
      finalLevel: Math.min(5, baseLevel + levelGain),
      isOverload: cappedSize >= 4 || talentOverload,
      groupSize: cappedSize
    };
  }

  async function playMergeFeedback(level, cell, overload) {
    if (!canFx("merge")) {
      return;
    }

    const intensity = overload && overload.isOverload ? 1.3 : 1;
    playSound("merge", { level, chain: runtime.combo });
    if (level === 2) {
      createParticlesAtCell(cell, Math.round(16 * intensity), "#dff8ff", 74 * intensity);
      shake("micro");
    } else if (level === 3) {
      createParticlesAtCell(cell, Math.round(32 * intensity), "#7ce7ff", 104 * intensity);
      flashScreen("glow", Math.round(420 * intensity));
      shake("soft");
    } else if (level === 4) {
      playSound("burst", { level });
      createParticlesAtCell(cell, Math.round(46 * intensity), "#9ee7ff", 124 * intensity, "crystal-spectrum");
      createParticlesAtBoard(Math.round(78 * intensity), "#66d9ff", 190 * intensity, "crystal-spectrum");
      flashBoardFrame("l4", Math.round(900 * intensity));
      kickCamera("l4", Math.round(680 * intensity));
      flashScreen("half-blue", Math.round(760 * intensity));
      shake("medium");
    } else if (level === 5) {
      await delay(300);
      playSound("burst", { level });
      createParticlesAtCell(cell, Math.round(72 * intensity), "#ffffff", 146 * intensity, "genesis-spectrum");
      createRainbowBurstAtBoard(intensity);
      createCoinRain(Math.round(26 * intensity));
      flashBoardFrame("l5", 1500);
      kickCamera("l5", 1500);
      flashScreen("white-full", 1500);
      shake("hard");
    }

    if (overload && overload.isOverload) {
      showFloatText("Tier Jump", cell, "overload");
      createParticlesAtCell(cell, 28, "#ffd45c", 132, "crystal-gold");
    }
  }

  function flashBoardFrame(level, duration) {
    if (!dom.boardWrap || !canFx("screenFlash")) {
      return;
    }

    const className = `board-frame-${level}`;
    dom.boardWrap.classList.remove("board-frame-l4", "board-frame-l5");
    void dom.boardWrap.offsetWidth;
    dom.boardWrap.classList.add(className);
    window.setTimeout(() => {
      dom.boardWrap.classList.remove(className);
    }, duration || 900);
  }

  function kickCamera(level, duration) {
    if (!canFx("screenFlash")) {
      return;
    }

    const className = `camera-impact-${level}`;
    document.body.classList.remove("camera-impact-l4", "camera-impact-l5");
    void document.body.offsetWidth;
    document.body.classList.add(className);
    window.setTimeout(() => {
      document.body.classList.remove(className);
    }, duration || 700);
  }

  function getMergeFeedbackDelay(level, overload) {
    if (level === 5) {
      return 1500;
    }
    if (level === 4) {
      return overload && overload.isOverload ? 520 : 420;
    }
    if (level === 3) {
      return overload && overload.isOverload ? 360 : 280;
    }
    return overload && overload.isOverload ? 280 : 180;
  }

  function playChainMergeFeedback() {
    if (!canFx("merge")) {
      return;
    }

    shake("soft");
    pulseComboHud();
  }

  function applySpecialEffect(level, cell) {
    if (level === 2) {
      clearNearbyLowGems(cell, 2);
      setTip("Tier2 Focus Crystal clears low-tier crystals within 2 tiles.");
    } else if (level === 3) {
      clearColumn(cell.c, cell);
      setTip("Tier3 Starcore Crystal clears its full column.");
    } else if (level === 4) {
      runtime.lineBoost = Math.max(runtime.lineBoost, 1.5);
      clearHalfBoard(cell);
      setTip("Tier4 Void Prism triggers a half-board chain clear and boosts new line wins to x1.5.");
    } else if (level === 5) {
      const retained = tryRetainL5ByTalent(cell);
      runtime.fullBoost = Math.max(runtime.fullBoost, 2);
      clearFullBoard(retained ? cell : null);
      grantFreeSpins(3);
      tryTriggerJackpot(cell);
      setTip("Tier5 Genesis Crystal triggers full-screen impact, rainbow burst, board-wide x2 rewards, and 3 Free Spins.");
    }
  }

  function tryRetainL5ByTalent(cell) {
    if (!hasTalent("l5Retain") || Math.random() >= TALENT_L5_RETAIN_CHANCE) {
      return false;
    }

    const symbol = getGem(cell);
    if (symbol && symbol.kind === "gem" && symbol.level === 5) {
      symbol.retainedTurns = 1;
    }
    showFloatText("Genesis Echo", cell, "stardust");
    return true;
  }

  function tryTriggerJackpot(cell) {
    if (!isAdventureMode() || Math.random() >= JACKPOT_TRIGGER_CHANCE) {
      return false;
    }

    const prize = state.jackpot;
    awardCoins(prize);
    state.jackpot = JACKPOT_INITIAL_VALUE;
    grantAchievement("jackpotBreak", cell);
    if (runtime.voidBonusActive) {
      showFloatText(`JACKPOT Banked +${formatNumber(prize)}`, cell, "mega-jackpot");
      createCoinRain(32);
      createParticlesAtBoard(72, "#ffd45c", 190);
      flashScreen("jackpot", 980);
      shake("medium");
      renderHud();
      saveState();
      return true;
    }

    playJackpotBreakFeedback(prize, cell);
    return true;
  }

  function clearNearbyLowGems(origin, maxCount) {
    const originGem = getGem(origin);
    if (!originGem || originGem.kind !== "gem") {
      return;
    }

    const cells = [];
    for (let r = 0; r < ROWS; r += 1) {
      for (let c = 0; c < COLS; c += 1) {
        const cell = { r, c };
        const symbol = getGem(cell);
        if (!symbol || symbol.kind !== "gem" || sameCell(cell, origin) || symbol.level >= originGem.level) {
          continue;
        }

        const distance = Math.abs(origin.r - r) + Math.abs(origin.c - c);
        if (distance <= 2) {
          cells.push({ cell, distance });
        }
      }
    }

    let cleared = 0;
    const baseCells = cells
      .sort((a, b) => a.distance - b.distance)
      .slice(0, maxCount)
      .map(({ cell }) => cell);
    const splashCells = collectElementSplashCells([...baseCells, origin], [...baseCells, origin]);
    baseCells.forEach((cell) => {
      if (isGem(getGem(cell))) {
        cleared += 1;
      }
      state.board[cell.r][cell.c] = null;
      createParticlesAtCell(cell, 10, "#c9f3ff");
    });
    cleared += clearSplashCells(splashCells);
    updateChallengeProgress("clear", cleared);
    addClearEffectRiftEnergy(cleared);
  }

  function clearColumn(column, keepCell) {
    let cleared = 0;
    const baseCells = [];
    for (let r = 0; r < ROWS; r += 1) {
      const cell = { r, c: column };
      if (!sameCell(cell, keepCell)) {
        baseCells.push(cell);
      }
    }
    const splashCells = collectElementSplashCells([...baseCells, keepCell], [...baseCells, keepCell]);
    baseCells.forEach((cell) => {
      if (isGem(getGem(cell))) {
        cleared += 1;
      }
      state.board[cell.r][cell.c] = null;
      createParticlesAtCell(cell, 12, "#88d9ff");
    });
    cleared += clearSplashCells(splashCells);
    updateChallengeProgress("clear", cleared);
    addClearEffectRiftEnergy(cleared);
  }

  function clearHalfBoard(keepCell) {
    const cells = allCells()
      .filter((cell) => !sameCell(cell, keepCell) && getGem(cell))
      .sort(() => Math.random() - 0.5)
      .slice(0, Math.floor((ROWS * COLS) / 2));

    let cleared = 0;
    const splashCells = collectElementSplashCells([...cells, keepCell], [...cells, keepCell]);
    cells.forEach((cell) => {
      if (isGem(getGem(cell))) {
        cleared += 1;
      }
      state.board[cell.r][cell.c] = null;
      createParticlesAtCell(cell, 12, "#ffd45c");
    });
    cleared += clearSplashCells(splashCells);
    updateChallengeProgress("clear", cleared);
    addClearEffectRiftEnergy(cleared);
  }

  function clearFullBoard(keepCell) {
    let cleared = 0;
    const protectedCells = keepCell ? [keepCell] : [];
    const baseCells = allCells().filter((cell) => !sameCell(cell, keepCell));
    const splashCells = collectElementSplashCells([...baseCells, ...protectedCells], [...baseCells, ...protectedCells]);
    baseCells.forEach((cell) => {
      if (isGem(getGem(cell))) {
        cleared += 1;
      }
      state.board[cell.r][cell.c] = null;
      createParticlesAtCell(cell, 14, "#ffffff");
    });
    cleared += clearSplashCells(splashCells);
    updateChallengeProgress("clear", cleared);
    addClearEffectRiftEnergy(cleared);
  }

  async function settleBoard(dropTable) {
    let changed = false;
    const fillTable = dropTable || (runtime.voidBonusActive ? getVoidSpinDrops() : FILL_DROPS);

    for (let c = 0; c < COLS; c += 1) {
      const stack = [];
      for (let r = ROWS - 1; r >= 0; r -= 1) {
        if (state.board[r][c]) {
          stack.push(state.board[r][c]);
        }
      }

      for (let r = ROWS - 1; r >= 0; r -= 1) {
        const nextSymbol = stack.shift() || createDrop(fillTable);
        if (state.board[r][c] !== nextSymbol) {
          changed = true;
        }
        state.board[r][c] = nextSymbol;
      }
    }

    if (changed) {
      dom.board.classList.add("falling");
      renderBoard();
      await delay(340);
      dom.board.classList.remove("falling");
      const highestDrop = Math.max(1, ...state.board.flat().filter(isGem).map((symbol) => symbol.level));
      playSound("drop", { level: highestDrop });
    }
  }

  async function awardLineWins(label) {
    const wins = findHorizontalLineWins();
    if (wins.length === 0) {
      return 0;
    }

    let total = 0;
    const cells = [];
    wins.forEach((win) => {
      bumpCombo("Line Win", win.cells[Math.floor(win.cells.length / 2)]);
      const base = getSpinCost() * LEVELS[win.level].multiplier * win.length;
      total += Math.round(base * getIncomeBoost());
      cells.push(...win.cells);
      updateChallengeProgress("line", 1);
    });

    awardCoins(total);
    trackEvent("lineWin", { amount: wins.length });
    await playLineWinFeedback(wins, total);
    return total;
  }

  async function playLineWinFeedback(wins, total) {
    const cells = wins.flatMap((win) => win.cells);
    if (canFx("lineWin")) {
      highlightCells(cells, "line-win", 1120);
      showLineWinMarkers(wins);
      createParticlesAtBoard(24, "#ffd45c", 96);
    }
    await delay(200);
    showFloatText(`+${formatNumber(total)}`, null, "coin-3d");
    await delay(340);
  }

  function showLineWinMarkers(wins) {
    wins.forEach((win) => {
      const middle = win.cells[Math.floor(win.cells.length / 2)];
      const cellElement = findCellElement(middle);
      if (!cellElement) {
        return;
      }
      const rect = cellElement.getBoundingClientRect();
      const marker = document.createElement("span");
      marker.className = "line-win-marker";
      marker.textContent = "✦";
      marker.style.left = `${rect.left + rect.width / 2}px`;
      marker.style.top = `${rect.top + rect.height * 0.16}px`;
      dom.floatLayer.appendChild(marker);
      window.setTimeout(() => marker.remove(), 760);
    });
  }

  function findHorizontalLineWins() {
    const wins = [];
    const seen = new Set();

    for (let r = 0; r < ROWS; r += 1) {
      for (let level = 1; level <= 5; level += 1) {
        let c = 0;
        while (c < COLS) {
          if (!matchesLineLevel(state.board[r][c], level)) {
            c += 1;
            continue;
          }

          const start = c;
          const cells = [];
          let hasBaseGem = false;
          while (c < COLS && matchesLineLevel(state.board[r][c], level)) {
            const symbol = state.board[r][c];
            if (symbol && symbol.kind === "gem" && symbol.level === level) {
              hasBaseGem = true;
            }
            cells.push({ r, c });
            c += 1;
          }

          if (cells.length >= 3 && hasBaseGem) {
            const key = `${r}:${level}:${start}:${c - 1}`;
            if (!seen.has(key)) {
              seen.add(key);
              wins.push({ level, length: cells.length, cells });
            }
          }
        }
      }
    }

    return wins;
  }

  function findGoldCoreLineWins() {
    const wins = [];

    for (let r = 0; r < ROWS; r += 1) {
      let c = 0;
      while (c < COLS) {
        const symbol = state.board[r][c];
        if (!isGoldCore(symbol)) {
          c += 1;
          continue;
        }

        const cells = [{ r, c }];
        let nextC = c + 1;
        while (nextC < COLS && isGoldCore(state.board[r][nextC])) {
          cells.push({ r, c: nextC });
          nextC += 1;
        }

        if (cells.length >= 3) {
          wins.push({ length: cells.length, cells });
        }
        c = nextC;
      }
    }

    return wins;
  }

  function completeLevelIfNeeded() {
    if (!isAdventureMode()) {
      captureCurrentModeState();
      renderHud();
      saveState();
      return;
    }

    if (runtime.voidBonusActive) {
      renderHud();
      saveState();
      return;
    }

    const primaryComplete = state.targetProgress >= state.targetRequired;
    const altComplete = !(state.targetAltLevel > 0 && state.targetAltRequired > 0) || state.targetAltProgress >= state.targetAltRequired;
    if (!primaryComplete || !altComplete) {
      renderHud();
      saveState();
      return;
    }

    const levelCompleted = state.level;
    const chapterLevel = getChapterLevel();
    const coins = 120 + levelCompleted * 20;
    const freeSpins = chapterLevel === 10 ? 3 : 1;
    const item = randomItemKey();
    const chapterReward = chapterLevel === 10;
    const stardustReward = chapterReward ? 120 + getChapter() * 20 : 20 + Math.floor(levelCompleted / 2);
    const challengeCompleted = state.challengeProgress >= state.challengeRequired;
    const challengeFreeSpins = challengeCompleted ? 2 + Math.floor(levelCompleted / 10) : 0;

    state.coins += coins;
    grantStardust(stardustReward, chapterReward ? "Chapter Chest" : "Stage Clear");
    grantFreeSpins(freeSpins);
    if (challengeCompleted) {
      grantFreeSpins(challengeFreeSpins);
    }
    state.items[item] += 1;
    revealItemIfNeeded(item);
    if (chapterReward) {
      state.items.summon += 1;
      revealItemIfNeeded("summon");
    }
    trackEvent("levelComplete", { amount: 1, level: levelCompleted });
    if (chapterReward) {
      trackEvent("chapterComplete", { amount: 1, chapter: getChapter() });
    }

    state.level += 1;
    state.targetProgress = 0;
    state.targetAltProgress = 0;
    state.challengeProgress = 0;
    state.stageSpinQuotaLevel = state.level;
    state.stageSpinsLeft = getInitialStageSpinQuota(state.level);
    state.stageChallengeSpinAwarded = false;
    state.stagePity = createDefaultStagePity(state.level);
    applyLevelConfig();
    render();

    openLevelRewardModal({
      title: chapterReward ? "Chapter Chest Opened" : "Stage Complete",
      coins,
      freeSpins,
      stardustReward,
      item,
      challengeFreeSpins,
      challengeCompleted,
      chapterReward
    });
  }

  function closeRewardModal() {
    dom.rewardModal.hidden = true;
    if (dom.rewardModalTitle.textContent === "Void Realm Result") {
      setTip("Returned to normal Campaign Mode. Keep spinning and merging.");
      return;
    }
    if (dom.rewardModalTitle.textContent === "Time Challenge Result") {
      setTip("Time Challenge ended. Spin again to start a new run.");
      return;
    }
    setTip("Next Stage unlocked. Keep spinning and merging.");
  }

  function closeJackpotModal() {
    dom.jackpotModal.hidden = true;
    setTip("Grand Jackpot reset. Keep building the next global prize.");
  }

  function openLevelRewardModal(reward) {
    dom.rewardModalTitle.textContent = reward.title;
    dom.rewardModalText.innerHTML = [
      `<span>Coins <strong class="rolling-number" data-value="${reward.coins}">0</strong></span>`,
      `<span>Free Spins <strong class="rolling-number" data-value="${reward.freeSpins}">0</strong></span>`,
      `<span>Stardust <strong class="rolling-number" data-value="${reward.stardustReward}">0</strong></span>`,
      `<span>${ITEM_LABELS[reward.item]} <strong class="rolling-number" data-value="1">0</strong></span>`,
      reward.challengeCompleted ? `<span>Side Challenge <strong class="rolling-number" data-value="${reward.challengeFreeSpins}">0</strong> Free Spins</span>` : "",
      reward.chapterReward ? `<span>Chapter Perk <strong class="rolling-number" data-value="1">0</strong></span>` : ""
    ].filter(Boolean).join("");
    dom.rewardModal.hidden = false;
    playSound("victory");
    updateBgmMix();
    triggerHaptic("epic", { force: true });
    createRewardCelebration();
    animateRewardCounters(dom.rewardModal);
    scheduleTextFit(dom.rewardModal);
  }

  function animateRewardCounters(container) {
    const counters = [...container.querySelectorAll(".rolling-number")];
    if (counters.length === 0) {
      return;
    }

    const start = performance.now();
    const duration = 720;
    const tick = (now) => {
      const progress = Math.min(1, (now - start) / duration);
      const eased = 1 - Math.pow(1 - progress, 3);
      counters.forEach((counter) => {
        const value = Math.floor(Number(counter.dataset.value || 0) * eased);
        counter.textContent = formatNumber(value);
      });
      if (progress < 1) {
        requestAnimationFrame(tick);
      } else {
        counters.forEach((counter) => {
          counter.textContent = formatNumber(Number(counter.dataset.value || 0));
        });
      }
    };
    requestAnimationFrame(tick);
  }

  function createRewardCelebration() {
    if (!canFx("particles")) {
      return;
    }

    const rect = dom.rewardModal.getBoundingClientRect();
    createParticles(rect.left + rect.width / 2, rect.top + rect.height * 0.38, 46, "#ffd45c", 180);
    createParticles(rect.left + rect.width / 2, rect.top + rect.height * 0.42, 30, "#4edcff", 150);
  }

  function playJackpotBreakFeedback(prize, cell) {
    showFloatText(`JACKPOT +${formatNumber(prize)}`, cell, "mega-jackpot");
    createCoinRain(64);
    createParticlesAtBoard(96, "#ffd45c", 240);
    createParticlesAtBoard(72, "#ffffff", 220);
    flashScreen("jackpot", 1600);
    shake("hard");
    playSound("jackpot");
    dom.jackpotModalText.textContent = `Won the full Grand Jackpot: ${formatNumber(prize)} Coins. Grand Jackpot reset to ${formatNumber(JACKPOT_INITIAL_VALUE)}.`;
    dom.jackpotModal.hidden = false;
    renderHud();
    saveState();
    scheduleTextFit(dom.jackpotModal);
  }

  function grantFreeSpins(amount) {
    if (!isAdventureMode()) {
      return;
    }

    const total = state.freeSpins + amount;
    const cap = getFreeSpinCap();
    if (total <= cap) {
      state.freeSpins = total;
      playSound("freeSpin");
      return;
    }

    const overflow = total - cap;
    state.freeSpins = cap;
    awardCoins(overflow * getSpinCost());
  }

  function beginComboChain() {
    if (runtime.comboFadeTimer) {
      window.clearTimeout(runtime.comboFadeTimer);
    }
    if (runtime.comboResetTimer) {
      window.clearTimeout(runtime.comboResetTimer);
    }
    document.body.classList.remove("combo-fading");
    runtime.combo = 0;
    renderComboHud();
    updateBgmMix();
  }

  function bumpCombo(label, cell) {
    document.body.classList.remove("combo-fading");
    runtime.combo += 1;
    updateModeComboBest(runtime.combo);
    if (isAdventureMode()) {
      state.bestCombo = Math.max(state.bestCombo || 0, runtime.combo);
      trackEvent("comboThreshold", { amount: 1, combo: runtime.combo }, cell);
      if (runtime.combo >= 5) {
        grantAchievement("combo5", cell);
      }
      if (runtime.combo >= 10) {
        grantAchievement("combo10", cell);
      }
    }
    renderComboHud();
    updateChallengeProgress("combo", runtime.combo, "max");
    pulseComboHud();
    playSound("combo", { combo: runtime.combo });
    if (runtime.combo === 10 || runtime.combo === 15) {
      playSound("sideChallenge");
      triggerHaptic("milestone", { force: true });
    }
    if (runtime.combo >= 10) {
      flashComboMilestone();
    }
  }

  function showComboText(label, cell) {
    void label;
    void cell;
  }

  function getComboMultiplier() {
    if (runtime.combo >= 10) {
      return hasTalent("comboCap") ? TALENT_COMBO_CAP_MULTIPLIER : 2.2;
    }
    if (runtime.combo >= 8) {
      return 1.8;
    }
    if (runtime.combo >= 5) {
      return 1.5;
    }
    if (runtime.combo >= 3) {
      return 1.2;
    }
    return 1;
  }

  function scheduleComboFade() {
    if (runtime.combo <= 0) {
      return;
    }

    if (runtime.comboFadeTimer) {
      window.clearTimeout(runtime.comboFadeTimer);
    }
    if (runtime.comboResetTimer) {
      window.clearTimeout(runtime.comboResetTimer);
    }

    runtime.comboFadeTimer = window.setTimeout(() => {
      document.body.classList.add("combo-fading");
    }, 2000);
    runtime.comboResetTimer = window.setTimeout(() => {
      runtime.combo = 0;
      document.body.classList.remove("combo-fading");
      renderComboHud();
      updateBgmMix();
    }, 3000);
  }

  function updateChallengeProgress(type, amount, mode) {
    if (!isAdventureMode() || runtime.voidBonusActive) {
      return;
    }

    if (state.challengeType !== type || state.challengeProgress >= state.challengeRequired) {
      return;
    }

    const wasComplete = state.challengeProgress >= state.challengeRequired;
    if (mode === "max") {
      state.challengeProgress = Math.max(state.challengeProgress, Math.min(amount, state.challengeRequired));
    } else {
      state.challengeProgress = Math.min(state.challengeRequired, state.challengeProgress + amount);
    }
    if (!wasComplete && state.challengeProgress >= state.challengeRequired) {
      grantStageChallengeSpinBonus();
    }
    renderHud();
  }

  function grantStageChallengeSpinBonus() {
    if (!isAdventureMode() || state.stageChallengeSpinAwarded) {
      return;
    }

    state.stageChallengeSpinAwarded = true;
    state.stageSpinsLeft += STAGE_QUOTA_BONUS_SPINS;
    playSound("sideChallenge");
    showFloatText(`Side Challenge +${STAGE_QUOTA_BONUS_SPINS} Stage Spins`, null, "stardust");
    setTip(`Side Challenge completed. Stage Spins Left +${STAGE_QUOTA_BONUS_SPINS}.`);
  }

  function addRiftEnergy(amount) {
    if (!isAdventureMode() || amount <= 0 || state.riftReady || state.voidBonusReady || runtime.voidBonusActive) {
      return;
    }

    state.riftEnergy = Math.min(RIFT_MAX, state.riftEnergy + amount * getEnergyChargeMultiplier());
    if (state.riftEnergy >= RIFT_MAX) {
      state.riftEnergy = RIFT_MAX;
      playSound("riftFull");
      if (Math.random() < VOID_BONUS_TRIGGER_CHANCE) {
        state.voidBonusReady = true;
        state.riftReady = false;
        playRiftEnergyBurst("Void Realm Open");
        setTip("Rift Energy overflowed: Void Realm is ready. Tap SPIN to enter 3 free realm rounds.");
      } else {
        state.riftReady = true;
        state.voidBonusReady = false;
        playRiftEnergyBurst("Neon Rift Charged");
        setTip("Rift Energy full: next spin guarantees +1 crystal tier, doubles special rates, and boosts merge/line rewards x1.5.");
      }
    }
    renderRiftHud();
  }

  function addClearEffectRiftEnergy(clearedCount) {
    if (clearedCount > 0) {
      trackEvent("clear", { amount: clearedCount });
      addRiftEnergy(5);
    }
  }

  function playRiftEnergyBurst(text) {
    showFloatText(text, null, "rift");
    flashScreen("rift", 900);
    shake("medium");
    createParticlesAtRiftMeter(72, "#4edcff", 170);
    createParticlesAtRiftMeter(42, "#ff6f9e", 150);
  }

  function playRiftActivationFeedback() {
    playRiftEnergyBurst("Neon Rift Active");
  }

  function playVoidBonusStartFeedback() {
    showFloatText("Void Realm", null, "void");
    flashScreen("void", 1200);
    shake("medium");
    createParticlesAtBoard(86, "#a78bfa", 210);
    createParticlesAtBoard(58, "#ff6f9e", 190);
  }

  function playVoidBonusEndFeedback(baseTotal, payout) {
    showFloatText(`Realm Result +${formatNumber(payout)}`, null, "void");
    createCoinRain(42);
    createParticlesAtBoard(80, "#ffd45c", 210);
    createParticlesAtBoard(64, "#a78bfa", 190);
    flashScreen("void", 1050);
    shake("medium");
    dom.rewardModalTitle.textContent = "Void Realm Result";
    dom.rewardModalText.textContent = `Realm winnings ${formatNumber(baseTotal)}. Paid ${formatNumber(payout)} Coins after doubling.`;
    dom.rewardModal.hidden = false;
    scheduleTextFit(dom.rewardModal);
  }

  function collectElementSplashCells(originCells, blockedCells) {
    const blocked = new Set((blockedCells || originCells).map(cellKey));
    const reserved = new Set(blocked);
    const splashCells = [];

    originCells.forEach((origin) => {
      const originSymbol = getGem(origin);
      if (!isGem(originSymbol)) {
        return;
      }

      const defeatedElement = getDefeatedElement(originSymbol.element);
      if (!defeatedElement) {
        return;
      }

      const target = getNeighbors(origin).find((neighbor) => {
        const key = cellKey(neighbor);
        const symbol = getGem(neighbor);
        return !reserved.has(key) && isGem(symbol) && normalizeElementKey(symbol.element) === defeatedElement;
      });
      if (target) {
        reserved.add(cellKey(target));
        splashCells.push(target);
      }
    });

    return splashCells;
  }

  function clearSplashCells(cells) {
    let cleared = 0;
    cells.forEach((cell) => {
      const symbol = getGem(cell);
      if (!isGem(symbol)) {
        return;
      }

      cleared += 1;
      state.board[cell.r][cell.c] = null;
      createParticlesAtCell(cell, 16, ELEMENTS[normalizeElementKey(symbol.element)].color, 92);
      showFloatText("Element Splash", cell, "element");
    });
    return cleared;
  }

  function getDefeatedElement(element) {
    const elementKey = normalizeElementKey(element);
    return ELEMENTS[elementKey] ? ELEMENTS[elementKey].beats : null;
  }

  function getChallengeTitle() {
    const template = CHALLENGE_LABELS[state.challengeType] || "Complete the challenge";
    return template.replace("{n}", state.challengeRequired);
  }

  function findGroupAfterSwap(cells) {
    return findGroupsAfterSwap(cells)[0] || null;
  }

  function findGroupsAfterSwap(cells) {
    const seeds = [];
    cells.forEach((cell) => {
      seeds.push(cell);
      seeds.push(...getNeighbors(cell));
    });

    const groups = [];
    for (const seed of seeds) {
      for (const level of getCandidateMergeLevels(seed)) {
        const visited = new Set();
        const group = collectConnected(seed, level, visited);
        if (group.length >= 3 && group.some((cell) => cells.some((changed) => sameCell(cell, changed)))) {
          groups.push(group);
        }
      }
    }

    groups.sort(sortMergeGroups);
    return groups;
  }

  function findNextMergeGroup() {
    const groups = findAllMergeGroups();
    groups.sort(sortMergeGroups);
    return groups[0] || null;
  }

  function findNextCascadeMergeGroup() {
    const groups = findAllMergeGroups();
    groups.sort(sortCascadeMergeGroups);
    return groups[0] || null;
  }

  function findAllMergeGroups() {
    const groups = [];
    const visited = new Set();

    for (let r = 0; r < ROWS; r += 1) {
      for (let c = 0; c < COLS; c += 1) {
        const cell = { r, c };
        for (const level of getCandidateMergeLevels(cell)) {
          const key = `${level}:${cellKey(cell)}`;
          if (visited.has(key)) {
            continue;
          }

          const group = collectConnected(cell, level, visited);
          if (group.length >= 3) {
            groups.push(group);
          }
        }
      }
    }

    return groups;
  }

  function sortMergeGroups(a, b) {
    const levelA = getGroupLevel(a) || 0;
    const levelB = getGroupLevel(b) || 0;
    if (levelA !== levelB) {
      return levelB - levelA;
    }
    return b.length - a.length;
  }

  function sortCascadeMergeGroups(a, b) {
    const targetA = chooseMergeTarget(a);
    const targetB = chooseMergeTarget(b);
    if (targetA.r !== targetB.r) {
      return targetB.r - targetA.r;
    }
    if (targetA.c !== targetB.c) {
      return targetA.c - targetB.c;
    }
    return sortMergeGroups(a, b);
  }

  function collectConnected(start, targetLevel, visited) {
    const result = [];
    const queue = [start];

    while (queue.length > 0) {
      const cell = queue.shift();
      const key = `${targetLevel}:${cellKey(cell)}`;
      if (visited.has(key)) {
        continue;
      }

      if (!matchesMergeLevel(getGem(cell), targetLevel)) {
        continue;
      }

      visited.add(key);
      result.push(cell);
      getNeighbors(cell).forEach((next) => {
        const nextKey = `${targetLevel}:${cellKey(next)}`;
        if (!visited.has(nextKey)) {
          queue.push(next);
        }
      });
    }

    return result;
  }

  function matchesMergeLevel(symbol, level) {
    if (!symbol) {
      return false;
    }
    if (symbol.kind === "gem") {
      const effectiveLevel = getEffectiveGemLevel(symbol);
      return effectiveLevel === level && effectiveLevel < 5;
    }
    return symbol.kind === "special" && symbol.special === "wild";
  }

  function matchesLineLevel(symbol, level) {
    if (!symbol) {
      return false;
    }
    return symbol.kind === "gem" && getEffectiveGemLevel(symbol) === level;
  }

  function getCandidateMergeLevels(cell) {
    const symbol = getGem(cell);
    if (!symbol) {
      return [];
    }

    if (symbol.kind === "gem") {
      const effectiveLevel = getEffectiveGemLevel(symbol);
      return effectiveLevel < 5 ? [effectiveLevel] : [];
    }

    if (symbol.kind !== "special" || symbol.special !== "wild") {
      return [];
    }

    const levels = new Set();
    getNeighbors(cell).forEach((neighbor) => {
      const neighborSymbol = getGem(neighbor);
      if (neighborSymbol && neighborSymbol.kind === "gem" && getEffectiveGemLevel(neighborSymbol) < 5) {
        levels.add(getEffectiveGemLevel(neighborSymbol));
      }
    });
    return [...levels];
  }

  function getGroupLevel(group) {
    for (const cell of group) {
      const symbol = getGem(cell);
      if (symbol && symbol.kind === "gem" && getEffectiveGemLevel(symbol) < 5) {
        return getEffectiveGemLevel(symbol);
      }
    }
    return null;
  }

  function chooseMergeTarget(group) {
    return group.reduce((best, cell) => {
      if (cell.r > best.r) {
        return cell;
      }
      if (cell.r === best.r && Math.abs(cell.c - 2) < Math.abs(best.c - 2)) {
        return cell;
      }
      return best;
    }, group[0]);
  }

  function findCellsByLevel(level) {
    return allCells().filter((cell) => {
      const symbol = getGem(cell);
      return symbol && symbol.kind === "gem" && getEffectiveGemLevel(symbol) === level;
    });
  }

  function pickLeapGroup(candidates, clickedCell) {
    const group = [clickedCell];
    candidates.forEach((cell) => {
      if (group.length < 3 && !sameCell(cell, clickedCell)) {
        group.push(cell);
      }
    });
    return group;
  }

  function getNeighbors(cell) {
    return [
      { r: cell.r - 1, c: cell.c },
      { r: cell.r + 1, c: cell.c },
      { r: cell.r, c: cell.c - 1 },
      { r: cell.r, c: cell.c + 1 }
    ].filter(isInsideBoard);
  }

  function getAreaCells(center, radius) {
    const cells = [];
    for (let r = center.r - radius; r <= center.r + radius; r += 1) {
      for (let c = center.c - radius; c <= center.c + radius; c += 1) {
        const cell = { r, c };
        if (isInsideBoard(cell)) {
          cells.push(cell);
        }
      }
    }
    return cells;
  }

  function allCells() {
    const cells = [];
    for (let r = 0; r < ROWS; r += 1) {
      for (let c = 0; c < COLS; c += 1) {
        cells.push({ r, c });
      }
    }
    return cells;
  }

  function createSpinDropTable(specialProbabilityMultiplier, nativeHighTierChance) {
    const totalWeight = 10000;
    const multiplier = specialProbabilityMultiplier || 1;
    const bombWeight = Math.round(SPECIAL_SPAWN_CONFIG.bomb.probability * totalWeight * multiplier);
    const wildWeight = Math.round(SPECIAL_SPAWN_CONFIG.wild.probability * totalWeight * multiplier);
    const coinWeight = Math.round(SPECIAL_SPAWN_CONFIG.coin.probability * totalWeight * multiplier);
    const gemWeight = totalWeight - bombWeight - wildWeight - coinWeight;
    const highTierChance = clampNumber((nativeHighTierChance || 0) * NATIVE_HIGH_TIER_WEIGHT_MULTIPLIER, 0, 0.3);
    const highTierWeight = Math.round(gemWeight * highTierChance);
    const lowTierWeight = gemWeight - highTierWeight;
    const ratios = [
      { level: 1, ratio: 0.62 },
      { level: 2, ratio: 0.28 },
      { level: 3, ratio: 0.10 }
    ];
    const gemDrops = ratios.map((item, index) => {
      const previous = ratios.slice(0, index).reduce((sum, prev) => sum + Math.round(lowTierWeight * prev.ratio), 0);
      const weight = index === ratios.length - 1 ? lowTierWeight - previous : Math.round(lowTierWeight * item.ratio);
      return { kind: "gem", level: item.level, weight };
    });
    if (highTierWeight > 0) {
      const level4Weight = Math.round(highTierWeight * 0.8);
      gemDrops.push({ kind: "gem", level: 4, weight: level4Weight });
      gemDrops.push({ kind: "gem", level: 5, weight: highTierWeight - level4Weight });
    }

    return [
      ...gemDrops,
      { kind: "special", special: "bomb", weight: bombWeight },
      { kind: "special", special: "wild", weight: wildWeight },
      { kind: "special", special: "coin", weight: coinWeight }
    ].filter((item) => item.weight > 0);
  }

  function createVoidSpinDropTable(specialProbabilityMultiplier) {
    const totalWeight = 10000;
    const multiplier = VOID_SPECIAL_PROBABILITY_MULTIPLIER * (specialProbabilityMultiplier || 1);
    const bombWeight = Math.round(SPECIAL_SPAWN_CONFIG.bomb.probability * totalWeight * multiplier);
    const wildWeight = Math.round(SPECIAL_SPAWN_CONFIG.wild.probability * totalWeight * multiplier);
    const coinWeight = Math.round(SPECIAL_SPAWN_CONFIG.coin.probability * totalWeight * multiplier);
    const gemWeight = totalWeight - bombWeight - wildWeight - coinWeight;
    const ratios = [
      { level: 2, ratio: 0.60 },
      { level: 3, ratio: 0.28 },
      { level: 4, ratio: 0.09 },
      { level: 5, ratio: 0.03 }
    ];
    const gemDrops = ratios.map((item, index) => {
      const previous = ratios.slice(0, index).reduce((sum, prev) => sum + Math.round(gemWeight * prev.ratio), 0);
      const weight = index === ratios.length - 1 ? gemWeight - previous : Math.round(gemWeight * item.ratio);
      return { kind: "gem", level: item.level, weight };
    });

    return [
      ...gemDrops,
      { kind: "special", special: "bomb", weight: bombWeight },
      { kind: "special", special: "wild", weight: wildWeight },
      { kind: "special", special: "coin", weight: coinWeight }
    ].filter((item) => item.weight > 0);
  }

  function createBoard(dropTable, levelBoost) {
    const board = [];
    for (let r = 0; r < ROWS; r += 1) {
      const row = [];
      for (let c = 0; c < COLS; c += 1) {
        row.push(createDrop(dropTable, levelBoost));
      }
      board.push(row);
    }
    return board;
  }

  function createDrop(dropTable, levelBoost) {
    const item = weightedItem(dropTable);
    if (item.kind === "special") {
      return createSpecial(item.special);
    }
    return createGem(Math.min(5, item.level + (levelBoost || 0)));
  }

  function createGem(level, element) {
    return {
      id: nextGemId++,
      kind: "gem",
      level,
      color: level <= 3 ? COLORS[Math.floor(Math.random() * COLORS.length)] : "mythic",
      element: normalizeElementKey(element)
    };
  }

  function createSpecial(special) {
    return {
      id: nextGemId++,
      kind: "special",
      special,
      level: 0,
      color: "special"
    };
  }

  function weightedItem(items) {
    const total = items.reduce((sum, item) => sum + item.weight, 0);
    let roll = Math.random() * total;
    for (const item of items) {
      roll -= item.weight;
      if (roll <= 0) {
        return item;
      }
    }
    return items[0];
  }

  function placeGuaranteedGem(level) {
    const cell = randomCell();
    state.board[cell.r][cell.c] = createGem(level);
  }

  function applyStagePityGuarantee() {
    if (!isAdventureMode() || runtime.voidBonusActive) {
      return;
    }

    normalizeStagePressureState(state);
    const pity = state.stagePity;
    const hasTier3Plus = boardHasGemAtLeast(3);
    const hasTier4Plus = boardHasGemAtLeast(4);
    pity.noTier3 = hasTier3Plus ? 0 : pity.noTier3 + 1;
    pity.noTier4 = hasTier4Plus ? 0 : pity.noTier4 + 1;

    if (pity.triggers >= STAGE_PITY_TRIGGER_LIMIT) {
      return;
    }

    if (pity.noTier4 >= PITY_TIER4_LIMIT) {
      placeGuaranteedGem(4);
      pity.noTier4 = 0;
      pity.noTier3 = 0;
      pity.triggers += 1;
      showFloatText("Stage Pity: Tier4", null, "rift");
      return;
    }

    if (pity.noTier3 >= PITY_TIER3_LIMIT) {
      placeGuaranteedGem(3);
      pity.noTier3 = 0;
      pity.triggers += 1;
      showFloatText("Stage Pity: Tier3", null, "rift");
    }
  }

  function boardHasGemAtLeast(level) {
    return state.board.some((row) => row.some((symbol) => symbol && symbol.kind === "gem" && symbol.level >= level));
  }

  function placeSpecial(special) {
    const cell = randomCell();
    state.board[cell.r][cell.c] = createSpecial(special);
  }

  function randomCell() {
    return {
      r: Math.floor(Math.random() * ROWS),
      c: Math.floor(Math.random() * COLS)
    };
  }

  function randomItemKey() {
    const keys = Object.keys(ITEM_LABELS);
    return keys[Math.floor(Math.random() * keys.length)];
  }

  function swapCells(a, b) {
    const temp = state.board[a.r][a.c];
    state.board[a.r][a.c] = state.board[b.r][b.c];
    state.board[b.r][b.c] = temp;
  }

  function getGem(cell) {
    if (!isInsideBoard(cell)) {
      return null;
    }
    return state.board[cell.r][cell.c];
  }

  function isGem(symbol) {
    return Boolean(symbol && symbol.kind === "gem");
  }

  function isLowTierGem(symbol) {
    return Boolean(symbol && symbol.kind === "gem" && symbol.level <= 2);
  }

  function isGoldCore(symbol) {
    return Boolean(symbol && symbol.kind === "special" && symbol.special === "coin");
  }

  function areAdjacent(a, b) {
    return Math.abs(a.r - b.r) + Math.abs(a.c - b.c) === 1;
  }

  function sameCell(a, b) {
    return a && b && a.r === b.r && a.c === b.c;
  }

  function isInsideBoard(cell) {
    return cell && cell.r >= 0 && cell.r < ROWS && cell.c >= 0 && cell.c < COLS;
  }

  function cellKey(cell) {
    return `${cell.r},${cell.c}`;
  }

  function getCellFromEvent(event) {
    const element = event.target.closest(".cell");
    return element ? readCell(element) : null;
  }

  function readCell(element) {
    if (!element || element.dataset.r === undefined || element.dataset.c === undefined) {
      return null;
    }
    return {
      r: Number(element.dataset.r),
      c: Number(element.dataset.c)
    };
  }

  function getIncomeBoost() {
    const freeSpinBoost = runtime.freeSpinRound ? 1.5 : 1;
    const riftBoost = runtime.riftSpinRound ? 1.5 : 1;
    return freeSpinBoost * riftBoost * runtime.lineBoost * runtime.fullBoost * getComboMultiplier();
  }

  function canFx(key) {
    return FX_CONFIG.enabled && FX_CONFIG[key] !== false;
  }

  function updateAudioSettings(settings) {
    const normalized = normalizeSettings(settings || state.settings);
    if (audioRuntime && audioRuntime.master) {
      const dynamicScale = getAudioDynamicScale();
      audioRuntime.master.gain.setTargetAtTime(normalized.sound ? normalized.volume * dynamicScale : 0, audioRuntime.context.currentTime, 0.035);
      if (!normalized.sound) {
        pauseBgmDecks();
      } else if (audioRuntime.unlocked && !document.hidden) {
        startBgmDecks();
      }
      updateBgmMix();
    }
  }

  function getAudioDynamicScale() {
    const strength = state && state.settings ? state.settings.hapticStrength : "high";
    const deviceScale = document.body.classList.contains("is-tablet") ? 0.86 : 1;
    if (strength === "low") {
      return 0.86 * deviceScale;
    }
    if (strength === "medium") {
      return 0.94 * deviceScale;
    }
    return deviceScale;
  }

  function ensureAudio() {
    const AudioContext = window.AudioContext || window.webkitAudioContext;
    if (!AudioContext) {
      return null;
    }

    if (!audioRuntime) {
      const context = new AudioContext();
      const master = context.createGain();
      const compressor = context.createDynamicsCompressor();
      compressor.threshold.value = -15;
      compressor.knee.value = 22;
      compressor.ratio.value = 4;
      compressor.attack.value = 0.002;
      compressor.release.value = 0.12;
      master.connect(compressor);
      compressor.connect(context.destination);
      audioRuntime = {
        context,
        master,
        unlocked: false,
        bgmStarted: false,
        bgmDecks: createBgmDecks(),
        bgmFadeTimer: null,
        lastSoundAt: Object.create(null),
        activeSfx: 0
      };
      updateAudioSettings(state.settings);
    }

    if (audioRuntime.context.state === "suspended") {
      audioRuntime.context.resume();
    }
    return audioRuntime;
  }

  function createBgmDecks() {
    const decks = {};
    Object.keys(BGM_TRACKS).forEach((key) => {
      const element = new Audio(BGM_TRACKS[key]);
      element.loop = true;
      element.preload = "auto";
      element.playsInline = true;
      element.volume = 0;
      decks[key] = element;
    });
    return decks;
  }

  function unlockAudioExperience() {
    const settings = normalizeSettings(state.settings);
    if (!settings.sound || settings.volume <= 0) {
      return;
    }
    requestNativeAudioActivation();
    const audio = ensureAudio();
    if (!audio) {
      return;
    }
    audio.unlocked = true;
    if (audio.context.state === "suspended") {
      audio.context.resume();
    }
    startBgmDecks();
  }

  function requestNativeAudioActivation() {
    const bridge = window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.nativeAudio;
    if (!bridge || typeof bridge.postMessage !== "function") {
      return;
    }
    try {
      bridge.postMessage({ action: "activate" });
    } catch (error) {
      void error;
    }
  }

  function hasNativeAudioBridge() {
    const bridge = window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.nativeAudio;
    return Boolean(bridge && typeof bridge.postMessage === "function");
  }

  function postNativeAudio(message) {
    if (!hasNativeAudioBridge()) {
      return false;
    }
    try {
      window.webkit.messageHandlers.nativeAudio.postMessage(message);
      return true;
    } catch (error) {
      void error;
      return false;
    }
  }

  function startBgmDecks() {
    const audio = audioRuntime;
    if (!audio || !audio.unlocked || document.hidden) {
      return;
    }
    const settings = normalizeSettings(state.settings);
    if (!settings.sound || settings.volume <= 0) {
      return;
    }
    if (hasNativeAudioBridge()) {
      audio.bgmStarted = true;
      updateBgmMix(true);
      return;
    }
    Object.values(audio.bgmDecks).forEach((deck) => {
      if (deck.paused) {
        deck.play().catch(() => {});
      }
    });
    audio.bgmStarted = true;
    updateBgmMix(true);
  }

  function pauseBgmDecks() {
    if (!audioRuntime || !audioRuntime.bgmDecks) {
      return;
    }
    Object.values(audioRuntime.bgmDecks).forEach((deck) => deck.pause());
    audioRuntime.bgmStarted = false;
  }

  function pauseAudioExperience() {
    if (!audioRuntime) {
      return;
    }
    pauseBgmDecks();
    postNativeAudio({ action: "pause" });
    if (audioRuntime.context.state === "running") {
      audioRuntime.context.suspend();
    }
  }

  function resumeAudioExperience() {
    if (!audioRuntime || !audioRuntime.unlocked) {
      return;
    }
    requestNativeAudioActivation();
    if (audioRuntime.context.state === "suspended") {
      audioRuntime.context.resume();
    }
    startBgmDecks();
  }

  function setupAudioSceneObserver() {
    const observer = new MutationObserver((mutations) => {
      if (mutations.some((mutation) => mutation.type === "attributes" && mutation.attributeName === "hidden")) {
        syncModalVisibilityState();
        updateBgmMix();
      }
    });
    document.querySelectorAll(".modal-backdrop, #talentModal, #settingsModal").forEach((element) => {
      observer.observe(element, { attributes: true, attributeFilter: ["hidden"] });
    });
    syncModalVisibilityState();
  }

  function syncModalVisibilityState() {
    const modalVisible = Array.from(document.querySelectorAll(".modal-backdrop"))
      .some((modal) => !modal.hidden);
    document.body.classList.toggle("modal-open", modalVisible);
  }

  function getBgmScene() {
    if (!document.body.classList.contains("game-started")) {
      return "menu";
    }
    if ((dom.stageFailModal && !dom.stageFailModal.hidden) || document.body.classList.contains("stage-failed")) {
      return "fail";
    }
    return "game";
  }

  function isBgmDucked() {
    return Boolean(
      (dom.settingsModal && !dom.settingsModal.hidden)
      || (dom.talentModal && !dom.talentModal.hidden)
    );
  }

  function updateBgmMix(immediate) {
    const settings = normalizeSettings(state.settings);
    const scene = getBgmScene();
    const deviceScale = document.body.classList.contains("is-tablet") ? 0.82 : 1;
    const duckScale = isBgmDucked() ? 0.65 : 1;
    const enabledScale = settings.sound && !document.hidden ? settings.volume * deviceScale * duckScale : 0;
    const settlementVisible = Boolean(dom.rewardModal && !dom.rewardModal.hidden);
    const comboScale = scene === "game" && !settlementVisible ? Math.min(1, Math.max(0, runtime.combo - 2) / 10) : 0;
    const targets = {
      menu: scene === "menu" ? 0.2 * enabledScale : 0,
      game: scene === "game" ? 0.23 * enabledScale : 0,
      fail: scene === "fail" ? 0.19 * enabledScale : 0,
      combo: 0.075 * enabledScale * comboScale
    };

    if (postNativeAudio({ action: "mix", volumes: targets })) {
      if (audioRuntime && audioRuntime.bgmDecks) {
        Object.values(audioRuntime.bgmDecks).forEach((deck) => {
          deck.volume = 0;
          deck.pause();
        });
      }
      return;
    }

    const audio = audioRuntime;
    if (!audio || !audio.bgmDecks) {
      return;
    }

    if (audio.bgmFadeTimer) {
      window.clearInterval(audio.bgmFadeTimer);
      audio.bgmFadeTimer = null;
    }
    const steps = immediate ? 1 : 12;
    let step = 0;
    const starts = {};
    Object.keys(targets).forEach((key) => {
      starts[key] = audio.bgmDecks[key].volume;
    });
    const applyStep = () => {
      step += 1;
      const progress = Math.min(1, step / steps);
      Object.keys(targets).forEach((key) => {
        audio.bgmDecks[key].volume = clampNumber(starts[key] + (targets[key] - starts[key]) * progress, 0, 1);
      });
      if (progress >= 1 && audio.bgmFadeTimer) {
        window.clearInterval(audio.bgmFadeTimer);
        audio.bgmFadeTimer = null;
      }
    };
    applyStep();
    if (steps > 1) {
      audio.bgmFadeTimer = window.setInterval(applyStep, 45);
    }
  }

  function canStartSfx(type) {
    const audio = audioRuntime;
    if (!audio) {
      return false;
    }
    const now = performance.now();
    const cooldown = AUDIO_COOLDOWNS[type] || 20;
    if (now - (audio.lastSoundAt[type] || 0) < cooldown) {
      return false;
    }
    const maxVoices = perf.profile === "small" ? 8 : perf.profile === "tablet" ? 10 : 12;
    const priority = ["burst", "riftFull", "victory", "fail", "skillBlast", "jackpot"].includes(type);
    if (!priority && audio.activeSfx >= maxVoices) {
      return false;
    }
    audio.lastSoundAt[type] = now;
    audio.activeSfx += 1;
    window.setTimeout(() => {
      if (audioRuntime) {
        audioRuntime.activeSfx = Math.max(0, audioRuntime.activeSfx - 1);
      }
    }, priority ? 1100 : 460);
    return true;
  }

  function playSound(type, options) {
    const settings = normalizeSettings(state.settings);
    if (!settings.sound || settings.volume <= 0) {
      return;
    }

    const audio = ensureAudio();
    if (!audio) {
      return;
    }
    if (!canStartSfx(type)) {
      return;
    }

    const level = options && options.level ? options.level : 1;
    const combo = options && options.combo ? options.combo : runtime.combo;
    const chain = options && options.chain ? options.chain : 0;
    if (type === "button") {
      playCrystalTone(1320, 0.055, 0.048);
      return;
    }
    if (type === "tooltip") {
      playCrystalTone(1680, 0.038, 0.016);
      return;
    }
    if (type === "panelOpen") {
      playCrystalTone(1046, 0.075, 0.046);
      playCrystalTone(1397, 0.09, 0.036, 0.028);
      updateBgmMix();
      return;
    }
    if (type === "panelClose") {
      playCrystalTone(1318, 0.065, 0.04);
      playCrystalTone(988, 0.075, 0.032, 0.024);
      updateBgmMix();
      return;
    }
    if (type === "lock") {
      playCrystalTone(720, 0.06, 0.048);
      return;
    }
    if (type === "undo") {
      playCrystalTone(1175, 0.065, 0.04);
      playCrystalTone(988, 0.07, 0.032, 0.026);
      playCrystalTone(1318, 0.075, 0.026, 0.052);
      return;
    }
    if (type === "talent") {
      [1318, 1760, 2093].forEach((frequency, index) => playCrystalTone(frequency, 0.09, 0.032, index * 0.034));
      return;
    }
    if (type === "reset") {
      playCrystalTone(880, 0.07, 0.044);
      playCrystalTone(1175, 0.08, 0.032, 0.038);
      return;
    }
    if (type === "spinPress") {
      playCrystalTone(1046, 0.085, 0.06);
      playCrystalTone(1568, 0.1, 0.044, 0.042);
      return;
    }
    if (type === "skillCharge") {
      playStaticChargeSound();
      return;
    }
    if (type === "skillDenied") {
      playCrystalTone(420, 0.075, 0.04);
      playCrystalTone(360, 0.08, 0.028, 0.038);
      return;
    }
    if (type === "skillBlast") {
      playBurstSound(5);
      playStaticChargeSound(true);
      return;
    }
    if (type === "drop") {
      playCrystalTone(680 + level * 145, 0.055 + level * 0.012, 0.032 + level * 0.004);
      return;
    }
    if (type === "coin") {
      const layers = Math.min(4, Math.max(1, options && options.layers ? options.layers : 1));
      for (let i = 0; i < layers; i += 1) {
        playCrystalTone(1180 + i * 170, 0.055, 0.03, i * 0.022);
      }
      return;
    }
    if (type === "freeSpin") {
      [880, 1175, 1568].forEach((frequency, index) => playCrystalTone(frequency, 0.1, 0.044, index * 0.045));
      return;
    }
    if (type === "warning") {
      playCrystalTone(320, 0.13, 0.022);
      return;
    }
    if (type === "modeSelect") {
      playModeSelectSound();
      return;
    }
    if (type === "exit") {
      playExitSound();
      return;
    }
    if (type === "spinStart") {
      playSpinStartSound();
      return;
    }
    if (type === "spinStop") {
      playCrystalTone(1175, 0.075, 0.065);
      playCrystalTone(1568, 0.085, 0.04, 0.035);
      return;
    }
    if (type === "merge") {
      const chainPitch = Math.min(520, chain * 44);
      const base = 820 + level * 150 + chainPitch;
      const duration = 0.075 + level * 0.014;
      playCrystalTone(base, duration, 0.052 + level * 0.003);
      if (level >= 3) {
        playCrystalTone(base * 1.25, duration * 0.92, 0.036, 0.032);
      }
      if (level >= 4) {
        playCrystalTone(base * 1.58, duration * 0.82, 0.026, 0.062);
      }
      if (level >= 5) {
        playCrystalTone(base * 1.92, duration * 0.76, 0.022, 0.094);
      }
      return;
    }
    if (type === "combo") {
      playCrystalTone(1046 + Math.min(combo, 15) * 46, 0.06, 0.048);
      updateBgmMix();
      return;
    }
    if (type === "burst") {
      playBurstSound(level);
      return;
    }
    if (type === "riftFull") {
      playRiftFullSound();
      return;
    }
    if (type === "reward") {
      playRewardSound();
      return;
    }
    if (type === "sideChallenge") {
      [1175, 1568, 2093].forEach((frequency, index) => playCrystalTone(frequency, 0.085, 0.04, index * 0.036));
      return;
    }
    if (type === "jackpot") {
      playBurstSound(5);
      [1046, 1318, 1568, 2093].forEach((frequency, index) => playCrystalTone(frequency, 0.12, 0.045, 0.12 + index * 0.06));
      return;
    }
    if (type === "fail") {
      playFailSound();
      return;
    }
    if (type === "victory") {
      playVictorySound();
    }
  }

  function playCrystalTone(frequency, duration, volume, delayTime) {
    const audio = audioRuntime;
    if (!audio) {
      return;
    }

    const context = audio.context;
    const start = context.currentTime + (delayTime || 0);
    const baseFrequency = clampNumber(frequency, 260, 4200);
    const envelope = context.createGain();
    const highpass = context.createBiquadFilter();
    highpass.type = "highpass";
    highpass.frequency.value = 240;
    highpass.Q.value = 0.72;
    envelope.gain.setValueAtTime(0.0001, start);
    envelope.gain.exponentialRampToValueAtTime(Math.max(0.0001, volume), start + 0.0045);
    envelope.gain.exponentialRampToValueAtTime(0.0001, start + duration);
    highpass.connect(envelope);
    envelope.connect(audio.master);

    const partials = [
      { ratio: 1, amount: 0.74, type: "sine" },
      { ratio: 2.01, amount: 0.2, type: "sine" },
      { ratio: 3.93, amount: 0.065, type: "triangle" }
    ];
    partials.forEach((partial, index) => {
      const oscillator = context.createOscillator();
      const partialGain = context.createGain();
      const partialFrequency = Math.min(11000, baseFrequency * partial.ratio);
      oscillator.type = partial.type;
      oscillator.frequency.setValueAtTime(partialFrequency, start);
      oscillator.frequency.exponentialRampToValueAtTime(partialFrequency * (index === 0 ? 1.008 : 0.996), start + duration * 0.7);
      partialGain.gain.value = partial.amount;
      oscillator.connect(partialGain);
      partialGain.connect(highpass);
      oscillator.start(start);
      oscillator.stop(start + duration + 0.018);
    });
  }

  function playSpinStartSound() {
    playCrystalTone(988, 0.075, 0.05);
    playCrystalTone(1318, 0.085, 0.042, 0.045);
    playCrystalTone(1760, 0.095, 0.032, 0.09);
    playNoiseBurst(0.16, 0.018, 3600);
  }

  function playStaticChargeSound(blast) {
    const audio = audioRuntime;
    if (!audio) {
      return;
    }
    const duration = blast ? 0.26 : 0.15;
    const context = audio.context;
    const buffer = context.createBuffer(1, Math.floor(context.sampleRate * duration), context.sampleRate);
    const data = buffer.getChannelData(0);
    for (let i = 0; i < data.length; i += 1) {
      const progress = i / data.length;
      data[i] = (Math.random() * 2 - 1) * Math.sin(Math.PI * progress) * (blast ? 0.42 : 0.24);
    }
    const source = context.createBufferSource();
    const filter = context.createBiquadFilter();
    const gain = context.createGain();
    source.buffer = buffer;
    filter.type = "bandpass";
    filter.frequency.setValueAtTime(blast ? 2200 : 2800, context.currentTime);
    filter.frequency.exponentialRampToValueAtTime(blast ? 5600 : 4900, context.currentTime + duration);
    filter.Q.value = blast ? 2.2 : 3.4;
    gain.gain.setValueAtTime(0.0001, context.currentTime);
    gain.gain.exponentialRampToValueAtTime(blast ? 0.07 : 0.035, context.currentTime + duration * 0.38);
    gain.gain.exponentialRampToValueAtTime(0.0001, context.currentTime + duration);
    source.connect(filter);
    filter.connect(gain);
    gain.connect(audio.master);
    source.start();
  }

  function playModeSelectSound() {
    [988, 1318, 1760].forEach((frequency, index) => {
      playCrystalTone(frequency, 0.085, 0.044, index * 0.042);
    });
  }

  function playExitSound() {
    [1318, 1046, 784].forEach((frequency, index) => {
      playCrystalTone(frequency, 0.075, 0.038, index * 0.048);
    });
  }

  function playRewardSound() {
    [1046, 1318, 1568, 2093].forEach((frequency, index) => {
      playCrystalTone(frequency, 0.095, 0.046, index * 0.042);
    });
  }

  function playFailSound() {
    [784, 659, 523].forEach((frequency, index) => {
      playCrystalTone(frequency, 0.09, 0.04 - index * 0.006, index * 0.055);
    });
  }

  function playBurstSound(level) {
    const highTier = level >= 5;
    const notes = highTier ? [1046, 1318, 1568, 2093, 2637] : [880, 1175, 1568];
    notes.forEach((frequency, index) => {
      playCrystalTone(frequency, highTier ? 0.115 : 0.085, highTier ? 0.042 : 0.038, index * (highTier ? 0.036 : 0.032));
    });
    playNoiseBurst(highTier ? 0.2 : 0.12, highTier ? 0.04 : 0.026, highTier ? 4400 : 3600);
  }

  function playNoiseBurst(duration, volume, centerFrequency) {
    const audio = audioRuntime;
    if (!audio) {
      return;
    }

    const context = audio.context;
    const buffer = context.createBuffer(1, Math.floor(context.sampleRate * duration), context.sampleRate);
    const data = buffer.getChannelData(0);
    for (let i = 0; i < data.length; i += 1) {
      data[i] = (Math.random() * 2 - 1) * (1 - i / data.length);
    }
    const source = context.createBufferSource();
    const gain = context.createGain();
    const filter = context.createBiquadFilter();
    source.buffer = buffer;
    filter.type = "bandpass";
    filter.frequency.value = centerFrequency || 3400;
    filter.Q.value = 2.8;
    gain.gain.setValueAtTime(Math.max(0.0001, volume), context.currentTime);
    gain.gain.exponentialRampToValueAtTime(0.0001, context.currentTime + duration);
    source.connect(filter);
    filter.connect(gain);
    gain.connect(audio.master);
    source.start();
  }

  function playRiftFullSound() {
    [784, 988, 1175, 1568, 2093].forEach((frequency, index) => {
      playCrystalTone(frequency, 0.1, 0.045, index * 0.047);
    });
  }

  function playVictorySound() {
    [784, 988, 1175, 1568, 2093].forEach((frequency, index) => {
      playCrystalTone(frequency, 0.12, 0.052, index * 0.062);
    });
  }

  function highlightCells(cells, className, duration) {
    cells.forEach((cell) => {
      const element = findCellElement(cell);
      if (element) {
        element.classList.add(className);
      }
    });

    window.setTimeout(() => {
      cells.forEach((cell) => {
        const element = findCellElement(cell);
        if (element) {
          element.classList.remove(className);
        }
      });
    }, duration);
  }

  function highlightMergeCells(cells, targetCell, className, duration) {
    const targetElement = findCellElement(targetCell);
    const targetRect = targetElement ? targetElement.getBoundingClientRect() : null;
    const targetX = targetRect ? targetRect.left + targetRect.width / 2 : 0;
    const targetY = targetRect ? targetRect.top + targetRect.height / 2 : 0;

    cells.forEach((cell) => {
      const element = findCellElement(cell);
      if (!element) {
        return;
      }

      if (targetRect) {
        const rect = element.getBoundingClientRect();
        const dx = Math.round(targetX - (rect.left + rect.width / 2));
        const dy = Math.round(targetY - (rect.top + rect.height / 2));
        element.style.setProperty("--merge-x", `${dx}px`);
        element.style.setProperty("--merge-y", `${dy}px`);
      }
      element.classList.add(className);
    });

    window.setTimeout(() => {
      cells.forEach((cell) => {
        const element = findCellElement(cell);
        if (element) {
          element.classList.remove(className);
          element.style.removeProperty("--merge-x");
          element.style.removeProperty("--merge-y");
        }
      });
    }, duration);
  }

  function playMergeSpawn(cell, level, overload) {
    if (!canFx("merge")) {
      return;
    }

    const element = findCellElement(cell);
    if (!element) {
      return;
    }

    element.classList.add("merge-spawn", `merge-spawn-l${level}`);
    if (overload && overload.isOverload) {
      element.classList.add("merge-spawn-overload");
    }
    window.setTimeout(() => {
      element.classList.remove("merge-spawn", `merge-spawn-l${level}`, "merge-spawn-overload");
    }, level >= 4 ? 760 : 620);
  }

  function findCellElement(cell) {
    return dom.board.querySelector(`[data-r="${cell.r}"][data-c="${cell.c}"]`);
  }

  function createParticlesAtCell(cell, count, color, spread, variant) {
    const element = findCellElement(cell);
    if (!element) {
      return;
    }

    const rect = element.getBoundingClientRect();
    createParticles(rect.left + rect.width / 2, rect.top + rect.height / 2, count, color, spread, variant);
  }

  function createParticlesAtBoard(count, color, spread, variant) {
    const rect = dom.board.getBoundingClientRect();
    createParticles(rect.left + rect.width / 2, rect.top + rect.height / 2, count, color, spread || 150, variant);
  }

  function createParticlesAtRiftMeter(count, color, spread, variant) {
    const element = dom.sideRiftEnergyBar || dom.riftEnergyBar;
    if (!element) {
      createParticlesAtBoard(count, color, spread, variant);
      return;
    }

    const rect = element.getBoundingClientRect();
    createParticles(rect.left + rect.width / 2, rect.top + rect.height / 2, count, color, spread || 140, variant);
  }

  function createRainbowBurstAtBoard(intensity) {
    const strength = intensity || 1;
    const layerRect = dom.floatLayer.getBoundingClientRect();
    const width = window.innerWidth || layerRect.width || 960;
    const height = window.innerHeight || layerRect.height || 640;
    const x = width / 2;
    const y = height / 2;
    const spread = Math.max(width, height) * 0.72 * strength;
    ["#ff4e88", "#ffd45c", "#57e5a2", "#4edcff", "#a78bfa", "#ffffff"].forEach((color) => {
      createParticles(x, y, Math.round(24 * strength), color, spread, "genesis-spectrum");
    });
  }

  function createParticles(x, y, count, color, spread, variant) {
    if (!canFx("particles") || perf.hidden || !isEffectInViewport(x, y, spread || 96)) {
      return;
    }

    pruneVisualNodeSet(perf.activeParticles);
    const burstMultiplier = PARTICLE_BURST_MULTIPLIERS[variant || "default"] || PARTICLE_BURST_MULTIPLIERS.default;
    const scale = (FX_CONFIG.particleScale || 1) * (FX_CONFIG.deviceParticleScale || 1) * burstMultiplier;
    const requestedCount = Math.max(1, Math.round(count * scale));
    const maxActive = Math.max(0, FX_CONFIG.maxParticles || 0);
    const maxBurst = Math.max(1, Math.round((FX_CONFIG.maxBurst || 24) * (variant === "genesis-spectrum" ? 1 : burstMultiplier)));
    const availableSlots = Math.max(0, maxActive - perf.activeParticles.size);
    const particleCount = Math.min(requestedCount, maxBurst, availableSlots);
    if (particleCount <= 0) {
      return;
    }

    const distanceBase = spread || 96;
    const duration = distanceBase >= 200 ? variant === "genesis-spectrum" ? 1320 : 1080 : 760;
    const fragment = document.createDocumentFragment();
    for (let i = 0; i < particleCount; i += 1) {
      const particle = document.createElement("span");
      const angle = Math.random() * Math.PI * 2;
      const distance = 34 + Math.random() * distanceBase;
      const size = 5 + Math.random() * 7;
      particle.className = `particle crystal-shard ${variant || ""}`.trim();
      particle.style.setProperty("--x", `${x}px`);
      particle.style.setProperty("--y", `${y}px`);
      particle.style.setProperty("--dx", `${Math.cos(angle) * distance}px`);
      particle.style.setProperty("--dy", `${Math.sin(angle) * distance}px`);
      particle.style.setProperty("--p-color", color);
      particle.style.setProperty("--p-size", `${size}px`);
      particle.style.setProperty("--p-rotate", `${Math.round(Math.random() * 540 - 270)}deg`);
      particle.style.setProperty("--p-skew", `${Math.round(Math.random() * 20 - 10)}deg`);
      particle.style.setProperty("--p-stretch", `${(1.15 + Math.random() * 0.85).toFixed(2)}`);
      particle.style.setProperty("--p-duration", `${duration}ms`);
      perf.activeParticles.add(particle);
      particle.addEventListener("animationend", () => cleanupVisualNode(particle, perf.activeParticles), { once: true });
      window.setTimeout(() => cleanupVisualNode(particle, perf.activeParticles), duration + 120);
      fragment.appendChild(particle);
    }
    dom.floatLayer.appendChild(fragment);
  }

  function isEffectInViewport(x, y, spread) {
    const width = window.innerWidth || document.documentElement.clientWidth || 960;
    const height = window.innerHeight || document.documentElement.clientHeight || 640;
    const radius = Math.min(Math.max(spread || 0, 48), Math.max(width, height) * 0.78) + EFFECT_VIEWPORT_MARGIN;
    return x + radius >= 0 && x - radius <= width && y + radius >= 0 && y - radius <= height;
  }

  function cleanupVisualNode(node, set) {
    if (set) {
      set.delete(node);
    }
    if (node && node.isConnected) {
      node.remove();
    }
  }

  function pruneVisualNodeSet(set) {
    set.forEach((node) => {
      if (!node || !node.isConnected) {
        set.delete(node);
      }
    });
  }

  function trimVisualNodeSet(set, limit) {
    pruneVisualNodeSet(set);
    while (set.size > limit) {
      const oldest = set.values().next().value;
      cleanupVisualNode(oldest, set);
    }
  }

  function clearTransientVisualEffects() {
    if (!dom.floatLayer) {
      return;
    }
    dom.floatLayer.querySelectorAll(".particle, .coin-rain, .float-text, .line-win-marker, .merge-preview-badge").forEach((node) => node.remove());
    perf.activeParticles.clear();
    perf.activeFloatNodes.clear();
    if (dom.screenFlash) {
      dom.screenFlash.className = "screen-flash";
    }
  }

  function showFloatText(text, cell, variant) {
    if (!canFx("floatText") || perf.hidden) {
      return;
    }

    const normalizedVariant = variant || "coin";
    if (normalizedVariant === "combo") {
      return;
    }
    let displayText = text;
    if (cell) {
      const rewardVariants = new Set(["coin", "coin-3d", "jackpot", "mega-jackpot", "overload"]);
      if (!rewardVariants.has(normalizedVariant)) {
        return;
      }
      const rewardMatch = String(text).match(/[+＋]\s*[\d,，.]+/);
      if (!rewardMatch) {
        return;
      }
      displayText = rewardMatch[0].replace("＋", "+").replace(/，/g, ",");
    }

    const element = document.createElement("span");
    element.className = `float-text ${normalizedVariant}`;
    element.textContent = displayText;
    element.dataset.textFit = "float";
    if (String(displayText).length > 12 || ["coin-3d", "jackpot", "mega-jackpot", "overload", "rift", "void", "stardust"].includes(normalizedVariant)) {
      element.classList.add("float-long");
    }

    let x = window.innerWidth / 2;
    let y = window.innerHeight / 2;
    if (cell) {
      const cellElement = findCellElement(cell);
      if (cellElement) {
        const rect = cellElement.getBoundingClientRect();
        x = rect.left + rect.width / 2;
        y = rect.top + rect.height / 2;
      }
    } else {
      const boardRect = getFloatSafeRect();
      x = boardRect.left + boardRect.width / 2;
      y = boardRect.top + boardRect.height * 0.44;
    }

    const lane = runtime.floatTextLane % 5;
    runtime.floatTextLane += 1;
    const laneOffsets = [
      { x: 0, y: 0 },
      { x: -44, y: -30 },
      { x: 44, y: 28 },
      { x: -22, y: 54 },
      { x: 22, y: -58 }
    ];
    const offset = laneOffsets[lane];
    const safeRect = getFloatSafeRect();
    element.style.left = `${clampNumber(x + offset.x, safeRect.left + 18, safeRect.right - 18)}px`;
    element.style.top = `${clampNumber(y + offset.y, safeRect.top + 22, safeRect.bottom - 22)}px`;
    element.style.maxWidth = `${Math.max(150, Math.min(safeRect.width - 24, window.innerWidth - 28))}px`;
    trimVisualNodeSet(perf.activeFloatNodes, FLOAT_NODE_LIMIT - 1);
    perf.activeFloatNodes.add(element);
    const ttl = variant === "jackpot" || variant === "mega-jackpot" || variant === "overload" || variant === "rift" || variant === "void" ? 1500 : 1160;
    element.addEventListener("animationend", () => cleanupVisualNode(element, perf.activeFloatNodes), { once: true });
    dom.floatLayer.appendChild(element);
    fitFloatingText(element);
    window.setTimeout(() => cleanupVisualNode(element, perf.activeFloatNodes), ttl + 120);
  }

  function getFloatSafeRect() {
    const source = dom.boardWrap || dom.board || document.body;
    const rect = source.getBoundingClientRect();
    const viewportWidth = window.innerWidth || document.documentElement.clientWidth || 960;
    const viewportHeight = window.innerHeight || document.documentElement.clientHeight || 640;
    if (!rect.width || !rect.height) {
      return { left: 14, top: 14, right: viewportWidth - 14, bottom: viewportHeight - 14, width: viewportWidth - 28, height: viewportHeight - 28 };
    }
    const margin = 10;
    return {
      left: Math.max(margin, rect.left + margin),
      top: Math.max(margin, rect.top + margin),
      right: Math.min(viewportWidth - margin, rect.right - margin),
      bottom: Math.min(viewportHeight - margin, rect.bottom - margin),
      width: Math.max(120, Math.min(viewportWidth - margin * 2, rect.width - margin * 2)),
      height: Math.max(120, Math.min(viewportHeight - margin * 2, rect.height - margin * 2))
    };
  }

  function fitFloatingText(element) {
    requestAnimationFrame(() => {
      fitTextElement(element, { minFont: 14, maxLines: 3 });
      constrainFloatingText(element);
      requestAnimationFrame(() => constrainFloatingText(element));
    });
  }

  function constrainFloatingText(element) {
    if (!element || !element.isConnected) {
      return;
    }
    const safeRect = getFloatSafeRect();
    const rect = element.getBoundingClientRect();
    let left = Number.parseFloat(element.style.left) || rect.left + rect.width / 2;
    let top = Number.parseFloat(element.style.top) || rect.top + rect.height / 2;
    if (rect.left < safeRect.left) {
      left += safeRect.left - rect.left;
    }
    if (rect.right > safeRect.right) {
      left -= rect.right - safeRect.right;
    }
    if (rect.top < safeRect.top) {
      top += safeRect.top - rect.top;
    }
    if (rect.bottom > safeRect.bottom) {
      top -= rect.bottom - safeRect.bottom;
    }
    element.style.left = `${clampNumber(left, safeRect.left + 18, safeRect.right - 18)}px`;
    element.style.top = `${clampNumber(top, safeRect.top + 22, safeRect.bottom - 22)}px`;
  }

  function flashScreen(type, duration) {
    if (!canFx("screenFlash") || perf.hidden) {
      return;
    }

    dom.screenFlash.className = `screen-flash flash-${type}`;
    window.setTimeout(() => {
      dom.screenFlash.className = "screen-flash";
    }, duration || (type === "white-full" ? 1500 : type === "full" ? 860 : 620));
  }

  function triggerHaptic(type, options) {
    if (!canFx("shake") || perf.hidden) {
      return;
    }

    const settings = normalizeSettings(state.settings);
    if (!settings.shake) {
      return;
    }

    const eventType = HAPTIC_PRIORITY[type] ? type : "basic";
    const now = performance.now();
    const priority = HAPTIC_PRIORITY[eventType] || HAPTIC_PRIORITY.basic;
    const force = Boolean(options && options.force);
    if (!force && now - hapticRuntime.lastAt < 85 && priority <= hapticRuntime.lastPriority) {
      return;
    }

    hapticRuntime.lastAt = now;
    hapticRuntime.lastPriority = priority;
    window.setTimeout(() => {
      if (performance.now() - hapticRuntime.lastAt >= 120) {
        hapticRuntime.lastPriority = 0;
      }
    }, 140);

    const strength = HAPTIC_STRENGTHS[settings.hapticStrength] ? settings.hapticStrength : "high";
    const bridge = window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.nativeHaptic;
    if (bridge && typeof bridge.postMessage === "function") {
      try {
        bridge.postMessage({ type: eventType, strength });
        return;
      } catch (error) {
        // Fall through to browser fallback outside the native shell.
      }
    }

    runFallbackHaptic(eventType, strength);
  }

  function runFallbackHaptic(type, strength) {
    if (!navigator.vibrate) {
      return;
    }

    const scale = HAPTIC_STRENGTHS[strength] ? HAPTIC_STRENGTHS[strength].scale : HAPTIC_STRENGTHS.high.scale;
    const source = HAPTIC_FALLBACK_PATTERNS[type] || HAPTIC_FALLBACK_PATTERNS.basic;
    const pattern = source.map((value) => Math.max(8, Math.round(value * scale)));
    try {
      navigator.vibrate(pattern);
    } catch (error) {
      // Browser fallback is optional.
    }
  }

  function mapShakePowerToHaptic(power) {
    if (power === "hard") {
      return "epic";
    }
    if (power === "medium") {
      return "heavy";
    }
    if (power === "soft") {
      return "medium";
    }
    if (power === "micro") {
      return "basic";
    }
    if (power === "failure") {
      return "failure";
    }
    return "basic";
  }

  function shake(power) {
    if (!canFx("shake") || perf.hidden) {
      return;
    }

    const className = power === "hard" ? "shaking-hard" : power === "medium" ? "shaking-medium" : power === "soft" ? "shaking-soft" : power === "micro" ? "shaking-micro" : "shaking";
    document.body.classList.add(className);
    triggerHaptic(mapShakePowerToHaptic(power));
    window.setTimeout(() => {
      document.body.classList.remove("shaking");
      document.body.classList.remove("shaking-micro");
      document.body.classList.remove("shaking-soft");
      document.body.classList.remove("shaking-medium");
      document.body.classList.remove("shaking-hard");
    }, power === "hard" ? 540 : power === "medium" ? 420 : power === "soft" ? 220 : power === "micro" ? 140 : 380);
  }

  function createCoinRain(count) {
    if (!canFx("coinRain") || perf.hidden) {
      return;
    }

    pruneVisualNodeSet(perf.activeParticles);
    const maxActive = Math.max(0, FX_CONFIG.maxParticles || 0);
    const availableSlots = Math.max(0, maxActive - perf.activeParticles.size);
    const coinCount = Math.min(
      Math.max(1, Math.round(count * Math.max(0.25, (FX_CONFIG.particleScale || 1) * (FX_CONFIG.deviceParticleScale || 1)))),
      FX_CONFIG.maxCoinRain || 10,
      availableSlots
    );
    if (coinCount <= 0) {
      return;
    }
    const fragment = document.createDocumentFragment();
    for (let i = 0; i < coinCount; i += 1) {
      const coin = document.createElement("span");
      coin.className = "coin-rain";
      coin.textContent = "+";
      coin.style.setProperty("--x", `${8 + Math.random() * 84}vw`);
      coin.style.setProperty("--delay", `${Math.random() * 260}ms`);
      coin.style.setProperty("--drift", `${-70 + Math.random() * 140}px`);
      coin.style.setProperty("--scale", `${0.72 + Math.random() * 0.7}`);
      perf.activeParticles.add(coin);
      coin.addEventListener("animationend", () => cleanupVisualNode(coin, perf.activeParticles), { once: true });
      window.setTimeout(() => cleanupVisualNode(coin, perf.activeParticles), 1840);
      fragment.appendChild(coin);
    }
    dom.floatLayer.appendChild(fragment);
  }

  function pulseComboHud() {
    if (!canFx("comboBounce")) {
      return;
    }

    document.body.classList.remove("combo-bounce");
    void document.body.offsetWidth;
    document.body.classList.add("combo-bounce");
    window.setTimeout(() => {
      document.body.classList.remove("combo-bounce");
    }, 320);
  }

  function flashComboMilestone() {
    if (!canFx("comboBounce")) {
      return;
    }

    document.body.classList.remove("combo-milestone");
    void document.body.offsetWidth;
    document.body.classList.add("combo-milestone");
    window.setTimeout(() => {
      document.body.classList.remove("combo-milestone");
    }, 760);
  }

  function refreshDailyElement(targetState) {
    const today = getDailyDateKey();
    if (targetState.dailyElementDate !== today || !ELEMENTS[targetState.dailyElement]) {
      targetState.dailyElement = randomElementKey();
      targetState.dailyElementDate = today;
    }
  }

  function getDailyDateKey() {
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, "0");
    const day = String(now.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
  }

  function normalizeElementKey(element) {
    return ELEMENTS[element] ? element : randomElementKey();
  }

  function randomElementKey() {
    return ELEMENT_KEYS[Math.floor(Math.random() * ELEMENT_KEYS.length)];
  }

  function createDefaultSkillState() {
    return {
      unlocked: [...DEFAULT_UNLOCKED_SKILLS],
      equipped: DEFAULT_ACTIVE_SKILL,
      cooldowns: DEFAULT_UNLOCKED_SKILLS.reduce((cooldowns, skillKey) => {
        cooldowns[skillKey] = 0;
        return cooldowns;
      }, {})
    };
  }

  function normalizeSkillState(skills) {
    const fallback = createDefaultSkillState();
    const unlocked = Array.isArray(skills.unlocked)
      ? skills.unlocked.filter((skillKey) => ACTIVE_SKILLS[skillKey])
      : fallback.unlocked;
    const normalizedUnlocked = unlocked.length > 0 ? [...new Set(unlocked)] : fallback.unlocked;
    const equipped = normalizedUnlocked.includes(skills.equipped) ? skills.equipped : normalizedUnlocked[0];
    const cooldowns = { ...fallback.cooldowns };
    DEFAULT_UNLOCKED_SKILLS.forEach((skillKey) => {
      cooldowns[skillKey] = clampNumber(skills.cooldowns ? skills.cooldowns[skillKey] : 0, 0, ACTIVE_SKILLS[skillKey].cooldown);
    });
    return {
      unlocked: normalizedUnlocked,
      equipped,
      cooldowns
    };
  }

  function normalizeTalents(talents) {
    if (!Array.isArray(talents)) {
      return [];
    }

    return [...new Set(talents.filter((key) => TALENT_NODE_MAP[key]))];
  }

  function normalizeAchievements(achievements) {
    const normalized = {};
    Object.keys(ACHIEVEMENTS).forEach((key) => {
      const value = achievements && achievements[key];
      normalized[key] = Boolean(value && (value === true || value.completed !== false));
    });
    return normalized;
  }

  function normalizeAchievementProgress(progress, achievements) {
    const normalized = {};
    Object.keys(ACHIEVEMENTS).forEach((key) => {
      const achievement = ACHIEVEMENTS[key];
      const saved = Number(progress && progress[key]);
      normalized[key] = achievements[key] ? achievement.target : clampNumber(Number.isFinite(saved) ? saved : 0, 0, achievement.target);
    });
    return normalized;
  }

  function getEffectiveGemLevel(symbol) {
    if (!symbol || symbol.kind !== "gem") {
      return 0;
    }
    return Math.min(5, symbol.level + (runtime.skillLevelBoostActive ? 1 : 0));
  }

  function setupTextFitObserver() {
    if (!window.MutationObserver || runtime.textFitObserver) {
      scheduleTextFit();
      return;
    }
    runtime.textFitObserver = new MutationObserver((mutations) => {
      if (mutations.length && mutations.every(isVisualEffectMutation)) {
        return;
      }
      scheduleTextFit();
    });
    runtime.textFitObserver.observe(document.body, {
      childList: true,
      characterData: true,
      subtree: true
    });
    scheduleTextFit();
  }

  function isVisualEffectMutation(mutation) {
    const target = mutation.target;
    if (target && target.nodeType === 1 && target.closest && target.closest("#floatLayer")) {
      return true;
    }
    const nodes = [...mutation.addedNodes, ...mutation.removedNodes];
    return nodes.length > 0 && nodes.every((node) => {
      if (node.nodeType !== 1) {
        return true;
      }
      return node.id === "floatLayer" || Boolean(node.closest && node.closest("#floatLayer"));
    });
  }

  function scheduleTextFit() {
    if (!runtime || runtime.textFitScheduled || perf.hidden) {
      return;
    }
    const now = performance.now ? performance.now() : Date.now();
    const interval = FX_CONFIG.textFitInterval || 130;
    const elapsed = now - (perf.textFitLast || 0);
    if (elapsed < interval) {
      if (!perf.textFitTimer) {
        perf.textFitTimer = window.setTimeout(() => {
          perf.textFitTimer = null;
          scheduleTextFit();
        }, Math.max(24, interval - elapsed));
      }
      return;
    }
    perf.textFitLast = now;
    runtime.textFitScheduled = true;
    requestAnimationFrame(() => {
      runtime.textFitScheduled = false;
      fitAllText();
    });
  }

  function fitAllText() {
    getTextFitTargets().forEach((element) => fitTextElement(element));
  }

  function getTextFitTargets() {
    const selector = [
      ".modal-card h2",
      ".modal-card h3",
      ".modal-card p",
      ".modal-card span",
      ".modal-card strong",
      ".modal-card small",
      ".modal-card em",
      ".modal-card button",
      ".task-panel h2",
      ".task-panel span",
      ".task-panel strong",
      ".task-panel button",
      ".mode-panel span",
      ".mode-panel strong",
      ".mode-panel p",
      ".status-grid strong",
      ".control-panel button",
      ".control-panel span",
      ".control-panel strong",
      ".left-hud button",
      ".left-hud strong",
      ".left-hud em",
      ".side-hud span",
      ".side-hud strong",
      ".side-hud em",
      ".side-hud small",
      ".tool-hover-tip",
      ".tool-hover-tip *",
      ".tip-strip span",
      ".item-unlock-toast",
      ".item-unlock-toast *",
      ".gem-unlock-toast",
      ".gem-unlock-toast *",
      ".float-text"
    ].join(",");
    return [...document.querySelectorAll(selector)].filter((element) => shouldFitTextElement(element));
  }

  function shouldFitTextElement(element) {
    if (!element || !element.isConnected || element.closest("[hidden]") || element.closest(".board")) {
      return false;
    }
    if (element.tagName === "SVG" || element.tagName === "PATH") {
      return false;
    }
    const text = (element.textContent || "").trim();
    if (!text || text.length < 2) {
      return false;
    }
    const rect = element.getBoundingClientRect();
    return rect.width > 0 && rect.height > 0;
  }

  function fitTextElement(element, options) {
    if (!shouldFitTextElement(element) && !element.classList.contains("float-text")) {
      return;
    }
    const settings = options || {};
    element.classList.add("text-fit-auto");
    element.classList.remove("text-fit-tight");
    element.style.removeProperty("font-size");

    const computed = getComputedStyle(element);
    const baseFont = Number.parseFloat(computed.fontSize) || 14;
    const minFont = settings.minFont || (element.classList.contains("float-text") ? 14 : 10);
    const maxLines = settings.maxLines || (element.classList.contains("float-text") ? 3 : 4);
    element.style.setProperty("--text-fit-lines", String(maxLines));

    let fontSize = baseFont;
    let guard = 0;
    while (hasTextOverflow(element) && fontSize > minFont && guard < 12) {
      fontSize = Math.max(minFont, fontSize * 0.92);
      element.style.fontSize = `${fontSize}px`;
      guard += 1;
    }

    if (hasTextOverflow(element)) {
      element.classList.add("text-fit-tight");
    }
  }

  function hasTextOverflow(element) {
    const widthOverflow = element.scrollWidth > element.clientWidth + 1;
    const heightOverflow = element.scrollHeight > element.clientHeight + 1 && getComputedStyle(element).overflowY !== "visible";
    return widthOverflow || heightOverflow;
  }

  function setTip(text) {
    dom.tipText.textContent = text;
    scheduleTextFit(dom.tipText);
    const tip = dom.tipText.closest(".tip-strip");
    if (!tip) {
      return;
    }

    tip.classList.add("tip-visible");
    if (runtime.tipTimer) {
      window.clearTimeout(runtime.tipTimer);
    }
    runtime.tipTimer = window.setTimeout(() => {
      tip.classList.remove("tip-visible");
      runtime.tipTimer = null;
    }, 3000);
  }

  function formatNumber(value) {
    return Math.floor(value).toLocaleString("en-US");
  }

  function clampNumber(value, min, max) {
    const number = Number(value);
    if (!Number.isFinite(number)) {
      return min;
    }
    return Math.max(min, Math.min(max, number));
  }

  function getMaxGemId(board) {
    let max = 0;
    board.forEach((row) => {
      if (!Array.isArray(row)) {
        return;
      }
      row.forEach((symbol) => {
        if (symbol && Number.isFinite(symbol.id)) {
          max = Math.max(max, symbol.id);
        }
      });
    });
    return max;
  }

  function delay(ms) {
    return new Promise((resolve) => {
      window.setTimeout(resolve, ms);
    });
  }
}());
