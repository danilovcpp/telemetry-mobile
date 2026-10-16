# Telemetry Mobile

.NET 10 MAUI приложение для мониторинга инфраструктуры.

Сейчас реализован экран входа. Сервис авторизации и остальные бизнес-экраны ещё не подключены. Демонстрационный менеджер проектов и задач удалён.

Дизайн-система, локализация RU/EN/DE и правила создания новых экранов описаны в [STYLE_GUIDE.md](TelemetryMobile/docs/design/STYLE_GUIDE.md). Результаты проверки — в [VALIDATION.md](TelemetryMobile/docs/design/VALIDATION.md).

```powershell
pwsh -File scripts/Validate-DesignSystem.ps1
dotnet build TelemetryMobile/TelemetryMobile.csproj -f net10.0-android
```

Debug-каталог компонентов запускается только при явном `-p:DesignSystemShowcase=true` и полностью исключён из Release. Для установки Debug APK непосредственно через adb добавьте `-p:EmbedAssembliesIntoApk=true` и нужный `RuntimeIdentifier`; обычный Debug использует Fast Deployment из IDE.
