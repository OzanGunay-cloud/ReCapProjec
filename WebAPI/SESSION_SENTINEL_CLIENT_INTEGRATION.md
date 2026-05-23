# ReCap Frontend Fingerprint Entegrasyonu

Bu repo şu an ayrı bir frontend projesi içermiyor. Bu yüzden fingerprint entegrasyonu için tarayıcı tarafı yardımcı kod `WebAPI/wwwroot/js/session-sentinel-client.js` içine eklendi.

## Ne yapar
- Tarayıcı özelliklerinden stabil bir fingerprint üretir.
- Fingerprint değerini `localStorage` içinde tutar.
- Her isteğe `X-Sentinel-Fingerprint` header'ını ekleyen `fetchWithSessionSentinel(...)` yardımcı fonksiyonunu sağlar.

## API kullanımı
```html
<script src="/js/session-sentinel-client.js"></script>
<script>
  const fingerprint = window.sessionSentinelClient.getOrCreateFingerprint();

  const response = await window.sessionSentinelClient.fetchWithSessionSentinel("/api/auth/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      email: "user@example.com",
      password: "12345"
    })
  });
</script>
```

## Login sonrası
Aynı browser oturumu şu pattern ile devam etmeli:
- `Authorization: Bearer <token>`
- `X-Sentinel-Fingerprint: <aynı fingerprint>`

## Ayrı frontend projesi geldiğinde
Aynı mantığı React, Angular veya başka client projesine şu şekilde taşıyacağız:
- login isteğinde fingerprint ekle
- protected API isteklerinde fingerprint ekle
- logout isteğinde fingerprint ekle
- istekleri tek tek değil ortak `http client` katmanında merkezileştir

## Demo sayfası
`/session-sentinel-demo.html` artık bu helper'ı kullanır ve manuel fingerprint girişi istemez.
