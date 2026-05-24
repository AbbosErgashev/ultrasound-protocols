function applyTheme(theme) {
    const resolvedTheme = theme === 'dark' ? 'dark' : 'light';
    document.documentElement.setAttribute('data-theme', resolvedTheme);

    document.querySelectorAll('[data-theme-toggle]').forEach(toggle => {
        const isDark = resolvedTheme === 'dark';
        const icon = isDark ? '\u263e' : '\u2600';
        toggle.setAttribute('aria-label', isDark ? 'Kun rejimini yoqish' : 'Tun rejimini yoqish');
        toggle.setAttribute('title', isDark ? 'Kun rejimini yoqish' : 'Tun rejimini yoqish');

        const iconSlot = toggle.querySelector('[data-theme-toggle-icon]');
        if (iconSlot) {
            iconSlot.textContent = icon;
        }
    });
}

function getStoredTheme() {
    try {
        return localStorage.getItem('meduzi-theme') === 'dark' ? 'dark' : 'light';
    } catch (_) {
        return 'light';
    }
}

function setStoredTheme(theme) {
    try {
        localStorage.setItem('meduzi-theme', theme);
    } catch (_) {
        // Theme still applies for this page even when storage is unavailable.
    }
}

applyTheme(getStoredTheme());

document.addEventListener('DOMContentLoaded', function () {

    // ===== THEME TOGGLE =====
    applyTheme(getStoredTheme());

    document.querySelectorAll('[data-theme-toggle]').forEach(toggle => {
        toggle.addEventListener('click', () => {
            const nextTheme = document.documentElement.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
            setStoredTheme(nextTheme);
            applyTheme(nextTheme);
        });
    });

    // ===== SCROLL REVEAL =====
    document.body.classList.add('js-ready');

    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach(e => {
            if (e.isIntersecting) {
                e.target.classList.add('visible');
                revealObserver.unobserve(e.target);
            }
        });
    }, { threshold: 0.08, rootMargin: '0px 0px -30px 0px' });

    document.querySelectorAll('.reveal, .reveal-left, .reveal-right').forEach(el => {
        revealObserver.observe(el);
    });

    // ===== NAVBAR SCROLL =====
    const nav = document.getElementById('mainNav');
    if (nav) {
        window.addEventListener('scroll', () => {
            nav.classList.toggle('scrolled', window.scrollY > 50);
        }, { passive: true });
    }

    // ===== SMOOTH SCROLL =====
    document.querySelectorAll('a[href^="#"]').forEach(a => {
        a.addEventListener('click', (e) => {
            const href = a.getAttribute('href');
            if (href.length > 1) {
                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                }
            }
        });
    });

});

// Re-run reveal after Blazor navigation (SPA mode)
document.addEventListener('enhancedload', function () {
    document.body.classList.add('js-ready');
    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach(e => {
            if (e.isIntersecting) {
                e.target.classList.add('visible');
                revealObserver.unobserve(e.target);
            }
        });
    }, { threshold: 0.08, rootMargin: '0px 0px -30px 0px' });

    document.querySelectorAll('.reveal:not(.visible), .reveal-left:not(.visible), .reveal-right:not(.visible)').forEach(el => {
        revealObserver.observe(el);
    });
});
