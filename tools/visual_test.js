const { chromium } = require("playwright");
const path = require("path");

(async () => {
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({
    viewport: { width: 430, height: 932 },
    deviceScaleFactor: 3,
  });
  const errors = [];
  page.on("console", message => {
    if (message.type() === "error") errors.push(message.text());
  });
  page.on("pageerror", error => errors.push(error.message));
  await page.goto(`file://${path.resolve(__dirname, "../index.html")}`);
  await page.evaluate(() => localStorage.clear());
  await page.reload();
  await page.waitForTimeout(700);
  const authInitial = await page.evaluate(() => ({
    authVisible: !document.querySelector("#authGate").classList.contains("hidden"),
    gameHidden: document.querySelector("#gameShell").classList.contains("hidden"),
    checked: document.querySelector("#agreeCheck").checked,
  }));
  await page.screenshot({ path: path.resolve(__dirname, "../previews/redesign-auth.png") });
  await page.click("#accountLoginBtn");
  await page.waitForTimeout(120);
  const loginBlockedToast = await page.evaluate(() => document.querySelector("#toast").textContent);
  await page.fill("#accountInput", "farm@001!");
  await page.fill("#passwordInput", "pass#123");
  await page.waitForTimeout(120);
  const sanitizedLogin = await page.evaluate(() => ({
    account: document.querySelector("#accountInput").value,
    password: document.querySelector("#passwordInput").value,
    toast: document.querySelector("#toast").textContent,
  }));
  await page.click("#privacyLink");
  await page.waitForSelector("#policyDialog[open]");
  const privacyOpen = await page.evaluate(() => ({
    open: document.querySelector("#policyDialog").open,
    title: document.querySelector("#policyTitle").textContent,
  }));
  await page.click("#closePolicy");
  await page.click("#guestLoginBtn");
  await page.waitForTimeout(120);
  const noAgreementToast = await page.evaluate(() => document.querySelector("#toast").textContent);
  await page.check("#agreeCheck");
  await page.fill("#accountInput", "farm001");
  await page.fill("#passwordInput", "pass123");
  await page.click("#accountLoginBtn");
  await page.waitForTimeout(120);
  const loginToast = await page.evaluate(() => document.querySelector("#toast").textContent);
  await page.waitForTimeout(2300);
  await page.click("#guestLoginBtn");
  await page.waitForFunction(() => document.querySelector("#gameShell") && !document.querySelector("#gameShell").classList.contains("hidden"));
  const lobbyMetrics = await page.evaluate(() => ({
    active: document.querySelector(".view.active").id,
    authHidden: document.querySelector("#authGate").classList.contains("hidden"),
    cards: document.querySelectorAll(".lobby-card").length,
    body: [document.body.scrollWidth, document.body.scrollHeight],
  }));
  await page.screenshot({ path: path.resolve(__dirname, "../previews/redesign-lobby.png") });
  await page.click('.lobby-card[data-view="slot"]');
  await page.waitForFunction(() => document.querySelector(".view.active").id === "slotView");
  const slotMetrics = await page.evaluate(() => ({
    viewport: [innerWidth, innerHeight],
    body: [document.body.scrollWidth, document.body.scrollHeight],
    shell: [
      document.querySelector(".app-shell").clientWidth,
      document.querySelector(".app-shell").clientHeight,
      document.querySelector(".app-shell").scrollWidth,
      document.querySelector(".app-shell").scrollHeight,
    ],
    reels: document.querySelectorAll(".symbol").length,
  }));
  await page.screenshot({ path: path.resolve(__dirname, "../previews/redesign-slot.png") });
  await page.click("#spinBtn");
  await page.waitForFunction(() => !document.querySelector("#spinBtn").disabled);
  const postSpin = await page.evaluate(() => ({
    coins: document.querySelector("#coins").textContent,
    productiveSpins: document.querySelector("#productiveSpins").textContent,
    symbols: document.querySelectorAll(".symbol").length,
  }));
  await page.click("#settingsBtn");
  await page.waitForSelector("#settingsDialog[open]");
  await page.click("#soundToggleBtn");
  await page.click("#soundToggleBtn");
  await page.locator("#volumeRange").fill("55");
  const settings = await page.evaluate(() => ({
    open: document.querySelector("#settingsDialog").open,
    sound: document.querySelector("#soundToggleBtn").getAttribute("aria-checked"),
    volume: document.querySelector("#volumeValue").textContent,
  }));
  await page.screenshot({ path: path.resolve(__dirname, "../previews/redesign-settings.png") });
  await page.click("#closeSettings");
  await page.click('.bottom-nav [data-view="journey"]');
  await page.evaluate(() => {
    state.dailyStats.spins = 37;
    state.dailyStats.growth = 3;
    state.dailyStats.symbols = 555;
    state.dailyStats.symbolCounts.wheat = 15;
    state.nextTownEventAt = 50;
    renderAll();
  });
  const journeyMetrics = await page.evaluate(() => ({
    active: document.querySelector(".view.active").id,
    orders: document.querySelectorAll(".order-card").length,
    milestones: document.querySelectorAll(".milestone").length,
    body: [document.body.scrollWidth, document.body.scrollHeight],
  }));
  await page.screenshot({ path: path.resolve(__dirname, "../previews/redesign-journey.png") });
  await page.click(".order-card:nth-child(3) .order-claim");
  const orderClaimed = await page.evaluate(() => ({
    completed: state.dailyStats.ordersCompleted,
    orders: state.orders.length,
  }));
  await page.evaluate(() => { state.pendingTownEvent = 0; renderAll(); });
  await page.click("#eventPreviewBtn");
  await page.waitForSelector("#eventDialog[open]");
  await page.screenshot({ path: path.resolve(__dirname, "../previews/redesign-event.png") });
  await page.click("#eventChoiceA");
  const eventResolved = await page.evaluate(() => ({
    open: document.querySelector("#eventDialog").open,
    pending: state.pendingTownEvent,
    events: state.dailyStats.eventsCompleted,
  }));
  await page.evaluate(() => {
    state.dailyStats.symbolCounts.wheat = 20;
    state.dailyStats.symbolCounts.apple = 20;
    state.dailyStats.symbolCounts.milk = 20;
    renderAll();
  });
  await page.click("#collectionBtn");
  await page.waitForSelector("#collectionDialog[open]");
  const collectionMetrics = await page.evaluate(() => ({
    rows: document.querySelectorAll(".collection-row").length,
    ready: document.querySelectorAll(".collection-row.ready").length,
  }));
  await page.screenshot({ path: path.resolve(__dirname, "../previews/redesign-collection.png") });
  await page.click(".collection-row.ready button");
  const collectionClaimed = await page.evaluate(() => ({
    claims: state.collectionClaims.length,
    open: document.querySelector("#collectionDialog").open,
  }));
  await page.click("#closeCollection");
  await page.evaluate(() => { state.dailyStats.spins = 150; renderAll(); });
  await page.click("#harvestChestBtn");
  await page.waitForSelector("#summaryDialog[open]");
  const chestClaimed = await page.evaluate(() => ({
    claimed: state.harvestChestClaimed,
    milestone: state.milestoneClaims.includes(4),
  }));
  await page.screenshot({ path: path.resolve(__dirname, "../previews/redesign-summary.png") });
  await page.click("#closeSummary");
  await page.click('.bottom-nav [data-view="town"]');
  await page.waitForTimeout(300);
  await page.click('[data-building="lighthouse"]');
  const townMetrics = await page.evaluate(() => ({
    active: document.querySelector(".view.active").id,
    nodes: document.querySelectorAll(".town-building-node").length,
    selected: document.querySelector(".town-building-node.selected")?.dataset.building,
    body: [document.body.scrollWidth, document.body.scrollHeight],
  }));
  await page.screenshot({ path: path.resolve(__dirname, "../previews/redesign-town.png") });
  console.log(JSON.stringify({ authInitial, loginBlockedToast, sanitizedLogin, loginToast, privacyOpen, noAgreementToast, lobbyMetrics, slotMetrics, postSpin, settings, journeyMetrics, orderClaimed, eventResolved, collectionMetrics, collectionClaimed, chestClaimed, townMetrics, errors }, null, 2));
  await browser.close();
})();
