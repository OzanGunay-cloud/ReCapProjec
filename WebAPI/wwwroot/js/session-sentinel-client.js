(function (global) {
    const storageKey = "recap.sessionSentinel.fingerprint";

    function hashString(value) {
        let hash = 2166136261;
        for (let index = 0; index < value.length; index += 1) {
            hash ^= value.charCodeAt(index);
            hash +=
                (hash << 1) +
                (hash << 4) +
                (hash << 7) +
                (hash << 8) +
                (hash << 24);
        }

        return (hash >>> 0).toString(16).padStart(8, "0");
    }

    function buildRawFingerprint() {
        const parts = [
            navigator.userAgent || "",
            navigator.language || "",
            screen.width || 0,
            screen.height || 0,
            Intl.DateTimeFormat().resolvedOptions().timeZone || "",
            navigator.platform || "",
            navigator.hardwareConcurrency || 0
        ];

        return parts.join("|");
    }

    function getOrCreateFingerprint() {
        const existing = localStorage.getItem(storageKey);
        if (existing) {
            return existing;
        }

        const fingerprint = `web-${hashString(buildRawFingerprint())}`;
        localStorage.setItem(storageKey, fingerprint);
        return fingerprint;
    }

    async function fetchWithSessionSentinel(input, init) {
        const requestInit = init || {};
        const headers = new Headers(requestInit.headers || {});
        headers.set("X-Sentinel-Fingerprint", getOrCreateFingerprint());

        return fetch(input, {
            ...requestInit,
            headers
        });
    }

    global.sessionSentinelClient = {
        getOrCreateFingerprint,
        fetchWithSessionSentinel
    };
})(window);
