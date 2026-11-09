document.addEventListener('DOMContentLoaded', () => {
    if (!document.querySelector('[data-celebrate="true"]') || window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;
    const layer = document.createElement('div');
    layer.className = 'confetti-layer';
    layer.setAttribute('aria-hidden', 'true');
    const colors = ['#f6df35', '#fff4a6', '#ffffff', '#8d903d'];
    for (let i = 0; i < 100; i++) {
        const piece = document.createElement('i');
        piece.className = 'confetti';
        piece.style.left = `${Math.random() * 100}%`;
        piece.style.background = colors[i % colors.length];
        piece.style.setProperty('--duration', `${2.5 + Math.random() * 2}s`);
        piece.style.setProperty('--delay', `${Math.random() * 1.2}s`);
        piece.style.setProperty('--drift', `${Math.random() * 240 - 120}px`);
        layer.appendChild(piece);
    }
    document.body.appendChild(layer);
    window.setTimeout(() => layer.remove(), 6500);
});
