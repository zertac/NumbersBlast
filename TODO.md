# Numbers Blast - TODO List

## KRITIK - PDF Gereksinimleri
- [x] Game State Machine (Idle -> Dragging -> Processing -> Tutorial -> GameOver)
- [x] Animasyon sirasinda input engelleme (soft-lock riski)
- [x] Debug log'lari temizle (merge, drag, tutorial)
- [ ] README (mimari, kararlar, bilinen sorunlar, gelecek iyilestirmeler) - PDF ZORUNLU

## Part 1 - Tamamlanacak
- [ ] Ana menu + scene flow (Boot -> Menu -> Gameplay)
- [ ] Base UI siniflari (BaseWindow, BasePopup)
- [ ] Menu ve popup'lari base class'lardan turet
- [ ] Audio sistemi + SFX entegrasyonu
- [ ] Haptic feedback (mobil)

## Polish
- [ ] UI juice (buton animasyonlari, gecis efektleri)
- [ ] Feedback tweaking
- [ ] Ana menu tasarimi (gorsel)

## Kod Kalitesi
- [ ] Kod sadelestirme / refactor
- [ ] Summary ve XML comment'ler
- [ ] Kullanilmayan kod temizligi
- [ ] Namespace kontrolu
- [ ] "No unnecessary classes" audit (PDF gereksinimi)

## Dokumantasyon
- [ ] Teknik dokuman (sistemler arasi iliskiler, data flow)
- [ ] Kisisel calisma dokumani (her kodun ne yaptigi, mulakat sorulari icin)

## Test & QA
- [ ] Audit raporu (kod kalitesi, performans, best practice)
- [ ] Performans testleri + profiler
- [ ] GC allocation kontrolu
- [ ] Build (Android APK)
- [ ] APK test (gercek cihaz)
- [ ] Edge case testleri (board dolu, chain reaction limitleri vs.)
- [ ] Stabilite testi: desync, soft-lock, mis-score kontrolu

## Part 2 - Fake Multiplayer
- [ ] AI rakip hamle sistemi
- [ ] Fake davranis (hover, tereddut, iptal)
- [ ] Turn sistemi + timer
- [ ] Penalty sistemi

## Ekstra
- [ ] Localization destegi (string'lerin ayrilmasi)
- [ ] Save/Load (high score persistent)
- [ ] Pause menusu
- [ ] Settings (ses acma/kapama)
- [ ] Oyun sonu istatistikleri (toplam merge, max chain vs.)
