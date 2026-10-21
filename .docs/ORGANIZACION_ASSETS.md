# Organización de Assets de Escape Calaverita

> Estructura vigente desde el 21 de octubre de 2026.

## Objetivo

Todo recurso creado específicamente para el juego vive dentro de `Assets/_Project`. Esto separa el contenido propio de los recursos administrados por Unity y facilita encontrar, revisar y versionar cada sistema.

## Estructura

```text
Assets/
├── _Project/
│   ├── Art/
│   │   ├── Backgrounds/
│   │   ├── Characters/Skins/
│   │   ├── Obstacles/Lapidas/
│   │   └── UI/{Buttons,Frames,Titles}/
│   ├── Audio/{Music,SFX}/
│   ├── Data/Skins/
│   ├── Fonts/PixelifySans/
│   ├── Prefabs/Gameplay/Lapidas/
│   ├── Scenes/Production/
│   └── Scripts/
│       ├── Core/
│       ├── Data/
│       ├── Gameplay/{Obstacles,Player}/
│       └── UI/
├── Settings/
└── TextMesh Pro/
```

## Responsabilidad de cada carpeta

| Carpeta | Contenido |
| --- | --- |
| `Art/Backgrounds` | Fondos completos de escenas. |
| `Art/Characters/Skins` | Sprites seleccionables del jugador. |
| `Art/Obstacles/Lapidas` | Sprites superiores e inferiores de obstáculos. |
| `Art/UI/Buttons` | Imágenes de botones. |
| `Art/UI/Frames` | Marcos y fondos parciales de UI. |
| `Art/UI/Titles` | Títulos gráficos de las pantallas. |
| `Audio/Music` | Música de menú y partida, normalmente en loop. |
| `Audio/SFX` | Efectos cortos, como el impacto al morir. |
| `Data/Skins` | ScriptableObjects de configuración de skins. |
| `Fonts/PixelifySans` | Fuentes originales y su Font Asset de TextMesh Pro. |
| `Prefabs/Gameplay/Lapidas` | Variantes completas utilizadas por el spawner. |
| `Scenes/Production` | Únicamente escenas incluidas en el build. |
| `Scripts/Core` | Coordinación global de una escena o sesión. |
| `Scripts/Data` | Definiciones de datos y ScriptableObjects. |
| `Scripts/Gameplay/Player` | Movimiento, apariencia y comportamiento del jugador. |
| `Scripts/Gameplay/Obstacles` | Movimiento y generación de lápidas. |
| `Scripts/UI` | Menús, selectores y control de paneles. |

`Assets/Settings` y `Assets/TextMesh Pro` permanecen fuera de `_Project` porque son configuraciones o recursos gestionados por Unity.

## Convenciones

- Los scripts usan PascalCase y el archivo debe llamarse igual que su clase.
- Las escenas de producción usan PascalCase: `Menu.unity` y `Game.unity`.
- Los prefabs conservan nombres descriptivos como `Lapida_04_GenericaArriba`.
- Las texturas mantienen nombres en minúsculas con guion bajo para ser consistentes con el arte existente.
- La música y los efectos deben estar separados. Un efecto nuevo nunca se coloca en `Audio/Music`.
- No crear carpetas genéricas como `Misc`, `Otros`, `Material` o `Nueva carpeta`.
- Una carpeta nueva debe representar una categoría que contendrá más de un recurso o un sistema claramente independiente.

## Procedimiento seguro para mover assets

Desde Git, mover siempre el asset y su `.meta` en la misma operación lógica:

```powershell
git mv "Assets/ruta-anterior/recurso.png" "Assets/_Project/Art/categoria/recurso.png"
git mv "Assets/ruta-anterior/recurso.png.meta" "Assets/_Project/Art/categoria/recurso.png.meta"
```

Desde Unity también puede usarse la ventana Project, porque el editor conserva el `.meta`. Nunca copiar únicamente el asset, regenerar su `.meta` o cambiar manualmente un GUID: escenas, prefabs y ScriptableObjects enlazan recursos mediante ese GUID.

Después de mover escenas se deben revisar `File > Build Profiles` y `ProjectSettings/EditorBuildSettings.asset`. Después de cualquier reorganización hay que esperar el reimportado, abrir `Menu` y `Game`, comprobar que no existan referencias `Missing` y ejecutar una partida completa.

## Agregar recursos nuevos

### Skin

1. Importar el PNG en `Art/Characters/Skins`.
2. Configurarlo como Sprite Single, 100 PPU, Point y sin compresión.
3. Agregarlo a `Data/Skins/SkinCatalog.asset`.
4. Probar selector, partida y reintento.

### Variante de lápida

1. Importar ambos sprites en `Art/Obstacles/Lapidas`.
2. Duplicar un prefab de `Prefabs/Gameplay/Lapidas`.
3. Ajustar sprites, posiciones y colliders.
4. Agregar el prefab al arreglo de `LapidaSpawner`.

### Audio

- Música o loops: `Audio/Music`.
- Impactos, botones y efectos breves: `Audio/SFX`.

### Script

Elegir la carpeta por responsabilidad. No colocar scripts nuevos directamente en `Scripts`.

## Limpieza realizada

Durante esta reorganización se eliminaron, tras comprobar sus referencias:

- La escena de recuperación `_Recovery/0.unity`; futuras carpetas `_Recovery` están excluidas mediante `.gitignore`.
- `SampleScene.unity`, que no estaba incluida en el build.
- Los recursos `Welcome` de la plantilla 2D.
- `herencia.jpg`, que no estaba referenciado.
- Los prefabs antiguos `Lapida.prefab` y `Lapida Variant.prefab`, usados únicamente por la escena de recuperación.

La plantilla URP 2D permanece en `Assets/Settings/Scenes/URP2DSceneTemplate.unity`.
