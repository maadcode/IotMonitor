document.addEventListener('DOMContentLoaded', () => {
    const terminalOutput = document.getElementById('terminal-output');
    const cursor = document.querySelector('.cursor');
    
    const messages = [
        { text: "> Cargando configuración del sistema...", delay: 500 },
        { text: "> Conectando con base de datos SQL...", delay: 800 },
        { text: "> [OK] Base de datos conectada.", delay: 300 },
        { text: "> Iniciando orquestador de dispositivos...", delay: 600 },
        { text: "> Cargando módulos de protocolo:", delay: 400 },
        { text: "   - MODBUS TCP [CARGADO]", delay: 200 },
        { text: "   - HTTP/TCP    [CARGADO]", delay: 200 },
        { text: "   - UDP         [CARGADO]", delay: 200 },
        { text: "> Escaneando red local...", delay: 1000 },
        { text: "> Dispositivos detectados:", delay: 500 },
        { text: "   - Cámara Acceso 1 (HTTP)  -> ONLINE", delay: 300 },
        { text: "   - Sensor Presión B (Modbus) -> ONLINE", delay: 300 },
        { text: "   - Panel LED Norte (UDP)     -> READY", delay: 300 },
        { text: "> Dashboard listo. Esperando comandos...", delay: 500 },
        { text: "> _", delay: 1000 }
    ];

    let currentMessageIndex = 0;

    function typeMessage() {
        if (currentMessageIndex < messages.length - 1) {
            const messageObj = messages[currentMessageIndex];
            const line = document.createElement('span');
            line.className = 'line';
            line.textContent = '';
            
            // Insert before the cursor
            terminalOutput.insertBefore(line, cursor);
            
            let charIndex = 0;
            const text = messageObj.text;
            
            const typingEffect = setInterval(() => {
                if (charIndex < text.length) {
                    line.textContent += text[charIndex];
                    charIndex++;
                    // Auto-scroll terminal
                    terminalOutput.scrollTop = terminalOutput.scrollHeight;
                } else {
                    clearInterval(typingEffect);
                    currentMessageIndex++;
                    setTimeout(typeMessage, messageObj.delay);
                }
            }, 30); // Speed of typing
        }
    }

    // Start simulation after a small delay
    setTimeout(typeMessage, 1000);
});
