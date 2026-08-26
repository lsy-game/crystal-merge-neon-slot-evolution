"use strict";

const CONFIG = {
  bet: 10,
  basePayoutScale: 0.386,
  maxPayoutScale: 0.407,
  dailyProductiveCap: 400,
  offlineCapHours: 8,
  baseOfflinePerHour: 600,
  lossProtectionThreshold: 4800,
  lossProtectionGrant: 1200,
  luckyThreshold: 6,
  freeSpinsAward: 5,
  saveKey: "wheatTownMvpSaveV1",
  agreementKey: "wheatTownAgreementAcceptedV1",
};

const SYMBOLS = {
  wheat:   { image: "assets/images/symbols/wheat.webp", name: "麦穗", weight: 27, pays: { 3: 2, 4: 5, 5: 12 }, normal: true },
  apple:   { image: "assets/images/symbols/apple.webp", name: "苹果", weight: 23, pays: { 3: 3, 4: 7, 5: 16 }, normal: true },
  milk:    { image: "assets/images/symbols/milk.webp", name: "牛奶", weight: 19, pays: { 3: 4, 4: 9, 5: 22 }, normal: true },
  bread:   { image: "assets/images/symbols/bread.webp", name: "面包", weight: 15, pays: { 3: 6, 4: 14, 5: 35 }, normal: true },
  gem:     { image: "assets/images/symbols/gem.webp", name: "宝石", weight: 10, pays: { 3: 10, 4: 26, 5: 70 }, normal: true },
  wild:    { image: "assets/images/symbols/wild.webp", name: "风车WILD", weight: 4, pays: { 3: 12, 4: 35, 5: 100 }, wild: true },
  scatter: { image: "assets/images/symbols/scatter.webp", name: "丰收篮", weight: 2, scatter: true },
};

const PAYLINES = [
  [0,0,0,0,0],[1,1,1,1,1],[2,2,2,2,2],
  [0,1,2,1,0],[2,1,0,1,2],[0,0,1,2,2],[2,2,1,0,0],
  [1,0,0,0,1],[1,2,2,2,1],[0,1,1,1,0],[2,1,1,1,2],
  [1,0,1,2,1],[1,2,1,0,1],[0,1,0,1,0],[2,1,2,1,2],
  [0,2,0,2,0],[2,0,2,0,2],[0,2,2,2,0],[2,0,0,0,2],
  [0,0,2,0,0],[2,2,0,2,2],[1,0,2,0,1],[1,2,0,2,1],
  [0,1,2,2,2],[2,1,0,0,0],
];

const BUILDINGS = {
  bakery:  { name: "面包房", image: "assets/images/buildings/bakery.webp", rtp: 1.0, desc: "提升小镇长期收获效率", costs: [0, 500, 1400, 3600, 8000] },
  market:  { name: "集市", image: "assets/images/buildings/market.webp", rtp: 0.8, desc: "改善集市订单与收获节奏", costs: [0, 600, 1600, 4000, 9000] },
  workshop:{ name: "宝石工坊", image: "assets/images/buildings/workshop.webp", rtp: 0.8, desc: "提升小镇长期收获效率", costs: [0, 700, 1800, 4500, 10000] },
  vault:   { name: "城镇金库", image: "assets/images/buildings/vault.webp", rtp: 0.6, desc: "稳定小额金币收益", costs: [0, 800, 2100, 5200, 11500] },
  lighthouse:{ name: "田园灯塔", image: "assets/images/buildings/lighthouse.webp", rtp: 0.8, desc: "提升小镇长期收获效率", costs: [0, 1200, 3500, 8000, 18000], materials: [null,{wood:60,ore:20},{wood:140,ore:50},{wood:260,ore:110},{wood:600,ore:300}] },
};

const BUFFS = [
  { id: "offline", name: "勤劳晨光", text: "离线金币 +20%", value: .2 },
  { id: "material", name: "丰收之风", text: "成长材料 +10%", value: .1 },
  { id: "gift", name: "友谊花环", text: "礼物发现速度 +25%", value: .25 },
];

const GOALS = [
  { text: "将任意建筑升至2级", reward: 1200, test: s => Object.values(s.buildings).some(v => v >= 2) },
  { text: "让4座基础建筑达到2级", reward: 2600, test: s => ["bakery","market","workshop","vault"].every(k => s.buildings[k] >= 2) },
  { text: "将田园灯塔升至2级", reward: 4000, test: s => s.buildings.lighthouse >= 2 },
  { text: "让4座基础建筑达到3级", reward: 7500, test: s => ["bakery","market","workshop","vault"].every(k => s.buildings[k] >= 3) },
  { text: "将田园灯塔升至3级", reward: 12000, test: s => s.buildings.lighthouse >= 3 },
  { text: "完成MVP阶段目标：灯塔达到4级", reward: 20000, test: s => s.buildings.lighthouse >= 4 },
];

const ORDER_TEMPLATES = [
  { id:"wheat", title:"面包房备货", text:"收集18个麦穗符号", metric:"symbol", key:"wheat", target:18, icon:SYMBOLS.wheat.image, reward:{coins:220,wood:5} },
  { id:"growth", title:"照料连片作物", text:"累计生长4个相邻符号", metric:"growth", target:4, icon:SYMBOLS.wild.image, reward:{coins:260,gifts:1} },
  { id:"spins20", title:"开启清晨收获", text:"完成20次有效收获", metric:"spins", target:20, icon:SYMBOLS.scatter.image, reward:{coins:300,ore:3} },
  { id:"apple", title:"集市水果订单", text:"收集20个苹果符号", metric:"symbol", key:"apple", target:20, icon:SYMBOLS.apple.image, reward:{coins:280,wood:6} },
  { id:"coins", title:"充实城镇金库", text:"通过田园收获获得120金币", metric:"coins", target:120, icon:SYMBOLS.gem.image, reward:{coins:350,ore:4} },
  { id:"symbols", title:"整理丰收仓库", text:"累计收集150个符号", metric:"symbols", target:150, icon:SYMBOLS.bread.image, reward:{coins:320,gifts:1} },
  { id:"bread", title:"面包节准备", text:"收集14个面包符号", metric:"symbol", key:"bread", target:14, icon:SYMBOLS.bread.image, reward:{coins:360,wood:8} },
  { id:"scatter", title:"寻找丰收篮", text:"累计出现3个丰收篮", metric:"scatter", target:3, icon:SYMBOLS.scatter.image, reward:{coins:400,freeSpins:2} },
  { id:"spins35", title:"午后田园巡游", text:"完成35次有效收获", metric:"spins", target:35, icon:SYMBOLS.wheat.image, reward:{coins:450,ore:5} },
];

const COLLECTIONS = [
  { id:"staple", title:"谷仓基础套", text:"麦穗、苹果、牛奶各收集12个", icons:["wheat","apple","milk"], target:12, reward:{coins:500,wood:12} },
  { id:"bakery", title:"面包节陈列", text:"麦穗、面包各收集18个", icons:["wheat","bread"], target:18, reward:{coins:700,gifts:2} },
  { id:"craft", title:"工坊闪光展柜", text:"宝石、风车各收集6个", icons:["gem","wild"], target:6, reward:{coins:900,ore:8} },
  { id:"harvest", title:"丰收纪念篮", text:"丰收篮累计出现5个", icons:["scatter"], target:5, reward:{coins:1000,freeSpins:3} },
];

const MILESTONES = [
  { spins:10, label:"金币200", reward:{coins:200} },
  { spins:30, label:"礼物×1", reward:{gifts:1} },
  { spins:60, label:"矿石×3", reward:{coins:300,ore:3} },
  { spins:100, label:"赠送收获×3", reward:{freeSpins:3} },
  { spins:150, label:"丰收宝箱", reward:null },
];

const TOWN_EVENTS = [
  { title:"旅行商人到访", description:"集市外停下了一辆装满货物的马车。", choices:[
    { label:"帮忙卸货", detail:"木板 +15", reward:{wood:15} },
    { label:"请米娅接待", detail:"礼物 +2", reward:{gifts:2} },
  ]},
  { title:"河边发现矿石", description:"雨后的河滩闪着微光，似乎能带回一些材料。", choices:[
    { label:"带回矿石", detail:"矿石 +8", reward:{ore:8} },
    { label:"修整河岸", detail:"金币 +450", reward:{coins:450} },
  ]},
  { title:"面包节试吃", description:"面包房的新配方香气飘满了小镇。", choices:[
    { label:"分给居民", detail:"礼物 +2", reward:{gifts:2} },
    { label:"摆摊售卖", detail:"金币 +500", reward:{coins:500} },
  ]},
  { title:"灯塔需要维护", description:"米娅邀请你一起整理灯塔周围的旧木架。", choices:[
    { label:"加固木架", detail:"木板 +20", reward:{wood:20} },
    { label:"清理仓库", detail:"矿石 +6", reward:{ore:6} },
  ]},
];

function freshDailyStats() {
  return { spins:0, growth:0, scatters:0, coinsWon:0, symbols:0, symbolCounts:{}, ordersCompleted:0, eventsCompleted:0 };
}

function startingOrders() {
  return [0,1,2].map(templateId=>({templateId,startValue:0}));
}

function freshState() {
  return {
    coins: 5000, wood: 0, ore: 0, gifts: 0, friendshipXp: 0,
    buildings: { bakery:1, market:1, workshop:1, vault:1, lighthouse:1 },
    productiveSpins: 0, productiveDate: dateKey(), materialAccumulator: 0, materialUnits: 0,
    freeSpins: 0, lucky: 0, dailyNetLoss: 0, lossGrantClaimed: false,
    dailyDate: dateKey(), buff: null, buffDate: null, goalIndex: 0,
    totalSpins: 0, totalWon: 0, totalBet: 0, sound: true, volume: 70,
    dailyStats: freshDailyStats(), orders: startingOrders(), orderCursor: 3,
    milestoneClaims: [], nextTownEventAt: 25, pendingTownEvent: null,
    harvestChestClaimed: false, summaryShown: false, collectionClaims: [],
    lastSeen: Date.now(), log: ["欢迎来到麦穗小镇。"],
  };
}

let state = loadState();
let spinning = false;
let autoRemaining = 0;
let toastTimer;
let selectedBuildingKey = "bakery";

const $ = id => document.getElementById(id);
const fmt = value => Math.floor(value).toLocaleString("zh-CN");
function dateKey() { return new Date().toLocaleDateString("en-CA"); }

function normalizeState(raw) {
  const base = freshState();
  const merged = { ...base, ...raw, buildings: { ...base.buildings, ...(raw?.buildings || {}) } };
  merged.dailyStats = { ...base.dailyStats, ...(raw?.dailyStats || {}), symbolCounts:{...(raw?.dailyStats?.symbolCounts || {})} };
  if (!Array.isArray(merged.orders) || merged.orders.length !== 3) merged.orders = startingOrders();
  if (!Array.isArray(merged.milestoneClaims)) merged.milestoneClaims = [];
  if (!Array.isArray(merged.collectionClaims)) merged.collectionClaims = [];
  if (merged.dailyDate !== dateKey()) {
    merged.dailyDate = dateKey(); merged.productiveDate = dateKey(); merged.productiveSpins = 0;
    merged.dailyNetLoss = 0; merged.lossGrantClaimed = false; merged.materialAccumulator = 0;
    merged.buff = null; merged.buffDate = null;
    resetDailyContent(merged);
  }
  return merged;
}

function resetDailyContent(target=state) {
  target.dailyStats=freshDailyStats();target.orders=startingOrders();target.orderCursor=3;
  target.milestoneClaims=[];target.nextTownEventAt=25;target.pendingTownEvent=null;
  target.harvestChestClaimed=false;target.summaryShown=false;
}

function loadState() {
  try { return normalizeState(JSON.parse(localStorage.getItem(CONFIG.saveKey))); }
  catch { return freshState(); }
}

function saveState(show = false) {
  state.lastSeen = Date.now();
  try { localStorage.setItem(CONFIG.saveKey, JSON.stringify(state)); }
  catch { if (show) toast("浏览器阻止了本地存档，请通过静态服务器运行"); return; }
  if (show) toast("游戏已保存");
}

function rolloverIfNeeded() {
  if (state.dailyDate === dateKey()) return;
  state.dailyDate = dateKey(); state.productiveDate = dateKey(); state.productiveSpins = 0;
  state.dailyNetLoss = 0; state.lossGrantClaimed = false; state.materialAccumulator = 0;
  state.buff = null; state.buffDate = null;
  resetDailyContent();
  addLog("新的一天开始了，400次有效收获额度已经刷新。");
}

function applyOfflineReward() {
  const now = Date.now();
  const elapsedHours = Math.min(CONFIG.offlineCapHours, Math.max(0, (now - (state.lastSeen || now)) / 36e5));
  if (elapsedHours < .05) return;
  const multiplier = state.buff?.id === "offline" && state.buffDate === dateKey() ? 1.2 : 1;
  const coins = Math.floor(elapsedHours * offlineRate() * multiplier);
  state.coins += coins;
  const minutes = Math.floor(elapsedHours * 60);
  state.log.unshift(`离线${minutes}分钟，小镇仓库生产了${coins}金币。`);
  setTimeout(() => toast(`离线收益 +${fmt(coins)} 金币`), 450);
}

function currentRtp() {
  let gain = 0;
  for (const [key, data] of Object.entries(BUILDINGS)) gain += data.rtp * ((state.buildings[key] - 1) / 4);
  return 92 + gain;
}

function offlineRate() {
  const homeLevels = Object.values(state.buildings).reduce((a,b)=>a+b,0) - 5;
  return CONFIG.baseOfflinePerHour + homeLevels * 35;
}

function renderInitialGrid() {
  const ids = ["wheat","apple","milk","bread","gem","wild","scatter"];
  return Array.from({length:15}, (_,i) => ids[(i * 3 + Math.floor(i/5)) % ids.length]);
}

function renderGrid(grid, grown = new Set(), winners = new Set()) {
  const reels = $("reels"); reels.innerHTML = "";
  for (let row=0; row<3; row++) for (let col=0; col<5; col++) {
    const index = col*3+row, id = grid[index], data = SYMBOLS[id];
    const el = document.createElement("div");
    el.className = `symbol ${data.wild?"wild":""} ${data.scatter?"scatter":""} ${grown.has(index)?"grown":""} ${winners.has(index)?"winner":""}`;
    el.innerHTML = `<img src="${data.image}" alt="${data.name}"><small>${data.name}</small>`;
    reels.appendChild(el);
  }
}

function weightedSymbol() {
  const entries = Object.entries(SYMBOLS).map(([id,d]) => [id, d.weight]);
  let roll = Math.random() * entries.reduce((a,[,w])=>a+w,0);
  for (const [id,w] of entries) { roll -= w; if (roll <= 0) return id; }
  return "wheat";
}

function makeGrid() { return Array.from({length:15}, weightedSymbol); }
function idx(col,row) { return col*3+row; }

function applyGrowth(grid) {
  const grown = new Set();
  const visited = new Set();
  const cap = 2;
  for (let start=0; start<15; start++) {
    if (visited.has(start) || !SYMBOLS[grid[start]]?.normal) continue;
    const symbol = grid[start], component = [], queue = [start]; visited.add(start);
    while (queue.length) {
      const p = queue.shift(), c = Math.floor(p/3), r = p%3; component.push(p);
      [[c-1,r],[c+1,r],[c,r-1],[c,r+1]].forEach(([nc,nr]) => {
        const ni = idx(nc,nr);
        if (nc>=0&&nc<5&&nr>=0&&nr<3&&!visited.has(ni)&&grid[ni]===symbol) { visited.add(ni); queue.push(ni); }
      });
    }
    if (component.length < 3) continue;
    const candidates = [];
    component.forEach(p => {
      const c=Math.floor(p/3),r=p%3;
      [[c-1,r],[c+1,r],[c,r-1],[c,r+1]].forEach(([nc,nr])=>{
        const ni=idx(nc,nr); if(nc>=0&&nc<5&&nr>=0&&nr<3&&!component.includes(ni)&&SYMBOLS[grid[ni]]?.normal&&!candidates.includes(ni)) candidates.push(ni);
      });
    });
    candidates.sort(()=>Math.random()-.5).slice(0,cap-grown.size).forEach(ni=>{grid[ni]=symbol;grown.add(ni);});
    if (grown.size >= cap) break;
  }
  return grown;
}

function evaluate(grid) {
  let payout = 0; const winning = new Set(); const wins = [];
  PAYLINES.forEach((rows,lineNo) => {
    const line = rows.map((r,c)=>idx(c,r));
    const ids = line.map(i=>grid[i]);
    let target = ids.find(id=>id!=="wild" && id!=="scatter");
    if (!target && ids[0] === "wild") target = "wild";
    if (!target || ids[0] === "scatter") return;
    let count=0;
    for (const id of ids) { if (id===target || id==="wild") count++; else break; }
    if (count >= 3 && SYMBOLS[target].pays?.[count]) {
      let value = SYMBOLS[target].pays[count];
      payout += value; line.slice(0,count).forEach(i=>winning.add(i)); wins.push(`${SYMBOLS[target].name}${count}连`);
    }
  });
  const scatters = grid.reduce((n,id)=>n+(id==="scatter"),0);
  return { payout, winning, wins, scatters };
}

async function spin() {
  if (spinning) return;
  rolloverIfNeeded();
  const isFree = state.freeSpins > 0;
  if (!isFree && state.coins < CONFIG.bet) { toast("金币不足，离线一会儿再回来吧"); return; }
  spinning = true; $("spinBtn").disabled = true; $("reels").classList.add("spinning");
  if (isFree) state.freeSpins--; else { state.coins -= CONFIG.bet; state.totalBet += CONFIG.bet; }
  state.totalSpins++;
  for (let i=0;i<4;i++) { renderGrid(makeGrid()); await delay(75); }
  const grid = makeGrid();
  const grown = applyGrowth(grid);
  let result = evaluate(grid);
  // Internal reward table uses readable points, then applies the calibrated coin scale.
  const growthProgress = (currentRtp() - 92) / 4;
  const payoutScale = CONFIG.basePayoutScale + (CONFIG.maxPayoutScale - CONFIG.basePayoutScale) * growthProgress;
  let payout = Math.round(result.payout * payoutScale);
  let guarantee = false;
  const qualifyingLoss = payout < CONFIG.bet;
  if (qualifyingLoss) state.lucky++; else state.lucky = 0;
  if (state.lucky >= CONFIG.luckyThreshold) {
    const grant = Math.max(18, CONFIG.bet - payout + 12);
    payout += grant; state.lucky = 0; guarantee = true;
  }
  if (result.scatters >= 3) {
    state.freeSpins += CONFIG.freeSpinsAward + Math.max(0,result.scatters-3)*2;
    result.wins.push(`丰收篮${result.scatters}个`);
  }
  state.coins += payout; state.totalWon += payout;
  const net = (isFree ? payout : payout-CONFIG.bet);
  if (net < 0) state.dailyNetLoss += -net;
  else state.dailyNetLoss = Math.max(0,state.dailyNetLoss-net);
  grantProductiveRewards();
  checkLossProtection();
  recordDailySpin(grid,grown,result,payout);
  $("reels").classList.remove("spinning"); renderGrid(grid,grown,result.winning);
  const pieces = [];
  if (payout) pieces.push(`获得 ${payout} 金币`); else pieces.push("本次没有金币奖励");
  if (grown.size) pieces.push(`连锁生长 ${grown.size} 格`);
  if (guarantee) pieces.push("互助能量送来补给");
  if (result.scatters>=3) pieces.push(`赠送收获 +${CONFIG.freeSpinsAward}`);
  $("roundMessage").textContent = pieces.join(" · ");
  addLog(pieces.join("，") + "。");
  renderAll(); saveState();
  await delay(380); spinning=false; $("spinBtn").disabled=false;
  if(state.pendingTownEvent!==null){autoRemaining=0;renderAll();openTownEvent();return}
  if (autoRemaining > 0) { autoRemaining--; renderAll(); setTimeout(spin, 260); }
  else { autoRemaining=0; renderAll(); }
}

function grantProductiveRewards() {
  if (state.productiveSpins >= CONFIG.dailyProductiveCap) return;
  state.productiveSpins++;
  const materialBoost = state.buff?.id === "material" && state.buffDate === dateKey() ? 1.1 : 1;
  state.materialAccumulator += materialBoost;
  const whole = Math.floor(state.materialAccumulator); state.materialAccumulator -= whole;
  for (let i=0; i<whole; i++) {
    state.wood++; state.materialUnits++;
    if (state.materialUnits % 3 === 0) state.ore++;
  }
  const giftEvery = state.buff?.id === "gift" && state.buffDate === dateKey() ? 8 : 10;
  if (state.productiveSpins % giftEvery === 0) state.gifts++;
}

function recordDailySpin(grid,grown,result,payout) {
  const stats=state.dailyStats;
  stats.spins++;stats.growth+=grown.size;stats.scatters+=result.scatters;stats.coinsWon+=payout;stats.symbols+=grid.length;
  grid.forEach(id=>stats.symbolCounts[id]=(stats.symbolCounts[id]||0)+1);
  if(stats.spins>=state.nextTownEventAt&&state.pendingTownEvent===null){
    state.pendingTownEvent=stats.eventsCompleted%TOWN_EVENTS.length;
  }
  if(stats.spins>=100&&!state.summaryShown){
    state.summaryShown=true;
    setTimeout(()=>{if(!$("eventDialog").open)showDailySummary()},700);
  }
}

function applyReward(reward) {
  if(!reward)return;
  state.coins+=reward.coins||0;state.wood+=reward.wood||0;state.ore+=reward.ore||0;
  state.gifts+=reward.gifts||0;state.freeSpins+=reward.freeSpins||0;
}

function rewardText(reward) {
  return [
    reward.coins&&`金币${reward.coins}`,reward.wood&&`木板${reward.wood}`,
    reward.ore&&`矿石${reward.ore}`,reward.gifts&&`礼物${reward.gifts}`,
    reward.freeSpins&&`赠送收获${reward.freeSpins}`
  ].filter(Boolean).join(" · ");
}

function metricValue(template) {
  const stats=state.dailyStats;
  if(template.metric==="symbol")return stats.symbolCounts[template.key]||0;
  if(template.metric==="growth")return stats.growth;
  if(template.metric==="spins")return stats.spins;
  if(template.metric==="coins")return stats.coinsWon;
  if(template.metric==="symbols")return stats.symbols;
  if(template.metric==="scatter")return stats.scatters;
  return 0;
}

function orderProgress(order) {
  const template=ORDER_TEMPLATES[order.templateId];
  return Math.max(0,metricValue(template)-order.startValue);
}

function claimOrder(slot) {
  const order=state.orders[slot],template=ORDER_TEMPLATES[order.templateId],progress=orderProgress(order);
  if(progress<template.target)return;
  applyReward(template.reward);state.dailyStats.ordersCompleted++;
  const nextId=state.orderCursor%ORDER_TEMPLATES.length;const next=ORDER_TEMPLATES[nextId];
  state.orders[slot]={templateId:nextId,startValue:metricValue(next)};state.orderCursor++;
  toast(`订单完成：${rewardText(template.reward)}`);addLog(`完成“${template.title}”，获得${rewardText(template.reward)}。`);
  renderAll();saveState();
}

function claimMilestone(index) {
  const milestone=MILESTONES[index];
  if(state.dailyStats.spins<milestone.spins||state.milestoneClaims.includes(index))return;
  if(index===MILESTONES.length-1){claimHarvestChest();return}
  applyReward(milestone.reward);state.milestoneClaims.push(index);
  toast(`阶段奖励：${rewardText(milestone.reward)}`);renderAll();saveState();
}

function claimHarvestChest() {
  if(state.dailyStats.spins<150||state.harvestChestClaimed)return;
  const reward={coins:1500,wood:30,ore:10,gifts:3};
  applyReward(reward);state.harvestChestClaimed=true;state.milestoneClaims.push(4);
  toast("丰收宝箱：金币1500、木板30、矿石10、礼物3");addLog("完成今日150次收获路线，开启丰收宝箱。");
  renderAll();saveState();showDailySummary();
}

function openTownEvent() {
  if(state.pendingTownEvent===null){toast(`再收获${Math.max(0,state.nextTownEventAt-state.dailyStats.spins)}次触发小镇事件`);return}
  const event=TOWN_EVENTS[state.pendingTownEvent];
  $("eventTitle").textContent=event.title;$("eventDescription").textContent=event.description;
  [[$("eventChoiceA"),event.choices[0]],[$("eventChoiceB"),event.choices[1]]].forEach(([button,choice])=>{
    button.querySelector("b").textContent=choice.label;button.querySelector("small").textContent=choice.detail;
  });
  $("eventDialog").showModal();
}

function resolveTownEvent(choiceIndex) {
  if(state.pendingTownEvent===null)return;
  const event=TOWN_EVENTS[state.pendingTownEvent],choice=event.choices[choiceIndex];
  applyReward(choice.reward);state.dailyStats.eventsCompleted++;state.pendingTownEvent=null;state.nextTownEventAt+=25;
  $("eventDialog").close();toast(`${event.title}：${choice.detail}`);addLog(`${event.title}，选择“${choice.label}”。`);
  renderAll();saveState();
}

function checkLossProtection() {
  if (!state.lossGrantClaimed && state.dailyNetLoss > CONFIG.lossProtectionThreshold) {
    state.coins += CONFIG.lossProtectionGrant; state.lossGrantClaimed = true;
    toast(`小镇互助金 +${CONFIG.lossProtectionGrant}`); addLog("小镇互助机制生效，获得1200金币恢复金。");
  }
}

function giveGifts() {
  if (!state.gifts) { toast("还没有礼物，继续收获即可发现"); return; }
  state.friendshipXp += state.gifts; const amount=state.gifts; state.gifts=0;
  const buff = BUFFS[Math.floor(state.friendshipXp/5) % BUFFS.length];
  state.buff = buff; state.buffDate = dateKey();
  addLog(`赠送了${amount}份礼物，米娅送来“${buff.name}”。`); toast(`今日祝福：${buff.text}`); renderAll(); saveState();
}

function buildingLevelTotal() { return Object.values(state.buildings).reduce((a,b)=>a+b,0); }
function upgradeBuilding(key) {
  const data=BUILDINGS[key], level=state.buildings[key]; if(level>=5)return;
  const cost=data.costs[level], mats=data.materials?.[level];
  if(state.coins<cost){toast("金币不足");return}
  if(mats&&(state.wood<mats.wood||state.ore<mats.ore)){toast("建材不足");return}
  state.coins-=cost; if(mats){state.wood-=mats.wood;state.ore-=mats.ore}
  state.buildings[key]++; addLog(`${data.name}升至${level+1}级，小镇收获加成提升至+${(currentRtp()-92).toFixed(1)}%。`);
  toast(`${data.name}升级成功`); renderAll(); saveState();
}

function renderBuildings() {
  const grid=$("buildingGrid"); grid.innerHTML="";
  Object.entries(BUILDINGS).forEach(([key,data])=>{
    const level=state.buildings[key];
    const node=document.createElement("button");
    node.className=`town-building-node ${selectedBuildingKey===key?"selected":""}`;
    node.dataset.building=key;
    node.setAttribute("aria-label",`${data.name}，等级${level}`);
    node.innerHTML=`<img src="${data.image}" alt=""><span class="node-label">${data.name}<em class="node-level">Lv.${level}</em></span>`;
    node.addEventListener("click",()=>{selectedBuildingKey=key;renderBuildings();renderSelectedBuilding()});
    grid.appendChild(node);
  });
  renderSelectedBuilding();
}

function renderSelectedBuilding() {
  const key=selectedBuildingKey, data=BUILDINGS[key], level=state.buildings[key];
  const max=level>=5, cost=max?0:data.costs[level], mats=data.materials?.[level];
  const can=!max&&state.coins>=cost&&(!mats||(state.wood>=mats.wood&&state.ore>=mats.ore));
  $("selectedBuildingImage").src=data.image;
  $("selectedBuildingImage").alt=data.name;
  $("selectedBuildingName").textContent=data.name;
  $("selectedBuildingLevel").textContent=`等级 ${level}/5`;
  $("selectedBuildingDesc").textContent=data.desc;
  $("selectedBuildingEffect").textContent=`当前加成 +${(data.rtp*((level-1)/4)).toFixed(1)}% · 满级 +${data.rtp.toFixed(1)}%`;
  $("selectedMaterialCost").textContent=mats?`木板${mats.wood} · 矿石${mats.ore}`:"";
  $("selectedBuildingCost").textContent=max?"已满级":`${fmt(cost)}金币`;
  $("upgradeSelectedBtn").disabled=!can;
  $("upgradeSelectedBtn").firstChild.textContent=max?"满级":"升级";
}

function renderGoal() {
  const goal=GOALS[state.goalIndex];
  if(!goal){$("goalText").textContent="全部MVP阶段目标已完成";$("goalPercent").textContent="100%";$("goalBar").style.width="100%";$("claimGoalBtn").disabled=true;$("claimGoalBtn").textContent="已完成";return}
  const done=goal.test(state);$("goalText").textContent=`${goal.text} · 奖励${fmt(goal.reward)}金币`;$("goalPercent").textContent=done?"100%":"进行中";$("goalBar").style.width=done?"100%":"35%";$("claimGoalBtn").disabled=!done;$("claimGoalBtn").textContent=done?"领取阶段奖励":"尚未完成";
}

function renderOrders() {
  const list=$("orderList");list.innerHTML="";
  state.orders.forEach((order,slot)=>{
    const template=ORDER_TEMPLATES[order.templateId],progress=Math.min(template.target,orderProgress(order)),ready=progress>=template.target;
    const card=document.createElement("article");card.className="order-card";
    card.innerHTML=`<img class="order-icon" src="${template.icon}" alt=""><h3>${template.title}</h3><p>${template.text}</p><div class="order-progress"><i style="width:${progress/template.target*100}%"></i></div><div class="order-meta"><span>${progress} / ${template.target}</span><span>${rewardText(template.reward)}</span></div><button class="order-claim ${ready?"ready":""}" ${ready?"":"disabled"}>${ready?"领取":"进行中"}</button>`;
    card.querySelector("button").addEventListener("click",()=>claimOrder(slot));list.appendChild(card);
  });
}

function journeyHasReward() {
  const orderReady=state.orders.some(order=>orderProgress(order)>=ORDER_TEMPLATES[order.templateId].target);
  const milestoneReady=MILESTONES.some((m,i)=>state.dailyStats.spins>=m.spins&&!state.milestoneClaims.includes(i));
  const collectionReady=COLLECTIONS.some(item=>collectionProgress(item)>=item.target&&!state.collectionClaims.includes(item.id));
  return orderReady||milestoneReady||collectionReady||state.pendingTownEvent!==null;
}

function collectionProgress(item) {
  return Math.min(...item.icons.map(key=>state.dailyStats.symbolCounts[key]||0));
}

function renderCollection() {
  const list=$("collectionList"); if(!list)return;
  list.innerHTML="";
  COLLECTIONS.forEach(item=>{
    const progress=collectionProgress(item),ready=progress>=item.target,claimed=state.collectionClaims.includes(item.id);
    const row=document.createElement("article");row.className=`collection-row ${ready?"ready":""} ${claimed?"claimed":""}`;
    row.innerHTML=`<div class="collection-icons">${item.icons.map(key=>`<img src="${SYMBOLS[key].image}" alt="${SYMBOLS[key].name}">`).join("")}</div><div class="collection-copy"><b>${item.title}</b><small>${item.text}</small><div class="collection-bar"><i style="width:${Math.min(100,progress/item.target*100)}%"></i></div><em>${progress} / ${item.target} · ${rewardText(item.reward)}</em></div><button ${ready&&!claimed?"":"disabled"}>${claimed?"已领取":ready?"领取":"收集中"}</button>`;
    row.querySelector("button").addEventListener("click",()=>claimCollection(item.id));
    list.appendChild(row);
  });
}

function claimCollection(id) {
  const item=COLLECTIONS.find(entry=>entry.id===id); if(!item)return;
  if(state.collectionClaims.includes(id))return;
  if(collectionProgress(item)<item.target){toast("图鉴尚未完成");return}
  state.collectionClaims.push(id); applyReward(item.reward); addLog(`完成图鉴“${item.title}”，获得${rewardText(item.reward)}。`);
  toast(`图鉴奖励：${rewardText(item.reward)}`); renderAll(); renderCollection(); saveState();
}

function openCollection() {
  renderCollection();
  $("collectionDialog").showModal();
}

function renderJourney() {
  const spins=state.dailyStats.spins;
  $("journeySpinCount").textContent=spins;$("routeProgressText").textContent=`${Math.min(spins,150)} / 150`;
  $("routeProgressBar").style.width=`${Math.min(100,spins/150*100)}%`;$("ordersCompleted").textContent=state.dailyStats.ordersCompleted;
  const track=$("milestoneTrack");track.innerHTML="";
  MILESTONES.forEach((milestone,index)=>{
    const reached=spins>=milestone.spins,claimed=state.milestoneClaims.includes(index);
    const button=document.createElement("button");button.className=`milestone ${reached?"reached":""} ${claimed?"claimed":""}`;
    button.innerHTML=`<span class="medal">${milestone.spins}</span><strong>${milestone.spins}次</strong><small>${claimed?"已领取":milestone.label}</small>`;
    button.addEventListener("click",()=>claimMilestone(index));track.appendChild(button);
  });
  const remaining=Math.max(0,state.nextTownEventAt-spins);
  $("nextEventText").textContent=state.pendingTownEvent!==null?"事件等待处理":`再收获${remaining}次触发`;
  $("summaryHint").textContent=spins>=100?"今日记录已可回顾":`再完成${Math.max(0,100-spins)}次形成完整总结`;
  $("harvestChestBtn").disabled=spins<150||state.harvestChestClaimed;
  $("harvestChestBtn").querySelector("span").textContent=state.harvestChestClaimed?"今日宝箱已领取":"150次丰收宝箱";
  const readyCollections=COLLECTIONS.filter(item=>collectionProgress(item)>=item.target&&!state.collectionClaims.includes(item.id)).length;
  $("collectionHint").textContent=readyCollections?`${readyCollections} 项奖励可领`:`${state.collectionClaims.length}/${COLLECTIONS.length} 已完成`;
  $("journeyBadge").classList.toggle("hidden",!journeyHasReward());
  const nextOrder=state.orders.map((order,index)=>({order,index,progress:orderProgress(order)})).find(item=>item.progress<ORDER_TEMPLATES[item.order.templateId].target);
  $("orderTeaser").textContent=nextOrder?`${ORDER_TEMPLATES[nextOrder.order.templateId].title} · ${nextOrder.progress}/${ORDER_TEMPLATES[nextOrder.order.templateId].target}`:"订单奖励等待领取";
  renderOrders();renderCollection();
}

function showDailySummary() {
  const stats=state.dailyStats;
  $("summarySpins").textContent=stats.spins;$("summaryCoins").textContent=fmt(stats.coinsWon);
  $("summaryGrowth").textContent=stats.growth;$("summaryOrders").textContent=stats.ordersCompleted;
  $("summaryEvents").textContent=stats.eventsCompleted;$("summarySymbols").textContent=stats.symbols;
  $("summaryMessage").textContent=stats.spins>=150?"今日丰收路线已经完成，明天再来建设小镇吧。":stats.spins>=100?"今日已经形成完整收获记录，还可以继续挑战150次丰收宝箱。":"继续完成订单，100次收获后形成完整日报。";
  if(!$("summaryDialog").open)$("summaryDialog").showModal();
}

function claimGoal() {
  const goal=GOALS[state.goalIndex];if(!goal||!goal.test(state))return;state.coins+=goal.reward;state.goalIndex++;toast(`阶段奖励 +${fmt(goal.reward)}金币`);addLog(`完成阶段目标，领取${goal.reward}金币。`);renderAll();saveState();
}

function renderAll() {
  $("coins").textContent=fmt(state.coins);$("wood").textContent=fmt(state.wood);$("ore").textContent=fmt(state.ore);
  $("productiveSpins").textContent=state.productiveSpins;$("productiveBar").style.width=`${state.productiveSpins/CONFIG.dailyProductiveCap*100}%`;
  $("rtpValue").textContent=`成长+${(currentRtp()-92).toFixed(1)}%`;$("townRtpValue").textContent=`+${(currentRtp()-92).toFixed(1)}%`;$("offlineRate").textContent=`${fmt(offlineRate())}/小时`;$("warehouseRate").textContent=`${fmt(offlineRate())} 金币/小时`;$("buildingLevels").textContent=buildingLevelTotal();
  $("friendshipLevel").textContent=1+Math.floor(state.friendshipXp/10);$("freeSpinsBadge").classList.toggle("hidden",state.freeSpins<=0);$("freeSpinsBadge").querySelector("b").textContent=state.freeSpins;
  $("spinBtn").querySelector("small").textContent=state.freeSpins>0?"赠送收获":"10 金币";
  $("autoBtn").querySelector("small").textContent=autoRemaining?`停止·${autoRemaining}`:"连续×20";$("autoBtn").classList.toggle("active",autoRemaining>0);
  $("luckyPips").innerHTML=Array.from({length:CONFIG.luckyThreshold},(_,i)=>`<i class="${i<state.lucky?"on":""}"></i>`).join("");
  const activeBuff=state.buff&&state.buffDate===dateKey()?state.buff:null;
  $("buffCard").innerHTML=activeBuff?`<b>${activeBuff.name}</b><small>${activeBuff.text} · 礼物 <em id="gifts">${fmt(state.gifts)}</em></small>`:`<b>祝福未激活</b><small>礼物 <em id="gifts">${fmt(state.gifts)}</em></small>`;
  $("townBuffSummary").textContent=activeBuff?`${activeBuff.name}：${activeBuff.text}`:"当前没有生效中的祝福";
  $("soundToggleBtn").classList.toggle("on",state.sound);
  $("soundToggleBtn").setAttribute("aria-checked",String(state.sound));
  $("soundToggleBtn").querySelector("span").textContent=state.sound?"开启":"关闭";
  $("volumeRange").value=state.volume;
  $("volumeRange").disabled=!state.sound;
  $("volumeValue").textContent=`${state.volume}%`;
  renderBuildings();renderGoal();renderJourney();
}

function addLog(text){state.log.unshift(text);state.log=state.log.slice(0,20)}
function delay(ms){return new Promise(r=>setTimeout(r,ms))}
function toast(text){const el=$("toast");el.textContent=text;el.classList.add("show");clearTimeout(toastTimer);toastTimer=setTimeout(()=>el.classList.remove("show"),2200)}
function switchView(name){document.querySelectorAll(".view").forEach(v=>v.classList.toggle("active",v.id===`${name}View`));document.querySelectorAll(".nav-item[data-view]").forEach(v=>v.classList.toggle("active",v.dataset.view===name));window.scrollTo({top:0,behavior:"smooth"})}
function sanitizeLoginInput(input) {
  const clean = input.value.replace(/[^a-zA-Z0-9]/g, "");
  if (input.value !== clean) {
    input.value = clean;
    toast("账号和密码只能输入字母或数字");
  }
}
function agreementAccepted() {
  if (!$("agreeCheck").checked) {
    toast("请先阅读并同意隐私协议和用户协议");
    return false;
  }
  return true;
}

function enterGame() {
  $("authGate").classList.add("hidden");
  $("gameShell").classList.remove("hidden");
  switchView("lobby");
}

function showPolicy(kind) {
  const isPrivacy = kind === "privacy";
  $("policyTitle").textContent = isPrivacy ? "隐私协议" : "用户协议";
  $("policyContent").innerHTML = isPrivacy
    ? `<p>本原型仅在当前设备保存游戏进度、声音设置、协议同意状态和游客登录状态。</p><p>本原型不会收集真实姓名、手机号、身份证、定位、通讯录或支付信息，也不提供现金、实物或外部价值兑换。</p><p>后续正式上架时，应根据目标地区和平台要求接入完整隐私政策、数据清单、儿童隐私与第三方 SDK 说明。</p>`
    : `<p>欢迎体验麦穗小镇。本游戏为休闲经营原型，所有金币、木板、矿石、礼物和奖励均为游戏内虚拟资源。</p><p>游客登录仅用于本设备体验，不代表已创建线上账号。点击 Log in 会模拟账号不存在，不会进入游戏。</p><p>请合理安排游玩时间。继续进入即表示你理解并同意上述体验规则。</p>`;
  $("policyDialog").showModal();
}

function initAuthGate() {
  const accepted = localStorage.getItem(CONFIG.agreementKey) === "true";
  $("agreeCheck").checked = accepted;
  $("authGate").classList.remove("hidden");
  $("gameShell").classList.add("hidden");
}

function showPaytable(){
  $("dialogTitle").textContent="收获说明";
  $("dialogContent").innerHTML=`<p>每次田园收获消耗10金币，所有丰收路线固定开放。相同符号从最左侧连续出现时会获得金币奖励；风车可帮助普通符号完成连续收获。</p><div class="paytable">${Object.values(SYMBOLS).filter(s=>s.pays).map(s=>`<div class="pay-row"><img src="${s.image}" alt="${s.name}"><span>3连 ${s.pays[3]} · 4连 ${s.pays[4]} · 5连 ${s.pays[5]}</span></div>`).join("")}</div><p>丰收篮3个或以上会获得5次赠送收获。相邻3个同类产业符号会先发生连锁生长，再计算收获奖励。连续多次低收获时，互助能量会送来一次公开补给。</p>`;
  $("infoDialog").showModal();
}

function resetGame(){if(!confirm("确定清除全部小镇进度吗？此操作不可撤销。"))return;localStorage.removeItem(CONFIG.saveKey);state=freshState();autoRemaining=0;renderGrid(renderInitialGrid());renderAll();saveState();toast("小镇已重新开始")}

document.querySelectorAll("[data-view]").forEach(btn=>btn.addEventListener("click",()=>switchView(btn.dataset.view)));
["accountInput","passwordInput"].forEach(id=>$(`${id}`).addEventListener("input",event=>sanitizeLoginInput(event.target)));
$("guestLoginBtn").addEventListener("click",()=>{
  if(!agreementAccepted())return;
  localStorage.setItem(CONFIG.agreementKey,"true");
  enterGame();
});
$("accountLoginBtn").addEventListener("click",()=>{
  if(!agreementAccepted())return;
  sanitizeLoginInput($("accountInput"));sanitizeLoginInput($("passwordInput"));
  if(!$("accountInput").value || !$("passwordInput").value){toast("请输入账号和密码");return}
  toast("账号不存在，请使用游客登录体验");
});
$("privacyLink").addEventListener("click",()=>showPolicy("privacy"));
$("termsLink").addEventListener("click",()=>showPolicy("terms"));
$("closePolicy").addEventListener("click",()=>$("policyDialog").close());
$("lobbySettingsBtn").addEventListener("click",()=>$("settingsDialog").showModal());
$("spinBtn").addEventListener("click",spin);
$("autoBtn").addEventListener("click",()=>{if(autoRemaining){autoRemaining=0;renderAll()}else{autoRemaining=20;renderAll();spin()}});
$("giveGiftBtn").addEventListener("click",giveGifts);$("claimGoalBtn").addEventListener("click",claimGoal);
$("upgradeSelectedBtn").addEventListener("click",()=>upgradeBuilding(selectedBuildingKey));
$("eventPreviewBtn").addEventListener("click",openTownEvent);
$("eventChoiceA").addEventListener("click",()=>resolveTownEvent(0));$("eventChoiceB").addEventListener("click",()=>resolveTownEvent(1));
$("summaryBtn").addEventListener("click",showDailySummary);$("closeSummary").addEventListener("click",()=>$("summaryDialog").close());
$("collectionBtn").addEventListener("click",openCollection);$("closeCollection").addEventListener("click",()=>$("collectionDialog").close());
$("harvestChestBtn").addEventListener("click",claimHarvestChest);
$("paytableBtn").addEventListener("click",showPaytable);$("closeDialog").addEventListener("click",()=>$("infoDialog").close());
$("saveBtn").addEventListener("click",()=>saveState(true));$("resetBtn").addEventListener("click",resetGame);
$("settingsBtn").addEventListener("click",()=>$("settingsDialog").showModal());
$("closeSettings").addEventListener("click",()=>$("settingsDialog").close());
$("soundToggleBtn").addEventListener("click",()=>{state.sound=!state.sound;renderAll();saveState()});
$("volumeRange").addEventListener("input",event=>{state.volume=Number(event.target.value);$("volumeValue").textContent=`${state.volume}%`;saveState()});
window.addEventListener("beforeunload",()=>saveState());document.addEventListener("visibilitychange",()=>{if(document.hidden)saveState()});

applyOfflineReward();renderGrid(renderInitialGrid());renderAll();saveState();initAuthGate();
