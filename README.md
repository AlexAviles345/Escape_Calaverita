# Escape Calaverita

Juego 2D inspirado en la mecánica de Flappy Bird, ambientado en Día de Muertos.

## Requisitos

- Unity `6000.6.0f1`.
- Git.
- Git LFS.

## Abrir el proyecto

```bash
git clone <URL_DEL_REPOSITORIO>
cd "escape-calaverita"
git lfs pull
```

Después, abre la carpeta desde Unity Hub. La primera escena del build es
`Assets/Scenes/Menu.unity`.

## Controles

- Espacio, flecha arriba, `W`, clic izquierdo o toque: volar.
- Reintentar: inicia una partida nueva inmediatamente.
- Menú: regresa a la escena principal.

## Archivos versionados

Se deben compartir `Assets/`, `Packages/` y `ProjectSettings/`, incluyendo los
archivos `.meta`. Las carpetas generadas por Unity, como `Library/`, `Temp/`,
`Logs/` y `UserSettings/`, están excluidas mediante `.gitignore`.

Los recursos binarios —imágenes, audio, fuentes, modelos y video— se almacenan
con Git LFS. No elimines ni renombres un archivo `.meta` de forma separada a su
asset correspondiente.

## Flujo recomendado

1. Mantener `main` estable.
2. Crear ramas breves como `feature/nombre-del-cambio` o `fix/nombre-del-error`.
3. Hacer commits pequeños con mensajes descriptivos.
4. Integrar los cambios mediante pull requests cuando participe más de una persona.
