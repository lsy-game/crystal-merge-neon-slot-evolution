using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DestinyRanger
{
    public sealed class DestinyRangerPrototype : MonoBehaviour
    {
        private enum RoomKind { Tutorial, Combat, Elite, Shop, Boss }
        private enum EnemyKind { ShardServant, ArcShooter, ToxicElite, Boss, Drone }
        private enum RuneId { SwordSplit, DodgeTrace, FrostNova, CritOverload, ReturningBlade, ChainArc, Sharpness, Shield, SwiftStep, Greed }
        private enum RuneTier { None, Pair, Triple }

        private sealed class RuneDef
        {
            public RuneId Id;
            public string Name;
            public string ShortName;
            public bool Core;
            public int Weight;
            public Color Color;
            public string TripleText;
            public string PairText;
        }

        private sealed class EnemySpec
        {
            public EnemyKind Kind;
            public Vector2 Position;
            public EnemySpec(EnemyKind kind, Vector2 position) { Kind = kind; Position = position; }
        }

        private sealed class RoomSpec
        {
            public RoomKind Kind;
            public string Name;
            public List<EnemySpec> Enemies = new List<EnemySpec>();
            public bool ShrineChance;
        }

        private sealed class EnemyState
        {
            public EnemyKind Kind;
            public GameObject Root;
            public Transform Body;
            public float Health;
            public float MaxHealth;
            public float Damage;
            public float Speed;
            public float AttackTimer;
            public float TelegraphTimer;
            public bool Telegraphing;
            public bool Frozen;
            public float FrozenTimer;
            public bool PhaseTwo;
            public bool CountForRoom = true;
        }

        private sealed class Projectile
        {
            public GameObject Root;
            public Vector2 Velocity;
            public float Damage;
            public float Life;
            public bool PlayerOwned;
            public bool SplitOnHit;
            public bool Returning;
            public Vector2 Origin;
            public float Travel;
        }

        private sealed class DamageZone
        {
            public GameObject Root;
            public Vector2 Center;
            public Vector2 Size;
            public float DamagePerSecond;
            public float Life;
            public bool Circle;
            public bool PlayerOwned;
        }

        private const float RoomHalfWidth = 10.8f;
        private const float RoomHalfHeight = 6.2f;
        private const float PlayerRadius = .45f;
        private const float EnemyRadius = .55f;

        [Header("Optional Generated Art")]
        [SerializeField] private Sprite conceptBackground;
        [SerializeField] private Sprite runeConceptSheet;
        [SerializeField] private Sprite characterSpriteSheet;
        [SerializeField] private Sprite runeIconSheet;

        private readonly List<RoomSpec> rooms = new List<RoomSpec>();
        private readonly List<EnemyState> enemies = new List<EnemyState>();
        private readonly List<Projectile> projectiles = new List<Projectile>();
        private readonly List<DamageZone> damageZones = new List<DamageZone>();
        private readonly Dictionary<RuneId, int> runeStacks = new Dictionary<RuneId, int>();
        private readonly System.Random rng = new System.Random();

        private Camera mainCamera;
        private Transform worldRoot;
        private Transform enemyRoot;
        private Transform projectileRoot;
        private Transform effectRoot;
        private GameObject player;
        private Transform playerBlade;
        private Canvas canvas;
        private RectTransform runeOverlay;
        private RectTransform shopOverlay;
        private RectTransform resultOverlay;
        private Text healthText;
        private Text coinText;
        private Text roomText;
        private Text runeText;
        private Text statusText;
        private Image healthFill;
        private Image skillCooldownFill;
        private Image dodgeCooldownFill;
        private Image joystickKnob;

        private Vector2 moveInput;
        private Vector2 facing = Vector2.right;
        private Vector2 playerVelocity;
        private float playerHealth = 100;
        private float playerMaxHealth = 100;
        private float playerSpeed = 5;
        private float attackMultiplier = 1;
        private float attackCooldown;
        private float comboTimer;
        private int comboIndex;
        private float dodgeCooldown;
        private float invulnerableTimer;
        private float frostCooldown;
        private float critOverloadTimer;
        private int coins;
        private int currentRoom;
        private int kills;
        private float runTimer;
        private bool roomCleared;
        private bool shrinePending;
        private bool inputLocked;
        private bool attacking;
        private bool bossStarted;
        private bool shopHealBought;
        private readonly List<RuneDef> runeDefs = new List<RuneDef>();
        private readonly List<RuneId> acquiredRunes = new List<RuneId>();

        private void Awake()
        {
            BuildRuneDefs();
            BuildRooms();
            BuildWorld();
            BuildUi();
            LoadRoom(0);
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            runTimer += dt;
            attackCooldown -= dt;
            comboTimer -= dt;
            dodgeCooldown -= dt;
            invulnerableTimer -= dt;
            frostCooldown -= dt;
            critOverloadTimer -= dt;

            if (!inputLocked)
            {
                ReadInput();
                MovePlayer(dt);
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && Input.mousePosition.x > Screen.width * .64f && Input.mousePosition.y < Screen.height * .32f) Attack();
                if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.J)) Dodge();
                if (Input.GetKeyDown(KeyCode.K)) CastFrostNova();
            }

            UpdateEnemies(dt);
            UpdateProjectiles(dt);
            UpdateDamageZones(dt);
            UpdateUi();
            CheckRoomClear();
        }

        private void BuildRuneDefs()
        {
            runeDefs.Add(new RuneDef { Id = RuneId.SwordSplit, Name = "剑气分裂", ShortName = "裂剑", Core = true, Weight = 20, Color = C("#66D9FF"), TripleText = "第三段斩出剑气，命中后分裂", PairText = "剑气范围提升" });
            runeDefs.Add(new RuneDef { Id = RuneId.DodgeTrace, Name = "闪避留痕", ShortName = "电痕", Core = true, Weight = 18, Color = C("#5DADE2"), TripleText = "闪避留下电弧轨迹", PairText = "闪避距离提升" });
            runeDefs.Add(new RuneDef { Id = RuneId.FrostNova, Name = "冰霜新星", ShortName = "冰环", Core = true, Weight = 16, Color = C("#A7E8FF"), TripleText = "获得冰环冻结技能", PairText = "移动速度提升" });
            runeDefs.Add(new RuneDef { Id = RuneId.CritOverload, Name = "暴击过载", ShortName = "过载", Core = true, Weight = 15, Color = C("#F07B7B"), TripleText = "暴击后攻速提升", PairText = "暴击率提升" });
            runeDefs.Add(new RuneDef { Id = RuneId.ReturningBlade, Name = "回旋剑意", ShortName = "回旋", Core = true, Weight = 14, Color = C("#C39BD3"), TripleText = "第三段投出回旋剑影", PairText = "第三段范围提升" });
            runeDefs.Add(new RuneDef { Id = RuneId.ChainArc, Name = "连锁电弧", ShortName = "链电", Core = true, Weight = 12, Color = C("#F7DC6F"), TripleText = "击杀释放连锁闪电", PairText = "击杀回复生命" });
            runeDefs.Add(new RuneDef { Id = RuneId.Sharpness, Name = "锋锐", ShortName = "锋锐", Core = false, Weight = 25, Color = C("#E74C3C"), TripleText = "攻击力提升", PairText = "攻击力小幅提升" });
            runeDefs.Add(new RuneDef { Id = RuneId.Shield, Name = "坚盾", ShortName = "坚盾", Core = false, Weight = 20, Color = C("#F0F4FF"), TripleText = "最大生命提升", PairText = "最大生命小幅提升" });
            runeDefs.Add(new RuneDef { Id = RuneId.SwiftStep, Name = "疾步", ShortName = "疾步", Core = false, Weight = 18, Color = C("#2ECC71"), TripleText = "移动速度提升", PairText = "移动速度小幅提升" });
            runeDefs.Add(new RuneDef { Id = RuneId.Greed, Name = "贪婪", ShortName = "金币", Core = false, Weight = 22, Color = C("#F1C40F"), TripleText = "获得 150 金币", PairText = "获得 40 金币" });
        }

        private void BuildRooms()
        {
            rooms.Add(new RoomSpec { Kind = RoomKind.Tutorial, Name = "房间 1 教学", Enemies = { new EnemySpec(EnemyKind.ShardServant, new Vector2(-2, 1)), new EnemySpec(EnemyKind.ShardServant, new Vector2(2, -1)) } });
            rooms.Add(new RoomSpec { Kind = RoomKind.Combat, Name = "房间 2 晶群", Enemies = { new EnemySpec(EnemyKind.ShardServant, new Vector2(-3, 1)), new EnemySpec(EnemyKind.ShardServant, new Vector2(0, 2)), new EnemySpec(EnemyKind.ShardServant, new Vector2(3, -1)) } });
            rooms.Add(new RoomSpec { Kind = RoomKind.Combat, Name = "房间 3 弧光", ShrineChance = true, Enemies = { new EnemySpec(EnemyKind.ShardServant, new Vector2(-3, -1)), new EnemySpec(EnemyKind.ShardServant, new Vector2(1, 2)), new EnemySpec(EnemyKind.ArcShooter, new Vector2(4, 1)) } });
            rooms.Add(new RoomSpec { Kind = RoomKind.Elite, Name = "房间 4 毒素精英", Enemies = { new EnemySpec(EnemyKind.ToxicElite, new Vector2(0, 1)), new EnemySpec(EnemyKind.ShardServant, new Vector2(-4, -2)), new EnemySpec(EnemyKind.ShardServant, new Vector2(4, -2)) } });
            rooms.Add(new RoomSpec { Kind = RoomKind.Combat, Name = "房间 5 混战", ShrineChance = true, Enemies = { new EnemySpec(EnemyKind.ShardServant, new Vector2(-4, 1)), new EnemySpec(EnemyKind.ShardServant, new Vector2(0, -2)), new EnemySpec(EnemyKind.ArcShooter, new Vector2(3, 2)), new EnemySpec(EnemyKind.ArcShooter, new Vector2(5, -1)) } });
            rooms.Add(new RoomSpec { Kind = RoomKind.Combat, Name = "房间 6 压力", Enemies = { new EnemySpec(EnemyKind.ShardServant, new Vector2(-4, -2)), new EnemySpec(EnemyKind.ShardServant, new Vector2(-2, 2)), new EnemySpec(EnemyKind.ArcShooter, new Vector2(3, 2)), new EnemySpec(EnemyKind.ArcShooter, new Vector2(5, -2)) } });
            rooms.Add(new RoomSpec { Kind = RoomKind.Shop, Name = "房间 7 商店" });
            rooms.Add(new RoomSpec { Kind = RoomKind.Boss, Name = "房间 8 守护者巨像", Enemies = { new EnemySpec(EnemyKind.Boss, Vector2.zero) } });
        }

        private void BuildWorld()
        {
            mainCamera = Camera.main;
            if (!mainCamera)
            {
                var cam = new GameObject("Main Camera");
                mainCamera = cam.AddComponent<Camera>();
                cam.tag = "MainCamera";
            }
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 7.4f;
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.backgroundColor = C("#2B3A42");

            worldRoot = new GameObject("DestinyRangerWorld").transform;
            enemyRoot = new GameObject("Enemies").transform; enemyRoot.SetParent(worldRoot);
            projectileRoot = new GameObject("Projectiles").transform; projectileRoot.SetParent(worldRoot);
            effectRoot = new GameObject("Effects").transform; effectRoot.SetParent(worldRoot);

            CreateConceptBackdrop();
            CreateRoomFloor();
            player = new GameObject("Player_Lightblade");
            player.transform.SetParent(worldRoot);
            player.transform.position = new Vector3(-7, 0, 0);
            CreatePlayerVisual(player.transform);
        }

        private void CreateRoomFloor()
        {
            var floor = RectObj("ArenaFloor", worldRoot, Vector2.zero, new Vector2(RoomHalfWidth * 2, RoomHalfHeight * 2), C("#3A4A52"), -1);
            for (int x = -10; x <= 10; x += 2) RectObj("GridV", floor.transform, new Vector2(x, 0), new Vector2(.035f, RoomHalfHeight * 2), C("#4D5F68"), 0);
            for (int y = -6; y <= 6; y += 2) RectObj("GridH", floor.transform, new Vector2(0, y), new Vector2(RoomHalfWidth * 2, .035f), C("#4D5F68"), 0);
            RectObj("TopWall", worldRoot, new Vector2(0, RoomHalfHeight + .25f), new Vector2(RoomHalfWidth * 2 + .8f, .5f), C("#1F2C33"), 0);
            RectObj("BottomWall", worldRoot, new Vector2(0, -RoomHalfHeight - .25f), new Vector2(RoomHalfWidth * 2 + .8f, .5f), C("#1F2C33"), 0);
            RectObj("LeftWall", worldRoot, new Vector2(-RoomHalfWidth - .25f, 0), new Vector2(.5f, RoomHalfHeight * 2 + .8f), C("#1F2C33"), 0);
            RectObj("RightWall", worldRoot, new Vector2(RoomHalfWidth + .25f, 0), new Vector2(.5f, RoomHalfHeight * 2 + .8f), C("#1F2C33"), 0);
            RectObj("PillarA", worldRoot, new Vector2(-4.8f, 2.8f), new Vector2(.85f, .85f), C("#5A3E32"), 0);
            RectObj("PillarB", worldRoot, new Vector2(4.8f, -2.8f), new Vector2(.85f, .85f), C("#5A3E32"), 0);
        }

        private void CreateConceptBackdrop()
        {
            if (!conceptBackground) return;
            var go = new GameObject("GeneratedConceptBackdrop");
            go.transform.SetParent(worldRoot);
            go.transform.position = new Vector3(0, 0, -0.2f);
            go.transform.localScale = new Vector3(11.4f, 6.45f, 1);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = conceptBackground;
            sr.color = new Color(1, 1, 1, .22f);
            sr.sortingOrder = -3;
        }

        private void CreatePlayerVisual(Transform root)
        {
            CircleObj("Head", root, new Vector2(0, .55f), .32f, C("#F0F4FF"), 3);
            RectObj("Body", root, new Vector2(0, -.1f), new Vector2(.65f, .9f), C("#DDE8FF"), 3);
            RectObj("LegL", root, new Vector2(-.18f, -.82f), new Vector2(.16f, .48f), C("#F0F4FF"), 3);
            RectObj("LegR", root, new Vector2(.18f, -.82f), new Vector2(.16f, .48f), C("#F0F4FF"), 3);
            RectObj("Arm", root, new Vector2(.48f, .05f), new Vector2(.18f, .62f), C("#F0F4FF"), 3).transform.rotation = Quaternion.Euler(0, 0, -25);
            var blade = RectObj("Lightblade", root, new Vector2(.9f, .18f), new Vector2(.18f, 1.05f), C("#66D9FF"), 4);
            blade.transform.rotation = Quaternion.Euler(0, 0, -48);
            playerBlade = blade.transform;
        }

        private void BuildUi()
        {
            if (!FindObjectOfType<EventSystem>())
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
            var canvasGo = new GameObject("DestinyRangerCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(2732, 2048);
            scaler.matchWidthOrHeight = .5f;

            var hud = UIBox("HUD", canvas.transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one, Color.clear);
            var hpBg = UIBox("HealthBg", hud, new Vector2(80, -60), new Vector2(360, 28), new Vector2(0, 1), new Vector2(0, 1), C("#111820"));
            healthFill = UIBox("HealthFill", hpBg, Vector2.zero, new Vector2(360, 28), new Vector2(0, .5f), new Vector2(0, .5f), C("#E74C3C")).GetComponent<Image>();
            healthText = UIText("HealthText", hud, "100", 48, TextAnchor.MiddleLeft, new Vector2(455, -74), new Vector2(180, 60), new Vector2(0, 1), new Vector2(0, 1));
            coinText = UIText("CoinText", hud, "金币 0", 44, TextAnchor.MiddleLeft, new Vector2(80, -132), new Vector2(300, 60), new Vector2(0, 1), new Vector2(0, 1));
            roomText = UIText("RoomText", hud, "", 38, TextAnchor.MiddleCenter, new Vector2(0, -55), new Vector2(900, 70), new Vector2(.5f, 1), new Vector2(.5f, 1));
            runeText = UIText("RuneText", hud, "", 30, TextAnchor.UpperLeft, new Vector2(80, -205), new Vector2(720, 160), new Vector2(0, 1), new Vector2(0, 1));
            statusText = UIText("StatusText", hud, "", 34, TextAnchor.MiddleCenter, new Vector2(0, -190), new Vector2(1100, 80), new Vector2(.5f, 1), new Vector2(.5f, 1));
            UIBox("Minimap", hud, new Vector2(-40, -40), new Vector2(260, 260), new Vector2(1, 1), new Vector2(1, 1), new Color(0, 0, 0, .45f));

            var joy = UIBox("JoystickBase", hud, new Vector2(120, 40), new Vector2(280, 280), new Vector2(0, 0), new Vector2(0, 0), new Color(1, 1, 1, .12f));
            joystickKnob = UIBox("JoystickKnob", joy, Vector2.zero, new Vector2(140, 140), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Color(1, 1, 1, .28f)).GetComponent<Image>();
            UIButton("AttackButton", hud, "攻", 72, new Vector2(-160, 180), new Vector2(240, 240), new Vector2(1, 0), new Vector2(1, 0), C("#D64545"), Attack);
            var dodgeBtn = UIButton("DodgeButton", hud, "闪", 56, new Vector2(-190, 440), new Vector2(180, 180), new Vector2(1, 0), new Vector2(1, 0), C("#5DADE2"), Dodge);
            dodgeCooldownFill = dodgeBtn.GetComponent<Image>();
            var skillBtn = UIButton("SkillButton", hud, "冰", 52, new Vector2(-200, 680), new Vector2(160, 160), new Vector2(1, 0), new Vector2(1, 0), C("#273746"), CastFrostNova);
            skillCooldownFill = skillBtn.GetComponent<Image>();

            runeOverlay = BuildRuneOverlay();
            shopOverlay = BuildShopOverlay();
            resultOverlay = BuildResultOverlay();
            runeOverlay.gameObject.SetActive(false);
            shopOverlay.gameObject.SetActive(false);
            resultOverlay.gameObject.SetActive(false);
        }

        private RectTransform BuildRuneOverlay()
        {
            var root = UIBox("RuneOverlay", canvas.transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one, new Color(0, 0, 0, .75f));
            if (runeConceptSheet)
            {
                var art = UIBox("GeneratedRuneUiReference", root, Vector2.zero, new Vector2(2732, 1537), new Vector2(.5f, .5f), new Vector2(.5f, .5f), Color.white);
                var img = art.GetComponent<Image>();
                img.sprite = runeConceptSheet;
                img.color = new Color(1, 1, 1, .2f);
                art.SetAsFirstSibling();
            }
            UIText("RuneTitle", root, "命运符文", 76, TextAnchor.MiddleCenter, new Vector2(0, -520), new Vector2(900, 100), new Vector2(.5f, 1), new Vector2(.5f, 1));
            for (int i = 0; i < 3; i++) UIBox("RuneWheel" + i, root, new Vector2((i - 1) * 320, -820), new Vector2(280, 280), new Vector2(.5f, 1), new Vector2(.5f, 1), C("#F0F4FF"));
            UIButton("OpenRune", root, "启封", 60, new Vector2(0, -1300), new Vector2(360, 140), new Vector2(.5f, 1), new Vector2(.5f, 1), C("#B8860B"), () => StartCoroutine(RollRune(false)));
            UIButton("ContinueRune", root, "继续", 36, new Vector2(0, -1600), new Vector2(220, 86), new Vector2(.5f, 1), new Vector2(.5f, 1), C("#273746"), CloseRuneOverlay);
            return root;
        }

        private RectTransform BuildShopOverlay()
        {
            var root = UIBox("ShopOverlay", canvas.transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one, Color.clear);
            var panel = UIBox("ShopPanel", root, Vector2.zero, new Vector2(860, 520), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Color(.03f, .07f, .12f, .9f));
            UIText("ShopTitle", panel, "符文商店", 54, TextAnchor.MiddleCenter, new Vector2(0, -40), new Vector2(500, 80), new Vector2(.5f, 1), new Vector2(.5f, 1));
            UIButton("BuyHeal", panel, "恢复 50% 生命\n50 金币", 34, new Vector2(-210, 0), new Vector2(300, 190), new Vector2(.5f, .5f), new Vector2(.5f, .5f), C("#2ECC71"), BuyHeal);
            UIButton("BuyRune", panel, "破碎符文\n100 金币", 34, new Vector2(210, 0), new Vector2(300, 190), new Vector2(.5f, .5f), new Vector2(.5f, .5f), C("#B8860B"), BuyRune);
            UIButton("CloseShop", panel, "X", 40, new Vector2(-35, -35), new Vector2(80, 80), new Vector2(1, 1), new Vector2(1, 1), C("#D64545"), () => shopOverlay.gameObject.SetActive(false));
            return root;
        }

        private RectTransform BuildResultOverlay()
        {
            var root = UIBox("ResultOverlay", canvas.transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one, new Color(.02f, .03f, .07f, .96f));
            UIText("ResultTitle", root, "最终命运评价", 82, TextAnchor.MiddleCenter, new Vector2(0, -420), new Vector2(1000, 110), new Vector2(.5f, 1), new Vector2(.5f, 1));
            UIText("ResultRunes", root, "", 42, TextAnchor.MiddleCenter, new Vector2(0, -690), new Vector2(1500, 180), new Vector2(.5f, 1), new Vector2(.5f, 1));
            UIText("ResultBuild", root, "", 72, TextAnchor.MiddleCenter, new Vector2(0, -950), new Vector2(1300, 120), new Vector2(.5f, 1), new Vector2(.5f, 1)).color = C("#F1C40F");
            UIText("ResultStats", root, "", 40, TextAnchor.MiddleCenter, new Vector2(0, -1160), new Vector2(1200, 150), new Vector2(.5f, 1), new Vector2(.5f, 1));
            UIButton("Restart", root, "再次挑战", 48, new Vector2(0, -1460), new Vector2(420, 120), new Vector2(.5f, 1), new Vector2(.5f, 1), C("#B8860B"), RestartRun);
            return root;
        }

        private void ReadInput()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (Input.touchCount > 0)
            {
                foreach (var touch in Input.touches)
                {
                    if (touch.position.x < Screen.width * .42f && touch.position.y < Screen.height * .42f)
                    {
                        Vector2 center = new Vector2(Screen.width * .095f, Screen.height * .088f);
                        moveInput = Vector2.ClampMagnitude((touch.position - center) / (Screen.height * .12f), 1);
                    }
                }
            }
            moveInput = Vector2.ClampMagnitude(moveInput, 1);
            if (moveInput.sqrMagnitude > .04f) facing = moveInput.normalized;
            if (joystickKnob) joystickKnob.rectTransform.anchoredPosition = moveInput * 70;
        }

        private void MovePlayer(float dt)
        {
            if (!player) return;
            playerVelocity = moveInput * CurrentMoveSpeed();
            Vector3 next = player.transform.position + (Vector3)(playerVelocity * dt);
            next.x = Mathf.Clamp(next.x, -RoomHalfWidth + PlayerRadius, RoomHalfWidth - PlayerRadius);
            next.y = Mathf.Clamp(next.y, -RoomHalfHeight + PlayerRadius, RoomHalfHeight - PlayerRadius);
            player.transform.position = next;
            player.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg);
        }

        private void Attack()
        {
            if (attackCooldown > 0 || attacking || inputLocked) return;
            if (comboTimer <= 0) comboIndex = 0;
            comboIndex = (comboIndex % 3) + 1;
            comboTimer = .5f;
            float speedBonus = critOverloadTimer > 0 ? .72f : 1f;
            attackCooldown = .28f * speedBonus;
            StartCoroutine(AttackRoutine(comboIndex));
        }

        private IEnumerator AttackRoutine(int index)
        {
            attacking = true;
            float damage = (index == 3 ? 36 : 12) * attackMultiplier;
            float range = index == 3 && HasRune(RuneId.ReturningBlade) ? 2.2f : 1.55f;
            float angle = index == 3 ? 95 : 72;
            playerBlade.localScale = new Vector3(1.25f, 1.25f, 1);
            DamageEnemiesInArc(damage, range, angle);
            if (index == 3)
            {
                if (HasRune(RuneId.SwordSplit)) SpawnPlayerProjectile(player.transform.position, facing, 10 * attackMultiplier, true, false);
                if (HasRune(RuneId.ReturningBlade)) SpawnPlayerProjectile(player.transform.position, Rotate(facing, 10), 16 * attackMultiplier, false, true);
                if (HasRune(RuneId.CritOverload)) critOverloadTimer = 2f + Stack(RuneId.CritOverload) * .25f;
            }
            yield return new WaitForSeconds(.12f);
            playerBlade.localScale = Vector3.one;
            attacking = false;
        }

        private void DamageEnemiesInArc(float damage, float range, float angle)
        {
            Vector2 origin = player.transform.position;
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var e = enemies[i];
                Vector2 to = (Vector2)e.Root.transform.position - origin;
                if (to.magnitude <= range && Vector2.Angle(facing, to) <= angle * .5f)
                {
                    DamageEnemy(e, damage);
                    Flash(e.Body.gameObject, Color.white);
                }
            }
        }

        private void Dodge()
        {
            if (dodgeCooldown > 0 || inputLocked) return;
            float dist = HasRune(RuneId.DodgeTrace) ? 3.45f : 3f;
            Vector3 start = player.transform.position;
            Vector3 next = start + (Vector3)(facing.normalized * dist);
            next.x = Mathf.Clamp(next.x, -RoomHalfWidth + PlayerRadius, RoomHalfWidth - PlayerRadius);
            next.y = Mathf.Clamp(next.y, -RoomHalfHeight + PlayerRadius, RoomHalfHeight - PlayerRadius);
            player.transform.position = next;
            dodgeCooldown = 1.2f;
            invulnerableTimer = .25f;
            if (HasRune(RuneId.DodgeTrace))
            {
                var center = (start + next) * .5f;
                var zone = RectObj("DodgeTrace", effectRoot, center, new Vector2(dist, .22f), C("#5DADE2"), 2);
                zone.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg);
                damageZones.Add(new DamageZone { Root = zone, Center = center, Size = new Vector2(dist, .5f), DamagePerSecond = 16 + Stack(RuneId.DodgeTrace) * 2, Life = 1.5f + Stack(RuneId.DodgeTrace) * .3f, PlayerOwned = true });
            }
        }

        private void CastFrostNova()
        {
            if (!HasRune(RuneId.FrostNova) || frostCooldown > 0 || inputLocked) return;
            frostCooldown = 8f;
            float radius = 2.4f + Stack(RuneId.FrostNova) * .25f;
            var circle = CircleObj("FrostNova", effectRoot, player.transform.position, radius, new Color(.5f, .9f, 1f, .28f), 2);
            damageZones.Add(new DamageZone { Root = circle, Center = player.transform.position, Size = new Vector2(radius, radius), DamagePerSecond = 0, Life = .35f, Circle = true, PlayerOwned = true });
            foreach (var e in enemies)
            {
                if (Vector2.Distance(e.Root.transform.position, player.transform.position) <= radius)
                {
                    e.Frozen = true;
                    e.FrozenTimer = 1.5f + Stack(RuneId.FrostNova) * .3f;
                }
            }
        }

        private void UpdateEnemies(float dt)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var e = enemies[i];
                if (!e.Root) { enemies.RemoveAt(i); continue; }
                if (e.Frozen)
                {
                    e.FrozenTimer -= dt;
                    if (e.FrozenTimer <= 0) e.Frozen = false;
                    continue;
                }
                if (e.Kind == EnemyKind.Boss) UpdateBoss(e, dt);
                else if (e.Kind == EnemyKind.ArcShooter || e.Kind == EnemyKind.Drone) UpdateShooter(e, dt);
                else UpdateMelee(e, dt);
            }
        }

        private void UpdateMelee(EnemyState e, float dt)
        {
            Vector2 toPlayer = player.transform.position - e.Root.transform.position;
            float dist = toPlayer.magnitude;
            if (e.Kind == EnemyKind.ToxicElite)
            {
                if (dist < 1.7f) DamagePlayer(5 * dt);
            }
            if (dist > 1.15f && !e.Telegraphing)
            {
                e.Root.transform.position += (Vector3)(toPlayer.normalized * e.Speed * dt);
            }
            else
            {
                e.AttackTimer -= dt;
                if (e.AttackTimer <= 0 && !e.Telegraphing)
                {
                    e.Telegraphing = true;
                    e.TelegraphTimer = .3f;
                    e.Body.localScale = Vector3.one * 1.2f;
                }
                if (e.Telegraphing)
                {
                    e.TelegraphTimer -= dt;
                    if (e.TelegraphTimer <= 0)
                    {
                        e.Body.localScale = Vector3.one;
                        if (dist < 1.35f) DamagePlayer(e.Damage);
                        e.AttackTimer = 1.15f;
                        e.Telegraphing = false;
                    }
                }
            }
        }

        private void UpdateShooter(EnemyState e, float dt)
        {
            Vector2 toPlayer = player.transform.position - e.Root.transform.position;
            float keep = e.Kind == EnemyKind.Drone ? 5f : 4.4f;
            if (toPlayer.magnitude < keep) e.Root.transform.position -= (Vector3)(toPlayer.normalized * e.Speed * .6f * dt);
            else if (toPlayer.magnitude > keep + 1.5f) e.Root.transform.position += (Vector3)(toPlayer.normalized * e.Speed * .4f * dt);
            e.AttackTimer -= dt;
            if (e.AttackTimer <= 0)
            {
                e.AttackTimer = e.Kind == EnemyKind.Drone ? 1.4f : 2f;
                var dir = toPlayer.normalized;
                SpawnEnemyProjectile(e.Root.transform.position, dir, e.Kind == EnemyKind.Drone ? 4.2f : 3.5f, e.Damage);
            }
        }

        private void UpdateBoss(EnemyState e, float dt)
        {
            if (!bossStarted)
            {
                bossStarted = true;
                e.AttackTimer = 1f;
            }
            if (!e.PhaseTwo && e.Health <= e.MaxHealth * .5f)
            {
                e.PhaseTwo = true;
                statusText.text = "守护者巨像碎甲，浮游炮启动";
                SpawnEnemy(EnemyKind.Drone, new Vector2(-3.5f, 2.8f), false);
                SpawnEnemy(EnemyKind.Drone, new Vector2(3.5f, 2.8f), false);
            }
            e.AttackTimer -= dt;
            if (e.AttackTimer > 0) return;
            if (rng.NextDouble() < .5) StartCoroutine(BossSlam(e));
            else StartCoroutine(BossLaser(e));
            e.AttackTimer = e.PhaseTwo ? 3.2f : 3.8f;
        }

        private IEnumerator BossSlam(EnemyState e)
        {
            var dir = ((Vector2)player.transform.position - (Vector2)e.Root.transform.position).normalized;
            var center = (Vector2)e.Root.transform.position + dir * 4f;
            var warn = RectObj("SlamWarning", effectRoot, center, new Vector2(8f, 2f), new Color(.9f, .1f, .08f, .35f), 1);
            warn.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            yield return new WaitForSeconds(.8f);
            Destroy(warn);
            var wave = RectObj("SlamWave", effectRoot, center, new Vector2(8f, 2f), new Color(1f, .28f, .2f, .5f), 1);
            wave.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            damageZones.Add(new DamageZone { Root = wave, Center = center, Size = new Vector2(8f, 2f), DamagePerSecond = e.PhaseTwo ? 24 : 20, Life = .22f, PlayerOwned = false });
        }

        private IEnumerator BossLaser(EnemyState e)
        {
            float t = 0;
            while (t < 2f && e.Root)
            {
                t += Time.deltaTime;
                float a = t * 120f;
                Vector2 dirA = new Vector2(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad));
                Vector2 dirB = -dirA;
                DrawLaser(e.Root.transform.position, dirA);
                DrawLaser(e.Root.transform.position, dirB);
                if (DistancePointLine(player.transform.position, e.Root.transform.position, dirA) < .28f) DamagePlayer((e.PhaseTwo ? 18 : 15) * Time.deltaTime);
                if (DistancePointLine(player.transform.position, e.Root.transform.position, dirB) < .28f) DamagePlayer((e.PhaseTwo ? 18 : 15) * Time.deltaTime);
                yield return null;
            }
        }

        private void DrawLaser(Vector2 origin, Vector2 dir)
        {
            var laser = RectObj("LaserTick", effectRoot, origin + dir * 4.5f, new Vector2(9f, .12f), new Color(.4f, .8f, 1f, .65f), 2);
            laser.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            Destroy(laser, .05f);
        }

        private void UpdateProjectiles(float dt)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var p = projectiles[i];
                if (!p.Root) { projectiles.RemoveAt(i); continue; }
                p.Life -= dt;
                p.Travel += p.Velocity.magnitude * dt;
                if (p.Returning && p.Travel > 3.2f)
                {
                    var back = ((Vector2)player.transform.position - (Vector2)p.Root.transform.position).normalized;
                    p.Velocity = back * 8f;
                }
                p.Root.transform.position += (Vector3)(p.Velocity * dt);
                if (p.PlayerOwned) CheckPlayerProjectileHit(p);
                else if (Vector2.Distance(p.Root.transform.position, player.transform.position) < .45f)
                {
                    DamagePlayer(p.Damage);
                    DestroyProjectileAt(i);
                    continue;
                }
                if (p.Life <= 0 || Mathf.Abs(p.Root.transform.position.x) > RoomHalfWidth + 1 || Mathf.Abs(p.Root.transform.position.y) > RoomHalfHeight + 1)
                {
                    DestroyProjectileAt(i);
                }
            }
        }

        private void CheckPlayerProjectileHit(Projectile p)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var e = enemies[i];
                if (Vector2.Distance(p.Root.transform.position, e.Root.transform.position) < .6f)
                {
                    DamageEnemy(e, p.Damage);
                    if (p.SplitOnHit)
                    {
                        for (int n = -1; n <= 1; n++) SpawnPlayerProjectile(p.Root.transform.position, Rotate(-p.Velocity.normalized, n * 28), 6 + Stack(RuneId.SwordSplit) * 1.2f, false, false);
                    }
                    p.Life = 0;
                    break;
                }
            }
        }

        private void UpdateDamageZones(float dt)
        {
            for (int i = damageZones.Count - 1; i >= 0; i--)
            {
                var z = damageZones[i];
                z.Life -= dt;
                if (z.Root) z.Root.transform.localScale *= 1f + dt * .18f;
                if (z.PlayerOwned)
                {
                    for (int enemyIndex = enemies.Count - 1; enemyIndex >= 0; enemyIndex--)
                    {
                        var e = enemies[enemyIndex];
                        if (z.Circle ? Vector2.Distance(e.Root.transform.position, z.Center) < z.Size.x : Mathf.Abs(e.Root.transform.position.x - z.Center.x) < z.Size.x * .5f && Mathf.Abs(e.Root.transform.position.y - z.Center.y) < z.Size.y)
                            DamageEnemy(e, z.DamagePerSecond * dt);
                    }
                }
                else
                {
                    if (Mathf.Abs(player.transform.position.x - z.Center.x) < z.Size.x * .5f && Mathf.Abs(player.transform.position.y - z.Center.y) < z.Size.y * .5f)
                        DamagePlayer(z.DamagePerSecond);
                }
                if (z.Life <= 0)
                {
                    if (z.Root) Destroy(z.Root);
                    damageZones.RemoveAt(i);
                }
            }
        }

        private void DamageEnemy(EnemyState e, float amount)
        {
            e.Health -= amount;
            if (e.Health > 0) return;
            if (e.CountForRoom) { kills++; coins += e.Kind == EnemyKind.Boss ? 200 : 15; }
            if (HasRune(RuneId.ChainArc) && e.CountForRoom) ChainFrom(e.Root.transform.position);
            Destroy(e.Root);
            enemies.Remove(e);
            if (e.Kind == EnemyKind.Boss) ShowResults();
        }

        private void ChainFrom(Vector2 origin)
        {
            var target = enemies.Where(e => e.Root).OrderBy(e => Vector2.Distance(origin, e.Root.transform.position)).FirstOrDefault();
            if (target == null)
            {
                playerHealth = Mathf.Min(playerMaxHealth, playerHealth + 5);
                return;
            }
            var dir = ((Vector2)target.Root.transform.position - origin).normalized;
            SpawnPlayerProjectile(origin, dir, 12 + Stack(RuneId.ChainArc) * 2, false, false);
        }

        private void DamagePlayer(float amount)
        {
            if (invulnerableTimer > 0 || resultOverlay.gameObject.activeSelf) return;
            playerHealth -= amount;
            Flash(player, Color.white);
            if (playerHealth <= 0) ShowResults(true);
        }

        private void CheckRoomClear()
        {
            if (roomCleared || rooms[currentRoom].Kind == RoomKind.Shop || rooms[currentRoom].Kind == RoomKind.Boss) return;
            if (enemies.Any(e => e.CountForRoom)) return;
            roomCleared = true;
            if (currentRoom == 0)
            {
                shrinePending = true;
                StartCoroutine(OpenRuneAfterDelay(true));
            }
            else if (rooms[currentRoom].Kind == RoomKind.Elite)
            {
                shrinePending = true;
                StartCoroutine(OpenRuneAfterDelay(false));
            }
            else if (rooms[currentRoom].ShrineChance && rng.NextDouble() < .2)
            {
                shrinePending = true;
                StartCoroutine(OpenRuneAfterDelay(false));
            }
            else
            {
                statusText.text = "房间清空，按 N 进入下一房间";
            }
            if (currentRoom < rooms.Count - 1 && Input.GetKeyDown(KeyCode.N)) LoadRoom(currentRoom + 1);
        }

        private IEnumerator OpenRuneAfterDelay(bool first)
        {
            statusText.text = first ? "符文圣殿开启：首次保底剑气分裂" : "破碎符文共鸣机会";
            yield return new WaitForSeconds(.8f);
            ShowRuneOverlay(first);
        }

        private void ShowRuneOverlay(bool first)
        {
            inputLocked = true;
            runeOverlay.gameObject.SetActive(true);
            foreach (Text t in runeOverlay.GetComponentsInChildren<Text>()) if (t.name.StartsWith("RuneTitle")) t.text = first ? "首次命运符文" : "命运符文";
            StartCoroutine(RollRune(first));
        }

        private IEnumerator RollRune(bool first)
        {
            foreach (var btn in runeOverlay.GetComponentsInChildren<Button>()) btn.interactable = false;
            RuneId[] result;
            if (first)
            {
                result = new[] { RuneId.SwordSplit, RuneId.SwordSplit, RuneId.SwordSplit };
            }
            else
            {
                result = new[] { WeightedRune(), WeightedRune(), WeightedRune() };
            }
            var wheels = runeOverlay.Cast<Transform>().Where(t => t.name.StartsWith("RuneWheel")).Select(t => t.GetComponent<RectTransform>()).ToArray();
            for (int i = 0; i < wheels.Length; i++)
            {
                yield return new WaitForSeconds(.22f);
                var def = Def(result[i]);
                wheels[i].GetComponent<Image>().color = def.Color;
                var txt = wheels[i].GetComponentInChildren<Text>();
                if (!txt) txt = UIText("RuneLabel", wheels[i], def.ShortName, 48, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(260, 80), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
                txt.text = def.ShortName;
            }
            ApplyRuneResult(result);
            foreach (var btn in runeOverlay.GetComponentsInChildren<Button>()) btn.interactable = true;
        }

        private void ApplyRuneResult(RuneId[] result)
        {
            var groups = result.GroupBy(r => r).OrderByDescending(g => g.Count()).ToArray();
            var best = groups.First();
            if (best.Count() >= 3)
            {
                GrantRune(best.Key, RuneTier.Triple);
                statusText.text = "三星共鸣：" + Def(best.Key).Name;
            }
            else if (best.Count() == 2)
            {
                GrantRune(best.Key, RuneTier.Pair);
                statusText.text = "二星共鸣：" + Def(best.Key).Name;
            }
            else
            {
                playerHealth = Mathf.Min(playerMaxHealth, playerHealth + playerMaxHealth * .2f);
                statusText.text = "逆运祝福：恢复 20% 生命";
            }
        }

        private void GrantRune(RuneId id, RuneTier tier)
        {
            if (!runeStacks.ContainsKey(id)) runeStacks[id] = 0;
            runeStacks[id]++;
            acquiredRunes.Add(id);
            float value = tier == RuneTier.Triple ? 1f : .35f;
            switch (id)
            {
                case RuneId.Sharpness: attackMultiplier = Mathf.Min(2.2f, attackMultiplier + (tier == RuneTier.Triple ? .25f : .08f)); break;
                case RuneId.Shield: playerMaxHealth += 100 * (tier == RuneTier.Triple ? .25f : .08f); playerHealth = Mathf.Min(playerMaxHealth, playerHealth + 25 * value); break;
                case RuneId.SwiftStep: playerSpeed = Mathf.Min(7.4f, playerSpeed + (tier == RuneTier.Triple ? 1f : .4f)); break;
                case RuneId.Greed: coins += tier == RuneTier.Triple ? 150 : 40; break;
            }
        }

        private void CloseRuneOverlay()
        {
            runeOverlay.gameObject.SetActive(false);
            inputLocked = false;
            shrinePending = false;
            if (roomCleared && currentRoom < rooms.Count - 1) LoadRoom(currentRoom + 1);
        }

        private void LoadRoom(int index)
        {
            currentRoom = Mathf.Clamp(index, 0, rooms.Count - 1);
            roomCleared = false;
            shrinePending = false;
            bossStarted = false;
            statusText.text = "";
            foreach (var e in enemies.ToArray()) if (e.Root) Destroy(e.Root);
            enemies.Clear();
            foreach (var p in projectiles.ToArray()) if (p.Root) Destroy(p.Root);
            projectiles.Clear();
            foreach (var z in damageZones.ToArray()) if (z.Root) Destroy(z.Root);
            damageZones.Clear();
            var shopNpc = GameObject.Find("ShopNpc");
            if (shopNpc) Destroy(shopNpc);
            player.transform.position = new Vector3(-7, 0, 0);
            var room = rooms[currentRoom];
            if (room.Kind == RoomKind.Shop)
            {
                CreateShopNpc();
                shopOverlay.gameObject.SetActive(true);
                statusText.text = "商店：补给后按 N 进入 Boss 房";
            }
            else
            {
                foreach (var spec in room.Enemies) SpawnEnemy(spec.Kind, spec.Position, true);
            }
        }

        private void SpawnEnemy(EnemyKind kind, Vector2 pos, bool countForRoom)
        {
            EnemyState e = new EnemyState { Kind = kind, CountForRoom = countForRoom };
            e.Root = new GameObject(kind.ToString());
            e.Root.transform.SetParent(enemyRoot);
            e.Root.transform.position = pos;
            switch (kind)
            {
                case EnemyKind.ShardServant: e.Health = e.MaxHealth = 30; e.Damage = 10; e.Speed = 3; CreateShardVisual(e.Root.transform, 1f); break;
                case EnemyKind.ArcShooter: e.Health = e.MaxHealth = 22; e.Damage = 8; e.Speed = 2; CreateShooterVisual(e.Root.transform); break;
                case EnemyKind.ToxicElite: e.Health = e.MaxHealth = 50; e.Damage = 12; e.Speed = 3; CreateShardVisual(e.Root.transform, 1.3f); CircleObj("ToxicAura", e.Root.transform, Vector2.zero, 1.5f, new Color(.18f, .8f, .42f, .28f), 1); break;
                case EnemyKind.Boss: e.Health = e.MaxHealth = 450; e.Damage = 20; e.Speed = 0; CreateBossVisual(e.Root.transform); break;
                case EnemyKind.Drone: e.Health = e.MaxHealth = 40; e.Damage = 8; e.Speed = 2.4f; CreateDroneVisual(e.Root.transform); break;
            }
            e.Body = e.Root.transform.GetChild(0);
            enemies.Add(e);
        }

        private void CreateShardVisual(Transform root, float scale)
        {
            var body = RectObj("Body", root, new Vector2(0, -.08f) * scale, new Vector2(.55f, .68f) * scale, C("#D64545"), 2);
            TriangleObj("Head", root, new Vector2(0, .55f) * scale, .55f * scale, C("#F07B7B"), 3);
            RectObj("LegL", root, new Vector2(-.16f, -.58f) * scale, new Vector2(.14f, .34f) * scale, C("#F07B7B"), 2);
            RectObj("LegR", root, new Vector2(.16f, -.58f) * scale, new Vector2(.14f, .34f) * scale, C("#F07B7B"), 2);
            root.localScale = Vector3.one;
        }

        private void CreateShooterVisual(Transform root)
        {
            CircleObj("Head", root, new Vector2(0, .35f), .35f, C("#F7DC6F"), 3);
            RectObj("Body", root, new Vector2(0, -.35f), new Vector2(.35f, .75f), C("#F7DC6F"), 2);
            CircleObj("EarL", root, new Vector2(-.38f, .42f), .17f, C("#F39C12"), 3);
            CircleObj("EarR", root, new Vector2(.38f, .42f), .17f, C("#F39C12"), 3);
        }

        private void CreateBossVisual(Transform root)
        {
            RectObj("Body", root, Vector2.zero, new Vector2(2f, 2f), C("#707B7C"), 2);
            CircleObj("ShoulderL", root, new Vector2(-1.4f, .45f), .65f, C("#85929E"), 2);
            CircleObj("ShoulderR", root, new Vector2(1.4f, .45f), .65f, C("#85929E"), 2);
            RectObj("Eye", root, new Vector2(0, .25f), new Vector2(.45f, .8f), C("#66D9FF"), 3).transform.rotation = Quaternion.Euler(0, 0, 45);
            RectObj("ArmL", root, new Vector2(-1.75f, -.8f), new Vector2(.5f, 1.3f), C("#707B7C"), 1);
            RectObj("ArmR", root, new Vector2(1.75f, -.8f), new Vector2(.5f, 1.3f), C("#707B7C"), 1);
        }

        private void CreateDroneVisual(Transform root)
        {
            RectObj("DroneBody", root, Vector2.zero, new Vector2(.65f, .65f), C("#85929E"), 3);
        }

        private void CreateShopNpc()
        {
            var old = GameObject.Find("ShopNpc");
            if (old) Destroy(old);
            var npc = new GameObject("ShopNpc");
            npc.transform.SetParent(worldRoot);
            npc.transform.position = Vector3.zero;
            CircleObj("CoinHead", npc.transform, new Vector2(0, .45f), .35f, C("#F1C40F"), 3);
            RectObj("CoinBody", npc.transform, new Vector2(0, -.2f), new Vector2(.7f, .85f), C("#B8860B"), 2);
        }

        private void SpawnPlayerProjectile(Vector2 origin, Vector2 dir, float damage, bool split, bool returning)
        {
            var go = RectObj("PlayerProjectile", projectileRoot, origin + dir.normalized * .6f, new Vector2(.8f, .18f), C("#66D9FF"), 4);
            go.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            projectiles.Add(new Projectile { Root = go, Velocity = dir.normalized * 8f, Damage = damage, Life = returning ? 2.2f : 1.4f, PlayerOwned = true, SplitOnHit = split, Returning = returning, Origin = origin });
        }

        private void SpawnEnemyProjectile(Vector2 origin, Vector2 dir, float speed, float damage)
        {
            var go = TriangleObj("EnemyProjectile", projectileRoot, origin + dir.normalized * .5f, .28f, C("#F7DC6F"), 3);
            go.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90);
            projectiles.Add(new Projectile { Root = go, Velocity = dir.normalized * speed, Damage = damage, Life = 4f, PlayerOwned = false });
        }

        private void DestroyProjectileAt(int i)
        {
            if (projectiles[i].Root) Destroy(projectiles[i].Root);
            projectiles.RemoveAt(i);
        }

        private void BuyHeal()
        {
            if (shopHealBought || coins < 50) return;
            coins -= 50;
            playerHealth = Mathf.Min(playerMaxHealth, playerHealth + playerMaxHealth * .5f);
            shopHealBought = true;
        }

        private void BuyRune()
        {
            if (coins < 100) return;
            coins -= 100;
            ShowRuneOverlay(false);
        }

        private void ShowResults(bool died = false)
        {
            inputLocked = true;
            resultOverlay.gameObject.SetActive(true);
            var texts = resultOverlay.GetComponentsInChildren<Text>();
            string list = acquiredRunes.Count == 0 ? "无符文" : string.Join("  ", runeStacks.Select(kv => Def(kv.Key).Name + " x" + kv.Value));
            texts.First(t => t.name == "ResultRunes").text = list;
            texts.First(t => t.name == "ResultBuild").text = died ? "试炼中止" : BuildTitle();
            texts.First(t => t.name == "ResultStats").text = $"击败 {kills}    金币 {coins}    时间 {Mathf.FloorToInt(runTimer / 60)}:{Mathf.FloorToInt(runTimer % 60):00}";
        }

        private string BuildTitle()
        {
            if (HasRune(RuneId.SwordSplit) && HasRune(RuneId.ChainArc)) return "电弧剑客";
            if (HasRune(RuneId.FrostNova) && HasRune(RuneId.DodgeTrace)) return "冰霜游侠";
            if (HasRune(RuneId.CritOverload) && HasRune(RuneId.ReturningBlade)) return "疾风斩者";
            if (Stack(RuneId.Sharpness) >= 3) return "纯粹的锋锐";
            return "命运行路人";
        }

        private void RestartRun()
        {
            foreach (var key in runeStacks.Keys.ToList()) runeStacks[key] = 0;
            runeStacks.Clear();
            acquiredRunes.Clear();
            playerMaxHealth = 100; playerHealth = 100; playerSpeed = 5; attackMultiplier = 1; coins = 0; kills = 0; runTimer = 0; shopHealBought = false;
            resultOverlay.gameObject.SetActive(false);
            inputLocked = false;
            LoadRoom(0);
        }

        private void UpdateUi()
        {
            if (Input.GetKeyDown(KeyCode.N) && roomCleared && !shrinePending && currentRoom < rooms.Count - 1) LoadRoom(currentRoom + 1);
            if (rooms[currentRoom].Kind == RoomKind.Shop && Input.GetKeyDown(KeyCode.N)) LoadRoom(currentRoom + 1);
            healthText.text = Mathf.CeilToInt(playerHealth) + "/" + Mathf.CeilToInt(playerMaxHealth);
            healthFill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 360 * Mathf.Clamp01(playerHealth / playerMaxHealth));
            coinText.text = "金币 " + coins;
            roomText.text = rooms[currentRoom].Name;
            runeText.text = runeStacks.Count == 0 ? "符文：无" : "符文：" + string.Join(" / ", runeStacks.Select(kv => Def(kv.Key).ShortName + "x" + kv.Value));
            if (skillCooldownFill) skillCooldownFill.color = HasRune(RuneId.FrostNova) ? (frostCooldown > 0 ? C("#566573") : C("#A7E8FF")) : C("#273746");
            if (dodgeCooldownFill) dodgeCooldownFill.color = dodgeCooldown > 0 ? C("#566573") : C("#5DADE2");
        }

        private RuneId WeightedRune()
        {
            int total = runeDefs.Sum(r => r.Weight);
            int pick = rng.Next(total);
            foreach (var r in runeDefs)
            {
                pick -= r.Weight;
                if (pick < 0) return r.Id;
            }
            return RuneId.Sharpness;
        }

        private bool HasRune(RuneId id) => runeStacks.TryGetValue(id, out int count) && count > 0;
        private int Stack(RuneId id) => runeStacks.TryGetValue(id, out int count) ? count : 0;
        private RuneDef Def(RuneId id) => runeDefs.First(r => r.Id == id);
        private float CurrentMoveSpeed() => playerSpeed;

        private static Vector2 Rotate(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float c = Mathf.Cos(rad), s = Mathf.Sin(rad);
            return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
        }

        private static float DistancePointLine(Vector2 point, Vector2 origin, Vector2 dir)
        {
            Vector2 projected = origin + Vector2.Dot(point - origin, dir.normalized) * dir.normalized;
            if (Vector2.Dot(projected - origin, dir) < 0) return 999;
            return Vector2.Distance(point, projected);
        }

        private static Color C(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out var c);
            return c;
        }

        private static void Flash(GameObject go, Color color)
        {
            var sr = go.GetComponentInChildren<SpriteRenderer>();
            if (sr) sr.color = color;
        }

        private static GameObject RectObj(string name, Transform parent, Vector2 pos, Vector2 size, Color color, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = pos;
            go.transform.localScale = new Vector3(size.x, size.y, 1);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteCache.White;
            sr.color = color;
            sr.sortingOrder = order;
            return go;
        }

        private static GameObject CircleObj(string name, Transform parent, Vector2 pos, float radius, Color color, int order)
        {
            var go = RectObj(name, parent, pos, new Vector2(radius * 2, radius * 2), color, order);
            go.GetComponent<SpriteRenderer>().sprite = SpriteCache.Circle;
            return go;
        }

        private static GameObject TriangleObj(string name, Transform parent, Vector2 pos, float size, Color color, int order)
        {
            var go = RectObj(name, parent, pos, new Vector2(size, size), color, order);
            go.GetComponent<SpriteRenderer>().sprite = SpriteCache.Triangle;
            return go;
        }

        private static RectTransform UIBox(string name, Transform parent, Vector2 pos, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(.5f, .5f);
            rt.anchoredPosition = pos;
            if (size != Vector2.zero) rt.sizeDelta = size;
            go.GetComponent<Image>().color = color;
            return rt;
        }

        private static Text UIText(string name, Transform parent, string value, int size, TextAnchor align, Vector2 pos, Vector2 wh, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.pivot = new Vector2(.5f, .5f); rt.anchoredPosition = pos; rt.sizeDelta = wh;
            var t = go.GetComponent<Text>();
            t.text = value; t.fontSize = size; t.alignment = align; t.color = Color.white; t.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); t.fontStyle = FontStyle.Bold;
            return t;
        }

        private static RectTransform UIButton(string name, Transform parent, string value, int size, Vector2 pos, Vector2 wh, Vector2 anchorMin, Vector2 anchorMax, Color color, UnityEngine.Events.UnityAction action)
        {
            var rt = UIBox(name, parent, pos, wh, anchorMin, anchorMax, color);
            var btn = rt.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(action);
            UIText("Label", rt, value, size, TextAnchor.MiddleCenter, Vector2.zero, wh, new Vector2(.5f, .5f), new Vector2(.5f, .5f));
            return rt;
        }

        private static class SpriteCache
        {
            public static readonly Sprite White = MakeRect();
            public static readonly Sprite Circle = MakeCircle();
            public static readonly Sprite Triangle = MakeTriangle();

            private static Sprite MakeRect()
            {
                var tex = new Texture2D(8, 8);
                for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++) tex.SetPixel(x, y, Color.white);
                tex.Apply();
                return Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(.5f, .5f), 8);
            }

            private static Sprite MakeCircle()
            {
                var tex = new Texture2D(64, 64);
                for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(31.5f, 31.5f));
                    tex.SetPixel(x, y, d <= 31 ? Color.white : Color.clear);
                }
                tex.Apply();
                return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(.5f, .5f), 64);
            }

            private static Sprite MakeTriangle()
            {
                var tex = new Texture2D(64, 64);
                for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
                {
                    bool inside = y > 8 && x > 32 - (y - 8) * .55f && x < 32 + (y - 8) * .55f;
                    tex.SetPixel(x, y, inside ? Color.white : Color.clear);
                }
                tex.Apply();
                return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(.5f, .5f), 64);
            }
        }
    }
}
