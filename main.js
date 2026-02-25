/* ═══════════════════════════════════════════════════════════
   TOWER — Animated Jenga-like hero decoration
   17 blocks with randomized shapes, colors and CSS animations.
   Collapses on hover, then auto-rebuilds.
   ═══════════════════════════════════════════════════════════ */
(function () {
  const wrap  = document.getElementById('towerWrap');
  const tower = document.getElementById('tower');
  if (!tower) return;

  const BLOCKS = [
    {l:'AI',c:'c-ai'},{l:'Cloud',c:'c-cloud'},{l:'Big Data',c:'c-data'},
    {l:'Sec',c:'c-sec'},{l:'GPU',c:'c-hw'},{l:'Algo',c:'c-algo'},
    {l:'Network',c:'c-net'},{l:'App',c:'c-app'},{l:'DNS',c:'c-dns'},
    {l:'ML',c:'c-ai'},{l:'API',c:'c-app'},{l:'DB',c:'c-data'},
    {l:'IoT',c:'c-net'},{l:'AWS',c:'c-cloud'},{l:'Neural',c:'c-hw'},
    {l:'VR',c:'c-sec'},{l:'Linux',c:'c-dns'},
  ];
  const SIZES = [
    {w:140,h:22},{w:90,h:28},{w:120,h:20},{w:70,h:32},{w:150,h:18},
    {w:100,h:26},{w:130,h:24},{w:80,h:30},{w:110,h:22},{w:95,h:28},
    {w:145,h:20},{w:75,h:34},{w:125,h:18},{w:105,h:26},{w:135,h:22},
    {w:85,h:30},{w:115,h:24},
  ];
  const SHAPES = ['diamond','circle','hexagon','triangle','parallelogram','roundrect','pentagon','tag'];

  /* Apply clip-path/border-radius for a given shape type */
  function applyShape(el, type, w, h) {
    const s = el.style;
    s.clipPath = 'none'; s.borderRadius = '3px'; s.width = w+'px'; s.height = h+'px';
    if      (type==='diamond')      { s.width='42px';s.height='42px';s.clipPath='polygon(50% 0%,100% 50%,50% 100%,0% 50%)';s.borderRadius='0'; }
    else if (type==='circle')       { s.width='34px';s.height='34px';s.clipPath='circle(50% at 50% 50%)';s.borderRadius='0'; }
    else if (type==='hexagon')      { s.width='46px';s.height='40px';s.clipPath='polygon(25% 0%,75% 0%,100% 50%,75% 100%,25% 100%,0% 50%)';s.borderRadius='0'; }
    else if (type==='triangle')     { s.width='48px';s.height='40px';s.clipPath='polygon(50% 0%,100% 100%,0% 100%)';s.borderRadius='0'; }
    else if (type==='parallelogram'){ s.clipPath='polygon(15% 0%,100% 0%,85% 100%,0% 100%)';s.borderRadius='0'; }
    else if (type==='roundrect')    { s.clipPath='none';s.borderRadius='999px'; }
    else if (type==='pentagon')     { s.width='44px';s.height='42px';s.clipPath='polygon(50% 0%,100% 38%,82% 100%,18% 100%,0% 38%)';s.borderRadius='0'; }
    else if (type==='tag')          { s.clipPath='polygon(0% 0%,90% 0%,100% 50%,90% 100%,0% 100%)';s.borderRadius='0'; }
  }

  /* Assign special shapes to ~35% of rows, avoiding consecutive repeats */
  function randomShapeList(n) {
    const pool = Array(n).fill('rect');
    const special = new Set();
    while (special.size < Math.round(n * 0.35)) special.add(Math.floor(Math.random() * n));
    let last = '';
    special.forEach(i => {
      let s; do { s = SHAPES[Math.floor(Math.random() * SHAPES.length)]; } while (s === last);
      pool[i] = s; last = s;
    });
    return pool;
  }

  const ROWS = 17;
  let collapsed = false;

  function build() {
    tower.innerHTML = '';
    wrap.classList.remove('collapsing', 'rebuilding');
    wrap.classList.add('swaying');
    const shapes = randomShapeList(ROWS);
    for (let r = 0; r < ROWS; r++) {
      const b = BLOCKS[r % BLOCKS.length], sz = SIZES[r % SIZES.length];
      const el = document.createElement('div');
      el.className = `t-block ${b.c}${r >= ROWS-4 ? ' top-blk' : ''}`;
      applyShape(el, shapes[r], sz.w, sz.h);
      el.textContent = b.l;
      /* Per-block CSS vars drive randomised animation behaviour */
      el.style.setProperty('--tilt',    ((Math.random()-.5)*3).toFixed(1)+'deg');
      el.style.setProperty('--dur',     (2+Math.random()*2).toFixed(1)+'s');
      el.style.setProperty('--dly',     (Math.random()*1.8).toFixed(2)+'s');
      el.style.setProperty('--sx',      (Math.random()>.5?1:-1)*(0.5+Math.random()*1.5).toFixed(1)+'px');
      el.style.setProperty('--fx',      ((Math.random()-.5)*120).toFixed(0)+'px');
      el.style.setProperty('--fy',      (40+Math.random()*160).toFixed(0)+'px');
      el.style.setProperty('--fr',      ((Math.random()-.5)*120).toFixed(0)+'deg');
      el.style.setProperty('--fly-dur', (.25+Math.random()*.3).toFixed(2)+'s');
      el.style.setProperty('--fly-dly', ((ROWS-1-r)*.05).toFixed(2)+'s'); /* top falls first */
      tower.appendChild(el);
    }
  }

  function collapse() {
    if (collapsed) return;
    collapsed = true;
    wrap.classList.replace('swaying', 'collapsing');
    setTimeout(() => {
      wrap.classList.replace('collapsing', 'rebuilding');
      build();
      setTimeout(() => { wrap.classList.replace('rebuilding', 'swaying'); collapsed = false; }, ROWS*20+500);
    }, 2800);
  }

  wrap.addEventListener('mouseenter', collapse);
  wrap.addEventListener('touchstart', collapse, { passive:true });
  build();
})();

/* ═══════════════════════════════════════════════════════════
   AUDIO SPOILERS — toggle open/close; pause audio on close
   ═══════════════════════════════════════════════════════════ */
function toggleSpoiler(id) {
  const el = document.getElementById(id);
  if (!el) return;
  const wasOpen = el.classList.contains('open');
  el.classList.toggle('open');
  if (wasOpen) el.querySelectorAll('audio').forEach(a => a.pause());
}

/* ═══════════════════════════════════════════════════════════
   LIGHTBOX — open, zoom in/out, drag-to-pan, pinch, close
   ═══════════════════════════════════════════════════════════ */
(function () {
  const lb       = document.getElementById('lightbox');
  const img      = document.getElementById('lightbox-img');
  const label    = document.getElementById('lightbox-label');
  const viewport = document.getElementById('lightbox-viewport');
  const lvlEl    = document.getElementById('lb-zoom-level');

  const STEP = 0.25, MIN = 0.5, MAX = 5;
  let scale = 1, tx = 0, ty = 0;

  /* Apply transform to image and refresh % readout */
  function draw(animate) {
    img.style.transition = animate ? 'transform .2s ease' : 'none';
    img.style.transform  = `scale(${scale}) translate(${tx}px,${ty}px)`;
    lvlEl.textContent    = Math.round(scale*100)+'%';
    img.classList.toggle('zoomed', scale > 1);
  }

  function reset() { scale=1; tx=0; ty=0; draw(true); }

  /* Zoom toward a screen-space pivot (defaults to viewport center) */
  function zoomTo(next, px, py) {
    const r  = viewport.getBoundingClientRect();
    const cx = px ?? r.left + r.width/2;
    const cy = py ?? r.top  + r.height/2;
    const imgCX = r.left + r.width/2  + tx*scale;
    const imgCY = r.top  + r.height/2 + ty*scale;
    const ratio = next / scale;
    scale = Math.min(MAX, Math.max(MIN, next));
    tx -= (cx - imgCX) * (ratio-1) / scale;
    ty -= (cy - imgCY) * (ratio-1) / scale;
    draw(true);
  }

  /* Zoom buttons and % label (click to reset) */
  document.getElementById('lb-zoom-in') .addEventListener('click', () => zoomTo(scale+STEP));
  document.getElementById('lb-zoom-out').addEventListener('click', () => zoomTo(scale-STEP));
  lvlEl.addEventListener('click', reset);

  /* Click image: step-zoom in, or reset if already zoomed */
  let dragging = false;
  img.addEventListener('click', e => {
    if (dragging) return;
    scale < 1.5 ? zoomTo(scale+STEP*2, e.clientX, e.clientY) : reset();
  });

  /* Scroll-wheel zoom toward cursor position */
  viewport.addEventListener('wheel', e => {
    e.preventDefault();
    zoomTo(scale + (e.deltaY > 0 ? -STEP : STEP), e.clientX, e.clientY);
  }, { passive:false });

  /* Mouse drag-to-pan when zoomed in */
  let ox=0, oy=0, otx=0, oty=0;
  img.addEventListener('mousedown', e => {
    if (scale <= 1) return;
    dragging=false; ox=e.clientX; oy=e.clientY; otx=tx; oty=ty;
    img.classList.add('dragging');
    const move = ev => {
      const dx=(ev.clientX-ox)/scale, dy=(ev.clientY-oy)/scale;
      if (Math.abs(dx)>3||Math.abs(dy)>3) dragging=true;
      tx=otx+dx; ty=oty+dy; draw(false);
    };
    const up = () => {
      img.classList.remove('dragging');
      document.removeEventListener('mousemove', move);
      document.removeEventListener('mouseup', up);
      setTimeout(()=>{ dragging=false; }, 0);
    };
    document.addEventListener('mousemove', move);
    document.addEventListener('mouseup', up);
  });

  /* Pinch-to-zoom on touch devices */
  let lastDist = null;
  viewport.addEventListener('touchstart', e => {
    if (e.touches.length===2)
      lastDist = Math.hypot(e.touches[0].clientX-e.touches[1].clientX, e.touches[0].clientY-e.touches[1].clientY);
  }, { passive:true });
  viewport.addEventListener('touchmove', e => {
    if (e.touches.length!==2||!lastDist) return;
    e.preventDefault();
    const d   = Math.hypot(e.touches[0].clientX-e.touches[1].clientX, e.touches[0].clientY-e.touches[1].clientY);
    const mid = [(e.touches[0].clientX+e.touches[1].clientX)/2, (e.touches[0].clientY+e.touches[1].clientY)/2];
    zoomTo(scale*(d/lastDist), mid[0], mid[1]);
    lastDist = d;
  }, { passive:false });
  viewport.addEventListener('touchend', () => { lastDist=null; });

  /* Open lightbox from any .doc-card click */
  document.querySelectorAll('.doc-card').forEach(card => {
    card.addEventListener('click', () => {
      const i = card.querySelector('.doc-img');
      const l = card.querySelector('.doc-label');
      img.src = i.src; img.alt = i.alt;
      label.textContent = l ? l.textContent : i.alt;
      lb.classList.add('active');
      document.body.style.overflow = 'hidden';
      reset();
    });
  });

  /* Close: button, backdrop click, or keyboard shortcuts */
  window.closeLightbox = () => {
    lb.classList.remove('active');
    document.body.style.overflow = '';
    img.src = '';
    reset();
  };
  lb.addEventListener('click', e => { if (e.target===lb||e.target===viewport) closeLightbox(); });
  document.addEventListener('keydown', e => {
    if (!lb.classList.contains('active')) return;
    if (e.key==='Escape')         closeLightbox();
    if (e.key==='+'||e.key==='=') zoomTo(scale+STEP);
    if (e.key==='-')              zoomTo(scale-STEP);
    if (e.key==='0')              reset();
  });
})();

/* ═══════════════════════════════════════════════════════════
   SCENE CAROUSEL LIGHTBOX
   Click a scene card → opens lightbox with prev/next arrows
   ═══════════════════════════════════════════════════════════ */
(function () {
  const lb      = document.getElementById('carousel-lightbox');
  const img     = document.getElementById('clb-img');
  const label   = document.getElementById('clb-label');
  const counter = document.getElementById('clb-counter');
  const dots    = document.getElementById('clb-dots');
  const btnPrev = document.getElementById('clb-prev');
  const btnNext = document.getElementById('clb-next');

  let images = [], index = 0;

  function renderDots() {
    dots.innerHTML = '';
    images.forEach((_, i) => {
      const d = document.createElement('span');
      d.className = 'clb-dot' + (i === index ? ' active' : '');
      d.addEventListener('click', () => goTo(i));
      dots.appendChild(d);
    });
  }

  function goTo(i) {
    index = (i + images.length) % images.length;
    img.style.opacity = '0';
    setTimeout(() => {
      img.src = images[index];
      img.style.opacity = '1';
    }, 150);
    counter.textContent = (index + 1) + ' / ' + images.length;
    renderDots();
    btnPrev.style.display = images.length > 1 ? '' : 'none';
    btnNext.style.display = images.length > 1 ? '' : 'none';
  }

  function open(imgs, lbl) {
    images = imgs; index = 0;
    label.textContent = lbl;
    img.style.opacity = '1';
    img.src = images[0];
    counter.textContent = '1 / ' + images.length;
    renderDots();
    btnPrev.style.display = images.length > 1 ? '' : 'none';
    btnNext.style.display = images.length > 1 ? '' : 'none';
    lb.classList.add('active');
    document.body.style.overflow = 'hidden';
  }

  window.closeCarousel = () => {
    lb.classList.remove('active');
    document.body.style.overflow = '';
    img.src = '';
  };

  btnPrev.addEventListener('click', () => goTo(index - 1));
  btnNext.addEventListener('click', () => goTo(index + 1));

  lb.addEventListener('click', e => { if (e.target === lb) closeCarousel(); });

  document.addEventListener('keydown', e => {
    if (!lb.classList.contains('active')) return;
    if (e.key === 'Escape')      closeCarousel();
    if (e.key === 'ArrowLeft')   goTo(index - 1);
    if (e.key === 'ArrowRight')  goTo(index + 1);
  });

  /* Swipe support for mobile */
  let touchX = null;
  lb.addEventListener('touchstart', e => { touchX = e.touches[0].clientX; }, { passive: true });
  lb.addEventListener('touchend',   e => {
    if (touchX === null) return;
    const dx = e.changedTouches[0].clientX - touchX;
    if (Math.abs(dx) > 40) dx < 0 ? goTo(index + 1) : goTo(index - 1);
    touchX = null;
  });

  /* Attach click to each scene card */
  document.querySelectorAll('.scene-card').forEach(card => {
    card.addEventListener('click', () => {
      const imgs  = JSON.parse(card.dataset.images);
      const lbl   = card.dataset.label;
      open(imgs, lbl);
    });
  });
})();
