const players = new Map();

export function init(playerId, videoEl, dotNetRef) {
    function onTimeUpdate() {
        dotNetRef.invokeMethodAsync('OnTimeUpdate', videoEl.currentTime, videoEl.duration || 0);
    }
    function onEnded() { dotNetRef.invokeMethodAsync('OnEnded'); }
    function onPlay() { dotNetRef.invokeMethodAsync('OnPlayStateChanged', true); }
    function onPause() { dotNetRef.invokeMethodAsync('OnPlayStateChanged', false); }

    videoEl.addEventListener('timeupdate', onTimeUpdate);
    videoEl.addEventListener('ended', onEnded);
    videoEl.addEventListener('play', onPlay);
    videoEl.addEventListener('pause', onPause);

    players.set(playerId, { videoEl, listeners: { onTimeUpdate, onEnded, onPlay, onPause } });
}

export function play(playerId) {
    players.get(playerId)?.videoEl.play();
}

export function pause(playerId) {
    players.get(playerId)?.videoEl.pause();
}

export function seek(playerId, time) {
    const p = players.get(playerId);
    if (p) p.videoEl.currentTime = time;
}

export function setVolume(playerId, volume) {
    const p = players.get(playerId);
    if (p) p.videoEl.volume = volume;
}

export function toggleMute(playerId) {
    const p = players.get(playerId);
    if (p) p.videoEl.muted = !p.videoEl.muted;
}

export function requestFullscreen(playerId) {
    players.get(playerId)?.videoEl.requestFullscreen?.();
}

export function setPlaybackRate(playerId, rate) {
    const p = players.get(playerId);
    if (p) p.videoEl.playbackRate = rate;
}

export function destroy(playerId) {
    const p = players.get(playerId);
    if (p) {
        const { videoEl, listeners } = p;
        videoEl.removeEventListener('timeupdate', listeners.onTimeUpdate);
        videoEl.removeEventListener('ended', listeners.onEnded);
        videoEl.removeEventListener('play', listeners.onPlay);
        videoEl.removeEventListener('pause', listeners.onPause);
    }
    players.delete(playerId);
}
