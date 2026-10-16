# Telemetry Mobile

.NET 10 MAUI приложение для мониторинга инфраструктуры.

Реализованы вход, список устройств, детали устройства и список событий по дизайн-референсам. Демо-учётка: **test / test**. После входа доступны вкладки «Устройства» и «События»; нажатие на устройство открывает его детали, «Все» в деталях — события только этого устройства. Нажатие на событие открывает связанное устройство.

Mock-источник содержит 16 устройств (12 онлайн, 3 оффлайн, 1 с ошибкой) и 8 событий. Доступны поиск по имени/IP, фильтры статуса и важности, пустые состояния и графики mock-метрик. В меню списка устройств можно переключить тему или выйти. Поддерживаются RU/EN/DE. Пароли не сохраняются, backend не вызывается.

Дизайн-система, локализация RU/EN/DE и правила создания новых экранов описаны в [STYLE_GUIDE.md](TelemetryMobile/docs/design/STYLE_GUIDE.md). Результаты проверки — в [VALIDATION.md](TelemetryMobile/docs/design/VALIDATION.md).

```powershell
pwsh -File scripts/Validate-DesignSystem.ps1
dotnet build TelemetryMobile/TelemetryMobile.csproj -f net10.0-android
dotnet run --project tests/TelemetryMobile.SmokeTests
```

Debug-каталог компонентов запускается только при явном `-p:DesignSystemShowcase=true` и полностью исключён из Release. Для установки Debug APK непосредственно через adb добавьте `-p:EmbedAssembliesIntoApk=true` и нужный `RuntimeIdentifier`; обычный Debug использует Fast Deployment из IDE.
