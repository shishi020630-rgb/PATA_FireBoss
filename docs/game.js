(() => {
  const canvas = document.getElementById("game");
  const ctx = canvas.getContext("2d", { alpha: false });
  const W = 960;
  const H = 540;

  const assetFiles = {
    bg: "3_J.jpg",
    bgAlt: "2_J.jpg",
    logo: "logo.png",
    cardArmor: "img_Card_FHKJ.png",
    cardBow: "img_Card_FHCG.png",
    cardQuick: "img_Card_MG.png",
    win: "win_1.png",
    fail: "fail_1.png",
    fail2: "fail_2.png",
    bossIcon: "FireIcon.png",
    spiderIcon: "SpiderMotherIcon.png",
    finger: "img_guide_finger.png",
    bossBarBack: "img_Monster_LifeBar_D.png",
    bossBarFill: "img_Monster_LifeBar_T.png",
    hpBack: "img_RoleLifeBar_D.png",
    hpFill: "img_RoleLifeBar_T.png",
    coinIcon: "img_coinIcon.png",
    power: "img_power.png",
    attackCircle: "img_attack_circle.png",
    arrowUi: "img_jiguanArrow.png",
    fireBig: "T_Seq_Fire_035.png",
    fireSmall: "T_Seq_Fire_01.png",
    arrow: "T_Obj_Arrow_02.png",
    coin: "T_Obj_GoldCoin_01.jpg",
    ring: "T_Ring_01.png",
    shadow: "T_shadow_01.png"
  };

  const assets = {};
  const pointer = {
    down: false,
    active: false,
    x: 260,
    y: 360
  };

  let state = "loading";
  let lastTime = performance.now();
  let shake = 0;
  let time = 0;
  let level = 1;
  let coins = 0;
  let nextUpgradeAt = 8;
  let upgradePulse = 0;
  let resultPulse = 0;
  let chosenCard = -1;

  let player;
  let boss;
  let arrows;
  let fireballs;
  let minions;
  let bursts;
  let rewards;
  let floaters;
  let upgradeCards;

  const clamp = (value, min, max) => Math.max(min, Math.min(max, value));
  const dist = (a, b) => Math.hypot(a.x - b.x, a.y - b.y);
  const rand = (min, max) => min + Math.random() * (max - min);

  function loadAssets() {
    return Promise.all(Object.entries(assetFiles).map(([key, file]) => new Promise((resolve, reject) => {
      const img = new Image();
      img.onload = () => {
        assets[key] = img;
        resolve();
      };
      img.onerror = reject;
      img.src = `./assets/${file}`;
    })));
  }

  function resetGame() {
    state = "playing";
    time = 0;
    level = 1;
    coins = 0;
    nextUpgradeAt = 8;
    chosenCard = -1;
    shake = 0;
    player = {
      x: 235,
      y: 360,
      targetX: 235,
      targetY: 360,
      r: 24,
      hp: 180,
      maxHp: 180,
      damage: 24,
      fireDelay: 0.42,
      fireTimer: 0.2,
      crit: 0.12,
      split: 0,
      invuln: 0
    };
    boss = {
      x: 735,
      y: 245,
      r: 70,
      hp: 780,
      maxHp: 780,
      attackTimer: 1.65,
      waveTimer: 4.4,
      spawnTimer: 5.5,
      flash: 0,
      phase: 1
    };
    arrows = [];
    fireballs = [];
    minions = [];
    bursts = [];
    rewards = [];
    floaters = [];
    upgradeCards = [];
  }

  function clientToWorld(event) {
    const rect = canvas.getBoundingClientRect();
    return {
      x: (event.clientX - rect.left) * W / rect.width,
      y: (event.clientY - rect.top) * H / rect.height
    };
  }

  function setTarget(point) {
    pointer.active = true;
    pointer.x = point.x;
    pointer.y = point.y;
    player.targetX = clamp(point.x, 90, 520);
    player.targetY = clamp(point.y, 165, 470);
  }

  canvas.addEventListener("pointerdown", (event) => {
    canvas.setPointerCapture(event.pointerId);
    const p = clientToWorld(event);
    pointer.down = true;

    if (state === "win" || state === "lose") {
      resetGame();
      return;
    }

    if (state === "upgrade") {
      chooseUpgrade(p);
      return;
    }

    if (state === "playing") {
      setTarget(p);
    }
  });

  canvas.addEventListener("pointermove", (event) => {
    if (!pointer.down || state !== "playing") return;
    setTarget(clientToWorld(event));
  });

  canvas.addEventListener("pointerup", () => {
    pointer.down = false;
  });

  canvas.addEventListener("pointercancel", () => {
    pointer.down = false;
  });

  window.addEventListener("keydown", (event) => {
    if (state === "win" || state === "lose") {
      resetGame();
      return;
    }
    if (state !== "playing") return;
    const step = 42;
    if (event.key === "ArrowLeft" || event.key === "a") player.targetX -= step;
    if (event.key === "ArrowRight" || event.key === "d") player.targetX += step;
    if (event.key === "ArrowUp" || event.key === "w") player.targetY -= step;
    if (event.key === "ArrowDown" || event.key === "s") player.targetY += step;
    if (event.key === "ArrowLeft" || event.key === "ArrowRight" || event.key === "ArrowUp" || event.key === "ArrowDown" || event.key === "a" || event.key === "d" || event.key === "w" || event.key === "s") {
      player.targetX = clamp(player.targetX, 90, 520);
      player.targetY = clamp(player.targetY, 165, 470);
      pointer.active = true;
    }
  });

  function chooseUpgrade(point) {
    const index = upgradeCards.findIndex((card) => (
      point.x >= card.x &&
      point.x <= card.x + card.w &&
      point.y >= card.y &&
      point.y <= card.y + card.h
    ));
    if (index < 0) return;
    const card = upgradeCards[index];
    chosenCard = index;

    if (card.type === "armor") {
      player.maxHp += 45;
      player.hp = clamp(player.hp + 70, 0, player.maxHp);
      addFloater(player.x, player.y - 58, "+HP", "#95ff85");
    }
    if (card.type === "bow") {
      player.damage += 16;
      player.crit += 0.1;
      addFloater(player.x, player.y - 58, "+ATK", "#ffd46c");
    }
    if (card.type === "quick") {
      player.fireDelay = Math.max(0.22, player.fireDelay - 0.1);
      player.split = Math.min(2, player.split + 1);
      addFloater(player.x, player.y - 58, "+SPD", "#9bd9ff");
    }

    level += 1;
    nextUpgradeAt = time + 8;
    setTimeout(() => {
      if (state === "upgrade") state = "playing";
      chosenCard = -1;
    }, 220);
  }

  function openUpgrade() {
    const scale = 0.48;
    const w = 322 * scale;
    const h = 472 * scale;
    const gap = 28;
    const startX = (W - w * 3 - gap * 2) / 2;
    upgradeCards = [
      { type: "armor", img: assets.cardArmor, x: startX, y: 270, w, h },
      { type: "bow", img: assets.cardBow, x: startX + w + gap, y: 270, w, h },
      { type: "quick", img: assets.cardQuick, x: startX + (w + gap) * 2, y: 270, w, h }
    ];
    state = "upgrade";
  }

  function update(dt) {
    time += dt;
    upgradePulse += dt;
    resultPulse += dt;
    shake = Math.max(0, shake - dt * 25);

    updateParticles(dt);

    if (state !== "playing") return;

    player.invuln = Math.max(0, player.invuln - dt);
    boss.flash = Math.max(0, boss.flash - dt * 5);

    updatePlayer(dt);
    updatePlayerShots(dt);
    updateBoss(dt);
    updateMinions(dt);

    if (time >= nextUpgradeAt && level < 4 && boss.hp > boss.maxHp * 0.14) {
      openUpgrade();
    }

    if (boss.hp <= 0) {
      boss.hp = 0;
      state = "win";
      resultPulse = 0;
      for (let i = 0; i < 18; i += 1) spawnReward(boss.x + rand(-30, 30), boss.y + rand(-30, 30));
    }

    if (player.hp <= 0) {
      player.hp = 0;
      state = "lose";
      resultPulse = 0;
    }
  }

  function updatePlayer(dt) {
    const dx = player.targetX - player.x;
    const dy = player.targetY - player.y;
    const d = Math.hypot(dx, dy);
    if (d > 1) {
      const speed = pointer.down ? 530 : 390;
      const step = Math.min(d, speed * dt);
      player.x += dx / d * step;
      player.y += dy / d * step;
    }

    player.fireTimer -= dt;
    if (player.fireTimer <= 0) {
      player.fireTimer = player.fireDelay;
      fireArrow(0);
      if (player.split > 0) {
        fireArrow(-0.16);
        fireArrow(0.16);
      }
    }
  }

  function fireArrow(offset) {
    const angle = Math.atan2(boss.y - player.y, boss.x - player.x) + offset;
    arrows.push({
      x: player.x + Math.cos(angle) * 26,
      y: player.y - 4 + Math.sin(angle) * 26,
      vx: Math.cos(angle) * 760,
      vy: Math.sin(angle) * 760,
      angle,
      damage: player.damage,
      ttl: 1.35,
      crit: Math.random() < player.crit
    });
  }

  function updatePlayerShots(dt) {
    for (let i = arrows.length - 1; i >= 0; i -= 1) {
      const arrow = arrows[i];
      arrow.x += arrow.vx * dt;
      arrow.y += arrow.vy * dt;
      arrow.ttl -= dt;

      if (Math.hypot(arrow.x - boss.x, arrow.y - boss.y) < boss.r * 0.86) {
        const damage = Math.round(arrow.damage * (arrow.crit ? 1.9 : 1));
        boss.hp -= damage;
        boss.flash = 1;
        shake = Math.max(shake, arrow.crit ? 7 : 4);
        coins += arrow.crit ? 3 : 1;
        addFloater(boss.x + rand(-26, 26), boss.y - 70 + rand(-12, 10), arrow.crit ? `${damage}!` : `${damage}`, arrow.crit ? "#fff3a1" : "#ffffff");
        spawnBurst(arrow.x, arrow.y, arrow.crit ? "#ffd75d" : "#ff8a3d", arrow.crit ? 14 : 8);
        if (Math.random() < 0.25) spawnReward(arrow.x, arrow.y);
        arrows.splice(i, 1);
        continue;
      }

      if (arrow.ttl <= 0 || arrow.x > W + 60 || arrow.y < -60 || arrow.y > H + 60) {
        arrows.splice(i, 1);
      }
    }
  }

  function updateBoss(dt) {
    const rage = 1 - boss.hp / boss.maxHp;
    boss.phase = rage > 0.62 ? 3 : rage > 0.34 ? 2 : 1;
    boss.attackTimer -= dt;
    boss.waveTimer -= dt;
    boss.spawnTimer -= dt;

    if (boss.attackTimer <= 0) {
      boss.attackTimer = Math.max(0.62, 1.32 - rage * 0.35);
      fireBossShot(boss.x - 20, boss.y + 22, player.x + rand(-24, 24), player.y + rand(-20, 20), 135 + rage * 62, 18, 12);
    }

    if (boss.waveTimer <= 0) {
      boss.waveTimer = Math.max(2, 3.9 - rage);
      const count = boss.phase + 2;
      for (let i = 0; i < count; i += 1) {
        const angle = -Math.PI + (i + 1) * (Math.PI / (count + 1));
        fireballs.push({
          x: boss.x - 30,
          y: boss.y + 8,
          vx: Math.cos(angle) * (155 + rage * 80),
          vy: Math.sin(angle) * (155 + rage * 80),
          r: 17,
          damage: 9,
          spin: rand(-2.5, 2.5),
          life: 4.5,
          kind: "wave"
        });
      }
    }

    if (boss.spawnTimer <= 0) {
      boss.spawnTimer = Math.max(3, 5.2 - rage * 1.5);
      minions.push({
        x: boss.x - 26,
        y: boss.y + 68,
        r: 22,
        hp: 60,
        speed: 82 + rage * 48,
        wobble: rand(0, Math.PI * 2)
      });
    }
  }

  function fireBossShot(x, y, tx, ty, speed, radius, damage) {
    const angle = Math.atan2(ty - y, tx - x);
    fireballs.push({
      x,
      y,
      vx: Math.cos(angle) * speed,
      vy: Math.sin(angle) * speed,
      r: radius,
      damage,
      spin: rand(-3, 3),
      life: 4.5,
      kind: "direct"
    });
  }

  function updateMinions(dt) {
    for (let i = minions.length - 1; i >= 0; i -= 1) {
      const minion = minions[i];
      minion.wobble += dt * 5;
      const angle = Math.atan2(player.y - minion.y, player.x - minion.x) + Math.sin(minion.wobble) * 0.25;
      minion.x += Math.cos(angle) * minion.speed * dt;
      minion.y += Math.sin(angle) * minion.speed * dt;

      for (let j = arrows.length - 1; j >= 0; j -= 1) {
        const arrow = arrows[j];
        if (Math.hypot(arrow.x - minion.x, arrow.y - minion.y) < minion.r + 9) {
          minion.hp -= arrow.damage;
          arrows.splice(j, 1);
          spawnBurst(arrow.x, arrow.y, "#ffd06a", 7);
          break;
        }
      }

      if (Math.hypot(player.x - minion.x, player.y - minion.y) < player.r + minion.r && player.invuln <= 0) {
        hitPlayer(8);
        minions.splice(i, 1);
        continue;
      }

      if (minion.hp <= 0) {
        coins += 4;
        addFloater(minion.x, minion.y - 26, "+4", "#ffe26a");
        spawnReward(minion.x, minion.y);
        spawnBurst(minion.x, minion.y, "#aef981", 12);
        minions.splice(i, 1);
      }
    }

    for (let i = fireballs.length - 1; i >= 0; i -= 1) {
      const shot = fireballs[i];
      shot.x += shot.vx * dt;
      shot.y += shot.vy * dt;
      shot.life -= dt;
      if (Math.hypot(player.x - shot.x, player.y - shot.y) < player.r + shot.r * 0.72 && player.invuln <= 0) {
        hitPlayer(shot.damage);
        spawnBurst(shot.x, shot.y, "#ff6f2b", 16);
        fireballs.splice(i, 1);
        continue;
      }
      if (shot.life <= 0 || shot.x < -80 || shot.x > W + 80 || shot.y < -80 || shot.y > H + 80) {
        fireballs.splice(i, 1);
      }
    }
  }

  function hitPlayer(damage) {
    player.hp -= damage;
    player.invuln = 0.75;
    shake = Math.max(shake, 9);
    addFloater(player.x, player.y - 42, `-${damage}`, "#ffb29d");
  }

  function spawnBurst(x, y, color, count) {
    for (let i = 0; i < count; i += 1) {
      const angle = rand(0, Math.PI * 2);
      const speed = rand(80, 260);
      bursts.push({
        x,
        y,
        vx: Math.cos(angle) * speed,
        vy: Math.sin(angle) * speed,
        life: rand(0.35, 0.75),
        maxLife: rand(0.45, 0.85),
        size: rand(2, 7),
        color
      });
    }
  }

  function spawnReward(x, y) {
    rewards.push({
      x,
      y,
      vx: rand(-70, 70),
      vy: rand(-150, -80),
      life: 0.95,
      size: rand(18, 26)
    });
  }

  function addFloater(x, y, text, color) {
    floaters.push({ x, y, text, color, life: 0.9, maxLife: 0.9 });
  }

  function updateParticles(dt) {
    for (let i = bursts.length - 1; i >= 0; i -= 1) {
      const p = bursts[i];
      p.life -= dt;
      p.x += p.vx * dt;
      p.y += p.vy * dt;
      p.vx *= 0.94;
      p.vy = p.vy * 0.94 + 120 * dt;
      if (p.life <= 0) bursts.splice(i, 1);
    }

    for (let i = rewards.length - 1; i >= 0; i -= 1) {
      const p = rewards[i];
      p.life -= dt;
      p.x += p.vx * dt;
      p.y += p.vy * dt;
      p.vx *= 0.96;
      p.vy += 240 * dt;
      if (p.life <= 0) rewards.splice(i, 1);
    }

    for (let i = floaters.length - 1; i >= 0; i -= 1) {
      const p = floaters[i];
      p.life -= dt;
      p.y -= 42 * dt;
      if (p.life <= 0) floaters.splice(i, 1);
    }
  }

  function draw() {
    ctx.save();
    const sx = shake > 0 ? rand(-shake, shake) : 0;
    const sy = shake > 0 ? rand(-shake, shake) : 0;
    ctx.translate(sx, sy);

    drawBackground();
    drawArenaEffects();
    drawRewards();
    drawMinions();
    drawBoss();
    drawPlayer();
    drawArrows();
    drawFireballs();
    drawBursts();
    drawHud();
    ctx.restore();

    if (state === "loading") drawLoading();
    if (state === "upgrade") drawUpgrade();
    if (state === "win" || state === "lose") drawResult();
    if (state === "playing" && !pointer.active && time < 5.5) drawFinger();
  }

  function drawBackground() {
    drawCover(assets.bg, 0, 0, W, H);
    ctx.save();
    const grad = ctx.createLinearGradient(0, 0, W, H);
    grad.addColorStop(0, "rgba(43, 83, 82, 0.18)");
    grad.addColorStop(0.42, "rgba(37, 42, 57, 0.02)");
    grad.addColorStop(1, "rgba(92, 35, 23, 0.28)");
    ctx.fillStyle = grad;
    ctx.fillRect(0, 0, W, H);
    ctx.restore();
  }

  function drawArenaEffects() {
    ctx.save();
    ctx.globalAlpha = 0.24 + Math.sin(time * 2.2) * 0.04;
    ctx.translate(boss.x, boss.y);
    ctx.rotate(time * 0.32);
    ctx.drawImage(assets.ring, -108, -108, 216, 216);
    ctx.restore();

    ctx.save();
    ctx.globalAlpha = 0.18;
    ctx.fillStyle = "#ff7730";
    ctx.beginPath();
    ctx.ellipse(760, 305, 160, 86, -0.18, 0, Math.PI * 2);
    ctx.fill();
    ctx.restore();
  }

  function drawPlayer() {
    ctx.save();
    ctx.globalAlpha = 0.34;
    ctx.drawImage(assets.attackCircle, player.x - 56, player.y - 55, 112, 112);
    ctx.restore();

    ctx.save();
    ctx.globalAlpha = 0.34;
    ctx.drawImage(assets.shadow, player.x - 38, player.y + 12, 76, 38);
    ctx.restore();

    if (player.invuln > 0 && Math.floor(time * 18) % 2 === 0) ctx.globalAlpha = 0.55;

    ctx.save();
    ctx.translate(player.x, player.y);
    const bob = Math.sin(time * 8) * 2;
    ctx.translate(0, bob);

    const body = ctx.createLinearGradient(-18, -34, 22, 30);
    body.addColorStop(0, "#fff7d2");
    body.addColorStop(0.45, "#3d9cff");
    body.addColorStop(1, "#23427f");
    ctx.fillStyle = body;
    ctx.beginPath();
    ctx.roundRect(-19, -31, 38, 62, 18);
    ctx.fill();

    ctx.fillStyle = "#f6d0a6";
    ctx.beginPath();
    ctx.arc(0, -42, 16, 0, Math.PI * 2);
    ctx.fill();

    ctx.fillStyle = "#ffe05c";
    ctx.beginPath();
    ctx.moveTo(-20, -54);
    ctx.quadraticCurveTo(0, -74, 22, -51);
    ctx.quadraticCurveTo(8, -57, -20, -54);
    ctx.fill();

    ctx.save();
    ctx.translate(24, -10);
    ctx.rotate(-0.2);
    ctx.strokeStyle = "#7a3a18";
    ctx.lineWidth = 8;
    ctx.beginPath();
    ctx.arc(-2, -4, 38, -1.05, 1.05);
    ctx.stroke();
    ctx.strokeStyle = "rgba(255,255,255,0.82)";
    ctx.lineWidth = 2.5;
    ctx.beginPath();
    ctx.moveTo(17, -36);
    ctx.lineTo(16, 30);
    ctx.stroke();
    ctx.restore();

    ctx.restore();
  }

  function drawBoss() {
    ctx.save();
    ctx.globalAlpha = 0.42;
    ctx.drawImage(assets.shadow, boss.x - 74, boss.y + 52, 148, 52);
    ctx.restore();

    ctx.save();
    const pulse = 1 + Math.sin(time * 4) * 0.025 + boss.flash * 0.07;
    ctx.translate(boss.x, boss.y);
    ctx.scale(pulse, pulse);
    ctx.globalAlpha = 0.62;
    ctx.drawImage(assets.fireSmall, -86, -92, 172, 172);
    ctx.globalAlpha = 1;
    if (boss.flash > 0) {
      ctx.filter = "brightness(1.6)";
    }
    ctx.drawImage(assets.bossIcon, 0, 0, 160, 160, -76, -86, 152, 152);
    ctx.filter = "none";
    ctx.restore();

    ctx.save();
    ctx.globalAlpha = 0.85;
    ctx.fillStyle = "rgba(35, 13, 8, 0.68)";
    ctx.beginPath();
    ctx.roundRect(boss.x - 86, boss.y + 76, 172, 28, 10);
    ctx.fill();
    ctx.fillStyle = "#ffe6b4";
    ctx.font = "bold 18px Arial, Helvetica, sans-serif";
    ctx.textAlign = "center";
    ctx.fillText("FIRE BOSS", boss.x, boss.y + 97);
    ctx.restore();
  }

  function drawArrows() {
    arrows.forEach((arrow) => {
      drawRotated(assets.arrow, arrow.x, arrow.y, 58, 15, arrow.angle);
      ctx.save();
      ctx.globalAlpha = 0.35;
      ctx.strokeStyle = arrow.crit ? "#ffe36b" : "#ffb055";
      ctx.lineWidth = arrow.crit ? 5 : 3;
      ctx.beginPath();
      ctx.moveTo(arrow.x - Math.cos(arrow.angle) * 22, arrow.y - Math.sin(arrow.angle) * 22);
      ctx.lineTo(arrow.x - Math.cos(arrow.angle) * 70, arrow.y - Math.sin(arrow.angle) * 70);
      ctx.stroke();
      ctx.restore();
    });
  }

  function drawFireballs() {
    fireballs.forEach((shot) => {
      drawRotated(assets.fireBig, shot.x, shot.y, shot.r * 2.9, shot.r * 2.9, time * shot.spin);
      ctx.save();
      ctx.globalAlpha = 0.18;
      ctx.fillStyle = "#ff4c1a";
      ctx.beginPath();
      ctx.arc(shot.x, shot.y, shot.r * 1.7, 0, Math.PI * 2);
      ctx.fill();
      ctx.restore();
    });
  }

  function drawMinions() {
    minions.forEach((minion) => {
      ctx.save();
      ctx.globalAlpha = 0.35;
      ctx.drawImage(assets.shadow, minion.x - 24, minion.y + 15, 48, 22);
      ctx.restore();
      ctx.save();
      ctx.translate(minion.x, minion.y);
      ctx.rotate(Math.sin(minion.wobble) * 0.12);
      ctx.drawImage(assets.spiderIcon, 0, 0, 160, 160, -33, -37, 66, 66);
      ctx.restore();
    });
  }

  function drawRewards() {
    rewards.forEach((coin) => {
      ctx.save();
      ctx.globalAlpha = clamp(coin.life / 0.35, 0, 1);
      ctx.translate(coin.x, coin.y);
      ctx.rotate(time * 8);
      ctx.drawImage(assets.coin, -coin.size / 2, -coin.size / 2, coin.size, coin.size);
      ctx.restore();
    });
  }

  function drawBursts() {
    bursts.forEach((p) => {
      const alpha = clamp(p.life / p.maxLife, 0, 1);
      ctx.save();
      ctx.globalAlpha = alpha;
      ctx.fillStyle = p.color;
      ctx.beginPath();
      ctx.arc(p.x, p.y, p.size * (1 + (1 - alpha) * 0.8), 0, Math.PI * 2);
      ctx.fill();
      ctx.restore();
    });

    floaters.forEach((p) => {
      ctx.save();
      ctx.globalAlpha = clamp(p.life / p.maxLife, 0, 1);
      ctx.font = "bold 24px Arial, Helvetica, sans-serif";
      ctx.textAlign = "center";
      ctx.lineWidth = 5;
      ctx.strokeStyle = "rgba(42, 20, 12, 0.8)";
      ctx.strokeText(p.text, p.x, p.y);
      ctx.fillStyle = p.color;
      ctx.fillText(p.text, p.x, p.y);
      ctx.restore();
    });
  }

  function drawHud() {
    ctx.save();
    ctx.drawImage(assets.logo, 28, 18, 104, 71);

    drawHpBar(154, 30, 220, 24, player.hp / player.maxHp, "#35d774", "#0f592a");
    ctx.font = "bold 18px Arial, Helvetica, sans-serif";
    ctx.fillStyle = "#ffffff";
    ctx.textAlign = "left";
    ctx.lineWidth = 4;
    ctx.strokeStyle = "rgba(0,0,0,0.55)";
    const hpText = `${Math.ceil(player.hp)} / ${player.maxHp}`;
    ctx.strokeText(hpText, 165, 50);
    ctx.fillText(hpText, 165, 50);

    ctx.drawImage(assets.coinIcon, 150, 62, 36, 36);
    ctx.font = "bold 22px Arial, Helvetica, sans-serif";
    ctx.strokeText(String(coins), 190, 88);
    ctx.fillText(String(coins), 190, 88);

    drawHpBar(600, 34, 286, 26, boss.hp / boss.maxHp, "#ff4d2f", "#5a1611");
    ctx.font = "bold 18px Arial, Helvetica, sans-serif";
    ctx.textAlign = "center";
    ctx.strokeText(`BOSS  Lv.${boss.phase}`, 743, 54);
    ctx.fillText(`BOSS  Lv.${boss.phase}`, 743, 54);

    ctx.save();
    ctx.globalAlpha = 0.78;
    ctx.drawImage(assets.power, 710, 406, 182, 91);
    ctx.restore();
    ctx.font = "bold 20px Arial, Helvetica, sans-serif";
    ctx.textAlign = "center";
    ctx.strokeText(`Lv.${level}`, 797, 462);
    ctx.fillText(`Lv.${level}`, 797, 462);
    ctx.restore();
  }

  function drawHpBar(x, y, w, h, ratio, fill, back) {
    ratio = clamp(ratio, 0, 1);
    ctx.save();
    ctx.fillStyle = "rgba(21, 15, 13, 0.74)";
    ctx.beginPath();
    ctx.roundRect(x - 5, y - 5, w + 10, h + 10, 13);
    ctx.fill();
    ctx.fillStyle = back;
    ctx.beginPath();
    ctx.roundRect(x, y, w, h, 10);
    ctx.fill();
    const grad = ctx.createLinearGradient(x, y, x + w, y);
    grad.addColorStop(0, fill);
    grad.addColorStop(1, "#fff171");
    ctx.fillStyle = grad;
    ctx.beginPath();
    ctx.roundRect(x, y, w * ratio, h, 10);
    ctx.fill();
    ctx.strokeStyle = "rgba(255,255,255,0.75)";
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.roundRect(x, y, w, h, 10);
    ctx.stroke();
    ctx.restore();
  }

  function drawUpgrade() {
    ctx.save();
    ctx.fillStyle = "rgba(12, 14, 20, 0.72)";
    ctx.fillRect(0, 0, W, H);
    ctx.font = "bold 36px Arial, Helvetica, sans-serif";
    ctx.textAlign = "center";
    ctx.lineWidth = 7;
    ctx.strokeStyle = "rgba(83, 28, 6, 0.85)";
    ctx.fillStyle = "#ffeec8";
    ctx.strokeText("POWER UP", W / 2, 216);
    ctx.fillText("POWER UP", W / 2, 216);

    upgradeCards.forEach((card, index) => {
      const hover = chosenCard === index ? 1 : 0;
      const lift = Math.sin(upgradePulse * 4 + index) * 4 - hover * 16;
      ctx.save();
      ctx.translate(card.x + card.w / 2, card.y + card.h / 2 + lift);
      ctx.scale(1 + hover * 0.06, 1 + hover * 0.06);
      ctx.shadowColor = "rgba(255, 210, 96, 0.55)";
      ctx.shadowBlur = 18 + hover * 16;
      ctx.drawImage(card.img, -card.w / 2, -card.h / 2, card.w, card.h);
      ctx.restore();
    });

    const first = upgradeCards[1] || upgradeCards[0];
    const fx = first.x + first.w * 0.55 + Math.sin(upgradePulse * 5) * 10;
    const fy = first.y + first.h * 0.7 + Math.cos(upgradePulse * 5) * 8;
    ctx.drawImage(assets.finger, fx, fy, 78, 60);
    ctx.restore();
  }

  function drawResult() {
    ctx.save();
    ctx.fillStyle = "rgba(8, 10, 14, 0.76)";
    ctx.fillRect(0, 0, W, H);

    const pulse = 1 + Math.sin(resultPulse * 5) * 0.025;
    ctx.save();
    ctx.translate(W / 2, 190);
    ctx.scale(pulse, pulse);
    if (state === "win") {
      ctx.drawImage(assets.win, -256, -120, 512, 239);
    } else {
      ctx.drawImage(assets.fail, -256, -118, 512, 239);
      ctx.drawImage(assets.fail2, -206, -20, 412, 219);
    }
    ctx.restore();

    ctx.drawImage(assets.coinIcon, W / 2 - 54, 290, 42, 42);
    ctx.font = "bold 34px Arial, Helvetica, sans-serif";
    ctx.textAlign = "left";
    ctx.lineWidth = 7;
    ctx.strokeStyle = "rgba(0,0,0,0.65)";
    ctx.fillStyle = "#fff0ac";
    ctx.strokeText(`${coins}`, W / 2 - 4, 324);
    ctx.fillText(`${coins}`, W / 2 - 4, 324);

    ctx.fillStyle = state === "win" ? "#f6aa32" : "#3e90ff";
    ctx.beginPath();
    ctx.roundRect(W / 2 - 106, 374, 212, 58, 18);
    ctx.fill();
    ctx.strokeStyle = "rgba(255,255,255,0.8)";
    ctx.lineWidth = 3;
    ctx.beginPath();
    ctx.roundRect(W / 2 - 106, 374, 212, 58, 18);
    ctx.stroke();
    ctx.font = "bold 27px Arial, Helvetica, sans-serif";
    ctx.textAlign = "center";
    ctx.fillStyle = "#ffffff";
    ctx.strokeStyle = "rgba(74, 35, 13, 0.75)";
    ctx.lineWidth = 4;
    ctx.strokeText("PLAY AGAIN", W / 2, 412);
    ctx.fillText("PLAY AGAIN", W / 2, 412);
    ctx.restore();
  }

  function drawLoading() {
    ctx.save();
    ctx.fillStyle = "#171a20";
    ctx.fillRect(0, 0, W, H);
    ctx.drawImage(assets.logo || new Image(), 352, 158, 256, 174);
    ctx.font = "bold 28px Arial, Helvetica, sans-serif";
    ctx.textAlign = "center";
    ctx.fillStyle = "#fff3ca";
    ctx.fillText("LOADING", W / 2, 390);
    ctx.restore();
  }

  function drawFinger() {
    ctx.save();
    const x = 205 + Math.sin(time * 4.4) * 54;
    const y = 398 + Math.cos(time * 4.4) * 18;
    ctx.globalAlpha = clamp(1 - Math.max(0, time - 4.2) / 1.3, 0, 1);
    ctx.drawImage(assets.finger, x, y, 86, 66);
    ctx.restore();
  }

  function drawCover(img, x, y, w, h) {
    const imgRatio = img.width / img.height;
    const boxRatio = w / h;
    let sx = 0;
    let sy = 0;
    let sw = img.width;
    let sh = img.height;
    if (imgRatio > boxRatio) {
      sw = img.height * boxRatio;
      sx = (img.width - sw) / 2;
    } else {
      sh = img.width / boxRatio;
      sy = (img.height - sh) / 2;
    }
    ctx.drawImage(img, sx, sy, sw, sh, x, y, w, h);
  }

  function drawRotated(img, x, y, w, h, angle) {
    ctx.save();
    ctx.translate(x, y);
    ctx.rotate(angle);
    ctx.drawImage(img, -w / 2, -h / 2, w, h);
    ctx.restore();
  }

  function loop(now) {
    const dt = clamp((now - lastTime) / 1000, 0, 0.033);
    lastTime = now;
    update(dt);
    draw();
    requestAnimationFrame(loop);
  }

  CanvasRenderingContext2D.prototype.roundRect = CanvasRenderingContext2D.prototype.roundRect || function roundRect(x, y, w, h, r) {
    const radius = Math.min(r, w / 2, h / 2);
    this.moveTo(x + radius, y);
    this.arcTo(x + w, y, x + w, y + h, radius);
    this.arcTo(x + w, y + h, x, y + h, radius);
    this.arcTo(x, y + h, x, y, radius);
    this.arcTo(x, y, x + w, y, radius);
    return this;
  };

  loadAssets().then(() => {
    resetGame();
    lastTime = performance.now();
    requestAnimationFrame(loop);
  }).catch((error) => {
    state = "loading";
    ctx.fillStyle = "#15171d";
    ctx.fillRect(0, 0, W, H);
    ctx.fillStyle = "#fff";
    ctx.font = "20px Arial, Helvetica, sans-serif";
    ctx.fillText(`Asset load failed: ${error.message || error}`, 32, 56);
  });
})();
