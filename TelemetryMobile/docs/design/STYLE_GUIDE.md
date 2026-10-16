# Telemetry Mobile — Design System

## Назначение и границы

Premium Dark Monitoring Dashboard: почти чёрный синий фон, спокойные поверхности, тонкие границы, электрический синий акцент и бирюзовый логотип. Основа — предоставленные восемь изображений в `references/`. Карточки не используют тяжёлые тени или blur.

Проект остаётся на .NET 10 MAUI и CommunityToolkit.Mvvm. Шаблонный менеджер проектов/задач, тестовые данные, SQLite и Syncfusion удалены. Реализованы Login, Devices, Device Details и Events. После демо-входа test/test открывается Shell с вкладками устройств и событий. MockTelemetryService содержит 16 устройств и 8 событий; поиск, фильтры, детали и события устройства используют общий набор данных. Настоящий backend авторизации пока не подключён; пароль не сохраняется. Statistics, Map и Profile не входят в текущую навигацию.

## Структура и порядок ресурсов

```text
Resources/Styles/
  Themes/DarkTheme.xaml         # тёмная палитра семантических цветов
  Themes/LightTheme.xaml        # светлая палитра с теми же ключами
  Tokens/Dimensions.xaml        # интервалы, Thickness, размеры, формы
  Tokens/Typography.xaml        # семейство, размеры, межстрочный интервал
  Controls/Labels.xaml
  Controls/Buttons.xaml
  Controls/Inputs.xaml
  Controls/Borders.xaml
  Controls/CollectionItems.xaml
  Styles.xaml                  # единственная точка подключения
UI/Components/                 # ContentView + BindableProperties
Localization/                  # live binding к RESX
Resources/Localization/        # RU (neutral), EN, DE
Pages/Development/             # исключается из Release
```

`App.xaml` подключает только `Styles.xaml`. В нём сначала загружается тема, затем токены, затем стили в порядке зависимостей. Названия ресурсов уникальны. Компоненты не задают собственный BindingContext: внешние bindings продолжают работать, внутренние используют типизированный `x:Reference Root`.

Каждый словарь контролов дополнительно подключает используемые им словари токенов через MergedDictionaries. Это необходимо для разрешения StaticResource во время создания словаря: соседний словарь ещё не гарантированно виден через Application.Resources. Значения токенов определены только в исходных файлах Tokens; повторно объявлять их в Controls не нужно. Скрипт проверки контролирует эту область видимости и циклы зависимостей.

## Цвета

Источник значений — **Themes/DarkTheme.xaml** и **Themes/LightTheme.xaml**. Этот документ намеренно не содержит вторую таблицу HEX. Палитра полностью видна в showcase.

| Назначение | Ключи |
| --- | --- |
| Фон | `ColorPageBackground`, `ColorBackgroundSecondary` |
| Поверхности | `ColorSurfaceCard`, `ColorSurfaceElevated`, `ColorSurfaceInput` |
| Границы | `ColorBorderSubtle` |
| Акценты | `ColorAccentPrimary`, `ColorAccentPressed`, `ColorAccentSecondary` |
| Статусы | `ColorStatusSuccess`, `ColorStatusWarning`, `ColorStatusError`, `ColorStatusOffline` |
| Текст | `ColorTextPrimary`, `ColorTextSecondary`, `ColorTextMuted` |
| Текст на заливках | `ColorTextOnAccent`, `ColorTextOnStatus` |
| Дополнительно | `ColorChartPurple`, `ColorTransparent` |

Цвета в компонентах и страницах подключаются через `DynamicResource`. Цвет статуса сопровождается локализованным текстом. `ThemeManager` заменяет активный словарь палитры и устанавливает `UserAppTheme`; цвета компонентов обновляются автоматически. Переключатель «Светлая тема» доступен на Login и в showcase. Выбор сохраняется в Preferences (`AppTheme`), по умолчанию используется тёмная тема. Обе палитры содержат **полный набор одинаковых ключей**; скрипт проверки контролирует их совпадение. Строка состояния и тонирование иконок используют явные bindings к ThemeManager: у Behavior нет родителя для поиска DynamicResource.

## Типографика

Используется системный шрифт платформы (`FontBody`), поддерживающий кириллицу и латиницу. `FontAutoScalingEnabled=True`; размеры — DIP. Значения шкалы и интервалов находятся в `Tokens/Typography.xaml`.

| Роль | Стиль |
| --- | --- |
| Крупный заголовок | `DisplayLabelStyle` |
| Заголовок страницы | `PageTitleLabelStyle` |
| Заголовок секции | `SectionTitleLabelStyle` |
| Название карточки | `CardTitleLabelStyle` |
| Основной текст | `BodyLabelStyle` (также implicit Label) |
| Дополнительный текст | `BodySecondaryLabelStyle` |
| Подпись | `CaptionLabelStyle` |
| Числовой показатель | `MetricValueLabelStyle` |
| Текст кнопки | `ButtonTextLabelStyle` |
| Статус | `StatusTextLabelStyle` |
| Время | `TimestampLabelStyle` |

Заголовки и метрики выделены Bold. Не фиксируйте высоту текстовых контейнеров, не отключайте перенос длинных немецких строк. Для больших заголовков используется более плотный межстрочный интервал, для обычного текста — свободный.

## Интервалы, формы и размеры

`Space4/8/12/16/20/24/32/40/48` — шкала на основе 4 DIP. Семантические ключи: `PagePadding`, `CardPadding`, `ContentSpacing`, `SectionSpacing`, `InputSpacing`, `InputPadding`, `ButtonPadding`, `BadgePadding`. Используйте их преимущественно вместо чисел.

Уровни скругления: Small, Medium, Large, ExtraLarge, Pill. Числовые ключи — `Radius…`, готовые формы Border — `Shape…`. Для Button предусмотрен `ButtonCornerRadius` правильного типа Int32.

Общие размеры: `ButtonMinHeight`, `InputMinHeight`, `TouchTarget`, `IconSize`, `IconSizeSmall`, `StatusDotSize`, `BorderWidth`. Контентные контейнеры растут вместе со шрифтом; значения высоты кнопок и полей задаются как **минимальные**. `LoginMaxWidth` и `ContentMaxWidth` ограничивают ширину на планшетах/desktop.

## Стили контролов

- Кнопки: `PrimaryButtonStyle`, `SecondaryButtonStyle`, `OutlineButtonStyle`, `TextButtonStyle`, `IconButtonStyle`, `DangerButtonStyle`. Primary применяется implicit. Варианты наследуют `BaseButtonStyle`, имеют Normal, Pressed и Disabled через VisualStateManager. IconButton ожидает текстовый символ; для SVG используйте ImageButton с локализованным описанием и touch target.
- Поле: implicit Entry + `InputBorderStyle`; полноценные focused/error/disabled состояния реализованы в AppTextField.
- Карточки: `StandardCard`, `ElevatedCard`, `InteractiveCard`, `SummaryCard`. Это Border, без тяжёлых теней. Стиль InteractiveCard сам по себе не добавляет действие — команда подключается компонентом или страницей.
- `CardActionButtonStyle` — прозрачная нативная кнопка поверх интерактивной карточки. Pressed добавляет полупрозрачный акцент, сохраняя читаемость содержимого.
- Строки: `CollectionItemStyle` для Grid. Неограниченные списки размещайте в CollectionView, без внешнего ScrollView.

## Компоненты и публичные свойства

| Компонент | Свойства и поведение |
| --- | --- |
| `AppTextField` | `Text` (TwoWay), `Placeholder`, `ErrorText`, `IsPassword`, `IsReadOnly`, `Keyboard`, `ReturnType`, `LeadingIcon`, `TrailingIcon`, `TrailingCommand`, `TrailingDescription`; событие `Completed`, метод `FocusInput()`. Пустой ErrorText убирает ошибку. Приоритет состояния: Disabled → Error → Focused → Normal. |
| `AppButton` | `Text`, `BusyText`, `Command`, `CommandParameter`, `IsBusy`. При загрузке блокирует нажатие и показывает ActivityIndicator; обычные варианты кнопок используйте через стили. |
| `StatusBadge` | `State`, `Text`. Состояния `Online`, `Offline`, `Warning`, `Error`, `Unknown`; текст передаёт вызывающая сторона, компонент ничего не переводит самостоятельно. |
| `DeviceCard` | `Icon`, `Name`, `SecondaryText`, `StatusText`, `State`, `ShowNavigation`, `Command`, `CommandParameter`. При наличии команды используется настоящая Button для клавиатуры и accessibility. |
| `SummaryStatCard` | `Value` (уже отформатированная строка), `Caption`, `State`; Unknown использует основной цвет текста. |
| `MetricRow` | `Icon`, `Name`, `Value`, `Unit`, `Trend` (необязательный текст). Графики не рисует. Для графиков в деталях используется Sparkline (GraphicsView) с mock-выборкой. |
| `EventListItem` | `Title`, `Description`, `Timestamp`, `StatusText`, `State`. Время и форматирование задаёт page model. |
| `SectionHeader` | `Title`, `ActionText`, `ActionCommand`, `ActionParameter`. Без ActionText кнопка скрыта. |

Каждое публичное изменяемое свойство реализовано через BindableProperty. Компоненты содержат только визуальное поведение; преобразование доменной модели в UI-state и команды остаются в PageModels. Строковые значения необязательных свойств по умолчанию пустые.

## Примеры XAML

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:c="clr-namespace:TelemetryMobile.UI.Components"
             xmlns:loc="clr-namespace:TelemetryMobile.Localization">
    <VerticalStackLayout Padding="{StaticResource PagePadding}"
                         Spacing="{StaticResource SectionSpacing}">
        <c:SectionHeader Title="{loc:Translate Metrics}" />
        <c:AppTextField Text="{Binding CustomerId}"
                        Placeholder="{loc:Translate CustomerId}"
                        LeadingIcon="person.png" />
        <c:StatusBadge State="Online" Text="{loc:Translate Online}" />
        <c:AppButton Text="{loc:Translate SignIn}"
                     BusyText="{loc:Translate Loading}"
                     IsBusy="{Binding IsBusy}"
                     Command="{Binding SignInCommand}" />
    </VerticalStackLayout>
</ContentPage>
```

Для списка устройств используйте DataTemplate с **реальным типом вашей модели**, `x:DataType` и следующими bindings:

```xml
<c:DeviceCard Name="{Binding Name}" SecondaryText="{Binding Address}"
              State="{Binding VisualState}" StatusText="{Binding LocalizedStatus}"
              Icon="server.png" ShowNavigation="True"
              Command="{Binding OpenCommand}" CommandParameter="{Binding .}" />
```

Страницы используют DeviceItem и TelemetryDevice, а навигация передаёт стабильный deviceId. Демонстрационные адреса находятся в MockTelemetryService; при подключении backend замените источник данных.

## Локализация

RU — neutral RESX, EN/DE — satellite resources. `loc:Translate` возвращает live Binding к `LocalizationManager.Instance[key]`; смена культуры обновляет уже созданные страницы. `SetCulture("ru" | "en" | "de")` сохраняет язык в Preferences, устанавливает культуру форматирования и уведомляет bindings. Метод вызывается на UI thread. Переключатель на Login доступен до авторизации.

Новые пользовательские строки добавляйте одновременно во все три RESX. Внутри переиспользуемых компонентов строк быть не должно. ViewModel может передать готовую локализованную строку, но тогда он сам должен обновить её при смене языка. Не переводите пользовательские имена устройств. Секреты не сохраняются в Preferences.

## Ассеты и адаптивность

Реальные PNG логотипа и волны перенесены из Assets в `Resources/Images/`, без изменения содержимого. SVG outline-иконки проходят штатный MauiImage pipeline; в XAML на них ссылаются как на `.png`. Оформление SVG и app icon/splash — статическая графика, поэтому её внутренние цвета не переключаются вместе с темой. Цвета обычных контролов всегда берутся из XAML-ресурсов. Монохромные иконки компонентов тонируются через IconTintColorBehavior и ColorTextSecondary, поэтому остаются видимыми в обеих темах.

Не используйте скриншоты референсов как интерфейс, PNG вместо текста, base64 или изображения стандартных полей/кнопок. Для дополнительных устройств добавляйте самостоятельные небольшие SVG. Не добавляйте стороннюю UI-библиотеку ради обычной карточки.

Страницы используют MAUI 10 SafeAreaEdges=All. Login размещён в ScrollView с естественной высотой содержимого: при клавиатуре и большом шрифте форму можно прокрутить. На низком viewport декоративная волна скрывается (порог `LoginDecorationMinHeight`), чтобы не мешать чтению текста. Grid с `*`/`Auto` предпочтителен для адаптивной компоновки; горизонтальные ряды текста не должны требовать фиксированной ширины телефона. Для адаптации Android IME установлен AdjustResize. Кнопкам только с иконкой всегда задавайте SemanticProperties.Description; заголовкам — HeadingLevel. Проверяйте 320 DIP, крупный системный шрифт и немецкий язык.

## Showcase и проверка

Обычный запуск всегда открывает Login. Каталог не зарегистрирован в навигации. Включение только для разработки:

```powershell
dotnet build TelemetryMobile/TelemetryMobile.csproj -f net10.0-android -c Debug -p:DesignSystemShowcase=true
```

При Release файлы `Pages/Development` исключаются из Compile и MauiXaml. Не включайте dev-каталог в production-маршруты. Состав каталога: все цвета, типографика, кнопки, loading/disabled, поля, ошибки, статусы и примеры карточек/метрик/событий. Нажатие интерактивных примеров выводит локализованную подпись.

Перед сдачей изменений: выполните `pwsh -File scripts/Validate-DesignSystem.ps1`, затем сборку Android Debug/Release. При доступном устройстве проверьте Login, клавиатуру, ошибки, RU/EN/DE, крупный шрифт и showcase. Компиляция XAML не заменяет визуальную проверку на устройстве. Результаты текущей проверки записаны отдельно в `VALIDATION.md`.
