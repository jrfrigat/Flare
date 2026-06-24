/**
 * Flare Popover Collision Engine
 * 
 * Provides flip/shift/auto-placement for popovers, tooltips, and menus.
 * Similar to Floating UI but lightweight and integrated into Flare.
 */

/**
 * Calculate the best placement for a floating element to avoid viewport overflow.
 * @param {HTMLElement} anchor - The anchor element
 * @param {HTMLElement} floating - The floating element
 * @param {string} preferredPlacement - Preferred placement (top, bottom, left, right)
 * @param {object} options - Configuration options
 * @returns {object} - { placement, top, left, arrowTop, arrowLeft }
 */
export function calculatePlacement(anchor, floating, preferredPlacement, options = {}) {
    const {
        offset = 8,
        flip = true,
        shift = true,
        arrowSize = 0,
        boundaryPadding = 0
    } = options;

    const anchorRect = anchor.getBoundingClientRect();
    const floatingRect = floating.getBoundingClientRect();
    const viewport = {
        width: window.innerWidth,
        height: window.innerHeight
    };

    // Available space in each direction
    const space = {
        top: anchorRect.top - boundaryPadding,
        bottom: viewport.height - anchorRect.bottom - boundaryPadding,
        left: anchorRect.left - boundaryPadding,
        right: viewport.width - anchorRect.right - boundaryPadding
    };

    // Floating element dimensions
    const floatingWidth = floatingRect.width;
    const floatingHeight = floatingRect.height;

    // Determine if flip is needed
    let placement = preferredPlacement;
    let needsFlip = false;

    if (flip) {
        switch (preferredPlacement) {
            case 'top':
                if (space.top < floatingHeight + offset) {
                    placement = 'bottom';
                    needsFlip = true;
                }
                break;
            case 'bottom':
                if (space.bottom < floatingHeight + offset) {
                    placement = 'top';
                    needsFlip = true;
                }
                break;
            case 'left':
                if (space.left < floatingWidth + offset) {
                    placement = 'right';
                    needsFlip = true;
                }
                break;
            case 'right':
                if (space.right < floatingWidth + offset) {
                    placement = 'left';
                    needsFlip = true;
                }
                break;
        }
    }

    // Calculate position based on placement
    let top, left;

    switch (placement) {
        case 'top':
            top = anchorRect.top - floatingHeight - offset;
            left = anchorRect.left + (anchorRect.width - floatingWidth) / 2;
            break;
        case 'bottom':
            top = anchorRect.bottom + offset;
            left = anchorRect.left + (anchorRect.width - floatingWidth) / 2;
            break;
        case 'left':
            top = anchorRect.top + (anchorRect.height - floatingHeight) / 2;
            left = anchorRect.left - floatingWidth - offset;
            break;
        case 'right':
            top = anchorRect.top + (anchorRect.height - floatingHeight) / 2;
            left = anchorRect.right + offset;
            break;
    }

    // Apply shift to keep within viewport
    if (shift) {
        const minX = boundaryPadding;
        const maxX = viewport.width - floatingWidth - boundaryPadding;
        const minY = boundaryPadding;
        const maxY = viewport.height - floatingHeight - boundaryPadding;

        if (left < minX) left = minX;
        if (left > maxX) left = maxX;
        if (top < minY) top = minY;
        if (top > maxY) top = maxY;
    }

    // Calculate arrow position relative to floating element
    let arrowTop = 0;
    let arrowLeft = 0;

    if (arrowSize > 0) {
        switch (placement) {
            case 'top':
            case 'bottom':
                arrowTop = placement === 'top' ? floatingHeight - arrowSize / 2 : -arrowSize / 2;
                arrowLeft = anchorRect.left + anchorRect.width / 2 - left - arrowSize / 2;
                arrowLeft = Math.max(0, Math.min(floatingWidth - arrowSize, arrowLeft));
                break;
            case 'left':
            case 'right':
                arrowTop = anchorRect.top + anchorRect.height / 2 - top - arrowSize / 2;
                arrowTop = Math.max(0, Math.min(floatingHeight - arrowSize, arrowTop));
                arrowLeft = placement === 'left' ? floatingWidth - arrowSize / 2 : -arrowSize / 2;
                break;
        }
    }

    return {
        placement,
        top,
        left,
        arrowTop,
        arrowLeft,
        needsFlip
    };
}

/**
 * Setup collision detection for a popover.
 * @param {HTMLElement} anchor - The anchor element
 * @param {HTMLElement} floating - The floating element
 * @param {string} preferredPlacement - Preferred placement
 * @param {object} options - Configuration options
 * @returns {object} - { update, destroy }
 */
export function setupCollision(anchor, floating, preferredPlacement, options = {}) {
    let currentPlacement = preferredPlacement;

    function update() {
        const result = calculatePlacement(anchor, floating, currentPlacement, options);
        
        floating.style.position = 'fixed';
        floating.style.top = `${result.top}px`;
        floating.style.left = `${result.left}px`;

        if (options.arrowSize > 0) {
            const arrow = floating.querySelector('[data-flare-arrow]');
            if (arrow) {
                arrow.style.top = `${result.arrowTop}px`;
                arrow.style.left = `${result.arrowLeft}px`;
            }
        }

        return result;
    }

    function destroy() {
        floating.style.position = '';
        floating.style.top = '';
        floating.style.left = '';
    }

    // Initial positioning
    update();

    // Re-position on scroll/resize
    const handleReposition = () => update();
    window.addEventListener('scroll', handleReposition, { passive: true });
    window.addEventListener('resize', handleReposition, { passive: true });

    return {
        update,
        destroy,
        setPlacement: (newPlacement) => {
            currentPlacement = newPlacement;
            update();
        }
    };
}
