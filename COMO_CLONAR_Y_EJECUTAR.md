# Cómo clonar el proyecto y ejecutarlo

Este repositorio es un **proyecto Unity 2D**. Los pasos siguientes asumen que trabajas en Windows o macOS con [Unity Hub](https://unity.com/download) instalado.

## Requisitos

- **Git** (por ejemplo [Git for Windows](https://git-scm.com/downloads)).
- **Unity Hub** y un editor compatible con el proyecto. La versión con la que se desarrolla está indicada en `ProjectSettings/ProjectVersion.txt` (actualmente **Unity 6000.3.12f1**). Es recomendable usar esa misma versión o una **6000.3.x** cercana para evitar diferencias al abrir el proyecto.

## 1. Clonar desde GitHub

En una carpeta donde quieras guardar el código, abre una terminal y ejecuta:

```bash
git clone https://github.com/MaxiBernardoni/roguelike.git
cd roguelike
```

Si el remoto de tu fork o copia es otro, sustituye la URL por la que te muestre GitHub (botón **Code** → HTTPS o SSH).

## 2. Abrir el proyecto en Unity

1. Abre **Unity Hub**.
2. **Add** → **Add project from disk** y selecciona la carpeta raíz del clon (la que contiene las carpetas `Assets`, `Packages` y `ProjectSettings`).
3. Elige el editor **6000.3.12f1** (o el que indique `ProjectVersion.txt`) y abre el proyecto.
4. La primera vez, Unity importará paquetes y generará la carpeta `Library` localmente (no se sube al repo; es normal que tarde unos minutos).

## 3. Escena de juego y probar

La forma rápida recomendada:

1. Con el proyecto abierto, en la barra de menú: **Roguelite → Generar escena Arena (MVP)**.
2. Se crea o actualiza la escena en `Assets/Scenes/Arena_MVP.unity`.
3. Abre esa escena (doble clic en el archivo en el **Project**) y pulsa **Play**.

Si esa escena ya existe en tu clon, puedes abrirla directamente y dar a **Play** sin regenerar.

Más detalle sobre capas, jerarquía y componentes: `Assets/SCENE_SETUP.txt`.

## Problemas frecuentes

| Situación | Qué hacer |
|-----------|-----------|
| El menú **Roguelite** no aparece | Espera a que termine la compilación de scripts; revisa la **Console** por errores en C#. |
| Pantalla azul al dar a Play | Abre `Arena_MVP` (u otra escena con **ArenaBootstrap**). Revisa `SCENE_SETUP.txt`. |
| Errores de versión de Unity | Instala la versión del `ProjectVersion.txt` con Unity Hub (**Installs → Install Editor → Archive / 6000.3**). |

## Resumen

1. `git clone` de la URL del repo.  
2. Añadir la carpeta del proyecto en Unity Hub y abrirla.  
3. **Roguelite → Generar escena Arena (MVP)** (si hace falta) → escena **Arena_MVP** → **Play**.
