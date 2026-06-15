# Diseño Visual de IotMonitor Landing Page

Este documento detalla la identidad visual y las decisiones técnicas para el sitio de presentación de **IotMonitor**.

## 🎨 Identidad Visual
El diseño se basa en una estética **"Dark & Techy"**, inspirada en los centros de control de infraestructura crítica y terminales tácticas.

### Paleta de Colores
*   **Fondo (Base):** `#0d1117` (Oscuro profundo, inspirado en el tema dark de GitHub).
*   **Texto (Principal):** `#c9d1d9` (Gris claro para alta legibilidad).
*   **Acento (Acción/Éxito):** `#7ee787` (Verde terminal "Matrix").
*   **Acento (Información/Highlight):** `#58a6ff` (Azul neón).
*   **Terminal:** `#161b22` (Fondo de ventana de terminal ligeramente más claro que el fondo base).

### Tipografía
*   **Títulos:** Sans-serif moderno (Inter, Roboto o Arial).
*   **Terminal / Código:** Monospace (Consolas, Monaco o 'Courier New').

## 🎬 Animaciones
Para transmitir que el proyecto está "vivo" y activo:
1.  **Terminal Simulator:** Un componente en la sección Hero que simula la secuencia de inicio de IotMonitor, mostrando comandos de diagnóstico y conexión.
2.  **Cursor parpadeante:** Refuerza la estética de consola interactiva.
3.  **Hover effects:** Las tarjetas de beneficios se iluminan sutilmente con un borde de color neón al pasar el mouse.

## 📱 Estructura de la Página
1.  **Hero:** Propuesta de valor clara + Simulador de terminal.
2.  **Problema & Solución:** Explicación simple de por qué centralizar el control de dispositivos es vital.
3.  **Capacidades (Cards):**
    *   **Monitoreo:** Vista instantánea del estado de salud.
    *   **Control Inteligente:** Comandos contextuales para cada dispositivo.
    *   **Extensibilidad:** Listo para nuevos dispositivos.
4.  **Simulación:** Mención especial al entorno Node-RED para pruebas rápidas.
5.  **CTA & Versión:** Enlace al repo y etiqueta de versión `v1.0.0`.

## 🛠️ Stack Técnico
*   **HTML5 / CSS3 (Vanilla):** Sin frameworks externos para garantizar velocidad de carga y simplicidad.
*   **JavaScript (Vanilla):** Para la lógica de la animación de la terminal.
